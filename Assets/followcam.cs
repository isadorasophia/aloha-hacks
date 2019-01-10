using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followcam : MonoBehaviour
{
    GameObject mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCam.transform.position + 3 *  mainCam.transform.forward;
    }
}