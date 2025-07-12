using System.Collections;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private ResourcesPool _resources;
    [SerializeField] private float _spawnDelay = 1;
    
    private WaitForSeconds _wait;

    private void Awake()
    {
        _wait = new(_spawnDelay);
    }

    private void Start()
    {
        StartCoroutine(SpawnResources());
    }

    private IEnumerator SpawnResources()
    {
        while (enabled)
        {
            _resources.CreateResource();
            yield return _wait;
        }
    }
}
