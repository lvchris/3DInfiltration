using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private float _detectionRadius = 1.5f;
    [SerializeField]
    private Light _light;
    [SerializeField]
    private Material _aggressiveMaterial;
    [SerializeField]
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField]
    private Transform _detectionTransform;
    [SerializeField]
    private Transform _ownHead;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private BoolVariable _isCrouching;
    #endregion

    #region Private
    private Material _passiveMaterial;
    private Transform _transform;
    private Color _passiveColor;
    private float _initialDetectionRadius;
    private bool _isPlayerDetected;
    #endregion
    public bool IsPlayerDetected { get => _isPlayerDetected; set => _isPlayerDetected = value; }

    #region Unity Lifecycle
    private void Awake()
    {
        _initialDetectionRadius = _detectionRadius;
        _passiveMaterial = _skinnedMeshRenderer.material;
        _passiveColor = _light.color;
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        Detect();
        ChangeColorOnDetection();
    }
    #endregion

    #region Functions
    private void Detect()
    {
        Collider[] hits = Physics.OverlapSphere(_detectionTransform.position, _detectionRadius);
        foreach(Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                RaycastHit firstHit;
                // ray start from agent head
                Vector3 direction = _target.position - _ownHead.position;
                Debug.DrawRay(_ownHead.position, direction, Color.yellow);
                Physics.Raycast(_ownHead.position, direction, out firstHit);
                // if the ray touch the player without touching anything else first
                // the player is detected
                if (firstHit.collider.CompareTag("Player"))
                {
                    // change value for Patrol script
                    _isPlayerDetected = true;
                    // when player detected, detection radius increase for the chase
                    _detectionRadius = _initialDetectionRadius * 2.5f;
                    return;
                }
            }
        }
        _isPlayerDetected = false;
        // if player is crouching detection radius decrease
        if (_isCrouching.value) _detectionRadius = _initialDetectionRadius / 2f;
        else _detectionRadius = _initialDetectionRadius;
    }

    private void ChangeColorOnDetection()
    {
        if (_isPlayerDetected)
        {
            _skinnedMeshRenderer.material = _aggressiveMaterial;
            _light.color = new Color(1, 0, 0);
        }
        else
        {
            _skinnedMeshRenderer.material = _passiveMaterial;
            _light.color = _passiveColor;
        }
    }
    #endregion

#if UNITY_EDITOR
    #region Debug
    private void OnDrawGizmos()
    {
        if (_detectionTransform == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_detectionTransform.position, _detectionRadius);
    }
    #endregion
#endif

}
