using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TopPanel: MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private TextMeshProUGUI bestPlatformTMP;
    [SerializeField] private TextMeshProUGUI coinsTMP;
#pragma warning restore CS0649

    public int BestPlatformValue
    {
        set
        {
            bestPlatformTMP.text = value.ToString();
        }
    }

    public int Coins
    {
        set
        {
            coinsTMP.text = value.ToString();
        }
    }
}
