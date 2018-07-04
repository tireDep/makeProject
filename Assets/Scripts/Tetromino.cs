using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {
    float fall = 0;
    public float fallSpeed = 1; // 떨어지는 속도

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckUserInput();
	}

    void CheckUserInput()   // 게임 실행 관련
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))    // 오른쪽 화살표 입력
        {
            transform.position += new Vector3(1, 0, 0);

            if(CheckIsValidPosition()){
            }else
                transform.position += new Vector3(-1, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) // 왼쪽 화살표 입력
        {
            transform.position += new Vector3(-1, 0, 0);

            if (CheckIsValidPosition()){
            }else
                transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) // 위쪽 화살표 입력
        {
            transform.Rotate(0, 0, 90); // 90도 회전

            if(CheckIsValidPosition()){
            }else
                transform.Rotate(0, 0, -90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time-fall>=fallSpeed) // 아래쪽 화살표 입력 or 자동으로 한칸씩 떨어짐
        {
            transform.position += new Vector3(0, -1, 0);    // 아래로 한 칸 이동

            if (CheckIsValidPosition()){
            }else
                transform.position += new Vector3(0, 1, 0);

            fall = Time.time;   // 떨어지는 속도 증가!
            // 시간 - 지난 시간 >= 떨어지는 속도
        }
        // 스페이스바 -> 한번에 떨어지는 것 추가하기
    }

    bool CheckIsValidPosition() // 올바른 위치인지 판별
    {
        foreach(Transform Block in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(Block.position);

            if (FindObjectOfType<Game>().CheckInsideGrid(pos) == false)
                return false;
        }
        return true;
    }
}
