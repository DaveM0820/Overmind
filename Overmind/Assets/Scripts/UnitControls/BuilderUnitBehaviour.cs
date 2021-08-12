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
    private Animator animator;
    private bool isBuilding;
    private bool isMoving;
    private bool angleSet;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("/XR Rig");
        builderBeam = transform.Find("BuilderBeam");
        updateTimestep = player.GetComponent<GlobalGameInformation>().updateTimestep;
        currentlyBuilding = false;
        builderBeam.transform.localScale = new Vector3(0,0,0);
        builderBeam.gameObject.SetActive(false);
        animator = transform.Find("LOD0").gameObject.GetComponent<Animator>();
        isBuilding = false;
        isMoving = false;
        angleSet = false;
    }
    public void Build(GameObject building)
        {
        if (isBuilding == false)
        {
            builderBeam.gameObject.SetActive(true);
            isBuilding = true;
            isMoving = false;

            Vector3 unitAngle = (building.transform.position - transform.position).normalized;
            transform.forward = unitAngle;
            animator.SetBool("Building", true);

        }

        Debug.Log("public void Build running");

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
                Vector3 beamHit = building.transform.position + new Vector3(Random.Range(0, 30), Random.Range(0, scaffoldHeight), Random.Range(0, 30)) + new Vector3(-15, 0,-15);
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
        builderBeam.gameObject.SetActive(false);
        animator.SetBool("Building", false);
        isBuilding = false;

    }
    public void Move(Vector3 movetarget)
    {
        if (isMoving == false)
        {
            builderBeam.gameObject.SetActive(false);
            isMoving = true;
            isBuilding = false;
            animator.SetBool("Moving", true);
        }
        Vector3 moveDir = (movetarget - transform.position).normalized;
        if (Vector3.Distance(movetarget, transform.position) < 1f) // if within 1m of destination
        {
            animator.SetBool("Moving", false);
            isMoving = false;
            gameObject.GetComponent<UnitBehaviour>().OrderComplete();

        }
        else
        {
            Vector3 moveDistance = moveDir * gameObject.GetComponent<UnitBehaviour>().moveSpeed * updateTimestep;
            transform.forward = moveDir;
            moveDistance = new Vector3(moveDistance.x, 0, moveDistance.z);
            transform.position += moveDistance;
        }
    }
    // Update is called once per frame

}
