using UnityEngine;

public class AimHelper : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _maxAimDistance = 1000f;
    [SerializeField] private float _defaultAimDistance = 100f;

    private RaycastHit[] _raycastHits = new RaycastHit[1];

    public Vector3 GetAimPoint(LayerMask layerMask)
    {
        if (_playerCamera == null)
            return transform.position + transform.forward * _defaultAimDistance;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray ray = _playerCamera.ScreenPointToRay(screenCenter);

        int hits = Physics.RaycastNonAlloc(ray, _raycastHits, _maxAimDistance, layerMask);

        return hits > 0 ? _raycastHits[0].point : ray.origin + ray.direction * _defaultAimDistance;
    }

    public Vector3 GetAimDirection(Vector3 fromPosition, LayerMask layerMask)
    {
        Vector3 aimPoint = GetAimPoint(layerMask);

        return (aimPoint - fromPosition).normalized;
    }
}