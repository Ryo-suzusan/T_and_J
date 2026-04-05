using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheese : MonoBehaviour
{
    private float cheesePosY;
    private float cheeseRotY;
    bool wasGot = false;

    void Start()
    {
        cheesePosY = this.transform.position.y;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, cheesePosY + Mathf.PingPong(Time.time / 3, 0.1f), transform.position.z);
        transform.Rotate(0, 1, 0, Space.World); 
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Mouse" && !wasGot)
        {
            wasGot = true;
            GameManager.instance.getCheese();
            Destroy(gameObject);
        }
    }
}
