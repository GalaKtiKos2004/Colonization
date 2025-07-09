using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotDispatcher : MonoBehaviour
{
    private Queue<Bot> _freeBots;
    private List<Bot> _bots;

    private BotCreator _creator;

    public event Action<Resource> BotCameBack;

    private void OnDisable()
    {
        foreach (var bot in _bots)
        {
            bot.CameBack -= OnBotCameBack;
        }
    }

    public void Init(List<Bot> bots, BotCreator creator)
    {
        _freeBots = new();
        _bots = bots.ToList();
        _creator = creator;

        foreach (var bot in _bots)
        {
            _freeBots.Enqueue(bot);
            bot.CameBack += OnBotCameBack;
        }
    }

    public bool CheckBotMembership(Bot bot)
    {
        if (_bots.Contains(bot))
        {
            return true;
        }
        
        return false;
    }

    public bool TrySendBotToResource(List<Resource> resources)
    {
        if (_freeBots.Count == 0 || resources.Count == 0)
        {
            return false;
        }

        _freeBots.Dequeue().GoToResource(resources[0]);
        return true;
    }

    public bool TryDeleteBot(out Bot bot)
    {
        bot = null;
        
        if (_bots.Count <= 1)
        {
            return false;
        }
        
        bot = _bots[0];
        Bot deletedBot = bot;
        _bots.Remove(bot);
        _freeBots = new Queue<Bot>(_freeBots.Where(freeBot => freeBot != deletedBot));
        return true;
    }

    public void CreateBot(Vector3 position)
    {
        Bot bot = _creator.Create(position);
        _freeBots.Enqueue(bot);
        _bots.Add(bot);
        bot.CameBack += OnBotCameBack;
    }

    private void OnBotCameBack(Bot bot, Resource resource)
    {
        _freeBots.Enqueue(bot);
        BotCameBack?.Invoke(resource);
    }
}
