using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Autocunnect : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] players;
    Camera M_camera;
    int i = 0;

    void Start()
    {
        M_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        while (i < players.Length)
        {
            players[i].AddComponent<Object>();
            players[i].AddComponent<GetObject>();
            players[i].AddComponent<GetObject>().getCamera = M_camera;
        }
    }
}
