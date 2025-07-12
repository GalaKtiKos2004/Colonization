using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseScaner))]
[RequireComponent(typeof(BotDispatcher))]
public class Base : MonoBehaviour
{
    [SerializeField] private float _scanDelay;
    [SerializeField] private int _resourcesToNewBot = 3;
    [SerializeField] private int _resourcesToNewBase = 5;

    private WaitForSeconds _wait;

    private Wallet _wallet;
    private BaseScaner _scaner;
    private BaseCreator _baseCreator;
    private BotCreator _botCreator;
    private BotDispatcher _botDispatcher;
    private Flag _flagPrefab;
    private Flag _flag;
    private ResourceStorage _resources;

    private bool _isCreatingNewBase;
    private Vector3 _newBasePoint;
    
    public event Action<Vector3> NewBaseCreating;
    public event Action NewBaseCreated;

    private void OnDisable()
    {
        _botDispatcher.BotCameBack -= OnBotCameBack;
    }

    private void Update()
    {
        TrySendBotToResource();
    }

    public void Init(Wallet wallet, BaseCreator baseCreator, BotCreator botCreator, List<Bot> bots, Flag flag,
        ResourceStorage resources) // Вся логика загрузки перенесена в Init, чтобы не было разсинхрона с Awake
    {
        _wait = new(_scanDelay);
        _scaner = GetComponent<BaseScaner>();
        _botDispatcher = GetComponent<BotDispatcher>();
        _wallet = wallet;
        _baseCreator = baseCreator;
        _botCreator = botCreator;
        _isCreatingNewBase = false;
        _botDispatcher.Init(bots, botCreator);
        _flagPrefab = flag;
        _resources = resources;

        _botDispatcher.BotCameBack += OnBotCameBack;
        StartCoroutine(ScanDelay());
    }

    public bool IsBotMember(Bot bot) => _botDispatcher.IsBotMember(bot);

    public void OnBaseCreating(Vector3 newBasePoint)
    {
        if (_botDispatcher.BotsCount <= 1)
        {
            return;
        }
        
        _newBasePoint = newBasePoint;
        NewBaseCreating?.Invoke(newBasePoint);
        _isCreatingNewBase = true;
    }

    private void CreateNewBase()
    {
        _botDispatcher.DeleteBot(out Bot bot);
        SendBotToNewBase(bot);
        _wallet.SpendResource(_resourcesToNewBase);
    }

    private void SendBotToNewBase(Bot bot)
    {
        bot.ChangeStartPosition(new Vector2(_newBasePoint.x, _newBasePoint.z));
        bot.ComeToNewBase += OnBotComeToNewBase;
    }

    private void OnBotComeToNewBase(Bot bot)
    {
        NewBaseCreated?.Invoke();
        bot.ComeToNewBase -= OnBotComeToNewBase;
        _baseCreator.Create(_newBasePoint, bot, _botCreator, _flagPrefab, _resources);
        _isCreatingNewBase = false;
    }

    private void TrySendBotToResource()
    {
        if (_resources.TryTakeResource(out Resource resource))
        {
            _botDispatcher.TrySendBotToResource(resource);
        }
    }

    private void OnBotCameBack(Resource resource)
    {
        Destroy(resource.gameObject);

        _wallet.AddResource();

        if (_wallet.ResourcesCount == _resourcesToNewBot && _isCreatingNewBase == false)
        {
            CreateNewBot();
        }

        if (_wallet.ResourcesCount == _resourcesToNewBase && _isCreatingNewBase)
        {
            CreateNewBase();
        }

        _resources.RemoveTransit(resource);
    }

    private void CreateNewBot()
    {
        _botDispatcher.CreateBot(transform.position);
        _wallet.SpendResource(_resourcesToNewBot);
    }

    private IEnumerator ScanDelay()
    {
        while (enabled)
        {
            _scaner.Scan(_resources);
            yield return _wait;
        }
    }
}
