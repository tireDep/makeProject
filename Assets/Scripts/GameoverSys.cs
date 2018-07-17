// 게임종료 화면 스크립트
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Level 씬 불러오기 위해 사용
using UnityEngine.UI;
using UnityEngine;

public class GameoverSys : MonoBehaviour {
    public Text UI_Score;
    public static int playerScore;
    // 게임 종료 시 점수 출력

    void Start()
    {
        UI_Score.text = playerScore.ToString(); // 점수 출력
    }

    public void GotoMenu() // 게임시작화면 불러오는 함수
    {
        SceneManager.LoadScene("Gamestart");
    }   // 함수 끝

    public void PlayStart() // 게임플레이화면 불러오는 함수
    {
        if (Game.startingLv == 0)
            Game.startLvIsZero = true;
        else
            Game.startLvIsZero = false;
        // 시작 레벨 설정

        SceneManager.LoadScene("Level");    // 게임 시작
    }   //  함수 끝

    public void Fin()   // 게임종료 함수
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // play모드를 false로
#elif UNITY_WEBPLAYER
    Application.OpenURL("http://google.com");   // 구글 웹으로 전화
#else
        Application.Quit(); // 일반 응용프로그램, 모바일 처리 가능 but 웹이나 유니티 에디터 동작 x
#endif
    }   // 함수 끝
}
