using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeftHandUIUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject oreDisplay;
    Text oreDisplayText;

    public GameObject player;
    float counter = 0;
    int playernum;
    GlobalGameInformation globalGameInformation;
    public void UpdateOreDisplay() {

        oreDisplayText.text = "Ore: " + globalGameInformation.playerResources[playernum] + "(" + globalGameInformation.resourcesPerMin[playernum] + "/min)";


    }
    void Start()
    {
        oreDisplayText = oreDisplay.GetComponent<Text>();
        player = GameObject.Find("/XR Rig");
        globalGameInformation = player.GetComponent<GlobalGameInformation>();
        playernum = globalGameInformation.player;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > 1)
        {
            UpdateOreDisplay();
            counter = 0; 
        }
    }
}
