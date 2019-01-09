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
    public Material m_pHoldingSomethingMaterial;
    public PhysicMaterial m_pHandMaterial;

    FollowerScript m_pPalmFollowerScript;

    float m_LastTimeBallSpawned;
    Animator m_pAnimator;
    float m_fLastTriggerAmount = 0.0f;
    string m_szTriggerAxisName;
    string m_szAnimationName;
    string m_szGripAxisName;
    string m_szTouchpadVerticalAxisName;
    bool m_bIsLeftHand;

    GameObject m_pHeldBall;

    static HandScript s_pLeftHand;
    static HandScript s_pRightHand;

    void Start()
    {
        if (name.Contains("Left"))
        {
            m_bIsLeftHand = true;
            m_szTriggerAxisName = "TriggerLeft";
            m_szAnimationName = "Ghost_GripBallState_L" + "";
            m_szGripAxisName = "GripLeft";
            m_szTouchpadVerticalAxisName = "Touchpad Vertical Left";
            s_pLeftHand = this;
        }
        else if (name.Contains("Right"))
        {
            m_bIsLeftHand = false;
            m_szTriggerAxisName = "TriggerRight";
            m_szAnimationName = "Ghost_GripBallState_R";
            m_szGripAxisName = "GripRight";
            m_szTouchpadVerticalAxisName = "Touchpad Vertical Right";
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
            Vector3 pHandPos_L = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftHand);
            Quaternion pHandRot_L = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftHand);
            transform.position = pHandPos_L;
            transform.rotation = pHandRot_L;
#endif

            if (Input.GetAxis(m_szGripAxisName) >= 0.5f)
            {
                if (Input.GetAxis(m_szTriggerAxisName) > 0.5)
                {
                    float tNow = Time.time;
                    float tElapsed = tNow - m_LastTimeBallSpawned;
                    if (tElapsed > 0.1f)
                    {
                        if (!IsHoldingBall())
                        {
                            SpawnBall();
                        }
                    }
                }
            }

        }
        else
        {
#if STEAM_VR
            Vector3 pHandPos_R = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightHand );
            Quaternion pHandRot_R = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightHand );
            transform.position = pHandPos_R;
            transform.rotation = pHandRot_R;
#endif

            if (Input.GetAxis(m_szGripAxisName) >= 0.5f)
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
        }
    }

    public void SpawnBall( )
    {
        m_LastTimeBallSpawned = Time.time;

        Vector3 pNewBallPos = GetSpawnPoint();
        GameObject pNewBall = Instantiate(m_pBallPrefab);
        pNewBall.transform.position = pNewBallPos;

        if( m_bIsLeftHand == true )
        {
            BallScript pBallScript = pNewBall.GetComponent<BallScript>();
            pBallScript.SpawnBall(this, false);
            Rigidbody rb = pNewBall.GetComponent<Rigidbody>();
            rb.velocity = -transform.up;
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
        Renderer pRenderer = GetComponentInChildren<Renderer>();
        pRenderer.material = m_pHoldingSomethingMaterial;

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
}
