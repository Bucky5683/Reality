using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylindertest : MonoBehaviour
{
    public Object undotarget = null;        // 이전에 클릭한 오브젝트 Transform
    public Object target=null;            // 오브젝트 Transform

    void Start()
    {
        //canvas = GetComponent<Transform>();
    }

    void Update()
    {
        Checkobject();
        if (target != null)
        {
            Debug.Log(target.object_name.GetInstanceID() + " leftclick = " + target.leftclick + " rightclick = " + target.rightclick);
            FollowCamera();
        }
    }
    
    //캠 설정
    void FollowCamera()
    {
        Maincamera maincamera = GameObject.Find("Main Camera").GetComponent<Maincamera>();
        if (target.rightclick == true || target.leftclick == true)//오브젝트의 왼쪽클릭 또는 오른쪽 클릭이 true일 경우
            maincamera.target = this.target; //maincamera의 target이 선택된 오브젝트.
        else
            maincamera.target = null;
    }

    //이전에 선택한 오브젝트와 현재 오브젝트가 같은지 다른지 확인
    void Checkobject()
    {
        if (target!= null)//오브젝트가 선택되었을 때
        {
            if (undotarget != target) // 이전 오브젝트와 현재 오브젝트가 다를 때
            {
                if (undotarget != null) // 이전 오브젝트가 있을 때
                {
                    undotarget.leftclick = false; //이전 오브젝트의 왼쪽클릭확인을 false로 만듦
                    undotarget.rightclick = false; //이전 오브젝트의 오른쪽클릭확인을 false로 만듦
                }
                undotarget = target;//현재 오브젝트를 이전 오브젝트에 저장
            }
        }
        else
        {
            target = null;
            undotarget = null;
        }
    }
}
