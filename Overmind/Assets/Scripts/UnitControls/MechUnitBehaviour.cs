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
     
    GameObject player;
    private float updateTimestep;
    private bool turningRight;
    private bool turningLeft;
    private bool movingForward;
    public GameObject pilot;
    public Transform pilotTargetRightHand;
    public Transform pilotTargetLeftHand;
    public Transform mechTargetRightHand;
    public Transform mechTargetLeftHand;
    public Transform pilotHead;
    public Transform pilotNeck;
    public Transform vrTargetLeftHand;
    public Transform vrTargetRightHand;
    public Transform mechNeck;
    public Transform mechHead;
    public Transform cockpit;
    Transform leftHand;
    Transform rightHand;
    public GameObject pilotMeshAndRig;
    public GameObject mechMeshAndRig;

    public InputActionReference leftJoystick = null;
    public InputActionReference rightJoystick = null;
    public InputActionReference leftTrigger = null;
    public InputActionReference rightTrigger = null;
    public Vector2 leftJoystickValue;
    public Vector2 rightJoystickValue;
    public float leftTriggerValue;
    public float rightTriggerValue;
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
    void start() {
        velocity = new Vector3(0, 0, 0);
        mechAnimator.SetBool("Dynamic", true);

    }
    public void EnterDirectControl() {
        cockpit.gameObject.SetActive(true);

        // pilotAnimator = unitMeshAndRig.GetComponent<Animator>();
        //   mechAnimator = GameObject.Find("Mech").gameObject.GetComponent<Animator>();
        vrTargetLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
        vrTargetRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetRightHand").transform;
        rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller").transform;
        leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller").transform;

        // rigTargetHead = transform.Find("Pilot/LOD0/Armature/Root/Spine1/Spine2/Neck/Bone.004/Head");
        // rigTargetneck = transform.Find("Pilot/LOD0/Armature/Root/Spine1/Spine2/Neck");
        moveSpeed = gameObject.GetComponent<UnitBehaviour>().moveSpeed;

        //  mechNeck = transform.Find("Mech/Armature/Root/Neck/Bone.002");

        //  cockpit = transform.Find("Cockpit");

        //headConstraint = transform.Find("Pilot/LOD0/VR Constraints/Head Constraint");
       // pilotMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        mechMeshAndRig.GetComponent<RigBuilder>().enabled = true;

        pilotNeck.localScale = new Vector3(0, 0, 0); // make pilot head dissappear

        mechNeck.localScale = new Vector3(0, 0, 0); // make mech head dissappear

        player = GameObject.Find("/XR Rig");

        rotationVelocity = 0;
        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = true;

        gameObject.GetComponent<UnitBehaviour>().OrderComplete();
        player.transform.position = pilotHead.position;
        pilotAnimator.SetBool("Sitting", true);
        mechAnimator.SetBool("Dynamic", true);

        // find out how big the person is compared to the mech
        sizeRatio = (mechHead.position.y - transform.position.y) / (pilotHead.position.y - pilot.transform.position.y);
        Debug.Log("sise ratio " + sizeRatio);
    }
    public void ExitDirectControl() {
    }
    public void UnderDirectControl() {

        player.transform.position = pilotHead.position + new Vector3(0, 0.5f, 0);// new Vector3(pilot.transform.position.x, rigTargetHead.position.y, pilot.transform.position.z);
        player.transform.rotation = pilotHead.rotation;// new Vector3(pilot.transform.position.x, rigTargetHead.position.y, pilot.transform.position.z);

        leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
        rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();

        velocity += transform.rotation * new Vector3(leftJoystickValue.x, 0, leftJoystickValue.y) * acceleration * Time.deltaTime * 0.04f;
        transform.position += velocity;

        if (velocity.magnitude > moveSpeed)
        {
            velocity *= 0.7f;
        }
        else
        {
            velocity *= dampening;
        }

        rotationVelocity += rightJoystickValue.x * Time.deltaTime * turnacceleration * 50;
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
        Vector3 rightHandDelta = rightHand.position - player.transform.position;
        Vector3 leftHandDelta = leftHand.position - player.transform.position;

        mechTargetRightHand.position = transform.position + (rightHandDelta * sizeRatio);
        mechTargetLeftHand.position = transform.position + (leftHandDelta * sizeRatio);
        pilotTargetLeftHand.eulerAngles = vrTargetLeftHand.eulerAngles;
        pilotTargetRightHand.eulerAngles = vrTargetRightHand.eulerAngles;
        
        signedangletotarget = Vector3.SignedAngle(leftJoystickValue, transform.forward, Vector3.up);
        targetAnimationVector = Quaternion.AngleAxis(-signedangletotarget, Vector3.up) * Vector3.forward;
        cockpit.rotation = Quaternion.Lerp(cockpit.rotation, mechHead.rotation, 0.01f) ;
      
        currentAnimationVector = Vector3.Lerp(currentAnimationVector, targetAnimationVector, 0.004f);
        Debug.Log(targetAnimationVector);

        // Vector3 animationnormalized = (transform.rotation * velocity) / moveSpeed;

        mechAnimator.SetFloat("X", currentAnimationVector.x);
        mechAnimator.SetFloat("Y", currentAnimationVector.z);



    }
    void Start() {

        player = GameObject.Find("/XR Rig");
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        turningRight = false;
        turningLeft = false;
        movingForward = false;

    }
    public void Stop() {
        mechAnimator.SetBool("Moving", false);
        mechAnimator.SetBool("TurnLeft", false);
        mechAnimator.SetBool("TurnRight", false);

    }
    public void Move(Vector3 movetarget) {
        Debug.Log("Move ran");
        if (Vector3.Distance(movetarget, transform.position) < 1f) // if within 1m of destination
        {
            mechAnimator.SetBool("Moving", false);
            mechAnimator.SetBool("TurnLeft", false);
            mechAnimator.SetBool("TurnRight", false);
            turningRight = false;
            turningLeft = false;
            movingForward = false;
            gameObject.GetComponent<UnitBehaviour>().OrderComplete();

        }
        else
        {
            mechAnimator.SetBool("Dynamic", true);

            Vector3 direction = Vector3.Normalize(movetarget - transform.position);
            //float angletotarget = 1;

            //Quaternion.Angle(transform.rotation , targetRotation);
            float signedangletotarget = Vector3.SignedAngle(direction, transform.forward, Vector3.up);

            float angletotarget = Vector3.Angle(direction, transform.forward);



            /* if (signedangletotarget > 90)
               {
                 mechAnimator.SetBool("Dynamic", false);

                 mechAnimator.SetBool("TurnLeft", true);

               }
               else if (signedangletotarget < -90)
               {
                 mechAnimator.SetBool("Dynamic", false);

                 mechAnimator.SetBool("TurnRight", true);

               }
               else
               {*/

            currentAnimationVector = Vector3.Lerp(currentAnimationVector, targetAnimationVector, 0.005f);
            targetAnimationVector = Quaternion.AngleAxis(-signedangletotarget, Vector3.up) * Vector3.forward;


            // Vector3 animationnormalized = (transform.rotation * velocity) / moveSpeed;
            
                mechAnimator.SetFloat("X", currentAnimationVector.x);
                mechAnimator.SetFloat("Y", currentAnimationVector.z);

            // }
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
            //transform.Rotate(new Vector3(0, rotationVelocity, 0));
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.25f);
            transform.rotation = newRotation;
            velocity += direction * acceleration * Time.deltaTime             ;

            if (velocity.magnitude > moveSpeed)
            {
                velocity *= 0.8f;
            }
            else
            {
                velocity *= dampening;
            }

            transform.position += velocity;

       }
    }
}
