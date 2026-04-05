using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    [SerializeField]
    FieldGenerator fieldGenerator;
    [Header("カメラのサイズ")]
    float sizeMultiplier = 0.5f;
    [SerializeField]
    GameObject mouseCamera;
    [SerializeField]
    GameObject catCamera;

    private Camera _camera;
    private int cameraAngle = 3;
    Vector3 centerPos;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        AdjustCamera();
    }

    // Update is called once per frame
    void AdjustCamera()
    {
        int mapSize = fieldGenerator.mapSize;

        // マップの中央を計算（FieldGeneratorと同じ l = 0.5f）
        float l = 0.5f;
        centerPos = new Vector3(mapSize * l / 2f, 10, mapSize * l / 2f);

        // カメラをマップの真上に配置
        changeCamera();

        // Orthographicサイズをマップサイズに応じて調整
        if (_camera.orthographic)
        {
            _camera.orthographicSize = mapSize * l * sizeMultiplier;
        }
        else
        {
            _camera.fieldOfView = mapSize * l * sizeMultiplier * 10;
        }
    }

    public void changeCamera()
    {
        cameraAngle = (cameraAngle + 1) % 4;

        switch (cameraAngle)
        {
            case 0:
                mouseCamera.SetActive(false);
                gameObject.SetActive(true);
                transform.position = centerPos;
                transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 真上から見下ろす
                break;
            case 1:
                transform.position = new Vector3(centerPos.x, 4.6f, -4.56f);
                transform.rotation = Quaternion.Euler(30f, 0f, 0f); // 横から見下ろす
                break;
            case 2:
                gameObject.SetActive(false);
                catCamera.SetActive(true);
                break;
            case 3:
                catCamera.SetActive(false);
                mouseCamera.SetActive(true);
                break;
        }
    }
}
