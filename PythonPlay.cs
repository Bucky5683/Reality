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
                //���̽� �������� ���
                psi.StartInfo.FileName = "C:/Users/user-pc/anaconda3/python.exe";
                // ������ ���ø����̼� �Ǵ� ����
                psi.StartInfo.Arguments = "REALITY.py";
                // ���� ���۽� ����� �μ�
                psi.StartInfo.CreateNoWindow = true;
                // ��â �ȶ����
                psi.StartInfo.UseShellExecute = false;
                // ���μ����� �����Ҷ� �ü�� ���� �������
                psi.Start();

                UnityEngine.Debug.Log("[�˸�] .py file ����");

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[�˸�] �����߻�: " + e.Message);
            }
        }
    }
}
