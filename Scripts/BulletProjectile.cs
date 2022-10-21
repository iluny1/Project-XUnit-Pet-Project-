using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVFXPrefab;


    private Vector3 targetPosition;
    private Vector3 hitPoint;
    private Unit targetUnit;

    public event EventHandler OnBulletEnd;

    public void Setup(Vector3 targetPosition, Unit targetUnit)
    {
        this.targetPosition = targetPosition;
        this.targetUnit = targetUnit;
        CalculatingHit();
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float distanceFromTarget = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 50f;

        if (distanceFromTarget <= moveSpeed)
        {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            OnBulletEnd?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
            Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.Euler(moveDir));
        }
        else
        {
            transform.position += (moveSpeed * moveDir) * Time.deltaTime;
        }
    }    

    private void CalculatingHit()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, targetPosition);
        hitPoint = Vector3.MoveTowards(transform.position, hitPoint, distanceFromTarget - 0.1f);
        targetUnit.GetComponent<UnitRagdollSpawner>().SetHitPosition(hitPoint);
    }
}
