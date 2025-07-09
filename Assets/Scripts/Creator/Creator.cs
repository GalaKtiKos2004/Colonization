using UnityEngine;

public class Creator<T> : MonoBehaviour where T : MonoBehaviour, ICreatable
{
    [SerializeField] private T _prefab;
    
    protected T Prefab => _prefab;
} 
