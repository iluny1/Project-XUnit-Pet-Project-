using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static ShootAction;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private ParticleSystem gunFlareParticle;
    [SerializeField] private AudioSource shootingAR;    
    [SerializeField] private float shots;
    [SerializeField] private float gunRateOfFire;

    private OnShootActionEventArgs onShootActionEventArgs;
    private float gunRecoil = 0.1f;
    private float shootTimerCurrent;
    private float shootTimerConstant;
    private float shootTimerAll;
    private bool isShooting;    

    public float second;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += moveAction_OnStartMoving;
            moveAction.OnStopMoving += moveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShootAction += shootAction_OnShootAction;
        }

        if (TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.OnDead += healthSystem_OnDead;
        }
        
        float secondsInMinute = 60f;
        shootTimerConstant = secondsInMinute / gunRateOfFire;        
        shootTimerCurrent = shootTimerConstant;  
        isShooting = false;        
    }

    private void Update()
    {
        if (isShooting == true)
        {
            //Time.timeScale = 0.3f;
            if (shootTimerCurrent == 0f)
            {
                BulletProjectileShooting(onShootActionEventArgs);
                shootTimerCurrent = shootTimerConstant;
            }
            else
            {
                shootTimerCurrent = shootTimerCurrent - 1f * Time.deltaTime;
                if (shootTimerCurrent < 0f) shootTimerCurrent = 0f;
            }
            shootTimerAll = shootTimerAll - 1f * Time.deltaTime;

            if (shootTimerAll <= 0f)
            {
                isShooting = false;
                //Time.timeScale = 1f;
            }
        }
    }

    private void moveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void moveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void shootAction_OnShootAction(object sender, ShootAction.OnShootActionEventArgs e)
    {

        onShootActionEventArgs = e;
        shootTimerAll = shootTimerConstant * shots;        
        isShooting = true;
        e.targetUnit.GetComponent<HealthSystem>();
    }

    private void BulletProjectileShooting(OnShootActionEventArgs e)
    {
        if (e.targetUnit == null)
        {
            isShooting=false;
            shootTimerAll=0;
            return;
        }
        
        gunFlareParticle.Play();
        shootingAR.Play();
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y + UnityEngine.Random.Range(-gunRecoil, gunRecoil);
        targetUnitShootAtPosition.x = targetUnitShootAtPosition.x + UnityEngine.Random.Range(-gunRecoil, gunRecoil);
        targetUnitShootAtPosition.z = targetUnitShootAtPosition.z + UnityEngine.Random.Range(-gunRecoil, gunRecoil);
        bulletProjectile.Setup(targetUnitShootAtPosition, e.targetUnit);
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        animator.SetTrigger("Die");
    }
}
