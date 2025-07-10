using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject startingChunk;
    [SerializeField] private GameObject endingChunk;
    
    [Space]

    [SerializeField] private GameObject[] chunkPresets;

    [Space]

    [SerializeField] private GameObject goal;

    private GameObject previousChunk;
    private Vector3 spawnPosition;

    private void Start()
    {
        int seed = System.DateTime.UtcNow.Day + System.DateTime.UtcNow.Month * 31 + System.DateTime.UtcNow.Year * 365;

        previousChunk = Instantiate(startingChunk, transform.position, Quaternion.identity);
        spawnPosition = previousChunk.transform.Find("END_NODE").position;

        for (int i = 0; i < 2; i++)
        {
            int index = seed % chunkPresets.Length;

            GameObject chunkToSpawn = chunkPresets[index];
            previousChunk = Instantiate(chunkToSpawn, spawnPosition, Quaternion.identity);

            spawnPosition = previousChunk.transform.Find("END_NODE").position;

            seed *= 5;
        }

        GameObject go = Instantiate(endingChunk, spawnPosition, Quaternion.identity);

        goal.transform.position = go.transform.Find("GOAL_SPAWN").position;
    }
}
