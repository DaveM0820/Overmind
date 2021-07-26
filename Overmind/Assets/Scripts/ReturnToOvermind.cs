using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class ReturnToOvermind : MonoBehaviour
{
    // Start is called before the first frame update
    private float buttonValue;
    public static bool assumingDirectControl;
    public static GameObject directControlTarget;
    private Vector3 RTSPlayerPosition;
    private float RTSPlayerSize;
    private Quaternion RTSPlayerRotation;
    private Vector3 targetPlayerPosition;
    private float targetPlayerSize;
    private Vector3 targetPlayerRotation;
    public float cameraZoomSpeed;
    private GameObject selectedUnit;
    private GameObject player;
    private bool movingCam;
    public Button returnToOvermindButton;
    private float targetHeadHeight;

    void Start()
    {
        returnToOvermindButton.onClick.AddListener(OverMindViewInitalize);

    }

    void OverMindViewInitalize()
    { 
        player = GameObject.Find("/XR Rig");
        targetPlayerSize = player.GetComponent<CameraControls>().OvermindCamScale;
        targetPlayerPosition = player.GetComponent<CameraControls>().OvermindCamPosition;
        targetPlayerRotation = player.GetComponent<CameraControls>().OvermindCamRotation;
        movingCam = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingCam == true)
        {
            Vector3 postitionDelta = targetPlayerPosition - player.transform.position;
            float sizeDelta = player.transform.localScale.x - targetPlayerSize;
            Vector2 rotationDelta = targetPlayerRotation - player.transform.localEulerAngles;

            float decreaseSizeAmount = 0.01f * sizeDelta * cameraZoomSpeed;
            player.transform.localScale -= new Vector3(decreaseSizeAmount, decreaseSizeAmount, decreaseSizeAmount);

            Vector3 decresePositionAmount = 0.01f * postitionDelta * cameraZoomSpeed;
            player.transform.position += decresePositionAmount;

            Vector3 correctRotationAmount = 0.05f * rotationDelta * cameraZoomSpeed;
            player.transform.Rotate(correctRotationAmount);

            if (sizeDelta < 0.02 && postitionDelta.x < 0.05)
              {
                player.GetComponent<Select>().SwitchLeftHandUIScreen("none");
                player.GetComponent<CameraControls>().assumingDirectControl = false;
                movingCam = false;


            }
        }
    }



}
