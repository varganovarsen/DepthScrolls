using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Rendering;


public static class RockController
{
    const int ROCK_COUNT = 20;
    const string ROCK_PREFAB_PATH = "Prefabs/Rock";
    private static List<GameObject> rocks = new List<GameObject>();

    private static GameObject _rockPrefab;
    public const float spawnMinOffsetY = 3f;
    const float spawnMaxOffsetY = 20f;

    const float CENTER_OFFSET = 5f;

    private static GameObject rockPrefab
    {
        get
        {
            if (_rockPrefab == null)
            {
                _rockPrefab = Resources.Load<GameObject>(ROCK_PREFAB_PATH);
            }
            return _rockPrefab;
        }
    }

    public static void PrepareRocks()
    {
        for (int i = 0; i < ROCK_COUNT; i++)
        {
            GameObject rockObject = GameObject.Instantiate(rockPrefab);
            rockObject.SetActive(false);
            rocks.Add(rockObject);
        }
    }


    public static void SpawnRock()
    {
        Vector2 edges = GetCameraBottomEdgePosition();
        float spawnX = Random.Range(edges.x, edges.y);

        if(spawnX < CENTER_OFFSET && spawnX >= 0)
            spawnX += CENTER_OFFSET;

        if (spawnX <= 0 && spawnX > -CENTER_OFFSET)
            spawnX -= CENTER_OFFSET;
        
        float spawnY = Camera.main.transform.position.y - Camera.main.orthographicSize - Random.Range(spawnMinOffsetY, spawnMaxOffsetY);

        GameObject rock = GetInactiveRock();
        if (rock != null)
        {
            rock.transform.position = new Vector2(spawnX, spawnY);
            rock.GetComponent<Rock>().depth = PlayerController.Depth - spawnY;
            rock.SetActive(true);
        }
    }

    static GameObject GetInactiveRock()
    {
        foreach (GameObject rock in rocks)
        {
            if (!rock.activeInHierarchy) return rock;
        }

        return null;
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
}
