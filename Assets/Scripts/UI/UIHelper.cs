using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private TopPanel topPanel;
    [SerializeField] private GameObject preStartPanel;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private GameObject blackPanel;

    [SerializeField] private TextMeshProUGUI currPlatformValueTMP;
#pragma warning restore CS0649

    public TopPanel TopPanel { get => topPanel; set => topPanel = value; }
    public GameOverPanel GameOverPanel { get => gameOverPanel; set => gameOverPanel = value; }

    public int CurrPlatformValue
    {
        set
        {
            currPlatformValueTMP.text = value.ToString();
            currPlatformValueTMP.gameObject.SetActive(value != 0? true: false);
        }
    }

    public bool PreStartPanelActive
    {
        set
        {
            preStartPanel.SetActive(value);
        }
    }

    //Мне кажется это лучше чем использование аниматора, который загрязнял бы объекты.
    public IEnumerator SetScorePanelActive(bool isActive)
    {
        if (isActive)
        {
            gameOverPanel.gameObject.SetActive(isActive);
        }
        var time = 0f;
        var startPosition = new Vector3(isActive ? 320 : 0, 0, 0);
        var endPosition = new Vector3(isActive ? 0 : 320, 0, 0);

        gameOverPanel.scorePanel.localPosition = new Vector3(
            startPosition.x,
            gameOverPanel.scorePanel.localPosition.y,
            0);
        gameOverPanel.restartButton.localPosition = new Vector3(
            -startPosition.x,
            gameOverPanel.restartButton.localPosition.y,
            0);
       
        while (true)
        {
            yield return new WaitForFixedUpdate();

            time += 0.06f;
            var position = Vector3.Lerp(startPosition, endPosition, time);

            gameOverPanel.scorePanel.localPosition = new Vector3(
                position.x,
                gameOverPanel.scorePanel.localPosition.y,
                0);
            gameOverPanel.restartButton.localPosition = new Vector3(
                -position.x,
                gameOverPanel.restartButton.localPosition.y,
                0);

            if (time >= 1)
            {
                if (!isActive)
                {
                    gameOverPanel.gameObject.SetActive(isActive);
                }
                yield break;
            }
        }
    }

    public IEnumerator BlackScreenAnimation(bool show)
    {
        float alfa = show ? 0 : 1;
        float speed = 0.05f * (show? 1 : -1);
        var image = blackPanel.GetComponent<Image>();
        var color = image.color;

        if (show)
        {
            blackPanel.SetActive(show);
        }

        while (true)
        {
            alfa += speed;
            color.a = alfa;
            image.color = color;

            if(show && alfa >= 1 || !show && alfa <= 0)
            {
                if (!show)
                {
                    blackPanel.SetActive(show);
                }
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetCurrPlatformTextPosition(Transform platform)
    {
        currPlatformValueTMP.gameObject.transform.position =
            GameManager.Instance.MainCamera.WorldToScreenPoint(platform.position + new Vector3(-0.5f, 0.5f));
    } 
}
