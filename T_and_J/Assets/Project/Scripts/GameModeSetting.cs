using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSetting : MonoBehaviour
{
    [SerializeField]
    GameObject catPrefab;
    [SerializeField]
    GameObject mousePrefab;
    [SerializeField]
    GameObject catCamera;
    [SerializeField]
    GameObject mouseCamera;
    [SerializeField]
    GameObject catSPCamera;
    [SerializeField]
    GameObject mouseSPCamera;
    [SerializeField]
    GameObject catMouse;

    private CatFinish catFinish;

    // Start is called before the first frame update
    void Start()
    {
        catCamera.SetActive(false);
        mouseCamera.SetActive(false);
        switch (GameManager.instance.gameMode)
        {
            case 0:
                catPrefab.GetComponent<Cat>().enabled = true;
                mousePrefab.GetComponent<ObstacleAvoidance>().enabled = true;
                mousePrefab.GetComponent<CapsuleCollider>().enabled = false;
                catPrefab.GetComponent<CapsuleCollider>().enabled = false;
                break;
            case 1:
                catPrefab.GetComponent<PlayerController>().enabled = true;
                mousePrefab.GetComponent<ObstacleAvoidance>().enabled = true;
                catPrefab.GetComponent<CapsuleCollider>().enabled = true;
                mousePrefab.GetComponent<CapsuleCollider>().enabled = false;
                catCamera.SetActive(true);
                break;
            case 2:
                catPrefab.GetComponent<Cat>().enabled = true;
                mousePrefab.GetComponent<PlayerController>().enabled = true;
                mousePrefab.GetComponent<ObstacleAvoidance>().enabled = false;
                catPrefab.GetComponent<CapsuleCollider>().enabled = false;
                mousePrefab.GetComponent<CapsuleCollider>().enabled = true;
                mouseCamera.SetActive(true);
                break;
        }

        catFinish = catPrefab.GetComponent<CatFinish>();
    }

    private void Update()
    {
        if (GameManager.instance.allCollected)
        {
            catFinish.beCollected();
            if (GameManager.instance.gameMode != 2)
            {
                mouseSPCamera.SetActive(true);
            }
            GameManager.instance.isFinished = true;
        }
    }

    // Update is called once per frame
    public void winCat()
    {
        catMouse.SetActive(true);
        if (GameManager.instance.gameMode != 1)
        {
            catSPCamera.SetActive(true);
        }
        GameManager.instance.isFinished = true;
    }
}
