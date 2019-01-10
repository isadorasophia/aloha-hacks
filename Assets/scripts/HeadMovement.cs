using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadMovement : MonoBehaviour
{
    private Player _player;
    private Animator _animator;
    
    void Start()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        UpdateAnimator();
    }
    
    private void UpdateAnimator()
    {
        _animator.SetFloat("velocity", _player.Velocity);
    }

}
