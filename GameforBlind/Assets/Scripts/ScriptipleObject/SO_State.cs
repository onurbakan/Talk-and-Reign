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


    public bool canInform;


}
