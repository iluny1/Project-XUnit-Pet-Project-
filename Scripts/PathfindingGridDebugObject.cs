using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;
    [SerializeField] private SpriteRenderer IsWalkableSpriteRenderer;
    [SerializeField] private GameObject IsWalkableSprite;
    [SerializeField] private bool IsWalkableStatusSeen;

    private PathNode pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();

        UpdateColor();

    }

    private void UpdateColor()
    {
        if (IsWalkableStatusSeen == true)
        {
            IsWalkableSprite.SetActive(true);
            if (pathNode.IsWalkable() == true)
            {
                IsWalkableSpriteRenderer.color = Color.green;
            }
            else IsWalkableSpriteRenderer.color = Color.red;
        }
        else
            IsWalkableSprite.SetActive(false);
    }
}
