using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D map;

    public ColorToPrefab[] colorMappings;
    public List<Vector2> playerSpawnPoints;


    public float divideMultiplicator;

    void Start()
    {
        //GenerateLevel();
    }

    public List<Vector2> GenerateLevel()
    {
        playerSpawnPoints = new List<Vector2>();
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);
            }
        }
        return playerSpawnPoints;
    }

    void GenerateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            return;
        }

        if (colorMappings[0].color.Equals(pixelColor)) // is player position
        {
            Vector2 position = new Vector2(transform.position.x + x / divideMultiplicator, transform.position.y + y / divideMultiplicator);
            playerSpawnPoints.Add(position);
            return;
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
            }
        }
    }
}
