using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player _player;
    private Camera _camera;

    private float xThreshold = 0.25f;
    private float yThreshold = 0.75f;

    private Vector3 FacingDirection
    {
        get
        {
            return _camera.gameObject.transform.forward;
        }
    }

    private void Start()
    {
        _player = GetComponent<Player>();
        _camera = Camera.main;
    }

    private void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical"); // as we are rotated 180 degrees

        // gets rid of sensitive input
        if (Mathf.Abs(xInput) < xThreshold) xInput = 0;
        if (Mathf.Abs(yInput) < yThreshold) yInput = 0;

        var input = new Vector3(xInput, 0, yInput);
        
        // rotate the input torwards the direction we are facing
        var angle = Vector3.Angle(FacingDirection, Vector3.forward);
        var lookingRotation = Quaternion.AngleAxis(angle, Vector3.up);
        var target = lookingRotation * input;

        _player.SetDirectionalInput(target);
    }
}
