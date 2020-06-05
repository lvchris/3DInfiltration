using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private float _chaseSpeed = 2.5f;
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private DetectPlayer _detectPlayer;
    [SerializeField]
    private Transform _target;
    #endregion

    #region Private
    private float _initialSpeed;
    private float _resetTimer = 1f;
    private float _initialResetTimer;
    private bool _playerWasDetected;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _initialSpeed = _agent.speed;
        _initialResetTimer = _resetTimer;
    }

    private void Update()
    {
        if (_detectPlayer.IsPlayerDetected)
        {
            _agent.speed = _chaseSpeed;
            _agent.ResetPath();
            _agent.destination = _target.position;
            _resetTimer = _initialResetTimer;
            _playerWasDetected = true;
        }
        else
        {
            if (_resetTimer >= 0) _resetTimer -= Time.deltaTime;
            if (_playerWasDetected && _agent.remainingDistance < 0.5f)
            {
                _agent.ResetPath();
                if (_resetTimer < 0)
                {
                    _agent.isStopped = true;
                    _playerWasDetected = false;
                }
            }
            _agent.speed = _initialSpeed;
        }
    }
    #endregion
}
