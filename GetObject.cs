using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObject : MonoBehaviour
{
    public Camera getCamera; //카메라받아옴
    private Object target; //현재오브젝트
    private RaycastHit hit;
    private string object_name; //오브젝트 이름

    void Start()
    {
        target = GameObject.Find(this.name).GetComponent<Object>(); //현재의 오브젝트를 가져옴
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //왼쪽클릭시
        {
            if (checking() == this.name)//마우스포인터가 현재 오브젝트에 있을 경우
            {
                if (target.rightclick == false && target.leftclick == false)
                {
                    //타겟의 오른쪽 클릭 X, 왼쪽 클릭 X일 경우 왼쪽 클릭을 true로
                    target.rightclick = false;
                    target.leftclick = true;
                }
                else if (target.rightclick == true && target.leftclick == false)
                {
                    //타겟의 오른쪽 클릭 O, 왼쪽 클릭 X일 경우 왼쪽 클릭을 true로, 오른쪽클릭을 해제
                    target.rightclick = false;
                    target.leftclick = true;
                }
                else
                {
                    //타겟의 왼쪽클릭이 O일경우, 오른쪽 왼쪽 클릭 해제
                    target.leftclick = false;
                    target.rightclick = false;
                }
            }
            else//마우스포인터가 현재 오브젝트에 없을 경우
            {
                //타겟의 왼쪽 오른쪽 클릭 해제
                target.leftclick = false;
                target.rightclick = false;

            }
        }
        else if (Input.GetMouseButtonDown(1)) //오른쪽 클릭한 경우 -> 왼쪽클릭했을 때처럼 진행
        {
            //Debug.Log("Pressed right click");
            if (checking() == this.name)
            {
                if (target.leftclick == false && target.rightclick == false)
                {
                    target.leftclick = false;
                    target.rightclick = true;
                }
                else if(target.leftclick == true && target.rightclick == false)
                {
                    target.leftclick = false;
                    target.rightclick = true;
                }
                else
                {
                    target.rightclick = false;
                    target.leftclick = false;
                }
            }
            else
            {
                target.rightclick = false;
                target.leftclick = false;
            }
        }
    }

    string checking() //마우스포인터가 오브젝트에 있는지 없는지 확인 -> 정상작동 되므로 신경 X
    {
        Ray ray = getCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            object_name = hit.collider.gameObject.name;
        }
        //Debug.Log(object_name);
        return object_name;
    }
}
