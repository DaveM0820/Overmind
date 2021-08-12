using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> buildableUnits;
    private Transform scaffold;
    public float maxScaffoldHeight;
    private float scaffoldPosition;
    public float scaffoldHeight;
    public bool placed;
    public bool built;
    private float hp;
    private float hpMax;
    private float hpPercent;
    private float elapsed;
    public Vector3 rallyPoint;
    public Vector3 CreationPoint;
    private float unitBuildProgress;
    public float unitBuildTime;

    public bool leftSideClear;
    public bool rightSideClear;
    public bool topSideClear;
    public bool bottomSideClear;
    GameObject player;

    public Vector2 buidlingDimensions;

    private void Start()
    {
        unitBuildProgress = 0;
        unitBuildTime = 0;
        rallyPoint = new Vector3(transform.position.x, transform.position.y , transform.position.z - 10);

    }

    void Update()
    {
        if (built == false)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 0.25f)
            {
                elapsed = elapsed % 0.25f;
                UpdateScaffold();
            }
        } else
        {



        }
    }
    public void BuildUnit(GameObject unit)
    {

        if (bottomSideClear || rightSideClear || topSideClear || leftSideClear)
        {
            if (Select.SelectedUnits[0] == gameObject && Select.SelectedUnits.Count == 1)
            {
                GameObject BuildingScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen");
                if (BuildingScreen.activeInHierarchy == true)
                {
                    GameObject buildprogress = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel/BuildingQueue/Viewport/Content/UnitIcon/UnitName/BuildProgress");
                    buildprogress.transform.localScale = new Vector3((unitBuildProgress / unitBuildTime), buildprogress.transform.localScale.y, buildprogress.transform.localScale.z);
                }

            }



            unitBuildProgress += GetComponent<UnitBehaviour>().updateTimestep;
        unitBuildTime = unit.GetComponent<UnitBehaviour>().unitBuildTime;

            if (unitBuildProgress> unitBuildTime)
        {
            GameObject.Find("/XR Rig").GetComponent<GlobalGameInformation>().numberOfUnitsBuilt += 1;
            GameObject newUnit = Instantiate(unit,unit.transform.parent);
            newUnit.name = unit.name + GameObject.Find("/XR Rig").GetComponent<GlobalGameInformation>().numberOfUnitsBuilt;
            newUnit.GetComponent<UnitBehaviour>().hp = unit.GetComponent<UnitBehaviour>().hp;
            newUnit.GetComponent<UnitBehaviour>().hpMax = unit.GetComponent<UnitBehaviour>().hpMax;
            newUnit.GetComponent<UnitBehaviour>().owner = unit.GetComponent<UnitBehaviour>().owner;
            newUnit.GetComponent<UnitBehaviour>().unitType = unit.GetComponent<UnitBehaviour>().unitType;
            newUnit.GetComponent<UnitBehaviour>().unitRange = unit.GetComponent<UnitBehaviour>().unitRange; 
            newUnit.GetComponent<UnitBehaviour>().cameraSizeWhenUnderDirectControl = unit.GetComponent<UnitBehaviour>().cameraSizeWhenUnderDirectControl;
            newUnit.GetComponent<UnitBehaviour>().headHeight = unit.GetComponent<UnitBehaviour>().headHeight;
            newUnit.GetComponent<UnitBehaviour>().moveType = unit.GetComponent<UnitBehaviour>().moveType;
            newUnit.GetComponent<UnitBehaviour>().moveSpeed = unit.GetComponent<UnitBehaviour>().moveSpeed;
            newUnit.GetComponent<UnitBehaviour>().turnSpeed = unit.GetComponent<UnitBehaviour>().turnSpeed;
            newUnit.GetComponent<UnitBehaviour>().acceleration = unit.GetComponent<UnitBehaviour>().acceleration;

                if (bottomSideClear)
                {
                    newUnit.transform.position = transform.position + new Vector3(0, 0, -23);
                }
   
            unitBuildProgress = 0;
            newUnit.SetActive(true);
            GetComponent<UnitBehaviour>().OrderComplete();
            Debug.Log("BuildingBehaviour finished order");
                GameObject BuildingScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen");

                if (BuildingScreen.activeInHierarchy == true)
            {

                    BuildingScreen.GetComponent<LefthandUIBuilding>().UpdateUnitQueueDisplay();
                }
        }
        } else
        {
            Debug.Log("all exits to buidling blocked");
        }
    }
    // Update is called once per frame
    public void UpdateScaffold()
    {
        scaffold = transform.Find("Scaffold");
        hp = gameObject.GetComponent<UnitBehaviour>().hp;
        hpMax = gameObject.GetComponent<UnitBehaviour>().hpMax;

        hpPercent = 100 * (hp / hpMax);
        scaffoldHeight = maxScaffoldHeight - ((maxScaffoldHeight) * (hpPercent / 100));

        scaffold.position = new Vector3(transform.position.x, scaffoldHeight, transform.position.z);
        if (hp >= hpMax)
        {
           built = true;
           gameObject.GetComponent<UnitBehaviour>().hp = hpMax;
            Destroy(scaffold);


        }

    }

}
