using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairScript : MonoBehaviour {

    Time m_StartTime;
    Vector3 m_pStartPos;

	// Use this for initialization
	void Start () {
        m_pStartPos = transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
        float fNow = Time.time;
        int nNow = (int)(fNow * 1000) % 1000;
        if( nNow < 750 )
        {
            transform.position = m_pStartPos + new Vector3(0, -0.05f * (nNow / 750.0f), 0);
        }
        else
        {
            transform.position = m_pStartPos + new Vector3(0, -0.05f * (( nNow - 750.0f ) / 250.0f ), 0);
        }
	}
}
