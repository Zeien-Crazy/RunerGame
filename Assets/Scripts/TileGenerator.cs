using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    private List<GameObject> activityTiles = new List<GameObject>();
    private float spawnPos = 1;
    private float tileLength = 4;

    [SerializeField] private Transform player;
    [SerializeField] private int startTiles = 6;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startTiles; i++)
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.z + 8 < spawnPos + (startTiles * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
            DeleteTile();
        }
    }

    private void SpawnTile(int tileIndex)
    {
        GameObject nextTile = Instantiate(tilePrefabs[tileIndex], transform.forward * spawnPos, transform.rotation);
        activityTiles.Add(nextTile);
        spawnPos -= tileLength;
    }

    private void DeleteTile()
    {
        Destroy(activityTiles[0]);
        activityTiles.RemoveAt(0);
    }
}
