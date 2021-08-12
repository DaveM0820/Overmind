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
    public float unitBuildTime;
    public bool isBuilding;

    public float unitRange; //range of weapons
    public float cameraSizeWhenUnderDirectControl;
    public bool assumingDirectControl;
    public float headHeight;// for asssuming direct control
    public string moveType;//either direct or velocity
    public float moveSpeed;
    public float turnSpeed;
    public float acceleration;
    private float elapsed;
    public float updateTimestep;
    public List<Order> OrderQueue;
    public Sprite unitIcon;
    private bool queueIsEmpty = true;
    private void Start()
    {
        assumingDirectControl = false;
        elapsed = 0;
        player = GameObject.Find("/XR Rig");
        OrderQueue = new List<Order>();
        selectionMarker = transform.Find("SelectionMarker");
        Deselect();
        if (isBuilding == false)
        {
            updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        }
        else
        {
            updateTimestep = player.GetComponent<GlobalGameInformation>().buildingUpdateTimestep;
        }
    }
    // bascially orders are sent from UnitCommand and added to the order queue for each unit in their UnitBehaviour, the orders are then sent back to UnitCommand to be executed
    // UnitCommand determines when the order is complete and remove it from the queue


    void Update() // updates every updateTimeStep, based on updateFPS in GlobalGameInformation. 
    {
        if (queueIsEmpty == false)
        {
            if (assumingDirectControl == false)
            { 
                elapsed += Time.deltaTime;
                if (elapsed >= updateTimestep)
                {
                    elapsed = 0;
                    UpdateUnit();
                }
            }
        }
    }
    private void UpdateUnit()
    {
        if (OrderQueue[0].orderType == "move")
        {
             if (unitType == "Builder")
             {
                gameObject.GetComponent<BuilderUnitBehaviour>().Move(OrderQueue[0].orderPosition);
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
        if (OrderQueue[0].orderType == "buildunit")
        {
           gameObject.GetComponent<BuildingBehaviour>().BuildUnit(OrderQueue[0].orderTarget);
        }
        
    }
    public void addOrderToQueue(Order order)
    {
        if (order.emptyQueue == true)
        {
            OrderComplete();
            OrderQueue.Clear();
        }
        OrderQueue.Add(order);
        queueIsEmpty = false;
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
        if (OrderQueue.Count == 0)
        {
            queueIsEmpty = true;
        }
    }
    public void RemoveOrder(int orderNumber)
    {
        if (OrderQueue.Count > 0)
        {
            if (OrderQueue[orderNumber].orderType == "build")
            {
                gameObject.GetComponent<BuilderUnitBehaviour>().stopBuilding();
            }
            OrderQueue.RemoveRange(orderNumber, 1);
        }
        if (OrderQueue.Count == 0)
        {
            queueIsEmpty = true;
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




