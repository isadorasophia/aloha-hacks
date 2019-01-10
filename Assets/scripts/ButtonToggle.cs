using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 8)
        {
            text.color = new Vector4(140f, 218f, 228f, 255f);
            GetComponent<Image>().color = new Vector4(250f, 148f, 45f, 255f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 8)
        {
            text.color = new Vector4(100f, 178f, 188f, 255f);
            GetComponent<Image>().color = new Vector4(220f, 110f, 15f, 255f);
        }
    }
}
