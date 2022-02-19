using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class AssumeDirectControl : MonoBehaviour
{
    // Start is called before the first frame update

    public static GameObject directControlTarget;

    private Vector3 targetPlayerPosition;
    private float targetPlayerSize;
    private Quaternion targetPlayerRotation;
    private float cameraZoomSpeed;
    private GameObject selectedUnit;
    private GameObject player;
    private bool movingCam;
    public Button assumeDirectControlButton;
    private float targetHeadHeight;
    private float targetCameraSize;
    private float cameraSizeDelta;
    private float decreaseCameraSizeAmount;
    private float updateTimestep;
    public GameObject cameraObject;
    Camera myCamera;
    UnitBehaviour selectedUnitBehaviour;
    void Start()
    {
        assumeDirectControlButton.onClick.AddListener(AssumeDirectControlInitalize);
        cameraZoomSpeed = 2f;
        myCamera = cameraObject.GetComponent<Camera>();


    }

    void AssumeDirectControlInitalize()
    {
        var volume = GameObject.Find("Post Processing").GetComponent<Volume>();
        selectedUnit = Select.SelectedUnits[0];
        selectedUnitBehaviour = selectedUnit.GetComponent<UnitBehaviour>();
        player = GameObject.Find("/XR Rig");
        player.GetComponent<CameraControls>().OvermindCamPosition = player.transform.position;
        player.GetComponent<CameraControls>().OvermindCamRotation = player.transform.localEulerAngles;
        player.GetComponent<CameraControls>().OvermindCamScale = player.transform.localScale.x;
        player.GetComponent<CameraControls>().unitBeingControlled = selectedUnit;
        targetCameraSize = selectedUnitBehaviour.cameraSizeWhenUnderDirectControl;
        targetPlayerSize = 1f;
        targetHeadHeight = selectedUnit.transform.position.y + selectedUnitBehaviour.headHeight;
        targetPlayerPosition = selectedUnit.transform.position + new Vector3 (0, targetHeadHeight, 0);
        targetPlayerRotation = selectedUnit.transform.rotation;
        player.GetComponent<CameraControls>().assumingDirectControl = true;
        selectedUnitBehaviour.assumingDirectControl = true;
        selectedUnitBehaviour.addOrderToQueue(new Order("stop"));
        movingCam = true;

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


            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetPlayerRotation, 0.02f);
                
                
            float sizeDelta = player.transform.localScale.x - targetPlayerSize;
            float decreaseSizeAmount = sizeDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);
             if (volume.profile.TryGet<Vignette>(out var vignette))
             {
                 if (vignette.smoothness.value < 1) {
                        vignette.smoothness.value += 0.01f;
                 }
                 if (vignette.intensity.value < 2) 
                 {

                     vignette.intensity.value += 0.01f;
                 }
             }
            if (postitionDelta.y > -0.1f)
              {
                movingCam = false;
                player.GetComponent<CameraControls>().assumingDirectControl = true;

                myCamera.nearClipPlane = 0.05f;
                myCamera.farClipPlane = 30000;
                GameObject leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller");
                  GameObject rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller");
                // leftHand.GetComponent<XRInteractorLineVisual>().enabled = false;
                 //rightHand.GetComponent<XRInteractorLineVisual>().enabled = false;
                //GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/DirectControlScreen").SetActive(true);
                player.transform.localScale = new Vector3(targetPlayerSize, targetPlayerSize, targetPlayerSize);
                player.transform.rotation = targetPlayerRotation;
                player.transform.position = targetPlayerPosition;
                GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/OvermindLeftHand").SetActive(false);
                GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/OvermindRightHand").SetActive(false);
                selectedUnitBehaviour.Deselect();
                if (selectedUnitBehaviour.unitType == "Builder")
                {
                    GameObject.Find("/XR Rig/Camera Offset/Main Camera/Builder Helmet").SetActive(true);
                }
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
                    filmGrain.intensity.value = 0.6f;
                }
                selectedUnitBehaviour.addOrderToQueue(new Order("enterDirectControl"));


            }
        }
    }



}
