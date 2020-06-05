using UnityEngine;
using Cinemachine;

public class ResetCamera : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private CinemachineFreeLook _freeLookCam;
    [SerializeField]
    private BoolVariable _activateDebug;
    #endregion

    #region Private
    private float _initialCameraValueX;
    private float _initialCameraValueY;
    #endregion

    private void Awake()
    {
        _initialCameraValueX = _freeLookCam.m_XAxis.Value;
        _initialCameraValueY = _freeLookCam.m_YAxis.Value;
    }

    private void Update()
    {
        if (Input.GetButtonDown("ResetCamera"))
        {
            _freeLookCam.m_XAxis.Value = _initialCameraValueX;
            _freeLookCam.m_YAxis.Value = _initialCameraValueY;
        }
    }

#if UNITY_EDITOR
    #region Debug
    private void OnGUI()
    {
        if (!_activateDebug.value) return;
        GUI.Button(new Rect(10, 70, 200, 30), "<b>HR:</b> " + Input.GetAxisRaw("Horizontal_R"));
        GUI.Button(new Rect(10, 100, 200, 30), "<b>VR:</b> " + Input.GetAxisRaw("Vertical_R"));
    }
    #endregion
#endif
}
