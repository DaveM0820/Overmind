using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    private bool selected;
    private Vector3 selectionMarkerScale;
    Transform selectionMarker;
    [SerializeField] private int healthPointsMax;
    private int currenthealthpoints;
    public string unitType;
    public float unitSize;


    // private GameObject selectionMarker;
    // Start is called before the first frame update
    void Start()
    {
        //selectionMarker = GameObject.Find("SelectionMarker").transform.localScale = new Vector3(0, 0, 0);
        selectionMarker = transform.Find("SelectionMarker");
        selectionMarkerScale = selectionMarker.localScale;
        Deselect();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Select()
    {
        selected = true;
        selectionMarker.localScale = selectionMarkerScale;
    }
    public void Deselect()
    {
        selected = false;
        selectionMarker.localScale = new Vector3(0, 0, 0) ;
    }
}


