using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanAndZoom : MonoBehaviour
{
    public InputActionReference ButtonHeldActionRefrence = null;
    //public InputActionReference InitPressActionRefrence = null;
    private float ButtonValue;
    private Vector3 Armpos;
    private Transform Campos;
    private Transform CamposAngleCorrection;
    private Vector3 InitArmHeadDelta;
    private Vector3 InitPlayerPosition;
    private Vector3 CurrentArmHeadDelta;
    private Vector3 Distancecontrollermoved;
    private float NewX;
    private float NewZ;
    private float CameraHeightChange;
    public float MaxZoom;
    public float MinZoom;
    public float ZoomSpeed;
    public float PanSpeed;
    public float armDistanceFromCenter;

    // Start is called before the first frame update
    private void Start()
    {
        PanSpeed = 4f;
        ZoomSpeed = 0.01f; //approximate distance movable in one arm length 
        MaxZoom = 500; //max camera height in m
        MinZoom = 5; //min camera height in m

    }
    

    // Update is called once per frame
    void Update()
    {
        ButtonValue = ButtonHeldActionRefrence.action.ReadValue<float>();//get current value of grip button
        Armpos = GameObject.FindWithTag("RightController").transform.position;
        Campos = GameObject.FindWithTag("MainCamera").transform;
        Campos.Rotate(45f, 0f, 0f, Space.Self);
        CurrentArmHeadDelta = Campos.InverseTransformPoint(Armpos);//find arm position relative to camera

        if (ButtonValue > 0)//if grip is pressed
        {
            CamposAngleCorrection = Campos;
            armDistanceFromCenter = 1 / (Armpos.x + Armpos.y); 
            Distancecontrollermoved = InitArmHeadDelta - CurrentArmHeadDelta; //distance the controller moved is the difference between controller inital position and current positi
            CameraHeightChange = transform.position.y * ZoomSpeed * Distancecontrollermoved.z * -1;
            //the amount the camera hight should change is based on current player height, zoom speed, and how far the player's arm has moved on the z axis relative to the head since pressing the button
            InitArmHeadDelta.z = InitArmHeadDelta.z - (Distancecontrollermoved.z * 0.1f);
            //make it so the camera stops moving when you stop moving the arm by moving the initial position towards the current controller position, thereby reducing CameraHeightChange (WIP)
            if (transform.position.y + CameraHeightChange > MaxZoom) //if beyond max zoom
            {
                transform.position = new Vector3(transform.position.x, MaxZoom, transform.position.z); //don't let player go past max zoom
                InitArmHeadDelta.z = CurrentArmHeadDelta.z; //reset initial z position so player have to move the controler back to where they first pressed the button to to zoom back in
            }
            else if(transform.position.y + CameraHeightChange < MinZoom) //if below min zoom
            {
                transform.position = new Vector3(transform.position.x, MinZoom, transform.position.z);
                InitArmHeadDelta.z = CurrentArmHeadDelta.z;

            } else 
            {
                transform.position = transform.position + new Vector3(transform.position.x, CameraHeightChange, transform.position.z); //change player height by CameraHeightChange
            }
            if (transform.position.y < InitPlayerPosition.y)
            {
                NewX = InitPlayerPosition.x + (transform.position.y * Distancecontrollermoved.x * PanSpeed);// panning left and right 
                NewZ = InitPlayerPosition.z + (transform.position.y * Distancecontrollermoved.y * PanSpeed);// 
            }
            else
            {
                NewX = InitPlayerPosition.x + (transform.position.y * Distancecontrollermoved.x * PanSpeed);// panning left and right 
                NewZ = InitPlayerPosition.z + (transform.position.y * Distancecontrollermoved.y * PanSpeed);// 
            }

            //transform.position = new Vector3(NewX, transform.position.y, NewZ);
        }
        else //if grip is not being pressed
        {
            InitArmHeadDelta = CurrentArmHeadDelta;//update inital controller position to = current controller position
            InitPlayerPosition = transform.position;
        }
    }

}
