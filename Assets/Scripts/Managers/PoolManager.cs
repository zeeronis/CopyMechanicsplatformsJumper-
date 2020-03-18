using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PoolManager: MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance { get => instance; private set => instance = value; }

#pragma warning disable CS0649
    [SerializeField] private PoolledObject platformPrefab;
#pragma warning restore CS0649

    private List<PoolledObject> poolledObjects = new List<PoolledObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public PoolledObject GetObject(Vector3 position)
    {
        foreach (var item in poolledObjects)
        {
            if (!item.isUsed)
            {
                item.isUsed = true;
                item.gameObject.SetActive(true);
                item.transform.position = position;
                return item;
            }
        }

        var obj = Instantiate(platformPrefab, position, Quaternion.identity);
        poolledObjects.Add(obj);
        obj.isUsed = true;
        return obj;
    }
}
