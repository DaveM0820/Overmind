using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechHologram : MonoBehaviour
{
    // Start is called before the first frame update
   // public List<Transform> mechtransforms = new List<Transform>();
   // public List<Transform> hologramtransforms = new List<Transform>();

    public Transform mech;
    public Transform mechRoot;
    public Transform mechHead;

    public Transform hologram;
    public Transform hologramRoot;
    public Transform hologramHead;

    Transform[] mechtransforms;
    Transform[] hologramtransforms;
    float sizeRatio;
    Vector3 refrenceposition;
    void Start()
    {
        sizeRatio = (hologramHead.position.y - hologramRoot.position.y) / (mechHead.position.y - mechRoot.position.y);
 
        mechtransforms = mechRoot.GetComponentsInChildren<Transform>();
        hologramtransforms = hologramRoot.GetComponentsInChildren<Transform>();

            
  
    }

    // Update is called once per frame
    void Update()
    {
        int counter = 0;
        foreach (Transform bodypart in mechtransforms)
        {
            refrenceposition = bodypart.position - mech.position;
            hologramtransforms[counter].position = hologram.position + (refrenceposition * sizeRatio);
            hologramtransforms[counter].rotation = bodypart.rotation;
            counter++;
        }
        Debug.Log("bodyparts modified " + counter);

    }
}
