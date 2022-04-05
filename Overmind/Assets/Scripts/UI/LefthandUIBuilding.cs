using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LefthandUIBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> selectedUnits;
    private List<GameObject> unitIcons = null;
    private List<Image> unitImages;
    public Text BuildingSelected;
    public Button buildBuilder;
    public List<Button> unitIconButton;
    int unitIconsPlaced;
    Image buildingIconImage;
    public GameObject buildingIconGameObject;
    GameObject unitIconGameObject;
    Image unitIconImage;
    int buttonpresses;
    public GameObject builderPrefab;
    public GameObject panel2;
    public GameObject BarracksScreen;
    public GameObject HQScreen;
    public GameObject RefineryScreen;
    public GameObject TurretScreen;
    public GameObject BuildingQueue;
    GameObject unitqueueViewport;
        GameObject unitIcon;
        GameObject buildprogress;
    GameObject healthbar;
    GameObject player;
    void Start()
    {
        buttonpresses = 0;
        buildBuilder.onClick.AddListener(BuildBuilder);


        unitqueueViewport = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel2/BuildingQueue/Viewport/Content");
        unitIcon = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel2/BuildingQueue/Viewport/Content/UnitIcon");
        buildprogress = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel2/BuildingQueue/Viewport/Content/UnitIcon/UnitName/BuildProgress");
    }
    private void OnEnable()
    {
        selectedUnits = Select.SelectedUnits;

        foreach (Transform child in panel2.transform)
        {
            child.gameObject.SetActive(false);
        }
        BuildingQueue.SetActive(false);

       // buildingIconGameObject.GetComponent<Image>().sprite = selectedUnits[0].GetComponent<UnitBehaviour>().unitIcon;
        
        BuildingSelected.text = selectedUnits[0].GetComponent<UnitBehaviour>().unitType;

        if (selectedUnits[0].GetComponent<UnitBehaviour>().unitType == "Barracks")
        {
            BarracksScreen.SetActive(true);
            BuildingQueue.SetActive(true);
            UpdateUnitQueueDisplay();

        }
        else if (selectedUnits[0].GetComponent<UnitBehaviour>().unitType == "HQ")
        {
            HQScreen.SetActive(true);
            BuildingQueue.SetActive(true);
            UpdateUnitQueueDisplay();

        }
        else if (selectedUnits[0].GetComponent<UnitBehaviour>().unitType == "Refinery")
        {
            RefineryScreen.SetActive(true);

        }
        else if (selectedUnits[0].GetComponent<UnitBehaviour>().unitType == "Turret")
        {
            TurretScreen.SetActive(true);

        }

        if (selectedUnits.Count == 1)
        {
            UpdateUnitQueueDisplay();
        }

    }
    public void UpdateUnitQueueDisplay()
    {

        unitIcons = new List<GameObject>();
        //Image unitImage;
        //unitImages = new List<Image>();

        foreach (Transform child in unitqueueViewport.transform)
        {
            if (child.gameObject == unitIcon) {
                unitIcon.SetActive(false);
            }
            else
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        buildprogress.transform.localScale = new Vector3(0, buildprogress.transform.localScale.y, buildprogress.transform.localScale.z);
        if (selectedUnits.Count == 1)
        {
            Text unitNameText;
            unitIconsPlaced = 0;
            while (unitIconsPlaced < selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue.Count)
            {

                if (unitIconsPlaced == 0)
                {

                    unitIcons.Add(unitIcon);
                    unitIcon.SetActive(true);
                 //   Debug.Log("palcing first building unit icon");

                    float newX = unitIcon.transform.localPosition.x + (unitIconsPlaced * 55);
                    unitIcons[unitIconsPlaced].transform.localPosition = new Vector3(newX, unitIcon.transform.localPosition.y, unitIcon.transform.localPosition.z);
                    unitNameText = unitIcons[unitIconsPlaced].transform.Find("UnitName").GetComponent<Text>();
                    unitNameText.text = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType;
                  //  unitImage = unitIcons[unitIconsPlaced].transform.Find("Image").gameObject.GetComponent<Image>();
                   // unitImage.sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
                   // unitImages.Add(unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>());
                    unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>().sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
             //       Debug.Log("placed  first building unit icon");

                }
                else
                {
                 //   Debug.Log("about to placed a new uniticon in the building screen");

                    unitIcons.Add(Instantiate(unitIcon, unitIcon.transform.parent));
                 //   Debug.Log("total unit icons in list = " + unitIcons.Count);
                   // Debug.Log("current one tryin to modify =" + unitIconsPlaced);

                    unitIcons[unitIconsPlaced].SetActive(true);
                    unitIcons[unitIconsPlaced].transform.localPosition = new Vector3(unitIcon.transform.localPosition.x + (unitIconsPlaced * 55), unitIcon.transform.localPosition.y, unitIcon.transform.localPosition.z);
                    unitIcons[unitIconsPlaced].name = "unitIcon" + unitIconsPlaced;
                   unitNameText = unitIcons[unitIconsPlaced].transform.Find("UnitName").GetComponent<Text>();
                    unitNameText.text = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType;
                    unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>().sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;

                 //   Debug.Log("Placed a new uniticon in the building screen");
                     //unitImage = unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>();
                   // unitImage.sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
                    //unitImages.Add(unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>());
                    //unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>().sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
                }
                unitIconsPlaced++;

                //Debug.Log("Lefthandbuilding str name = " + name);

                //unitIcons[unitIconsPlaced].name =  "unitIcon" + unitIconsPlaced;;
                // Debug.Log("Lefthandbuilding str unitIcons[unitIconsPlaced].name = " + unitIcons[unitIconsPlaced].name);

                // Debug.Log("Lefthandbuildingui Text for unit icon should be " + selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType);

                // unitNameText = unitIcons[unitIconsPlaced].transform.Find("UnitName").GetComponent<Text>();
                //unitNameText.text = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType;
                // unitIconButton[unitIconsPlaced] = unitIcons[unitIconsPlaced].GetComponent<Button>();
                // unitIconButton[unitIconsPlaced].onClick.AddListener(delegate { CancelBuildOrder(unitIconsPlaced); });
                // Debug.Log("Lefthandbuilding ui count = " + unitIconsPlaced);


            }

        }

    }
    public void CancelBuildOrder(int orderNumber)
    {
        if (selectedUnits.Count == 1)
        {
            selectedUnits[0].GetComponent<UnitBehaviour>().RemoveOrder(orderNumber);
            UpdateUnitQueueDisplay();
        }
    }
    public void BuildBuilder()
    {
        if (selectedUnits.Count == 1)
        {
            if (selectedUnits[0].GetComponent<BuildingBehaviour>().built == true)
            {
                player = GameObject.Find("/XR Rig");

                GameObject buildTarget = builderPrefab;
                player.GetComponent<GlobalGameInformation>().playerResources[player.GetComponent<GlobalGameInformation>().player] -= buildTarget.GetComponent<UnitBehaviour>().cost;

                Order buildorder = new Order("build", buildTarget, false);
                selectedUnits[0].GetComponent<UnitBehaviour>().addOrderToQueue(buildorder);
                buttonpresses += 1;
                UpdateUnitQueueDisplay();


            }
        }
   
    }

    // Update is called once per frame

}
