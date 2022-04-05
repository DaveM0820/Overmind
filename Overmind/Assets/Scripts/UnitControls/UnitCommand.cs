using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//unit command takes commands from the controls and other scripts (like the buildnewbuilding script), determines exactly what to do with selected units
// it then sends the approiate orders to those unit's order queues in their UnitBehaviour scripts, then the orders are passed back to methods in UnitCommand to be executed.

public class UnitCommand : MonoBehaviour
{
    public InputActionReference triggerActionRefrence = null;
    private float buttonValue;
    private List<GameObject> selectedUnits;
    private Vector3 goTo;
    private bool orderIssuedFromButtonPress;
    private GameObject rightArmPosition;
    private RaycastHit rightRaycastHit;
    public bool placingBuilding;
    bool issuingOrders;
    string currentOrderType;
    int unitsOrdered;
    GameObject orderTarget;
    Vector3 orderLocation;
    GameObject player;
    List<Vector3> targetPositionList;
    GameObject builderUnitScreen;
    int count;

    void start()
    {

        builderUnitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuilderUnitScreen");
        orderIssuedFromButtonPress = false;
        selectedUnits = Select.SelectedUnits;
        player = GameObject.Find("/XR Rig");
        issuingOrders = false;
        placingBuilding = false;
        unitsOrdered = 0;
        count = 0;
    }

    private void GiveCollectOrder(GameObject collectTarget) //gives collect order to all selected units, unit orders will be given in update once per frame
    {
        currentOrderType = "collect";
        issuingOrders = true;
        unitsOrdered = 0;
        orderTarget = collectTarget;
    }
    public void GiveBuildOrder(GameObject buildTarget) //gives build order to all selected units
    {
        if (buildTarget.GetComponent<BuildingBehaviour>().built == false) {
            Debug.Log("GiveBuildOrder just ran");

            currentOrderType = "build";
            issuingOrders = true;
            unitsOrdered = 0;
            orderTarget = buildTarget;
        }
        else
        {
            Debug.Log("building already built");


        }
    }
    public void GiveMoveOrder(Vector3 location) //gives move order to all selected units
    {

        currentOrderType = "move";
        issuingOrders = true;
        unitsOrdered = 0;
        orderLocation = location;
    }
    // Update is called once per frame

    public void GiveAssistOrder(GameObject assistTarget) //gives move order to all selected units
    {
        currentOrderType = "assist";
        issuingOrders = true;
        unitsOrdered = 0;
        orderTarget = assistTarget;
    }
    void Update()
    {

        //check if order button is pressed, if so determine what order type to give
        buttonValue = triggerActionRefrence.action.ReadValue<float>();

        if (buttonValue == 1)//if trigger is pressed
        {
            selectedUnits = Select.SelectedUnits;
            if (selectedUnits.Count > 0)
            {
                orderIssuedFromButtonPress = true; //if there are units selected, set orderIssued to true
            }
        } else
        {
            if (orderIssuedFromButtonPress == true) //if the button isn't being pressed check if orderIssued is true and if so run DetermineOrderType() and set order Issued to false
            {
                count++;
                orderIssuedFromButtonPress = false;
                DetermineOrderType();

            }

        }

        //give 1 order per frame to spread out cpu load and prevent potential fps drop when giving orders
        if (issuingOrders == true) {
            selectedUnits = Select.SelectedUnits;

            if (currentOrderType == "build")
            {

                GiveOneBuildOrder(selectedUnits[unitsOrdered], orderTarget);
                unitsOrdered++;
            }
            if (currentOrderType == "move")
            {

                // if virtual squad leader has direct line of sight to target simply order all units to move to points around target
                // otherwise do path finding for the virtual squad leader, make it follow the path at the speed of the slowest units, and give units a follow order on it

                if (unitsOrdered == 0) { // f no one has been ordered yet generate the list of positions to send them
                    getMovePositions(orderLocation);
                }
                if (selectedUnits[unitsOrdered].GetComponent<UnitBehaviour>().isBuilding == false) { 
                GiveOneDirectMoveOrder(selectedUnits[unitsOrdered], targetPositionList[unitsOrdered]);
                }
                unitsOrdered++;
            }
            if (currentOrderType == "collect")
            {
                // GiveOneBuildOrder(selectedUnits[unitsOrdered], orderTarget);
                unitsOrdered++;
            }
            if (currentOrderType == "assist")
            {
                // GiveOneBuildOrder(selectedUnits[unitsOrdered], orderTarget);
                unitsOrdered++;
            }

            if (unitsOrdered == selectedUnits.Count) // if orders have been issued to all selected units then done ordering units 
            {
                Debug.Log("Done ordering units");
                unitsOrdered = 0;
                issuingOrders = false;
            }
        }

    }
    private void DetermineOrderType()//when player presses command button figure out what the units should do based on what its are selected and what the target is
    {
        player = GameObject.Find("/XR Rig");

        //if cursor is on gound then give move order, regardless of unity type
        selectedUnits = Select.SelectedUnits;
        rightArmPosition = GameObject.FindWithTag("RightController");
        Physics.Raycast(rightArmPosition.transform.position, rightArmPosition.transform.TransformDirection(Vector3.forward), out rightRaycastHit, 500);
        // Debug.Log("hit :" + rightRaycastHit.collider.gameObject.name);

        if (rightRaycastHit.collider.gameObject.layer == 6) // if player issued order on on ground, move units to that point
        {
            if (placingBuilding == false)
            {
                //  Debug.Log("placingBuilding == false");
                if (player.GetComponent<Select>().hasMilitaryunits == true || player.GetComponent<Select>().hasBuilderUnits == true) { 
                GiveMoveOrder(rightRaycastHit.point);
                }
            }
            else
            {
                //   Debug.Log("placingBuilding == true");

            }
        }
        //check if all unit(s) are builders, if so check what target is hitting and if it is a resource consume, if it is a building assist
        //  Debug.Log("Determine order type: not a move order");


        if (player.GetComponent<Select>().allBuilder == true) {
            {
                //check if target is resource or building

                if (rightRaycastHit.collider.gameObject.layer == 8)
                {
                    GiveCollectOrder(rightRaycastHit.collider.gameObject);
                    //  Debug.Log("GiveCollectOrder");


                }
                else if (rightRaycastHit.collider.gameObject.layer == 7)
                {
                    //  Debug.Log("hit is on building");

                    if (rightRaycastHit.collider.gameObject.GetComponent<UnitBehaviour>().faction == player.GetComponent<GlobalGameInformation>().player)
                    {
                        // Debug.Log("owner is player");

                        GiveBuildOrder(rightRaycastHit.collider.gameObject);
                        //   Debug.Log("GiveBuildOrder ran");


                    }
                    else
                    {
                        //   Debug.Log("owner is not player");


                    }


                }
                else if (transform.parent.name == "Units")
                {
                    if (rightRaycastHit.collider.gameObject.GetComponent<UnitBehaviour>().unitType == "Builder")
                    {
                        //  Debug.Log("transform.parent.name == Units");

                        GiveAssistOrder(rightRaycastHit.collider.gameObject);
                    }

                    //check if enemy or friendly

                }
            }
        } else
        {

            //  Debug.Log("all builder = false");

        }
        //Debug.Log("done determining order type");




    }


