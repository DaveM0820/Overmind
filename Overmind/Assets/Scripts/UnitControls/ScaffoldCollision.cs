using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScaffoldCollision : MonoBehaviour
{
        void OnTriggerStay(Collider collider)
    {
        GameObject builderUnitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuilderUnitScreen");
        GameObject parentBuilding = transform.parent.gameObject;
        bool parentIsPlaced = parentBuilding.GetComponent<BuildingBehaviour>().placed;
        Transform valid = transform.Find("Valid");
        Transform invalid = transform.Find("Invalid");

        if (parentIsPlaced == false) {
            if (collider.transform.parent != null)
            {

                if (collider.transform.parent.name == "Buildings" || collider.transform.parent.parent.gameObject.name == "Buildings")
                {
             

                    if (collider.transform.gameObject.name != transform.parent.gameObject.name)
                    {
                        builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = true;
                    valid.gameObject.SetActive(false);
                    invalid.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            this.GetComponent<ScaffoldCollision>().enabled = false;

        }
    }
    void OnTriggerExit(Collider collider)
    {
        GameObject builderUnitScreen = GameObject.Find("/XR Rig/Camera Offset/LeftHand Controller/LeftHandUI/BuilderUnitScreen");
        GameObject parentBuilding = transform.parent.gameObject;
        Transform valid = transform.Find("Valid");
        Transform invalid = transform.Find("Invalid");
        if (parentBuilding.GetComponent<BuildingBehaviour>().placed == false)
        {
            if (collider.transform.parent != null)
        {
                if (collider.transform.parent.name == "Buildings" || collider.transform.parent.parent.gameObject.name == "Buildings")
                {
                    if (collider.transform.gameObject.name != transform.parent.gameObject.name)
                    {
                        builderUnitScreen.GetComponent<BuildNewBuilding>().buildingCollision = false;
                        valid.gameObject.SetActive(true);
                        invalid.gameObject.SetActive(false);
                    }
                    }
            }
        }
    }
}
