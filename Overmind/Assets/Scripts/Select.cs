using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.XR.Interaction.Toolkit;

public class Select : MonoBehaviour
{
    public InputActionReference SelectActionRefrence = null;
    private float ButtonValue;

    private bool DrawingSelectionBox;//is the selection box being drawn
    private Vector3 StartSelectionBox;//location to start drawing selectoin box

    private RaycastHit RightRaycastHit;//hit point of right controller raycast
    private RaycastHit LeftRaycastHit;

    private Transform RightArmPosition;
    private Transform LeftArmPosition;

    private GameObject player;
    private GameObject RightCursor; // the cursors that appear on the ground
    private GameObject LeftCursor;
    private GameObject SelectionBox = null; //the selection box itself

    private bool isStuck;

    public bool allBuilder;
    public bool hasMilitaryunits = false;
    public  bool hasBuildings = false;
    public bool hasBuilderUnits = false;
    public GameObject selectionBox;

    GameObject buildingScreen;
    GameObject unitScreen;
    GameObject builderUnitScreen;

    GameObject directControlButton;
    GameObject defaultScreen;


    public static List<GameObject> SelectedUnits = null;
    private void Awake() {
        buildingScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen");
        unitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/UnitScreen");
        builderUnitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuilderUnitScreen");
        directControlButton = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/DirectControl");
        defaultScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/DefaultScreen");
        RightCursor = GameObject.Find("/UI/RightCursor");
        LeftCursor = GameObject.Find("/UI/LeftCursor");
        RightArmPosition = GameObject.FindWithTag("RightController").transform;
        LeftArmPosition = GameObject.FindWithTag("LeftController").transform;
    }
    void Start()
    {
        DrawingSelectionBox = false;
        ButtonValue = 0;

        SelectedUnits = new List<GameObject>();
        allBuilder = false;
        player = GameObject.Find("/XR Rig");
        isStuck = false;

        //int doubleclickcounter = 0;
        SwitchLeftHandUIScreen("none");
    }

