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
    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("/XR Rig");
        builderBeam = transform.Find("BuilderBeam");
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        unitBehaviour = gameObject.GetComponent<UnitBehaviour>(); 
        currentlyBuilding = false;
        builderBeam.transform.localScale = new Vector3(0,0,0);
        builderBeam.gameObject.SetActive(false);
        animator = transform.Find("LOD0").gameObject.GetComponent<Animator>();
        builderBeam2 = Instantiate(builderBeam.gameObject);
        isBuilding = false;
        isMoving = false;
        //vr stuff

    }
    public void Attack(GameObject target) {
    
    
    }
    public void BuildUnit(GameObject unit) {
    }
     public void ExtractOre() {
    }
    public void UpdateScaffold() {
    }
    public void EnterDirectControl() {
        Stop(); 
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
        Debug.Log("enter direct control builder");

        animator.SetBool("DirectControl", true);
        snappedRight = false;
        snappedLeft = false;
        helmet.SetActive(true);
        Debug.Log("enter direct control builder2");

        unitMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        rigTargetneck.localScale = new Vector3(0, 0, 0); // make head dissappear
        player = GameObject.Find("/XR Rig");
        Debug.Log("enter direct control builder3");

        player.transform.position = rigTargetHead.position;
        animator = unitMeshAndRig.GetComponent<Animator>();
        GetComponent<UnitBehaviour>().OrderComplete();
        Debug.Log("enter direct control builder4");

        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = true;

    }
    public void ExitDirectControl() {

        animator.SetBool("DirectControl", false);

        helmet.SetActive(false);
        unitMeshAndRig.GetComponent<RigBuilder>().enabled = false;
        gameObject.GetComponent<UnitBehaviour>().assumingDirectControl = false;

        rigTargetneck.localScale = new Vector3(1, 1, 1);
        unitBehaviour.OrderComplete();

    }
    public void UnderDirectControl() {
        Debug.Log("enter direct control builder5");

        leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
        rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();
        rightTriggerValue = rightTrigger.action.ReadValue<float>();
        leftTriggerValue = leftTrigger.action.ReadValue<float>();
        if (rightTriggerValue > 0.5f) //shoot beam from right hand 
        {

            Physics.Raycast(rightHand.position + rightHand.forward, rightHand.forward, out rightHit, 500, LayerMask.GetMask("Buildings", "Units", "Ground"));
            //Debug.Log("right hand position " + vrTargetRightHand.position + "right hand direction " + vrTargetRightHand.forward);
            builderBeam.gameObject.SetActive(true);
            if (rightHit.point == null)
            {
                builderBeam.transform.position = rightHand.position + (rightHand.forward*0.5f);
                builderBeam.transform.forward = rightHand.forward;
                builderBeam.transform.localScale = new Vector3(1, 1, 100);
            }
            else
            {

                beamHit = rightHit.point;
                builderBeam.transform.position = rightHand.position + (rightHand.forward * 0.5f);
                builderBeam.LookAt(beamHit, Vector3.up);
                float beamLength = Vector3.Distance(rightHand.position, beamHit);
                builderBeam.localScale = new Vector3(1, 1, beamLength);
                rightBeamRecharge += Time.deltaTime;
            }
  
        }
        else
        {
            rightBeamRecharge = 0;
            builderBeam.gameObject.SetActive(false);

        }
        if (leftTriggerValue > 0.5f) //shoot beam from left hand 
        {
            builderBeam2.gameObject.SetActive(true);
            Physics.Raycast(leftHand.position + leftHand.forward, leftHand.forward, out leftHit, 500, LayerMask.GetMask("Buildings", "Units", "Ground"));
            if (leftHit.point == null)
            {
                builderBeam2.transform.position = leftHand.position + (leftHand.forward * 0.5f);
                builderBeam2.transform.forward = leftHand.forward;
                builderBeam2.transform.localScale = new Vector3(1, 1, 100);
            }
            else
            {
            beamHit2 = leftHit.point;
            builderBeam2.transform.position = leftHand.position + (leftHand.forward * 0.5f);
            builderBeam2.transform.LookAt(beamHit2, Vector3.up);
            float beamLength = Vector3.Distance(rightHand.position, beamHit2);
            builderBeam2.transform.localScale = new Vector3(1, 1, beamLength);
            }
            leftBeamRecharge += Time.deltaTime;

        }
        else
        {
            leftBeamRecharge = 0;

            builderBeam2.gameObject.SetActive(false);

        }
        if (rightBeamRecharge > buildRate/2)
        {
            UnitBehaviour objecthitGameObject = rightHit.collider.gameObject.GetComponent<UnitBehaviour>();
            if (objecthitGameObject.owner == unitBehaviour.owner)
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
            if (objecthitGameObject.owner == unitBehaviour.owner)
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

        rigTargetLeftHand.position = vrTargetLeftHand.position;// TransformPoint(0, 0, 0);
        rigTargetRightHand.position = vrTargetRightHand.position;// TransformPoint(0,0,0);
 
        rigTargetLeftHand.eulerAngles = vrTargetLeftHand.eulerAngles;
        rigTargetRightHand.eulerAngles = vrTargetRightHand.eulerAngles;
        player.transform.position = new Vector3(transform.position.x, rigTargetneck.position.y, transform.position.z);


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
               // GameObject newText = Instantiate(text);
                //newText.GetComponent<TextMesh>().text = "+ " + buildStrength + "HP"; 
                //newText.transform.position = beamHit;
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
    public void Move(Vector3 movetarget)
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
