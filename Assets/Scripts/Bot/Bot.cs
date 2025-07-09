using System;
using UnityEngine;

[RequireComponent(typeof(BotMover))]
public class Bot : MonoBehaviour, ICreatable
{
    [SerializeField] private float _distanceToTarget = 0.1f;
    
    private Resource _resource;
    private BotMover _mover;

    private Vector2 _startPosition;

    private bool _isResourceSelected = false;

    public event Action<Bot, Resource> CameBack;

    private void Awake()
    {
        _mover = GetComponent<BotMover>();
        _startPosition = new Vector2(transform.position.x, transform.position.z);
    }

    private void Update()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);

        if (_resource == null)
        {
            return;
        }

        Vector2 resourcePosition = new Vector2(_resource.transform.position.x, _resource.transform.position.z);

        if (currentPosition.IsEnoughClose(resourcePosition, _distanceToTarget) && _isResourceSelected == false)
        {
            _isResourceSelected = true;
            _resource.SetParent(transform);
            _mover.GoToPoint(new Vector3(_startPosition.x, 0f, _startPosition.y));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Base collidedBase) == false || _resource == null)
        {
            return;
        }

        if (collidedBase.CheckBotMembership(this))
        {
            CameBack?.Invoke(this, _resource);
            _isResourceSelected = false;
        }
    }

    public void InitAgent(int priority)
    {
        _mover.InitAgent(priority);
    }

    public void GoToResource(Resource resource)
    {
        _resource = resource;
        _mover.GoToPoint(resource.transform.position);
        Debug.Log(resource.transform.position);
    }

    public void ChangeStartPosition(Vector2 position)
    {
        _startPosition = position;
        _mover.GoToPoint(new Vector3(_startPosition.x, 0f, _startPosition.y));
    }
}
