using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToOvermind : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject player;

    public Button returnToOvermindButton;


    // now you have a reference pointer to the camera, modify properties :


    void Start()
    {
        returnToOvermindButton.onClick.AddListener(OverMindViewInitalize);
        player = GameObject.Find("/XR Rig");
    }

    void OverMindViewInitalize()
    {
        player.GetComponent<CameraControls>().OverMindViewInitalize();

    }
    // Update is called once per frame



}
