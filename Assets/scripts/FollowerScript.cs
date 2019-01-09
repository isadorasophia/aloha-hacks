using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerScript : MonoBehaviour {

    Transform m_pWhoToFollow;
    Rigidbody _rigidbody;
    Vector3 m_pLastPos;
    Vector3 m_pGetLatestVelocity;
    List<Vector3> m_pVelocityList = new List<Vector3>( );

    public void SetWhoToFollow( Transform pWhoToFollow )
    {
        m_pWhoToFollow = pWhoToFollow;

        transform.localScale = pWhoToFollow.localScale;
        transform.position = pWhoToFollow.position;
        transform.rotation = pWhoToFollow.rotation;

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(m_pWhoToFollow.position);
        _rigidbody.MoveRotation(m_pWhoToFollow.rotation);
        Vector3 pCurPos = transform.position;
        m_pGetLatestVelocity = ( pCurPos - m_pLastPos ) / Time.fixedDeltaTime;
        m_pLastPos = pCurPos;
        m_pVelocityList.Add(m_pGetLatestVelocity);
        if( m_pVelocityList.Count > 5 )
        {
            m_pVelocityList.RemoveAt(0);
        }
    }

    public Vector3 GetLatestVelocityForThrowingBall( )
    {
        // take the avg of the last N frames
        int n = 5;
        Vector3 pAvg = new Vector3( );
        for (int i = 0; i < n; i++)
        {
            pAvg += m_pVelocityList [i];
        }
        pAvg /= n;
        pAvg.Normalize( );

        // find the max speed
        float fMaxMagnitude = 0;
        foreach( Vector3 p in m_pVelocityList )
        {
            float fMag = p.magnitude;
            if( fMag > fMaxMagnitude )
            {
                fMaxMagnitude = fMag;
            }
        }

        // return the average angle * the max speed
        //
        return fMaxMagnitude * pAvg;
    }
}
