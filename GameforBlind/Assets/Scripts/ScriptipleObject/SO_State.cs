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

    public float positiveFirstStepValue;
    public float positiveSecondStepValue;
    public float positiveThirdStepValue;

    public float negativeFirstStepValue;
    public float negativeSeconStepValue;
    public float negativeThirdStepValue;


    public bool canInform;


}
