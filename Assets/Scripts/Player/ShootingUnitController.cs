using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShootingUnitController : MonoBehaviour
{
    [SerializeField] private List<TargetEnemy> targetsInRange = new List<TargetEnemy>();
    [SerializeField] private TurretState turretState;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Range(0.01f, 10f)] private float delayBetweenShots = 0.5f;
    [SerializeField, Range(0.1f, 100f)] private float shootingRange = 10f;

    private Transform targetTF;
    private float lastFireTime = 0f;

    [SerializeField, Range(0.01f, 10f)] private float targetsRefreshAfter = 2f;
    private float timeSinceTargetsRefresh = 0;

    private enum TurretState
    {
        Idle,
        Track,
        Attack
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shootingRange);

    }
    private void Start()
    {
        turretState = TurretState.Idle;
    }

    public void UpdateTurret()
    {
        UpdateTurretState();

        if (turretState == TurretState.Idle) TurretIdleAction();
        else if (turretState == TurretState.Attack) TurretAttackAction();
    }
    private void UpdateTurretState()
    {
        RefreshTargets();

        //Switch Between States of turret 
        if (targetsInRange.Count > 0 && turretState != TurretState.Attack)
        {
            turretState = TurretState.Attack;
        }
        else if (targetsInRange.Count == 0 && turretState != TurretState.Idle)
        {
            turretState = TurretState.Idle;
        }
    }

    void RefreshTargets()
    {
        // Refresh the target
        timeSinceTargetsRefresh += Time.deltaTime;
        if (targetsRefreshAfter < timeSinceTargetsRefresh)
        {
            targetTF = null;
            targetsInRange.Clear();
            timeSinceTargetsRefresh = 0;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootingRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("TurretTarget"))
                {
                    hitCollider.TryGetComponent(out TargetEnemy target);
                    if (target) targetsInRange.Add(target);
                }
            }
        }
    }

    private void TurretIdleAction()
    {

    }
    private void TurretAttackAction()
    {

        // Look for targets within shooting range
        if (targetTF == null)
        {
            float closestDistance = Mathf.Infinity;

            foreach (TargetEnemy targetObj in targetsInRange)
            {
                GameObject enemy = targetObj.gameObjectSelf;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance && distance <= shootingRange)
                {
                    closestDistance = distance;
                    targetTF = enemy.transform;
                }
            }
        }

        // Shoot at the target if found
        if (targetTF != null)
        {
            if (lastFireTime > delayBetweenShots)
            {
                lastFireTime = 0f;
                ShootAtTarget();
            }
            else lastFireTime += Time.deltaTime;
        }
    }

    void ShootAtTarget()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Bullet>().initializeBullet(targetTF);
    }
}
