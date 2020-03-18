using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; private set => instance = value; }

#pragma warning disable CS0649
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UIHelper uiHelper;
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Transform startPlatformTransform;
    [Space]
    [SerializeField] private float spikesOffsetY = 4.6f;
    [SerializeField] private Transform spikesTopTransform;
    [SerializeField] private Transform spikesBottomTransform;
#pragma warning restore CS0649

    private List<PoolledObject> platforms = new List<PoolledObject>();
    private Transform currPlatformTransform;

    private int bestPlatformNum = 0;
    private int currPlatformValue = 0;
    private int coins = 0;

    private float startPlatformFallSpeed = 0.01f;
    private float platformFallSpeed = 0.01f;
    private bool waitPlayerMoving = false;

    public Camera MainCamera { get => mainCamera; set => mainCamera = value; }

    public bool GameIsRunned { get; private set; }
    public bool GameIsReadyForStart { get; private set; }

    private int BestPlatformNum
    {
        get
        {
            return bestPlatformNum;
        }
        set
        {
            bestPlatformNum = value;
            uiHelper.TopPanel.BestPlatformValue = value;
        }
    }

    private int Coins
    {
        get
        {
            return coins;
        }
        set
        {
            coins = value;
            uiHelper.TopPanel.Coins = value;
        }
    }

    public int CurrPlatformValue
    {
        get
        {
            return currPlatformValue;
        }
        set
        {

            currPlatformValue = value;
            uiHelper.CurrPlatformValue = value;
        }
    }


    private void Start()
    {
        if (Instance == null)
            Instance = this;

        RestartGame(true);
    }


    //Мне кажется, я злоупотребляю количесвтом корутин в этом проекте.
    //Как минимум можно было бы вынести код из FallPatform и FollowPatformTextToPlatform в fixedUpdate.
    private IEnumerator OnPlayerChangePlatform(float posY, bool needMoveUp)
    {
        if (needMoveUp)
        {
            yield return StartCoroutine(MoveGameObject(player.transform, GetPlayerPosOnPlatform(currPlatformTransform).y, true));
        }

        var time = 0f;
        var startPosition = mainCamera.transform.position;
        var endPosition = new Vector3(0, posY, -10);

        while (true)
        {
            if (!waitPlayerMoving)
            {
                time += 0.08f;

                var position = Vector3.Lerp(startPosition, endPosition, time);
                mainCamera.transform.position = position;

                position.z = 0;
                spikesTopTransform.position = position + new Vector3(0, spikesOffsetY, 0);
                spikesBottomTransform.position = position + new Vector3(0, -spikesOffsetY, 0);
            }
            if (time >= 1)
            {
                StartCoroutine(FallPlatform(currPlatformTransform, platformFallSpeed));
                SpawnNextPlatform();

                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DespawnPlatform(PoolledObject platform)
    {
        yield return StartCoroutine(MoveGameObject(platform.transform, player.position.y - 6));

        platforms.Remove(platform);
        platform.ReturnToPool();
    }

    private IEnumerator MoveGameObject(Transform _objectTransform, float posY, bool setPlayerWait = false)
    {
        var time = 0f;
        var startPosition = _objectTransform.position;
        var endPosition = new Vector3(0, posY, 0);

        while (true)
        {
            time += 0.1f;
            _objectTransform.position = Vector3.Lerp(startPosition, endPosition, time);

            yield return new WaitForFixedUpdate();

            if (time >= 1)
            {
                waitPlayerMoving = false;
                yield break;
            }
           
        }
    }

    private IEnumerator FallPlatform(Transform platform, float speed)
    {
        while (true)
        {
            if (currPlatformTransform != platform)
                yield break;

            platform.position -= new Vector3(0, speed, 0);

            if(spikesBottomTransform.transform.position.y + 0.6f > player.position.y)
            {
                player.transform.parent = null;
                GameOver(true);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FollowPatformTextToPlatform()
    {
        while (true)
        {
            if(currPlatformValue > 0 && currPlatformTransform != null)
            {
                uiHelper.SetCurrPlatformTextPosition(currPlatformTransform);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator GameOverCoroutine()
    {
        StartCoroutine(MoveGameObject(player.transform, GetPlayerPosOnPlatform(spikesBottomTransform).y));
        yield return StartCoroutine(MoveGameObject(currPlatformTransform, spikesBottomTransform.position.y - 0.5f));
        yield return StartCoroutine(uiHelper.SetScorePanelActive(true));

        StopAllCoroutines();
    }

    private IEnumerator RestartGameCoroutine(bool skipAnimations)
    {
        if (!skipAnimations)
        {
            yield return StartCoroutine(uiHelper.SetScorePanelActive(false));
            yield return StartCoroutine(uiHelper.BlackScreenAnimation(true));
        }
        

        foreach (var item in platforms)
        {
            item.ReturnToPool();
        }

        CurrPlatformValue = 0;
        waitPlayerMoving = false;
        currPlatformTransform = null;
        platformFallSpeed = startPlatformFallSpeed;

        platforms.Add(PoolManager.Instance.GetObject(new Vector3(0, mainCamera.transform.position.y - 1, 0)));
        startPlatformTransform.position = new Vector3(0, mainCamera.transform.position.y - 4, 0);
        player.transform.position = GetPlayerPosOnPlatform(startPlatformTransform);

        uiHelper.PreStartPanelActive = true;

        if (!skipAnimations)
        {
            yield return StartCoroutine(uiHelper.BlackScreenAnimation(false));
        }

        GameIsReadyForStart = true;
    }

    private void SpawnNextPlatform()
    {
        var platform = PoolManager.Instance.GetObject(
            new Vector3(0, player.position.y + 6, 0));
        platforms.Add(platform);

        StartCoroutine(MoveGameObject(platform.transform, player.position.y + UnityEngine.Random.Range(1.8f, 2.7f)));
    }

    public Vector3 GetPlayerPosOnPlatform(Transform platform)
    {
        var playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
        var platformRenderer = platform.GetComponent<SpriteRenderer>();
        var posY = playerRenderer.size.y * playerRenderer.transform.lossyScale.y / 2 +
            platformRenderer.size.y * platformRenderer.transform.lossyScale.y / 2 +
            platform.position.y;

        return new Vector3(0, posY, 0);
    }

    public void StartGame()
    {
        GameIsRunned = true;
        uiHelper.PreStartPanelActive = false;
        StartCoroutine(FollowPatformTextToPlatform());
    }

    public void RestartGame(bool skipAnimations)
    {
        StopAllCoroutines();
        StartCoroutine(RestartGameCoroutine(skipAnimations));
      
    }

    public void GameOver(bool isSpikes)
    {
        if (!GameIsRunned)
            return;

        GameIsReadyForStart = false;
        GameIsRunned = false;
        player.simulated = false;
        player.transform.parent = null;

        uiHelper.GameOverPanel.Score = currPlatformValue;
        if (isSpikes)
        {
            player.velocity = Vector2.zero;
            StartCoroutine(uiHelper.SetScorePanelActive(true));
        }
        else
        {
            StartCoroutine(GameOverCoroutine());
        }
    }

    public void SetPlayerPlatform(Transform _currPlatform, bool needMoveUp = false)
    {
        if ( _currPlatform == startPlatformTransform || !GameIsRunned)
            return;

        if(_currPlatform == currPlatformTransform)
        {
            GameOver(false);
            return;
        }

        if (currPlatformTransform == null)
        {
            StartCoroutine(MoveGameObject(startPlatformTransform, startPlatformTransform.position.y - 3));
        }
        else
        {
            StartCoroutine(DespawnPlatform(currPlatformTransform.GetComponent<PoolledObject>()));
        }

        currPlatformTransform = _currPlatform;
        if (bestPlatformNum < ++CurrPlatformValue)
            BestPlatformNum = CurrPlatformValue;

        player.transform.parent = _currPlatform.transform;
        player.velocity = Vector2.zero;
        player.simulated = false;

        if (needMoveUp)
        {
            waitPlayerMoving = true;
        }
        StartCoroutine(OnPlayerChangePlatform(player.position.y, needMoveUp));
    }
}
