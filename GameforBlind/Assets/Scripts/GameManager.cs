using IBM.Watsson.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TTS_STT stt;

    private void Start()
    {



    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            stt.canListen = true;
            States.instance.isOpen = true;
            Debug.Log("inv başladı");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            stt.canListen = false;
            Debug.Log("inv bitti");
        }
    }


}
