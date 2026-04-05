using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private bool isPressed = false;

    [SerializeField]
    GameObject buttons;
    [SerializeField]
    GameObject inputFields;
    [SerializeField]
    GameObject errorPanel;
    [SerializeField]
    TextMeshProUGUI errorText;

    float timeS, timeE;
    bool isError = false;

    // Start is called before the first frame update
    void Start()
    {
        buttons.SetActive(true);
        inputFields.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeE = GameManager.instance.timer;

        if (timeE > timeS + 3)
        {
            errorPanel.SetActive(false);
            isError = false;
        }
    }

    public void pressStart()
    {
        if (!isPressed)
        {
            errorText.text = null;
            if (GameManager.instance.mapSize < 3)
            {
                errorText.text = "Map size is too small.";
                showError();
            }
            else if (GameManager.instance.mapSize > 50)
            {
                errorText.text = "Map size is too large.";
                showError();
            }
            if (GameManager.instance.seed <= 0)
            {
                if (errorText.text != null) errorText.text += "\n";
                errorText.text += "The seed must be greater than or equal to 1.";
                showError();
            }
            if (!isError)
            {
                isPressed = true;
                Debug.Log("start!");
                //シーン遷移
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    public void pressMode(int gameMode)
    {
        GameManager.instance.gameMode = gameMode;
        buttons.SetActive(false);
        inputFields.SetActive(true);
    }

    public void pressGoBack()
    {
        buttons.SetActive(true);
        inputFields.SetActive(false);
    }

    void showError()
    {
        errorPanel.SetActive(true);
        isError = true;
        timeS = GameManager.instance.timer;
    }
}
