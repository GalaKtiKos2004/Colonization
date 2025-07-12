using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class ResourcesPool : MonoBehaviour
{
    [SerializeField] private Resource _prefab;
    [SerializeField] private int _poolCapcity = 10;
    [SerializeField] private int _maxPoolSize = 50;
    [SerializeField] private float _minPositionX = -9.94f;
    [SerializeField] private float _minPositionZ = -5.95f;
    [SerializeField] private float _maxPositionX = 9.98f;
    [SerializeField] private float _maxPositionZ = 8.82f;
    
    private ObjectPool<Resource> _resources;

    private void Awake()
    {
        _resources = new ObjectPool<Resource>(
            createFunc: () => Instantiate(_prefab),
            actionOnGet: resource => ActionOnGet(resource),
            actionOnRelease: resource => ActionOnRelease(resource),
            actionOnDestroy: resource => Destroy(resource.gameObject),
            collectionCheck: true,
            defaultCapacity: _poolCapcity,
            maxSize: _maxPoolSize);
    }

    public void CreateResource()
    {
        _resources.Get();
    }

    public void ReleaseResource(Resource resource)
    {
        _resources.Release(resource);
    }
    
    private void ActionOnGet(Resource resource)
    {
        resource.transform.position = new Vector3(Random.Range(_minPositionX, _maxPositionX), _prefab.transform.position.y,
            Random.Range(_minPositionZ, _maxPositionZ));
        resource.gameObject.SetActive(true);
    }
    
    private void ActionOnRelease(Resource resource)
    {
        resource.SetParent(null);
        resource.gameObject.SetActive(false);
    }
}
