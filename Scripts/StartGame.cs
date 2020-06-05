using UnityEngine;

public class StartGame : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private BoolVariable _playerDetected;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _playerDetected.value = false;
    }
    #endregion
}
