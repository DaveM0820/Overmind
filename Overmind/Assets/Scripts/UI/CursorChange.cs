using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour
{
    Transform move;
    Transform assist;
    Transform attack;
    GameObject player;
    List<Collider> colliderList;
    string target;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("/XR Rig");
        move = transform.Find("Move");
        assist = transform.Find("assist");
        attack = transform.Find("attack");
        target = "move";
        colliderList = new List<Collider>();
    }
    public void ChangeCursorColourMove()
    {
        move.gameObject.SetActive(true);
        assist.gameObject.SetActive(false);
        attack.gameObject.SetActive(false);
    }
    public void ChangeCursorColourAssist()
    {
        move.gameObject.SetActive(false);
        assist.gameObject.SetActive(true);
        attack.gameObject.SetActive(false);
    }
    public void ChangeCursorColourAttack()
    {
        move.gameObject.SetActive(false);
        assist.gameObject.SetActive(false);
        attack.gameObject.SetActive(true);
    }
    // Update is called once per frame

    private void OnTriggerEnter(Collider collider)   
    {
        Debug.Log("Collision with cursor detected");
        if (!colliderList.Contains(collider))
        {
            colliderList.Add(collider);
        }
        if (player.GetComponent<Select>().allBuilder == true)
        {

            if (collider.gameObject.transform.parent.name == "Resources")
            {
                target = "assist";

            }
            else if (collider.gameObject.transform.parent.name == "Buildings")
            {
                if (collider.gameObject.GetComponent<UnitBehaviour>().owner == player.GetComponent<GlobalGameInformation>().player)
                {
                    target = "assist";
                }
            }

            else if (collider.gameObject.transform.parent.name == "Units")
            {
                if (collider.gameObject.GetComponent<UnitBehaviour>().unitType == "builder")
                {
                    target = "assist";
                }
            }

        }
        if (target == "assist")
        {
            ChangeCursorColourAssist();
        }

    }

    //called when something exits the trigger
    private void OnTriggerExit(Collider collider)
    {
        ChangeCursorColourMove();
        //if the object is in the list
        if (colliderList.Contains(collider))
        {
            //remove it from the list
            colliderList.Remove(collider);
        }

    }


}
