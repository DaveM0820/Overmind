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
            objectToPlace.GetComponent<BuildingBehaviour>().maxScaffoldHeight = 9;
            objectToPlace.GetComponent<BuildingBehaviour>().name = "barracks " + buildingsbuilt;
            objectToPlace.GetComponent<BuildingBehaviour>().placed = false;
            objectToPlace.GetComponent<BuildingBehaviour>().built = false;
            objectToPlace.GetComponent<UnitBehaviour>().hpMax = 1000;
            objectToPlace.GetComponent<UnitBehaviour>().hp = 1;
            objectToPlace.GetComponent<UnitBehaviour>().unitType = "barracks";
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
            reduceScale.x /= 4f;
            reduceScale.z /= 4f;
            Vector3 roundedPlacement = new Vector3(Mathf.Round(reduceScale.x), cursorPosition.y, Mathf.Round(reduceScale.z));
            roundedPlacement.x *= 4f;
            roundedPlacement.z *= 4f;
            roundedPlacement.y = 0;
            objectToPlace.transform.position = roundedPlacement;
            if (triggerValue == 1 && buildingCollision == false)
            {
                player.GetComponent<UnitCommand>().placingBuilding = false;
                player.GetComponent<UnitCommand>().GiveBuildOrder(objectToPlace);
                Debug.Log("sent bulid order from BuildNewBuilding.cs");

                objectToPlace.GetComponent<BuildingBehaviour>().placed = true;
                player.GetComponent<GlobalGameInformation>().playerResources[playerNumber] -= buildingCost;
                placing = false;

            }
        }
  

    }
}
