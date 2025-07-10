using System;
using UnityEngine;

[RequireComponent(typeof(BotMover))]
public class Bot : MonoBehaviour, ICreatable
{
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Base collidedBase) && _isResourceSelected && collidedBase.CheckBotMembership(this))
        {
            CameBack?.Invoke(this, _resource);
            _isResourceSelected = false;
        }

        if (other.TryGetComponent(out Resource resource) && resource == _resource)
        {
            _resource.SetParent(transform);
            _isResourceSelected = true;
            _mover.GoToPoint(new Vector3(_startPosition.x, 0f, _startPosition.y));
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
    }

    public void ChangeStartPosition(Vector2 position)
    {
        Debug.Log(gameObject.name + " : ChangeStartPosition");
        _startPosition = position;
        _mover.GoToPoint(new Vector3(_startPosition.x, 0f, _startPosition.y));
    }
}
