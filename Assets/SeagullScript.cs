using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter( Collision collision )
    {
        Transform pRoot = transform.parent;
        GameObject pSeagullGO = pRoot.gameObject;
        Animator pAnimator = pSeagullGO.GetComponent<Animator>();
        int i = (int) Random.Range( 0, 10 );
        switch( i )
        {
            case 0:
                pAnimator.Play( "Attack" );
                break;
            case 1:
                pAnimator.Play( "Hit" );
                break;
            case 2:
                pAnimator.Play( "Flap" );
                break;
            default:
                pAnimator.Play( "EatStart" );
                break;
        }
    }
}
