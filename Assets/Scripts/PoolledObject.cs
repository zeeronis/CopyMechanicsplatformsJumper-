using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PoolledObject: MonoBehaviour
{
    [HideInInspector] public bool isUsed;

    public virtual void ReturnToPool()
    {
        isUsed = false;
        gameObject.SetActive(false);
    }
}
