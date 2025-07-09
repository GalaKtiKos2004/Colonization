using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotMover : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>(); 
    }

    public void InitAgent(int priority)
    {
        _agent.avoidancePriority = priority;
    }

    public void GoToPoint(Vector3 targetPosition)
    {
        _agent.SetDestination(targetPosition);
    }
}
