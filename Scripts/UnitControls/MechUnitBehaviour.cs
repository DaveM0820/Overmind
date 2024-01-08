using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;


public class MechUnitBehaviour : MonoBehaviour, IUnitActionInterface
{
    // Start is called before the first frame update

    // Update is called once per frame

    private bool isMoving;
    public Animator pilotAnimator;
    public Animator mechAnimator;
    public float mechArmVelocityWhenDirectControlled;
    public float velocityModifierWhenDirectControlled;
    public float rotationSpeedModifierWhenDirectControlled;

    GameObject player;
    GameObject mainCamera;
    private float updateTimestep;
    private bool turningRight;
    private bool turningLeft;
    private bool movingForward;
    public GameObject pilot;
    public Transform pilotTargetRightHand;
    public Transform pilotTargetLeftHand;
    public Transform mechTargetRightHand;
    public Transform mechTargetLeftHand;
    Vector3 mechVirtualRightHandVelocity;
    Vector3 mechVirtualLeftHandVelocity;
    Vector3 mechVirtualRightHand;
    Vector3 mechVirtualLeftHand;
    Vector3 mechVirtualRightHandTarget;
    Vector3 mechVirtualLeftHandTarget;
    Vector3 leftHandDelta;
    Vector3 rightHandDelta;
    public Transform pilotHead;
    public Transform pilotNeck;
    public Transform pilotRoot;
    public Transform vrTargetLeftHand;
    public Transform vrTargetRightHand;
    public Transform mechNeck;
    public Transform mechHead;
    public Transform mechRoot;

    public Transform cockpit;
    Transform leftHand;
    Transform rightHand;
    public GameObject pilotMeshAndRig;
    public GameObject mechMeshAndRig;

    public InputActionReference leftJoystick = null;
    public InputActionReference rightJoystick = null;
    public InputActionReference leftTrigger = null;
    public InputActionReference rightTrigger = null;
    float leftTriggerValue;
    float rightTriggerValue;
    public Vector2 leftJoystickValue;
    public Vector2 rightJoystickValue;
    public InputActionReference SelectActionRefrence = null;
    float ButtonValue;
    public Vector3 velocity;
    float moveSpeed;
    public float turnSpeed;
    public float acceleration;
    public float turnacceleration;
    Vector3 currentAnimationVector;
    Vector3 targetAnimationVector;
    public float rotationVelocity;
    public float dampening;
    public float rotationDampening;
    UnitBehaviour buildingUnitBehaviour;
    UnitBehaviour unitBehaviour;
    GameObject builderBeam2;
    float rightBeamRecharge;
    float leftBeamRecharge;
    float signedangletotarget;
    float sizeRatio;
    float timestep;
    
    float leftWeaponRecharge;
    float rightWeaponRecharge;

    public float leftWeaponCooldownLimit;
    float leftWeaponHeat;
    public float rightWeaponRechargeTime;
    public float leftWeaponRechargeTime;

    public GameObject artyshellprefab;
    public GameObject gatlingBulletprefab;
    GameObject[] gatlingShots;
    GatlingShot[] gatlingShotScript;
    public Transform rightBarrel;
    public Transform leftBarrel;
    ParticleSystem leftMuzzleFlash;


    GameObject artyshell1;
    GameObject artyshell2;
    GameObject artyshell3;
    TrackedArty shell1script;
    TrackedArty shell2script;
    TrackedArty shell3script;

    GameObject pilotHelmet;

