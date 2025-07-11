using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCreator : Creator<BaseRoot>
{
    private Vector3 _position;
    private Bot _bot;
    private BotCreator _botCreator;
    private Flag _flag;
    private UncollectedResources _resources;

    public void SendBotToNewBase(Vector3 position, Bot bot, BotCreator botCreator, Flag flag, UncollectedResources resources)
    {
        _position = position;
        _bot = bot;
        _botCreator = botCreator;
        _flag = flag;
        _resources = resources;
        bot.ChangeStartPosition(new Vector2(position.x, position.z));
        bot.ComeToNewBase += Create;
    }

    private void Create(Flag flag)
    {
        _bot.ComeToNewBase -= Create;

        Destroy(flag.gameObject);
        BaseRoot newBase = Instantiate(Prefab, new Vector3(_position.x, Prefab.transform.position.y, _position.z), Quaternion.identity);
        newBase.Init(new Wallet(), this, _botCreator, new List<Bot>() { _bot }, _flag, _resources);
    }
}
