using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone;

    private Vector3 hitPosition;
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();


        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
        unitRagdoll.Setup(originalRootBone);
        Transform unitRagdollRoot = unitRagdoll.GetRagdollRootBone();
        unitRagdoll.ApplyExplosionToRagdoll(unitRagdollRoot, 250f, hitPosition, 10f);
    }

    public void SetHitPosition(Vector3 hitPositionToSet)
    {
        hitPosition = hitPositionToSet;
    }

}
