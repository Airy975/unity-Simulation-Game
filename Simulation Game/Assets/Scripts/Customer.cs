using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [HideInInspector] public CreateCustomer spawner;
    [HideInInspector] public int spotIndex;
    [HideInInspector] public Transform targetSpot;

    public float moveSpeed = 2f;   // 移动速度
    public float stayTime = 5f;    // 停留时间

    private bool reachedSpot = false;
    private float timer;
    private Canvas feedbackCanvas;

    // 定义一个枚举，下拉菜单会显示三个选项
    public enum WantedItem
    {
        书1,
        书2,
        书3
    }

    // 在 Inspector 中会显示下拉选项
    public WantedItem wantedItem;

    public bool TryReceiveItem(GameObject heldItem)
    {
        if (heldItem == null) return false;

        // 拿到物品的名字
        string itemName = heldItem.name;

        // 检查物品名字是否与顾客想要的物品名字相同
        if (itemName == wantedItem.ToString())
        {
            Debug.Log($"{gameObject.name} 顾客收到了 {wantedItem}！");
            Destroy(heldItem);
            // 如果物品匹配，则显示隐藏的 Canvas
            if (feedbackCanvas != null && feedbackCanvas.gameObject.activeSelf)
            {
                feedbackCanvas.gameObject.SetActive(false); // 将其隐藏
            }
            return true;
        }
        else
        {
            // 如果物品不匹配，则显示隐藏的 Canvas
            if (feedbackCanvas != null)
            {
                feedbackCanvas.gameObject.SetActive(true);
                StartCoroutine(HideCanvasAfterDelay(3f));
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} 没有找到提示 Canvas！");
            }

            Debug.Log($"{gameObject.name}：这不是我想要的！");
            return false;
        }
    }

    private IEnumerator HideCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackCanvas != null)
        {
            feedbackCanvas.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        timer = stayTime;

        // 自动查找顾客预制体下的隐藏 Canvas（包括未激活的）
        feedbackCanvas = GetComponentInChildren<Canvas>(true);

        if (feedbackCanvas != null)
        {
            // 确保初始状态下是隐藏的
            feedbackCanvas.gameObject.SetActive(false);
        }
    }

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

    void Leave()
    {
        if (spawner != null)
        {
            spawner.FreeSpot(spotIndex);
        }
        Destroy(gameObject);
    }
}




