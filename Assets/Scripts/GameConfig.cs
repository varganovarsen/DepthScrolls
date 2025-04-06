using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    private static GameConfig instance;
    public static GameConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameConfig>("GameConfig");
                if (instance == null)
                {
                    Debug.LogError("GameConfig not found");
                }
            }
            return instance;
        }
    }
    [SerializeField] public int startMoney = 100;
    [SerializeField] public SpawnOption[] spawnOptions;
}

[Serializable]
public class SpawnOption
{
    public float minDepth;
    public float newSpawnEveryXMeters = 3f;
    public int spawnAmount = 5;

    public GameObject[] rockPrefabs;
    public float[] rockChanses;
    public GameObject[] rockFillingPrefabs;
    public float[] rockFillingChanses;

}
