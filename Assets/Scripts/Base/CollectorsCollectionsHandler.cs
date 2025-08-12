using System.Collections.Generic;
using UnityEngine;

public class CollectorsCollectionsHandler : MonoBehaviour
{
    private List<Collector> _collectors = new List<Collector>();
    private List<Collector> _freeCollectors = new List<Collector>();
    
    public List<Collector> Collectors => _collectors;
    public List<Collector> FreeCollectors => _freeCollectors;

    public void AddFreeCollector(Collector collector)
    {
        _freeCollectors.Add(collector);
    }
    
    public void AddCollector(Collector collector)
    {
        _collectors.Add(collector);
    }

    public void RemoveCollector(Collector collector)
    {
        _collectors.Remove(collector);
    }
    
    public void RemoveFreeCollector(Collector collector)
    {
        _freeCollectors.Remove(collector);
    }

    public Collector GetFreeCollector()
    {
        return _freeCollectors[Random.Range(0, _freeCollectors.Count)];
    }
}
