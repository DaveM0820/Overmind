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
        Debug.Log("UnitMovementCommand script start()");

        orderIssued = false;
        selectedUnits = Select.SelectedUnits;
    }


    // Update is called once per frame

    void Update()
    {
        Debug.Log("UnitMovementCommand script Update()");

        buttonValue = triggerActionRefrence.action.ReadValue<float>();
        Debug.Log("TriggerButtonvlaue:" + buttonValue );
       // Debug.Log("selectedUnits.Count " + selectedUnits.Count);

        if (buttonValue == 1)
        {
            selectedUnits = Select.SelectedUnits;

            if (selectedUnits != null) { 
                Debug.Log("TriggerButtonvlaue if loop" + buttonValue);
                Debug.Log("selectedUnits.Count" + selectedUnits.Count);

                 if (selectedUnits.Count > 0)
                 {
                         orderIssued = true;
                         Debug.Log("OrderIssued = " + orderIssued);
                 }
            }else
            {

                Debug.Log("selectedUnits.Count is null");

            }
        } else
        {
            if (orderIssued == true)
            {
                Debug.Log("DetermineOrderType() started ");

                DetermineOrderType();

                orderIssued = false;
            }

        }
    }
    private void DetermineOrderType()
    {
        //if cursor is on gound give move order 
        Debug.Log("DetermineOrderType() running ");
        selectedUnits = Select.SelectedUnits;

        rightArmPosition = GameObject.FindWithTag("RightController");
        Physics.Raycast(rightArmPosition.transform.position, rightArmPosition.transform.TransformDirection(Vector3.forward), out rightRaycastHit, Mathf.Infinity);
        Debug.Log("MoveOrderTarget hit detection: " + rightRaycastHit.collider.gameObject.name);
        if (rightRaycastHit.collider.gameObject.name == "Ground") // if player issued order on on ground, move units to point
        {
            Debug.Log("GiveMoveOrder() to ground at point: " + rightRaycastHit.point);
            GiveMoveOrder(rightRaycastHit.point);
        } 
        //if unit(s) are all builders check what target is hitting, if it is a resource consume, if building assist
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
    private void GiveMoveOrder(Vector3 location)
    {
        GameObject moveOrderIndicator = Instantiate(GameObject.Find("/MoveOrderIndicator"));
        //moveOrderIndicator.SetActive(true);
        moveOrderIndicator.transform.position = location;
        Destroy(moveOrderIndicator, 4);
        Debug.Log("withing GiveMoveOrder(), location: " + location);
        List<Vector3> targetPositionList = GetPositionListAroundPoint(location, new float[] { 2f, 4f, 8f }, new int[] { 4, 8, 12 }); //give units individual points around move order location
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


