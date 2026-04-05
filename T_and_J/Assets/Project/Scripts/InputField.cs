using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputField : MonoBehaviour
{
    //�I�u�W�F�N�g�ƌ��т���
    TMP_InputField inputField;
    public string type;

    void Start()
    {
        //Component�������悤�ɂ���
        inputField = GetComponent<TMP_InputField>();

        if (type == "MapSize")
        {
            inputField.text = GameManager.instance.mapSize.ToString();
            Debug.Log(GameManager.instance.mapSize.ToString());
        }
        if (type == "Seed")
        {
            Debug.Log(GameManager.instance.seed.ToString());
            inputField.text = GameManager.instance.seed.ToString();
        }
    }

    public void Input(string st)
    {
        if (st == "MapSize")
        {
            GameManager.instance.mapSize = int.Parse(inputField.text);
        }
        if (st == "Seed")
        {
            GameManager.instance.seed = int.Parse(inputField.text);
        }
    }
}