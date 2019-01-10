#define STEAM_VR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandScript : MonoBehaviour
{
    public Transform m_pPalm;
    public GameObject m_pBallPrefab;
    public Transform GenerationPoint;
    public Material m_pNormalMaterial;
    public PhysicMaterial m_pHandMaterial;

    FollowerScript m_pPalmFollowerScript;

    float m_LastTimeBallSpawned;
    Animator m_pAnimator;
    float m_fLastTriggerAmount = 0.0f;
    string m_szTriggerAxisName;
    string m_szAnimationName;
    string m_szGripAxisName;
    bool m_bIsLeftHand;

    GameObject m_pHeldBall;

    static HandScript s_pLeftHand;
    static HandScript s_pRightHand;

    float m_fStartHold = 0;

    void Start()
    {
        if (name.Contains("Left"))
        {
            m_bIsLeftHand = true;
            m_szTriggerAxisName = "TriggerLeft";
            m_szAnimationName = "Ghost_GripBallState_L" + "";
            m_szGripAxisName = "GripLeft";
            s_pLeftHand = this;
        }
        else if (name.Contains("Right"))
        {
            m_bIsLeftHand = false;
            m_szTriggerAxisName = "TriggerRight";
            m_szAnimationName = "Ghost_GripBallState_R";
            m_szGripAxisName = "GripRight";
            s_pRightHand = this;
        }
        else
        {
            s_pRightHand = this;
            m_bIsLeftHand = false;

            m_szTriggerAxisName = "TriggerRight";
            m_szGripAxisName = "GripRight";
        }

        m_pAnimator = GetComponent<Animator>();

        AddBoneScriptToAllChildren(this.transform);

#if !STEAM_VR
        InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
#endif
    }

    public static HandScript LeftHand( )
    {
        return s_pLeftHand;
    }

    public static HandScript RightHand( )
    {
        return s_pRightHand;
    }

    public Vector3 GetSpawnPoint( )
    {
        return GenerationPoint.position;
    }

#if !STEAM_VR
    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        try
        {
            var st = obj.state;
            var source = obj.state.source;
            var pt = obj.state.sourcePose;
            Vector3 pos;
            Quaternion rot;

            // follow the motion controller, and animate hand based on trigger
            //
            if (source.handedness == m_eHandedness )
            {
                bool bWorkedPos = pt.TryGetPosition(out pos);
                if (bWorkedPos)
                {
                    transform.position = pos;
                }
                bool bWorkedRot = pt.TryGetRotation(out rot);
                if (bWorkedRot)
                {
                    transform.rotation = rot;
                }

            }
        }
        catch (Exception ex)
        {
            int DebugThis = 0;
        }
    }
#endif

    public float GetLastTriggerAmount( )
    {
        return m_fLastTriggerAmount;
    }

    // Update is called once per frame
    void Update ()
    {
        float f = Input.GetAxis(m_szTriggerAxisName);
        // only close hand halfway if not holding ball
        if (!IsHoldingBall())
        {
            f /= 2.0f;
        }
        if (f != m_fLastTriggerAmount)
        {
            m_fLastTriggerAmount = f;
            if (m_pAnimator != null)
            {
                m_pAnimator.Play(m_szAnimationName, 0, f * 0.9f);
            }
        }

        if (m_bIsLeftHand == true)
        {
#if STEAM_VR
            // we get the local position of the hand relative to the camera. wherever the camera is, the
            // position is relative to that.
            Vector3 pHandPos_L = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftHand);
            Quaternion pHandRot_L = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftHand);
            
            // so we are able to juggle around
            pHandRot_L = RotateUpsideDown(pHandRot_L);

            transform.localPosition = pHandPos_L;
            transform.localRotation = pHandRot_L;
            // transform.position = pHandPos_L;
            // transform.rotation = pHandRot_L;
#endif

            if( Input.GetAxis( m_szTriggerAxisName ) > 0.05f )
            {
                // turn collider off, we want to GRAB the ball, not smack it
                _TurnColliderOff();
            }
            else
            {
                _TurnColliderOn();
            }

            if (Input.GetAxis(m_szGripAxisName) >= 0.5f) // middle finger
            {
                if (Input.GetAxis(m_szTriggerAxisName) > 0.5)
                {
                    float tNow = Time.time;
                    float tElapsed = tNow - m_LastTimeBallSpawned;
                    {
                    if (tElapsed > 0.3f)
                        if (!IsHoldingBall())
                        {
                            SpawnBall();
                        }
                    }
                }
            }
            else
            {
                m_fStartHold = -1.0f;
            }

        }
        else
        {
#if STEAM_VR
            // we get the local position of the hand relative to the camera. wherever the camera is, the
            // position is relative to that.
            Vector3 pHandPos_R = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightHand );
            Quaternion pHandRot_R = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightHand );

            // so we are able to juggle around
            pHandRot_R = RotateUpsideDown( pHandRot_R );

            transform.localPosition = pHandPos_R;
            transform.localRotation = pHandRot_R;
            // transform.position = pHandPos_R;
            // transform.rotation = pHandRot_R;
