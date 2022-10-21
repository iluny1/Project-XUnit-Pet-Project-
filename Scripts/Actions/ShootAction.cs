using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private int maxShootDistance = 3;
    [SerializeField] private LayerMask obstacleLayerMasks;

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    public EventHandler<OnShootActionEventArgs> OnShootAction;

    public class OnShootActionEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private State state;
    private float stateTimer = 2f;
    private Unit targetUnit;
    private bool canShootBullet;



    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 targetPosition = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 5f;
                transform.forward = Vector3.Lerp(transform.forward, targetPosition, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void Shoot()
    {

        OnShootAction(this, new OnShootActionEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
        });
        targetUnit.Damage(minDamage, maxDamage);        
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                if (stateTimer <= 0f)
                {
                    state = State.Shooting;
                    stateTimer = 2f;
                }
                break;
            case State.Shooting:
                if (stateTimer <= 0f)
                {
                    state = State.Cooloff;
                    stateTimer = 2.5f;
                }
                break;
            case State.Cooloff:
                if (stateTimer <= 0f)
                {
                    ActionComplete();
                }
                break;
        }
    }


    public override Material GetActionLogo()
    {
        return baseMaterial;
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; //Outside of Grid
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue; //No unit on this grid
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // If both Units on same "team" 
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;

                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstacleLayerMasks))
                {
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float aimingStateTime = 2f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            actionName = GetActionName(),
            gridPosition = gridPosition,
            actionValue = 150 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f) - targetUnit.GetHealthCount(),
        };

    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
