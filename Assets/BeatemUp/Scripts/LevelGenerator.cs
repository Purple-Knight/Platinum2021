using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> mapZone1;
    public List<GameObject> mapZone2;
    public List<GameObject> maps;
    [SerializeField] float positionOffset = -1;
    //public List<Texture2D> map;
    int currentMap = 0;

    public ColorToPrefab[] colorMappings;
    public List<Vector2> playerSpawnPoints;

    public float divideMultiplicator;

    public float spawnOffset;
    GameObject currentLevel;
    //List<GameObject> currentObjectInLevel;

    Vector2 gridSize = new Vector2(1, 0.5f);

    //[SerializeField] List<GameObject> groundPrefabs;

    private void Awake()
    {
        playerSpawnPoints = new List<Vector2>();
        //currentObjectInLevel = new List<GameObject>();
        maps = new List<GameObject>();
        if (RhythmManager.Instance.bpm == BPM.BPM115)
        {
            for (int i = 0; i < mapZone1.Count; i++)
            {
                maps.Add(mapZone1[i]);
            }

        }
        else
        {
            for (int i = 0; i < mapZone2.Count; i++)
            {
                maps.Add(mapZone2[i]);
            }
        }
    }

    private void Start()
    {
        
    }

    public List<Vector2> SpawnNextMap()
    {
        playerSpawnPoints.Clear();
        if(currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
        }
        int i = 0;
        do
        {
            i = Random.Range(0, maps.Count);

         } while (currentMap == i);
        currentMap = i;
        currentLevel = Instantiate(maps[i], transform);
        MapManager manager = currentLevel.GetComponent<MapManager>();
        transform.position = new Vector3((-manager.mapSize.x * gridSize.x) / 2f + positionOffset, (manager.mapSize.y * gridSize.y) / 2f  +positionOffset,0) ;
        foreach (Transform pos in manager.playerSpawnPoints)
        {
            playerSpawnPoints.Add(pos.position);
        }
       return playerSpawnPoints;

    }
    /*public List<Vector2> SpawnNextMap()
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
*/
   /* GameObject GenerateTile(int x, int y)
    {
        Color pixelColor = map[currentMap].GetPixel(x, y);
        //Debug.Log(pixelColor);
        if (pixelColor.a == 0)
        {
            *//*Vector2 position = new Vector2((transform.position.x + x / divideMultiplicator) * gridSize.x, (transform.position.y + y / divideMultiplicator) * gridSize.y);
            var block = Instantiate(colorMappings[2].prefab, transform.position, Quaternion.identity);
            block.transform.parent = transform;
            block.transform.position = position;
            block.name = colorMappings[2].name + " " + x + " " + y;
            return block;*//*
            return null;
        }

        if (colorMappings[0].color.Equals(pixelColor)) // is player position
        {
            Vector2 position = new Vector2((transform.position.x + x / divideMultiplicator) * gridSize.x, (transform.position.y + y / divideMultiplicator) * gridSize.y);
            playerSpawnPoints.Add(position);
            *//*var block = Instantiate(colorMappings[2].prefab, transform.position, Quaternion.identity);
            block.transform.parent = transform;
            block.transform.position = position;
            block.name = colorMappings[2].name + " " + x + " " + y;
            return block;*//*
            return null;
        }

        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                int i = 0;
                if(y != map[currentMap].height -1)
                {
                    i = Random.Range(1, groundPrefabs.Count);
                }
                Vector2 position = new Vector2((transform.position.x + x / divideMultiplicator) * gridSize.x, (transform.position.y + y / divideMultiplicator) * gridSize.y);
                var block = Instantiate(*//*colorMapping.prefab*//* groundPrefabs[i], transform.position, Quaternion.identity);
                block.transform.parent = transform;
                block.transform.position = position;
                block.name = colorMapping.name + " " + x + " " + y;
                return block;
            }
        }
        return null;
    }*/
}
