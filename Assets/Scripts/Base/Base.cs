using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private UncollectedResources _resources;

    private bool _isCreatingNewBase;
    private Vector3 _newBasePoint;

    private void OnDisable()
    {
        _botDispatcher.BotCameBack -= OnBotCameBack;
    }

    private void Update()
    {
        TrySendBotToResource();
    }

    public void Init(Wallet wallet, BaseCreator baseCreator, BotCreator botCreator, List<Bot> bots, Flag flag, UncollectedResources resources) // Вся логика загрузки перенесена в Init, чтобы не было разсинхрона с Awake
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

    public bool CheckBotMembership(Bot bot) => _botDispatcher.CheckBotMembership(bot);

    public void OnBaseCreating(Vector3 newBasePoint)
    {
        if (_isCreatingNewBase)
        {
            return;
        }
        
        Transform flag = Instantiate(_flagPrefab, newBasePoint, Quaternion.identity).transform;
        _isCreatingNewBase = true;
        _newBasePoint = flag.position;
    }

    private void TryCreateNewBase()
    {
        if (_botDispatcher.TryDeleteBot(out Bot bot))
        {
            _baseCreator.Create(_newBasePoint, bot, _botCreator, _flagPrefab, _resources);
            _wallet.SpendResource(_resourcesToNewBase);
        }
    }

    private void TrySendBotToResource()
    {
        if (_botDispatcher.TrySendBotToResource(_resources.Spawned) == false)
        {
            return;
        }
        
        _resources.TookResource(_resources.Spawned[0]);
        _resources.RemoveSpawned(_resources.Spawned[0]);
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
            TryCreateNewBase();
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
