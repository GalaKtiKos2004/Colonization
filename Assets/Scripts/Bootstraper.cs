using System.Collections.Generic;
using UnityEngine;

public class Bootstraper : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private BaseView _baseView;
    [SerializeField] private BaseCreator _baseCreator;
    [SerializeField] private BotCreator _botCreator;
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private List<Bot> _bots;
    [SerializeField] private UncollectedResources _resources;

    private void Awake()
    {
        Wallet wallet = new();
        _base.Init(wallet, _baseCreator, _botCreator, _bots, _flagPrefab, _resources);
        _baseView.Init(wallet);
    }
}
