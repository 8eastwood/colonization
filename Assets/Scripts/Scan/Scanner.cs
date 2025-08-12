using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private DataBase _dataBase;
    [Header("Settings")]
    [SerializeField] private float _scanRadius;
    [SerializeField] private LayerMask _targetLayer;

    private Coroutine _scanRoutine;
    private float _delay = 1f;

    public event Action SuppliesFounded;

    private void OnEnable()
    {
        _dataBase.NoSuppliesLeft += StartScan;
    }

    private void OnDisable()
    {
        _dataBase.NoSuppliesLeft -= StartScan;
    }

    public void StartScan()
    {
        _scanRoutine = StartCoroutine(Scan());
    }

    private void ScanForSupplies()
    {
        Collider[] suppliesBuffer = new Collider[20];

        int hitsCount = Physics.OverlapSphereNonAlloc(transform.position, _scanRadius, suppliesBuffer, _targetLayer);

        Queue<SupplyBox> toCollect = new();

        for (int i = 0; i < hitsCount; i++)
        {
            SupplyBox supplyBox = suppliesBuffer[i].GetComponent<SupplyBox>();

            if (supplyBox != null && !toCollect.Contains(supplyBox))
            {
                toCollect.Enqueue(supplyBox);
            }
        }

        if (toCollect.Count > 0)
        {
            _dataBase.GetSuppliesToCollect(toCollect);
            SuppliesFounded?.Invoke();
            StopCoroutine(_scanRoutine);
        }
    }

    private IEnumerator Scan()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(_delay);

            ScanForSupplies();
        }
    }
}