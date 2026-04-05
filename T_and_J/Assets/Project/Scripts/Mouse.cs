using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField]
    GameModeSetting gameModeSetting;
    [SerializeField]
    GameObject mouseCamera;
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Cat")
        {
            //GameModeSettingにネズミと衝突した合図を送る
            gameModeSetting.winCat();
            beEaten();
            Debug.Log("食べられた！");
        }
    }
    public void beEaten()
    {
        Destroy(gameObject);
        Destroy(mouseCamera);
    }
}
