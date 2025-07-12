using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private Resource _prefab;

    [SerializeField] private float _spawnDelay;

    private WaitForSeconds _wait;
    private List<SpawnPoint> _occupiedPoints = new List<SpawnPoint>();

    private void Awake()
    {
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

    private void OnResourceLeft(SpawnPoint spawnPoint) =>
        _occupiedPoints.Remove(spawnPoint);

    private IEnumerator SpawnResources()
    {
        while (enabled)
        {
            SpawnPoint spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

            if (_occupiedPoints.Contains(spawnPoint) == false)
            {
                spawnPoint.SpawnResource(_prefab);
                _occupiedPoints.Add(spawnPoint);
            }

            yield return _wait;
        }
    }
}
