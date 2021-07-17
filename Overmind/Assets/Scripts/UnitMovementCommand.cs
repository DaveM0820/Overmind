using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovementCommand : MonoBehaviour
{
    public InputActionReference triggerActionRefrence = null;
    private float buttonValue;
    private List<GameObject> selectedUnits;
    private Vector3 goTo;
    private bool orderIssued;
    private GameObject rightArmPosition;
    private RaycastHit rightRaycastHit;


    void start()
    {

        orderIssued = false;
        selectedUnits = Select.SelectedUnits;
    }


    // Update is called once per frame

    void Update()
    {

        buttonValue = triggerActionRefrence.action.ReadValue<float>();
       // Debug.Log("selectedUnits.Count " + selectedUnits.Count);

        if (buttonValue == 1)//if trigger is pressed
        {
            selectedUnits = Select.SelectedUnits;//get list of selected units

            if (selectedUnits != null) { 
                 if (selectedUnits.Count > 0)
                 {
                         orderIssued = true; //if there are units selected set orderIssued to true
                 }
            }else
            {


            }
        } else
        {
            if (orderIssued == true) //if the button isn't being pressed check if orderIssued is true and if so run DetermineOrderType() and set order Issued to false
            {

                DetermineOrderType();

                orderIssued = false;
            }

        }
    }
    private void DetermineOrderType()
    {
        //if cursor is on gound then give move order 
        selectedUnits = Select.SelectedUnits;
        rightArmPosition = GameObject.FindWithTag("RightController");
        Physics.Raycast(rightArmPosition.transform.position, rightArmPosition.transform.TransformDirection(Vector3.forward), out rightRaycastHit, Mathf.Infinity);
        Debug.Log("MoveOrderTarget hit detection: " + rightRaycastHit.collider.gameObject.name);
        if (rightRaycastHit.collider.gameObject.name == "Ground") // if player issued order on on ground, move units to that point
        {
            Debug.Log("GiveMoveOrder() to ground at point: " + rightRaycastHit.point);
            GiveMoveOrder(rightRaycastHit.point);
        } 
        //check if all unit(s) are all builders, if so check what target is hitting and if it is a resource consume, if it is a building assist
        bool allBuilder = true;
        int counter = 0;
        while (counter < selectedUnits.Count)
        {
            if (selectedUnits[counter].GetComponent<UnitBehaviour>().unitType != "builder")
            {
                allBuilder = false; 
            }
            counter++;
        }
        Debug.Log("allBuilder =" + allBuilder);
        if (allBuilder == true)
        {
            //check if target is resource or building
            if (rightRaycastHit.collider.gameObject.transform.parent.name == "Enviornment")
            {
                GiveCollectOrder(rightRaycastHit.collider.gameObject);
                Debug.Log("collection order given");



            }
            else if (rightRaycastHit.collider.gameObject.transform.parent.name == "Buildings")
            {
                Debug.Log("building assist order given");

                //check if enemy or friendly

            }
        } else
        {

        }

        //if unit types are mixed 



    }
    private void GiveCollectOrder(GameObject collectTarget)
    {
        Debug.Log("Collect " + collectTarget.name);


    }
    private void GiveMoveOrder(Vector3 location) //what to do when a move order is given
    {
        GameObject moveOrderIndicator = Instantiate(GameObject.Find("/MoveOrderIndicator"));
        //moveOrderIndicator.SetActive(true);
        moveOrderIndicator.transform.position = location;
        Destroy(moveOrderIndicator, 4);
        List<Vector3> targetPositionList = GetPositionListAroundPoint(location, new float[] { 2f, 3f, 4.5f, 6f, 8f, 10f}, new int[] { 4, 8, 10, 15, 20, 25}); //give units individual points around move order location
        int counter = 0;
        while (counter < selectedUnits.Count)
        {
            selectedUnits[counter].GetComponent<MoveToPosition>().SetMovePosition(targetPositionList[counter]);
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
            positionList.Add(position);
        }
        return positionList;
    }
}


