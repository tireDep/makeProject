// 실제적인 게임 실행 클래스, grid에 적용됨
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Gameover 씬 불러오기 위해 사용
using UnityEngine;
using UnityEngine.UI;   // 점수 계산 사용

public class Game : MonoBehaviour {

    public static int gridWidth = 10;
    public static int gridHeight = 20;
    // 테트리스 게임 화면 크기(블록 공간)

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    // 각각의 블록이 차지하는 공간 변수

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200; // Tetris!
    // 점수 변수

    private int numberOfRowsThisTurn = 0;
    // 몇 줄이 없어졌나 계산하는 변수

    public Text UI_Score;
    // 게임오브젝트 UI 변수 -> 유니티에서 연동가능

   public static int current_score = 0;
    // 점수계산&저장 변수

    void Start() // 게임 시작 시 가장 먼저 실행
    {
        SpawnNextTetromino();   // 랜덤으로 블록 자동 생성
        current_score = 0;  // 점수 초기화
    }   // 함수 끝

    void Update() // 점수 계산을 위해 업데이트 함수 재생성    -> 프레임당 점수를 불러와야 함
    {
        UpdateScore();  // 점수 계산
        UpdateUI();     // UI 출력 업데이트
    }

    public void UpdateUI()  // 점수 UI 업데이트 함수
    {
        UI_Score.text = current_score.ToString();
    }   // 함수 끝 

    public void UpdateScore()   // 점수 계산
    {
        if(numberOfRowsThisTurn>0)
        {
            if(numberOfRowsThisTurn==1)
                ClearedOneLine();  
            else if(numberOfRowsThisTurn == 2)
                ClearedTwoLine();
            else if (numberOfRowsThisTurn == 3)
                ClearedThreeLine();
            else if (numberOfRowsThisTurn == 4)
                ClearedFourLine();
            numberOfRowsThisTurn = 0;   // 점수 계산 후, 삭제 줄 수 초기화
        }
    }   // 함수 끝

    public void ClearedOneLine()    // 한 줄 삭제 점수 함수
    {
        current_score += scoreOneLine;
    }   // 함수 끝
    public void ClearedTwoLine()    // 두 줄 삭제 점수 함수
    {
        current_score += scoreTwoLine;
    }   // 함수 끝
    public void ClearedThreeLine()    // 세 줄 삭제 점수 함수
    {
        current_score += scoreThreeLine;
    }   // 함수 끝
    public void ClearedFourLine()    // 네 줄 삭제 점수 함수(테트리스)
    {
        current_score += scoreFourLine;
    }   // 함수 끝

    public bool CheckIsAboveGrid(Tetromino tetromino)   // 블록이 맨 위에 닿았는지 검사
    {
        for(int location_x=0; location_x < gridWidth; ++location_x)
        {
            foreach(Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);

                if(pos.y>gridHeight-1)
                {
                    return true;
                }
            }
        }
        return false;
    }   // 함수 끝

    public bool IsFullRowAt(int location_y) // 행이 다 차있는지 검사하는 함수
    {
        for(int location_x = 0; location_x < gridWidth; ++location_x)
        {
            if (grid[location_x, location_y] == null)
                return false;
        }
        numberOfRowsThisTurn++; // 가득 찬 줄을 발견했을 경우, 증가 -> 점수계산에 사용함
        return true;
    }   // 함수 끝

    public void DeleteMinoAt(int location_y) // 블록 제거
    {
        for(int location_x = 0; location_x < gridWidth; ++location_x)
        {
            Destroy(grid[location_x, location_y].gameObject);

            grid[location_x, location_y] = null;
        }
    }   // 함수 끝

    public void MoveRowDown(int location_y)  // 행 내리기
    {
        for(int location_x = 0; location_x < gridWidth;++location_x)
        {
            if(grid[location_x, location_y] !=null)
            {
                grid[location_x, location_y - 1] = grid[location_x, location_y];
                grid[location_x, location_y] = null;
                grid[location_x, location_y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }   // 함수 끝

    public void MoveAllRowsDown(int location_y)  // 전체 행 내리기
    {
        for (int i = location_y; i < gridHeight; ++i)
            MoveRowDown(i);
    }   // 함수 끝

    public void DeleteRow() // 행 삭제
    {
        for(int location_y = 0; location_y < gridHeight; ++location_y)
        {
            if(IsFullRowAt(location_y))  // 행이 다 차있을 경우 
            {
                DeleteMinoAt(location_y);    // 블록제거
                MoveAllRowsDown(location_y + 1); // 행 내리기
                --location_y;
            }
        }
    }   // 함수 끝

    public void UpdateGrid(Tetromino tetromino) // 전체 공간 계산(남은공간)
    {
        for(int location_y = 0; location_y < gridHeight;++location_y)
        {
            for(int location_x = 0; location_x < gridWidth; ++location_x)
            {
                if(grid[location_x, location_y] !=null)
                {
                    if (grid[location_x, location_y].parent == tetromino.transform)
                    {
                        grid[location_x, location_y] = null;
                    }
                }
            }
        }

        foreach(Transform mino in tetromino.transform)  // 블록이 차지하고 있는 공간 계산
        {
            Vector2 pos = Round(mino.position);

            if(pos.y<gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }   // 함수 끝

    public Transform GetTransformAtGridPosition(Vector2 pos)    // 블록이 차지하고 있는 공간 계산
    {
        if (pos.y > gridHeight - 1)
            return null;
        else
            return grid[(int)pos.x, (int)pos.y];
    }   // 함수 끝

    public void SpawnNextTetromino()    // 다음 블록 생성
    {
        GameObject nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
        // 랜덤으로 다음 블록 생성, 위치정보, 회전정보
        // 선언되면서 사용이 되는데 직접 쓰이는게 아니라서 경고메시지 뜸..
        // Assets/Scripts/Game.cs(188,20): warning CS0219: The variable `nextTetromino' is assigned but its value is never used
    }   // 함수 끝

    public bool CheckInsideGrid(Vector2 pos)    // 창 안에 있는지 유무
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
        // 가로가 0보다 크거나 최대크기보다 작음 & 세로가 최소크기보다 크거나 같음
    }   // 함수 끝

    public Vector2 Round(Vector2 pos)   // 위치값 반올림
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        // 해당 위치 값 반올림
    }   // 함수 끝

    string GetRandomTetromino() // 랜덤 블록 생성
    {
        int randomTetromino = Random.Range(1, 8);   // 랜덤 지정
        string randomTetrominoName = "Prefabs/TBlock";  // 기본 지정 블록
        // 프리팹 내부에 블록이 존재하기 때문에 Prefabs/추가

        switch(randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/TBlock";
                break;
            case 2:
                randomTetrominoName = "Prefabs/IBlock";
                break;
            case 3:
                randomTetrominoName = "Prefabs/OBlock";
                break;
            case 4:
                randomTetrominoName = "Prefabs/LBlock";
                break;
            case 5:
                randomTetrominoName = "Prefabs/L2Block";
                break;
            case 6:
                randomTetrominoName = "Prefabs/SBlock";
                break;
            case 7:
                randomTetrominoName = "Prefabs/S2Block";
                break;
        }
        return randomTetrominoName;
    }   // 함수 끝

    public void GameOver()  // 게임오버
    {
        // Application.LoadLevel("GameOver");
        SceneManager.LoadScene("GameOver");
    }   // 함수 끝
}
