using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    private const int LeftMouseButton = 0;

    private Camera _camera;
    private Base _selectedBase;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftMouseButton))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) == false)
            {
                return;
            }

            if (hit.collider.gameObject.TryGetComponent(out Plane _) && _selectedBase)
            {
                _selectedBase.OnBaseCreating(hit.point);
                _selectedBase = null;
            }
            else if (hit.collider.gameObject.TryGetComponent(out Base selectedBase))
            {
                _selectedBase = selectedBase;
            }
        }
    }
}
