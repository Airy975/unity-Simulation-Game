using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    public Transform holdPoint;
    private GameObject heldItem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                // 1. 如果没拿东西，尝试拾取物品
                if (heldItem == null && hit.collider.CompareTag("Item"))
                {
                    PickUp(hit.collider.gameObject);
                }
                // 2. 如果拿着东西，尝试交给顾客
                else if (heldItem != null && hit.collider.TryGetComponent(out Customer customer))
                {
                    if (customer.TryReceiveItem(heldItem))
                    {
                        heldItem = null; // 物品已交付
                    }
                }
            }
        }

        // 让物体跟随手中位置
        if (heldItem != null)
        {
            heldItem.transform.position = holdPoint.position;
        }
    }

    void PickUp(GameObject item)
    {
        heldItem = item;
        heldItem.transform.SetParent(holdPoint);
        heldItem.transform.localPosition = Vector3.zero;

        if (heldItem.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;
    }
}

