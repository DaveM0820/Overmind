using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuilderUnitBehaviour : MonoBehaviour
{
    public bool currentlyBuilding;
    private GameObject building;
    GameObject player;

    private float reloadProgress;
    public float buildRate;
    public float buildStrength;
    private float updateTimestep;
    private Transform builderBeam;
    // Start is called before the first frame update
    private void Start() {
        player = GameObject.Find("/XR Rig");
        builderBeam = transform.Find("BuilderBeam");
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        currentlyBuilding = false;
        builderBeam.transform.localScale = new Vector3(0,0,0);
    }
        public void Build(GameObject building)
        {
        Debug.Log("public void Build running");

        currentlyBuilding = true;
        reloadProgress += updateTimestep;
        if (reloadProgress >= buildRate)
        {
            Debug.Log("(reloadProgress >= buildRate)");
            if (building.GetComponent<BuildingBehaviour>().built == false)
            {
                Debug.Log("building.GetComponent<BuildingBehaviour>().built == false");
                //fire the assist gun
                //generate new random point on building
                float scaffoldHeight = building.GetComponent<BuildingBehaviour>().scaffoldHeight;
                Vector3 beamHit = building.transform.position + new Vector3(Random.Range(0, 10), Random.Range(0, scaffoldHeight), Random.Range(0, 10)) + new Vector3(-5, 0, -5);
                builderBeam.LookAt(beamHit,Vector3.up);
                float beamLength = Vector3.Distance(transform.position, beamHit);
                builderBeam.localScale = new Vector3(1, 1, beamLength);
                building.GetComponent<UnitBehaviour>().hp += buildStrength;
                reloadProgress = 0;
            }
            else
            {
                stopBuilding();
                gameObject.GetComponent<UnitBehaviour>().OrderComplete();
            }
            
        }

    }
    public void stopBuilding()
    {
        builderBeam.transform.localScale = new Vector3(0, 0, 0);
        currentlyBuilding = false;


    }

    // Update is called once per frame

}
