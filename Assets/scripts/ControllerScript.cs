using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerScript : MonoBehaviour {

    public GameObject m_pBucket;
    public GameObject m_pFerrisWheel;
    public GameObject m_pRoomAndWalls;

    // Use this for initialization
    void Start ()
    {
        m_pRoomAndWalls.SetActive(true);
        
        System.Random r = new System.Random();

        for( int i = 0; i < 32; i++ )
        {
            float degrees = 360.0f * i / 32.0f;

            Vector3 pNewPos = Quaternion.Euler(new Vector3(0, 0, degrees)) * new Vector3(3.2f, 0, 0);

            Quaternion pNewRot = Quaternion.Euler(new Vector3(0, 0, degrees));
            GameObject pNewBucket = Instantiate(m_pBucket, pNewPos, pNewRot, m_pFerrisWheel.transform);
            pNewBucket.transform.position = pNewPos; //  UnityEngine.Random.onUnitSphere * (float) ( r.NextDouble() * 100.0f );
            BucketScript bs = pNewBucket.GetComponent<BucketScript>();
            bs.m_fAngle = degrees;
        }

        m_pFerrisWheel.transform.position = Camera.main.transform.position;
    }
}
