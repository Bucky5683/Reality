/*
    오브젝트의 몸통부분이라고 생각하면 됨
    
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Object : MonoBehaviour
{
    public Transform object_name;   // 오브젝트 자신의 Transform(위치)
    private Cylindertest calling;   // UI 스크립트 접근 - 현재 선택된 오브젝트(target)에 접근
    private int a = 1;              //신경 안써도됨
    public bool leftclick = false;  // 마우스 왼쪽 클릭
    public bool rightclick = false; // 마우스 오른쪽 클릭

    //시작하자마자 한번 실행됨 -> 아두이노 생각하면 편함
    void Start()
    {
        object_name = GetComponent<Transform>();//실행되는 스크립트의 오브젝트 이름 가져오기
        calling = GameObject.Find("Canvas").GetComponent<Cylindertest>();//UI 스크립트 접근
    }
    //시작하고 무한반복 -> 아두이노 Loop 생각하면 편함
    void Update()
    {
        Movieng(); //오브젝트 움직이기 --> 신경 안써도 됨
        Click_behaving(); //마우스 왼쪽 클릭, 오른쪽 클릭 확인
    }

    //오브젝트 움직임 관련된 것 신경안써도 됨
    void Movieng()
    {
        //반복움직임
        if (transform.position.z < -10.0f)
        {
            a = 1;
        }
        else if (transform.position.z > 10.0f)
        {
            a = -1;
        }
        transform.Translate(Vector3.forward * 1.0f * Time.deltaTime * a);
    }

    //클릭시 자신을 선택된 오브젝트로 설정
    void Click_behaving()
    {
        //왼쪽 클릭시
        if (leftclick == true) //클릭이 되면
        {
            rightclick = false; //오른쪽 클릭을 해제
            if (calling.target != null && calling.target != GameObject.Find(this.name).GetComponent<Object>()) 
            {
                // 선택된 오브젝트가 있고, 현재 실행되고 있는 코드의 오브젝트와 그전에 선택된 오브젝트와 같지 않으면
                // 그전에 선택된 오브젝트의 왼쪽클릭, 오른쪽 클릭 해제 후, target 변수의 값을 null로 바꿈(선택을 없애버림)
                calling.target.leftclick = false;
                calling.target.rightclick = false;
            }
            calling.target = GameObject.Find(this.name).GetComponent<Object>(); //현재 클릭 오브젝트를 자신으로 바꿈
        }
        //오른쪽 클릭시
        else if (rightclick == true)
        {
            leftclick = false;
            if (calling.target != null && calling.target != GameObject.Find(this.name).GetComponent<Object>())
            {
                calling.target.leftclick = false;
                calling.target.rightclick = false;
            }
            calling.target = GameObject.Find(this.name).GetComponent<Object>(); //현재 클릭 오브젝트를 자신으로 바꿈
        }
        else
        {
            if (calling.target == GameObject.Find(this.name).GetComponent<Object>())
            {
                calling.target = null;
            }
        }
    }
}
