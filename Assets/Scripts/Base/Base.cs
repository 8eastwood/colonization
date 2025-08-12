using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private CollectorSpawner _collectorSpawner;
    [SerializeField] private DataBase _dataBase;
    [SerializeField] private Scanner _scanner;
    
    [Space(10)]
    
    [SerializeField] private SpawnPointProvider _spawnPointProvider;
    [SerializeField] private CollectorsCollectionsHandler _collectorsCollectionsHandler;
    [SerializeField] private CollisionHandler _collisionHandler;
    [SerializeField] private FlagPlacer _flagPlacer;
    [SerializeField] private Storage _storage;
    [SerializeField] private DropOff _dropOff;
    [SerializeField] private int _initialBotAmount = 3;

    private bool _initialSpawnDone = false;

    public int CollectorsCount => _collectorsCollectionsHandler.Collectors.Count;
    public FlagPlacer FlagPlacer => _flagPlacer;
    public DataBase DataBase => _dataBase;
    public DropOff DropOff => _dropOff;

    private void OnEnable()
    {
        _collisionHandler.CollectorReached += OnSupplyDeliver;

        if (_scanner != null)
        {
            _scanner.SuppliesFounded += AssignCollector;
        }
    }

    private void Start()
    {
        if (_initialSpawnDone == false)
        {
            _initialSpawnDone = true;
            SpawnInitialsCollectors();
        }

        _scanner.StartScan();
    }

    private void OnDisable()
    {
        _scanner.SuppliesFounded -= AssignCollector;
        _collisionHandler.CollectorReached -= OnSupplyDeliver;
    }

    public void Init(DataBase dataBase, Scanner scanner, CollectorSpawner collectorSpawner)
    {
        _dataBase = dataBase;
        _scanner = scanner;
        _collectorSpawner = collectorSpawner;
        _initialSpawnDone = true;
        _scanner.SuppliesFounded += AssignCollector;
        _flagPlacer.HideFlag();
    }

    public void AddCollector(Collector collector)
    {
        var spawnPoint = _spawnPointProvider.GetSpawnPoint();
        
        _collectorsCollectionsHandler.AddFreeCollector(collector);
        _collectorsCollectionsHandler.AddCollector(collector);
        collector.SetBaseInfo(DropOff, spawnPoint);
        collector.ResetToSpawnPoint();
    }

    public void SendBotToBuildBase()
    {
        if (_collectorsCollectionsHandler.FreeCollectors.Count > 0)
        {
            Collector collector = _collectorsCollectionsHandler.GetFreeCollector();

            collector.SetTargetToFlag(_flagPlacer.Flag.transform.position);
            _collectorsCollectionsHandler.RemoveFreeCollector(collector);
            collector.ReachedFlag += RemoveFlag;
            _collectorsCollectionsHandler.RemoveCollector(collector);
        }
    }

    private void SpawnInitialsCollectors()
    {
        for (int i = 0; i < _initialBotAmount; i++)
        {
            SpawnCollector();
        }
    }

    private void SpawnCollector()
    {
        Collector collector = _collectorSpawner.SpawnCollector();
        AddCollector(collector);
        
        collector.ResetToSpawnPoint();
    }

    private void RemoveFlag(Collector collector)
    {
        _flagPlacer.HideFlag();
        collector.ReachedFlag -= RemoveFlag;
    }

    public void SpawnAdditionalCollector()
    {
        SpawnCollector();
    }

    private void OnSupplyDeliver(Collector collector)
    {
        _storage.HandleScore(collector.TargetSupplyBox);
        DataBase.RemoveSuppliesFromCollection(collector.TargetSupplyBox);
        collector.TargetSupplyBox.Destroy();
        collector.FreeFromTask();
        collector.ResetToSpawnPoint();
        _collectorsCollectionsHandler.AddFreeCollector(collector);

        if (_collectorsCollectionsHandler.FreeCollectors.Count != 0 && _dataBase.SuppliesToCollect.Count > 0)
        {
            AssignCollector();
        }
    }

    private void AssignCollector()
    {
        if (_dataBase.SuppliesToCollect.Count == 0 || _collectorsCollectionsHandler.FreeCollectors.Count == 0)
            return;

        for (int i = _collectorsCollectionsHandler.FreeCollectors.Count - 1; i >= 0; i--)
        {
            if (_dataBase.SuppliesToCollect.Count == 0)
                break;

            SupplyBox task = RequestToAssignTask();
            Collector collector = _collectorsCollectionsHandler.FreeCollectors[i];
            collector.RecieveTargetPosition(task);
            _collectorsCollectionsHandler.RemoveFreeCollector(collector);;
        }
    }

    public SupplyBox RequestToAssignTask()
    {
        SupplyBox task = _dataBase.SuppliesToCollect.Dequeue();
        _dataBase.SuppliesToDeliver.Add(task);

        return task;
    }
}