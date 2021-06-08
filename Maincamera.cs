using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maincamera : MonoBehaviour
{
    private Vector3 firstlocation = new Vector3(0, 13, -20); //메인카메라 처음위치
    private Cylindertest target_object; // 선택된 오브젝트 받아오기 위해서
    public Object target;        // 따라다닐 타겟 오브젝트의 Transform
    private float speed_rota = 2.0f;
    public Transform tr;         // 카메라 자신의 Transform
    

    void Start()
    {
        tr = GetComponent<Transform>();
        target_object = GameObject.Find("Canvas").GetComponent<Cylindertest>();
    }

    void LateUpdate()
    {
        Debug.Log(target.leftclick + " " + target.rightclick);
        if (target == null || (target.rightclick == false && target.leftclick == false))
        {
            /*if (target != null)
                Debug.Log("Maincamera: No checked!");
            else
                Debug.Log("Maincamera: target is null!");*/
            tr.position = firstlocation;
            tr.rotation = Quaternion.Euler(90, 0, 0);
        }
        else if (target.rightclick == true || target.leftclick == true)
        {
            if (target.leftclick == true)
            {
                //Debug.Log("Maincamera: Left checked!");
                tr.position = new Vector3(target.object_name.position.x - 0.52f, tr.position.y, target.object_name.position.z - 6.56f);
                tr.rotation = Quaternion.Euler(30, 0, 0);
                tr.LookAt(target.object_name);
            }
            else if (target.rightclick == true)
            {
                //Debug.Log("Maincamera: right checked!");
                tr.position = new Vector3(target.object_name.position.x, target.object_name.position.y, target.object_name.position.z);
                tr.rotation = Quaternion.Euler(0, 0, 0);
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                transform.Rotate(Vector3.up * speed_rota * mouseX);
                transform.Rotate(Vector3.left * speed_rota * mouseY);
            }
            else
            {
                //Debug.Log("Maincamera: both checked!");
                tr.position = firstlocation;
                tr.rotation = Quaternion.Euler(90, 0, 0);
                /*tr.position = new Vector3(target.object_name.position.x - 0.52f, tr.position.y, target.object_name.position.z - 6.56f);
                tr.LookAt(target.object_name);*/
            }
        }
        if(target != null)
        {
            Debug.Log("Maincamera: " + target.object_name.GetInstanceID() + " leftclick = " + target.leftclick + " rightclick = " + target.rightclick);
        }
    }
}
