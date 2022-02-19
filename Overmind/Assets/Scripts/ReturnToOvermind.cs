using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToOvermind : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameObject directControlTarget;
    public static Vector3 RTSPlayerPosition;
    public static float RTSPlayerSize;
    public static Vector3 RTSPlayerRotation;
    private Vector3 targetPlayerPosition;
    private float targetPlayerSize;
    private Vector3 targetPlayerRotation;
    private float cameraZoomSpeed;
    private GameObject selectedUnit;
    private GameObject player;
    private bool movingCam;
    public Button assumeDirectControlButton;
    private float targetHeadHeight;
    private float targetCameraSize;
    private float cameraSizeDelta;
    private float decreaseCameraSizeAmount;
    public Button returnToOvermindButton;
    public GameObject cameraObject;
    Camera myCamera;

    // now you have a reference pointer to the camera, modify properties :


    void Start()
    {
        returnToOvermindButton.onClick.AddListener(OverMindViewInitalize);
        cameraZoomSpeed = 2f;
        myCamera = cameraObject.GetComponent<Camera>();
    }

    void OverMindViewInitalize()
    {
        selectedUnit = Select.SelectedUnits[0];
        player = GameObject.Find("/XR Rig");
        // RTSPlayerPosition = player.transform.position;
        // RTSPlayerSize = player.transform.localScale.x;
        // RTSPlayerRotation = player.transform.localEulerAngles;
        //  player.GetComponent<CameraControls>().OvermindCamPosition = RTSPlayerPosition;
        //  player.GetComponent<CameraControls>().OvermindCamRotation = RTSPlayerRotation;
        //  player.GetComponent<CameraControls>().OvermindCamScale = RTSPlayerSize;
        targetPlayerSize = player.GetComponent<CameraControls>().OvermindCamScale;
        targetPlayerPosition = player.GetComponent<CameraControls>().OvermindCamPosition;
        targetPlayerRotation = player.GetComponent<CameraControls>().OvermindCamRotation;
        Debug.Log("return position = " + player.transform.position + ", OvermindCamPostioin =" + targetPlayerPosition);
        player.GetComponent<CameraControls>().assumingDirectControl = false;
        selectedUnit.GetComponent<UnitBehaviour>().assumingDirectControl = false;
        selectedUnit.GetComponent<VRRig>().enabled = false;
        movingCam = true;
        if (selectedUnit.GetComponent<UnitBehaviour>().unitType == "Builder")
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
    }
    // Update is called once per frame
    void Update()
    {
        if (movingCam == true)
        {
            var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

            player = GameObject.Find("/XR Rig");
            Vector3 postitionDelta = targetPlayerPosition - player.transform.position;
            Vector3 decresePositionAmount = postitionDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.position += decresePositionAmount;


            Vector3 rotationDelta = targetPlayerRotation - player.transform.eulerAngles;
            Vector3 correctRotationAmount = rotationDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.localEulerAngles += correctRotationAmount;
            float sizeDelta = player.transform.localScale.x - targetPlayerSize;
            float decreaseSizeAmount = sizeDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);
            if (postitionDelta.y < 0.1f)
            {
                myCamera.nearClipPlane = 0.4f;
                myCamera.farClipPlane = 30000;
                GameObject leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller");
                GameObject rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller");
             //   leftHand.GetComponent<XRInteractorLineVisual>().enabled = true;
               // rightHand.GetComponent<XRInteractorLineVisual>().enabled = true;
                GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/DirectControlScreen").SetActive(false);
                player.transform.localScale = new Vector3(targetPlayerSize, targetPlayerSize, targetPlayerSize);
                player.transform.localEulerAngles = targetPlayerRotation;
                player.transform.position = targetPlayerPosition;
                if (volume.profile.TryGet<Vignette>(out var vignette))
                {
                    vignette.intensity.overrideState = true;
                    vignette.intensity.value = 0.47f;
                    vignette.smoothness.value = 0.20f;
                }
                if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
                {
                    colorAdjustments.hueShift.overrideState = false;
                    colorAdjustments.saturation.value = -10;
                    colorAdjustments.postExposure.overrideState = false;

                }
                if (volume.profile.TryGet<FilmGrain>(out var filmGrain))
                {
                    filmGrain.active = false;
                }
                GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/OvermindLeftHand").SetActive(true);
                GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/OvermindRightHand").SetActive(true);


            }
        }
    }



}
