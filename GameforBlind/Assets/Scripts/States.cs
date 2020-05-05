
using UnityEngine;

public class States : MonoBehaviour
{
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
            Debug.Log(++index);
            isOpen = false;
        }

    }

    public void DecreaseIndex()
    {
        if (isOpen)
        {
            Debug.Log(--index);
            isOpen = false;
        }
    }
}
