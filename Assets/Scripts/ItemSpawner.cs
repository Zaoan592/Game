using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject collectiblePrefab; // 物品的预制体
    public int itemsToSpawn = 5;         // 需要生成的数量

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;  // 所有的出生点候选位置

    void Start()
    {
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        // 1. 防错检查
        if (collectiblePrefab == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Prefab or Spawn Points are missing!");
            return;
        }

        // 2. 确保生成的数量不会超过候选点的总数
        int spawnCount = Mathf.Min(itemsToSpawn, spawnPoints.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            // 3. 随机挑一个点
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Transform selectedPoint = spawnPoints[randomIndex];

            // 4. 在这个点生成物品
            Instantiate(collectiblePrefab, selectedPoint.position, selectedPoint.rotation);

            // 5. 从列表里移除这个点，防止下一个物品也生成在这里（避免物品重叠）
            spawnPoints.RemoveAt(randomIndex);
        }
    }
}