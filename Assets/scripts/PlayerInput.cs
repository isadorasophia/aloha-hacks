﻿using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player _player;

    private float xThreshold = 0.075f;
    private float yThreshold = 0.75f;

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        // gets rid of sensitive input
        if (Mathf.Abs(xInput) < xThreshold) xInput = 0;

        // TODO: for some reason the y input default state is kinda messed up?
        if (yInput < 0 && Mathf.Abs(yInput) < yThreshold) yInput = 0;
        
        _player.SetDirectionalInput(-new Vector2(xInput, yInput));
    }
}
