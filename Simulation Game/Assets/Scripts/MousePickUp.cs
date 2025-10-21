using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePickUp : MonoBehaviour
{
    private GameObject heldItem;
    public float holdDistance = 2f; // 拾取时物品与相机的距离

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 获取所有命中的物体
            RaycastHit[] hits = Physics.RaycastAll(ray, 5f);

            // 没有命中任何物体
            if (hits.Length == 0)
            {
                return;
            }
            else
            {
                // 遍历所有命中的物体
                foreach (var hit in hits)
                {
                    // 1. 没拿东西，射线命中物品 → 点击物品拾取
                    if (heldItem == null && hit.collider.CompareTag("Item"))
                    {
                        PickUp(hit.collider.gameObject);
                        return;
                    }

                    // 2. 正在拿东西 → 点击顾客尝试交付
                    if (heldItem != null && hit.collider.CompareTag("Customer"))
                    {
                        if (hit.collider.TryGetComponent(out Customer customer))
                        {
                            if (customer.TryReceiveItem(heldItem))
                            {
                                heldItem = null; // 交付成功
                            }
                            break; // 找到顾客就结束循环
                        }
                    }
                    
                }
            }
        }

        if (heldItem != null && Input.GetMouseButtonDown(1))
        {
            Destroy(heldItem);
            heldItem = null;
        }

        // 让物品跟随鼠标
        if (heldItem != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = holdDistance;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            heldItem.transform.position = worldPos;
        }
    }

    private void PickUp(GameObject item)
    {
        // 复制物品
        Vector3 originalPos = item.transform.position;
        Quaternion originalRot = item.transform.rotation;

        GameObject clone = Instantiate(item, originalPos, originalRot);
        clone.name = item.name;

        // 拾取物品
        heldItem = item;
        if (heldItem.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        // 忽略物理碰撞
        Collider heldCollider = heldItem.GetComponent<Collider>();
        if (heldCollider != null)
        {
            foreach (var otherItem in GameObject.FindGameObjectsWithTag("Item"))
            {
                if (otherItem == heldItem) continue; // 忽略自己
                Collider otherCollider = otherItem.GetComponent<Collider>();
                if (otherCollider != null)
                    Physics.IgnoreCollision(heldCollider, otherCollider, true);
            }
        }
    }
}

