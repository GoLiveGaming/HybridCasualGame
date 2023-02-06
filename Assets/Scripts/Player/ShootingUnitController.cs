using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainPlayerControl;

public class ShootingUnitController : MonoBehaviour
{
    [SerializeField] public List<NPCManagerScript> targetsInRange = new List<NPCManagerScript>();
    [SerializeField] private TurretState turretState;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Range(0.01f, 10f)] private float delayBetweenShots = 0.5f;
    [SerializeField, Range(0.1f, 100f)] private float shootingRange = 10f;

    private Transform targetTF;
    private float lastFireTime = 0f;

    [SerializeField, Range(0.01f, 10f)] private float targetsRefreshAfter = 2f;
    private float timeSinceTargetsRefresh = 0;

    private float dist;
    private bool dragging = false;
    private Vector3 offset;
    private Transform toDrag;
    private Vector3 toDrags;

    internal Vector3 initialPos;

    private enum TurretState
    {
        Idle,
        Track,
        Attack
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, new Vector3(2, 2, 2));

    }
    private void Start()
    {
        initialPos = this.transform.position;
        turretState = TurretState.Idle;
    }
    private void FixedUpdate()
    {
        UpdateTurret();
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
                if (hitCollider.CompareTag("TargetEnemy"))
                {
                    hitCollider.TryGetComponent(out NPCManagerScript target);
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

            foreach (NPCManagerScript targetObj in targetsInRange)
            {
                GameObject enemy;
                if (targetObj != null)
                {
                    enemy = targetObj.gameObjectSelf;

                    float distance = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distance < closestDistance && distance <= shootingRange)
                    {
                        closestDistance = distance;
                        targetTF = enemy.transform;
                    }
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
    void Update()
    {

        Vector3 v3;

        if (Input.touchCount != 1)
        {
            dragging = false;
            return;
        }

        Touch touch = Input.touches[0];
        Vector3 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    toDrag = hit.transform;
                    dist = hit.transform.position.z - Camera.main.transform.position.z;
                    v3 = new Vector3(pos.x, pos.y, dist);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    offset = toDrag.position - v3;
                    dragging = true;
                }
            }
        }

        if (dragging && touch.phase == TouchPhase.Moved)
        {
            v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            v3 = Camera.main.ScreenToWorldPoint(v3);
            toDrag.position = v3 + offset;
        }

        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
        }
    }
    Vector3 mousePosition;

    Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
        //toDrags = Input.mousePosition;
        //dist = Input.mousePosition.z - Camera.main.transform.position.z;
        //v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        //v3 = Camera.main.ScreenToWorldPoint(v3);
        //offset = toDrags - v3;

    }
    private void OnMouseDrag()
    {
        Vector3 aa = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(aa.x, transform.position.y, aa.z);
        //v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        //v3 = Camera.main.ScreenToWorldPoint(v3);
        //toDrags = v3 + offset;

    }

    private void OnMouseUp()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(1, 1, 1));
        if (hitColliders.Length > 1)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Player") && hitColliders[i].gameObject != this.gameObject)
                {
                    Destroy(this.gameObject);
                }
            }
            transform.position = initialPos;

        }
        else
            transform.position = initialPos;
    }
}
