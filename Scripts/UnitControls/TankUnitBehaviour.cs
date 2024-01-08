using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUnitBehaviour : MonoBehaviour, IUnitActionInterface
{
    // Start is call   public GameObject explosion;
    public float turnSpeed;
    public float acceleration;
    public float dampening;

    GameObject explosionCopy;
    public Transform turret;
    public GameObject LOD1Turret;
    Renderer LOD1TurretRenderer;
    Transform LOD1TurretTransform;
    public Transform gun;
    public float turretRotationSpeed;
    UnitBehaviour unitBehaviour;
    JobManager jobManager;
    float currentGunCharge = 0;
    public float reloadtime;
    GameObject currentTarget;
    public bool hasTurret;
    float updateTimestep;
    float elapsed = 0;
    int currentOrder = 0;
    int LookForTargetsCounter = 20;
    int LookForTargetsCounterMax = 20;
    Unit thisUnit;
    int unitRange;
    int turretNumber;
    float recharge = 0;
    public ParticleSystem muzzleFlash;
    public ParticleSystem explosion;
    public ParticleSystem shot;
    public ParticleSystem die;

    public bool canShoot;
    Vector3 currentMoveTarget;
    int vehicleNumber;

    public GameObject driver;
    Animator animator;
    GameObject player;

    bool isMoving;
    Vector3 velocity;
    float moveSpeed;

    bool dead = false;
    bool hasTarget = false;
    UnitBehaviour targetUnitBehaviour;
    float distance;
    float targetCenterY;
    Vector3 directionToTarget;
    bool targetUnderDirectControl;
    int distanceCheckCount = 0;
    float currentAccuracy;
    public float accuracy;
    public int damagePerShot;
    float spread;
    public float shotSpeed;
    GlobalGameInformation gameInformation;


    public void UpdateUnit() {
        if (unitBehaviour.hasTarget)
        {
            if (currentTarget != null)
            {
                ShootAtTarget(currentTarget);
            }
            else
            {
                unitBehaviour.TargetGone();
                jobManager.LookForTarget(thisUnit, unitRange);
            }
    
        }
        switch (currentOrder)
        {
            case 1:
            Move(currentMoveTarget);
            LookForTargetsCounter++;
            break;
            case 2:
            Build(currentTarget);
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
            break;
        }
        if (LookForTargetsCounter >= LookForTargetsCounterMax)
        {
            LookForTargetsCounter = 0;

            if (!unitBehaviour.hasTarget)
            {
                jobManager.LookForTarget(thisUnit, unitRange);

            }
            else
            {
                if (LOD1TurretRenderer.isVisible)
                {
                    LOD1TurretTransform.rotation = turret.transform.rotation;
                }

            }
        }

    }
        void Start() {

        thisUnit = unitBehaviour.thisUnit;
        updateTimestep = jobManager.updateTimestep;
        vehicleNumber = unitBehaviour.vehicleNumber;
        turretNumber = unitBehaviour.turretNumber;
        jobManager.LookForTarget(thisUnit, unitRange);
       // unitBehaviour.addOrderToQueue(new Order(1,new Vector3 (transform.position.x+80,0,transform.position.z+80),true,true));

        spread = 1 / accuracy;
    }
    void Awake() {


        LOD1TurretRenderer = LOD1Turret.GetComponent<Renderer>();
        LOD1TurretTransform = LOD1Turret.transform;
        player = GameObject.Find("/XR Rig");
        // unitBehaviour = gameObject.GetComponent<UnitBehaviour>();



        isMoving = false;
        velocity = new Vector3(0, 0, 0);
        unitBehaviour = gameObject.GetComponent<UnitBehaviour>();   
        //vr stuff
        /* vrTargetLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
         vrTargetRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetRightHand").transform;


         rigTargetRightHand = transform.Find("LOD0/VR Constraints/Right Arm IK/Target");
         rigTargetLeftHand = transform.Find("LOD0/VR Constraints/Left Arm IK/Target");
         rigTargetHead = transform.Find("LOD0/Armature/Root/Spine1/Head");
         */
        unitRange = unitBehaviour.unitRange;


        moveSpeed = unitBehaviour.moveSpeed;
        jobManager = player.GetComponent<JobManager>();

        jobManager.AddTurretToMove(gameObject, turret, gun, turretRotationSpeed);
        jobManager.AddVehicleToMove(transform, moveSpeed, turnSpeed, acceleration);
    }

    public void ShootAtTarget(GameObject target) {
        if (currentTarget != target)
        {
            hasTarget = true;
            unitBehaviour.hasTarget = true;
            targetUnitBehaviour = target.GetComponent<UnitBehaviour>();
            currentTarget = target;
            distance = Vector3.Distance(transform.position, target.transform.position);
            targetCenterY = targetUnitBehaviour.headHeight / 2;
            directionToTarget = new Vector3(target.transform.position.x, 0, target.transform.position.z) - transform.position;
            targetUnderDirectControl = targetUnitBehaviour.assumingDirectControl;

        }
        distanceCheckCount++;

        if (distanceCheckCount > 30)
        {

            distanceCheckCount = 0;
            distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > unitRange || targetUnitBehaviour.hp <= 0 || !target.activeInHierarchy)
            {
               // currentTarget = null;
               // unitBehaviour.hasTarget = false;
                jobManager.LookForTarget(thisUnit, unitRange);
                return;
            }

            directionToTarget = -(transform.position - new Vector3(target.transform.position.x, targetCenterY, target.transform.position.z));


        }

        recharge += jobManager.updateTimestep;

        if (recharge > reloadtime)
        {

            if (unitBehaviour.canFire)
            {
                recharge = 0;
                muzzleFlash.Play();
                Vector3 hitPoint = new Vector3(target.transform.position.x + Random.Range(-2f-spread, 2f + spread) , 0, target.transform.position.z + Random.Range(-2f - spread, 2f + spread));
                jobManager.newProjectile(shot, explosion, shot.transform.position, hitPoint, null, 5, damagePerShot, shotSpeed, false);

            }
        }
        

    }
    public void Attack(GameObject target) {
       // Debug.Log("tank got attack order in tankunitbeh");

        if (currentTarget != target)
        {
            hasTarget = true;
            unitBehaviour.hasTarget = true;
            targetUnitBehaviour = target.GetComponent<UnitBehaviour>();
            currentTarget = target;
            distance = Vector3.Distance(transform.position, target.transform.position);
            targetCenterY = targetUnitBehaviour.headHeight / 2;
            directionToTarget = new Vector3(target.transform.position.x, 0, target.transform.position.z) - transform.position;
            targetUnderDirectControl = targetUnitBehaviour.assumingDirectControl;
            jobManager.MoveTurret(turretNumber, target.transform);


        }
        unitBehaviour.OrderComplete();




    }






  
    public void BuildUnit(GameObject unit) {
    }
    public void ExtractOre() {
    }
    public void UpdateScaffold() {
    }
    public void CollisionEnter(Collider collision) {

    }
    public void CollisionExit(Collider collision) {


    }
    public void Damage() {
     //   animator.SetTrigger("Hit");


    }
    public void Die() {
        if (!dead)
        {
            die.Play();
            Invoke("Destroy", 2);
            unitBehaviour.enabled = false;
            dead = true;
        }
    }

    void Destroy() {

        GameObject.Destroy(gameObject);

    }
    public void CollisionStay(Collider collision) {
        if (collision.gameObject.layer == 7)
        {
            transform.position -= (collision.gameObject.transform.position - transform.position).normalized;
            velocity *= 0.2f;
        }
    }
    public void EnterDirectControl() {
        currentOrder = 0;
        unitBehaviour.assumingDirectControl = true;

       
     //   driver.GetComponent<RigBuilder>().enabled = true;
      //  rigTargetHead.localScale = new Vector3(0, 0, 0);

      //  player.transform.position = rigTargetHead.position + new Vector3(0, 0.3f, 0);
        gameObject.GetComponent<UnitBehaviour>().OrderComplete();

        gameObject.GetComponent<UnitCollision>().enabled = true;

        Stop();

    }
    public void ExitDirectControl() {
        currentOrder = 0;
        Stop();

        gameObject.GetComponent<UnitCollision>().enabled = false;

       // driver.GetComponent<RigBuilder>().enabled = false;

        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = false;

      //  rigTargetHead.localScale = new Vector3(1, 1, 1);

        unitBehaviour.OrderComplete();

    }
    public void UnderDirectControl() {

      
    }

    public void Build(GameObject building) {
    }




    public void Move(Vector3 movetarget, bool finalDestination = false) {
        //  Debug.Log("tank got move order");

        if (movetarget != currentMoveTarget || currentOrder != 1) // if the previous order wasn't a move order or it is a move order to a new location
        {
             Debug.Log("in TankUnitBehaviour, tank got move order from " + transform.position + " to " + movetarget);
           
            
            currentMoveTarget = movetarget;


            jobManager.MoveVehicle(vehicleNumber, movetarget, finalDestination);

        //    Debug.Log("tank got move order2");

            currentOrder = 1;

        }
        if (unitBehaviour.hasTarget)
        {
            ShootAtTarget(currentTarget);
        }
            //  transform.position += moveSpeed * transform.forward * updateTimestep;

        }
        // Update is called once per frame
        public void Stop() {
       // jobManager.StopMovingVehicle(vehicleNumber);
       // jobManager.StopMovingTurret(turretNumber);
        unitBehaviour.ClearAllOrders();
        currentOrder = 0;
    }
}
