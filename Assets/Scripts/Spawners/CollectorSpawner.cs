using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CollectorSpawner : MonoBehaviour
{
    // [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private Storage _storage;
    [SerializeField] private Collector _collectorPrefab;
    [SerializeField] private DropOff _dropOff;
    [SerializeField] private Base _base;
    [SerializeField] private float _delay;

    private Coroutine _spawnCollectorsRoutine;

    private SupplyBox _targetSupplyBox;

    // private Transform _spawnPoint;
    private float _offsetZ;
    private int _amountOfCollectorsToSpawn = 3;
    private int _indexOfCollectors = 0;

    private void OnEnable()
    {
        _base.Reassigned += SendToWork;
    }

    private void OnDisable()
    {
        _base.Reassigned -= SendToWork;
    }

    public void StartSpawnCollectors()
    {
        _spawnCollectorsRoutine = StartCoroutine(SpawnCollectors());
    }

    public SupplyBox RequestToAssignTask()
    {
        SupplyBox supply = _storage.AssignTask();

        if (supply != null)
        {
            return supply;
        }

        return null;
    }

    public void ExpansionCollectorsAmount()
    {
        _amountOfCollectorsToSpawn++;

        if (_spawnCollectorsRoutine != null)
        {
            StopCoroutine(_spawnCollectorsRoutine);
        }

        _spawnCollectorsRoutine = StartCoroutine(SpawnCollectors());
    }

    private SpawnPoint GetSpawnPoint()
    {
        float stepBetweenSpawnPoints = -5;
        
        if (_offsetZ == 0)
        {
            _offsetZ += stepBetweenSpawnPoints;
            return _spawnPoint;
        }
        else
        {
            SpawnPoint newSpawnPoint = Instantiate(_spawnPoint, _spawnPoint.transform.position, _spawnPoint.transform.rotation);
            newSpawnPoint.transform.Translate(0f, 0f, _offsetZ);
            _offsetZ += stepBetweenSpawnPoints;

            return newSpawnPoint;
        }
    }

    private IEnumerator SpawnCollectors()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);

        while (enabled)
        {
            yield return wait;

            if (_indexOfCollectors < _amountOfCollectorsToSpawn)
            {
                _targetSupplyBox = RequestToAssignTask();

                if (_targetSupplyBox != null)
                {
                    SpawnPoint spawnPoint = GetSpawnPoint();
                    Collector collector = Spawn(spawnPoint);
                    collector.RecieveTargetPosition(_targetSupplyBox);
                    collector.RecieveDropOffPosition(_dropOff);
                    collector.RecieveSpawnPoint(spawnPoint);
                    SendToWork(collector);
                    _indexOfCollectors++;
                }
            }
            else
            {
                StopCoroutine(_spawnCollectorsRoutine);
            }
        }
    }

    private void SendToWork(Collector collector)
    {
        collector.Init();
    }

    private Collector Spawn(SpawnPoint spawnPoint)
    {
        return Instantiate(_collectorPrefab, spawnPoint.transform.position, Quaternion.identity);
    }
}