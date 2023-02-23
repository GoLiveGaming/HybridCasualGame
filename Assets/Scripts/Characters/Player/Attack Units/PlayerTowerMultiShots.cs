using System;
using System.Collections;
using UnityEngine;

public class PlayerTowerMultiShots : PlayerTower
{
    [Space(2), Header("ATTACK UNIT EXTENDED")]
    [SerializeField] private ShotType shotType;


    [Header("Burst Shot Properties")]
    [SerializeField, Range(1f, 10f)] private int burstCount = 3;
    [SerializeField, Range(0.01f, 2f)] private float burstInterval = 0.1f;


    [Header("Spread Shot Properties")]

    [SerializeField, Range(1, 5)] private int bulletsShotAtOnce = 1;             // Bullets that come out of nozzle at once
    [SerializeField, Range(0.0f, 20f)] private float horizontalSpread = 10.0f;   // Horizontal spread of bullets in degrees

    protected override void ShootAtTarget()
    {
        if (shotType == ShotType.BurstShot) StartCoroutine(FireBurstShot());
        else FireSpreadShot();
    }
    IEnumerator FireBurstShot()
    {
        Vector3 bulletSpawnPos = turretMuzzleTF ? turretMuzzleTF.position : transform.position;

        for (int i = 0; i < burstCount; i++)
        {
            if (targetTF == null) break;
            GameObject bullet = Instantiate(attackBulletPrefab, bulletSpawnPos, transform.rotation);
            bullet.GetComponent<Bullet>().InitializeBullet(targetTF.position);

            yield return new WaitForSeconds(burstInterval);
        }

    }

    void FireSpreadShot()
    {
        for (int i = 0; i < bulletsShotAtOnce; i++)
        {
            // Calculate the spread for each bullet
            float spread = (horizontalSpread / 2.0f) - ((horizontalSpread / (bulletsShotAtOnce - 1)) * i);

            // Create a new bullet
            GameObject bullet = Instantiate(attackBulletPrefab, transform.position + new Vector3(0, 0f, 0), transform.rotation);

            // Rotate the bullet by the spread
            bullet.transform.Rotate(Vector3.forward, spread);

            Vector3 OffsetPos = targetTF.transform.position;
            OffsetPos.x += spread;

            bullet.GetComponent<Bullet>().InitializeBullet(OffsetPos);
        }
    }

    private enum ShotType
    {
        BurstShot,
        SpreadShot
    }
}