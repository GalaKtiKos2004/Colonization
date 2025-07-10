using System.Collections.Generic;
using UnityEngine;

public class BaseRoot : MonoBehaviour, ICreatable
{
    [SerializeField] private Base _baseBody;
    [SerializeField] private BaseView _baseView;

    public void Init(Wallet wallet, BaseCreator baseCreator, BotCreator botCreator, List<Bot> bots, Flag flag, UncollectedResources resources)
    {
        _baseBody.Init(wallet, baseCreator, botCreator, bots, flag, resources);
        _baseView.Init(wallet);
    }
}
