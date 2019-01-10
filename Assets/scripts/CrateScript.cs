using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    // Start is called before the first frame update
    float m_fAngle = 0;
    Vector3 m_fOriginalPosition;

    void Start()
    {
        m_fOriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float fOffset = 0.1f * Mathf.Sin( m_fAngle );
        m_fAngle += 0.01f;
        transform.position = m_fOriginalPosition + new Vector3( 0, fOffset, 0 );
    }
}
