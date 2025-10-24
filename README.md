# 模拟经营类游戏生成随机到访、要求不同的顾客给予不同物品的功能实现

使用unity实现模拟经营游戏中顾客需求识别与对应物品交付的功能开发。

## 游戏展示
![](https://github.com/Airy975/unity-Simulation-Game/blob/main/image/1.png)
![](https://github.com/Airy975/unity-Simulation-Game/blob/main/image/1.png)

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
当两者匹配时，顾客会：显示“收到正确物品”的提示并且销毁被交付的物品。
如果不匹配，则会：显示顾客不需要的反馈UI并且在3秒后自动隐藏UI。