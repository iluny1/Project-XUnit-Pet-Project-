using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumber;
    [SerializeField] private GameObject unitActionSystemUI;
    [SerializeField] private GameObject turnCounter;
    [SerializeField] private GameObject enemyTurnVisual;

    private void Start()
    {
        TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
        OnEndTurnButtonClick();
        NumberUpdate();
        UpdateEnemyTurnVisual();
    }

    private void OnEndTurnButtonClick()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            EndTurn();
        });
    }


    private void EndTurn()
    {
        TurnSystem.Instance.NextTurn();
    }

    private void TurnSystem_OnNextTurn(object sender, EventArgs e)
    {
        NumberUpdate();
        UpdateEnemyTurnVisual();
    }

    private void NumberUpdate()
    {
        turnNumber.text = TurnSystem.Instance.GetTurnNumber().ToString();
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisual.gameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
        unitActionSystemUI.SetActive(TurnSystem.Instance.IsPlayerTurn());
        turnCounter.SetActive(TurnSystem.Instance.IsPlayerTurn());
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
