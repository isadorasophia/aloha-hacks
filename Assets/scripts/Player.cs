using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
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
    private Animator _animator;
    private AudioSource _walkingSound;

    public static Player Instance;

    void Start()
    {
        Instance = this;
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _walkingSound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        CalculateVelocity();
       
        _rigidBody.velocity = _velocity;
        
        UpdateAnimator();
        UpdateSounds();
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

    private void UpdateAnimator()
    {
        _animator.SetFloat("velocity", _velocity.magnitude);
    }

    private void UpdateSounds()
    {
        if (_velocity.magnitude > .1f)
        {
            if (!_walkingSound.isPlaying)
            {
                _walkingSound.Play();
            }
        }
        else
        {
            _walkingSound.Stop();
        }
    }
}
