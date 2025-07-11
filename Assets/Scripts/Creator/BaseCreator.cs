using System.Collections.Generic;
using UnityEngine;

public class BaseCreator : Creator<BaseRoot>
{
    public BaseRoot Create(Vector3 position, Bot bot, BotCreator botCreator, Flag flag, UncollectedResources resources)
    {
        bot.ChangeStartPosition(new Vector2(position.x, position.z));
        BaseRoot newBase = Instantiate(Prefab, new Vector3(position.x, Prefab.transform.position.y, position.z), Quaternion.identity);
        newBase.Init(new Wallet(), this, botCreator, new List<Bot>() { bot }, flag, resources);
        return newBase;
    }
}
