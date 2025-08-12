using UnityEngine;

public class SpawnPointProvider : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private SpawnPoint _spawnPoint;

    private float _offsetZ;

    public SpawnPoint GetSpawnPoint()
    {
        float stepBetweenSpawnPoints = -5;

        if (_offsetZ == 0)
        {
            _offsetZ += stepBetweenSpawnPoints;

            return _spawnPoint;
        }

        SpawnPoint newSpawnPoint = Instantiate(
            _spawnPoint,
            _spawnPoint.transform.position,
            _spawnPoint.transform.rotation
        );

        newSpawnPoint.transform.Translate(0f, 0f, _offsetZ);
        _offsetZ += stepBetweenSpawnPoints;

        return newSpawnPoint;
    }
}