using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private int turnNumber;

    private bool isPlayerTurn = true;

    public event EventHandler OnNextTurn;

    public static TurnSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        turnNumber = 1;
    }

    public void NextTurn()
    {
        if (!IsPlayerTurn())
        {
            turnNumber++;
        }
        isPlayerTurn = !isPlayerTurn;
        OnNextTurn?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
