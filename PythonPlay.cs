using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PythonPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            try
            {
                Process psi = new Process();
                //파이썬 실행파일 경로
                psi.StartInfo.FileName = "C:/Users/user-pc/anaconda3/python.exe";
                // 시작할 어플리케이션 또는 문서
                psi.StartInfo.Arguments = "REALITY.py";
                // 애플 시작시 사용할 인수
                psi.StartInfo.CreateNoWindow = true;
                // 새창 안띄울지
                psi.StartInfo.UseShellExecute = false;
                // 프로세스를 시작할때 운영체제 셸을 사용할지
                psi.Start();

                UnityEngine.Debug.Log("[알림] .py file 실행");

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[알림] 에러발생: " + e.Message);
            }
        }
    }
}
