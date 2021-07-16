using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrderIndicator : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
       // count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = transform.localScale * 0.01f;
    }
}
