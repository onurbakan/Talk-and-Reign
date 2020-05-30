using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Action soundFinished;
    public Action informFinished;

    public AudioSource _audioSource;
    public bool canListen;
    public bool canInform;
    public int indexAudio = 0;


    public AudioClip goldPositive;
    public AudioClip armyPositive;
    public AudioClip populationPositive;

    public AudioClip goldNegative;
    public AudioClip armyNegative;
    public AudioClip populationNegative;

    public List<AudioClip> myAudioList;

    private void Update()
    {
        if (!_audioSource.isPlaying && canListen)
        {
            //Debug.Log("[Sound Manager] : Şarkı Bitti");
            canListen = false;
            soundFinished?.Invoke();

        }

        if (!_audioSource.isPlaying && canInform)
        {
            canInform = false;
            //Debug.Log("[Sound Manager] : Bilgilendirme Bitti");
            indexAudio++;

            if (indexAudio == myAudioList.Count)
            {
                //Debug.Log("Invoke Inform");
                informFinished?.Invoke();
            }
            else
            {
                //Debug.Log("Bir daha çalıdm");

                PlayMyAudioList();
            }


        }

    }


    public void PlayCurrentState(SO_State input)
    {

        //Debug.Log("[Sound Manager] : Şarkım Başladı");

        _audioSource.clip = input.stateSound;
        _audioSource.Play();
        canListen = true;
    }



    public void InformUser(SO_State input, bool isPositive)
    {
        indexAudio = 0;
        myAudioList.Clear();
        if (isPositive)
        {
            if (input.canInformGoldPos)
            {
                myAudioList.Add(goldPositive);
            }

            if (input.canInformGoldNeg)
            {
                myAudioList.Add(goldNegative);

            }

            if (input.canInformArmyPos)
            {
                myAudioList.Add(armyPositive);
            }

            if (input.canInformArmyNeg)
            {
                myAudioList.Add(armyNegative);
            }

            if (input.canInformPopulationPos)
            {
                myAudioList.Add(populationPositive);
            }
            if (input.canInformPopulationNeg)
            {
                myAudioList.Add(populationNegative);
            }
            //Debug.Log(myAudioList.Count);
            if (myAudioList.Count != 0)
            {
                PlayMyAudioList();
            }
            else
            {

                informFinished?.Invoke();
            }
        }
        else
        {
            if (input.canInformGoldPos1)
            {
                myAudioList.Add(goldPositive);
            }

            if (input.canInformGoldNeg1)
            {
                myAudioList.Add(goldNegative);

            }

            if (input.canInformArmyPos1)
            {
                myAudioList.Add(armyPositive);
            }

            if (input.canInformArmyNeg1)
            {
                myAudioList.Add(armyNegative);
            }

            if (input.canInformPopulationPos1)
            {
                myAudioList.Add(populationPositive);
            }
            if (input.canInformPopulationNeg1)
            {
                myAudioList.Add(populationNegative);
            }
            //Debug.Log(myAudioList.Count);
            if (myAudioList.Count != 0)
            {
                PlayMyAudioList();
            }
            else
            {

                informFinished?.Invoke();
            }
        }


    }


    public void PlayMyAudioList()
    {
        //Debug.Log("[Sound Manager] : Bilgilendirme Başladı");

        _audioSource.clip = myAudioList[indexAudio];
        _audioSource.Play();
        canInform = true;

    }



}
