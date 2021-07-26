using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TopDisplay : MonoBehaviour
{
    public Text display;
    private GameObject player;
    private int playerNumber;
    private int resources;
    private int score;
    float elapsed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("/XR Rig");
        playerNumber = player.GetComponent<GlobalGameInformation>().player;
        resources = player.GetComponent<GlobalGameInformation>().playerResources[playerNumber];
        score = player.GetComponent<GlobalGameInformation>().playerScore[playerNumber];
        InvokeRepeating("UpdateTopDisplay", 0, 2f);
    }
    private void UpdateTopDisplay()
    {
        display.text = "Metal : " + resources + "  Score: " + playerNumber;

    }
    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= 0.50f)
        {
            elapsed = elapsed % 0.50f;
            UpdateTopDisplay();
        }

    }
}
