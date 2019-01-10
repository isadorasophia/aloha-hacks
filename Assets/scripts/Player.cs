using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6;

    [SerializeField]
    private float _accelarationTime = .2f;

    /* current status */
    private Vector3 _velocity;
    private Vector3 _directionalInput;

    private float _xVelocitySmoothing;
    private float _zVelocitySmoothing;

    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CalculateVelocity();
       
        _rigidBody.velocity = _velocity;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        _directionalInput = new Vector3(input.x, 0, input.y);
        Debug.Log("Processing input of: " + input.x + " and: " + input.y);
    }

    private void CalculateVelocity()
    {
        float xTargetVelocity = _directionalInput.x * _speed;
        float zTargetVelocity = _directionalInput.z * _speed;

        _velocity.x = Mathf.SmoothDamp(_velocity.x, xTargetVelocity, ref _xVelocitySmoothing, _accelarationTime);
        _velocity.z = Mathf.SmoothDamp(_velocity.z, zTargetVelocity, ref _zVelocitySmoothing, _accelarationTime);
    }
}