    public GameObject gatlingMuzzle;
    ParticleSystem gatlingMuzzleParticle;
    public void UpdateUnit() {
    }
        public void Attack(GameObject target) {


    }
    public void BuildUnit(GameObject unit) {
    }
    public void ExtractOre() {
    }
    public void UpdateScaffold() {
    }
    public void Build(GameObject gameobj) {


    }
    public void Die() {
    }
    public void Damage() {


    }
    void Start() {
        velocity = new Vector3(0, 0, 0);
        mechAnimator.SetBool("Dynamic", true);
        unitBehaviour = gameObject.GetComponent<UnitBehaviour>();
        moveSpeed = unitBehaviour.moveSpeed;
        leftWeaponHeat = 0;
        artyshell1 = Instantiate(artyshellprefab, transform.root);
        artyshell2 = Instantiate(artyshellprefab, transform.root);
        artyshell3 = Instantiate(artyshellprefab, transform.root);
        gatlingShots = new GameObject[6];
        gatlingShotScript = new GatlingShot[6];
        gatlingMuzzleParticle = gatlingMuzzle.GetComponent<ParticleSystem>();
        for (int i = 0; i < 6; i++)
        {
 
            gatlingShots[i] = Instantiate(gatlingBulletprefab, transform.root);

            gatlingShotScript[i] = gatlingShots[i].GetComponent<GatlingShot>();
            gatlingShots[i].SetActive(false);

        }


        shell1script = artyshell1.GetComponent<TrackedArty>();
        shell2script = artyshell2.GetComponent<TrackedArty>();
        shell3script = artyshell3.GetComponent<TrackedArty>();
        artyshell1.SetActive(false);
        artyshell2.SetActive(false);
        artyshell3.SetActive(false);
        player = GameObject.Find("/XR Rig");
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        turningRight = false;
        turningLeft = false;
        vrTargetLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
        vrTargetRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetRightHand").transform;
        rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller").transform;
        leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller").transform;
        leftMuzzleFlash = leftBarrel.GetComponent<ParticleSystem>();
        pilotHelmet = GameObject.Find("/XR Rig/Camera Offset/Main Camera/Builder Helmet");
        movingForward = false;
        //moveSpeed = gameObject.GetComponent<UnitBehaviour>().moveSpeed;
        sizeRatio = (mechHead.position.y - mechRoot.position.y) / (pilotHead.position.y - pilotRoot.transform.position.y);

    }
    public void EnterDirectControl() {
        cockpit.gameObject.SetActive(true);
        pilotNeck.localScale = new Vector3(0, 0, 0); // make pilot head dissappear
        Select.SelectedUnits.Clear();

        mechVirtualRightHand = mechTargetRightHand.transform.position;
        mechVirtualLeftHand = mechTargetLeftHand.transform.position;

        mechNeck.localScale = new Vector3(0, 0, 0); // make mech head dissappear
        pilotMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        mechMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        rotationVelocity = 0;
        velocity = new Vector3(0,0,0);
        player.transform.position = pilotHead.position + new Vector3(0, 0.5f, 0);// new Vector3(pilot.transform.position.x, rigTargetHead.position.y, pilot.transform.position.z);
        pilotAnimator.SetBool("Sitting", true);
        mechAnimator.SetBool("Dynamic", true);
        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = true;
        gameObject.GetComponent<UnitBehaviour>().OrderComplete();
        gameObject.GetComponent<UnitCollision>().enabled = true;

    }
    public void ExitDirectControl() {
        Debug.Log("exitDirectContrl Mech 1");
        pilotMeshAndRig.GetComponent<RigBuilder>().enabled = false;
        mechMeshAndRig.GetComponent<RigBuilder>().enabled = false;
        cockpit.gameObject.SetActive(false);
        mechNeck.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = false;
        //headConstraint = transform.Find("Pilot/LOD0/VR Constraints/Head Constraint");
        gameObject.GetComponent<UnitCollision>().enabled = false;
        Debug.Log("exitDirectContrl Mech done");

    }

