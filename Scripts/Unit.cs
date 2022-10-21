using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] private Animator unitAnimator;
    [SerializeField] private bool isEnemy;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;

    private GridPosition gridPosition;
    private BaseAction[] baseActionArray;
    private const int ACTION_POINTS_MAX = 2;
    private int actionPoints = ACTION_POINTS_MAX;

    private HealthSystem healthSystem;


    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
        healthSystem.OnDead += healthSystem_OnDead;

        OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMoveGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }

        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TryToSpendActionPoints(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amountToSpend)
    {
        actionPoints -= amountToSpend;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnNextTurn(object sender, EventArgs e)
    {
        if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() || !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int minDamage, int maxDamage)
    {
        healthSystem.Damage(minDamage, maxDamage);
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealthCount()
    {
        return healthSystem.GetHealthCount();
    }

    public int GetMaxHealthCount()
    {
        return healthSystem.GetMaxHealthCount();
    }

    public float GetHealthNormalized()
    {
        Debug.Log(this + "has: " + Mathf.RoundToInt(healthSystem.GetHealthNormalized() * 100f) + " HP");
        return healthSystem.GetHealthNormalized();
    }
}
