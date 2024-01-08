using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosition : MonoBehaviour {

    private Vector3 movePosition;
    private float velocity;
    bool moving;
    [SerializeField] private float turnspeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;


    private void Start()
    {
        movePosition = transform.position;
        moving = false;
    }
    public void SetMovePosition(Vector3 movePosition)
    {
        this.movePosition = movePosition;
        //float targetAgnle = Mathf.Atan2()
        //float angleTarget = Vector3.Angle(transform.position, movePosition);

       // transform.rotation =  Quaternion.Euler(0, angle, 0);
        moving = true;
    }

    private void Update()
    {
        if (moving == true) { 
        Vector3 moveDir = (movePosition - transform.position).normalized;
        if (Vector3.Distance(movePosition, transform.position) < 1f) // if within 1m of destination
        {
            moveDir = Vector3.zero; //stahp
            moving = false;
        }
        else
        { 
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        }
    }

}
