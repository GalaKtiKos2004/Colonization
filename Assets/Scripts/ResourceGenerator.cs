using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private Resource _prefab;

    [SerializeField] private int _poolCapcity = 10;
    [SerializeField] private int _maxPoolSize = 10;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _minPositionX;
    [SerializeField] private float _minPositionZ;
    [SerializeField] private float _maxPositionX;
    [SerializeField] private float _maxPositionZ;

    private WaitForSeconds _wait;
    private ObjectPool<Resource> _resources;

    private void Awake()
    {
        _resources = new ObjectPool<Resource>(
            createFunc: () => Instantiate(_prefab),
            actionOnGet: resource => ActionOnGet(resource),
            actionOnRelease: resource => resource.gameObject.SetActive(false),
            actionOnDestroy: resource => Destroy(resource.gameObject),
            collectionCheck: true,
            defaultCapacity: _poolCapcity,
            maxSize: _maxPoolSize);
        
        _wait = new(_spawnDelay);
    }

    private void Start()
    {
        StartCoroutine(SpawnResources());
    }

    private void ActionOnGet(Resource resource)
    {
        resource.transform.position = new Vector3(Random.Range(_minPositionX, _maxPositionX), _prefab.transform.position.y,
            Random.Range(_minPositionZ, _maxPositionZ));
        resource.gameObject.SetActive(true);
    }

    private IEnumerator SpawnResources()
    {
        while (enabled)
        {
            _resources.Get();
            yield return _wait;
        }
    }
}
