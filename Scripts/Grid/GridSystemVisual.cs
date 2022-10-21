using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        Red,
        Blue,
        Green,
        Enemy,
        Friendly,
        Yellow,
        White
    }



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SettingGridSystemVisual();

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_onAnyUnitMovedGridPosition;
        LevelGrid.Instance.OnAnyUnitGetRemoved += LevelGrid_OnAnyUnitGetRemoved;

        UpdateGridVisual();
    }

    private void SettingGridSystemVisual()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();

        GridVisualType gridVisualType;

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        switch (selectedAction)
        {
            default:
                gridVisualType = GridVisualType.White;
                break;
            case MoveAction moveAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Friendly;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Enemy;
                break;
        }

        GridSystemVisual.Instance.ShowGridPositionList(
            selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_onAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitGetRemoved(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError("Couldn't find material for grid type");
        return null;
    }
}
