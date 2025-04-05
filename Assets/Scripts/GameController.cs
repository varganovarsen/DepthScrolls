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


    public PlayerController player;
    public DepthController depthController;
    private bool isInit;

    public static GameController Instance
    {
        get
        {
            if(instance == null){
                GameObject controllerPrefab = Resources.Load(GAME_CONTROLLER_PREFAB_PATH) as GameObject;
                instance = GameObject.Instantiate(controllerPrefab).GetComponent<GameController>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
        GameObject depthControllerObject = new GameObject("DepthController");
        depthController = depthControllerObject.AddComponent<DepthController>();
        depthControllerObject.transform.SetParent(this.transform);


        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        PlayerController.MaxEnergy = 1000;
        PlayerController.EnergyUsePerSecond = 5f;
        player.ResetEnergy();
    }

    private void Start()
    {
        RockController.PrepareRocks();
    }

    private void SpawnRock() => RockController.SpawnRock();

    



    public void StartDig(){
        player.StartDigging();

        SpawnRock();
        SpawnRock();
        SpawnRock();

        InvokeRepeating(nameof(SpawnRock), 0f, 3f);
    }

    public void EndDig(){
        player.EndDigging();

        if(PlayerController.Depth < 15f){
            StartCoroutine(RestartDig(.5f));
        } else{
            StartCoroutine(RestartDig(2f));
        }

        player.CurrentControl = 0f;
    }

    private IEnumerator RestartDig(float timeToRestart){
        
        float startDepth = PlayerController.Depth;
        float endDepth = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < timeToRestart){
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / timeToRestart);
            PlayerController.Depth = Mathf.Lerp(startDepth, endDepth, t);
            yield return null;
        }

        PlayerController.Depth = endDepth;

        player.ResetEnergy();
        
    }
}
