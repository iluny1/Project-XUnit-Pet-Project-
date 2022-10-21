using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCellLogic : MonoBehaviour
{
    private bool isActive;

    private void Start()
    {
        isActive = true;
    }

    public void SetOn()
    {
        if (isActive)
        {
            Transform activeCell = gameObject.transform.GetChild(1);
            activeCell.gameObject.SetActive(true);
        }
    }

    public void SetOff()
    {
        Transform activeCell = gameObject.transform.GetChild(1);
        activeCell.gameObject.SetActive(false);
    }
}
