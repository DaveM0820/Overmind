using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class AssumeDirectControl : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionReference triggerActionRefrence = null;
    private float buttonValue;
    public static bool assumingDirectControl;
    public static GameObject directControlTarget;
    private Vector3 RTSPlayerPosition;
    private float RTSPlayerSize;
    private Quaternion RTSPlayerRotation;
    private Vector3 targetPlayerPosition;
    private float targetPlayerSize;
    private Quaternion targetPlayerRotation;
    private float cameraZoomSpeed;
    private bool zoomingOut;
    void Start()
    {
    assumingDirectControl = false;
        RTSPlayerPosition = transform.localScale;
        RTSPlayerSize = transform.position.x;
        RTSPlayerRotation = transform.rotation;
        targetPlayerPosition = directControlTarget.transform.position;
        targetPlayerSize = directControlTarget.GetComponent<UnitBehaviour>().unitSize;
        targetPlayerRotation = directControlTarget.transform.rotation;
        cameraZoomSpeed = 0.01f;
        zoomingOut = false;
}

    // Update is called once per frame
    void Update()
    {
        buttonValue = triggerActionRefrence.action.ReadValue<float>();
        // Debug.Log("selectedUnits.Count " + selectedUnits.Count);

        if (buttonValue == 1)//if trigger is pressed
        {
            if (assumingDirectControl == false)
            {
                zoomingOut = true;
            }

        }
        else
        {
            if (zoomingOut = true)
            {


            }
     
            //defaultPlayerRotation 

        }


    }



}
