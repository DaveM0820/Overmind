using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour //UnitBehavior is the core class that defines how basically everything in the game operates. Every unit and building has it. 
{
    GlobalGameInformation globalGameInformation;
    GameObject player;
    public bool selected;
    Transform selectionMarker;
    JobManager jobManager;
    public int faction;
    public int cost;
    public int hpMax;
    public int hp;
    public bool military;
    public float hpPercent;
    public string unitType;
    public float unitBuildTime;
    public bool isBuilding;
    public int unitRange; //range of weapons
    public float cameraSizeWhenUnderDirectControl;
    public bool assumingDirectControl = false;
    public float headHeight;// for asssuming direct control
    public string moveType;//either direct or velocity
    public float moveSpeed;
    private float elapsed = 0;
    public float updateTimestep;
    public List<Order> OrderQueue;
    public Sprite unitIcon;
    public bool queueIsEmpty = true;
    IUnitActionInterface action;
    public int hpChange;
    Unit thisUnit;
    int LookForTargetsCounter;
    public int LookForTargetsCounterMax;
    public GameObject currentTarget = null;
    bool unitsInRange = false;
    int rangechecks;
    float distance;
    public bool evading = false;
    GameObject closestEnemy;
    float closestEnemyDistance;
    bool dead = false;
    int currentOrderTypeNumber;
    float fullFPSTimeStep;
    bool hasTarget;
    void Start() {
        fullFPSTimeStep = 1f / 72f;
        player = GameObject.Find("/XR Rig");
        OrderQueue = new List<Order>();
        selectionMarker = transform.Find("SelectionMarker");
        // LookForTargetsCounterMax = player.GetComponent<GlobalGameInformation>().LookForTargetsCounterMax;
        Deselect();
        if (!military)
        {
            currentTarget = gameObject;
        }
        if (isBuilding == false)
        {
            updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        }
        else
        {
            updateTimestep = player.GetComponent<GlobalGameInformation>().buildingUpdateTimestep; //ex) if units update at 40fps, buildings update at 4fps
        }
        action = gameObject.GetComponent<IUnitActionInterface>();
        globalGameInformation = player.GetComponent<GlobalGameInformation>();
        thisUnit = new Unit(faction, gameObject, military, transform.position);
        globalGameInformation.unitList[faction].Add(thisUnit);
        jobManager = player.GetComponent<JobManager>();

    }

    // bascially orders are sent from UnitCommand and other scripts, and added to the order queue for each unit, which are then passed to the unit specific scrip through the IunitActionInterface
    void Update() // updates every updateTimeStep, initally set by updateFPS in GlobalGameInformation. This way the framerate of units can be increased or decreased depending on current performance.
    {
        if (!assumingDirectControl) // if you are not directly controlling the unit
        {
            if (!queueIsEmpty)
            {
                // and there are orders in the orderqueue
                elapsed += fullFPSTimeStep;
                if (elapsed >= updateTimestep) //run the order every updateTimestep using the UpdateUnit method
                {
                    elapsed = 0;
                    UpdateUnit();
                }
            }
            else
            {


                if (!hasTarget)  // if there are not orders in the queue 
                {

                    LookForTargetsCounter++;
                    if (LookForTargetsCounter > LookForTargetsCounterMax)
                    {

                        LookForTargets();
                    }
                }
            }

        }
        else
        {
            action.UnderDirectControl(); //if the unit is under direct control run that unit's UnderDirectControl script
        }

    }
    public void UpdateUnit() // this is the core function that impliments unit actions through the IUnitActionInterface, happens every updateTimeStep, by default 40fps
 {

        switch (currentOrderTypeNumber)
        {
            case 1:
            action.Move(OrderQueue[0].orderPosition);
            break;
            case 2:
            action.Build(OrderQueue[0].orderTarget);
            break;
            case 3:
            action.Attack(OrderQueue[0].orderTarget);
            break;
            case 4:
            action.Stop();
            break;
            case 5:
            action.ExtractOre();
            break;
            case 6:
            action.UpdateScaffold();
            break;
            case 7:
            action.EnterDirectControl();
            break;
            case 8:
            action.ExitDirectControl();
            break;
            default:
            OrderComplete();
            break;
        }

    }
    public void changeHP(int amount) {
        if (!dead)
        {
            if (amount < 0)
            {
                action.Damage();
            }
            hp += amount;
            if (hp <= 0)
            {
                hp = 0;
                globalGameInformation.unitList[faction].Remove(thisUnit);
                dead = true;
                action.Die();
            }
            hpChange = amount;
            if (hp >= hpMax)
            {
                hp = hpMax;
                hpChange = 0;
            }

        }

    }
    public void LookForTargets() {
        jobManager.LookForTarget(thisUnit, unitRange);

    }
    public void Attack(GameObject target) {
        currentTarget = target;
        hasTarget = true;
        addOrderToQueue(new Order("attack", target, true));
    }
    public void targetGone() {
        OrderComplete();
        currentTarget = null;
        hasTarget = false;
    }
    public void addOrderToQueue(Order order)//adds an order to the unit's order queue
    {

        if (order.emptyQueue)//should the order delete all preceding orders(IE be executed immediately)
        {
            OrderComplete();
            OrderQueue.Clear();
        }

        //convert the string to the coorisponding order number, that way the game isn't comparing 40 strings per second per unit, while allowing the code to remain understandable when issuing orders to units

        switch (order.orderType)
        {
            case "move":
            order.orderTypeNumber = 1;
            break;
            case "build":
            order.orderTypeNumber = 2;
            break;
            case "attack":
            order.orderTypeNumber = 3;
            break;
            case "stop":
            order.orderTypeNumber = 4;
            break;
            case "extractOre":
            order.orderTypeNumber = 5;
            break;
            case "updateScaffold":
            order.orderTypeNumber = 6;
            break;
            case "enterDirectControl":
            order.orderTypeNumber = 7;
            break;
            case "exitDirectControl":
            order.orderTypeNumber = 8;
            break;
            default:
            order.orderTypeNumber = 0;
            OrderComplete();
            break;

        }
        if (OrderQueue.Count == 0)
        {
            currentOrderTypeNumber = order.orderTypeNumber;
        }
        OrderQueue.Add(order);
        queueIsEmpty = false;

    }
    public void OrderComplete() {
        if (OrderQueue.Count == 0)
        {
            hasTarget = false;
            currentTarget = null;
            queueIsEmpty = true;
            currentOrderTypeNumber = 0;

        }
        else
        {

            if (OrderQueue[0].orderTypeNumber == 3)// if it was an attack order
            {
                hasTarget = true;
                currentTarget = null;
            }
            OrderQueue.RemoveRange(0, 1);

   
            if (OrderQueue.Count == 0)
            {
                hasTarget = false;
                currentTarget = null;
                queueIsEmpty = true;
                currentOrderTypeNumber = 0;
              //  LookForTargets();

            }
            else
            {
                currentOrderTypeNumber = OrderQueue[0].orderTypeNumber;
            }
        }

    }
    public void RemoveOrder(int orderNumber) {
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
            currentOrderTypeNumber = 0;

        }
        currentOrderTypeNumber = OrderQueue[0].orderTypeNumber;

    }
    public void Select() {
        selected = true;
        selectionMarker.gameObject.SetActive(true);

    }
    public void Deselect() {
        selected = false;
        selectionMarker.gameObject.SetActive(false);
    }
    public void CollisionEnter(Collider collision) {
        action.CollisionEnter(collision);

    }
    public void CollisionExit(Collider collision) {
        action.CollisionExit(collision);

    }
    public void CollisionStay(Collider collision) {
        action.CollisionExit(collision);

    }
}
public class Order
{
    public string orderType;
    public GameObject orderTarget;
    public Vector3 orderPosition;
    public bool emptyQueue = true;
    public int orderTypeNumber;
    public Order(string type, Vector3 position, bool emptyOrderQueue = true) {
        orderType = type;
        orderPosition = position;
        emptyQueue = emptyOrderQueue;
    }
    public Order(string type, GameObject target, bool emptyOrderQueue = true) {
        orderType = type;
        orderTarget = target;
        emptyQueue = emptyOrderQueue;
    }
    public Order(string type) {
        orderType = type;
        emptyQueue = true;

    }
    public Order(int type) {
        orderTypeNumber = type;
        emptyQueue = true;

    }
    public Order(int type, Vector3 position, bool emptyOrderQueue) {
        orderTypeNumber = type;
        orderPosition = position;
        emptyQueue = emptyOrderQueue;
    }
    public Order(int type, GameObject target) {
        orderTypeNumber = type;
        orderTarget = target;
        emptyQueue = true;

    }
}





