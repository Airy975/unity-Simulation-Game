# 模拟经营类游戏生成随机到访、要求不同的顾客给予不同物品的功能实现

使用unity实现模拟经营游戏中顾客需求识别与对应物品交付的功能开发。

## 游戏展示
![](https://github.com/Airy975/unity-Simulation-Game/blob/main/image/1.png)
![](https://github.com/Airy975/unity-Simulation-Game/blob/main/image/2.png)

## 判断需求不同的顾客的方法
在判断顾客需求的核心逻辑中，主要由 Customer 脚本内的 TryReceiveItem 方法完成。当玩家向顾客交付物品时，会将该物品对象作为参数传入TryReceiveItem方法中。在方法内部，脚本会先获取玩家所持物品的名称，并与顾客自身设定的wantedItem进行对比。
```csharp
public bool TryReceiveItem(GameObject heldItem)
{
    if (heldItem == null) return false;

    string itemName = heldItem.name;

    if (itemName == wantedItem.ToString())
    {
        Debug.Log($"{gameObject.name} 顾客收到了 {wantedItem}！");
        Destroy(heldItem);
        if (feedbackCanvas != null && feedbackCanvas.gameObject.activeSelf)
            feedbackCanvas.gameObject.SetActive(false);
        return true;
    }
    else
    {
        if (feedbackCanvas != null)
        {
            feedbackCanvas.gameObject.SetActive(true);
            StartCoroutine(HideCanvasAfterDelay(3f));
        }
        Debug.Log($"{gameObject.name}：这不是我想要的！");
        return false;
    }
}
```
当两者匹配时，顾客会：显示“收到正确物品”的提示并且销毁被交付的物品。<br>
如果不匹配，则会：显示顾客不需要的反馈UI并且在3秒后自动隐藏UI。

### 顾客生成与行为控制逻辑
在整个顾客系统中，顾客的刷新、移动、停留与离开是相互配合完成的。其中，CreateCustomer脚本负责定时生成顾客，而Customer脚本负责单个顾客的行为控制。

#### 顾客的生成逻辑
顾客的刷新由CreateCustomer脚本控制，其核心思想是每隔一定时间检测空位 → 随机生成顾客 → 分配目标点与参数。
首先遍历spawnedCustomers数组，找到哪些顾客位置为空。
```csharp
if (spawnedCustomers[i] == null)
    freeIndices.Add(i);
```
然后从空位中随机选择一个位置，从顾客预制体列表中随机选择一种顾客类型，并且调用Instantiate()实例化新顾客。
```csharp
int randomSpotIndex = freeIndices[UnityEngine.Random.Range(0, freeIndices.Count)];
GameObject prefab = customerPrefabs[UnityEngine.Random.Range(0, customerPrefabs.Length)];
GameObject newCustomer = Instantiate(prefab, transform.position, Quaternion.identity);
```
之后为新生成的顾客绑定：spawner（生成器引用），spotIndex（当前站位编号）和targetSpot（目标位置）。
```csharp
customerScript.spawner = this;
customerScript.spotIndex = randomSpotIndex;
customerScript.targetSpot = customerSpots[randomSpotIndex];
```
最后当顾客离开时，会触发FreeSpot(int index)，将该位置标记为空，等待下次刷新。
```csharp
public void FreeSpot(int index)
{
    if (index >= 0 && index < spawnedCustomers.Length)
    {
        spawnedCustomers[index] = null;
    }
}
```
#### 顾客的移动与停留逻辑
顾客生成后，Customer脚本中的Update()方法会控制其行为
```csharp
void Update()
{
    if (!reachedSpot && targetSpot != null)
    {
        // 移动到目标点
        transform.position = Vector3.MoveTowards(transform.position, targetSpot.position, moveSpeed * Time.deltaTime);

        // 到达位置
        if (Vector3.Distance(transform.position, targetSpot.position) < 0.05f)
        {
            reachedSpot = true;
            timer = stayTime;
        }
    }
    else if (reachedSpot)
    {
        // 停留倒计时
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Leave();
        }
    }
}
```
这让顾客生成时从CreateCustomer生成位置出现，然后自动朝目标点移动，在到达目标点后，等待几秒，最后停留时间结束后调用Leave()，自动销毁自身并释放占位。

通过这两个脚本的配合，系统能够实现持续、自动、随机的顾客流动效果