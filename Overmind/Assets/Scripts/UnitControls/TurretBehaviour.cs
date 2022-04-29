using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour, IUnitActionInterface
{
    public Transform turret;
    public Transform gun;
    public GameObject muzzleflash;
    public float rotationSpeed;
    UnitBehaviour unitBehaviour;
    GameObject player;
    JobManager jobManager;
    float currentGunCharge = 0;
    public float reloadtime;
    float elapsed = 0;
    float updateTimestep = 1;
    int currentOrder;
    GameObject currentTarget;
    int LookForTargetsCounter = 0;
    int LookForTargetsCounterMax = 2;
    Unit thisUnit;
    int unitRange;
    int turretNumber;
    float recharge = 0;
    public float rechargeTime;
    public ParticleSystem muzzleFlash;

    void Update() // updates every updateTimeStep, initally set by updateFPS in GlobalGameInformation. This way the framerate of units can be increased or decreased depending on current performance.
    {
        // and there are orders in the orderQueue
        elapsed += Time.deltaTime;
        if (elapsed >= updateTimestep) //run the order every updateTimestep using the UpdateUnit method
        {
            elapsed = 0;
            switch (currentOrder)
            {
                case 1:
               // Move(currentMoveTarget);
                break;
                case 2:
               // Build(currentTarget);
                break;
                case 3:
                Attack(currentTarget);
                break;
                case 4:
                Stop();
                break;
                case 5:
                ExtractOre();
                break;
                case 6:
                UpdateScaffold();
                break;
                case 7:
                EnterDirectControl();
                break;
                case 8:
                ExitDirectControl();
                break;
                case 9:
                UnderDirectControl();
                break;
                case 0:
                LookForTargetsCounter++;
                break;
                default:
                LookForTargetsCounter++;
                unitBehaviour.OrderComplete();
                currentOrder = 0;
                break;
            }
            if (LookForTargetsCounter > LookForTargetsCounterMax)
            {
                if (!unitBehaviour.attackMoveEnabled)
                {
                    jobManager.LookForTarget(thisUnit, unitRange);
                }
            }
        }
    }
    void Start() {
        thisUnit = unitBehaviour.thisUnit;
        unitRange = unitBehaviour.unitRange;
        turretNumber = unitBehaviour.turretNumber;
    }
    private void Awake() {
        player = GameObject.Find("/XR Rig");
        jobManager = player.GetComponent<JobManager>();
        unitBehaviour = gameObject.GetComponent<UnitBehaviour>();

    }
    // Start is called before the first frame update
    public void Move(Vector3 movetarget){}
    public void Build(GameObject building){}
    public void Attack(GameObject target){
        if (currentTarget != target)
        {
            jobManager.MoveTurret(turretNumber, target.transform);
        
        }
        recharge += updateTimestep;
        if (recharge > rechargeTime)
        {
            muzzleFlash.Play();


        }
    }
    public void Stop() {
    }
    public void ExtractOre(){}
    public void UpdateScaffold(){}
    public void EnterDirectControl(){}
    public void ExitDirectControl(){}
    public void UnderDirectControl(){}
    public void Die(){}
    public void Damage(){}
    public void CollisionEnter(Collider collision){}
    public void CollisionExit(Collider collision){}
    public void CollisionStay(Collider collision){}
}
