using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Unity.Jobs;

/*
public struct InfantryAttack : IJob
{
    public GameObject currentTarget;
            public GameObject target;
            public Animator animator;
            public UnitBehaviour targetUnitBehaviour;
            public float distance;
            public Transform transform;
            public float targetCenterY;
            public Vector3 directionToTarget;
            public int distanceCheckCount;
            public bool targetUnderDirectControl;
            public float unitRange;
            public UnitBehaviour unitBehaviour;
            public float currentAccuracy;
            public float accuracy;
            public Transform barrel;
            public float evadeChance;
            public bool evading;
            public Vector3 evadeVector;
            public bool crouching;
            public Vector3 velocity;
            public float weaponRecharge;
            public float updateTimestep;
            public float weaponRechargeRate;
            public float rand;
            public GameObject rifleshot;
            public ParticleSystem muzzleFlash;
            public RifleShot rifleShotScript;
    public int damagePerShot;
    public GameObject[] hitEffectMetalArray;
    public ParticleSystem[] hitEffectMetalParticle;

    public GameObject[] hitEffectGroundArray;
    public ParticleSystem[] hitEffectGroundParticle;
    //public int number;
    public void Execute() {
        //  number++;


        if (currentTarget != target)
        {

            animator.SetBool("Attacking", true);

            targetUnitBehaviour = target.GetComponent<UnitBehaviour>();

            currentTarget = target;
            distance = Vector3.Distance(transform.position, target.transform.position);
            targetCenterY = targetUnitBehaviour.headHeight / 2;
            directionToTarget = new Vector3(target.transform.position.x, 0, target.transform.position.z) - transform.position;
            transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.Rotate(Vector3.up, 28);
            targetUnderDirectControl = targetUnitBehaviour.assumingDirectControl;

        }
        distanceCheckCount++;

        if (distanceCheckCount > 15)
        {
            if (!target.activeInHierarchy)
            {
                unitBehaviour.targetGone();

            }
            distanceCheckCount = 0;
            distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > unitRange || targetUnitBehaviour.hp <= 0)
            {
                unitBehaviour.targetGone();
            }
            if (distance < unitRange * 0.2f)
            {
                currentAccuracy = 0.9f;
            }
            else
            {
                currentAccuracy = accuracy;
            }
            if (targetUnitBehaviour.evading)
            {
                currentAccuracy *= 0.5f;
            }
            if (targetUnitBehaviour.isBuilding)
            {
                currentAccuracy = 1;

            }
            directionToTarget = -(transform.position - new Vector3(target.transform.position.x, targetCenterY, target.transform.position.z));

            rand = Random.Range(-0.5f, 0.5f);
            transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.Rotate(Vector3.up, 25);
            if (UnityEngine.Random.Range(0f, 1f) > 0.5)
            {

                evading = true;
                int random = Random.Range(0, 6);
                evadeVector = new Vector3(0, 0, 0);
                unitBehaviour.evading = true;

                if (!crouching)
                {

                    if (random == 1)
                    {
                        evadeVector = new Vector3(0.15f, 0, 0);
                        animator.SetTrigger("StrafeRight");

                    }
                    else if (random == 2)
                    {
                        evadeVector = new Vector3(-0.15f, 0, 0);
                        animator.SetTrigger("StrafeLeft");


                    }
                    else if (random == 3)
                    {
                        ///   evadeVector = new Vector3(0, 0, -0.10f);
                        //animator.SetTrigger("StrafeBack");

                    }
                    else if (random == 4)
                    {
                        evadeVector = new Vector3(0, 0, 0.2f);

                        animator.SetTrigger("StrafeForward");
                    }
                    else if (random == 5)
                    {
                        crouching = true;
                        animator.SetBool("Crouching", true);

                    }
                }
                else
                {

                    if (random == 1)
                    {
                        evadeVector = new Vector3(-0.15f, 0, 0);
                        animator.SetTrigger("StrafeRight");

                    }
                    else if (random == 2)
                    {
                        evadeVector = new Vector3(0.15f, 0, 0);
                        animator.SetTrigger("StrafeLeft");


                    }
                    else if (random == 3)
                    {
                        //    evadeVector = new Vector3(0, 0, -0.10f);
                        //   animator.SetTrigger("StrafeBack");

                    }
                    else if (random == 4)
                    {
                        //   evadeVector = new Vector3(0, 0, -0.20f);

                        //  animator.SetTrigger("StrafeForward");

                    }
                    else if (random == 5)
                    {
                        crouching = false;
                        animator.SetBool("Crouching", false);

                    }

                }

                velocity = transform.rotation * evadeVector;



            }

        }



        weaponRecharge += updateTimestep;
        if (weaponRecharge > weaponRechargeRate * rand)
        {

            muzzleFlash.Play();
            weaponRecharge = 0;


            if (Random.Range(0f, 1f) < accuracy)
            {

                for (int i = 0; i < 4; i++)
                {
                    if (!hitEffectMetalArray[i].activeInHierarchy)
                    {
                        hitEffectMetalArray[i].SetActive(true);
                        hitEffectMetalArray[i].transform.position = target.transform.position + new Vector3(Random.Range(0.2f * targetCenterY, 0.2f * targetCenterY), Random.Range(0.5f, targetCenterY * 2), Random.Range(0.2f * targetCenterY, 0.2f * targetCenterY));
                        hitEffectMetalArray[i].transform.forward = directionToTarget;
                        hitEffectMetalParticle[i].Play();
                        targetUnitBehaviour.changeHP(-damagePerShot);
                        if (targetUnitBehaviour.hp <= 0)
                        {
                            unitBehaviour.OrderComplete();
                            unitBehaviour.currentTarget = null;

                        }
                        i = 7;

                    }

                }

            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!hitEffectGroundArray[i].activeSelf)
                    {
                        hitEffectGroundArray[i].SetActive(true);
                        hitEffectGroundArray[i].transform.position = target.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                        hitEffectGroundParticle[i].Play();
                        i = 7;
                    }

                }
            }




        }


        if (evading)
        {
            transform.position += velocity;
            velocity *= 0.98f;
            if (velocity.magnitude <= 0.07f)
            {
                evading = false;
                unitBehaviour.evading = false;


            }
        }
    }

}


*/
public class InfantyUnitBehaviour : MonoBehaviour, IUnitActionInterface
{

