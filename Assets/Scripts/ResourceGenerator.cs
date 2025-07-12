using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private Resource _prefab;

    [SerializeField] private int _poolCapcity = 10;
    [SerializeField] private int _maxPoolSize = 10;
    [SerializeField] private float _spawnDelay;

    private WaitForSeconds _wait;
    private List<SpawnPoint> _occupiedPoints = new List<SpawnPoint>();
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
        foreach (var spawnPoint in _spawnPoints)
        {
            spawnPoint.ResourceLeft += OnResourceLeft;
        }

        StartCoroutine(SpawnResources());
    }

    private void OnDestroy()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            spawnPoint.ResourceLeft -= OnResourceLeft;
        }
    }

    private void ActionOnGet(Resource resource)
    {
        SpawnPoint spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

        if (_occupiedPoints.Contains(spawnPoint))
        {
            return;
        }
        
        resource.transform.position = spawnPoint.transform.position;
        resource.gameObject.SetActive(true);
    }

    private void OnResourceLeft(SpawnPoint spawnPoint) =>
        _occupiedPoints.Remove(spawnPoint);

    private IEnumerator SpawnResources()
    {
        while (enabled)
        {
            _resources.Get();
            yield return _wait;
        }
    }
}
