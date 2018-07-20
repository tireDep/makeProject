// 게임 메뉴 시스템 관련 스크립트
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Level 씬 불러오기 위해 사용
using UnityEngine.UI;   // 슬라이더 값 불러오기 위해 사용
using UnityEngine;

public class MenuSystem : MonoBehaviour {

    public Text Lvtext;
    // 설정한 레벨을 불러오기 위한 변수

    public Text Highscoretext;
    public Text Highscoretext2;
    public Text Highscoretext3;
    // 최고점수 변수

    void Start()
    {
        Lvtext.text = "0";  // 시작할 때 0으로 표시
        Game.startingLv = 0;    // 시작할 때 0Lv으로 설정함

        Highscoretext.text = PlayerPrefs.GetInt("highscore").ToString();
        Highscoretext2.text = PlayerPrefs.GetInt("highscore2").ToString();
        Highscoretext3.text = PlayerPrefs.GetInt("highscore3").ToString();
    }   // 함수 끝

    public void ResetHs()
    {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.SetInt("highscore2", 0);
        PlayerPrefs.SetInt("highscore3", 0);
        // 점수 초기화
        Start();
        // 초기화 하고 다시 로딩
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

    public void ChangeLv(float value)   // 슬라이더로 시작 레벨 설정하는 함수
    {
        Game.startingLv = (int)value;   // 슬라이더 값으로 시작레벨 설정
        Lvtext.text = value.ToString();
    }   // 함수 끝
}