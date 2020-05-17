using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource _audioSource;
    public bool canListen;

    public Action soundFinished;


    private void Update()
    {
        if (!_audioSource.isPlaying && canListen)
        {
            Debug.Log("[Sound Manager] : Şarkı Bitti");
            canListen = false;
            soundFinished?.Invoke();
        }

    }


    public void PlayCurrentState(SO_State input)
    {
        Debug.Log("[Sound Manager] : Şarkım Başladı");

        _audioSource.clip = input.stateSound;
        _audioSource.Play();
        canListen = true;
    }


}
