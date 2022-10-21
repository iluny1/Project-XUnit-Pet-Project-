using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshPro;

    private object gridObject;

    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        TextUpdate();
    }

    private void TextUpdate()
    {
        if (gridObject != null)
        {
            textMeshPro.text = gridObject.ToString();
        }
    }
}
