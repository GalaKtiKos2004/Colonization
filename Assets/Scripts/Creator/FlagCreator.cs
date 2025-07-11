using System;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class FlagCreator : Creator<Flag>
{
    private Base _base;
    private Flag _flag;
    private bool _isFlagInstalled = false;
    
    private void Awake()
    {
        _base = GetComponent<Base>();
    }

    private void OnEnable()
    {
        _base.NewBaseCreating += Create;
        _base.NewBaseCreated += FlagDestroyed;
    }

    private void OnDisable()
    {
        _base.NewBaseCreating -= Create;
        _base.NewBaseCreated -= FlagDestroyed;
    }

    private void Create(Vector3 position)
    {
        if (_isFlagInstalled == false)
        {
            _isFlagInstalled = true;
            _flag = Instantiate(Prefab, position, Quaternion.identity);
            return;
        }
        
        _flag.transform.position = position;
    }

    private void FlagDestroyed() => _isFlagInstalled = false;
}
