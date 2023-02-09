using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AttackUnit), typeof(Stats))]
public class PlayerTower : MonoBehaviour
{
    [Header("ReadOnly")]
    [ReadOnly] public TowerState towerState; [Space(2)]

    [Header("Attack Properties")]
    public AttackUnit attackUnit; [Space(2)]

    [Header("Tower Properties")]
    [SerializeField, Range(1, 5)] private int _towerLevel = 1;
    [Range(0.01f, 10f)] public float upgradeDecreasedShootDelay = 0.1f;
    [HideInInspector] public MainPlayerControl mainPlayerControl;
    [HideInInspector] public Stats stats;

    private float dist;
    private bool dragging = false;
    private Vector3 offset;
    private Transform toDrag;
    private Vector3 toDrags;
    internal Vector3 initialPos;

    public enum TowerState
    {
        Idle,
        Attack,
        Destroyed
    }

    void OnDrawGizmosSelected()
    {
        if (!attackUnit) attackUnit = GetComponent<AttackUnit>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackUnit.shootingRange);
    }
    private void OnEnable()
    {
        if (mainPlayerControl)
            mainPlayerControl.activePlayerTowersList.Add(this);
    }
    private void OnDisable()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        mainPlayerControl.activePlayerTowersList.Remove(this);
    }
    private void Awake()
    {
        attackUnit = GetComponent<AttackUnit>();
        stats = GetComponent<Stats>();

        initialPos = transform.position;
        towerState = TowerState.Idle;
    }
    private void Start()
    {
        if (!mainPlayerControl) mainPlayerControl = MainPlayerControl.Instance;
        mainPlayerControl.activePlayerTowersList.Add(this);
        stats.SetCurrentLeveltext(_towerLevel);
    }

    private void Update()
    {
        UpdateTower();
        UpdateTouch();
    }
    public void UpdateTower()
    {
        UpdateTowerState();
        attackUnit.UpdateUnit();
    }
    private void UpdateTowerState()
    {
        //Switch Between States of turret 
        if (attackUnit.targetsInRange.Count > 0 && towerState != TowerState.Attack)
        {
            towerState = TowerState.Attack;
        }
        else if (attackUnit.targetsInRange.Count == 0 && towerState != TowerState.Idle)
        {
            towerState = TowerState.Idle;
        }
    }


    public void UpgradeTower()
    {
        _towerLevel += 1;
        Mathf.Clamp(_towerLevel, 1, 5);
        stats.SetCurrentLeveltext(_towerLevel);

        attackUnit.UpdateDelayBetweenShots(_towerLevel);
    }
    //MOVE THIS TO MAIN PLAYER CONTROL
    void UpdateTouch()
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
