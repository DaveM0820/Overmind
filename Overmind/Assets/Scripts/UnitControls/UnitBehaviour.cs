using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour //UnitBehavior is the core class that defines how basically everything in the game operates. Every unit and building has it. 
{
    GameObject player;
    public bool selected;
    Transform selectionMarker;
    public int owner;
    public int cost;
    public int hpMax;
    public int hp;
    public float hpPercent;
    public string unitType;
    public float unitBuildTime;
    public bool isBuilding;
    public float unitRange; //range of weapons
    public float cameraSizeWhenUnderDirectControl;
    public bool assumingDirectControl = false;
    public float headHeight;// for asssuming direct control
    public string moveType;//either direct or velocity
    public float moveSpeed;
    private float elapsed = 0;
    public float updateTimestep;
    public List<Order> OrderQueue;
    public Sprite unitIcon;
    private bool queueIsEmpty = true;
    IUnitActionInterface action;
    public int hpChange;
    private void Start()
    {
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
            updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep/10; //ex) if units update at 40fps, buildings update at 4fps
        }
        action = gameObject.GetComponent<IUnitActionInterface>();

    }
    // bascially orders are sent from UnitCommand and other scripts, and added to the order queue for each unit, which are then passed to the unit specific scrip through the IunitActionInterface
    void Update() // updates every updateTimeStep, initally set by updateFPS in GlobalGameInformation. This way the framerate of units can be increased or decreased depending on current performance.
    {
        if (assumingDirectControl == false)
        {
            if (queueIsEmpty == false)
            elapsed += Time.deltaTime;
            if (elapsed >= updateTimestep)
            {
                elapsed = 0;
                UpdateUnit();
            }
        }
        else 
        {
            action.UnderDirectControl();
        }
  
    }
    public void changeHP(int amount) {

        hp += amount;
        if (hp < 0)
        {
            die();
        }
        hpChange = amount;
        if (hp >= hpMax)
        {
            hp = hpMax;
            hpChange = 0;
        }

    }
    public void die()
    {
    
    
    }
    private void UpdateUnit() // this is the core function that impliments unit actions through the IUnitActionInterface, happens every updateTimeStep, typically 40fps
    {
        if (OrderQueue[0].orderTypeNumber == 1)
        {
            action.Move(OrderQueue[0].orderPosition);
        }
        else if (OrderQueue[0].orderTypeNumber == 2)
        {
            action.Build(OrderQueue[0].orderTarget);
        }
        else if (OrderQueue[0].orderTypeNumber == 3)
        {
            action.Attack(OrderQueue[0].orderTarget);
        }
        else if (OrderQueue[0].orderTypeNumber == 4)
        {
            action.Stop();
        }
        else if (OrderQueue[0].orderTypeNumber == 5)
        {
            action.ExtractOre();
        }
        else if (OrderQueue[0].orderTypeNumber == 6)
        {
            action.UpdateScaffold();
        }
        else if (OrderQueue[0].orderTypeNumber == 7)
        {
            action.EnterDirectControl();

        }
        else if (OrderQueue[0].orderTypeNumber == 8)
        {
            action.ExitDirectControl();
        }
        else
        {
            queueIsEmpty = true;
        }
    }
    public void addOrderToQueue(Order order)//adds an order to the unit's order queue
    {
        if (order.emptyQueue == true)//should the order delete all preceding orders(IE be executed immediately)
        {
            OrderComplete();
            OrderQueue.Clear();
        }
        OrderQueue.Add(order);
        queueIsEmpty = false;
        //convert the string to the coorisponding order number, that way the game isn't comparing 40 strings per second per unit, while allowing the code to remain understandable when issuing orders to units
        if (order.orderType == "move")
        {
            order.orderTypeNumber = 1;
        }
        if (order.orderType == "build")
        {
            order.orderTypeNumber = 2;
        }
        if (order.orderType == "attack")
        {
            order.orderTypeNumber = 3;
        }
        if (order.orderType == "stop")
        {
            order.orderTypeNumber = 4;
        }
        if (order.orderType == "extractOre")
        {
            order.orderTypeNumber = 5;
        }
        if (order.orderType == "updateScaffold")
        {
            order.orderTypeNumber = 6;
        }
        if (order.orderType == "enterDirectControl")
        {
            order.orderTypeNumber = 7;
            UpdateUnit();
        }
        if (order.orderType == "exitDirectControl")
        {
            order.orderTypeNumber = 8;
        }

    }
    public void OrderComplete()
    {
        if (OrderQueue.Count > 0) { 
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
        selectionMarker.gameObject.SetActive(true);

    }
    public void Deselect()
    {
        selected = false;
        selectionMarker.gameObject.SetActive(false);
    }
}
public class Order
{
    public string orderType;
    public GameObject orderTarget;
    public Vector3 orderPosition;
    public bool emptyQueue;
    public int orderTypeNumber;
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




