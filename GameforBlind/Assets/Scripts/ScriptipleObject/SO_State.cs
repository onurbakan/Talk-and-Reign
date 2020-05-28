using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Objeler/States", order = 1)]
public class SO_State : ScriptableObject
{
    public int stateID;
    public SO_State positiveNextState;
    public SO_State negativeNextState;

    public StateType stateType;
    public AudioClip stateSound;

    public float positivePopulationValue;
    public float positiveMoneyValue;
    public float positiveArmyValue;

    public float negativePopulationValue;
    public float negativeMoneyValue;
    public float negativeArmyValue;
    public bool checkPoint;


    public bool canInform;


    [Header("Positive Area")]
    public bool canInformGoldPos;
    public bool canInformArmyPos;
    public bool canInformPopulationPos;

    public bool canInformGoldNeg;
    public bool canInformArmyNeg;
    public bool canInformPopulationNeg;



    [Header("Negative Area")]
    public bool canInformGoldPos1;
    public bool canInformArmyPos1;
    public bool canInformPopulationPos1;

    public bool canInformGoldNeg1;
    public bool canInformArmyNeg1;
    public bool canInformPopulationNeg1;

}
