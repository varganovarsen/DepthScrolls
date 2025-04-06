using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

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

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject controllerPrefab = Resources.Load(GAME_CONTROLLER_PREFAB_PATH) as GameObject;
                instance = GameObject.Instantiate(controllerPrefab).GetComponent<GameController>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
        PlayerController.OnDepthChanged += DepthController.OnDepthChange;

        uiController = FindFirstObjectByType<UI_Controller>();
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
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
        RockController.DestroyAllRocks();
        player.ResetEnergy();
        PlayerController.Depth = 0f;
        player.transform.position = new Vector3(0f, 0f, 0f);
    }


    public void StartDig()
    {
        player.StartDigging();

        SpawnRock();
        SpawnRock();
        SpawnRock();

        rockSpawningCoroutine = StartCoroutine(RockSpawning());
    }

    private IEnumerator RockSpawning()
    {
        float timeToSpawn = Random.Range(2f, 5f);
        yield return new WaitForSeconds(timeToSpawn);

        SpawnRock();

        rockSpawningCoroutine = StartCoroutine(RockSpawning());

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
            float flightTime = PlayerController.Depth < 15f ? .5f : 2f;
            StartCoroutine(player.RocketLaunch(flightTime));

            yield return new WaitForSeconds(flightTime);



        }
        else
        {
            StartCoroutine(player.RocketLaunch(0.5f));
        
            yield return new WaitForSeconds(.5f);


            uiController.Fader.FadeOut();
            // StartCoroutine(LoseAnimation(2f));
            yield return new WaitForSeconds(uiController.Fader.FadeDuration + .5f);

            uiController.Fader.FadeIn();
            //TODO: Add fadeToBlack effect
        }


        LevelReset();
    }

    private IEnumerator LoseAnimation(float animationTime)
    {
        Vector3 startPos = player.transform.position;

        Vector3 endPos;
        if (PlayerController.Depth < 15f)
        {
            endPos = new Vector3(startPos.x, 15f - PlayerController.Depth, startPos.z);
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
        PlayerController.OnDepthChanged -= DepthController.OnDepthChange;

        if (instance == this)
        {
            if (player != null)
            {
                Destroy(player.gameObject); // Destroy the Player object
            }
            instance = null; // Clear the static instance reference
        }
    }
}
