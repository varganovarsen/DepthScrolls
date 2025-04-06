using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

    const string GAME_CONTROLLER_PREFAB_PATH = "Prefabs/GameController";
    private static GameController instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject guiPrefab;

    UI_Controller uiController;



    public PlayerController player;
    private bool isInit;

    Coroutine rockSpawningCoroutine;

    public static GameController Instance => instance;

    public static event Action OnGameRestart;

    public static event Action OnLevelReset;


    void Awake()
    {
        if(instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }

        GameReset();
    }

    private void Start()
    {
        uiController.Fader.SetToOpaue();
        uiController.Fader.FadeIn();
        RockController.PrepareRocks();
        LevelReset();
    }

    private void SpawnRock() => RockController.SpawnRock();


    private void LevelReset()
    {
        OnLevelReset?.Invoke();
        RockController.DestroyAllRocks();
        player.ResetEnergy();
        player.Depth = 0f;
        player.transform.position = new Vector3(0f, 0f, 0f);
    }


    private void GameReset()
    {
        MoneyController.Money = PlayerConfig.Instance.startMoney;

        if (player != null)
        {
            player.OnDepthChanged -= DepthController.OnDepthChange;
            Destroy(player.gameObject);

        }


        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        player.OnDepthChanged += DepthController.OnDepthChange;


        if (uiController == null)
        {
            uiController = FindFirstObjectByType<UI_Controller>(FindObjectsInactive.Include);
        }

        uiController.Init(player);

        MoneyController.Money = PlayerConfig.Instance.startMoney;
    }



    public void StartDig()
    {
        player.StartDigging();

        for (int i = 0; i < RockController.CurrentSpawnOption.spawnAmount; i++){
            RockController.SpawnRock();
        }

        rockSpawningCoroutine = StartCoroutine(SpawnRockCoroutine());
    }


    public IEnumerator SpawnRockCoroutine()
     {
        float startDepth = GameController.Instance.player.Depth;
        float endDepth = startDepth + RockController.CurrentSpawnOption.newSpawnEveryXMeters;

        yield return new WaitUntil (() => GameController.Instance.player.Depth >= endDepth);

        for (int i = 0; i < RockController.CurrentSpawnOption.spawnAmount; i++){
            RockController.SpawnRock();
        }

        rockSpawningCoroutine  = StartCoroutine(SpawnRockCoroutine());

    }

    public void EndDig()
    {
        player.EndDigging();

        player.CurrentControl = 0f;

        if (rockSpawningCoroutine != null)
        {
            StopCoroutine(rockSpawningCoroutine);
            rockSpawningCoroutine = null;
        }

        StartCoroutine(EndingDigging());
    }

    private IEnumerator EndingDigging()
    {

        bool canRocketLaunchToSurface = player.CanRocketLaunchToSurface;

        if (canRocketLaunchToSurface)
        {
            float flightTime = player.Depth < 15f ? .5f : 2f;
            StartCoroutine(player.RocketLaunch(flightTime));

            yield return new WaitForSeconds(flightTime);

            LevelReset();

        }
        else
        {
            StartCoroutine(player.RocketLaunch(0.5f));

            yield return new WaitForSeconds(.5f);

            int moneyFine = Mathf.FloorToInt(player.Depth * player.MoneyPerMeter);
            float meters = player.Depth;

            uiController.Fader.FadeOut();
            // StartCoroutine(LoseAnimation(2f));
            yield return new WaitForSeconds(uiController.Fader.FadeDuration + .5f);

            //TODO: Add fadeToBlack effect
            UI_FinePanel finePanle = uiController.ShowFinePanel();
            finePanle.Initialize(moneyFine, meters);
            yield return new WaitUntil(() => finePanle.isClicked);


            if (MoneyController.Money < moneyFine)
            {
                RestartGame();
            }
            else
            {
                MoneyController.Money = MoneyController.Money - moneyFine;
                LevelReset();
                uiController.Fader.FadeIn();
            }

        }



    }

    private IEnumerator LoseAnimation(float animationTime)
    {
        Vector3 startPos = player.transform.position;

        Vector3 endPos;
        if (player.Depth < 15f)
        {
            endPos = new Vector3(startPos.x, 15f - player.Depth, startPos.z);
        }
        else
        {
            endPos = new Vector3(startPos.x, RockController.GetCameraTopEdgeY() + 10f, startPos.z);
        }

        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 1f);
            player.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

    }


    void OnDestroy()
    {
        player.OnDepthChanged -= DepthController.OnDepthChange;

        if (instance == this)
        {
            if (player != null)
            {
                Destroy(player.gameObject); // Destroy the Player object
            }
            instance = null; // Clear the static instance reference
        }
    }

    internal void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameReset();
    }
}
