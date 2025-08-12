using System;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private ScoreCounter _scoreCounter;
    [SerializeField] private Base _base;

    private int _resourcesForNewCollector = 3;
    private int _resourcesForNewBase = 5;

    public void HandleScore(SupplyBox supply)
    {
        _scoreCounter.Add();
        CheckScoreMilestones();
    }

    private void CheckScoreMilestones()
    {
        bool hasResourcesForBot = _scoreCounter.Score >= _resourcesForNewCollector;
        bool isBotSingle = _base.CollectorsCount == 1;
        bool isFlagPlaced = _base.FlagPlacer.IsFlagSet;

        if (hasResourcesForBot && (isBotSingle || isFlagPlaced == false))
        {
            _scoreCounter.SpendScore(_resourcesForNewCollector);
            _base.SpawnAdditionalCollector();
            return;
        }

        if (_scoreCounter.Score >= _resourcesForNewBase)
        {
            _base.SendBotToBuildBase();
            _scoreCounter.SpendScore(_resourcesForNewBase);
        }
    }
}