using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    private List<Resource> _spawned;
    private List<Resource> _inTransit;

    private void Awake()
    {
        _spawned = new();
        _inTransit = new();
    }

    public List<Resource> Spawned => _spawned.ToList();
    public List<Resource> InTransit => _inTransit.ToList();

    public void ResourceSpawned(Resource resource)
    {
        foreach (var res in _inTransit)
        {
            if (res.GetInstanceID() == resource.GetInstanceID())
            {
                Debug.Log("БЛЯТЬ");
            }
        }
        
        _spawned.Add(resource);
    }

    public bool TryTakeResource(out Resource resource)
    {
        if (_spawned.Count == 0)
        {
            resource = null;
            return false;
        }
        
        resource = _spawned[0];
        _inTransit.Add(_spawned[0]);
        RemoveSpawned(resource);
        return true;
    }

    public void RemoveTransit(Resource resource) => 
        _inTransit.Remove(resource);
    
    private void RemoveSpawned(Resource resource) =>
        _spawned.Remove(resource);
}