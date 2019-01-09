using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour {

    public GameObject m_pBallPrefab;
    float m_fLastSpawnTime;

	// Use this for initialization
	void Start () {
        m_fLastSpawnTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
        float fNow = Time.time;
        if( fNow - m_fLastSpawnTime > 1.0f )
        {
            m_fLastSpawnTime = fNow;
            Instantiate(m_pBallPrefab, Camera.main.transform.position + new Vector3(0, 0.27f, 0.4f), Quaternion.identity);
        }
		
	}
}
