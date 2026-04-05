using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ← UIを使うために必要

public class MouseMovement : MonoBehaviour
{
    public Transform cheese;
    private float speed = 1.5f;

    private int cheeseCount = 0; // ← チーズの数をカウント
    public TextMeshProUGUI cheeseText;      // ← UIへの参照（Inspectorで設定）

    void Start()
    {
        UpdateCheeseUI();
    }

    void Update()
    {
        if (cheese == null)
        {
            FindNearestCheese();
        }

        if (cheese != null)
        {
            Vector3 direction = cheese.position - transform.position;
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void FindNearestCheese()
    {
        GameObject[] cheeses = GameObject.FindGameObjectsWithTag("Cheese");
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject cheese in cheeses)
        {
            float distance = Vector3.Distance(transform.position, cheese.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = cheese.transform;
            }
        }

        cheese = closest;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cheese"))
        {
            Destroy(other.gameObject);
            cheeseCount++;
            UpdateCheeseUI();
            cheese = null;
        }
    }

    void UpdateCheeseUI()
    {
        if (cheeseText != null)
        {
            cheeseText.text = "Cheese: " + cheeseCount.ToString();
        }
    }
}