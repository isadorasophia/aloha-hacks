using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public Text text;
    public bool valid;
    // Start is called before the first frame update
    void Start()
    {
        valid = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("I'm cry if toggle works");
        //layer 8 is hand
        if (col.gameObject.layer == 8)
        {
            valid = !valid;
            if (valid)
            {
                text.color = new Vector4(45, 250f, 140f, 255f);
                GetComponent<Image>().color = new Vector4(255f, 255f, 255f, 255f);
            }
            else
            {
                text.color = new Vector4(250, 140f, 45f, 255f);
                GetComponent<Image>().color = new Vector4(150f, 228f, 238f, 255f);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("omg pls leave the button");
        //layer 8 is hand
            if(valid)
            {
                text.color = new Vector4(15, 220f, 110f, 255f);
                GetComponent<Image>().color = new Vector4(205f, 205f, 205f, 255f);
            }
            else
            {
                text.color = new Vector4(220, 110f, 15f, 255f);
                GetComponent<Image>().color = new Vector4(100f, 178f, 188f, 255f);
            }
    }
}
