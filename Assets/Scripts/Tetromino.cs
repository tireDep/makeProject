// 블록관련 클래스, 각각의 블록에 적용됨
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {
    float fall = 0; // 낙하 속도 관련 변수
    public float fallSpeed = 1; // 떨어지는 속도 변수, 유니티에서 직접 수정 가능

    public bool allowRoatation = true;
    public bool limitRoatiotion = false;
    // 회전 관련 변수, 유니티에서 직접 수정 가능

    public int individualScore = 100;
    // 놓는 속도에 따라서 달라지는 점수 변수 

    private float individualScoreTime;
    // 놓는 속도 계산 변수

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {    // 프레임 당 실행
        CheckUserInput();
        UpdateIndividualScore();
	}   // 함수 끝

    void UpdateIndividualScore()    // 떨어지는 속도에 따른 점수 계산 함수
    {
        if(individualScoreTime<1)   // 시간이 1초보다 작을 때
        {
            individualScoreTime += Time.deltaTime;  // 플레이어의 시간을 더해줌
        }
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10,0);
        }
    }

    void CheckUserInput()   // 게임 입력 함수
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))    // 오른쪽 화살표 입력
        {
            transform.position += new Vector3(1, 0, 0);

            if(CheckIsValidPosition()) // 존재하고 있는지 확인
                FindObjectOfType<Game>().UpdateGrid(this); // 공간 계산
            else
                transform.position += new Vector3(-1, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) // 왼쪽 화살표 입력
        {
            transform.position += new Vector3(-1, 0, 0);

            if (CheckIsValidPosition())  // 존재하고 있는지 확인 
                FindObjectOfType<Game>().UpdateGrid(this); // 공간계산
            else
                transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) // 위쪽 화살표 입력
        {
            if (allowRoatation) // 회전 가능
            {
                if (limitRoatiotion)    // 제한된 회전 -> S블록 등은 회전 제한
                {
                    if(transform.rotation.eulerAngles.z>=90)    // z축 회전 각이 90이상
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
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time-fall>=fallSpeed) // 아래쪽 화살표 입력 or 자동으로 한칸씩 떨어짐
        {
            transform.position += new Vector3(0, -1, 0);    // 아래로 한 칸 이동

            if (CheckIsValidPosition()) // 존재하고 있는지 확인 
                FindObjectOfType<Game>().UpdateGrid(this);  // 공간계산
            else
            {
                transform.position += new Vector3(0, 1, 0);

                FindObjectOfType<Game>().DeleteRow();   // 행이 다 차있을 경우 행 삭제 실행

                if(FindObjectOfType<Game>().CheckIsAboveGrid(this)) // 블록이 마지막에 도달했는지 검사
                {
                    FindObjectOfType<Game>().GameOver();
                }

                FindObjectOfType<Game>().SpawnNextTetromino();  // 다음 블록 자동 생성
                Game.current_score += individualScore;  // 놓는 속도에 따라 점수 계산 실행

                enabled = false;    // 실행 유무..?
            }
            fall = Time.time;   // 떨어지는 속도 변경
            // 시간 - 지난 시간 >= 떨어지는 속도
        }
        // 스페이스바 -> 한번에 떨어지는 것 추가하기
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
