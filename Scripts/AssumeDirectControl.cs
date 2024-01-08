using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class AssumeDirectControl : MonoBehaviour
{
    // Start is called before the first frame update


    private GameObject player;
    public Button assumeDirectControlButton;

    void Start()
    {
        assumeDirectControlButton.onClick.AddListener(AssumeDirectControlInitalize);
        player = GameObject.Find("/XR Rig");
    }

    void AssumeDirectControlInitalize()
    {
        player.GetComponent<CameraControls>().AssumeDirectControlInitalize();
    }
    // Update is called once per frame
 



}