#endif

            if( Input.GetAxis( m_szTriggerAxisName ) > 0.05f )
            {
                // turn collider off, we want to GRAB the ball, not smack it
                _TurnColliderOff();
            }
            else
            {
                _TurnColliderOn();
            }

            if( Input.GetAxis(m_szGripAxisName) >= 0.5f)
            {
                if (Input.GetAxis(m_szTriggerAxisName) > 0.5)
                {
                    float tNow = Time.time;
                    float tElapsed = tNow - m_LastTimeBallSpawned;
                    if (tElapsed > 1.0f)
                    {
                        if (!IsHoldingBall())
                        {
                            SpawnBall();
                        }
                    }
                }
            }
            else
            {
                m_fStartHold = -1.0f;
            }
        }
    }

    public void SpawnBall( )
    {
        m_LastTimeBallSpawned = Time.time;

        Vector3 pNewBallPos = GetSpawnPoint();
        GameObject pNewBall = Instantiate(m_pBallPrefab);
        pNewBall.transform.position = pNewBallPos;

        if( m_fStartHold == -1 )
        {
            m_fStartHold = Time.time;
        }
        float fTimeHeld = Time.time - m_fStartHold;

        float fVelocityMultiplier = fTimeHeld;

        if( fVelocityMultiplier < 1.0f )
        {
            fVelocityMultiplier = 1.0f;
        }

        // the velocity multiplier isn't such a great idea...
        fVelocityMultiplier = 1.0f;

        if( m_bIsLeftHand == true )
        {
            BallScript pBallScript = pNewBall.GetComponent<BallScript>();
            pBallScript.SpawnBall(this, false);
            Rigidbody rb = pNewBall.GetComponent<Rigidbody>();
            rb.velocity = -transform.up * 2 * fVelocityMultiplier;
            rb.AddTorque( UnityEngine.Random.insideUnitSphere );
        }
        else
        {
            StartHoldingBall(pNewBall);

            BallScript pBallScript = pNewBall.GetComponent<BallScript>();
            pBallScript.SpawnBall(this, true);
        }
    }

    public bool IsHoldingBall( )
    {
        return m_pHeldBall != null;
    }

    public void StartHoldingBall( GameObject pBall )
    {
        m_pHeldBall = pBall;

    }

    public void ReleaseBall( )
    {
        m_pHeldBall = null;
        Renderer pRenderer = GetComponentInChildren<Renderer>();
        pRenderer.material = m_pNormalMaterial;
    }

    public Vector3 GetPalmVelocityDuringBallThrow( )
    {
        return m_pPalmFollowerScript.GetLatestVelocityForThrowingBall();
    }

    void _TurnColliderOn( )
    {
        int childCount = transform.childCount;
        for( int i = 0 ; i < childCount ; i++ )
        {
            Transform tChild = transform.GetChild( i );
            Collider pChildCollider = tChild.GetComponent<Collider>();
            if( pChildCollider )
            {
                pChildCollider.enabled = true;
            }
        }
    }

    void _TurnColliderOff( )
    {
        int childCount = transform.childCount;
        for( int i = 0 ; i < childCount ; i++ )
        {
            Transform tChild = transform.GetChild( i );
            Collider pChildCollider = tChild.GetComponent<Collider>();
            if( pChildCollider )
            {
                pChildCollider.enabled = false;
            }
        }
    }

    void AddBoneScriptToAllChildren( Transform t )
    {
        Collider c = t.GetComponent<Collider>();
        if( c != null )
        {
            t.gameObject.AddComponent<FollowerParentScript>();
            FollowerParentScript pParentScript = t.GetComponent<FollowerParentScript>();
            FollowerScript pFollowerScript = pParentScript.GenerateFollower( m_pHandMaterial );

            if( t.name.Contains( "Palm" ) )
            {
                m_pPalmFollowerScript = pFollowerScript;
            }
        }

        int childCount = t.childCount;
        for( int i = 0; i < childCount; i++ )
        {
            AddBoneScriptToAllChildren(t.GetChild(i));
        }
    }

    private Quaternion RotateUpsideDown( Quaternion original )
    {
        return original * Quaternion.Euler( new Vector3( 0, 0, 180 ) );
    }
}
