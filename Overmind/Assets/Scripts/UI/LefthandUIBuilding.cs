using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LefthandUIBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> selectedUnits;
    public Text BuildingSelected;
    public Button buildBuilder;
    void Start()
    {
        buildBuilder.onClick.AddListener(BuildBuilder);

    }
    private void OnEnable()
    {
        selectedUnits = Select.SelectedUnits;

        if (selectedUnits.Count == 1)
        {
            BuildingSelected.text = selectedUnits[0].GetComponent<UnitBehaviour>().unitType;

        }
    }
    private void BuildBuilder()
    {
        Debug.Log("LeftHandUIBuilding sending order to queue");

        GameObject buildTarget = GameObject.Find("/World/Units/Builder");
        Order buildorder = new Order("buildunit", buildTarget, false);
        selectedUnits[0].GetComponent<UnitBehaviour>().addOrderToQueue(buildorder);
        Debug.Log("LeftHandUIBuilding sent order to queue");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
