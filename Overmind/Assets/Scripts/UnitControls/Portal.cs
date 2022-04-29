using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float[] countdown;
    public GameObject[] unitPrefabs;
    public GameObject[] objectsToSpawn;

    public int[,] objectsToSpawnPerWave = new int[50, 5];//wave, object number, value is how many to spawn
        int currentWave = 0;
    float timerToNewWave;
    float timerToNextUnit = 0;
    bool allUnitsSpawned = true;
    public float secondsPerSpawn;
    int unitPrefabCounter;
    int unitToSpawnCounter;
    public ParticleSystem spawnExplosion;
    GlobalGameInformation globalGameInformation;
    bool active;
    // Start is called before the first frame update
    void Start() {
        objectsToSpawnPerWave[0, 0] = 5;
        objectsToSpawnPerWave[1, 0] = 6;
        objectsToSpawnPerWave[2, 0] = 7;
        globalGameInformation = GameObject.Find("/XR Rig").GetComponent<GlobalGameInformation>();
        timerToNewWave = countdown[currentWave];
    }

    // Update is called once per frame
    void Update() {
        if (active)
        {

        timerToNewWave -= Time.deltaTime;
        if (timerToNewWave < 0)
        {
            // Debug.Log("Wave " + currentWave);
            //Debug.Log("objects to spawn " + objectsToSpawnPerWave[currentWave, 0]);
           /* for (int i = 0; i < unitPrefabs.Length; i++)
            {
                for (int j = 0; j < objectsToSpawnPerWave[currentWave,i]; j++)
                {
                    //GameObject newSpawn = Instantiate(unitPrefabs[i]);
                   // newSpawn.transform.position = transform.position + new Vector3(i+j, i + j, i + j);

                }

            }*/

            currentWave++;
            allUnitsSpawned = false;
            unitPrefabCounter = 0;
            unitToSpawnCounter = objectsToSpawnPerWave[currentWave, 0];
            timerToNewWave = countdown[currentWave];
        }
        if (allUnitsSpawned == false)
        {
            timerToNextUnit -= Time.deltaTime;
            if (timerToNextUnit < 0)
            {
                timerToNextUnit = secondsPerSpawn;

                spawnExplosion.Play();

                GameObject newSpawn = Instantiate(unitPrefabs[unitPrefabCounter]);
                newSpawn.transform.position = transform.position;
                int unitToTarget = Random.Range(0, globalGameInformation.unitList[0].Count);
                GameObject target = globalGameInformation.unitList[0][unitToTarget].gameObject;
                newSpawn.GetComponent<UnitBehaviour>().addOrderToQueue(new Order("attack", new Vector3(0, 0, 0)));
                unitToSpawnCounter--;
                if (unitToSpawnCounter == 0)
                {
                    unitPrefabCounter++;
                    unitToSpawnCounter = objectsToSpawnPerWave[currentWave, unitPrefabCounter];
                    
                }
                if (unitPrefabCounter > unitPrefabs.Length)
                {
                    allUnitsSpawned = true;
                }
            }

        }
        }
    }
}