using UnityEngine;

public class NavMeshAgentPriority : MonoBehaviour
{
    [SerializeField] private int _priority;
    
    public int Priority => _priority;

    public void DecreasePriority()
    {
        _priority--;
    }
}
