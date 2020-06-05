using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private DetectPlayer _detectPlayer;
    [SerializeField]
    private Animator _animator;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        _animator.SetFloat("Speed", _agent.velocity.normalized.sqrMagnitude);
        _animator.SetBool("HasDetectedPlayer", _detectPlayer.IsPlayerDetected);
    }
    #endregion
}
