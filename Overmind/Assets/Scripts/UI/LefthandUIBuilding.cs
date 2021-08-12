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
    GameObject buildingIconGameObject;
    public GameObject buildingImage;
    GameObject unitIconGameObject;
    Image unitIconImage;

    void Start()
    {


    }
    private void OnEnable()
    {
        UpdateUnitQueueDisplay();
                buildBuilder.onClick.AddListener(BuildBuilder);

    }
    public void UpdateUnitQueueDisplay()
    {
        unitIcons = new List<GameObject>();
        //Image unitImage;
        //unitImages = new List<Image>();
        selectedUnits = Select.SelectedUnits;

        buildingIconGameObject = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel/BuildingIcon");
        buildingIconImage = buildingIconGameObject.GetComponent<Image>();
        buildingIconImage.sprite = selectedUnits[0].GetComponent<UnitBehaviour>().unitIcon;

        BuildingSelected = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel/BuildingType").GetComponent<Text>();
        BuildingSelected.text = selectedUnits[0].GetComponent<UnitBehaviour>().unitType;

        GameObject unitqueueViewport = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel/BuildingQueue/Viewport/Content");
        GameObject unitIcon = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel/BuildingQueue/Viewport/Content/UnitIcon");

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
        GameObject buildprogress = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuildingScreen/Panel/BuildingQueue/Viewport/Content/UnitIcon/UnitName/BuildProgress");
        buildprogress.transform.localScale = new Vector3(0, buildprogress.transform.localScale.y, buildprogress.transform.localScale.z);
        if (selectedUnits.Count == 1)
        {
            Debug.Log("Lefthandbuildingui Total in order Queue to display = " + selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue.Count);
            Debug.Log("Lefthandbuildingui Did it find unitIcon? : " + unitIcon.name);

            Text unitNameText;
            unitIconsPlaced = 0;
            while (unitIconsPlaced < selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue.Count)
            {

                if (unitIconsPlaced == 0)
                {

                    unitIcons.Add(unitIcon);
                    unitIcon.SetActive(true);

                    float newX = unitIcon.transform.localPosition.x + (unitIconsPlaced * 55);
                    unitIcons[unitIconsPlaced].transform.localPosition = new Vector3(newX, unitIcon.transform.localPosition.y, unitIcon.transform.localPosition.z);
                    unitNameText = unitIcons[unitIconsPlaced].transform.Find("UnitName").GetComponent<Text>();
                    unitNameText.text = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType;
                  //  unitImage = unitIcons[unitIconsPlaced].transform.Find("Image").gameObject.GetComponent<Image>();
                   // unitImage.sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
                    unitImages.Add(unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>());
                    unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>().sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;

                }
                else
                {

                    unitIcons.Add(Instantiate(unitIcon, unitIcon.transform.parent));
                    unitIcons[unitIconsPlaced].SetActive(true);
                    unitIcons[unitIconsPlaced].transform.localPosition = new Vector3(unitIcon.transform.localPosition.x + (unitIconsPlaced * 55), unitIcon.transform.localPosition.y, unitIcon.transform.localPosition.z);
                    unitIcons[unitIconsPlaced].name = "unitIcon" + unitIconsPlaced;
                   unitNameText = unitIcons[unitIconsPlaced].transform.Find("UnitName").GetComponent<Text>();
                    unitNameText.text = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType;
                    Debug.Log("Placed a new uniticon in the building screen");
                    // unitImage = unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>();
                   // unitImage.sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
                   // unitImages.Add(unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>());
                    //unitIcons[unitIconsPlaced].transform.Find("Image").GetComponent<Image>().sprite = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitIcon;
                }

                //Debug.Log("Lefthandbuilding str name = " + name);

                //unitIcons[unitIconsPlaced].name =  "unitIcon" + unitIconsPlaced;;
                // Debug.Log("Lefthandbuilding str unitIcons[unitIconsPlaced].name = " + unitIcons[unitIconsPlaced].name);

                // Debug.Log("Lefthandbuildingui Text for unit icon should be " + selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType);

                // unitNameText = unitIcons[unitIconsPlaced].transform.Find("UnitName").GetComponent<Text>();
                //unitNameText.text = selectedUnits[0].GetComponent<UnitBehaviour>().OrderQueue[unitIconsPlaced].orderTarget.GetComponent<UnitBehaviour>().unitType;
                // unitIconButton[unitIconsPlaced] = unitIcons[unitIconsPlaced].GetComponent<Button>();
                // unitIconButton[unitIconsPlaced].onClick.AddListener(delegate { CancelBuildOrder(unitIconsPlaced); });
                // Debug.Log("Lefthandbuilding ui count = " + unitIconsPlaced);
                unitIconsPlaced++;


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
        GameObject buildTarget = GameObject.Find("/World/Units/Builder");
        Order buildorder = new Order("buildunit", buildTarget, false);
        selectedUnits[0].GetComponent<UnitBehaviour>().addOrderToQueue(buildorder);
        UpdateUnitQueueDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
