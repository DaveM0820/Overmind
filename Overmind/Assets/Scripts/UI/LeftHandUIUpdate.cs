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
    public void UpdateOreDisplay() {
        Debug.Log("player resources:");
        Debug.Log(player.GetComponent<GlobalGameInformation>().playerResources[playernum]);
        Debug.Log("player resource rate:");
        Debug.Log(player.GetComponent<GlobalGameInformation>().resourcesPerMin[playernum]);
        oreDisplayText.text = "Ore: " + player.GetComponent<GlobalGameInformation>().playerResources[playernum] + "(" + player.GetComponent<GlobalGameInformation>().resourcesPerMin[playernum] + "/min)";


    }
    void Start()
    {
        oreDisplayText = oreDisplay.GetComponent<Text>();
        player = GameObject.Find("/XR Rig");
        playernum = player.GetComponent<GlobalGameInformation>().player;
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
