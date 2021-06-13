using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class MakeNewObject : MonoBehaviour
{

    //public GameObject cubePrefab;

    int num = 0; // ������ ���� �� �ѹ�
    int frSize = 627; // ������ ����
    

    // MakeData�� ���� ����
    public TextAsset txt;
    public string[,] Sentence;
    public int lineSize, rowSize;
    public float[,] frames; // �����Ӹ��� ��ü �� ������
    public int[,] objectMap; // �����Ӹ��� 0~10 (������ �ִ�) id ������ line, ������ 0


    public void Start()
    {
        PythonPlay.python();
        MakeData();
        //for (int i =0; i<10; i++)
        //    print(objectMap[0, i]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (num == 626)
        {
            Debug.Log("�� ���� ");
        }
        if (num > frSize-1)     // frames ������ Update�� �����
        {
            return;
        }


        for (int i = 1; i <= 383; i++)    // i �� Move ������Ʈ �� ID, i-1�� ��ü ���� ( GetChild�� ���� �� )
        {
            if (objectMap[num, i] == 0)      // �̹� �����ӿ��� i�� ID�� ��ü�� ����
            {
                // �� ��ȣ�� ��ü ����
                ObjectManager.instance.transform.GetChild(i-1).gameObject.SetActive(false);
            }
            else if (objectMap[num, i] > 0) // �̹� �����ӿ� i�� ID�� ��ü�� ����
            {
                // �� ��ȣ�� ��ü�� �� ��ǥ�� �����ֱ�
                 ObjectManager.instance.transform.GetChild(i-1).gameObject.SetActive(true);
                ObjectManager.instance.transform.GetChild(i-1).position
               = new Vector3(float.Parse(Sentence[objectMap[num, i], 1]), 0, float.Parse(Sentence[objectMap[num, i], 2]));
            }
        }


        num++;
        //Debug.Log("num : " + num);
    }

    void Create(int index)          // ���ڶ���ŭ �߰�
    {
        for (int i=0;i<index; i++)
        {
            ObjectManager.instance.Pop();
        }
    }

    void Return(int index)          // ���¸�ŭ ��ȯ
    {
        for(int i=0; i<index; i++)
        {
            ObjectManager.instance.Push(ObjectManager.instance.transform.GetChild(i).gameObject.GetComponent<Move>());
        }
    }

    public int MakeData()
    {
        // ���ʹ����� ������ ������ �迭�� ũ�� ����
        string curretText = txt.text.Substring(0, txt.text.Length - 1);
        string[] line = curretText.Split('\n');
        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;
        Sentence = new string[lineSize, rowSize];


        // �� �ٿ��� ������ ������ Sentence�� ä��
        for (int i = 0; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            for (int j = 0; j < rowSize; j++)
            {
                Sentence[i, j] = row[j];
                //(i + "," + j + "   " + Sentence[i, j] + "   " + rowSize + "\n");
            }
        }

        // frame � ��ü�� �� ������ ����. frame �迭�� 0���� ����. �� frame�� ������ ��line�������� ����.
        // frame �� �� �������� �̸� �ޱ�� ��. 
        frames = new float[frSize, 2];

        // ��ü id ���� �ִ밡 393
        objectMap = new int[frSize, 384]; 

        int f = 0;

            for (int i = 0; i < lineSize; i++)
            {
                if (Sentence[i, 0] == "frame")
                {
                    frames[f, 0] = float.Parse(Sentence[i, 2]); // ��ü ����
                    frames[f, 1] = i;                           // ���°��������
                    objectMap[f, 0] = f;

                for (int k = i + 1; k <= i + frames[f, 0]; k++) // frame������ line���� idȮ��
                { 
                    for (int id = 0; id <383; id++)
                    {
                        if (float.Parse(Sentence[k, 0]) == id)
                        {
                            objectMap[f, id] = k;   // id�� ��ü ������ ���° �������� ����
                        }
                        else
                        {
                            //objectMap[f, id] = -1;  // ������ ����
                        }
                        
                    }
                }
                //print(frames[f, 0] + ", " + frames[f, 1]);
                f++;
                }

            }

        return lineSize;
    }
}
