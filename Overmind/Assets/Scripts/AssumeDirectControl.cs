using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AssumeDirectControl : MonoBehaviour
{
    // Start is called before the first frame update

    public static GameObject directControlTarget;

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

    void Start()
    {
        assumeDirectControlButton.onClick.AddListener(AssumeDirectControlInitalize);
        cameraZoomSpeed = 2f;


    }

    void AssumeDirectControlInitalize()
    {
        var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

        selectedUnit = Select.SelectedUnits[0];
        player = GameObject.Find("/XR Rig");
        player.GetComponent<CameraControls>().OvermindCamPosition = player.transform.position;
        player.GetComponent<CameraControls>().OvermindCamRotation = player.transform.localEulerAngles;
        player.GetComponent<CameraControls>().OvermindCamScale = player.transform.localScale.x;
        targetCameraSize = selectedUnit.GetComponent<UnitBehaviour>().cameraSizeWhenUnderDirectControl;
        targetPlayerSize = 1f;
        targetHeadHeight = selectedUnit.GetComponent<UnitBehaviour>().headHeight;
        targetPlayerPosition = selectedUnit.transform.position + new Vector3 (0, targetHeadHeight, 0);
        targetPlayerRotation = selectedUnit.transform.localEulerAngles;
        player.GetComponent<CameraControls>().assumingDirectControl = true;
        selectedUnit.GetComponent<UnitBehaviour>().assumingDirectControl = true;
                movingCam = true;
        if (volume.profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 1;
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
            Vector3 correctRotationAmount = rotationDelta * cameraZoomSpeed * Time.deltaTime*0.01f;
            player.transform.rotation = Quaternion.Euler(correctRotationAmount);
            float sizeDelta = player.transform.localScale.x - targetPlayerSize;
            float decreaseSizeAmount = sizeDelta * cameraZoomSpeed * Time.deltaTime;
            player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);


            if (postitionDelta.y < 0.02f && postitionDelta.x < 0.02f && postitionDelta.z < 0.02f)
              {

                Debug.Log("Done moving");
                GameObject overmindLeftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/OvermindLeftHand");
                GameObject overmindRightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller/OvermindRightHand");
                overmindLeftHand.SetActive(false);
                overmindRightHand.SetActive(false);
              //  GameObject leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller");
             //   GameObject rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller");
              //  leftHand.GetComponent<LineRenderer>().enabled = false;
              //  rightHand.GetComponent<LineRenderer>().enabled = false;

                player.GetComponent<Select>().SwitchLeftHandUIScreen("DirectControlScreen");
                player.transform.localScale = new Vector3(targetPlayerSize, targetPlayerSize, targetPlayerSize);
                player.transform.localEulerAngles = targetPlayerRotation;
                player.transform.position = targetPlayerPosition;
                selectedUnit.GetComponent<VRRig>().enabled = true;
                selectedUnit.GetComponent<UnitBehaviour>().Deselect();
                movingCam = false;
                var volume = GameObject.Find("Post Processing").GetComponent<Volume>();

                if (volume.profile.TryGet<Vignette>(out var vignette))
                {
                    vignette.intensity.overrideState = true;
                    vignette.intensity.value = 0.9f;
                }
                if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
                {
                    colorAdjustments.hueShift.value = -10;
                    colorAdjustments.saturation.value = -50;
                    colorAdjustments.postExposure.value = 0.2f;

                }
                if (volume.profile.TryGet<FilmGrain>(out var filmGrain))
                {
                    filmGrain.active = true;
                    filmGrain.intensity.overrideState = true;
                    filmGrain.intensity.value = 0.5f;
                }
            }
        }
    }



}
