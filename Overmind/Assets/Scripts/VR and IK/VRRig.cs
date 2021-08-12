using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class VRRig : MonoBehaviour
{
    public Transform rigTargetRightHand;
    public Transform rigTargetLeftHand;
    public Transform rigTargetHead;
    public Transform rigTargetneck;
    public Transform vrTargetLeftHand;
    public Transform vrTargetRightHand;
    public Transform rigTargetRightHand2;
    public GameObject helmet;
    public GameObject unitMeshAndRig;
    // Start is called before the first frame update
    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public InputActionReference leftJoystick = null;
    public InputActionReference rightJoystick = null;
    public Vector2 leftJoystickValue;
    public Vector2 rightJoystickValue;

    private Animator animator;

    public Vector3 velocity;
    public float rotationVelocity;
    public float dampening;
    public float rotationDampening;

    public bool snappedRight;
    public bool snappedLeft;

    private GameObject player;

    void OnEnable()
    {
        player = GameObject.Find("/XR Rig");
        vrTargetLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
        vrTargetRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetRightHand").transform;

        if (gameObject.GetComponent<UnitBehaviour>().unitType == "Builder")
        {
            // rigTargetRightHand2 = transform.Find("Builder_LOD0/Armature/Root/Spine1/Spine2/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L");
            // rigTargetLeftHand2 = transform.Find("Builder_LOD0/Armature/Root/Spine1/Spine2/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R");
            unitMeshAndRig = transform.Find("LOD0").gameObject;
            rigTargetRightHand = transform.Find("LOD0/VR Constraints/Right Arm IK/Target");
            rigTargetLeftHand = transform.Find("LOD0/VR Constraints/Left Arm IK/Target");
            rigTargetHead = transform.Find("LOD0/Armature/Root/Spine1/Spine2/Neck/Bone.004/Head");
            rigTargetneck = transform.Find("LOD0/Armature/Root/Spine1/Spine2/Neck");
            headConstraint = transform.Find("LOD0/VR Constraints/Head Constraint");
            helmet = GameObject.Find("/XR Rig/Camera Offset/Main Camera/Builder Helmet");
            animator = unitMeshAndRig.GetComponent<Animator>();

       }
        animator.SetBool("DirectControl", true);

        headBodyOffset = transform.position - headConstraint.position;
        snappedRight = false;
        snappedLeft = false;
        helmet.SetActive(true);
        unitMeshAndRig.GetComponent<RigBuilder>().enabled = true;
        //leftHand.vrTarget = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/VRTargetLeftHand").transform;
        //rightHand.vrTarget = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/VRTargetLeftHand").transform;

        rigTargetneck.localScale = new Vector3(0, 0, 0);
    }
    void OnDisable()
    {
        animator.SetBool("DirectControl", false);

        helmet.SetActive(false);
        unitMeshAndRig.GetComponent<RigBuilder>().enabled = false;

        rigTargetneck.localScale = new Vector3(1, 1, 1);
            
    }
    // Update is called once per frame
    void Update()
    {
        leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
        rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();
        velocity += transform.rotation * new Vector3(leftJoystickValue.x,0, leftJoystickValue.y) * this.GetComponent<UnitBehaviour>().moveSpeed * Time.deltaTime * 0.1f;
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
        player.transform.position = new Vector3(transform.position.x, player.transform.position.y , transform.position.z);
        rigTargetLeftHand.position = vrTargetLeftHand.position;// TransformPoint(0, 0, 0);
        rigTargetRightHand.position = vrTargetRightHand.position;// TransformPoint(0,0,0);
        rigTargetLeftHand.rotation = vrTargetLeftHand.rotation;
        rigTargetRightHand.rotation = vrTargetRightHand.rotation;
        
    }

}
 