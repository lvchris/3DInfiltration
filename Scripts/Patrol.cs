using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private Transform[] _points;
    [SerializeField]
    private float _waitTimer = 3f;
    [SerializeField]
    private DetectPlayer _detectPlayer;
    #endregion

    #region Private
    private int _destPoint = 0;
    private float _initialWaitTimer;
    private NavMeshAgent _agent;
    private bool _playerWasDetected;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _initialWaitTimer = _waitTimer;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        GotoNextPoint();
    }

    void Update()
    {
        // if player is detected
        if (_detectPlayer.IsPlayerDetected)
        {
            _waitTimer = _initialWaitTimer;
            _playerWasDetected = true;
            return;
        }
        // if agent is stopped, agent stay still and search for a while
        if (_agent.isStopped && _waitTimer >= 0f) _waitTimer -= Time.deltaTime;
        // if agent didn't find anything after a while or arrived at a patrol point, agent goes to next patrol point
        if ((!_agent.pathPending && _agent.remainingDistance < 0.5f && !_playerWasDetected) || _waitTimer < 0)
        {
            if (!_playerWasDetected) _agent.isStopped = true;
            // after waiting, goes to next patrol point
            if (_waitTimer < 0)
            {
                GotoNextPoint();
                _playerWasDetected = false;
                _waitTimer = _initialWaitTimer;
            }
        }
    }
    #endregion

    #region Functions
    void GotoNextPoint()
    {
        if (_points.Length == 0) return;
        _agent.destination = _points[_destPoint].position;
        _destPoint = (_destPoint + 1) % _points.Length;
        _agent.isStopped = false;
    }
    #endregion
}