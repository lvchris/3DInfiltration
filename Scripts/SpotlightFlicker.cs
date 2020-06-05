using UnityEngine;

[RequireComponent(typeof(Light))]
public class SpotlightFlicker : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    private Material _unlitMaterial;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    #endregion

    #region Private
    private Light _light;
    private Material _litMaterial;

    private float _timer = 1.3f;
    private float _initialTimer;
    private float _initialIntensity;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _light = GetComponent<Light>();
        _initialIntensity = _light.intensity;
        _initialTimer = _timer;
        _litMaterial = _meshRenderer.material;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= -.5f)
        {
            _timer = _initialTimer;
            _light.intensity = _initialIntensity;
            _meshRenderer.material = _litMaterial;
        }
        else if (_timer <= -.2f)
        {
            _light.intensity = 0;
            _meshRenderer.material = _unlitMaterial;
        }
        else if (_timer <= -.1f)
        {
            _light.intensity = _initialIntensity;
            _meshRenderer.material = _litMaterial;
        }
        else if (_timer <= 0f)
        {
            _light.intensity = 0;
            _meshRenderer.material = _unlitMaterial;
        }
    }
    #endregion
}
