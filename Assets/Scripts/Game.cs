using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static int gridWidth = 10;
    public static int gridHeight = 20;  
    // 테트리스 게임 화면 크기

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool CheckInsideGrid(Vector2 pos)    // 창 안에 있는지 유무
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
        // 가로가 0보다 크거나 최대크기보다 작음 & 세로가 최소크기보다 크거나 같음
    }

    public Vector2 Round(Vector2 pos)   // 위치값 올림
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        // 해당 위치 값 올림
    }
}
