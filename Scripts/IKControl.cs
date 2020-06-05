using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKControl : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private bool _ikActive = true;
    [SerializeField]
    private Transform _objectToLook = null;
    #endregion

    #region Private
    private Animator _animator;
    private int _lookRange = 3;
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
        if (!_animator || !_ikActive) return;

        if (_objectToLook != null)
        {
            // distance between face and object to look at
            float distanceFaceObject = Vector3.Distance(_animator.GetBoneTransform(HumanBodyBones.Head).position, _objectToLook.position);

            _animator.SetLookAtPosition(_objectToLook.position);
            // blend based on the distance
            _animator.SetLookAtWeight(Mathf.Clamp01(_lookRange - distanceFaceObject), Mathf.Clamp01(1 - distanceFaceObject));
        }
    }
    #endregion
}
