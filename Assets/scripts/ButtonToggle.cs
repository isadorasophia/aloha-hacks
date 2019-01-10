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

    void OnCollisionEnter(Collider col)
    {
        if(col.gameObject.layer == 8)
        {
            text.color = new Vector4(0f, 0f, 0f, 255f);
            GetComponent<Image>().color = new Vector4(255f, 255f, 255f, 255f);
        }
    }

    void OnCollisionExit(Collider col)
    {
        if (col.gameObject.layer == 8)
        {
            text.color = new Vector4(100f, 178f, 188f, 255f);
            GetComponent<Image>().color = new Vector4(220f, 110f, 15f, 255f);
            valid = !valid;
        }
    }
}