    public bool currentlyBuilding;
    private GameObject building;
    private GameObject currentBuildingtoBuild;
    public int damagePerShot;
    bool isBuilding;
    GameObject player;
    Vector2 buidlingDimensions;
    private float reloadProgress;
    float updateTimestep;
    private Animator animator;
    private bool isMoving;
    public GameObject text;
    public Transform rigTargetRightHand;
    public Transform rigTargetLeftHand;
    public Transform rigTargetHead;
    public Transform rigTargetneck;
    public Transform vrTargetLeftHand;
    public Transform vrTargetRightHand;
    public Transform rigTargetRightHand2;
    public GameObject helmet;
    public GameObject unitMeshAndRig; public Transform headConstraint;
    public Vector3 headBodyOffset;
    public InputActionReference leftJoystick = null;
    public InputActionReference rightJoystick = null;
    public InputActionReference leftTrigger = null;
    public InputActionReference rightTrigger = null;
    JobHandle attackJob;
    public Vector2 leftJoystickValue;
    public Vector2 rightJoystickValue;
    float rightTriggerValue;
    public InputActionReference Abutton = null;
    float ButtonValue;
    public Vector3 velocity;
    public float rotationVelocity;
    public float dampening;
    public float rotationDampening;
    public bool snappedRight;
    public bool snappedLeft;
    public UnitBehaviour unitBehaviour;
    public GameObject rifleShotPrefab;
    GameObject[] rifleShots;
    RifleShot[] rifleShotScript;
    float weaponRecharge;
    public float weaponRechargeRate;
    public float accuracy;
    public ParticleSystem muzzleFlash;
    public Transform barrel;
    public GameObject gun;
    float distance;
    int distanceCheckCount;
    int unitRange;
    UnitBehaviour targetUnitBehaviour;
    GameObject currentTarget;
    float targetCenterY;
    float currentAccuracy;
    Vector3 directionToTarget;
    bool evading;
    Vector3 evadeVector;
    public float evadeChance;
    bool crouching = false;
    bool dead = false;
    Quaternion defaultGunRotation;
    bool targetUnderDirectControl;
    float rand = 1;
    float moveSpeed;
    Vector3 moveDirection;
    JobManager jobManager;
    public GameObject hitEffectMetal;
    public GameObject hitEffectGround;
    GameObject[] hitEffectMetalArray;
    GameObject[] hitEffectGroundArray;
    ParticleSystem[] hitEffectMetalParticle;
    ParticleSystem[] hitEffectGroundParticle;
    Vector3 currentMoveTarget;

