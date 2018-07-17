// 실제적인 게임 실행 스크립트
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Gameover 씬 불러오기 위해 사용
using UnityEngine;
using UnityEngine.UI;   // 점수 계산 사용
using UnityStandardAssets.ImageEffects;    // 블러 스크립트 사용하기 위해서 

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

    public static float fallSpeed=1.0f;
    // 떨어지는 속도 변수

    public int currentLv = 0;
    // 시작 레벨 변수

    private int cntLineClear = 0;
    // 삭제 줄 수 변수

    private int numberOfRowsThisTurn = 0;
    // 몇 줄이 없어졌나 계산하는 변수

    public Text UI_Score;
    public Text UI_Lv;
    public Text UI_Lines;
    public Canvas UI_Canvas;    // 퍼즈 관련
    public Canvas pauseing; // 퍼즈 관련
    // 게임오브젝트 UI 변수 -> 유니티에서 연동가능

    public static int currentScore = 0;
    // 점수계산&저장 변수

    private GameObject previewTetromino;
    private GameObject nextTetromino;
    // 미리보기 변수

    private GameObject holdedTetromino;
    // 홀드 변수

    private bool gameStarted = false;
    // 게임 시작 유무 변수(미리보기에 필요)

    private Vector2 previewTetrominoPosition = new Vector2(-6.5f, 16);
    // 미리보기 위치 변수

    private Vector2 HoldTetrominoPosition = new Vector2(-6.5f, 10);
    // 홀드 위치 변수

    public static bool startLvIsZero;
    public static int startingLv;
    // 시작 레벨, 선택하는 레벨 변수

    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;
    // 시작할 때 불러오는 최고점수 변수들

    public static bool isPause = false;
    // 퍼즈 실행 유무 변수(게임 제어)

    public int maxSwaps = 2; // 홀드 제어 변수
    private int currentSwaps = 0;    // 홀드 횟수 변수
    // 홀드 관련 변수

    private GameObject ghostTetromino;
    // 고스트 기능 변수

    public AudioClip clearLineSound;// 줄 삭제 소리 변수
    public AudioClip hold_pause_Sound; // 홀드+퍼즈 소리 변수
    private AudioSource audioSource;    // 소리 변수
    // 소리 변수들

    void Start() // 게임 시작 시 가장 먼저 실행
    {
        isPause = false;
        currentScore = 0;  // 점수 초기화
        UI_Score.text = "0";    // UI로 출력

        currentLv = startingLv; // 레벨 설정
        UI_Lv.text = currentLv.ToString();  // UI로 출력

        UI_Lines.text = "0";    // UI로 출력
       
        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3"); 
        // 게임 시작 시 저장된 최고점수 불러옴
        
        SpawnNextTetromino();   // 랜덤으로 블록 자동 생성
        audioSource = GetComponent<AudioSource>();    // 줄 삭제 소리 불러옴
    }   // 함수 끝

    void Update() // 점수 계산을 위해 업데이트 함수 재생성    -> 프레임당 점수를 불러와야 함
    {
        if(!isPause)
        {
            UpdateScore();  // 점수 계산
            UpdateUI();     // UI 출력 업데이트
            UpdateLevel();  // 레벨 계산
            UpdateSpeed();  // 속도 증가 계산
        }
        CheckUserInput();   // 퍼즈 기능 추가
    }   // 함수 끝

     void CheckUserInput()   // 퍼즈 입력 함수
     {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
            if (Time.timeScale == 1)   // 게임이 실행중일때 퍼즈
            {
                PauseGame();
                audioSource.Pause();
            }
            else if(Time.timeScale==0) // 한번 더 누를 경우 퍼즈 해제
            {
                ResumeGame();
                audioSource.UnPause();
            } 
            PlayHoldPauseAudio();   // 소리실행
        }

         if(Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            HoldTetromino(tempNextTetromino.transform);
            PlayHoldPauseAudio();   // 소리 실행
        }
     }   // 함수 끝

     void PauseGame()    // 게임 퍼즈 함수
     {
        Time.timeScale = 0;
        isPause = true; // 퍼즈 상태
        UI_Canvas.enabled = false;  // 기존 화면 UI 출력하지 않음
        pauseing.enabled = true;    // 퍼즈 출력
        Camera.main.GetComponent<Blur>().enabled = true;    // 퍼즈상태일 때 메인 카메라에 블러 설정
        // audioSource.Pause();
     }   // 함수 끝

     public void ResumeGame()   // 게임 퍼즈 해제 함수
     {
         Time.timeScale = 1;
         isPause = false;
         UI_Canvas.enabled = true; // 기존 화면UI 출력 
         pauseing.enabled = false;    // 퍼즈 출력하지 않음
         Camera.main.GetComponent<Blur>().enabled = false;    // 퍼즈상태일 때 메인 카메라에 블러 설정
         //audioSource.UnPause(); // 오디오 관련
     }   // 함수 끝

    void PlayHoldPauseAudio() // 홀드 입력시 소리
    {
        audioSource.PlayOneShot(hold_pause_Sound);
    }   // 함수 끝

   /* public void PlayMusicMute()    // 음소거
    {
        if (audioSource.mute != true)
        {
            audioSource.mute = true;
        }
        else
        {
            audioSource.mute = false;
        }  
    }
    */

    void UpdateLevel()  // 레벨 함수
    {
        if (startLvIsZero == true || startLvIsZero==false && cntLineClear/10 > startingLv)
            currentLv = cntLineClear / 10; // 10줄 삭제 후, 1이 될 경우 -> 레벨업
        // Debug.Log("currentLv : " + currentLv);    // 확인용
    }   // 함수 끝

    void UpdateSpeed()  // 레벨에 따른 속도
    {
        fallSpeed = 1.0f - ((float)currentLv * 0.05f);  // 20Lv 까지 하기 위해서 수정함(0.1f -> 0.05f)
    }   // 함수 끝

    public void UpdateUI()  // UI 업데이트 함수
    {
        UI_Score.text = currentScore.ToString();   // 점수
        UI_Lv.text = currentLv.ToString();  // 레벨
        UI_Lines.text = cntLineClear.ToString();    // 라인 수
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
            PlayLineClearSound();
        }
    }   // 함수 끝

    public void PlayLineClearSound()    // 줄 삭제 시 소리 함수
    {
        audioSource.PlayOneShot(clearLineSound);
    }

    public void ClearedOneLine()    // 한 줄 삭제 점수 함수
    {
        currentScore += scoreOneLine + (currentLv * 10);   // 점수 수정
        cntLineClear++;
    }   // 함수 끝

    public void ClearedTwoLine()    // 두 줄 삭제 점수 함수
    {
        currentScore += scoreTwoLine + (currentLv * 20);
        cntLineClear += 2;
    }   // 함수 끝

    public void ClearedThreeLine()    // 세 줄 삭제 점수 함수
    {
        currentScore += scoreThreeLine + (currentLv * 30);
        cntLineClear += 3;
    }   // 함수 끝

    public void ClearedFourLine()    // 네 줄 삭제 점수 함수(테트리스)
    {
        currentScore += scoreFourLine + (currentLv * 40);
        cntLineClear += 4;
    }   // 함수 끝

    public void UpdateHighScore()   // 최고 점수 불러오는 함수
    {
        if (currentScore > startingHighScore)   // 1등보다 클 경우
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore", currentScore);
        }
        else if(currentScore>startingHighScore2)    // 2등보다 클 경우
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startingHighScore3)  // 3등보다 클 경우
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }       
    }   // 함수 끝
    
    bool CheckIsValidPosition(GameObject tetromino)    // 홀드 함수
    {
        foreach(Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (!CheckIsInsideGrid(pos))
               return false;

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
               return false;
        }
        return true;
    }

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
        // 추가할 것! ->  프리뷰 3개 생성
        if(!gameStarted)    // 첫 시작
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            // 랜덤으로 다음 블록 생성, 위치정보, 회전정보
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            // 미리 블록 생성
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            // 미리 생성한 블록 제어x

            nextTetromino.tag = "currentActiveTetromino";   // 태그 설정
            SpawnGhostTetromino();  // 고스트 생성
        }
        else  // 첫 시작 이후
        {
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);    // 미리보기 블록 위치 변경
            nextTetromino = previewTetromino;   // 다음 블록으로 설정
            nextTetromino.GetComponent<Tetromino>().enabled = true; // 제어 가능
            nextTetromino.tag = "currentActiveTetromino";   // 태그 설정
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            // 미리 블록 생성
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            // 미리 생성한 블록 제어x
            SpawnGhostTetromino();  // 고스트 생성
        }
        currentSwaps = 0;   // 교체횟수 리셋
    }   // 함수 끝

    public void SpawnGhostTetromino()   // 고스트 생성 함수
    {
        if (GameObject.FindGameObjectsWithTag("currentGhostTetromino") != null)
            Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));

        ghostTetromino=(GameObject)Instantiate(nextTetromino,nextTetromino.transform.position,Quaternion.identity);

        Destroy(ghostTetromino.GetComponent<Tetromino>());
        ghostTetromino.AddComponent<GhostTetromino>();
    }   // 함수 끝

    public void HoldTetromino(Transform t)  // 블록 저장
    {
        // 추가? -> 홀드 증가?(적용하면 ls : 저장, rs : 사용 이런식일듯)
        currentSwaps++;

        if (currentSwaps > maxSwaps)    // 최대 교체횟수 초과시 실행x
            return;
        if (holdedTetromino != null) // 홀드 존재o시 교체
        {
            GameObject tempHoldTetromino = GameObject.FindGameObjectWithTag("currentHoldTetromino");
            tempHoldTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);  // 중간 위치에서 저장함

            if (!CheckIsValidPosition(tempHoldTetromino))
            {
                tempHoldTetromino.transform.localPosition = HoldTetrominoPosition;
                return;
            }
            holdedTetromino = (GameObject)Instantiate(t.gameObject);
            holdedTetromino.GetComponent<Tetromino>().enabled = false;
            // holdedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            holdedTetromino.transform.localPosition = HoldTetrominoPosition;
            holdedTetromino.tag = "currentHoldTetromino";

            nextTetromino = (GameObject)Instantiate(tempHoldTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempHoldTetromino);
            DestroyImmediate(ghostTetromino);
            FindObjectOfType<Game>().SpawnGhostTetromino();
        }
        else // 홀드 존재x시 저장
        {
            holdedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            holdedTetromino.GetComponent<Tetromino>().enabled = false;
            holdedTetromino.transform.localPosition = HoldTetrominoPosition;
            holdedTetromino.tag = "currentHoldTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            SpawnNextTetromino();
        }
        return;
    }   // 함수 끝

    public bool CheckIsInsideGrid(Vector2 pos)    // 창 안에 있는지 유무
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
        GameoverSys.playerScore = currentScore; // 최종 점수 출력
        UpdateHighScore();  // 게임종료시 최고점수 갱신
        SceneManager.LoadScene("GameOver");
    }   // 함수 끝
}
