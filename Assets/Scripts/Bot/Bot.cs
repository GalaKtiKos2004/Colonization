using System;
using UnityEngine;

[RequireComponent(typeof(BotMover))]
public class Bot : MonoBehaviour, ICreatable
{
    private Resource _resource;
    private BotMover _mover;

    private Vector2 _startPosition;

    private bool _isResourceSelected = false;
    private bool _isGoToNewBase = false;

    public event Action<Bot, Resource> CameBack;
    public event Action<Flag> ComeToNewBase;

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

        if (other.TryGetComponent(out Flag flag) && _isGoToNewBase)
        {
            _isGoToNewBase = false;
            ComeToNewBase?.Invoke(flag);
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
        _startPosition = position;
        _mover.GoToPoint(new Vector3(_startPosition.x, 0f, _startPosition.y));
        _isGoToNewBase = true;
    }
}