    private void GiveOneBuildOrder(GameObject currentUnit, GameObject buildTarget) // gives build order to one unit, runs once a frame until all units have their orders
    {
        int layerMask = 1 << 7;//layer 7 is buildings, bitshift 1 to 7 to return bit value of 7

        // first check if the units has a direct path to the building using a raycast which ignores units
        Vector3 directionToBuilding = (buildTarget.transform.position - currentUnit.transform.position).normalized;
        Vector3 raycastStartPositionHeightAdjustment = new Vector3(currentUnit.transform.position.x, 1, currentUnit.transform.position.z);
        Physics.Raycast(raycastStartPositionHeightAdjustment, directionToBuilding, out RaycastHit buildingHit, Mathf.Infinity, layerMask);
        Debug.Log("RaycastHit" + buildingHit.transform.gameObject.name);

        if (buildingHit.transform.gameObject == buildTarget || buildingHit.transform.parent.gameObject == buildTarget) //if the raycast hits the target building
        {
            //direct path to building found, give direct move order in direction of buidling with distance - unit range distance -10%
            float distanceToBuilding = Vector3.Distance(currentUnit.transform.position, buildingHit.point);
            float distanceToTravel = distanceToBuilding - (currentUnit.GetComponent<UnitBehaviour>().unitRange * 0.8f);
            Vector3 newPosition = currentUnit.transform.position + (directionToBuilding * distanceToTravel);
            
            if (Vector3.Distance(currentUnit.transform.position, buildTarget.transform.position) > currentUnit.GetComponent<UnitBehaviour>().unitRange * 0.8f) { // if the unit is far from the building move there first
                Order moveorder = new Order("move", newPosition + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)), true);
            currentUnit.GetComponent<UnitBehaviour>().addOrderToQueue(moveorder);

                Order buildorder = new Order("build", buildTarget, false);
            currentUnit.GetComponent<UnitBehaviour>().addOrderToQueue(buildorder);
            }
            else
            {
                Order buildorder = new Order("build", buildTarget, true);
                currentUnit.GetComponent<UnitBehaviour>().addOrderToQueue(buildorder);
            }
            

        }
        else
        {
            Debug.Log("No direct path from " + currentUnit.name + " to " + buildTarget.name);

            //there is no direct path
        }
    }
    public void GiveOneMoveOrder(GameObject unit, Vector3 location)
    {
        Order order = new Order("move", location, true);
        unit.GetComponent<UnitBehaviour>().addOrderToQueue(order);
    }
    public void GiveOneDirectMoveOrder(GameObject unit, Vector3 location)//give one unit a move order directly to a point, no pathing
    {
        Order order = new Order("move", location, true);
        unit.GetComponent<UnitBehaviour>().addOrderToQueue(order);
    }

    private void getMovePositions (Vector3 location) 
    {
        GameObject moveOrderIndicator = Instantiate(GameObject.Find("/UI/MoveOrderIndicator"));
        //moveOrderIndicator.SetActive(true);
        moveOrderIndicator.transform.position = location;
        Destroy(moveOrderIndicator, 2);

        targetPositionList = GetPositionListAroundPoint(location, new float[] { 2f, 3f, 4.5f, 6f, 8f, 10f,12f,14f,16f}, new int[] { 4, 8, 10, 15, 20, 25,28,32,36}); //give units individual points around move order location
        int counter = 0;
        while (counter < selectedUnits.Count)
        {
            counter++;
        }
    }
    private List<Vector3> GetPositionListAroundPoint(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)//generate rings around point
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }

        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)//generate points in ring
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * new Vector3(1, 0, 0);
            Vector3 position = startPosition + dir * distance;
            // check if point collides with anything, if so don't add that position
            positionList.Add(position);
        }
        return positionList;
    }


}


