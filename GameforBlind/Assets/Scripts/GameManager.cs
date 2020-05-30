using IBM.Watsson.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SO_State firstState;
    public TTS_STT stt;
    public SO_State currentState;
    public SoundManager soundManager;

    float firstStatus = 2;
    float seconStatus = 2;
    float thirdStatus = 2;

    bool soundPlaying;
    bool currentStateAnswer;

    public Text status1;
    public Text status2;
    public Text status3;
    public Text years;


    private void Start()
    {

        States.instance.saidYes += SaidYes;
        States.instance.saidNo += SaidNo;
        soundManager.PlayCurrentState(currentState);
        soundManager.soundFinished += SoundFinished;
        soundManager.informFinished += InformFinished;
        UpdateUI();
    }

    private void InformFinished()
    {
        if (currentStateAnswer)
        {
            if (seconStatus == 0 || firstStatus ==0 || thirdStatus==0)
            {
                currentState = firstState;
                firstStatus = 2;
                seconStatus = 2;
                thirdStatus = 2;
                soundManager.PlayCurrentState(currentState);
            }
            else
            {
                currentState = currentState.positiveNextState;

                soundManager.PlayCurrentState(currentState);
            }

        }
        else
        {
            if (seconStatus == 0 || firstStatus == 0 || thirdStatus == 0)
            {
                currentState = firstState;
                firstStatus = 2;
                seconStatus = 2;
                thirdStatus = 2;
                soundManager.PlayCurrentState(currentState);
                //Debug.Log("0 landı No dedim");
                //Debug.Log(firstStatus);
                //Debug.Log(seconStatus);
                //Debug.Log(thirdStatus);
            }
            else
            {
                //Debug.Log("No dedim");
                //Debug.Log(firstStatus);
                //Debug.Log(seconStatus);
                //Debug.Log(thirdStatus);
                currentState = currentState.negativeNextState;

                soundManager.PlayCurrentState(currentState);
            }

        }
    }

    private void SoundFinished()
    {
        //Debug.Log("Game Manager : Sound Bitti");
        if (currentState.stateType == StateType.OnlySound)
        {
            //Debug.Log("[Game Manager] :[Only Sound] Yes Dedim");
            currentState = currentState.positiveNextState;
            soundManager.PlayCurrentState(currentState);

        }
        else if (currentState.stateType == StateType.AnswerSound)
        {
            //Debug.Log("[Game Manager] :[Answer] Input Açtım");

            StartInput();
        }
        if (currentState.checkPoint)
        {
            firstState.positiveNextState = currentState;
        }

    }


    public void SaidYes()
    {
        currentStateAnswer = true;
        //Debug.Log("Yes Function");

        firstStatus += currentState.positivePopulationValue;
        seconStatus += currentState.positiveMoneyValue;
        thirdStatus += currentState.positiveArmyValue;
        UpdateUI();


        soundManager.InformUser(currentState, true);
        /* currentState = currentState.positiveNextState;

         soundManager.PlayCurrentState(currentState);*/

        StopInput();
    }

    public void SaidNo()
    {
        currentStateAnswer = false;

        //Debug.Log("No Function");
        firstStatus += currentState.negativePopulationValue;
        seconStatus += currentState.negativeMoneyValue;
        thirdStatus += currentState.negativeArmyValue;
        UpdateUI();
        
        soundManager.InformUser(currentState, false);
        /*  currentState = currentState.negativeNextState;
          soundManager.PlayCurrentState(currentState);*/
        StopInput();
    }


    public void StartInput()
    {
        //Debug.Log("Input Başladı");
        stt.canListen = true;
        States.instance.isOpen = true;
    }


    public void StopInput()
    {
        //Debug.Log("Stop Input");
        stt.canListen = false;
    }


    public void UpdateUI()
    {
        status1.text = firstStatus.ToString();
        status2.text = seconStatus.ToString();
        status3.text = thirdStatus.ToString();
        years.text = currentState.stateID.ToString();

    }
}


public enum StateType
{
    OnlySound,
    AnswerSound
}
