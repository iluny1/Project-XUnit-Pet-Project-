using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 5;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    // Update is called once per frame
    private void Update()
    {

        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {

            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;

            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }


    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }



        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; //Outside of Grid
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > maxMoveDistance)
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    continue; // Unit on this grid
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue; //Other object on this grid
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int PathFindingMulti = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * PathFindingMulti)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override Material GetActionLogo()
    {
        return baseMaterial;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        //Debug.Log("Count for Move at GP: " + gridPosition);        
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        float DistanceToTarget = 0f;
        //Debug.Log("Initialize DTT: " + DistanceToTarget);
        //Debug.Log("Enemy position is " + transform.position);
        List<Unit> friendlyUnitList = UnitManager.Instance.GetFriendlyUnitList();
        foreach (Unit unit in friendlyUnitList)
        {
            float DistanceToFriendlyUnit = Vector3.Distance(LevelGrid.Instance.GetWorldPosition(gridPosition), unit.GetWorldPosition());
            //Debug.Log("For F-Unit: " + unit + " distance from enemy is: " + DistanceToFriendlyUnit);
            if (DistanceToTarget == 0f)
            {
                DistanceToTarget = DistanceToFriendlyUnit;
            }
            else if (DistanceToTarget > DistanceToFriendlyUnit)
            {
                DistanceToTarget = DistanceToFriendlyUnit;
            }
        }

        //Debug.Log("Distance to close target: " + DistanceToTarget);

        if (targetCountAtGridPosition > 0)
        {
            return new EnemyAIAction
            {
                actionName = GetActionName(),
                gridPosition = gridPosition,
                actionValue = (targetCountAtGridPosition * 15),
            };
        }
        else
        {
            return new EnemyAIAction
            {
                actionName = GetActionName(),
                gridPosition = gridPosition,
                actionValue = 35 - Mathf.RoundToInt(DistanceToTarget),
            };
        }

    }
}
