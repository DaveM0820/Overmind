using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class BuildNewBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionReference triggerActionRefrence = null;
    private float triggerValue;
    public Button buildBarracks;
    public bool placing = false;
    private GameObject player;
    private int playerNumber;
    private int resources;
    private int buildingCost;
    GameObject RightCursor;
    GameObject objectToPlace;
    GameObject buildings;
    Transform scaffold;
    public bool buildingCollision;
    public int buildingsbuilt;


    void Start()
    {
        buildBarracks.onClick.AddListener(BuildBarracks);
        RightCursor = GameObject.Find("/UI/RightCursor");
        player = GameObject.Find("/XR Rig");
        buildings = GameObject.Find("/World/Buildings");
        playerNumber = player.GetComponent<GlobalGameInformation>().player;
        placing = false;
        buildingCollision = false;
        buildingsbuilt = 1;
    }
    void BuildBarracks()
    {
        resources = player.GetComponent<GlobalGameInformation>().playerResources[playerNumber];
        GameObject barracksOrgin = GameObject.Find("/World/Buildings/Barracks");
        buildingCost = player.GetComponent<GlobalGameInformation>().barracksCost;
        if (placing == false && resources > buildingCost) {
            buildingsbuilt += 1;
            objectToPlace = Instantiate(barracksOrgin, buildings.transform);
            objectToPlace.GetComponent<BuildingBehaviour>().maxScaffoldHeight = 11;
            objectToPlace.GetComponent<BuildingBehaviour>().name = "Barracks " + buildingsbuilt;
            objectToPlace.GetComponent<BuildingBehaviour>().placed = false;
            objectToPlace.GetComponent<BuildingBehaviour>().built = false;
            objectToPlace.GetComponent<BuildingBehaviour>().buidlingDimensions = new Vector2(20,20);
            objectToPlace.GetComponent<UnitBehaviour>().hpMax = 1000;
            objectToPlace.GetComponent<UnitBehaviour>().hp = 1;
            objectToPlace.GetComponent<UnitBehaviour>().unitType = "Barracks";
            objectToPlace.GetComponent<UnitBehaviour>().owner = player.GetComponent<GlobalGameInformation>().player;

            objectToPlace.GetComponent<BuildingBehaviour>().UpdateScaffold();
            player.GetComponent<UnitCommand>().placingBuilding = true;
            placing = true;
        }
 
    }
        // Update is called once per frame
        void Update()
    {
        if (placing == true)
        {
            triggerValue = triggerActionRefrence.action.ReadValue<float>();
            Vector3 cursorPosition = RightCursor.transform.position;
            Vector3 reduceScale = cursorPosition;
            reduceScale.x /= 5f;
            reduceScale.z /= 5f;
            Vector3 roundedPlacement = new Vector3(Mathf.Round(reduceScale.x), cursorPosition.y, Mathf.Round(reduceScale.z));
            roundedPlacement.x *= 5f;
            roundedPlacement.z *= 5f;
            roundedPlacement.y = 0;
            objectToPlace.transform.position = roundedPlacement;
            if (triggerValue == 1 && buildingCollision == false)
            {
                player.GetComponent<UnitCommand>().placingBuilding = false;
                player.GetComponent<UnitCommand>().GiveBuildOrder(objectToPlace);
                objectToPlace.GetComponent<BuildingBehaviour>().placed = true;
                player.GetComponent<GlobalGameInformation>().playerResources[playerNumber] -= buildingCost;
                placing = false;
                RaycastHit rightRaycastHit;
                RaycastHit leftRaycastHit;
                RaycastHit topRaycastHit;
                RaycastHit bottomRaycastHit;
                Debug.Log("BuildNewBuilding about to send raycasts to check sides");

                Physics.Raycast(objectToPlace.transform.position + new Vector3(objectToPlace.GetComponent<BuildingBehaviour>().buidlingDimensions.x + 5, 40, 0), Vector3.down, out rightRaycastHit, 30);
                Physics.Raycast(objectToPlace.transform.position + new Vector3(-objectToPlace.GetComponent<BuildingBehaviour>().buidlingDimensions.x - 5, 40, 0), Vector3.down, out leftRaycastHit, 30);
                Physics.Raycast(objectToPlace.transform.position + new Vector3(0, 40, objectToPlace.GetComponent<BuildingBehaviour>().buidlingDimensions.y + 5), Vector3.down, out topRaycastHit, 30);
                Physics.Raycast(objectToPlace.transform.position + new Vector3(0, 40, -objectToPlace.GetComponent<BuildingBehaviour>().buidlingDimensions.y - 5), Vector3.down, out bottomRaycastHit, 30);
                Debug.Log("BuildNewBuilding just sent raycasts to check sides");

                if (rightRaycastHit.point.y > objectToPlace.transform.position.y+3 || rightRaycastHit.transform.gameObject.layer == 7)
                {
                    objectToPlace.GetComponent<BuildingBehaviour>().rightSideClear = false;
                    rightRaycastHit.collider.gameObject.GetComponent<BuildingBehaviour>().leftSideClear = false;
                    Debug.Log("right side not clear");
                }  else {
                    objectToPlace.GetComponent<BuildingBehaviour>().rightSideClear = true;
                    Debug.Log("right side clear");

                }

                if (leftRaycastHit.point.y > objectToPlace.transform.position.y + 3 || leftRaycastHit.transform.gameObject.layer == 7)
                {
                    objectToPlace.GetComponent<BuildingBehaviour>().leftSideClear = false;
                    rightRaycastHit.collider.gameObject.GetComponent<BuildingBehaviour>().rightSideClear = false;
                    Debug.Log("left side not clear");


                }
                else {
                    objectToPlace.GetComponent<BuildingBehaviour>().leftSideClear = true;
                    Debug.Log("left side  clear");

                }

                if (topRaycastHit.point.y > objectToPlace.transform.position.y + 3 || topRaycastHit.transform.gameObject.layer == 7)
                {
                    objectToPlace.GetComponent<BuildingBehaviour>().topSideClear = false;
                    rightRaycastHit.collider.gameObject.GetComponent<BuildingBehaviour>().bottomSideClear = false;
                    Debug.Log("top side not clear");


                }
                else  {
                    objectToPlace.GetComponent<BuildingBehaviour>().topSideClear = true;
                    Debug.Log("top side  clear");

                }

                if (bottomRaycastHit.point.y > objectToPlace.transform.position.y + 3 || bottomRaycastHit.transform.gameObject.layer == 7)
                {
                    objectToPlace.GetComponent<BuildingBehaviour>().bottomSideClear = false;
                    rightRaycastHit.collider.gameObject.GetComponent<BuildingBehaviour>().topSideClear = false;
                    Debug.Log("bottom side not clear");

                }
                else  {
                    objectToPlace.GetComponent<BuildingBehaviour>().bottomSideClear = true;
                    Debug.Log("bottom side clear");

                }


            }
        }
  

    }
}
