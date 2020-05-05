using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audiom;
    private void Start()
    {
    }

    private void Update()
    {
        if (audiom.time >= 10)// audiom.clip.length)
        {
            audiom.Stop();
        }

        //Debug.Log(m_MyAudioSource.clip.length);

    }
}
