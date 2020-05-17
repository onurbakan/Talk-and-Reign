using IBM.Watsson.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TTS_STT stt;
    public SO_State currentState;
    public SoundManager soundManager;

    float firstStatus = 100;
    float seconStatus = 100;
    float thirdStatus = 100;

    bool soundPlaying;

    public Text status1;
    public Text status2;
    public Text status3;



    private void Start()
    {

        States.instance.saidYes += SaidYes;
        States.instance.saidNo += SaidNo;
        soundManager.PlayCurrentState(currentState);
        soundManager.soundFinished += SoundFinished;
        UpdateUI();


    }






    private void SoundFinished()
    {
        Debug.Log("Game Manager : Sound Bitti");
        if (currentState.stateType == StateType.OnlySound)
        {
            Debug.Log("[Game Manager] :[Only Sound] Yes Dedim");
            currentState = currentState.positiveNextState;
            soundManager.PlayCurrentState(currentState);
        }
        else if (currentState.stateType == StateType.AnswerSound)
        {
            Debug.Log("[Game Manager] :[Answer] Input Açtım");

            StartInput();

        }


    }


    public void SaidYes()
    {
        Debug.Log("Yes Function");

        firstStatus += currentState.positiveFirstStepValue;
        seconStatus += currentState.positiveSecondStepValue;
        thirdStatus += currentState.positiveThirdStepValue;
        UpdateUI();

        currentState = currentState.positiveNextState;
        soundManager.PlayCurrentState(currentState);

        StopInput();
    }

    public void SaidNo()
    {
        Debug.Log("No Function");
        firstStatus += currentState.negativeFirstStepValue;
        seconStatus += currentState.negativeSeconStepValue;
        thirdStatus += currentState.negativeThirdStepValue;
        UpdateUI();
        currentState = currentState.negativeNextState;
        soundManager.PlayCurrentState(currentState);
        StopInput();
    }


    public void StartInput()
    {
        Debug.Log("Input Başladı");
        stt.canListen = true;
        States.instance.isOpen = true;
    }


    public void StopInput()
    {
        Debug.Log("Stop Input");
        stt.canListen = false;
    }


    public void UpdateUI()
    {
        status1.text = firstStatus.ToString();
        status2.text = seconStatus.ToString();
        status3.text = thirdStatus.ToString();

    }
}


public enum StateType
{
    OnlySound,
    AnswerSound
}
