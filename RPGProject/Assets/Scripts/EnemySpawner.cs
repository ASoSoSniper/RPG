using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Tilemap floorMap;
    [SerializeField] Tilemap wallMap;

    PlayerMovement player;
    [SerializeField] float playerRadius = 20f;

    [SerializeField] GameObject enemyPrefab;
    GameObject enemySpawn;
    [SerializeField] float spawnTime = 5f;
    float currSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap[] foundMaps = FindObjectsOfType<Tilemap>();
        foreach (Tilemap map in foundMaps)
        {
            if (map.gameObject.name == "Floor")
            {
                floorMap = map;
            }
            else if (map.gameObject.name == "Walls")
            {
                wallMap = map;
            }
        }

        player = FindObjectOfType<PlayerMovement>();
        currSpawnTime = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        currSpawnTime -= Time.deltaTime;
        if (currSpawnTime <= 0f)
        {
            currSpawnTime = spawnTime;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemySpawn) Destroy(enemySpawn);

        List<Vector3Int> tilesNearPlayer = FindNodesNearPlayer();

        int rand = Random.Range(0, tilesNearPlayer.Count - 1);

        enemySpawn = Instantiate(enemyPrefab);
        enemySpawn.transform.position = floorMap.GetCellCenterWorld(tilesNearPlayer[rand]);
    }

    List<Vector3Int> FindNodesNearPlayer()
    {
        List<Vector3Int> foundNodes = new List<Vector3Int>();

        //Get grid position of player
        Vector3Int playerGridPos = floorMap.WorldToCell(player.transform.position);
        //Get center world position of center of grid cell
        Vector3 worldPosition = floorMap.GetCellCenterWorld(playerGridPos);
        //Get the bounds of the grid
        BoundsInt bounds = floorMap.cellBounds;

        //Scan the entire grid
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                //Get cell position
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                if (playerGridPos == cellPosition) continue;

                //Check for a wall tile at this position, continue if one is found
                TileBase wall = wallMap.GetTile(cellPosition);
                if (wall) continue;

                //Get Tile at cell position
                TileBase tile = floorMap.GetTile(cellPosition);

                //If tile exists:
                if (tile != null)
                {
                    //Get cell world position
                    Vector3 cellWorldPosition = floorMap.GetCellCenterWorld(cellPosition);

                    //Compare cell world position to player world position, and add to list if with radius
                    if (Vector3.Distance(cellWorldPosition, worldPosition) <= playerRadius)
                    {
                        foundNodes.Add(cellPosition);
                    }
                }
            }
        }
        //Debug.Log(foundNodes.Count + " Cells found!");
        return foundNodes;
    }
}
