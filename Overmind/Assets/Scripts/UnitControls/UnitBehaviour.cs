using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Vectrosity;

public class UnitBehaviour : MonoBehaviour //UnitBehavior is the core class that defines how basically everything in the game operates. Every unit and building has it. 
{
    public List<Order> orderQueue; // this has a list of all the orders this unit or building has to complete
    GlobalGameInformation globalGameInformation; // this contains key information about the game, such as lists of units in each faction, player resources, update rate of the game, ect
    GameObject player;//this is hte XR Rig, it has many important scripts used for selecting units, ordering units, moving the camera, ect
    public bool attackMoveEnabled; // should the unit attack any unit it encounters
    public bool attackWhileMoving; // can this unit shoot while moving
    public bool selected; // is this unit currently selected
    Transform selectionMarker; //the greeen health bar and circle at the units feet
    JobManager jobManager; // used to multithread tasks
    public int faction;
    public int cost;
    public int hpMax;
    public int hp;
    public bool military; // is this unit a military unit, used to determine if it should be a priority target when selecting target
    public float hpPercent;
    public string unitType; //the unit type, ex builder, infantry ect
    public float unitBuildTime;// how long it takes to build by default
    public bool isBuilding;// is it a building
    public int unitRange; //range of weapons
    public float cameraSizeWhenUnderDirectControl;// what is the size of player when unit is being directly controlled
    public bool assumingDirectControl = false; //currently assuming direct control of this unit?
    public float headHeight;// head hight, used for assuming direct control and when other units target and shoot it
    public float moveSpeed;
    private float elapsed = 0;// counter for when to update unit
    public float updateTimestep;// how long to wait before running the current order again
    Select selectScript;
    public Sprite unitIcon;
    public bool queueIsEmpty = true;
    IUnitActionInterface action;
    public int hpChange;
    public Unit thisUnit;

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
    public bool hasTarget;
    public VectorLine orderLine;
    public VectorLine rangeCircle;
    public Order finalOrder;
    public int transformNumber;
    public int turretNumber;
    public bool canFire;
    public int vehicleNumber;
    public bool hasTurret;
    public bool directMove;
    void Awake() {

        finalOrder = new Order(0);
        fullFPSTimeStep = 1f / 72f;
        player = GameObject.Find("/XR Rig");
        orderQueue = new List<Order>();
        selectScript = player.GetComponent<Select>();
        selectionMarker = transform.Find("SelectionMarker");
        // LookForTargetsCounterMax = player.GetComponent<GlobalGameInformation>().LookForTargetsCounterMax;
      
        if (!military)
        {
            currentTarget = gameObject;
        }
        if (isBuilding == false )
        {
            updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        }
        else
        {
            if (!military)
            {

                updateTimestep = player.GetComponent<GlobalGameInformation>().buildingUpdateTimestep; //ex) if units update at 40fps, buildings update at 4fps
            }
            else
            {
                updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;


            }
        }
        action = gameObject.GetComponent<IUnitActionInterface>();
        thisUnit = new Unit(faction, gameObject, military);

        globalGameInformation = player.GetComponent<GlobalGameInformation>();
        globalGameInformation.unitList[faction].Add(thisUnit);
        jobManager = player.GetComponent<JobManager>();
        if (directMove)
        {
        jobManager.AddTransformToMove(transform, moveSpeed);
        }
        Deselect();
    }

