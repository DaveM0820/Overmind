using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class AssumeDirectControl : MonoBehaviour
{
    // Start is called before the first frame update
    private float buttonValue;

    public static GameObject directControlTarget;
    public static Vector3 RTSPlayerPosition;
    public static float RTSPlayerSize;
    public static Vector3 RTSPlayerRotation;
    private Vector3 targetPlayerPosition;
    private float targetPlayerSize;
    private Vector3 targetPlayerRotation;
    public float cameraZoomSpeed;
    private GameObject selectedUnit;
    private GameObject player;
    private bool movingCam;
    public Button assumeDirectControlButton;
    private float targetHeadHeight;


    void Start()
    {
        assumeDirectControlButton.onClick.AddListener(AssumeDirectControlInitalize);

    }

    void AssumeDirectControlInitalize()
    {
        selectedUnit = Select.SelectedUnits[0];
        player = GameObject.Find("/XR Rig");
        RTSPlayerPosition = player.transform.position;
        RTSPlayerSize = player.transform.localScale.x;
        RTSPlayerRotation = player.transform.localEulerAngles;
        player.GetComponent<CameraControls>().OvermindCamPosition = RTSPlayerPosition;
        player.GetComponent<CameraControls>().OvermindCamRotation = RTSPlayerRotation;
        player.GetComponent<CameraControls>().OvermindCamScale = RTSPlayerSize;
        targetPlayerSize = selectedUnit.GetComponent<UnitBehaviour>().cameraSizeWhenUnderDirectControl;
        targetHeadHeight = selectedUnit.GetComponent<UnitBehaviour>().headHeight;
        targetPlayerPosition = selectedUnit.transform.position + new Vector3(0, targetHeadHeight, 0);
        targetPlayerRotation = selectedUnit.transform.localEulerAngles;
        selectedUnit.GetComponent<MoveToPosition>().SetMovePosition(selectedUnit.transform.position);
        movingCam = true;
    }
    // Update is called once per frame
    void Update()
    {

        if (movingCam == true)
        {
            Vector3 postitionDelta = targetPlayerPosition - player.transform.position;
            Vector3 rotationDelta = targetPlayerRotation - player.transform.localEulerAngles;
            float sizeDelta = player.transform.localScale.x - targetPlayerSize;

            float decreaseSizeAmount = 0.01f * sizeDelta * cameraZoomSpeed;
            player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);

            Vector3 decresePositionAmount = 0.005f * postitionDelta * cameraZoomSpeed;
            player.transform.position += decresePositionAmount;

           // if (sizeDelta < 4 && postitionDelta.x < 20)
           // {
                Vector3 correctRotationAmount = 0.005f * rotationDelta * cameraZoomSpeed;
                player.transform.Rotate(correctRotationAmount);
           // }
            if (sizeDelta < 0.1 && postitionDelta.x < 0.1)
              {
                Debug.Log("Done moving");
                player.GetComponent<Select>().SwitchLeftHandUIScreen("DirectControlScreen");
                player.GetComponent<CameraControls>().assumingDirectControl = true;
                movingCam = false;
              }
        }
    }



}
