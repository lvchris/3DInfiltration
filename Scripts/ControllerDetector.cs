using UnityEngine;
public class ControllerDetector : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private BoolVariable _playstationController;
    #endregion

    #region Private
    private string[] JoystickNames;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        JoystickNames = Input.GetJoystickNames();
        if (JoystickNames.Length >= 1 && JoystickNames[0] == "Wireless Controller")
            _playstationController.value = true;
        else _playstationController.value = false;
    }
    #endregion
}
