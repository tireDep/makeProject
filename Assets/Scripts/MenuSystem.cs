using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Level 씬 불러오기 위해 사용
using UnityEngine;

public class MenuSystem : MonoBehaviour {
    // 환경 설정이기 때문에 start(), update() 필요 없으므로 삭제함

    public void PlayStart()
    {
        SceneManager.LoadScene("Level");    // 게임 시작
    }

    public void fin()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // play모드를 false로
#elif UNITY_WEBPLAYER
    Application.OpenURL("http://google.com");   // 구글 웹으로 전화
#else
        Application.Quit();
#endif

        Application.Quit(); // 일반 응용프로그램, 모바일 처리 가능 but 웹이나 유니티 에디터 동작 x
    }
}