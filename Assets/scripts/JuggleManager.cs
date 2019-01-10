using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuggleManager : MonoBehaviour
{
    public static JuggleManager Instance;
    public AudioClip m_pBonusFx;
    public Text m_pInAirText;

    public List<BallScript> m_pBallList = new List<BallScript>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BallCaughtByHand( GameObject pBall, bool bLeftHand )
    {
        BallScript pBS = pBall.GetComponent<BallScript>();
        pBS.m_nHowManyTimesCaughtByHands++;
        pBS.m_bLastCaughtByLeftHand = bLeftHand;

        // has this ball been caught more than once, AND been shuffled from hand to hand?
        List<BallScript> pJuggledBalls = GetListOfBallsCurrentlyBeingJuggled();
        m_pInAirText.text = "InAir: " + pJuggledBalls.Count.ToString();

        if( pJuggledBalls.Count > 0 )
        {
            // there are more than 1 balls being juggled, now, have the balls been caught multiple times?
            bool bJackpot = true;
            foreach( BallScript pBS2 in pJuggledBalls )
            {
                if( pBS2.m_nHowManyTimesCaughtByHands < 2 )
                {
                    bJackpot = false;
                }
            }
            if( bJackpot )
            {
                AudioSource pAudioSource = Player.Instance.GetComponent<AudioSource>();
                pAudioSource.PlayOneShot( m_pBonusFx );
            }
        }
    }

    public void BallTouchedNonHand( GameObject pBall )
    {
        BallScript pBS = pBall.GetComponent<BallScript>();
        pBS.m_nHowManyTimesCaughtByHands = 0;
        pBS.m_bLastCaughtByLeftHand = false;
    }

    List<BallScript> GetListOfBallsCurrentlyBeingJuggled( )
    {
        List<BallScript> pList = new List<BallScript>();

        foreach( BallScript pBS in m_pBallList )
        {
            if( pBS.m_nHowManyTimesCaughtByHands > 0 )
            {
                pList.Add( pBS );
            }
        }
        return pList;
    }

    int HowManyBallsBeingJuggled( )
    {
        int nCount = 0;

        foreach( BallScript pBS in m_pBallList )
        {
            if( pBS.m_nHowManyTimesCaughtByHands > 0 )
            {
                nCount++;
            }
        }

        return nCount;
    }
}
