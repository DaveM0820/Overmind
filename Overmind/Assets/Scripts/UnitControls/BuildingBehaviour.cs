using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour, IUnitActionInterface
{
    // Start is called before the first frame update
    public List<string> buildableUnits;
    public Transform scaffold;
    public float maxScaffoldHeight;
    private float scaffoldPosition;
    public float scaffoldHeight;
    public bool placed;
    public bool built;
    public int hp;
    public int hpMax;
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
    public float oreExtractionTime;
    public float oreExtractionProgress;
    public int oreExtractorResourceDirection;
    public int oreExtrationRate;
    public Vector3 oreLocation;
    GameObject miningBeam;
    public Vector2 buidlingDimensions;
    public GameObject text;
    GameObject buildprogress;
    GlobalGameInformation gameInformation;
    GameObject BuildingScreen;
    UnitBehaviour buldingUnitBehaviour;
    Vector3 walkoutpoint;
    GameObject units;
    void Start() {
        unitBuildProgress = 0;
        unitBuildTime = 0;
        rallyPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10);
        buildprogress = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel2/BuildingQueue/Viewport/Content/UnitIcon/UnitName/BuildProgress");
        player = GameObject.Find("/XR Rig");
        gameInformation = player.GetComponent<GlobalGameInformation>();
        BuildingScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen");
        buldingUnitBehaviour = gameObject.GetComponent<UnitBehaviour>();
        walkoutpoint = transform.position;
        player = GameObject.Find("/XR Rig");
        units = GameObject.Find("/World/Units");
    }

    public void Move(Vector3 location) {

    }
    public void Attack(GameObject Target) {

    }
    public void Stop() {

    }
    public void EnterDirectControl() {
    }
    public void ExitDirectControl() {
    }
    public void UnderDirectControl() {
    }
    public void ExtractOre() {


        oreExtractionProgress += GetComponent<UnitBehaviour>().updateTimestep;
        if (oreExtractionProgress > oreExtractionTime)
        {

            Vector3 beamHit = oreLocation + new Vector3(Random.Range(0, 10), 1, Random.Range(0, 10)) + new Vector3(-5, 0, -5);

            miningBeam.transform.LookAt(beamHit, Vector3.up);


            float beamLength = Vector3.Distance(transform.position, beamHit);
            miningBeam.transform.localScale = new Vector3(1, 1, beamLength);
            GameObject newText = Instantiate(text);

            newText.GetComponent<TextMesh>().text = "+" + oreExtrationRate + " ORE";
            newText.transform.position = beamHit;
            gameInformation.playerResources[gameInformation.player] += oreExtrationRate;
            oreExtractionProgress = 0;

        }


    }
    public void Build(GameObject unit)//build a unit
    {

     
            if (Select.SelectedUnits[0] == gameObject && Select.SelectedUnits.Count == 1)
            {
                GameObject BuildingScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen");
                if (BuildingScreen.activeInHierarchy == true)
                {
                    buildprogress.transform.localScale = new Vector3((unitBuildProgress / unitBuildTime), buildprogress.transform.localScale.y, buildprogress.transform.localScale.z);
                }

            }



            unitBuildProgress += GetComponent<UnitBehaviour>().updateTimestep;
            unitBuildTime = unit.GetComponent<UnitBehaviour>().unitBuildTime;

            if (unitBuildProgress > unitBuildTime)
            {
                player.GetComponent<GlobalGameInformation>().numberOfUnitsBuilt += 1;

            GameObject newUnit = Instantiate(unit, units.transform);
                UnitBehaviour newUnitBehavior = newUnit.GetComponent<UnitBehaviour>();
                newUnitBehavior.owner = player.GetComponent<GlobalGameInformation>().player;

            newUnit.name = unit.name + player.GetComponent<GlobalGameInformation>().numberOfUnitsBuilt;



            buldingUnitBehaviour.OrderComplete();
 

            unitBuildProgress = 0;

                if (bottomSideClear)
                {
                    if (buldingUnitBehaviour.unitType == "HQ")
                    {
                        newUnit.transform.position = transform.position - new Vector3(0, 0, 10);
                        walkoutpoint = transform.position - new Vector3(0, 0, 20);

                    }
                }




                if (BuildingScreen.activeInHierarchy)
                {

                    BuildingScreen.GetComponent<LefthandUIBuilding>().UpdateUnitQueueDisplay();
                }
                newUnitBehavior.addOrderToQueue(new Order("move", walkoutpoint));
            newUnitBehavior.Deselect();

        }

    }
    // Update is called once per frame
    public void UpdateScaffold()//used to update the scaffolding while the building is building
    {

        hp = gameObject.GetComponent<UnitBehaviour>().hp;

        hpMax = gameObject.GetComponent<UnitBehaviour>().hpMax;
        hpPercent = 100f * ((float)hp / (float)hpMax);

        scaffoldHeight = maxScaffoldHeight - ((maxScaffoldHeight) * (hpPercent / 100));

        scaffold.position = new Vector3(transform.position.x, scaffoldHeight, transform.position.z);

        if (hp >= hpMax)
        {
            built = true;
            gameObject.GetComponent<UnitBehaviour>().OrderComplete();
            if (GetComponent<UnitBehaviour>().unitType == "Refinery")
            {
                Order extractOre = new Order("extractOre");
                gameObject.GetComponent<UnitBehaviour>().addOrderToQueue(extractOre);
                oreExtractorResourceDirection = scaffold.GetComponent<ScaffoldCollision>().oreExtractorResourceDirection;
                oreLocation = scaffold.GetComponent<ScaffoldCollision>().oreLocation;

                player.GetComponent<GlobalGameInformation>().resourcesPerMin[player.GetComponent<GlobalGameInformation>().player] += (int)(60f / oreExtractionTime) * oreExtrationRate;
                if (oreExtractorResourceDirection == -1)
                {
                    Destroy(gameObject.transform.Find("BuilderBeam1").gameObject);
                    gameObject.transform.Find("BuilderBeam-1").name = "BuilderBeam";
                }
                else
                {
                    Destroy(gameObject.transform.Find("BuilderBeam-1").gameObject);
                    gameObject.transform.Find("BuilderBeam1").name = "BuilderBeam";
                }
                miningBeam = gameObject.transform.Find("BuilderBeam").gameObject;
            }
            gameObject.GetComponent<UnitBehaviour>().hp = hpMax;
            Destroy(scaffold);


        }

    }

}
