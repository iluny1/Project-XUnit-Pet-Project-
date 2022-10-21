using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] Unit unit;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        UpdateActionPointText();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, System.EventArgs e)
    {
        UpdateActionPointText();
    }

    private void UpdateActionPointText()
    {
        actionPointText.text = unit.GetActionPoints().ToString();
    }
}
