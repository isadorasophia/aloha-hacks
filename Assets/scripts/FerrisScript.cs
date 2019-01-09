using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisScript : MonoBehaviour {

    float m_fAngle;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_fAngle));
        m_fAngle += 0.10f;
        int nChildren = transform.childCount;
        for( int i = 0; i < nChildren; i++ )
        {
            Transform pBucket = transform.GetChild(i);
            BucketScript bs = pBucket.gameObject.GetComponent<BucketScript>();
            bs.m_fAngle += 0.10f;
            pBucket.rotation = Quaternion.Euler(new Vector3(0, 0, bs.m_fAngle * 1.0f + 45.0f ));
        }
	}
}