    public void UnderDirectControl() {
        player.transform.position = pilotHead.position + new Vector3(0, 0.2f, 0);// new Vector3(pilot.transform.position.x, rigTargetHead.position.y, pilot.transform.position.z);


        player.transform.rotation = pilotHead.rotation;// new Vector3(pilot.transform.position.x, rigTargetHead.position.y, pilot.transform.position.z);
        
        mechHead.rotation = Quaternion.Lerp(mechHead.rotation, pilotHelmet.transform.rotation, 0.03f);
         
        cockpit.rotation = Quaternion.Lerp(cockpit.rotation, mechHead.rotation, 0.03f);

        leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
        rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();

        velocity += transform.rotation * new Vector3(leftJoystickValue.x, 0, leftJoystickValue.y) * acceleration * Time.deltaTime * velocityModifierWhenDirectControlled;
        transform.position += velocity;
        velocity *= dampening;

        if (velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
      
       

        rotationVelocity += rightJoystickValue.x * Time.deltaTime * turnacceleration * rotationSpeedModifierWhenDirectControlled;
        if (rotationVelocity > turnSpeed)
        {
            rotationVelocity = turnSpeed;
        }
        if (rotationVelocity < -turnSpeed)
        {
            rotationVelocity = -turnSpeed;
        }

        rotationVelocity *= rotationDampening;
        transform.Rotate(new Vector3(0, rotationVelocity, 0));



        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, pilotHead.rotation, 0.01f) ;

        pilotTargetLeftHand.position = vrTargetLeftHand.position;
        pilotTargetRightHand.position = vrTargetRightHand.position;

        leftHandDelta = pilotTargetLeftHand.position - pilotRoot.transform.position;
        rightHandDelta = pilotTargetRightHand.position - pilotRoot.transform.position;

        mechVirtualLeftHandTarget = mechRoot.position + (leftHandDelta * sizeRatio) - new Vector3(0,2,0);
        mechVirtualRightHandTarget = mechRoot.position + (rightHandDelta * sizeRatio) - new Vector3(0, 2, 0);

 
        mechVirtualLeftHandVelocity += (mechVirtualLeftHandTarget - mechVirtualLeftHand) * 0.001f;
        mechVirtualRightHandVelocity += (mechVirtualRightHandTarget - mechVirtualRightHand) * 0.001f;

        mechVirtualLeftHandVelocity *= 0.98f;
        mechVirtualRightHandVelocity *= 0.98f;

        mechVirtualLeftHand += mechVirtualLeftHandVelocity;
        mechVirtualRightHand += mechVirtualRightHandVelocity;
 

        mechTargetLeftHand.position = mechVirtualLeftHand;
        mechTargetRightHand.position = mechVirtualRightHand;

        mechTargetRightHand.rotation = Quaternion.Lerp(mechTargetRightHand.rotation, vrTargetRightHand.rotation, 0.035f);
        mechTargetLeftHand.rotation = Quaternion.Lerp(mechTargetLeftHand.rotation, vrTargetLeftHand.rotation, 0.035f);

        pilotTargetLeftHand.rotation = vrTargetLeftHand.rotation;
        pilotTargetRightHand.rotation = vrTargetRightHand.rotation;

        if (Vector3.Distance(mechTargetLeftHand.position, mechHead.position) < 7f)
        {
            mechVirtualLeftHandVelocity -= (mechHead.position - mechTargetLeftHand.position) * 0.005f;

        }
        if (Vector3.Distance(mechTargetRightHand.position, mechHead.position) < 6f)
        {
            mechVirtualRightHandVelocity -= (mechHead.position - mechTargetRightHand.position) * 0.005f;

        }
        targetAnimationVector = new Vector3(leftJoystickValue.x, leftJoystickValue.y, 0);
        targetAnimationVector.x += rightJoystickValue.x;
        currentAnimationVector = Vector3.Lerp(currentAnimationVector, targetAnimationVector, 0.005f);

        mechAnimator.SetFloat("X", currentAnimationVector.x);
        mechAnimator.SetFloat("Y", currentAnimationVector.y);
        rightTriggerValue = rightTrigger.action.ReadValue<float>();
        rightWeaponRecharge += Time.deltaTime;

        if (rightTriggerValue >= 1f && rightWeaponRecharge > rightWeaponRechargeTime)
        {
        mechVirtualRightHandVelocity += vrTargetRightHand.up * -0.3f + new Vector3(0, Random.Range(0, 0.1f), 0);
            rightWeaponRecharge = 0;
            if (!artyshell1.activeInHierarchy)
            {
                artyshell1.SetActive(true);
                shell1script.initalize(vrTargetRightHand.up * 6f, rightBarrel.position, gameObject);

            } else if (!artyshell2.activeInHierarchy)
            {
                artyshell2.SetActive(true);
                shell2script.initalize(vrTargetRightHand.up * 6f, rightBarrel.position, gameObject);

            }
            else if (!artyshell3.activeInHierarchy)
            {
                artyshell3.SetActive(true);
                shell3script.initalize(vrTargetRightHand.up * 6f, rightBarrel.position, gameObject);

            }
            else{
                artyshell1.SetActive(true);
                shell1script.initalize(vrTargetRightHand.up * 6f, rightBarrel.position, gameObject);
            }
        }
        leftTriggerValue = leftTrigger.action.ReadValue<float>();
        leftWeaponRecharge += Time.deltaTime;
        if (leftTriggerValue >= 0.75f && leftWeaponRecharge > leftWeaponRechargeTime)
        {
            gatlingMuzzleParticle.Play();
            leftWeaponRecharge = 0;
            mechVirtualLeftHandVelocity += vrTargetLeftHand.up * -0.04f + new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            ;

            for (int i = 0; i < 6; i++)
            {
                if (!gatlingShots[i].activeInHierarchy)
                {
                    Debug.Log("leftshot " + i);
                    gatlingShots[i].SetActive(true);
                    gatlingShotScript[i].Initalize(leftBarrel.forward * 10f + new Vector3(Random.Range(-0.2f,0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)), leftBarrel.position, leftBarrel.forward, gameObject);
                    i = 7;
                }
                if (i == 5)
                {
                    Debug.Log("leftshot " + i);
                    gatlingShots[0].SetActive(true);
                    gatlingShotScript[0].Initalize(leftBarrel.forward * 10f, leftBarrel.position, leftBarrel.forward, gameObject);
                    i = 7;

                }

            }

           

        }
        else
        {

        }

    }
    public void CollisionEnter(Collider collision) {
 

    }
    public void CollisionExit(Collider collision) {


    }
    public void CollisionStay(Collider collision) {
        if (collision.gameObject.layer == 7)
        {
            transform.position -= (collision.gameObject.transform.position - transform.position).normalized;
            velocity *= 0.2f;
        }
    }
    public void Stop() {
     //   mechAnimator.SetBool("Moving", false);
    //    mechAnimator.SetBool("Dynamic", true);

    }
    public void Move(Vector3 movetarget, bool finalDestination = false) {
        if (Vector3.Distance(movetarget, transform.position) < 1f) // if within 1m of destination
        {

            turningRight = false;
            turningLeft = false;
            movingForward = false;
            gameObject.GetComponent<UnitBehaviour>().OrderComplete();

        }
        else if (Vector3.Distance(movetarget, transform.position) < 4f) // if within 1m of destination
        {

            Vector3 direction = Vector3.Normalize(movetarget - transform.position);
            float signedangletotarget = Vector3.SignedAngle(direction, transform.forward, Vector3.up);
            float angletotarget = Vector3.Angle(direction, transform.forward);
            currentAnimationVector = Vector3.Slerp(currentAnimationVector, targetAnimationVector, 0.005f);
            targetAnimationVector = Quaternion.AngleAxis(-signedangletotarget, Vector3.up) * Vector3.forward;

            mechAnimator.SetFloat("X", currentAnimationVector.x);
            mechAnimator.SetFloat("Y", currentAnimationVector.z);

            rotationVelocity += Time.deltaTime * turnacceleration;
            if (rotationVelocity > turnSpeed)
            {
                rotationVelocity = turnSpeed;
            }
            if (rotationVelocity < -turnSpeed)
            {
                rotationVelocity = -turnSpeed;
            }
            rotationVelocity *= rotationDampening;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.25f);
            transform.rotation = newRotation;
            velocity *= dampening;


            velocity *= 0.98f;

            transform.position += velocity;

        }
        else
        {

            Vector3 direction = Vector3.Normalize(movetarget - transform.position);
            float signedangletotarget = Vector3.SignedAngle(direction, transform.forward, Vector3.up);
            float angletotarget = Vector3.Angle(direction, transform.forward);
            currentAnimationVector = Vector3.Slerp(currentAnimationVector, targetAnimationVector, 0.005f);
            targetAnimationVector = Quaternion.AngleAxis(-signedangletotarget, Vector3.up) * Vector3.forward;

                mechAnimator.SetFloat("X", currentAnimationVector.x);
                mechAnimator.SetFloat("Y", currentAnimationVector.z);

            rotationVelocity += Time.deltaTime * turnacceleration;
            if (rotationVelocity > turnSpeed)
            {
                rotationVelocity = turnSpeed;
            }
            if (rotationVelocity < -turnSpeed)
            {
                rotationVelocity = -turnSpeed;
            }
            rotationVelocity *= rotationDampening;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.25f);
            transform.rotation = newRotation;
            velocity *= dampening;

            velocity += direction * acceleration * unitBehaviour.updateTimestep;
            if (velocity.magnitude > moveSpeed/unitBehaviour.updateTimestep)
            {
                 velocity = velocity.normalized * (moveSpeed/unitBehaviour.updateTimestep);

            }

            transform.position += velocity;




       }
    }
}
