using System.Collections.Generic;
using UnityEngine;

public class BaseCreator : Creator<BaseRoot>
{
    public BaseRoot Create(Vector3 position, Bot bot, BotCreator botCreator, Flag flag)
    {
        Debug.Log(position);
        bot.ChangeStartPosition(position);
        BaseRoot newBase = Instantiate(Prefab, position, Quaternion.identity);
        newBase.Init(new Wallet(), this, botCreator, new List<Bot>() { bot }, flag);
        return newBase;
    }
}
