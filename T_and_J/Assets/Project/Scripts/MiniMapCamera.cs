using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField]
    FieldGenerator fieldGenerator;
    [Header("カメラのサイズ")]
    float sizeMultiplier = 0.5f;

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.gameMode == 0)
        {
            gameObject.SetActive(true);
            _camera = GetComponent<Camera>();
            AdjustCamera();
        }
        else gameObject.SetActive(false);
    }

    // Update is called once per frame
    void AdjustCamera()
    {
        int mapSize = fieldGenerator.mapSize;

        // マップの中央を計算（FieldGeneratorと同じ l = 0.5f）
        float l = 0.5f;
        Vector3 centerPos = new Vector3(mapSize * l / 2f, 10, mapSize * l / 2f);

        // カメラをマップの真上に配置
        transform.position = centerPos;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 真上から見下ろす

        // Orthographicサイズをマップサイズに応じて調整
        if (_camera.orthographic)
        {
            _camera.orthographicSize = mapSize * l * sizeMultiplier;
        }
        else
        {
            _camera.fieldOfView = mapSize * l * sizeMultiplier　* 10;
        }
    }
}
