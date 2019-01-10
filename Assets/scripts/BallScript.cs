using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

    public AudioClip m_pBallInSound;
    public AudioClip m_pBallOutSound;
    public AudioClip m_pBallSpawnedSound;
    public AudioClip m_pBouncedOffHandSound;
    public AudioClip m_pBallDieSound;
    public AudioClip m_pCatchBallSound;
    public AudioClip m_pCashRegisterSound;
    public GameObject m_pMarker;
    public GameObject m_pConfetti;
    public Material m_pNormalMaterial;
    public Material m_pColliderDisabledMaterial;
    public Material [] BallSkins;
    public bool m_valid;

    bool m_bJustReleased;
    float m_fStartTime;
    float m_fLastCollisionTime;
    float m_fReleaseStartTime;
    float m_fLastTouchedHandTime;
    Rigidbody m_pRigidBody;
    Collider m_pCollider;
    HandScript m_pLeftHand;
    HandScript m_pRightHand;
    HandScript m_pHoldingHand;
    bool m_bStartedDeath;
    bool m_bPlayedHitSound;
    bool m_bColliderOn;


    void _CheckLoadInitStuff( )
    {
        m_pRigidBody = GetComponent<Rigidbody>();
        m_pCollider = GetComponent<Collider>();

        EnsureColliderOff();
    }

    // Use this for initialization
    void Start ()
    {
        _CheckLoadInitStuff();
        m_fStartTime = Time.time;
        m_fLastTouchedHandTime = m_fStartTime;
        m_bColliderOn = true;

        if( BallSkins != null )
        {
            if( BallSkins.Length > 0 )
            {
                int SkinCount = BallSkins.Length;
                int RandomSkinIndex = (int) Random.Range( 0, SkinCount );
                Renderer pRenderer = GetComponent<Renderer>();
                pRenderer.material = BallSkins [RandomSkinIndex];
            }
        }
    }
	
    void StartDeath( )
    {
        if(m_bStartedDeath )
        {
            return;
        }

        m_bStartedDeath = true;

        _PlayAudioClip( m_pBallDieSound );

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (m_pHoldingHand)
        {
            m_pHoldingHand.ReleaseBall();
            m_pHoldingHand = null;
        }

        Destroy(gameObject, 1.0f);

        Object pConfetti = Instantiate(m_pConfetti, transform.position, Quaternion.identity);

        Destroy(pConfetti, 4.0f);

    }

    // Update is called once per frame
    void Update ()
    {
        float fNow = Time.time;

        if( fNow - m_fLastTouchedHandTime > 1000 )
        {
            StartDeath();
            return;
        }

        if( fNow - m_fStartTime > 10.0f )
        {
            StartDeath();
        }

        if( transform.position.y < -5.0f ) // world position, hit the floor
        {
            StartDeath();
            return;
        }

        if (m_bJustReleased )
        {
            // after a short time, re-enable the collider
            //
            if( fNow - m_fReleaseStartTime > 0.25f )
            {
                EnsureColliderOn( );
                m_bJustReleased = false;
            }

            return;
        }

        if( m_pLeftHand == null )
        {
            m_pLeftHand = HandScript.LeftHand();
        }
        if( m_pRightHand == null )
        {
            m_pRightHand = HandScript.RightHand();
        }

        // we do the test in the ball script because there are many balls, but only 2 hands

        if (m_pHoldingHand != null)
        {
            m_fLastTouchedHandTime = fNow;

            // this ball is being held. If the user maintains a grip on the index button, we'll follow the hand.
            // if the user releases the grip on the index button, we'll release the ball w/ the right velocity and 
            // disable the collider for a bit

            float fTrigger = m_pHoldingHand.GetLastTriggerAmount();
            if (fTrigger > 0.25f)
            {
                // move the ball along with the hand, at the spawn point

                Vector3 pSpawnPoint = m_pHoldingHand.GetSpawnPoint();
                transform.position = pSpawnPoint;
            }
            else
            {
                Vector3 pVelocity = m_pHoldingHand.GetPalmVelocityDuringBallThrow();

                // was being held, but now released. We need to figure out the velocity of the PALM
                // set the rigidbody velocity to that, and turn off the collider for a bit
                OnReleasedFromHolding();

                m_pRigidBody.velocity = pVelocity;

                Debug.Log("Release Velocity x= " + pVelocity.x + ", y=" + pVelocity.y + ", z=" + pVelocity.z);

                _PlayAudioClip( m_pBallOutSound );
            }
        }
        else
        {
            // not currently being held

            if( m_pLeftHand != null )
            {
                if( !m_pLeftHand.IsHoldingBall( ) )
                {
                    float fTrigger = m_pLeftHand.GetLastTriggerAmount();

                    // find the distance to the left hand
                    //
                    float fDistanceToHand_L = Distance(m_pLeftHand.GetSpawnPoint());

                    if (fDistanceToHand_L < 0.05f)
                    {
                        if (fTrigger > 0.25f)
                        {
                            // it's close enough to start holding it
                            OnStartHolding(m_pLeftHand);

                            _PlayAudioClip( m_pBallInSound );
                        }
                        else
                        {
                            EnsureColliderOn();
                        }
                    }
                    else if (fDistanceToHand_L < 0.3f) // 1 foot?
                    {
                        if (fTrigger > 0.25f)
                        {
                            EnsureColliderOff();

                            // tractor beam
                            Vector3 pDraw = m_pLeftHand.GetSpawnPoint() - transform.position;
                            pDraw /= 10.0f; // move by 1/10th per frame
                                            // move that direction
                            m_pRigidBody.AddForce(pDraw);
                        }
                        else
                        {
                            EnsureColliderOn();
                        }
                    }
                    else
                    {
                        // got too far, turn the collider on
                        EnsureColliderOn();
                    }
                }
                else
                {
                    EnsureColliderOn();
                }
            }

            if (m_pRightHand != null)
            {
                if( !m_pRightHand.IsHoldingBall( ) )
                {
                    float fTrigger = m_pRightHand.GetLastTriggerAmount();

                    // find the distance to the Right hand
                    //
                    float fDistanceToHand = Distance(m_pRightHand.GetSpawnPoint());

                    if (fDistanceToHand < 0.05f)
                    {
                        if (fTrigger > 0.25f)
                        {
                            // it's close enough to start holding it
                            OnStartHolding(m_pRightHand);

                            _PlayAudioClip( m_pBallInSound );
                        }
                        else
                        {
                            EnsureColliderOn();
                        }
                    }
                    else if (fDistanceToHand < 0.3f) // 1 foot?
                    {
                        if (fTrigger > 0.25f)
                        {
                            EnsureColliderOff();

                            // tractor beam
                            Vector3 pDraw = m_pRightHand.GetSpawnPoint() - transform.position;
                            //    pDraw /= 10.0f; // move by 1/10th per frame
                            // move that direction
                            m_pRigidBody.AddForce(pDraw);
                        }
                        else
                        {
                            EnsureColliderOn();
                        }
                    }
                    else
                    {
                        // got too far, turn the collider on
                        EnsureColliderOn();
                    }
                }
                else
                {
                    EnsureColliderOn();
                }
            }

            // if it's in water, make it bob
            if( m_bInWater )
            {
                m_fBobAngle += 0.1f;
                Rigidbody rb = GetComponent<Rigidbody>();
                transform.position += new Vector3( 0, 0.01f * ( 4.0f / 36.0f ) * Mathf.Sin( m_fBobAngle ), 0 );
            }

            // if the ball is heading too far away from the player, adjust it with some force to stay "on a cylinder" 
            if (m_valid)
            {
                _AdjustBallToBeInCylinderPlane();
            }

        } // not held

    }

    void _AdjustBallToBeInCylinderPlane()
    {
        Vector3 pPlayerPos = Player.Instance.transform.position;
        float fDistanceFromPlayer = Vector3.Distance( transform.position, pPlayerPos );
        // if the distance is too far...
        float fCylinderShellDistance = 0.466f; // about 2 feet
        if( fDistanceFromPlayer > fCylinderShellDistance )
        {
            // how far are we from the cylindrical shell? this doesn't depend on Y
            Vector2 pBallXZ = new Vector2( transform.position.x, transform.position.z );
            Vector3 pPlayerXZ = new Vector2( pPlayerPos.x, pPlayerPos.z );
            // the XY distance
            float fDistanceFromPlayerXZ = Vector2.Distance( pBallXZ, pPlayerXZ );
            float fDistanceFromCylinder = Mathf.Abs( fDistanceFromPlayerXZ - fCylinderShellDistance );

            // fashion a force to move the ball back towards the player
            Vector3 pTowardsPlayer = new Vector3( pPlayerPos.x - transform.position.x, 0, pPlayerPos.z - transform.position.z );

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce( pTowardsPlayer * fDistanceFromCylinder * 100.0f );
        }

    }

    public void OnStartHolding( HandScript pHand )
    {
        EnsureColliderOff();

        m_pRigidBody.Sleep();
        m_pHoldingHand = pHand;
        m_bJustReleased = false;
        pHand.StartHoldingBall( gameObject );
    }

    public void OnReleasedFromHolding( )
    {
        // don't enable collider just yet. after timer expires, sure
        m_bJustReleased = true;
        m_fReleaseStartTime = Time.time;
        m_pRigidBody.WakeUp();

        if( m_pHoldingHand != null )
        {
            m_pHoldingHand.ReleaseBall();
            m_pHoldingHand = null;
        }
    }

    float Distance( Vector3 t )
    {
        return Vector3.Distance(transform.position, t);
    }

    bool m_bInWater = false;
    float m_fBobAngle = 0;

    void _EnterWater( )
    {
        m_bInWater = true;
    }

    void _LeaveWater( )
    {
        m_bInWater = false;
    }

    void OnCollisionEnter(Collision c)
    {
        m_fLastCollisionTime = Time.time;

        if( c.transform.tag == "water" )
        {
            // the ball hit water. add a sine wave until it hits something NOT water
            _EnterWater();
        }
        else
        {
            _LeaveWater();
        }

        if( c.gameObject.layer == 8 )
        {
            m_fLastTouchedHandTime = Time.time;

            if( !m_bPlayedHitSound )
            {
                m_bPlayedHitSound = true;
                _PlayAudioClip( m_pCatchBallSound );
            }
        }
        else if( c.gameObject.tag == "Floor" )
        {
            StartDeath();
            return;
        }
        else if( c.gameObject.tag == "Crate_Small" )
        {
            _PlayAudioClip( m_pCashRegisterSound );
            StartDeath();
            return;
        }
        else
        {
            if( !m_bPlayedHitSound )
            {
                m_bPlayedHitSound = true;
                GameObject pObject = c.gameObject;
                _PlayAudioClip( m_pBouncedOffHandSound );
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        m_bPlayedHitSound = false;
    }

    void EnsureColliderOn( )
    {
        if(m_bColliderOn)
        {
            return;
        }

        m_bColliderOn = true;
        Renderer pRenderer = GetComponent<Renderer>();
        pRenderer.material = m_pNormalMaterial;
    }

    void EnsureColliderOff( )
    {
        if( !m_bColliderOn)
        {
            return;
        }

        m_bColliderOn = false;
        Renderer pRenderer = GetComponent<Renderer>();
        pRenderer.material = m_pColliderDisabledMaterial;
    }

    public void SpawnBall( HandScript pHand, bool bIsStartedWhileHeld )
    {
        // ball should spawn right in our palm and be held
        _CheckLoadInitStuff();

        if( bIsStartedWhileHeld )
        {
            OnStartHolding( pHand );
        }
        else
        {
            OnReleasedFromHolding();
        }

        _PlayAudioClip( m_pBallSpawnedSound );
    }

    void _PlayAudioClip( AudioClip pClip )
    {
        AudioSource pSource = GetComponent<AudioSource>();
        pSource.PlayOneShot( pClip );
    }
}
