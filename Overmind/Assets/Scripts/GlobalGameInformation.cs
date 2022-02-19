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
    public int startingResources;
    public int player;
    void Start()
    {
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
