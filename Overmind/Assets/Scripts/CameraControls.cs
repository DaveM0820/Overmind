using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    public InputActionReference RightGripActionRefrence = null;
    public InputActionReference LeftGripActionRefrence = null;

    //public InputActionReference InitPressActionRefrence = null;
    private bool initialPositionsSet;
    private float rightGripValue;
    private float leftGripValue;

    private Vector3 initialRightCursorPosition;
    private Vector3 initialLeftCursorPosition;

    private GameObject RightCursor;
    private GameObject LeftCursor;

    public float maxHeight;
    public float minHeight;
    public float zoomSpeed;
    public float panSpeed;
    public float rotateSpeed;
    public float zoomSmoothing;
    public float panSmoothing;
    public float rotateSmoothing;

    public Vector3 OvermindCamPosition;
    public Vector3 OvermindCamRotation;
    public float OvermindCamScale;
    public float OvermindDefaultCamScale;
    public bool assumingDirectControl;
    private bool initialRightPositionsSet;
    private bool initialLeftPositionsSet;
    private bool initialBothPositionsSet;

    float currentDistance;
    float deltaDistance;
    float initialDistance;
    float initialHeight;
    float zoomVelocity;
    Vector3 panVelocity;
    Vector3 initialPlayerPosition;
    Vector3 initialMidPoint;
    Vector3 currentMidPoint;
    float newHeight;
    private GameObject player;

    public float maxFog;

    float initialAngle;
    float currentAngle;
    float deltaAngle;
    float angleToRotate;
    float angleVelocity;

    GameObject virtualLeftCursor;
    GameObject virtualRightCursor;
    Vector3 virtualLeftCursorVelocity;
    Vector3 virtualRightCursorVelocity;
    Vector3 virtualRightCursorPositionDelta;
    Vector3 virtualLeftCursorPositionDelta;

    // Start is called before the first frame update
    private void Start()
    {
        assumingDirectControl = false;
        initialRightPositionsSet = false;
        initialLeftPositionsSet = false;
        initialBothPositionsSet = false;
        player = GameObject.Find("/XR Rig");
        RightCursor = GameObject.Find("/UI/RightCursor");
        LeftCursor = GameObject.Find("/UI/LeftCursor");
        virtualLeftCursor = Instantiate(RightCursor);
       virtualRightCursor = Instantiate(LeftCursor);
            virtualLeftCursorVelocity = new Vector3(0,0,0);
        virtualRightCursorVelocity = new Vector3(0, 0, 0);
    }


    // Update is called once per frame
    void Update()
    {
        if (assumingDirectControl == false)
        {          
            
            rightGripValue = RightGripActionRefrence.action.ReadValue<float>();//get current value of grip button
            leftGripValue = LeftGripActionRefrence.action.ReadValue<float>();//get current value of grip button
            if (rightGripValue > 0 && leftGripValue == 0)//if only right grip is pressed
            {
                if (initialRightPositionsSet == false)
                {
                    initialRightCursorPosition = RightCursor.transform.position;
                    initialPlayerPosition = transform.position;
                    initialRightPositionsSet = true;
                }
                Vector3 currentRightCursorPosition = RightCursor.transform.position;
                Vector3 positionDelta = initialRightCursorPosition - currentRightCursorPosition;
                positionDelta.y = 0;
                panVelocity += positionDelta * panSpeed;
                panVelocity *= panSmoothing;
                transform.position += panVelocity;
            }
            else
            {
                initialRightPositionsSet = false;
            }
            if (leftGripValue > 0 && rightGripValue == 0)//if only left grip is pressed
            {
                if (initialLeftPositionsSet == false)
                {
                    initialLeftCursorPosition = LeftCursor.transform.position;
                    initialPlayerPosition = transform.position;
                    initialLeftPositionsSet = true;
                }
                Vector3 currentLeftCursorPosition = LeftCursor.transform.position;
                Vector3 positionDelta = initialLeftCursorPosition - currentLeftCursorPosition;
                positionDelta.y = 0;
                panVelocity += positionDelta * panSpeed;
                panVelocity *= panSmoothing;
                transform.position += panVelocity;
            }
            else
            {
                initialLeftPositionsSet = false;
            }
            if (leftGripValue > 0 && rightGripValue > 0)//both are pressed
            {

                if (initialBothPositionsSet == false)
                {

                    initialRightCursorPosition = RightCursor.transform.position;
                    initialLeftCursorPosition = LeftCursor.transform.position;
                    initialMidPoint = (initialLeftCursorPosition + initialRightCursorPosition) / 2;
                    initialDistance = Vector3.Distance(initialLeftCursorPosition, initialRightCursorPosition);
                    initialPlayerPosition = transform.position;
                    initialHeight = transform.position.y;
                    initialAngle = Mathf.Atan2(LeftCursor.transform.position.x - RightCursor.transform.position.x, LeftCursor.transform.position.z - RightCursor.transform.position.z) * Mathf.Rad2Deg;
                    initialBothPositionsSet = true;


                }
                currentMidPoint = (LeftCursor.transform.position + RightCursor.transform.position) / 2;
                Vector3 positionDelta = initialMidPoint - currentMidPoint;
                panVelocity += positionDelta * panSpeed;
                panVelocity *= panSmoothing;
                currentDistance = Vector3.Distance(LeftCursor.transform.position, RightCursor.transform.position);
                deltaDistance = initialDistance - currentDistance;
                newHeight = initialHeight + (deltaDistance*zoomSpeed);
                if (newHeight > maxHeight)
                {
                    newHeight = maxHeight;
                }
                else if (newHeight < minHeight)
                {
                    newHeight = minHeight;
                }
                float deltaHeight = transform.position.y - newHeight;
                zoomVelocity -= deltaHeight;
                zoomVelocity *= zoomSmoothing;
                float newAngle = 45f + (40f * ((transform.position.y - minHeight) / (maxHeight - minHeight)));
                if (newAngle > 89f)
                {
                    newAngle = 89f;

                }
                currentAngle = Mathf.Atan2(LeftCursor.transform.position.x - RightCursor.transform.position.x, LeftCursor.transform.position.z - RightCursor.transform.position.z) * Mathf.Rad2Deg;

                deltaAngle = initialAngle - currentAngle;    
                transform.rotation = Quaternion.Euler(newAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
                angleToRotate = deltaAngle * rotateSpeed;

               // angleToRotate *= initialAngle - currentAngle;
            
               transform.RotateAround(initialMidPoint, Vector3.up, angleToRotate);
               
               initialAngle = currentAngle;
                //transform.position += panVelocity;
                transform.position = new Vector3(transform.position.x + panVelocity.x, transform.position.y + zoomVelocity, transform.position.z + panVelocity.z);

                // transform.RotateAround(initialMidPoint, Vector3.up, deltaAngle);

                float newFogDensity = maxFog - (maxFog * (transform.position.y/maxHeight));
                if (newFogDensity < 0.00015f)
                {
                    newFogDensity = 0.00015f;
                }
                if (newFogDensity > 0.002f)
                {
                    newFogDensity = 0.002f;
                }
            }
            else
            {
                initialBothPositionsSet = false;
            }

        }



    }


}
