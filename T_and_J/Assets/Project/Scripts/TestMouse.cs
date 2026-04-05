using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestMouse : MonoBehaviour
{
    public Transform cheese;
    private float speed = 1.5f;

    private int cheeseCount = 0;
    public TextMeshProUGUI cheeseText;

    [Header("視界設定")]
    public float viewDistance = 6f; // 視界の距離
    public float viewAngle = 90f;   // 視野角（左右45度）

    void Start()
    {
        UpdateCheeseUI();
    }

    void Update()
    {
        if (cheese == null)
        {
            FindNearestCheese(); // チーズが見つからないときだけ探す
        }

        if (cheese != null)
        {
            Vector3 direction = cheese.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f); // 曲がる早さを調節
            }

            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward;
        Quaternion leftRayRotation = Quaternion.Euler(0, -viewAngle / 2, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, viewAngle / 2, 0);

        Vector3 leftRay = leftRayRotation * forward;
        Vector3 rightRay = rightRayRotation * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftRay * viewDistance);
        Gizmos.DrawRay(transform.position, rightRay * viewDistance);
    }


    void FindNearestCheese()
    {
        GameObject[] cheeses = GameObject.FindGameObjectsWithTag("Cheese");
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject c in cheeses)
        {
            Vector3 toCheese = c.transform.position - transform.position;
            float distance = toCheese.magnitude;

            if (distance > viewDistance) continue; // 視界の距離外

            float angle = Vector3.Angle(transform.forward, toCheese);
            if (angle > viewAngle / 2f) continue; // 視野角外

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = c.transform;
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
