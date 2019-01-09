using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineappleScript : MonoBehaviour
{
    public GameObject m_pBallPrefab;
    float m_fLastSpawnTime = 0;
    public GameObject m_pExplosion;
    public AudioClip m_pExplosionSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( new Vector3( 0, 0.03f, 0 ) );
        float fNow = Time.time;
        if( fNow - m_fLastSpawnTime > 5.0f )
        {
            m_fLastSpawnTime = fNow;
            GameObject pNewBall = (GameObject) Instantiate( m_pBallPrefab, transform.position + new Vector3( 0, 1.0f, 0 ), Quaternion.identity);
            Rigidbody rb = pNewBall.GetComponent<Rigidbody>();
            rb.velocity = new Vector3( Random.Range( -1.0f, 1.0f ), Random.Range( 1.0f, 10.0f ), Random.Range( -1, 1.0f ) );
            GameObject pExplosion = (GameObject) Instantiate( m_pExplosion );
            AudioSource pSource = GetComponent<AudioSource>();
            pSource.PlayOneShot( m_pExplosionSound );
        }
    }
}
