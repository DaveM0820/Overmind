using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    GameObject player;

    public bool selected;
    Transform selectionMarker;
    public int owner;
    public float hpMax;
    public float hp;
    public float hpPercent;
    public string unitType;

    public float unitRange; //range of weapons
    public float cameraSizeWhenUnderDirectControl;
    public float headHeight;// for asssuming direct control
    public string moveType;//either direct or velocity
    public float moveSpeed;
    public float turnSpeed;
    public float acceleration;
    private bool moving;
    float elapsed;
    float updateTimestep;
    public List<Order> OrderQueue;
    int howmanyupdates;
    private void Start()
    {
        elapsed = 0;
        player = GameObject.Find("/XR Rig");
        OrderQueue = new List<Order>();
        selectionMarker = transform.Find("SelectionMarker");
        Deselect();
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
    }
    // bascially orders are sent from UnitCommand and added to the order queue for each unit in their UnitBehaviour, the orders are then sent back to UnitCommand to be executed
    // UnitCommand determines when the order is complete and remove it from the queue


    void Update() // updates every updateTimeStep, based on updateFPS in GlobalGameInformation. 
    {
          elapsed += Time.deltaTime;
        if (elapsed >= updateTimestep)
        {
            elapsed = 0;
            UpdateUnit();
        }
    }
    private void UpdateUnit()
    {
       // howmanyupdates++;

        //Debug.Log("UpdateUnit :" + howmanyupdates);

        if (OrderQueue[0].orderType == "move")
        {
             if (moveType == "direct")
              {
                MoveDirect();
                 }
                if (moveType == "velocity")
                 {
                 MoveVelocity();
               }
        }
        if (OrderQueue[0].orderType == "build")
        {
            gameObject.GetComponent<BuilderUnitBehaviour>().Build(OrderQueue[0].orderTarget);
        }
        if (OrderQueue[0].orderType == "attack")
        {

        }
        if (OrderQueue[0].orderType == "attackMove")
        {

        }
        if (OrderQueue[0].orderType == "defend")
        {

        }
        if (OrderQueue[0].orderType == "patrol")
        {

        }
    }


    public void addOrderToQueue(Order order)
    {
        Debug.Log("Got one " + order.orderType + " order");

        if (order.emptyQueue == true)
        {
            OrderComplete();
            OrderQueue.Clear();
        }
        OrderQueue.Add(order);
      /*  int count = 0;
        while (count < OrderQueue.Count)
        {
            count++;
        }*/
    }

    public void OrderComplete()
    {
        if (OrderQueue.Count > 0) { 
            if (OrderQueue[0].orderType == "build")
            {
                gameObject.GetComponent<BuilderUnitBehaviour>().stopBuilding();
            }
            OrderQueue.RemoveRange(0, 1);
        }
      
    }
    public void Select()
    {

        selected = true;
        selectionMarker = transform.Find("SelectionMarker");

        selectionMarker.gameObject.SetActive(true);

    }
    public void Deselect()
    {

        selected = false;
        selectionMarker = transform.Find("SelectionMarker");

        selectionMarker.gameObject.SetActive(false);

    }
    private void MoveDirect()
    {

        Vector3 moveDir = (OrderQueue[0].orderPosition - transform.position).normalized;
        if (Vector3.Distance(OrderQueue[0].orderPosition, transform.position) < 1f) // if within 1m of destination
        {
            OrderComplete();

        }
        else
        {
            transform.position += moveDir * moveSpeed * updateTimestep;

        }
    }
    private void MoveVelocity()
    {
        Vector3 moveDir = (OrderQueue[0].orderPosition - transform.position).normalized;
        if (Vector3.Distance(OrderQueue[0].orderPosition, transform.position) < 1f) // if within 1m of destination
        {
            OrderComplete();

        }
        else
        {

            transform.position += moveDir * moveSpeed * updateTimestep;

        }
    }
}



public class Order
{
    public string orderType;
    public GameObject orderTarget;
    public Vector3 orderPosition;
    public bool emptyQueue;
    public Order(string type, Vector3 position)
    {
        orderType = type;
        orderPosition = position;
        emptyQueue = true;
    }
    public Order(string type, GameObject target)
    {
        orderType = type;
        orderTarget = target;
        emptyQueue = true;
    }
    public Order(string type, Vector3 position, bool emptyOrderQueue)
    {
        orderType = type;
        orderPosition = position;
        emptyQueue = emptyOrderQueue;
    }
    public Order(string type, GameObject target, bool emptyOrderQueue)
    {
        orderType = type;
        orderTarget = target;
        emptyQueue = emptyOrderQueue; 
    }
    public Order(string type)
    {
        orderType = type;
        emptyQueue = true;

    }

   
}




