using UnityEngine;

public class FireAttackUnit : AttackUnit
{
    [Header("Fire Unit Properties")]
    [Range(1, 5)] public int bulletsShotAtOnce = 1;             // Bullets that come out of nozzle at once
    [Range(0.0f, 20f)] public float horizontalSpread = 10.0f;   // Horizontal spread of bullets in degrees
    
    protected override void ShootAtTarget()
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
}