    // bascially orders are sent from UnitCommand and other scripts, and added to the order queue for each unit, which are then passed to the unit specific scrip through the IunitActionInterface
    /* void Update() // updates every updateTimeStep, initally set by updateFPS in GlobalGameInformation. This way the framerate of units can be increased or decreased depending on current performance.
    {
    
               if (!assumingDirectControl) // if you are not directly controlling the unit
        {
            if (!queueIsEmpty)
            {
                // and there are orders in the orderQueue
                elapsed += fullFPSTimeStep;
                if (elapsed >= updateTimestep) //run the order every updateTimestep using the UpdateUnit method
                {
                    elapsed = 0;
                    UpdateUnit();
                }
                if (!hasTarget && currentOrderTypeNumber == 1 && (attackMoveEnabled || attackWhileMoving))
                {
                    LookForTargetsCounter++;
                    if (LookForTargetsCounter > LookForTargetsCounterMax)
                    {
                        LookForTargets();
                    }
                }
            }
            else
            {
                if (!hasTarget)
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

}*/
    public void UpdateUnit() // this is the core function that impliments unit actions through the IUnitActionInterface, happens every updateTimeStep, by default 40fps
 {
        switch (currentOrderTypeNumber)
        {
            case 1:
            action.Move(orderQueue[0].orderPosition);
            break;
            case 2:
            action.Build(orderQueue[0].orderTarget);
            break;
            case 3:
            action.Attack(orderQueue[0].orderTarget);
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
                if (hasTurret)
                {
                    jobManager.StopMovingTurret(turretNumber);
                }
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
    public void Attack(GameObject target) {
        currentTarget = target;
        hasTarget = true;
        if (currentOrderTypeNumber == 1 && attackMoveEnabled)
        {
            orderQueue[0] = new Order("attack", target, true);
            currentOrderTypeNumber = 3;
            UpdateUnit();

        }
        else
        {
            addOrderToQueue(new Order("attack", target, true));
            UpdateUnit();
        }
    }
    public void TargetGone() {
        if (hasTarget)
        {
        if (hasTurret)
        {
            jobManager.StopMovingTurret(turretNumber);
        }
        if (orderQueue[0].orderTypeNumber == 3)
        {
            OrderComplete();
        }
        currentTarget = null;
        hasTarget = false;
        }
    }
    public void ClearAllOrders() {
        hasTarget = false;
        currentTarget = null;
        queueIsEmpty = true;
        currentOrderTypeNumber = 0;
        orderQueue.Clear();

    }
    public void addOrderToQueue(Order order)//adds an order to the unit's order queue
    {

        if (order.emptyQueue)//should the order delete all preceding orders(IE be executed immediately)
        {
            OrderComplete();
            orderQueue.Clear();
        }

        //convert the string to the coorisponding order number, that way the game isn't comparing 40 strings per second per unit, while allowing the code to remain understandable when issuing orders to units
        if (order.orderTypeNumber == 0)
        {
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
                break;
            }
        }
        orderQueue.Add(order);
        currentOrderTypeNumber = orderQueue[0].orderTypeNumber;

        UpdateUnit();
        if (selected)
        {
             selectScript.DrawOrderLine(gameObject);
        }

    }
    public void OrderComplete() {
        if (orderQueue.Count == 0) // if there is no order to complete
        {
            hasTarget = false;
            currentTarget = null;
            queueIsEmpty = true;
            currentOrderTypeNumber = 0;
            action.Stop();
        }
        else
        {

            if (orderQueue[0].orderTypeNumber == 3)// if the order that was completed was an attack order
            {
                hasTarget = false;
                currentTarget = null;
            
            }
           // Debug.Log("OrderComplete about to remove order " + currentOrderTypeNumber + " at location " + orderQueue[0].orderPosition);

            orderQueue.RemoveRange(0, 1);
          //  Debug.Log("OrderComplete removed order for " + gameObject.name);

            if (orderQueue.Count == 0) // if there are now no orders
            {
                hasTarget = false;
                currentTarget = null;
                queueIsEmpty = true;
                currentOrderTypeNumber = 0;
                attackMoveEnabled = true;
                action.Stop();

            }
            else //if there are still orders left
            {
                currentOrderTypeNumber = orderQueue[0].orderTypeNumber;
             //   Debug.Log("OrderComplete removed order and ran update unit for " + gameObject.name);
              //  Debug.Log("OrderComplete new order type is " + currentOrderTypeNumber + " at location " + orderQueue[0].orderPosition );
                UpdateUnit();

            }
            if (selected)
            {
                  selectScript.DrawOrderLine(gameObject);
            }
        }


    }
    public void RemoveOrder(int orderNumber) {
        if (orderQueue.Count > 0)
        {
            if (orderQueue[orderNumber].orderType == "build")
            {
                gameObject.GetComponent<BuilderUnitBehaviour>().stopBuilding();
            }
            orderQueue.RemoveRange(orderNumber, 1);
        }
        if (orderQueue.Count == 0)
        {
            queueIsEmpty = true;
            currentOrderTypeNumber = 0;

        }

        currentOrderTypeNumber = orderQueue[0].orderTypeNumber;
        if (selected)
        {
            // selectScript.DrawOrderLine(gameObject);
        }
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
    public void OnPathComplete(Path p) {

        if (p.error)
        {


        }
        else
        {

            int numberOfOrders = 0;

            if (finalOrder.emptyQueue)
            {
                orderQueue.Clear();
                OrderComplete();
            }
            finalOrder.emptyQueue = false;

           // List<Vector3> orderpostitions = new List<Vector3>();
            foreach (Vector3 point in p.vectorPath)
            {
                if (numberOfOrders > 0)
                {
               // orderpostitions.Add(point);
                addOrderToQueue(new Order(1, point, false, finalOrder));
                }
                numberOfOrders++;

            }
            addOrderToQueue(finalOrder);
            if (selected)
            {
                selectScript.DrawOrderLine(gameObject);
            }
            if (currentOrderTypeNumber == 3)
            {
                attackMoveEnabled = false;
            
            }
        }
    }
}
public class Order
{
    public string orderType = "";
    public GameObject orderTarget = null;
    public Vector3 orderPosition = new Vector3(0,0,0);
    public bool emptyQueue = true;
    public int orderTypeNumber = 0;
    public Order(string type, Vector3 position, bool emptyorderQueue = true, Order assignedToGoal = null) {
        orderType = type;
        orderPosition = position;
        emptyQueue = emptyorderQueue;

    }
    public Order(string type, GameObject target, bool emptyorderQueue = true, Order assignedToGoal = null) {
        orderType = type;
        orderTarget = target;
        emptyQueue = emptyorderQueue;

    }
    public Order(string type, bool emptyorderQueue = true, Order assignedToGoal = null) {
        orderType = type;
        emptyQueue = true;


    }

    public Order(int orderTypeNumber, bool emptyorderQueue = true, Order assignedToGoal = null) {
        this.orderTypeNumber = orderTypeNumber;
        emptyQueue = true;


    }
    public Order(int orderTypeNumber, Vector3 position, bool emptyorderQueue = true, Order assignedToGoal = null) {
        this.orderTypeNumber = orderTypeNumber;
        orderPosition = position;
        emptyQueue = emptyorderQueue;

    }

    public Order(int orderTypeNumber, GameObject target, bool emptyorderQueue = true, Order assignedToGoal = null) {
        this.orderTypeNumber = orderTypeNumber;
        orderTarget = target;
        emptyQueue = true;

    }
}