    // Update is called once per frame
    void Update()
    {

        int layerMask = 1 << 6;//layer 6 is ground, bitshift 1 to 6
                               //  GameObject leftHand = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller");
                               //  GameObject rightHand = GameObject.Find("/XR Rig/Camera Offset/RightHand Controller");
                               //  leftHand.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out LeftRaycastHit);
                               //  rightHand.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RightRaycastHit);
                               // Debug.Log("LeftRaycastHit = " + LeftRaycastHit.transform.name);

        Physics.Raycast(RightArmPosition.position, RightArmPosition.TransformDirection(Vector3.forward), out RightRaycastHit, Mathf.Infinity, layerMask);
        Physics.Raycast(LeftArmPosition.position, LeftArmPosition.TransformDirection(Vector3.forward), out LeftRaycastHit, Mathf.Infinity, layerMask);
        if (gameObject.GetComponent<CameraControls>().assumingDirectControl == false || SelectedUnits.Count > 1) { 
        RightCursor.transform.position = new Vector3(RightRaycastHit.point.x, 0, RightRaycastHit.point.z);
        LeftCursor.transform.position = new Vector3(LeftRaycastHit.point.x, 0, LeftRaycastHit.point.z);
        float newScale = (transform.position.y / 100) + 1;
        RightCursor.transform.localScale = new Vector3(newScale, newScale, newScale);
        LeftCursor.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        ButtonValue = SelectActionRefrence.action.ReadValue<float>();

        if (ButtonValue == 1 && player.GetComponent<UnitCommand>().placingBuilding == false) //if the a button is pressed
        {
            if (player.GetComponent<CameraControls>().assumingDirectControl == true)
            {
                SelectUnit(player.GetComponent<CameraControls>().unitBeingControlled);

            }
            if (DrawingSelectionBox == false) //and you are not drawing the selection box
            {
                DeselectUnits();
                if (RightRaycastHit.collider.gameObject.layer == 9) //if you clicked a single unit or building select it and start the double click countdown
                {
                   
                    SelectUnit(RightRaycastHit.collider.gameObject);
                    if (RightRaycastHit.collider.gameObject.GetComponent<UnitBehaviour>().unitType == "Builder")
                    {        
                        SwitchLeftHandUIScreen("BuilderUnitScreen");
                    }
                    else
                    {
                        SwitchLeftHandUIScreen("UnitScreen");
                    }
                }    
                else if (RightRaycastHit.collider.gameObject.layer == 7)
                {
                    
                    SelectUnit(RightRaycastHit.collider.gameObject);
                    SwitchLeftHandUIScreen("BuildingScreen");

                }
                else
                {
                    //start drawing selection box
                    SelectionBox = Instantiate(selectionBox);
                     SelectionBox.name = "SelectionBox";
                    StartSelectionBox.x = RightRaycastHit.point.x;
                    StartSelectionBox.z = RightRaycastHit.point.z;
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
                SelectionBox.transform.localScale = new Vector3(DistanceDragged.x, SelectionBox.transform.localScale.y, DistanceDragged.y);
            }

        } else {//if button is not being pushed
            if (isStuck == true)
            {
                DrawingSelectionBox = false;
                isStuck = false;
                Destroy(SelectionBox);
            }
            isStuck = true;
            if (DrawingSelectionBox == true)// and the selection box was being drawn
            {
                /*basically, go through all the hit detections in the box and find out what kind of objects are there, if it's a mix only select the military units,
                 if it's builders and buildings only select the builders, if only buildings then select them*/

                //select units within
                DeselectUnits();
                hasMilitaryunits = false;
                hasBuildings = false;
                hasBuilderUnits = false;
                Collider[] hitColliders = Physics.OverlapBox(SelectionBox.transform.position, SelectionBox.transform.localScale / 2, Quaternion.identity);//get a list of objects within the selection box
                int counter = 0;
                while (counter < hitColliders.Length) //go through hit detections and see what kind of units are selected
                {

                    if (hitColliders[counter].name != "SelectionBox") { // if not looking at the selection box itself

                        if (hitColliders[counter].transform.parent.name == "Units")//and if a unit is selected

                        {

                            if (hitColliders[counter].gameObject.GetComponent<UnitBehaviour>().unitType == "Builder")//and its a builder 

                            {

                                hasBuilderUnits = true;//a builder is selected

                            }
                            else
                            {
                                hasMilitaryunits = true;//if its not a builder a military unit selected

                            }
                        } else if (hitColliders[counter].transform.parent.name == "Buildings") // if a building is selected
                            {
                                hasBuildings = true; //there is a building selected :O
                        }

                    }
                    counter++;
                }


                if (hasMilitaryunits == true) //if military units are detected
                {
                    counter = 0;
                    while (counter < hitColliders.Length)
                    {
                        if (hitColliders[counter].name != "SelectionBox") { 
                            if (hitColliders[counter].transform.parent.name == "Units"){
                                if (hitColliders[counter].gameObject.GetComponent<UnitBehaviour>().unitType != "Builder")
                                {
                                    SelectUnit(hitColliders[counter].gameObject); //select the units which are not builders
                                }
                            }
                        }
                        counter++;
                    }
                    SwitchLeftHandUIScreen("UnitScreen");
                }
                else // if no military units are detected
                {



                    if (hasBuilderUnits == true) //and builders are detected
                    {
                        counter = 0; 

                        while (counter < hitColliders.Length)//go through the detections and select only the builders
                        {

                            if (hitColliders[counter].name != "SelectionBox")
                            {

                                if (hitColliders[counter].transform.parent.name == "Units")
                                {

                                    if (hitColliders[counter].gameObject.GetComponent<UnitBehaviour>().unitType == "Builder")
                                    {

                                        SelectUnit(hitColliders[counter].gameObject);


                                    }
                                }
                            }

                            counter++;

                        }
                        SwitchLeftHandUIScreen("BuilderUnitScreen");
                    } else if (hasBuildings == true) //if only buildings are detected select them

                    {
                        allBuilder = false;
                        counter = 0;
                        while (counter < hitColliders.Length)
                        {

                                if (hitColliders[counter].name != "SelectionBox")
                                {
                                if (hitColliders[counter].gameObject.transform.parent.name == "Buildings")//if a building  is selected
                                {

                                    SelectUnit(hitColliders[counter].gameObject);
                                }
                                }

                            counter++;

                        }
                        SwitchLeftHandUIScreen("BuildingScreen");
                    }
                }
                DrawingSelectionBox = false;
                isStuck = false;
                Destroy(SelectionBox);
            }
            if (isStuck == true)
            {
                DrawingSelectionBox = false;
                isStuck = false;
                Destroy(SelectionBox);
            }
        }
    }
    private void SelectUnit(GameObject unit) //function to select a single unit
    {
        SelectedUnits.Add(unit);
        unit.GetComponent<UnitBehaviour>().Select();

    }
    public void SwitchLeftHandUIScreen(string screen) //switches the menu on the left hand depending on what is selected
    {

        if (screen == "DirectControlScreen")
        {
            buildingScreen.SetActive(false);
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(false);
            directControlButton.SetActive(true);
            defaultScreen.SetActive(false);

            allBuilder = false;

        }
        if (screen == "BuilderUnitScreen")
        {
            buildingScreen.SetActive(false);
            unitScreen.SetActive(false);
            builderUnitScreen.SetActive(true);
            defaultScreen.SetActive(false);

            allBuilder = true;
            if (SelectedUnits.Count == 1) {
                directControlButton.SetActive(true);
            }
            else
            {
                directControlButton.SetActive(false);
            }
        }
        if (screen == "UnitScreen")
        {
            buildingScreen.SetActive(false);
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(true);
            defaultScreen.SetActive(false);
            if (SelectedUnits.Count == 1)
            {
                directControlButton.SetActive(true);
            }
            else
            {
                directControlButton.SetActive(false);
            }
            allBuilder = false;

        }

        if (screen == "BuildingScreen")
        {
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(false);
            buildingScreen.SetActive(true);
            defaultScreen.SetActive(false);

            allBuilder = false;
            buildingScreen.GetComponent<LefthandUIBuilding>().UpdateUnitQueueDisplay();


        }
        if (screen == "none")
        {
            buildingScreen.SetActive(false);
            builderUnitScreen.SetActive(false);
            unitScreen.SetActive(false);
            directControlButton.SetActive(false);
            defaultScreen.SetActive(true);
            allBuilder = false;


        }

    }


    public void DeselectUnits() //deslects the units and resets the left hand UI
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
