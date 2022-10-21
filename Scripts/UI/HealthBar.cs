using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform healthPointPrefab;
    [SerializeField] private Unit thisUnit;
    [SerializeField] private HealthSystem healthSystem;

    private GridLayoutGroup gridLayout;
    private int currentHealth;
    private int MAX_Health;
    private List<GameObject> healthCellList;

    private void Awake()
    {
        healthCellList = new List<GameObject>();
        gridLayout = gameObject.GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        healthSystem.OnHealthChange += healthSystem_OnHealthChange;

        Setup();
    }

    private void Setup()
    {
        MAX_Health = thisUnit.GetHealthCount();
        currentHealth = MAX_Health;

        for (int i = 0; i < MAX_Health; i++)
        {
            Transform objectTransform;
            objectTransform = Instantiate(healthPointPrefab, gameObject.transform);
            healthCellList.Add(objectTransform.gameObject);
        }
        SetGridLayout(gridLayout, MAX_Health);
    }

    private void SetGridLayout(GridLayoutGroup grid, int mHealth)
    {
        switch (mHealth)
        {
            case <= 5:
                grid.cellSize = new Vector2(0.35f, 0.2f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 5;
                break;
            case <= 8:
                grid.cellSize = new Vector2(0.3f, 0.2f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 8;
                break;
            case <= 12:
                grid.cellSize = new Vector2(0.3f, 0.15f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 6;
                break;
            case <= 16:
                grid.cellSize = new Vector2(0.25f, 0.15f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 8;
                break;
            case <= 20:
                grid.cellSize = new Vector2(0.2f, 0.15f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 10;
                break;
            case <= 24:
                grid.cellSize = new Vector2(0.2f, 0.15f);
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 12;
                break;
            default:
                {
                    grid.cellSize = new Vector2(0.15f, 0.1f);
                    grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                    grid.constraintCount = 3;
                }
                break;
        }
    }

    private void healthSystem_OnHealthChange(object sender, EventArgs e)
    {
        int newCurrentHealth = thisUnit.GetHealthCount();

        if (currentHealth != newCurrentHealth)
        {
            currentHealth = newCurrentHealth;

            for (int i = 0; i < currentHealth; i++)
            {
                healthCellList[i].GetComponent<HealthCellLogic>().SetOn();
            }

            for (int i = currentHealth; i < MAX_Health; i++)
            {
                healthCellList[i].GetComponent<HealthCellLogic>().SetOff();
            }
        }
    }
}
