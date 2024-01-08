using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class BuilderUnitBehaviour : MonoBehaviour, IUnitActionInterface
{
    public bool currentlyBuilding;
    private GameObject building;
    private GameObject currentBuildingtoBuild;
    GameObject player;
    Vector2 buidlingDimensions;
    private float reloadProgress;
    public float buildRate;
    public int buildStrength;
    private float updateTimestep;
    private Transform builderBeam;
    private Animator animator;
    private bool isBuilding;
    private bool isMoving;
    float scaffoldHeight;
    public GameObject text;
    //VR stuff
    public Transform rigTargetRightHand;
    public Transform rigTargetLeftHand;
    public Transform rigTargetHead;
    public Transform rigTargetneck;
    public Transform vrTargetLeftHand;
    public Transform vrTargetRightHand;
    public Transform rigTargetRightHand2;
   Transform leftHand;
    Transform rightHand;
    public GameObject helmet;
    public GameObject unitMeshAndRig;    public Transform headConstraint;
    public Vector3 headBodyOffset;
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
    public float rotationVelocity;
    public float dampening;
    public float rotationDampening;
    UnitBehaviour buildingUnitBehaviour;
    public bool snappedRight;
    public bool snappedLeft;
    UnitBehaviour unitBehaviour;
    GameObject builderBeam2;
    float rightBeamRecharge;
    float leftBeamRecharge;
    RaycastHit leftHit;
    RaycastHit rightHit;
    Vector3 beamHit;
    Vector3 beamHit2;
    GameObject sparks;
    GameObject sparks2;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("/XR Rig");
        builderBeam = transform.Find("BuilderBeam");

        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        unitBehaviour = gameObject.GetComponent<UnitBehaviour>(); 
        currentlyBuilding = false;
        builderBeam.transform.localScale = new Vector3(0,0,0);
        animator = transform.Find("LOD0").gameObject.GetComponent<Animator>();
        builderBeam2 = Instantiate(builderBeam.gameObject);
        isBuilding = false;
        isMoving = false;
        //vr stuff
        vrTargetLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
        vrTargetRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetRightHand").transform;
        rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller").transform;
        leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller").transform;
        unitMeshAndRig = transform.Find("LOD0").gameObject;
        rigTargetRightHand = transform.Find("LOD0/VR Constraints/Right Arm IK/Target");
        rigTargetLeftHand = transform.Find("LOD0/VR Constraints/Left Arm IK/Target");
        rigTargetHead = transform.Find("LOD0/Armature/Root/Spine1/Spine2/Neck");
        rigTargetneck = transform.Find("LOD0/Armature/Root/Spine1/Spine2/Neck");
        headConstraint = transform.Find("LOD0/VR Constraints/Head Constraint");
        helmet = GameObject.Find("/XR Rig/Camera Offset/Main Camera/Builder Helmet");
        player = GameObject.Find("/XR Rig");
        sparks = builderBeam.Find("LOD0/SparksEffect").gameObject;
        sparks2 = builderBeam2.transform.Find("LOD0/SparksEffect").gameObject;
    }
    public void UpdateUnit() {
    }
        public void Attack(GameObject target) {
    
    
    }
    public void BuildUnit(GameObject unit) {
    }
     public void ExtractOre() {
    }
    public void Damage() {


    }
    public void UpdateScaffold() {
    }
    public void CollisionEnter(Collider collision) {

    }
    public void CollisionExit(Collider collision) {


    }
    public void Die() {
    }
    public void CollisionStay(Collider collision) {
        if (collision.gameObject.layer == 7)
        {
            transform.position -= (collision.gameObject.transform.position - transform.position).normalized;
            velocity *= 0.2f;
        }
    }
    public void EnterDirectControl() {
        Stop(); 

        animator.SetBool("DirectControl", true);
        snappedRight = false;
        snappedLeft = false;
        helmet.SetActive(true);

        unitMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        rigTargetneck.localScale = new Vector3(0, 0, 0); // make head dissappear

        player.transform.position = rigTargetHead.position + new Vector3(0,0.3f,0);
        animator = unitMeshAndRig.GetComponent<Animator>();

        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = true;
        gameObject.GetComponent<UnitCollision>().enabled = true;
        builderBeam.gameObject.SetActive(true);
        builderBeam2.gameObject.SetActive(true);
        sparks.gameObject.SetActive(true);
        sparks2.gameObject.SetActive(true);
        GetComponent<UnitBehaviour>().OrderComplete();

    }
    public void ExitDirectControl() {

        animator.SetBool("DirectControl", false);
        gameObject.GetComponent<UnitCollision>().enabled = false;

        helmet.SetActive(false);
        unitMeshAndRig.GetComponent<RigBuilder>().enabled = false;
        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = false;

        rigTargetneck.localScale = new Vector3(1, 1, 1);
        builderBeam.gameObject.SetActive(false);
        builderBeam2.gameObject.SetActive(false);
        unitBehaviour.OrderComplete();

    }
    public void UnderDirectControl() {
        Debug.Log("enter direct control builder5");


        player.transform.position = rigTargetneck.position + new Vector3(0, 0.5f, 0);
        leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
        rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();
        rightTriggerValue = rightTrigger.action.ReadValue<float>();
        leftTriggerValue = leftTrigger.action.ReadValue<float>();

        rigTargetLeftHand.position = vrTargetLeftHand.position + new Vector3 (0,-0.2f,0);// TransformPoint(0, 0, 0);
        rigTargetRightHand.position = vrTargetRightHand.position + new Vector3(0, -0.2f, 0);// TransformPoint(0,0,0);

        rigTargetLeftHand.rotation = vrTargetLeftHand.rotation;
        rigTargetRightHand.rotation = vrTargetRightHand.rotation;
        player.transform.position = new Vector3(transform.position.x, rigTargetneck.position.y + 0.3f, transform.position.z);
        if (rightTriggerValue > 0.5f) //shoot beam from right hand 
        {

            builderBeam.transform.localScale = new Vector3(1, 1, 1);

            Physics.Raycast(rightHand.position + rightHand.forward, rightHand.forward, out rightHit, 500, LayerMask.GetMask("Buildings", "Units", "Ground"));
            builderBeam.transform.position = rightHand.position + (rightHand.forward * 0.25f);
            builderBeam.transform.forward = rightHand.forward;
            builderBeam.transform.localScale = new Vector3(1, 1, 1);
            if (rightHit.collider == null)
            {

                builderBeam.transform.localScale = new Vector3(1, 1, 200);
            }
            else
            {
                sparks.gameObject.SetActive(true);

                beamHit = rightHit.point;
                float beamLength = Vector3.Distance(rightHand.position, beamHit);
                builderBeam.localScale = new Vector3(1, 1, beamLength);
                rightBeamRecharge += Time.deltaTime;
            }
  
        }
        else
        {
            rightBeamRecharge = 0;
            builderBeam.transform.localScale = new Vector3(0, 0, 0);
            sparks.gameObject.SetActive(false);

        }
        if (leftTriggerValue > 0.5f) //shoot beam from left hand 
        {

            builderBeam2.transform.localScale = new Vector3(1, 1, 1);
            Physics.Raycast(leftHand.position + leftHand.forward, leftHand.forward, out leftHit, 500, LayerMask.GetMask("Buildings", "Units", "Ground"));
            builderBeam2.transform.position = leftHand.position + (leftHand.forward * 0.25f);
            builderBeam2.transform.forward = leftHand.forward;
            if (leftHit.collider == null)
            {
              
                builderBeam2.transform.localScale = new Vector3(1, 1, 200);
            }
            else
            {
                sparks2.gameObject.SetActive(true);

                beamHit2 = leftHit.point;
            float beamLength = Vector3.Distance(rightHand.position, beamHit2);
            builderBeam2.transform.localScale = new Vector3(1, 1, beamLength);
            }
            leftBeamRecharge += Time.deltaTime;

        }
        else
        {
            leftBeamRecharge = 0;
            sparks2.gameObject.SetActive(false);

            builderBeam2.transform.localScale = new Vector3(0,0,0);

        }
        if (rightBeamRecharge > buildRate/2)
        {
            UnitBehaviour objecthitGameObject = rightHit.collider.gameObject.GetComponent<UnitBehaviour>();
            if (objecthitGameObject.faction == unitBehaviour.faction)
            {
                objecthitGameObject.changeHP((int)buildStrength);
                GameObject newText = Instantiate(text);
                newText.GetComponent<TextMesh>().text = "+" + objecthitGameObject.hpChange + " HP";
                newText.transform.position = beamHit;
            }
            else
            {
                objecthitGameObject.changeHP(-(int)buildStrength);
            }
            rightBeamRecharge = 0;


        }
        if (leftBeamRecharge > buildRate/2)
        {
            UnitBehaviour objecthitGameObject = leftHit.collider.gameObject.GetComponent<UnitBehaviour>();
            if (objecthitGameObject.faction == unitBehaviour.faction)
            {
                objecthitGameObject.changeHP((int)buildStrength);
                GameObject newText = Instantiate(text);
                newText.GetComponent<TextMesh>().text = "+" + objecthitGameObject.hpChange + " HP";
                newText.transform.position = beamHit2;
            }
            else
            {
                objecthitGameObject.changeHP(-(int)buildStrength);
            }
            leftBeamRecharge = 0;


        }

        velocity += transform.rotation * new Vector3(leftJoystickValue.x, 0, leftJoystickValue.y) * this.GetComponent<UnitBehaviour>().moveSpeed * Time.deltaTime * 0.2f;
        velocity *= dampening;
        transform.position += velocity;
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


        // transform.position = headConstraint.position + headBodyOffset;
        //transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;
        animator.SetFloat("X", leftJoystickValue.x);
        animator.SetFloat("Y", leftJoystickValue.y);




    }

    public void Build(GameObject building)
        {
        if (isBuilding == false)
        {
            builderBeam.gameObject.SetActive(true);
            isBuilding = true;
            isMoving = false;

            Vector3 unitAngle = (building.transform.position - transform.position).normalized;
            transform.forward = unitAngle;
            animator.SetBool("Building", true);

        }

        if (building != currentBuildingtoBuild)
        {
            currentBuildingtoBuild = building;
            scaffoldHeight = building.GetComponent<BuildingBehaviour>().scaffoldHeight;
            buidlingDimensions = building.GetComponent<BuildingBehaviour>().buidlingDimensions;
            buildingUnitBehaviour = building.GetComponent<UnitBehaviour>();

        }
        reloadProgress += updateTimestep;
        if (reloadProgress >= buildRate)
        {
            if (building.GetComponent<BuildingBehaviour>().built == false)
            {
                Vector3 beamHit = building.transform.position + new Vector3(Random.Range(0, buidlingDimensions.x), Random.Range(0, scaffoldHeight), Random.Range(0, buidlingDimensions.y)) + new Vector3(-buidlingDimensions.x/2, 0,-buidlingDimensions.y/2);
                builderBeam.LookAt(beamHit, Vector3.up);
                float beamLength = Vector3.Distance(transform.position, beamHit);
                builderBeam.localScale = new Vector3(1, 1, beamLength);
                buildingUnitBehaviour.hp += buildStrength;
                reloadProgress = 0;

            }
            else
            {
                stopBuilding();
                gameObject.GetComponent<UnitBehaviour>().OrderComplete();
            }
            
        }

    }



public void stopBuilding()
    {
        builderBeam.gameObject.SetActive(false);
        animator.SetBool("Building", false);
        isBuilding = false;

    }
    public void Move(Vector3 movetarget, bool finalDestination = false)
    {
        if (isMoving == false)
        {
            builderBeam.gameObject.SetActive(false);
            isMoving = true;
            isBuilding = false;
            animator.SetBool("Moving", true);
        }
        Vector3 moveDir = (movetarget - transform.position).normalized;
        if (Vector3.Distance(movetarget, transform.position) < 1f) // if within 1m of destination
        {
            animator.SetBool("Moving", false);
            isMoving = false;
            gameObject.GetComponent<UnitBehaviour>().OrderComplete();

        }
        else
        {
            Vector3 moveDistance = moveDir * gameObject.GetComponent<UnitBehaviour>().moveSpeed * updateTimestep;
            transform.forward = moveDir;
            moveDistance = new Vector3(moveDistance.x, 0, moveDistance.z);
            transform.position += moveDistance;
        }
    }
    // Update is called once per frame
    public void Stop()
    {
        animator.SetBool("Moving", false);
        animator.SetBool("Building", false);
    }
}
