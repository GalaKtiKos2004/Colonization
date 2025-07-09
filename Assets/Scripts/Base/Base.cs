using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseScaner))]
[RequireComponent(typeof(UncollectedResources))]
[RequireComponent(typeof(BotDispatcher))]
public class Base : MonoBehaviour
{
    [SerializeField] private float _scanDelay;
    [SerializeField] private int _resourcesToNewBot = 3;
    [SerializeField] private int _resourcesToNewBase = 5;

    private WaitForSeconds _wait;

    private UncollectedResources _resources;
    private Wallet _wallet;
    private BaseScaner _scaner;
    private BaseCreator _baseCreator;
    private BotCreator _botCreator;
    private BotDispatcher _botDispatcher;
    private Flag _flagPrefab;

    private bool _isCreatingNewBase;
    private Vector3 _newBasePoint;

    private void OnDisable()
    {
        _botDispatcher.BotCameBack -= OnBotCameBack;
    }

    private void Update()
    {
        if (_wallet.ResourcesCount == _resourcesToNewBase && _isCreatingNewBase)
        {
            TryCreateNewBase();
        }
        
        TrySendBotToResource();
    }

    public void Init(Wallet wallet, BaseCreator baseCreator, BotCreator botCreator, List<Bot> bots, Flag flag) // Вся логика загрузки перенесена в Init, чтобы не было разсинхрона с Awake
    {
        _wait = new(_scanDelay);
        _resources = GetComponent<UncollectedResources>();
        _scaner = GetComponent<BaseScaner>();
        _botDispatcher = GetComponent<BotDispatcher>();
        _wallet = wallet;
        _baseCreator = baseCreator;
        _botCreator = botCreator;
        _isCreatingNewBase = false;
        _botDispatcher.Init(bots, botCreator);
        _flagPrefab = flag;
        
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
        
        Instantiate(_flagPrefab, newBasePoint, Quaternion.identity);
        Debug.Log(newBasePoint);
        _isCreatingNewBase = true;
        _newBasePoint = newBasePoint;
    }

    private void TryCreateNewBase()
    {
        if (_botDispatcher.TryDeleteBot(out Bot bot))
        {
            _baseCreator.Create(_newBasePoint, bot, _botCreator, _flagPrefab);
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
        _resources.RemoveTransit(resource);
        Destroy(resource.gameObject);
        _wallet.AddResource();
        
        if (_wallet.ResourcesCount == _resourcesToNewBot && _isCreatingNewBase == false)
        {
            CreateNewBot();
        }
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
