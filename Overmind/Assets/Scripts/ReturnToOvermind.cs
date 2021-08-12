using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    void Start()
    {
        returnToOvermindButton.onClick.AddListener(OverMindViewInitalize);

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
        targetCameraSize = player.GetComponent<CameraControls>().OvermindCamScale;
        targetPlayerPosition = player.GetComponent<CameraControls>().OvermindCamPosition;
        targetPlayerRotation = player.GetComponent<CameraControls>().OvermindCamRotation;
        Debug.Log("position = " + player.transform.position + ", OvermindCamPostioin =" + targetPlayerPosition);
        player.GetComponent<CameraControls>().assumingDirectControl = false;
        selectedUnit.GetComponent<UnitBehaviour>().assumingDirectControl = false;
        selectedUnit.GetComponent<VRRig>().enabled = false;
        movingCam = true;
        var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

        if (volume.profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0.5f;
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
            player = GameObject.Find("/XR Rig");
            Vector3 postitionDelta = targetPlayerPosition - player.transform.position;
            Vector3 decresePositionAmount = postitionDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.position += decresePositionAmount;


            Vector3 rotationDelta = targetPlayerRotation - player.transform.localEulerAngles;
            Vector3 correctRotationAmount = rotationDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.Rotate(correctRotationAmount);

            float sizeDelta = player.transform.localScale.x - targetPlayerSize;
            float decreaseSizeAmount = sizeDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);

            if (sizeDelta < 0.02 && postitionDelta.x < 0.05)
              {
                GameObject overmindLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/OvermindLeftHand");
                GameObject overmindRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/OvermindRightHand");
                overmindLeftHand.SetActive(true);
                overmindRightHand.SetActive(true);
                //  GameObject leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller");
                //   GameObject rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller");
                //  leftHand.GetComponent<LineRenderer>().enabled = false;
                //  rightHand.GetComponent<LineRenderer>().enabled = false;

                player.GetComponent<Select>().SwitchLeftHandUIScreen("none");
                 player.transform.localEulerAngles = targetPlayerRotation;
                player.transform.position = targetPlayerPosition;

                selectedUnit.GetComponent<UnitBehaviour>().Select();
                movingCam = false;


            }
        }
    }



}
