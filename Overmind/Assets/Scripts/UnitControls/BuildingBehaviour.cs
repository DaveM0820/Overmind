using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> buildQueue;
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
    public bool xNegitiveSideClear;
    public bool xPositiveSideClear;
    public bool zNegitiveSideClear;
    public bool zPositiveSideClear;
    private float unitBuildProgress;
    public float unitBuildTime;

    private void Start()
    {
        unitBuildProgress = 0;
        unitBuildTime = 0;

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
            scaffold.gameObject.SetActive(false);


        }
    }
    public void BuildUnit(GameObject unit)
    {
        Debug.Log("BuildingBehaviour script got order");

        unitBuildProgress += GetComponent<UnitBehaviour>().updateTimestep;
        unitBuildTime = unit.GetComponent<UnitBehaviour>().unitBuildTime;
        if(unitBuildProgress> unitBuildTime)
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
            newUnit.transform.position = transform.position + new Vector3(0, 0, -20);
            unitBuildProgress = 0;
            newUnit.SetActive(true);
            GetComponent<UnitBehaviour>().OrderComplete();
            Debug.Log("BuildingBehaviour finished order");

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

        }
      
    }

}
