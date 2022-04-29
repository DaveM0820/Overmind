using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUnitBehaviour : MonoBehaviour, IUnitActionInterface
{
    // Start is call   public GameObject explosion;
    GameObject explosionCopy;
    public Transform turret;
    public Transform gun;
    public float turretRotationSpeed;
    UnitBehaviour unitBehaviour;
    JobManager jobManager;
    float currentGunCharge = 0;
    public float reloadtime;
    GameObject currentTarget;
    public bool hasTurret;
    float updateTimestep = 0.02f;
    float elapsed;
    int currentOrder = 0;
    int LookForTargetsCounter = 0;
    int LookForTargetsCounterMax = 2;
    Unit thisUnit;
    int unitRange;
    int turretNumber;
    float recharge = 0;
    public float rechargeTime;
    public ParticleSystem muzzleFlash;
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
            if (LookForTargetsCounter > LookForTargetsCounterMax)
            {
              
                    jobManager.LookForTarget(thisUnit, unitRange);

                
            }
        }
    }


    void Start() {

        thisUnit = unitBehaviour.thisUnit;
        updateTimestep = unitBehaviour.updateTimestep;
        vehicleNumber = unitBehaviour.vehicleNumber;
        turretNumber = unitBehaviour.turretNumber;

    }
    void Awake() {


        animator = driver.GetComponent<Animator>();

        player = GameObject.Find("/XR Rig");
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        // unitBehaviour = gameObject.GetComponent<UnitBehaviour>();



        animator = driver.gameObject.GetComponent<Animator>();
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

    }


    public void Attack(GameObject target) {

        if (currentTarget != target)
        {
            currentTarget = target;
            jobManager.MoveTurret(turretNumber, target.transform);

        }
        unitBehaviour.OrderComplete();

        recharge += updateTimestep;
 




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
        animator.SetTrigger("Hit");


    }
    public void Die() {
        if (!dead)
        {

            int anim = UnityEngine.Random.Range(1, 4);
            animator.SetInteger("Dying", anim);
            Invoke("Destroy", 10);
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
        updateTimestep = Time.deltaTime;
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
        updateTimestep = unitBehaviour.updateTimestep;
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




    public void Move(Vector3 movetarget) {
        if (movetarget != currentMoveTarget || currentOrder != 1) // if the previous order wasn't a move order or it is a move order to a new location
        {
            // Debug.Log("got move order from " + transform.position + " to " + movetarget);
            currentMoveTarget = movetarget;


          //  jobManager.moveVehicle();
            currentOrder = 1;

        }
        //  transform.position += moveSpeed * transform.forward * updateTimestep;

    }
    // Update is called once per frame
    public void Stop() {

        unitBehaviour.orderQueue.Clear();

    }
}
