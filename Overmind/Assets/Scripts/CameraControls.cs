using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    public Quaternion OvermindCamRotation;
    public float OvermindCamScale;
    public float OvermindDefaultCamScale;
    public bool assumingDirectControl;
    private bool initialRightPositionsSet;
    private bool initialLeftPositionsSet;
    private bool initialBothPositionsSet;
    public GameObject unitBeingControlled;
    public int cameraBoundTop;
    public int cameraBoundBottom;
    public int cameraBoundLeft;
    public int cameraBoundRight;


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
    GameObject player;

    public float maxFog;

    float initialAngle;
    float currentAngle;
    float deltaAngle;
    float angleToRotate;
    float currentAngleToRotate;
    public float angleVelocity;

    public InputActionReference leftJoystick = null;
    public InputActionReference rightJoystick = null;
    public InputActionReference directControlControllerButton = null;

    private Vector2 leftJoystickValue;
    private Vector2 rightJoystickValue;

    //switch to direct control
    public static GameObject directControlTarget;
    public static Vector3 RTSPlayerPosition;
    public static float RTSPlayerSize;
    public static Vector3 RTSPlayerRotation;
    public GameObject overmindLeftHand;
    public GameObject overmindRightHand;
    private Vector3 targetPlayerPosition;
    private float targetPlayerSize;
    private Quaternion targetPlayerRotation;
    private float cameraZoomSpeed;
    private GameObject selectedUnit;
    private bool movingCamToOvermindView;
    private bool movingCamToDirectControl;
    float newDeltaHeight;

    public GameObject assumeDirectControlButton;
    private float targetHeadHeight;
    private float targetCameraSize;
    private float cameraSizeDelta;
    private float decreaseCameraSizeAmount;
    public GameObject returnToOvermindButton;
    public GameObject directControlButton;
    public GameObject cameraObject;
    UnitBehaviour selectedUnitBehaviour;
    Camera myCamera;
    float directControlButtonValue;
    float timeSwitchingCameras = 0;



    // Start is called before the first frame update
    private void Start() {
        assumingDirectControl = false;
        initialRightPositionsSet = false;
        initialLeftPositionsSet = false;
        initialBothPositionsSet = false;
        player = GameObject.Find("/XR Rig");
        RightCursor = GameObject.Find("/UI/RightCursor");
        LeftCursor = GameObject.Find("/UI/LeftCursor");
        directControlButton = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/DirectControl");
        returnToOvermindButton = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/ReturnToOvermind");
        cameraZoomSpeed = 2.5f;
        myCamera = cameraObject.GetComponent<Camera>();
        GameObject leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller");
        GameObject rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller");
        var volume = GameObject.Find("Post Processing").GetComponent<Volume>();
    }


    // Update is called once per frame
    void Update() {
        directControlButtonValue = directControlControllerButton.action.ReadValue<float>();
        if (transform.position.x < cameraBoundLeft)
        {
            transform.position = new Vector3(cameraBoundLeft, transform.position.y, transform.position.z);
        }
        if (transform.position.x > cameraBoundRight)
        {
            transform.position = new Vector3(cameraBoundRight, transform.position.y, transform.position.z);

        }
        if (transform.position.z < cameraBoundBottom)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, cameraBoundBottom);

        }
        if (transform.position.z > cameraBoundTop)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, cameraBoundBottom);

        }
        if (transform.position.y < minHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, cameraBoundBottom);

        }
        if (assumingDirectControl == false)
        {
            if (directControlButtonValue == 1)
            {
                AssumeDirectControlInitalize();
            }
            rightGripValue = RightGripActionRefrence.action.ReadValue<float>();//get current value of grip button
            leftGripValue = LeftGripActionRefrence.action.ReadValue<float>();//get current value of grip button
            if (rightGripValue > 0 && leftGripValue == 0)//if only right grip is pressed
            {
                if (initialRightPositionsSet == false)
                {
                    if (transform.position.y > 50)
                    {

                        initialRightCursorPosition = RightCursor.transform.position;
                        initialPlayerPosition = transform.position;
                        initialRightPositionsSet = true;
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, RightCursor.transform.position) < 200)
                        {
                            initialRightCursorPosition = RightCursor.transform.position;
                            initialPlayerPosition = transform.position;
                            initialRightPositionsSet = true;
                        }
                    
                    }
                }
                else
                {
              
                Vector3 currentRightCursorPosition = RightCursor.transform.position;
                Vector3 positionDelta = initialRightCursorPosition - currentRightCursorPosition;
                positionDelta.y = 0;
                panVelocity += positionDelta * panSpeed;
                panVelocity *= panSmoothing;
                if (transform.position.y < 20)
                {
                    panVelocity *= 0.7f;
                }
                if (transform.position.x + panVelocity.x > cameraBoundLeft && transform.position.x + panVelocity.x < cameraBoundRight && transform.position.z + panVelocity.z > cameraBoundBottom && transform.position.z + panVelocity.z < cameraBoundTop)
                {
                    transform.position += panVelocity;
                    }
                }
            }
            else
            {
                initialRightPositionsSet = false;
            }
            if (leftGripValue > 0 && rightGripValue == 0)//if only left grip is pressed
            {
                if (initialLeftPositionsSet == false)
                {
                    if (transform.position.y > 50)
                    {
                        initialLeftCursorPosition = LeftCursor.transform.position;
                        initialPlayerPosition = transform.position;
                        initialLeftPositionsSet = true;
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, LeftCursor.transform.position) < 200)
                        {
                            initialLeftCursorPosition = LeftCursor.transform.position;
                            initialPlayerPosition = transform.position;
                            initialLeftPositionsSet = true;
                        }

                    }
                }
                else
                {
             
                Vector3 currentLeftCursorPosition = LeftCursor.transform.position;
                Vector3 positionDelta = initialLeftCursorPosition - currentLeftCursorPosition;
                positionDelta.y = 0;
                panVelocity += positionDelta * panSpeed;
                panVelocity *= panSmoothing;
        
                if (transform.position.x + panVelocity.x > cameraBoundLeft && transform.position.x + panVelocity.x < cameraBoundRight && transform.position.z + panVelocity.z > cameraBoundBottom && transform.position.z + panVelocity.z < cameraBoundTop)
                {
                    transform.position += panVelocity;
                    }
                }
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
                if (deltaDistance > transform.position.y)
                {
                    deltaDistance = transform.position.y;


                }
     
                newHeight = initialHeight + deltaDistance * zoomSpeed;
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
                if (zoomVelocity > 200)
                {

                    zoomVelocity = 200;
                }
                if (zoomVelocity < -200)
                {

                    zoomVelocity = -200;
                }
                zoomVelocity *= zoomSmoothing;
                float newAngle = (40f * ((transform.position.y - minHeight) / (maxHeight - minHeight)));
                if (newAngle > 89f)
                {
                    newAngle = 89f;

                }
                currentAngle = Mathf.Atan2(LeftCursor.transform.position.x - RightCursor.transform.position.x, LeftCursor.transform.position.z - RightCursor.transform.position.z) * Mathf.Rad2Deg;

                deltaAngle = initialAngle - currentAngle;
                transform.rotation = Quaternion.Euler(newAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
                angleToRotate = deltaAngle * rotateSpeed;
                if (currentAngleToRotate < angleToRotate)
                {
                    currentAngleToRotate += angleVelocity;


                }
                else
                {
                    currentAngleToRotate -= angleVelocity;


                }

                transform.RotateAround(initialMidPoint, Vector3.up, currentAngleToRotate);

                initialAngle = currentAngle;
     
                transform.position += panVelocity;
                transform.position = new Vector3(transform.position.x, transform.position.y + zoomVelocity, transform.position.z);


                float newFogDensity = maxFog - (maxFog * (transform.position.y / (maxHeight)));
  
                if (newFogDensity > maxFog)
                {
                    newFogDensity = maxFog;
                }
                RenderSettings.fogDensity = newFogDensity;

            }
            else
            {
                initialBothPositionsSet = false;
            }

            leftJoystickValue = leftJoystick.action.ReadValue<Vector2>();
            rightJoystickValue = rightJoystick.action.ReadValue<Vector2>();
            transform.position += new Vector3(leftJoystickValue.x, rightJoystickValue.y, leftJoystickValue.y) * panSpeed * transform.position.y * 0.01f;

        }
        else // if moving camera in or out of direct control
        {
            if (directControlButtonValue == 1 && movingCamToDirectControl == false && movingCamToDirectControl == false)

            {
                OverMindViewInitalize();
            }
            if (movingCamToDirectControl == true && movingCamToOvermindView == false)
            {
                timeSwitchingCameras += Time.deltaTime;
                var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

                Vector3 postitionDelta = targetPlayerPosition - player.transform.position;
                Vector3 decresePositionAmount = postitionDelta * cameraZoomSpeed * Time.deltaTime;
                player.transform.position += decresePositionAmount;


                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetPlayerRotation, 0.01f);

                if (cameraZoomSpeed < 2.5)
                {
                    cameraZoomSpeed += 0.05f;


                }
                float sizeDelta = player.transform.localScale.x - targetCameraSize;
                float decreaseSizeAmount = sizeDelta * cameraZoomSpeed * Time.deltaTime;
                player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);


                if (volume.profile.TryGet<Vignette>(out var vignette))
                {
                    if (vignette.smoothness.value < 1)
                    {
                        vignette.smoothness.value += 0.01f;
                    }
                    if (vignette.intensity.value < 2)
                    {

                        vignette.intensity.value += 0.01f;
                    }
                }
                if (postitionDelta.y > -1f || timeSwitchingCameras > 4)
                {
                    if (volume.profile.TryGet<Vignette>(out vignette))
                    {
                        vignette.intensity.overrideState = true;
                        vignette.intensity.value = 0.70f;
                        vignette.smoothness.value = 0.6f;
                    }
                    if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
                    {
                        colorAdjustments.hueShift.overrideState = true;
                        colorAdjustments.postExposure.overrideState = true;
                        colorAdjustments.hueShift.value = -10;
                        colorAdjustments.saturation.value = -50;
                        colorAdjustments.postExposure.value = 0.3f;

                    }
                    if (volume.profile.TryGet<FilmGrain>(out var filmGrain))
                    {
                        filmGrain.active = true;
                        filmGrain.intensity.overrideState = true;
                        filmGrain.intensity.value = 0.7f;
                    }
                    movingCamToDirectControl = false;
                    assumingDirectControl = true;

                    player.transform.localScale = new Vector3(targetPlayerSize, targetPlayerSize, targetPlayerSize);
                    player.transform.rotation = targetPlayerRotation;
                    player.transform.position = targetPlayerPosition;
                    GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/OvermindLeftHand").SetActive(false);
                    GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/OvermindRightHand").SetActive(false);
                    selectedUnitBehaviour.Deselect();
                    if (selectedUnitBehaviour.unitType == "Builder" || selectedUnitBehaviour.unitType == "Mech")
                    {
                        GameObject.Find("/XR Rig/Camera Offset/Main Camera/Builder Helmet").SetActive(true);
                    }


                    selectedUnitBehaviour.addOrderToQueue(new Order("enterDirectControl"));
                    returnToOvermindButton.SetActive(true);
                    player.GetComponent<Select>().SwitchLeftHandUIScreen("none");
                }
            }

            if (movingCamToOvermindView == true && movingCamToDirectControl == false)
            {

                var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

                Vector3 postitionDelta = targetPlayerPosition - player.transform.position;
                Vector3 decresePositionAmount = postitionDelta * cameraZoomSpeed * Time.deltaTime;
                player.transform.position += decresePositionAmount;

                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetPlayerRotation, 0.02f);

                float sizeDelta = player.transform.localScale.x - targetPlayerSize;
                float decreaseSizeAmount = sizeDelta * cameraZoomSpeed * Time.deltaTime;
                player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);
                timeSwitchingCameras += Time.deltaTime;

                if (cameraZoomSpeed < 2.5)
                {
                    cameraZoomSpeed += 0.05f;


                }
                if (postitionDelta.y < 2f || timeSwitchingCameras > 4)
                {

                    assumingDirectControl = false;
                    movingCamToDirectControl = false;

                    GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/OvermindLeftHand").SetActive(true);
                    GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/OvermindRightHand").SetActive(true);
                    myCamera.nearClipPlane = 0.4f;
                    myCamera.farClipPlane = 30000;
                    assumingDirectControl = false;


                    player.transform.localScale = new Vector3(targetPlayerSize, targetPlayerSize, targetPlayerSize);
                    player.transform.rotation = targetPlayerRotation;
                    player.transform.position = targetPlayerPosition;

                    if (volume.profile.TryGet<Vignette>(out var vignette))
                    {
                        vignette.intensity.overrideState = true;
                        vignette.intensity.value = 0.47f;
                        vignette.smoothness.value = 0.20f;
                    }
                    if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
                    {
                        colorAdjustments.active = false;
                    }
                    if (volume.profile.TryGet<FilmGrain>(out var filmGrain))
                    {
                        filmGrain.active = false;
                    }
                    selectedUnitBehaviour.orderQueue.Clear();

                    assumeDirectControlButton.SetActive(true);




                }
            }


        }

    }
    public void AssumeDirectControlInitalize() {
        Debug.Log("AssumeDirectControlInitalize 1");
        unitBeingControlled = Select.SelectedUnits[0];
        selectedUnit = Select.SelectedUnits[0];
        selectedUnitBehaviour = selectedUnit.GetComponent<UnitBehaviour>();
        Debug.Log("AssumeDirectControlInitalize 2 " + unitBeingControlled.name);
        cameraZoomSpeed = 0.5f;

        Debug.Log("AssumeDirectControlInitalize 3 " + selectedUnitBehaviour.unitType);

        if (selectedUnitBehaviour.faction == player.GetComponent<GlobalGameInformation>().player)
        {
            Debug.Log("AssumeDirectControlInitalize 4 ");

            selectedUnitBehaviour.addOrderToQueue(new Order("stop"));
            Select.SelectedUnits.Clear();
            selectedUnitBehaviour.orderQueue.Clear();
            timeSwitchingCameras = 0;


            myCamera.nearClipPlane = 0.05f;
            myCamera.farClipPlane = 30000;

            OvermindCamPosition = player.transform.position;
            OvermindCamRotation = player.transform.rotation;
            OvermindCamScale = player.transform.localScale.x;
            targetPlayerSize = selectedUnitBehaviour.cameraSizeWhenUnderDirectControl;
            targetHeadHeight = selectedUnit.transform.position.y + selectedUnitBehaviour.headHeight;
            targetPlayerPosition = selectedUnit.transform.position + new Vector3(0, targetHeadHeight, 0);
            targetPlayerRotation = selectedUnit.transform.rotation;
            assumingDirectControl = true;
            movingCamToDirectControl = true;
            movingCamToOvermindView = false;


        }

    }
    public void OverMindViewInitalize() {

        timeSwitchingCameras = 0;
        cameraZoomSpeed = 0.5f;
        selectedUnitBehaviour.addOrderToQueue(new Order("exitDirectControl"));
        selectedUnitBehaviour.UpdateUnit();
        movingCamToOvermindView = true;
        movingCamToDirectControl = false;
        timeSwitchingCameras = 0;
        targetPlayerSize = OvermindCamScale;
        targetPlayerPosition = OvermindCamPosition;
        targetPlayerRotation = OvermindCamRotation;
        selectedUnit = unitBeingControlled;
        selectedUnitBehaviour.assumingDirectControl = false;
        selectedUnitBehaviour.orderQueue.Clear();

        if (selectedUnit.GetComponent<UnitBehaviour>().unitType == "Builder" || selectedUnit.GetComponent<UnitBehaviour>().unitType == "Mech")
        {
            GameObject.Find("/XR Rig/Camera Offset/Main Camera/Builder Helmet").SetActive(false);
        }
        var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

        if (volume.profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0.5f;
            vignette.smoothness.value = 0.2f;
        }
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.hueShift.value = 00;
            colorAdjustments.saturation.value = -10;
            colorAdjustments.postExposure.value = 0f;
        }
        if (volume.profile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.active = false;
        }
        myCamera.nearClipPlane = 3f;
        myCamera.farClipPlane = 30000;
        selectedUnitBehaviour.addOrderToQueue(new Order("stop"));
        Debug.Log("return to overmind running2");

    }
}
