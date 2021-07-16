using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Select : MonoBehaviour
{
    public InputActionReference SelectActionRefrence = null;
    private float ButtonValue;

    private bool DrawingSelectionBox;
    private Vector3 StartSelectionBox;

    private RaycastHit RightRaycastHit;
    private RaycastHit LeftRaycastHit;

    private Transform RightArmPosition;
    private Transform LeftArmPosition;

    private GameObject RightCursor;
    private GameObject LeftCursor;
    private GameObject SelectionBox = null;

    private GameObject buildingScreen;
    private GameObject unitScreen;
    private GameObject builderUnitScreen;

    public static List<GameObject> SelectedUnits = null;

    void Start()
    {
        DrawingSelectionBox = false;
        ButtonValue = 0;
        RightCursor = GameObject.Find("/Cursor");
        LeftCursor = Instantiate(RightCursor);
        RightArmPosition = GameObject.FindWithTag("RightController").transform;
        LeftArmPosition = GameObject.FindWithTag("LeftController").transform;
        SelectedUnits = new List<GameObject>();
        //int doubleclickcounter = 0;
        SwitchLeftHandUIScreen("none");
    }

    // Update is called once per frame
    void Update()
    { 
        Physics.Raycast(RightArmPosition.position, RightArmPosition.TransformDirection(Vector3.forward), out RightRaycastHit, Mathf.Infinity);
        Physics.Raycast(LeftArmPosition.position, LeftArmPosition.TransformDirection(Vector3.forward), out LeftRaycastHit, Mathf.Infinity);
        RightCursor.transform.position = RightRaycastHit.point;
        LeftCursor.transform.position = LeftRaycastHit.point;
        ButtonValue = SelectActionRefrence.action.ReadValue<float>();
        Debug.Log("select script update running, a button value: " + ButtonValue);

        if (ButtonValue == 1) //if the a button is pressed
        {
            if (DrawingSelectionBox == false)
            {
                if (RightRaycastHit.collider.gameObject.transform.parent.tag == "Units") //if you click a single unit or building, select it
                {
                  DeselectUnits();
                  SelectedUnits.Add(RightRaycastHit.collider.gameObject);
                    if (RightRaycastHit.collider.gameObject.GetComponent<UnitBehaviour>().unitType == "builder")
                    {        
                        SwitchLeftHandUIScreen("BuilderUnitScreen");
                    }
                    else
                    {
                        SwitchLeftHandUIScreen("UnitScreen");
                    }
                }    
                else if (RightRaycastHit.collider.gameObject.transform.parent.tag == "Buildings")
                {
                    DeselectUnits();
                    SelectedUnits.Add(RightRaycastHit.collider.gameObject);
                    SwitchLeftHandUIScreen("BuildingScreen");

                }
                else
                {
                    //start drawing selection box
                    DeselectUnits();
                    SelectionBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Renderer cubeRenderer = SelectionBox.GetComponent<Renderer>();
                    cubeRenderer.material.SetColor("_Color", Color.blue);
                    SelectionBox.name = "SelectionBox";
                    StartSelectionBox.x = RightRaycastHit.point.x;
                    StartSelectionBox.z = RightRaycastHit.point.z;
                    StartSelectionBox.y = 1;
                    SelectionBox.transform.position = StartSelectionBox;
                    DrawingSelectionBox = true;
                }

            }
            else //if you are drawing selction box
            {
                //continue drawing selection box
                Vector2 LowerLeft = new Vector2(Mathf.Min(StartSelectionBox.x, RightRaycastHit.point.x), Mathf.Min(StartSelectionBox.z, RightRaycastHit.point.z));
                Vector2 UpperRight = new Vector2(Mathf.Max(StartSelectionBox.x, RightRaycastHit.point.x), Mathf.Max(StartSelectionBox.z, RightRaycastHit.point.z));
                Vector2 DistanceDragged = UpperRight - LowerLeft;
                SelectionBox.transform.position = new Vector3(LowerLeft.x + (0.5f * DistanceDragged.x), 1, LowerLeft.y + (0.5f * DistanceDragged.y));
                SelectionBox.transform.localScale = new Vector3(DistanceDragged.x, 1, DistanceDragged.y);
            }

        } else {//if button is not being pushed
            if (DrawingSelectionBox == true)// and the selection box was being drawn
            { 
                /*basically, go through all the hit detections in the box and find out what kind of objects are there, if it's a mix only select the military units,
                 if it's a mix of builders and buildings only select the builders, if only buildings are within the box then select them all*/

                //select units within
                bool hasMilitaryunits = false;
                bool hasBuildings = false;
                bool hasBuilderUnits = false;
                Collider[] hitColliders = Physics.OverlapBox(SelectionBox.transform.position, SelectionBox.transform.localScale / 2, Quaternion.identity);//get a list of objects within the selection box
                int counter = 0;
                while (counter < hitColliders.Length) //go through hit detections and see what kind of units are selected
                {
                    if (hitColliders[counter].name != "SelectionBox") { // if not looking at the selection box itself
                        if (hitColliders[counter].transform.parent.name == "Units")//if a unit is selected
                        {
                            if (hitColliders[counter].gameObject.GetComponent<UnitBehaviour>().unitType != "builder")//and its a builder 
                            {
                                hasBuilderUnits = true;//a builder is selected
                            } else
                            {
                                hasMilitaryunits = true;//if its not a builder a military unit selected
                            }
                        } else if (hitColliders[counter].transform.parent.name == "Buildings") // if a building is selected
                            {
                                hasBuildings = true; //there is a building selected, duh
                            }

                    }
                    counter++;
                }
                if (hasMilitaryunits == true) //if military units are detected select only the military units
                {
                    counter = 0;
                    while (counter < hitColliders.Length)
                    {
                        if (hitColliders[counter].transform.parent.name == "Units" && hitColliders[counter].gameObject.GetComponent<UnitBehaviour>().unitType != "builder")
                        {
                            SelectedUnits.Add(hitColliders[counter].gameObject);
                        }
                        counter++;
                    }
                    SwitchLeftHandUIScreen("UnitScreen");
                } else // if no military units are detected
                {
                    if (hasBuilderUnits == true) //and builders are detected
                    {
                        counter = 0;
                        while (counter < hitColliders.Length)//go through the detects and select only the builders
                        {
                            if (hitColliders[counter].transform.parent.name == "Units" && hitColliders[counter].gameObject.GetComponent<UnitBehaviour>().unitType == "builder")//if a unit is selected
                            {
                                SelectedUnits.Add(hitColliders[counter].gameObject);
                            }
                            counter++;
                        }
                        SwitchLeftHandUIScreen("BuilderUnitScreen");
                    } else if (hasBuildings == true) 
                    {
                        counter = 0;
                        while (counter < hitColliders.Length)
                        {
                            if (hitColliders[counter].transform.parent.name == "Buildings")//if a unit is selected
                            {
                                SelectedUnits.Add(hitColliders[counter].gameObject);
                            }
                            counter++;
                        }
                        SwitchLeftHandUIScreen("BuildingScreen");

                    }
               

                }

                Debug.Log("selectedUnits.Count " + SelectedUnits.Count);

                DrawingSelectionBox = false;
                Destroy(SelectionBox);
            }
        }
    }

    private void SwitchLeftHandUIScreen(string screen) //switches the menu on the left hand depending on what is selected
    {
        buildingScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen");
        unitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/UnitScreen");
        builderUnitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuilderUnitScreen");
        if (screen == "BuilderUnitScreen")
        {
            buildingScreen.SetActive(false);
            unitScreen.SetActive(false);
            builderUnitScreen.SetActive(true);
        }
        if (screen == "UnitScreen")
        {
            buildingScreen.SetActive(false);
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(true);
        }

        if (screen == "BuildingScreen")
        {
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(false);
            buildingScreen.SetActive(true);

        }
        if (screen == "none")
        {
            buildingScreen.SetActive(false);
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(false);
        }
    }


    private void DeselectUnits() //deslects the units and resets the left hand ui
    {
        if (SelectedUnits.Count > 0)
        {
            int counter = 0;
            while (counter < SelectedUnits.Count)
            {
                SelectedUnits[counter].GetComponent<UnitBehaviour>().Deselect();
                counter++;
            }
            SelectedUnits.Clear();
        }
        SwitchLeftHandUIScreen("none");

    }
  
}
