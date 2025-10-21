using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDropDown : MonoBehaviour
{
    private Camera cam;
    private GameObject heldItem = null;
    public float holdHeight = 1.5f;   // 物品离地高度
    public float gridSize = 2.0f;     // 吸附格子的大小（1表示1x1的方格）

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {

        // 找到场景中所有 Tag = "Counter" 的物体
        GameObject[] counters = GameObject.FindGameObjectsWithTag("Counter");

        foreach (GameObject counter in counters)
        {
            Outline outline = counter.GetComponent<Outline>();
            if (outline != null)
            {
                // 当手里有物品 → 显示描边
                // 当手里没物品 → 隐藏描边
                outline.enabled = (heldItem != null);
            }
        }

        if (Input.GetMouseButtonDown(0)) // 鼠标左键点击
        {
            if (heldItem == null)
            {
                TryPickup();
            }
            else
            {
                TryDrop();
            }
        }

        if (heldItem != null)
        {
            MoveHeldItem();
        }
    }

    void TryPickup()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Item")) // 物品 Tag = "Item"
            {
                heldItem = hit.collider.gameObject;

                // 禁用重力和物理碰撞，避免乱飞
                if (heldItem.GetComponent<Rigidbody>())
                {
                    heldItem.GetComponent<Rigidbody>().useGravity = false;
                    heldItem.GetComponent<Rigidbody>().isKinematic = true;
                }

                Debug.Log("拿起物品: " + heldItem.name);
            }
        }

        heldItem.GetComponent<Collider>().enabled = false;
    }

    void MoveHeldItem()
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0, holdHeight, 0));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 targetPos = ray.GetPoint(distance);
            heldItem.transform.position = targetPos;
        }
    }

    void TryDrop()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Counter")) // 命中货架
            {
                // 获取货架的包围盒
                Bounds shelfBounds = hit.collider.bounds;

                // 先计算吸附位置
                Vector3 dropPos = heldItem.transform.position;
                dropPos.x = Mathf.Round(dropPos.x / gridSize) * gridSize;
                dropPos.z = Mathf.Round(dropPos.z / gridSize) * gridSize;

                // 固定在货架顶面
                float itemHeight = heldItem.GetComponent<Collider>().bounds.size.y;
                dropPos.y = shelfBounds.max.y + itemHeight / 2f;

                // 检查是否在货架范围内 (只判断 XZ 平面即可)
                Vector3 checkPos = dropPos;
                checkPos.y = shelfBounds.center.y; // 忽略高度，只看水平范围

                if (shelfBounds.Contains(checkPos))
                {
                    // 合法放置
                    heldItem.transform.position = dropPos;
                    Debug.Log("物品放置在货架格子: " + dropPos);

                    // 恢复物理
                    if (heldItem.GetComponent<Rigidbody>())
                    {
                        heldItem.GetComponent<Rigidbody>().useGravity = true;
                        heldItem.GetComponent<Rigidbody>().isKinematic = false;
                    }

                    heldItem.GetComponent<Collider>().enabled = true;
                    heldItem = null; // ✅ 物品真正放下
                }
                else
                {
                    // 不在范围内，不放置
                    Debug.Log("超出货架范围，无法放置！");
                    return;
                }
            }
            else
            {
                Debug.Log("不能放在这里！");
            }
        }
    }


}
