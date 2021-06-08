using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;
    public static ObjectManager Instance
    {
        get
        {
            return instance;
        }
    }


    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }


    [SerializeField]
    private Move poolObj;
    [SerializeField]
    private int allocateCount = 10; // 미리 풀링 할 오브젝트 개수

    private Stack<Move> poolStack = new Stack<Move>();

    private void Start()
    {
        Allocate();
    }
   
    public void Allocate()               // 풀링 할 오브젝트를 미리 정한 개수만큼 생성, 스택에 할당
    {
        for (int i = 0; i < allocateCount; i++)
        {
            Move allocateObj = Instantiate(poolObj, this.gameObject.transform); 
            poolStack.Push(allocateObj);
            allocateObj.gameObject.SetActive(false);
            allocateObj.objectID = i;
            //Debug.Log(allocateObj.objectID);
        }
    }

    public Move Pop()            // 오브잭트를 풀에서 꺼냄
    {
        if (poolStack.Count > 0)
        {
            Move obj = poolStack.Pop();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var obj = Create();
            obj.gameObject.SetActive(true);
            poolStack.Push(obj);
            return poolStack.Pop();
        }
    }

    public void Push(Move obj)      // 오브잭트를 풀에 넣음
    {
        obj.gameObject.SetActive(false);
        poolStack.Push(obj);
    }

    public Move Create()
    {
        Move allocateObj = Instantiate(poolObj, this.gameObject.transform);
        return allocateObj;

    }

}
