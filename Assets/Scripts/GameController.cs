using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{

    const string GAME_CONTROLLER_PREFAB_PATH = "Prefabs/GameController";
    private static GameController instance;


    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject guiPrefab;

    PlayerController player;

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

    void Start()
    {
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
    }

    public void StartDig(){
        
    }
}
