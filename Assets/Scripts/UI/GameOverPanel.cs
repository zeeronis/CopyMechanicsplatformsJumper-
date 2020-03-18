using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] public Transform scorePanel;
    [SerializeField] public Transform restartButton;
    [SerializeField] private TextMeshProUGUI scoreTmp;

    public int Score
    {
        set
        {
            scoreTmp.text = value.ToString();
        }
    }
}