    Unit thisUnit;
    int distanceCheckCounter = 0;
    float elapsed = 0;
    int currentOrder;
    bool hasTarget;
    int LookForTargetsCounter;
    int LookForTargetsCounterMax = 30;
    int transformNumber;
    // Start is called before the first frame update
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
                if (!unitBehaviour.attackMoveEnabled)
                {
                    jobManager.LookForTarget(thisUnit, unitRange);
                }
            }
        } 
    }


    void Start() {

        thisUnit = unitBehaviour.thisUnit;
        updateTimestep = unitBehaviour.updateTimestep;
        transformNumber = unitBehaviour.transformNumber;
    }
    void Awake() {

        hitEffectMetalArray = new GameObject[4];
        hitEffectGroundArray = new GameObject[4];
        hitEffectMetalParticle = new ParticleSystem[4];
        hitEffectGroundParticle = new ParticleSystem[4];
        unitMeshAndRig = transform.Find("LOD0").gameObject;
        animator = unitMeshAndRig.GetComponent<Animator>();

        player = GameObject.Find("/XR Rig");
        evadeChance = 0.98f;
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
       // unitBehaviour = gameObject.GetComponent<UnitBehaviour>();
      
 

        animator = transform.Find("LOD0").gameObject.GetComponent<Animator>();
        isMoving = false;
        velocity = new Vector3(0, 0, 0);
        //vr stuff
        vrTargetLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
        vrTargetRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetRightHand").transform;


        rigTargetRightHand = transform.Find("LOD0/VR Constraints/Right Arm IK/Target");
        rigTargetLeftHand = transform.Find("LOD0/VR Constraints/Left Arm IK/Target");
        rigTargetHead = transform.Find("LOD0/Armature/Root/Spine1/Head");
        unitRange = unitBehaviour.unitRange;
        defaultGunRotation = gun.transform.localRotation;
        moveSpeed = unitBehaviour.moveSpeed;
        jobManager = player.GetComponent<JobManager>();
        for (int i = 0; i < 4; i++)
        {
            hitEffectMetalArray[i] = Instantiate(hitEffectMetal, transform.root);
            hitEffectGroundArray[i] = Instantiate(hitEffectGround, transform.root);
            hitEffectMetalParticle[i] = hitEffectMetalArray[i].GetComponent<ParticleSystem>();
            hitEffectGroundParticle[i] = hitEffectGroundArray[i].GetComponent<ParticleSystem>();
            hitEffectMetalArray[i].SetActive(false);
            hitEffectGroundArray[i].SetActive(false);
        }
    }
    // private JobHandle InfantryAttackJob() {
    //      InfantryAttack job = new InfantryAttack();
    //     return job.Schedule();
    //  }

    public void Attack(GameObject target) {
        currentOrder = 3;
        if (currentTarget != target)
        {
            hasTarget = true;
            animator.SetBool("Attacking", true);

            targetUnitBehaviour = target.GetComponent<UnitBehaviour>();

            currentTarget = target;
            distance = Vector3.Distance(transform.position, target.transform.position);
            targetCenterY = targetUnitBehaviour.headHeight / 2;
            directionToTarget = new Vector3(target.transform.position.x, 0, target.transform.position.z) - transform.position;
            transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.Rotate(Vector3.up, 28);
            targetUnderDirectControl = targetUnitBehaviour.assumingDirectControl;

        }
        distanceCheckCount++;

        if (distanceCheckCount > 30)
        {
       
            distanceCheckCount = 0;
            distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > unitRange || targetUnitBehaviour.hp <= 0 || !target.activeInHierarchy)
            {
                jobManager.LookForTarget(thisUnit, unitRange);
                return;
            }
   
            if (distance < unitRange * 0.2f)
            {
                currentAccuracy = 0.9f;
            }
            else
            {
                currentAccuracy = accuracy;
            }
            if (targetUnitBehaviour.evading)
            {
                currentAccuracy *= 0.5f;
            }
            if (targetUnitBehaviour.isBuilding)
            {
                currentAccuracy = 1;

            }
            directionToTarget = -(transform.position - new Vector3(target.transform.position.x, targetCenterY, target.transform.position.z));

            rand = Random.Range(0f, 2f);
            transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            transform.Rotate(Vector3.up, 25);
            if (UnityEngine.Random.Range(0f, 1f) > 0.5)
            {
                Evade();


            }

        }



        weaponRecharge += updateTimestep;
        if (weaponRecharge > weaponRechargeRate * rand)
        {

            muzzleFlash.Play();
            weaponRecharge = 0;


            if (Random.Range(0f, 1f) < accuracy)
            {

                for (int i = 0; i < 4; i++)
                {
                    if (!hitEffectMetalArray[i].activeInHierarchy)
                    {
                        hitEffectMetalArray[i].SetActive(true);
                        hitEffectMetalArray[i].transform.position = target.transform.position + new Vector3(Random.Range(0.2f * targetCenterY, 0.2f * targetCenterY), Random.Range(0.5f, targetCenterY * 2), Random.Range(0.2f * targetCenterY, 0.2f * targetCenterY));
                        hitEffectMetalArray[i].transform.forward = directionToTarget;
                        hitEffectMetalParticle[i].Play();
                        targetUnitBehaviour.changeHP(-damagePerShot);
                        if (targetUnitBehaviour.hp <= 0)
                        {
                            unitBehaviour.OrderComplete();
                            unitBehaviour.currentTarget = null;

                        }
                        i = 7;

                    }

                }

            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!hitEffectGroundArray[i].activeSelf)
                    {
                        hitEffectGroundArray[i].SetActive(true);
                        hitEffectGroundArray[i].transform.position = target.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                        hitEffectGroundParticle[i].Play();
                        i = 7;
                    }

                }
            }




        }


        if (evading)
        {
            transform.position += velocity;
            velocity *= 0.98f;
            if (velocity.magnitude <= 0.07f)
            {
                evading = false;
                unitBehaviour.evading = false;


            }
        }
    }
    void Evade() {

        evading = true;
        int random = Random.Range(0, 6);
        evadeVector = new Vector3(0, 0, 0);
        unitBehaviour.evading = true;

        if (!crouching)
        {

            if (random == 1)
            {
                evadeVector = new Vector3(0.15f, 0, 0);
                animator.SetTrigger("StrafeRight");

            }
            else if (random == 2)
            {
                evadeVector = new Vector3(-0.15f, 0, 0);
                animator.SetTrigger("StrafeLeft");


            }
            else if (random == 3)
            {
                ///   evadeVector = new Vector3(0, 0, -0.10f);
                //animator.SetTrigger("StrafeBack");

            }
            else if (random == 4)
            {
                evadeVector = new Vector3(0, 0, 0.2f);

                animator.SetTrigger("StrafeForward");
            }
            else if (random == 5)
            {
                crouching = true;
                animator.SetBool("Crouching", true);

            }
        }
        else
        {

            if (random == 1)
            {
                evadeVector = new Vector3(-0.15f, 0, 0);
                animator.SetTrigger("StrafeRight");

            }
            else if (random == 2)
            {
                evadeVector = new Vector3(0.15f, 0, 0);
                animator.SetTrigger("StrafeLeft");


            }
            else if (random == 3)
            {
                //    evadeVector = new Vector3(0, 0, -0.10f);
                //   animator.SetTrigger("StrafeBack");

            }
            else if (random == 4)
            {
                //   evadeVector = new Vector3(0, 0, -0.20f);

                //  animator.SetTrigger("StrafeForward");

            }
            else if (random == 5)
            {
                crouching = false;
                animator.SetBool("Crouching", false);

            }

        }

        velocity = transform.rotation * evadeVector;

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

        animator.SetBool("DirectControl", true);
        snappedRight = false;
        snappedLeft = false;

        unitMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        rigTargetHead.localScale = new Vector3(0, 0, 0);

        player.transform.position = rigTargetHead.position + new Vector3(0, 0.3f, 0);
        gameObject.GetComponent<UnitBehaviour>().OrderComplete();

        gameObject.GetComponent<UnitCollision>().enabled = true;

        Stop();

    }
    public void ExitDirectControl() {
        currentOrder = 0;
        updateTimestep = unitBehaviour.updateTimestep;
        Stop();

        gameObject.GetComponent<UnitCollision>().enabled = false;

        gun.transform.localRotation = defaultGunRotation;
        unitMeshAndRig.GetComponent<RigBuilder>().enabled = false;

        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = false;

        rigTargetHead.localScale = new Vector3(1, 1, 1);

        unitBehaviour.OrderComplete();

    }
    public void UnderDirectControl() {

        player.transform.position = rigTargetHead.position + new Vector3(0, 0.15f, 0);
        leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
        rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();
        rightTriggerValue = rightTrigger.action.ReadValue<float>();

        velocity += transform.rotation * new Vector3(leftJoystickValue.x, 0, leftJoystickValue.y) * this.GetComponent<UnitBehaviour>().moveSpeed * Time.deltaTime * 0.15f;
        velocity *= dampening;
        transform.position += velocity;

        //  gun.transform.Rotate(-90, 90, 90);

        // gun.transform.LookAt(vrTargetLeftHand.position,Vector3.up);

        // transform.position = headConstraint.position + headBodyOffset;
        //transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;
        animator.SetFloat("X", leftJoystickValue.x);
        animator.SetFloat("Y", leftJoystickValue.y);

        rigTargetLeftHand.position = vrTargetLeftHand.position;// +new Vector3(0, -0.25f, 0);// TransformPoint(0, 0, 0);
        rigTargetRightHand.position = vrTargetRightHand.position;// + new Vector3(0, -0.25f, 0);// TransformPoint(0,0,0);

        rigTargetLeftHand.rotation = vrTargetLeftHand.rotation;
        rigTargetRightHand.rotation = vrTargetRightHand.rotation;
        if (rightJoystickValue.x < -0.5)
        {
            if (snappedLeft == false)
            {
                transform.Rotate(0, -20, 0);
                player.transform.Rotate(0, -20, 0);
                snappedLeft = true;
            }

        }
        else
        {
            snappedLeft = false;



        }
        if (rightJoystickValue.x > 0.5)
        {
            if (snappedRight == false)
            {
                transform.Rotate(0, 20, 0);
                player.transform.Rotate(0, 20, 0);
                snappedRight = true;
            }

        }
        else
        {
            snappedRight = false;



        }
        float a = Abutton.action.ReadValue<float>();
        if (a > 0)
        {
            animator.SetTrigger("StrafeForward");
        }
        rightTriggerValue = rightTrigger.action.ReadValue<float>();
        weaponRecharge += Time.deltaTime;
        if (rightTriggerValue >= 0.75f && weaponRecharge > weaponRechargeRate / 4)
        {
            weaponRecharge = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!rifleShots[i].activeSelf)
                {
                    muzzleFlash.Play();

                    rifleShots[i].SetActive(true);
                    rifleShotScript[i].Initalize((barrel.position - barrel.forward * 2.5f), barrel.forward, damagePerShot, true); //+ new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f)
                    i = 7;
                }
                if (i == 4)
                {
                    muzzleFlash.Play();

                    rifleShots[0].SetActive(true);
                    rifleShotScript[0].Initalize(barrel.position - (barrel.forward * 2.5f), barrel.forward, damagePerShot, true);
                    i = 7;

                }

            }
        }
        else
        {


        }
    }

    public void Build(GameObject building) {
    }




    public void Move(Vector3 movetarget) {
        if (movetarget != currentMoveTarget || currentOrder != 1) // if the previous order wasn't a move order or it is a move order to a new location
        {
           // Debug.Log("got move order from " + transform.position + " to " + movetarget);
            currentMoveTarget = movetarget;
            moveDirection = (currentMoveTarget - transform.position).normalized;
            transform.forward = moveDirection;
            transformNumber = unitBehaviour.transformNumber;


            jobManager.MoveTransform(transformNumber, currentMoveTarget, moveDirection);
            currentOrder = 1;
 
            animator.SetBool("Moving", true);
        }
      //  transform.position += moveSpeed * transform.forward * updateTimestep;

    }
    // Update is called once per frame
    public void Stop() {
        animator.SetBool("Moving", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("DirectControl", false);

        unitBehaviour.orderQueue.Clear();

    }
}
