using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public event Action<SpawnPoint> ResourceLeft;

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Resource _))
        {
            ResourceLeft?.Invoke(this);
        }
    }

    public void SpawnResource(Resource resource)
    {
        Instantiate(resource, transform.position, Quaternion.identity);
    }
}
