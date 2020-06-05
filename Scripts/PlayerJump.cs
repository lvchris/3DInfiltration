using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private float _jumpPower = 5f;
    [SerializeField]
    private float _fallMult = 2.5f;
    [SerializeField]
    private float _offsetRayCast = 0.2f;
    [SerializeField]
    private float _floorRayCastLength = 1f;

    [SerializeField]
    private LayerMask _envLayer;

    [SerializeField]
    private AnimationCurve _animCurve;

    [SerializeField]
    private BoolVariable _isAlive;
    [SerializeField]
    private BoolVariable _isOnGround;
    [SerializeField]
    private BoolVariable _isLanding;
    [SerializeField]
    private BoolVariable _playstationController;

    [SerializeField]
    private Animator _animator;
    #endregion

    #region Private
    private float _timeJump = 0f;
    private float _jumpTime = 1f;
    private float _initialJumpTime;
    private float _initialFallMult;
    private float _fallMultIncrement = 5;

    private bool _isJumping = false;
    private bool _willLand = false;

    private Rigidbody _rigidbody;
    private Transform _transform;

    Vector3 CombinedRaycast;

    private int _isJumpingID = Animator.StringToHash("IsJumping");
    private int _isFallingId;
    private int _isLandingId = Animator.StringToHash("IsLanding");
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();

        _initialFallMult = _fallMult;
        _initialJumpTime = _jumpTime;
        _jumpTime = 0f;

        _isOnGround.value = true;
        _isLanding.value = false;
    }

    private void FixedUpdate()
    {
        if (!_isAlive.value) return;

        _isLanding.value = false;
        //Check if there is a ground underneath
        IsGrounded();

        _animator.SetBool(_isJumpingID, _isJumping);

        if ((_playstationController.value && Input.GetButtonDown("PS4_Jump")) ||
            (!_playstationController.value && Input.GetButtonDown("Jump")) &&
            _isOnGround.value)
            TriggerJump();
        JumpAndFall();

        if (_isOnGround.value && !_isJumping)
        {
            Vector3 floorMovement = new Vector3(_rigidbody.position.x, FindFloor().y, _rigidbody.position.z);

            // only stick to floor when grounded
            if (FloorRaycasts(0, 0, 0.6f) != Vector3.zero && floorMovement != _rigidbody.position)
            {
                // move the rigidbody to the floor
                _rigidbody.MovePosition(floorMovement);
            }
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        }

        _animator.SetBool(_isLandingId, _isLanding.value);
    }

    private void JumpAndFall()
    {
        if (_isJumping && _timeJump < 1)
        {
            _rigidbody.velocity += new Vector3(0, _jumpPower * _animCurve.Evaluate(_timeJump), 0);
            _timeJump += Time.deltaTime * 2;
        }
        if (!_isOnGround.value)
        {
            if (_isJumping && _timeJump >= 1)
            {
                _timeJump = 0f;
                _isJumping = false;
                _willLand = true;
            }
            else if (_isJumping) return;
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Physics.gravity.y * _fallMult * Time.deltaTime, _rigidbody.velocity.z);
        }
    }
    #endregion

    #region Functions
    private void TriggerJump()
    {
        if (Mathf.Approximately(_rigidbody.velocity.y, 0f))
        {
            _isJumping = true;
        }
    }

    private void IsGrounded()
    {
        Vector3 position = new Vector3(_transform.position.x, _transform.position.y + 0.5f, _transform.position.z);
        Vector3 direction = Vector3.down;
        float distance = 0.6f;

        Debug.DrawRay(position, direction * distance, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit, distance, _envLayer))
        {
            _isOnGround.value = true;
            if(_willLand || (_isOnGround.value && _timeJump >= 1))
            {
                _isLanding.value = true;
                _fallMult = _initialFallMult;
                _isJumping = false;
                _willLand = false;
                _timeJump = 0f;
            }
        }
        else
        {
            _isOnGround.value = false;
        }
    }
    #endregion

    #region AstroKat_MinionsArt
    Vector3 FindFloor()
    {
        // width of raycasts around the center of your character
        float raycastWidth = 0.25f;
        // check floor on 5 raycasts   , get the average when not Vector3.zero  
        int floorAverage = 1;

        CombinedRaycast = FloorRaycasts(0, 0, 1.6f);
        floorAverage += (getFloorAverage(raycastWidth, 0) + getFloorAverage(-raycastWidth, 0) + getFloorAverage(0, raycastWidth) + getFloorAverage(0, -raycastWidth));

        return CombinedRaycast / floorAverage;
    }

    // only add to average floor position if its not Vector3.zero
    int getFloorAverage(float offsetx, float offsetz)
    {

        if (FloorRaycasts(offsetx, offsetz, 1.6f) != Vector3.zero)
        {
            CombinedRaycast += FloorRaycasts(offsetx, offsetz, 1.6f);
            return 1;
        }
        else { return 0; }
    }


    Vector3 FloorRaycasts(float offsetx, float offsetz, float raycastLength)
    {
        RaycastHit hit;
        // move raycast
        Vector3 raycastFloorPos = transform.TransformPoint(0 + offsetx, 0 + 0.5f, 0 + offsetz);

        Debug.DrawRay(raycastFloorPos, Vector3.down, Color.magenta);
        if (Physics.Raycast(raycastFloorPos, -Vector3.up, out hit, raycastLength))
        {
            return hit.point;
        }
        else return Vector3.zero;
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, 0.05f);

        Vector3 oPos = new Vector3(position.x + _offsetRayCast, position.y, position.z);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(oPos, 0.05f);
        oPos = new Vector3(position.x - _offsetRayCast, position.y, position.z);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(oPos, 0.05f);
        oPos = new Vector3(position.x, position.y, position.z + _offsetRayCast);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(oPos, 0.05f);
        oPos = new Vector3(position.x, position.y, position.z - _offsetRayCast);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(oPos, 0.05f);
    }
    #endregion
}
