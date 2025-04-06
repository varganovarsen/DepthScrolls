using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public static class RockController
{
    private static List<GameObject> rocks = new List<GameObject>();

    private static GameObject _rockPrefab;
    public const float spawnMinOffsetY = 3f;

    const float CENTER_OFFSET = 2f;

    public static SpawnOption CurrentSpawnOption => GetCurrentSpawnOption();

    public static void PrepareRocks()
    {

        rocks = new List<GameObject>();

    }

    public static void DestroyAllRocks()
    {
        foreach (GameObject rock in rocks)
        {

            GameObject.Destroy(rock);
        }
    }


    public static void SpawnRock()
    {
        Vector2 edges = GetCameraBottomEdgePosition();
        float rnd = Random.value;
        float spawnX = 0f;
        if (rnd < 0.5f)
        {
            spawnX = Random.Range(edges.x, 0 - CENTER_OFFSET);
        }
        else
        {
            spawnX = Random.Range(0 + CENTER_OFFSET, edges.y);
        }

        float spawnY = Camera.main.transform.position.y - Camera.main.orthographicSize - Random.Range(spawnMinOffsetY, CurrentSpawnOption.newSpawnEveryXMeters);

        GameObject rock = GameObject.Instantiate(GetRandomRockPrefab(), new Vector2(spawnX, spawnY), Quaternion.identity);
        rock.GetComponent<Rock>().depth = GameController.Instance.player.Depth - spawnY;
        rock.GetComponent<Rock>().SetUp();
        rocks.Add(rock);


    }



    private static GameObject GetRandomRockPrefab()
    {
        SpawnOption currentOption = CurrentSpawnOption;
        float rnd = Random.value;
        float sum = 0f;
        for (int i = 0; i < currentOption.rockPrefabs.Length; i++)
        {
            sum += currentOption.rockChanses[i];

            if (rnd <= sum)
            {
                return currentOption.rockPrefabs[i];
            }
        }

        return currentOption.rockPrefabs[0];
    }

    static Vector2 GetCameraBottomEdgePosition()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Vector2 bottomCenter = (Vector2)cam.transform.position - new Vector2(0, halfHeight);
        float leftEdge = bottomCenter.x - halfWidth;
        float rightEdge = bottomCenter.x + halfWidth;

        return new Vector2(leftEdge, rightEdge);
    }

    public static float GetCameraTopEdgeY()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;

        Vector2 topCenter = (Vector2)cam.transform.position + new Vector2(0, halfHeight);

        return topCenter.y;
    }



    private static SpawnOption GetCurrentSpawnOption()
    {

        SpawnOption currentOption = null;
        float currentDepth = GameController.Instance.player.Depth;
        for (int i = 0; i < GameConfig.Instance.spawnOptions.Length; i++)
        {
            if (i == GameConfig.Instance.spawnOptions.Length - 1)
            {
                return GameConfig.Instance.spawnOptions[i];
            }

            if (currentDepth >= GameConfig.Instance.spawnOptions[i].minDepth && currentDepth < GameConfig.Instance.spawnOptions[i + 1].minDepth)
            {
                return GameConfig.Instance.spawnOptions[i];
            }

        }

        return currentOption;
    }
}
