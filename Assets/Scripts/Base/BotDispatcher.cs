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
    
    public int BotsCount => _bots.Count;
    public int FreeBotsCount => _freeBots.Count;

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

    public bool IsBotMember(Bot bot)
    {
        if (_bots.Contains(bot))
        {
            return true;
        }
        
        return false;
    }

    public void SendBotToResource(Resource resource)
    {
        _freeBots.Dequeue().GoToResource(resource);
    }

    public void DeleteBot(out Bot bot)
    {
        bot = null;
        
        bot = _freeBots.Dequeue();
        _bots.Remove(bot);
        bot.CameBack -= OnBotCameBack;
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
