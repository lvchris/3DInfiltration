using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Serialized
#pragma warning disable CS0649
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private float _rotateSpeed = 10f;
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    private float _maxVelocity = 10f;

    [SerializeField]
    private float _crouchMult = 0.5f;
    [SerializeField]
    private float _sprintMult = 2f;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private BoolVariable _isAlive;
    [SerializeField]
    private BoolVariable _isLanding;
    [SerializeField]
    private BoolVariable _isCrouching;
    [SerializeField]
    private BoolVariable _activateDebug;
    [SerializeField]
    private BoolVariable _playstationController;
#pragma warning restore CS0649
    #endregion

    #region Private
    private Transform _transform;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private Vector3 _initialColliderCenter;
    private Vector3 _spawnPosition;

    private int _speedXID = Animator.StringToHash("SpeedX");
    private int _speedZID = Animator.StringToHash("SpeedZ");
    private int _crouchID = Animator.StringToHash("IsCrouching");
    private int _sprintID = Animator.StringToHash("IsSprinting");

    private bool _isSprinting = false;

    private float _speedMult = 1f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _initialColliderCenter = _collider.center;
        _isAlive.value = true;
        _isCrouching.value = false;
    }

    private void FixedUpdate()
    {
        if(_isAlive.value || _isLanding.value) GamepadMovement();
    }
    #endregion

    #region Gamepad
    private void GamepadMovement()
    {
        _speedMult = 1f;
        CrouchToggle();
        WalkMovement();
        Taunt();
    }

    private void Taunt()
    {
        if ((_playstationController.value && Input.GetButtonDown("PS4_Taunt")) || 
            (!_playstationController.value && Input.GetButtonDown("Taunt")))
        {
            _animator.SetTrigger("Taunt");
        }
    }

    private void CrouchToggle()
    {
        if ((!_playstationController.value && Input.GetButtonDown("Crouch")) || 
            (_playstationController.value && Input.GetButtonDown("PS4_Crouch")))
        {
            _isCrouching.value = !_isCrouching.value;
        }
        if (_isCrouching.value)
        {
            _collider.center = new Vector3(0, _initialColliderCenter.y/2, 0);
            _speedMult = _crouchMult;
        }
        else
        {
            _collider.center = _initialColliderCenter;
        }
        _animator.SetBool(_crouchID, _isCrouching.value);
    }

    private void SprintToggle(float directionZ)
    {
        _isSprinting = false;
        if ((!_playstationController.value && Input.GetAxisRaw("Sprint") != 0) || 
            (_playstationController.value && Input.GetAxisRaw("PS4_Sprint") != -1) && 
                directionZ > 0.25)
        {
            _isSprinting = true;
            _speedMult = _sprintMult;
            _isCrouching.value = false;
            _animator.SetBool(_crouchID, _isCrouching.value);
        }
        _animator.SetBool(_sprintID, _isSprinting);
    }

    private void WalkMovement()
    {
        float directionX;
        float directionZ;
        if (_playstationController.value)
        {
            directionX = Input.GetAxisRaw("PS4_Hori_L");
            directionZ = Input.GetAxisRaw("PS4_Vert_L");
        }
        else
        {
            directionX = Input.GetAxisRaw("Horizontal_L");
            directionZ = Input.GetAxisRaw("Vertical_L");
        }

        if (directionX == 0 && directionZ == 0) _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);

        RotateInCameraDirection(directionZ);
        SprintToggle(directionZ);

        Vector3 newSpeed = ((_cameraTransform.right * directionX) + (_cameraTransform.forward * directionZ));

        newSpeed.x = Mathf.Abs(_rigidbody.velocity.x) + Mathf.Abs(newSpeed.x) > _maxVelocity ?
                     (newSpeed.x > 0 ? _maxVelocity : -(_maxVelocity)) - _rigidbody.velocity.x : newSpeed.x;

        newSpeed.z = Mathf.Abs(_rigidbody.velocity.z) + Mathf.Abs(newSpeed.z) > _maxVelocity ?
                     (newSpeed.z > 0 ? _maxVelocity : -(_maxVelocity)) - _rigidbody.velocity.z : newSpeed.z;

        newSpeed.y = 0;
        //Debug.Log(newSpeed);
        _rigidbody.velocity = new Vector3(newSpeed.normalized.x * _speedMult * _speed, _rigidbody.velocity.y, newSpeed.normalized.z * _speedMult * _speed);

        _animator.SetFloat(_speedXID, directionX);
        _animator.SetFloat(_speedZID, directionZ);
    }

    private void RotateInCameraDirection(float directionZ)
    {
        //tourne le joueur en direction de la camera quand on avance
        if (directionZ != 0)
        {
            Vector3 targetDirection = _cameraTransform.forward;
            targetDirection.y = 0;
            float singleStep = _rotateSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(_transform.forward, targetDirection, singleStep, 0.0f);
            
            _rigidbody.MoveRotation(Quaternion.LookRotation(newDirection));
        }
    }

    private float NormalizeValue(float value, float minValue, float maxValue)
    {
        return (value - minValue)/(maxValue - minValue);
    }
    #endregion

#if UNITY_EDITOR
    #region Debug
    private void OnGUI()
    {
        if (!_activateDebug.value) return;
        GUI.Button(new Rect(10, 10, 200, 30), "<b>HL:</b> " + Input.GetAxisRaw("Horizontal_L"));
        GUI.Button(new Rect(10, 40, 200, 30), "<b>VL:</b> " + Input.GetAxisRaw("Vertical_L"));
    }
    #endregion
#endif
}
