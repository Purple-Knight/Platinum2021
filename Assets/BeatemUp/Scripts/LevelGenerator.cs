using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<Texture2D> map;
    int currentMap = 0;

    public ColorToPrefab[] colorMappings;
    public List<Vector2> playerSpawnPoints;

    public float divideMultiplicator;

    public float spawnOffset;
    List<GameObject> currentObjectInLevel;

    private void Awake()
    {
        currentObjectInLevel = new List<GameObject>();
    }
    public List<Vector2> GenerateLevel()
    {
        int i=0;
        do
        {
            i = Random.Range(0, map.Count);
        } while (i == currentMap);
        currentMap = i;
        transform.position = new Vector2(-map[currentMap].width / 2f + spawnOffset, -map[currentMap].height / 2f  +spawnOffset);
        playerSpawnPoints = new List<Vector2>();

        if(currentObjectInLevel.Count !=0)
        {
            for (int j = currentObjectInLevel.Count -1; j > 0; j--)
            {
                Destroy(currentObjectInLevel[j]);
            }
            currentObjectInLevel.Clear();
        }
        for (int x = 0; x < map[currentMap].width; x++)
        {
            for (int y = 0; y < map[currentMap].height; y++)
            {
               GameObject obj = GenerateTile(x, y);
                if(obj != null)
                {
                    currentObjectInLevel.Add(obj);
                }
            }
        }
        return playerSpawnPoints;
    }

    GameObject GenerateTile(int x, int y)
    {
        Color pixelColor = map[currentMap].GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            return null;
        }

        if (colorMappings[0].color.Equals(pixelColor)) // is player position
        {
            Vector2 position = new Vector2(transform.position.x + x / divideMultiplicator, transform.position.y + y / divideMultiplicator);
            playerSpawnPoints.Add(position);
            return null;
        }

        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(transform.position.x + x / divideMultiplicator, transform.position.y + y / divideMultiplicator);
                var block = Instantiate(colorMapping.prefab, transform.position, Quaternion.identity);
                block.transform.parent = transform;
                block.transform.position = position;
                block.name = colorMapping.name + " " + x + " " + y;
                return block;
            }
        }
        return null;
    }
}
