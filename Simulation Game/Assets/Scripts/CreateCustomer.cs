using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateCustomer : MonoBehaviour
{
    [Header("顾客预制体")]
    public GameObject[] customerPrefabs;

    [Header("顾客目标位置")]
    public Transform[] customerSpots;

    [Header("生成间隔时间")]
    public float spawnInterval = 3f;

    // 内部状态
    private GameObject[] spawnedCustomers;
    private float timer;

    void Start()
    {
        spawnedCustomers = new GameObject[customerSpots.Length];
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // 当计时器归零时尝试生成顾客
        if (timer <= 0f)
        {
            TrySpawnCustomer();
            timer = spawnInterval; // 重置计时器
        }
    }

    void TrySpawnCustomer()
    {
        // 检查是否有空位
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < spawnedCustomers.Length; i++)
        {
            if (spawnedCustomers[i] == null) // 位置为空
            {
                freeIndices.Add(i);
            }
        }

        // 没有空位就直接返回
        if (freeIndices.Count == 0) return;

        // 随机选择一个空位置
        int randomSpotIndex = freeIndices[UnityEngine.Random.Range(0, freeIndices.Count)];

        // 随机选择一个顾客预制体
        GameObject prefab = customerPrefabs[UnityEngine.Random.Range(0, customerPrefabs.Length)];

        // 生成顾客
        GameObject newCustomer = Instantiate(prefab, transform.position, Quaternion.identity);

        // 保存生成的顾客引用
        spawnedCustomers[randomSpotIndex] = newCustomer;

        // 设置顾客的目标位置
        Customer customerScript = newCustomer.GetComponent<Customer>();
        if (customerScript != null)
        {
            customerScript.spawner = this;
            customerScript.spotIndex = randomSpotIndex;
            customerScript.targetSpot = customerSpots[randomSpotIndex];
        }
    }

    // 当顾客离开时释放位置
    public void FreeSpot(int index)
    {
        if (index >= 0 && index < spawnedCustomers.Length)
        {
            spawnedCustomers[index] = null;
        }
    }
}
