using UnityEngine;

public class BotCreator : Creator<Bot>
{
    [SerializeField] private NavMeshAgentPriority _navMeshAgent;

    public Bot Create(Vector3 position)
    {
        Bot bot = Instantiate(Prefab, position, Quaternion.identity);
        bot.InitAgent(_navMeshAgent.Priority);
        _navMeshAgent.DecreasePriority();
        return bot;
    }
}
