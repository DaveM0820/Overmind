using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameInformation : MonoBehaviour
{
    // Start is called before the first frame update
    public float updateFPS;
    public float updateTimestep;
    public float buildingUpdateFPS;
    public float buildingUpdateTimestep;

    public int numberOfUnitsBuilt;
    public int numberOfPlayers;
    public List<int> playerResources;
    public List<int> resourcesPerMin;
    public List<int> playerScore;
    public List<Material> factionMaterial;
    public List<Color32> factionColor;
    public List<Unit>[] unitList = new List<Unit>[8];
    public int startingResources;
    public int player;
    public int checkRangeCounterMax;
    public List<Material> unitSymbolTypes;
    public List<Material>[] unitSymbols = new List<Material>[8];

    void Awake()
    {
        int j = 0;
        for (int i = 0; i < numberOfPlayers; i++)

        {
            unitList[i] = new List<Unit>();

        }
        foreach (Material material in unitSymbolTypes)
        {

            unitSymbols[j] = new List<Material>();
            for (int i = 0; i < numberOfPlayers; i++)

            {

                Material newSymbol = Instantiate(material);
                //newSymbol.color = factionColor[i];
                //newSymbol.SetColor() = factionColor[i];
                newSymbol.SetColor(0, factionColor[i]);
                newSymbol.SetColor(1, factionColor[i]);
                newSymbol.SetColor("_Color", factionColor[i]);

                newSymbol.SetColor("_BaseColor", factionColor[i]);

                unitSymbols[j].Add(newSymbol);
            }

            j++;

        }
        for (int i = 0; i < 8; i++)

        {
        unitList[i] = new List<Unit>();
        
        }
        updateTimestep = 0.0416f;
        buildingUpdateTimestep = 1 / buildingUpdateFPS;
        numberOfUnitsBuilt = 0;
         int count = 0;
        while (count < numberOfPlayers)
        {
            playerResources.Add(startingResources);
            resourcesPerMin.Add(0);

            playerScore.Add(0);

            count++;
        }
    }

}
public class Unit
{
    public GameObject gameObject;
    public int faction;
    public bool military;
    public Vector3 position;
    public UnitBehaviour unitBehaviour;

    public Unit(int faction, GameObject gameObject, bool military) {
        this.gameObject = gameObject;
        this.faction = faction;
        this.military = military;
        this.unitBehaviour = gameObject.GetComponent<UnitBehaviour>();
     }

}