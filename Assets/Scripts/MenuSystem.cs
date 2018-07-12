// 게임 메뉴 시스템 관련
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Level 씬 불러오기 위해 사용
using UnityEngine.UI;   // 슬라이더 값 불러오기 위해 사용
using UnityEngine;

public class MenuSystem : MonoBehaviour {

    public Text Lvtext; // 설정한 레벨을 불러오기 위한 변수

    void start()
    {
        Lvtext.text = "0";  // 시작할 때 0으로 표시
    }

    public void PlayStart()
    {
        if (Game.startingLv == 0)
            Game.startLvIsZero = true;
        else
            Game.startLvIsZero = false;
        // 시작 레벨 설정

        SceneManager.LoadScene("Level");    // 게임 시작
    }   //  함수 끝

    public void fin()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // play모드를 false로
#elif UNITY_WEBPLAYER
    Application.OpenURL("http://google.com");   // 구글 웹으로 전화
#else
        Application.Quit(); // 일반 응용프로그램, 모바일 처리 가능 but 웹이나 유니티 에디터 동작 x
#endif
    }   // 함수 끝

    public void changeLv(float value)   // 슬라이더로 시작 레벨 설정하는 함수
    {
        Game.startingLv = (int)value;   // 슬라이더 값으로 시작레벨 설정
        Lvtext.text = value.ToString();
    }
}