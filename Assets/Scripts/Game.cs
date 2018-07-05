using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static int gridWidth = 10;
    public static int gridHeight = 20;  
    // 테트리스 게임 화면 크기

	// Use this for initialization
	void Start () {
        SpawnNextTetromino();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnNextTetromino()    // 다음 블록 생성
    {
        GameObject nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
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

    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);   // 랜덤 지정
        string randomTetrominoName = "Prefabs/TBlock";  // 기본 지정 블록

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
    }
}
