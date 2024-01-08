using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCollision : MonoBehaviour
{
    UnitBehaviour unitBehaviour;

    private void Start() {
        unitBehaviour = GetComponent<UnitBehaviour>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collider) {
        unitBehaviour.CollisionEnter(collider);
    }
    private void OnTriggerExit(Collider collision) {
        unitBehaviour.CollisionExit(collision);
    }
    private void OnTriggerStay(Collider collision) {
        unitBehaviour.CollisionStay(collision);
    }

}
