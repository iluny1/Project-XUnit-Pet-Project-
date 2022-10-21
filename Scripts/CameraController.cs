using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cVirtualCamera;

    private float moveSpeed = 75f;
    private const float MIN_Y_FOLLOW_OFFSET = 1.5f;
    private const float MAX_Y_FOLLOW_OFFSET = 10f;
    private const float MIN_Z_FOLLOW_OFFSET = -10f;
    private const float MAX_Z_FOLLOW_OFFSET = -4f;
    private bool isMovingToUnit;
    private CinemachineTransposer cTransposer;
    private Vector3 targetFollowOffset;

    private void Awake()
    {
        isMovingToUnit = false;
    }

    private void Start()
    {
        cTransposer = cVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cTransposer.m_FollowOffset;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
    }

    private void Update()
    {
        if (!isMovingToUnit)
        {
            HandleMovement();
        }
        HandleRotation();
        HandleZoom();
        MoveToSelectedUnit_Logic();
    }

    private void HandleMovement()
    {
        Vector3 InputMoveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            InputMoveDir.z += .1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            InputMoveDir.z -= .1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            InputMoveDir.x -= .1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            InputMoveDir.x += .1f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed *= 2f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed /= 2f;
        }

        Vector3 moveVector = transform.forward * InputMoveDir.z + transform.right * InputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y += 1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y -= 1f;
        }

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomAmount = 1f;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
            if (targetFollowOffset.y >= 4f) targetFollowOffset.z += zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
            if (targetFollowOffset.y >= 4f) targetFollowOffset.z -= zoomAmount;

        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_Y_FOLLOW_OFFSET, MAX_Y_FOLLOW_OFFSET);
        targetFollowOffset.z = Mathf.Clamp(targetFollowOffset.z, MIN_Z_FOLLOW_OFFSET, MAX_Z_FOLLOW_OFFSET);

        float zoomSpeed = 5f;
        cTransposer.m_FollowOffset = Vector3.Lerp(cTransposer.m_FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime);

    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        isMovingToUnit = true;
    }

    private void MoveToSelectedUnit_Logic()
    {
        if (isMovingToUnit)
            MoveToSelectedUnit();
    }

    private void MoveToSelectedUnit()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        GridPosition targetGridPosition = selectedUnit.GetGridPosition();
        Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        float moveToUnitSpeed = 5f;
        transform.position = Vector3.Lerp(transform.position, targetWorldPosition, moveToUnitSpeed * Time.deltaTime);

        if (MathF.Round(transform.position.x, 1) == MathF.Round(targetWorldPosition.x, 1) &&
            MathF.Round(transform.position.z, 1) == MathF.Round(targetWorldPosition.z, 1))
            isMovingToUnit = false;

    }
}
