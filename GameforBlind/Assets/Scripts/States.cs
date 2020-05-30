
using System;
using UnityEngine;

public class States : MonoBehaviour
{
    public Action saidYes;
    public Action saidNo;
    public int index = 0;
    public bool isOpen = false;
    #region Singleton

    public static States instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }





    #endregion


    public void IncreaseIndex()
    {
        if (isOpen)
        {
            //Debug.Log(++index);
            saidYes?.Invoke();
            isOpen = false;
        }

    }

    public void DecreaseIndex()
    {
        if (isOpen)
        {
            //Debug.Log(--index);
            saidNo?.Invoke();
            isOpen = false;

        }
    }
}
