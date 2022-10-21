using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private bool rotateCamera;
    private ShootAction shootActionForRotation;
    private float timer = 1.3f;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        rotateCamera = false;
    }

    private void Update()
    {
        if (!rotateCamera)
        {
            return;
        }

        Unit actingUnit = shootActionForRotation.GetUnit();
        Unit targetUnit = shootActionForRotation.GetTargetUnit();
        Transform cameraPosition = actingUnit.transform.GetChild(3);
        Vector3 cameraOnActingUnit = new Vector3(actingUnit.transform.position.x, cameraPosition.position.y, actingUnit.transform.position.z);
        Vector3 cameraOnTargetUnit = new Vector3(targetUnit.transform.position.x, cameraPosition.position.y, targetUnit.transform.position.z);

        timer -= Time.deltaTime;
        actionCameraGameObject.transform.LookAt(cameraOnActingUnit);
        if (timer <= 0)
        {
            actionCameraGameObject.transform.LookAt(cameraOnTargetUnit);
            rotateCamera = false;
            timer = 1.3f;
        }


    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                shootActionForRotation = shootAction;
                float disBetweenUnits = GetDistanceBetweenUnits(shootAction);
                Transform cameraPosition = GetCameraPosition(shootAction);
                float disBetweenCamAndUnit = GetDistanceBetweenCameraAndTarget(shootAction, cameraPosition);
                if (disBetweenCamAndUnit > disBetweenUnits)
                {
                    SettingShootShoulderCamera(shootAction);
                }
                if (disBetweenCamAndUnit < disBetweenUnits)
                {
                    SettingShootBetweenCamera(shootAction);
                }

                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void SettingShootShoulderCamera(ShootAction action)
    {
        Unit actingUnit = action.GetUnit();
        Unit targetUnit = action.GetTargetUnit();
        Transform cameraPosition = actingUnit.transform.GetChild(3);
        actionCameraGameObject.transform.position = cameraPosition.position;
        Vector3 cameraDirection = new Vector3(targetUnit.transform.position.x, cameraPosition.position.y, targetUnit.transform.position.z);
        actionCameraGameObject.transform.LookAt(cameraDirection);
    }

    private void SettingShootBetweenCamera(ShootAction action)
    {
        Unit actingUnit = action.GetUnit();
        Unit targetUnit = action.GetTargetUnit();
        Transform cameraPosition = actingUnit.transform.GetChild(3);
        actionCameraGameObject.transform.position = cameraPosition.position;
        Vector3 cameraOnActingUnit = new Vector3(actingUnit.transform.position.x, cameraPosition.position.y, actingUnit.transform.position.z);
        Vector3 cameraOnTargetUnit = new Vector3(targetUnit.transform.position.x, cameraPosition.position.y, targetUnit.transform.position.z);
        rotateCamera = true;
    }

    private float GetDistanceBetweenUnits(ShootAction action)
    {
        Unit actingUnit = action.GetUnit();
        Unit targetUnit = action.GetTargetUnit();
        float distance = Vector3.Distance(actingUnit.transform.position, targetUnit.transform.position);
        return distance;
    }

    private Transform GetCameraPosition(ShootAction action)
    {
        Unit actingUnit = action.GetUnit();
        Transform cameraPosition = actingUnit.transform.GetChild(3);
        return cameraPosition;
    }

    private float GetDistanceBetweenCameraAndTarget(ShootAction action, Transform cameraTransform)
    {
        Unit actingUnit = action.GetUnit();
        Unit targetUnit = action.GetTargetUnit();
        Vector3 a = cameraTransform.position - actingUnit.transform.position;
        Vector3 c = targetUnit.transform.position - actingUnit.transform.position;
        Vector3 desiredPosition = actingUnit.transform.position + Vector3.Project(a, c.normalized);
        float distance = Vector3.Distance(desiredPosition, targetUnit.transform.position);
        return distance;
    }

    private void RotateCameraShootAction(ShootAction action)
    {

    }
}
