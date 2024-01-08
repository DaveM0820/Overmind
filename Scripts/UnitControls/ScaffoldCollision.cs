using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScaffoldCollision : MonoBehaviour
{
    bool isatresource = false;
    bool buildingcollision = false;
    public int oreExtractorResourceDirection = 0;
    public Vector3 oreLocation;
    Transform valid;
    Transform invalid;
    GameObject builderUnitScreen;
    public GameObject parentBuilding;
    bool isRefinery;
    private void Start() {
        valid = transform.Find("Valid");
        invalid = transform.Find("Invalid");
        isRefinery = false;
        builderUnitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuilderUnitScreen");

        parentBuilding = transform.parent.gameObject;

        if (parentBuilding.GetComponent<BuildingBehaviour>().placed)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerStay(Collider collider) {
        if (!parentBuilding.TryGetComponent<BuildingBehaviour>(out BuildingBehaviour buildingBehaviour))
        {
            Destroy(gameObject);
        }
        if (parentBuilding.GetComponent<BuildingBehaviour>().placed == false)
        {

            if (parentBuilding.GetComponent<UnitBehaviour>().unitType == "Refinery")
            {
                isRefinery = true;
            }
     
                if (isRefinery)
                {
                    if (collider.gameObject.layer == 8)
                    {
                        if (collider.transform.gameObject.name != transform.parent.gameObject.name)
                        {
                            isatresource = true;
                            oreLocation = collider.transform.position;
                            if (collider.transform.position.x > parentBuilding.transform.position.x)
                            {
                                oreExtractorResourceDirection = 1;
                            }
                            else
                            {
                                oreExtractorResourceDirection = -1;
                            }
                        }
                    }
                }
                if (collider.gameObject.layer == 7)
                {


                    if (collider.transform.gameObject.name != transform.parent.gameObject.name)
                    {
                        buildingcollision = true;
                    }
                }

       
            else
            {
                this.GetComponent<ScaffoldCollision>().enabled = false;

            }
            if (isRefinery)
            {
                if (isatresource && buildingcollision == false)
                {
                    builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = false;
                    valid.gameObject.SetActive(true);
                    invalid.gameObject.SetActive(false);

                }
                else
                {
                    builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = true;
                    valid.gameObject.SetActive(false);
                    invalid.gameObject.SetActive(true);

                }

            }
            else if (buildingcollision == true)
            {
                builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = true;
                valid.gameObject.SetActive(false);
                invalid.gameObject.SetActive(true);

            }
        }
    }
    void OnTriggerExit(Collider collider) {
        if (parentBuilding.GetComponent<BuildingBehaviour>().placed == false)
        {

            if (parentBuilding.GetComponent<UnitBehaviour>().unitType == "Refinery")
            {
                isRefinery = true;
            }
     
                if (collider.gameObject.layer == 7)
                {
                    if (collider.transform.gameObject.name != transform.parent.gameObject.name)
                    {
                        buildingcollision = false;
                    }

                }
         
            if (isRefinery)
            {
                if (collider.gameObject.layer == 8)
                {
                    if (collider.transform.gameObject.name != transform.parent.gameObject.name)
                    {
                        isatresource = false;
                    }
                }

            }
            if (isRefinery)
            {
                if (isatresource && buildingcollision == false)
                {
                    builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = false;
                    valid.gameObject.SetActive(true);
                    invalid.gameObject.SetActive(false);

                }
                else
                {
                    builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = true;
                    valid.gameObject.SetActive(false);
                    invalid.gameObject.SetActive(true);

                }

            }
            else if (buildingcollision == false)
            {
                builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = false;
                valid.gameObject.SetActive(true);
                invalid.gameObject.SetActive(false);

            }
        }
    }
}