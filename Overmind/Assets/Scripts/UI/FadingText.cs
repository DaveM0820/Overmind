using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingText : MonoBehaviour
{
    float timeElapsed;
    private GameObject player;
    // Start is called before the first frame update
    void Start() {
        timeElapsed = 0;
        player = GameObject.Find("/XR Rig");
        transform.LookAt(player.transform);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,0.01f,0);
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 3)
        {
            Destroy(gameObject);

        }

    }
}
