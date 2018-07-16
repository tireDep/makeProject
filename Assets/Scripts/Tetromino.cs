// 블록관련 클래스, 각각의 블록에 적용됨
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // 씬 불러오기
using UnityEngine;

public class Tetromino : MonoBehaviour {
    float fall = 0; // 낙하 속도 관련 변수
    private float fallSpeed = 1; // 떨어지는 속도 변수, 난이도를 위해서 private으로 변경

    public bool allowRoatation = true;
    public bool limitRoatiotion = false;
    // 회전 관련 변수, 유니티에서 직접 수정 가능

    // 음악관련변수들추가 * 3

    private float continuousVerticalSpeed = 0.05f;  // 아래 화살표 누를 때의 속도
    private float continuousHorizontalSpeed = 0.1f; // 좌우 화살표 누를 때의 속도
    private float buttonDownWaitMax = 0.2f; // 버튼을 누르고 있는 것을 인식하기까지 기다리는 속도 -> 계속 누르고 있을 때
    // 움직이는 속도 변수들

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;
    // 타이머 변수들

    private bool moveImmediateHorizontal = false;
    private bool moveImmediateVertical = false;
    // 한 번 or 계속 누르고 있는지 판별 변수

    void Start ()
    {
        Time.timeScale = 1; // 이거 안하면 재시작때 렉걸림
    }   // 함수 끝

void Update () {    // 프레임 당 실행
        if (!Game.isPause)   // 퍼즈 상태가 아님
        {
            CheckUserInput();   //  유저 입력
            UpdateFallSpeed();  // 선택한 레벨에 따라 속도가 조절됨
        }
    }   // 함수 끝

    void UpdateFallSpeed()
    {
        fallSpeed = Game.fallSpeed;
    }   // 함수 끝

