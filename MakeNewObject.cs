using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class MakeNewObject : MonoBehaviour
{

    //public GameObject cubePrefab;

    int num = 0; // 프레임 순서 및 넘버
    int frSize = 56; // 프레임 개수
    

    // MakeData를 위한 선언
    public TextAsset txt;
    public string[,] Sentence;
    public int lineSize, rowSize;
    public float[,] frames; // 프레임마다 객체 몇 개인지
    public int[,] objectMap; // 프레임마다 0~10 (지금의 최대) id 있으면 line, 없으면 0


    public void Start()
    {
        MakeData();
        //for (int i =0; i<10; i++)
        //    print(objectMap[0, i]);
    }

    // Update is called once per frame
    void Update()
    {
        if (num > frSize-1)     // frames 끝나면 Update를 멈춘다
        {
            return;
        }


        for (int i = 0; i < 10; i++)    // i 는 ID
        {
            if (objectMap[num, i] == 0)      // 이번 프레임에는 i번 ID의 객체가 없음
            {
                // 그 번호의 객체 끄기
                ObjectManager.instance.transform.GetChild(i).gameObject.SetActive(false);
            }
            else if (objectMap[num, i] > 0) // 이번 프레임에 i번 ID의 객체가 있음
            {
                // 그 번호의 객체를 그 좌표로 보여주기
                //print("엥");
                 ObjectManager.instance.transform.GetChild(i).gameObject.SetActive(true);
                ObjectManager.instance.transform.GetChild(i).position
               = new Vector3(float.Parse(Sentence[objectMap[num, i], 1]), 0, float.Parse(Sentence[objectMap[num, i], 2]));
            }
        }


        num++;
        //Debug.Log("num : " + num);
    }

    void Create(int index)          // 모자란만큼 추가
    {
        for (int i=0;i<index; i++)
        {
            ObjectManager.instance.Pop();
        }
    }

    void Return(int index)          // 남는만큼 반환
    {
        for(int i=0; i<index; i++)
        {
            ObjectManager.instance.Push(ObjectManager.instance.transform.GetChild(i).gameObject.GetComponent<Move>());
        }
    }

    public int MakeData()
    {
        // 엔터단위와 탭으로 나눠서 배열의 크기 조정
        string curretText = txt.text.Substring(0, txt.text.Length - 1);
        string[] line = curretText.Split('\n');
        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;
        Sentence = new string[lineSize, rowSize];


        // 한 줄에서 탭으로 나누고 Sentence를 채움
        for (int i = 0; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            for (int j = 0; j < rowSize; j++)
            {
                Sentence[i, j] = row[j];
                //(i + "," + j + "   " + Sentence[i, j] + "   " + rowSize + "\n");
            }
        }

        // frame 몇에 객체가 몇 개인지 저장. frame 배열은 0부터 시작. 그 frame의 시작이 몇line인지까지 저장.
        // frame 이 몇 개인지는 미리 받기로 함. 
        frames = new float[frSize, 2];

        // 객체 id 지금 최대가 10
        objectMap = new int[frSize, 10]; 

        int f = 0;

            for (int i = 0; i < lineSize; i++)
            {
                if (Sentence[i, 0] == "frame")
                {
                    frames[f, 0] = float.Parse(Sentence[i, 2]); // 객체 개수
                    frames[f, 1] = i;                           // 몇번째라인인지
                    objectMap[f, 0] = f;

                for (int k = i + 1; k <= i + frames[f, 0]; k++) // frame내에서 line돌며 id확인
                { 
                    for (int id = 0; id <10; id++)
                    {
                        if (float.Parse(Sentence[k, 0]) == id)
                        {
                            objectMap[f, id] = k;   // id의 객체 있으면 몇번째 라인인지 넣음
                        }
                        else
                        {
                            //objectMap[f, id] = -1;  // 없으면 음수
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
