using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKControlDetection : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private BoolVariable _playerDetected;
    [SerializeField]
    private Transform _objectToLook = null;
    [SerializeField]
    private DetectPlayer _detectPlayer;
    #endregion

    #region Private
    private Animator _animator;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    #endregion

    #region IK
    private void OnAnimatorIK(int layerIndex)
    {
        if (!_animator || _objectToLook == null)
        {
            return;
        }
        if (_detectPlayer.IsPlayerDetected)
        {
            _animator.SetLookAtPosition(_objectToLook.position);
            _animator.SetLookAtWeight(1, 0.5f);
        }
    }
    #endregion
}
