using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class ColliderObject : MonoBehaviour
{
    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float m_fMinimumExtent;
    private float m_fPartialExtent;
    private float m_fSqrMinExtents;
    private Vector3 m_pPreviousPos;
    private Collider m_pCollider;

    //initialize values 
    void Start()
    {
        m_pCollider = GetComponent<Collider>();
        if( m_pCollider is SphereCollider )
        {
            SphereCollider sc = m_pCollider as SphereCollider;
            m_fMinimumExtent = sc.radius * 2.0f;
        }
        else
        {
            m_fMinimumExtent = Mathf.Min(Mathf.Min(m_pCollider.bounds.extents.x, m_pCollider.bounds.extents.y), m_pCollider.bounds.extents.z);
        }
        m_pPreviousPos = transform.position;
        m_fPartialExtent = m_fMinimumExtent * (1.0f - skinWidth);
        m_fSqrMinExtents = m_fMinimumExtent * m_fMinimumExtent;
        
        if( m_fMinimumExtent == 0 )
        {
            Debug.Assert(m_fMinimumExtent != 0);
        }
    }

    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = transform.position - m_pPreviousPos;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > m_fSqrMinExtents)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(m_pPreviousPos, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", m_pCollider);
                else
                {
                    CustomCollision c = new CustomCollision();
                    c.contact_point = hitInfo.point;
                    c.gameObject = this.gameObject;
                    hitInfo.collider.SendMessage("OnCollisionEnter2", c);
                }

            }
        }

        m_pPreviousPos = transform.position;
    }
}
