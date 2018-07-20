// 고스트 기능 스크립트
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetromino : MonoBehaviour {

	void Start ()
    {
        tag = "currentGhostTetromino";  // 태그 설정

        foreach(Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);  // 색 변화 없음, 알파 컴포넌트(투명도)
        }
	}   // 함수 끝
	
	void Update ()
    {
        FollowActiveTetromino();
        MoveDown();
	}   // 함수 끝

    void FollowActiveTetromino()    // 고스트 생성 함수
    {
        Transform currentActiveTetrominoTransform = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;
        transform.position = currentActiveTetrominoTransform.position;
        transform.rotation = currentActiveTetrominoTransform.rotation;
    }   // 함수 끝

    void MoveDown() // 고스트 하강 함수
    {
        while(CheckIsValidPosition())   // 올바른 위치일 경우
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if(!CheckIsValidPosition())
            transform.position += new Vector3(0, 1, 0);
    }   // 함수 끝

    bool CheckIsValidPosition() // 고스트 위치확인 함수
    {
        foreach(Transform mino in transform)
        {
           Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
           if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)    // 범위 안에 존재 하지 않을 경우 false 반환
                return false;   // 

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent.tag == "currentActiveTetromino")
                return true;    // 

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
                return false;   // 
        }
        return true;    // 좋은 위치이기 때문에 true 반환
    }   // 함수 끝
}  
