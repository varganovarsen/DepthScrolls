using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{

    const string GAME_CONTROLLER_PREFAB_PATH = "Prefabs/GameController";
    private static GameController instance;


    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject guiPrefab;

    public PlayerController player;
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
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
    }

    public void StartDig(){
        player.StartDigging();
    }

    public void EndDig(){
        player.EndDigging();

        if(player.Depth < 15f){
            StartCoroutine(RestartDig(.5f));
        } else{
            StartCoroutine(RestartDig(2f));
        }

        player.CurrentControl = 0f;
    }

    private IEnumerator RestartDig(float timeToRestart){
        
        float startDepth = player.Depth;
        float endDepth = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < timeToRestart){
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / timeToRestart);
            player.Depth = Mathf.Lerp(startDepth, endDepth, t);
            yield return null;
        }
        
    }
}
