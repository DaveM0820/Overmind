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
    public List<Unit>[] unitList = new List<Unit>[8];
    public int startingResources;
    public int player;
    public int checkRangeCounterMax;
    void Awake()
    {
        for (int i = 0; i < 8; i++)

        {
        unitList[i] = new List<Unit>();
        
        }
        updateTimestep = 1 / updateFPS;
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

    public Unit(int faction, GameObject gameObject, bool military, Vector3 position) {
        this.gameObject = gameObject;
        this.faction = faction;
        this.military = military;
        this.position = position;

     }

}