    void CheckUserInput()   // 게임 입력 함수
    {
        if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))    // 키를 놓았을 때, 타이머 변수 초기화
        {
            moveImmediateHorizontal = false;
            moveImmediateVertical = false;

            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }

        if(Input.GetKeyUp(KeyCode.DownArrow))   
        {
            moveImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        // else if -> if로 수정 => if문이어야지만 좌우로 계속 누를 때 고정되지 않음(else if의 경우 안됨)
        if(Input.GetKey(KeyCode.RightArrow))    // 오른쪽 화살표 입력
        {
            MoveRight();
        }   // 오른쪽 끝

        if(Input.GetKey(KeyCode.LeftArrow)) // 왼쪽 화살표 입력
        {
            MoveLeft();
        }   // 왼쪽 끝

        if (Input.GetKeyDown(KeyCode.UpArrow)) // 위쪽 화살표 입력
        {
            MoveRotate();
        } // 위쪽 끝

         if (Input.GetKey(KeyCode.DownArrow) || Time.time-fall>=fallSpeed) // 아래쪽 화살표 입력 or 자동으로 한칸씩 떨어짐
        {
            MoveDown();
        } // 아래쪽 끝

         if(Input.GetKeyUp(KeyCode.Space))  // 스페이스바 입력 즉시하강
        {
            SlamDown();
        }   // 즉시 하강
    }   // 함수 끝

    public void SlamDown() // 즉시 하강
    {
        while(CheckIsValidPosition())
            transform.position += new Vector3(0, -1, 0);

        if(!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().UpdateGrid(this);  // 공간계산
            // 공간계산이 없을 경우, 즉시 하강을 사용한 자리에 투명 벽 생겨서 바로 게임오버됨

            FindObjectOfType<Game>().DeleteRow();   // 행이 다 차있을 경우 행 삭제 실행

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this)) // 블록이 마지막에 도달했는지 검사
            {
                FindObjectOfType<Game>().GameOver();
            }

            // 나중에 음악 추가시 getkey로 변경!

            enabled = false;    // 움직일 수 없게 하는 것!(바닥에 착지)
            this.tag = "Untagged";  // 태그 추가
            FindObjectOfType<Game>().SpawnNextTetromino();  // 다음 블록 자동 생성
        }
    }

    // 키를 계속 누르고 있을 때, 블록이 아래로 내려가지 않는 문제 수정
    void MoveLeft() // 왼쪽 움직임
    {
        if (moveImmediateHorizontal) // 계속 누르고 있을 때
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)    // 딜레이 시간
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed) // 움직인 시간(누른 시간)에 따라서 속력이 결정됨 
            {
                horizontalTimer += Time.deltaTime;
                return; // 끝남을 의미함
            }
        }
        if (!moveImmediateHorizontal)   // 한번만 누를 경우
            moveImmediateHorizontal = true;

        horizontalTimer = 0;    // 초기화

        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition())  // 존재하고 있는지 확인 
            FindObjectOfType<Game>().UpdateGrid(this); // 공간계산
        else
            transform.position += new Vector3(1, 0, 0);
    }   // 함수 끝

    void MoveRight()    // 오른쪽 움직임
    {
        if (moveImmediateHorizontal)    // 계속 누르고 있을 때
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)   // 딜레이 시간
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed) // 움직인 시간(누른 시간)에 따라서 속력이 결정됨 
            {
                horizontalTimer += Time.deltaTime;
                return; // 끝남을 의미함
            }
        }
        if (!moveImmediateHorizontal)   // 한번만 누를 경우
            moveImmediateHorizontal = true;

        horizontalTimer = 0;    // 초기화

        transform.position += new Vector3(1, 0, 0);

        if (CheckIsValidPosition()) // 존재하고 있는지 확인
            FindObjectOfType<Game>().UpdateGrid(this); // 공간 계산
        else
            transform.position += new Vector3(-1, 0, 0);
    }   // 함수 끝

    void MoveDown() // 아래로 움직임
    {       
        if (moveImmediateVertical)   // 계속 누르고 있을 경우
        {
            if (buttonDownWaitTimerVertical < buttonDownWaitMax)    // 딜레이 시간
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVerticalSpeed)   // 움직인 시간(누른 시간)에 따라서 속력이 결정됨
            {
                verticalTimer += Time.deltaTime;
                return; // 끝남을 의미
            }
        }
        if (!moveImmediateVertical) // 한번만 누를 경우
            moveImmediateVertical = true;

        verticalTimer = 0;  // 초기화

        transform.position += new Vector3(0, -1, 0);    // 아래로 한 칸 이동

        if (CheckIsValidPosition()) // 존재하고 있는지 확인 
            FindObjectOfType<Game>().UpdateGrid(this);  // 공간계산
        else
        {
            transform.position += new Vector3(0, 1, 0);
            // 나중에 음악, 다른 기능 추가시 slam도 업데이트해야함(이 아래로)!
            FindObjectOfType<Game>().DeleteRow();   // 행이 다 차있을 경우 행 삭제 실행

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this)) // 블록이 마지막에 도달했는지 검사
            {
                FindObjectOfType<Game>().GameOver();
            }

            // 나중에 음악 추가시 getkey로 변경
            

            enabled = false;    // 움직일 수 없게 하는 것!(바닥에 착지)
            this.tag = "Untagged";   // 태그 추가
            FindObjectOfType<Game>().SpawnNextTetromino();  // 다음 블록 자동 생성
        }
        fall = Time.time;   // 떨어지는 속도 변경
                            // 시간 - 지난 시간 >= 떨어지는 속도
    }   // 함수 끝

    void MoveRotate()   // 회전(위방향키)
    {
        if (allowRoatation) // 회전 가능
        {
            if (limitRoatiotion)    // 제한된 회전 -> S블록 등은 회전 제한
            {
                if (transform.rotation.eulerAngles.z >= 90)    // z축 회전 각이 90이상
                    transform.Rotate(0, 0, -90);
                else
                    transform.Rotate(0, 0, 90);
            }
            else
                transform.Rotate(0, 0, 90); // 90도 회전

            if (CheckIsValidPosition()) // 존재하고 있는지 확인 
                FindObjectOfType<Game>().UpdateGrid(this); // 공간계산
            else
            {
                if (limitRoatiotion)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                        transform.Rotate(0, 0, -90);
                    else
                        transform.Rotate(0, 0, 90);
                }
                else
                    transform.Rotate(0, 0, -90);
            }
        }
    }   // 함수 끝

    bool CheckIsValidPosition() // 존재 유무, 올바른 위치인지 판별
    {
        foreach(Transform Block in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(Block.position);

            if (FindObjectOfType<Game>().CheckInsideGrid(pos) == false)
                return false;

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
                return false;
        }
        return true;
    }   // 함수 끝
}
