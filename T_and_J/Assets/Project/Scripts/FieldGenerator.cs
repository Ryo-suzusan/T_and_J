using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [Header("マップサイズ(奇数)")]
    public int mapSize;
    [Header("シード")]
    public int seed;

    [SerializeField]
    private GameObject groundPrefab;
    [SerializeField]
    private GameObject[] wallPrefabs;
    [SerializeField]
    private GameObject[] furniturePrefabsS;
    [SerializeField]
    private GameObject[] furniturePrefabsL;
    [SerializeField] 
    private GameObject ceilingPrefab;
    [SerializeField]
    private GameObject lightPrefab;
    [SerializeField] 
    private GameObject cheesePrefab;
    [SerializeField] 
    public Cat cat;
    //20250522追加
    public string newTag = "Obstacle";
    public int newLayer = 6;

    private int[] dx = { 1, 0, -1, 0 };
    private int[] dy = { 0, 1, 0, -1 };

    [SerializeField]
    GameObject catPrefab;
    [SerializeField]
    GameObject mousePrefab;
    [SerializeField]
    GameObject catCamera;
    [SerializeField]
    GameObject mouseCamera;

    //障害物の一片の長さl
    float l = 0.5f;

    private void Awake()
    {
        // GameManagerからmapSizeとseedを受け取る！！

        mapSize = GameManager.instance.mapSize;
        seed = GameManager.instance.seed;
        if (mapSize <= 0)
        {
            mapSize = 11;
        }
        if (mapSize % 2 == 0)
        {
            mapSize++;
        }
        if (seed <= 0)
        {
            seed = 1;
        }
    }
    void Start()
    {


        if (mapSize % 2 == 0)
        {
            Debug.Log("サイズが偶数になってるよ");
        }
        else
        {
            GenerateField();
            //20250522追加←20250604修正
            // CatとMouseの初期位置を設定
            if (catPrefab)
            {
                // 左上のスタート位置に配置する例
                Vector3 startPos = new Vector3(l, 0,l);
                catPrefab.transform.position = startPos;
                startPos.y = catCamera.transform.position.y;
                catCamera.transform.position = startPos;
            }
            if (mousePrefab)
            {
                Vector3 startPos = new Vector3(l * (mapSize - 2), 0, l * (mapSize - 2));
                mousePrefab.transform.position = startPos;
                startPos.y = mouseCamera.transform.position.y;
                mouseCamera.transform.position = startPos;
            }

            ChangeAllChildLayers();
            ChangeAllChildTags();
        }
    }

    public void ChangeAllChildLayers()
    {
        ChangeChildLayersRecursively(transform, newLayer);
    }

    public void ChangeChildLayersRecursively(Transform parent, int newLayer)
    {
        foreach (Transform child in parent)
        {
            // MiniMapLayerは除外する
            if (LayerMask.LayerToName(child.gameObject.layer) != "MiniMapLayer")
            {
                child.gameObject.layer = newLayer;
            }

            if (child.childCount > 0)
            {
                ChangeChildLayersRecursively(child, newLayer);
            }
        }
    }

    public void ChangeAllChildTags()
    {
        ChangeChildTagsRecursively(transform, newTag);
    }

    private void ChangeChildTagsRecursively(Transform parent, string newTag)
    {
        foreach(Transform child in parent)
        {

            if (child.name == "Chest(Clone)" || child.name == "Table(Clone)" || child.name == "Chair(Clone)" || child.name == "Sofa(Clone)")
            {
                child.gameObject.tag = newTag;
            }
            if (child.childCount > 0)
            {
                ChangeChildTagsRecursively(child, newTag);
            }
        }
    }

    void GenerateField()
    {
        Random.InitState(seed);

        //*GameManagerの配列のサイズを決定*
        GameManager.instance.wallMap = new bool[(mapSize - 2) * 4 + 2, (mapSize - 2) * 4 + 2];
        Debug.Log(GameManager.instance.wallMap.GetLength(0));

        bool[,] preWallMap = new bool[mapSize, mapSize];
        bool[,] visited = new bool[mapSize, mapSize];

        // *全部壁で初期化*
        for (int i = 0; i < mapSize; i++)
            for (int j = 0; j < mapSize; j++)
                preWallMap[i, j] = true;

        int startX = Random.Range(0, (mapSize - 1) / 2) * 2 + 1;
        int startY = Random.Range(0, (mapSize - 1) / 2) * 2 + 1;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));
        visited[startX, startY] = true;
        preWallMap[startX, startY] = false;

        // 通路生成
        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            int startDir = Random.Range(0, 4); // Unity乱数

            for (int i = 0; i < 4; i++)
            {
                int dir = (startDir + i) % 4;
                int ni = current.x + dx[dir] * 2;
                int nj = current.y + dy[dir] * 2;

                if (ni < 0 || ni >= mapSize || nj < 0 || nj >= mapSize) continue;
                if (visited[ni, nj]) continue;

                visited[ni, nj] = true;
                preWallMap[ni, nj] = false;
                preWallMap[current.x + dx[dir], current.y + dy[dir]] = false;
                stack.Push(new Vector2Int(ni, nj));
            }
        }

        // 通路の拡張
        for (int i = 1; i < mapSize - 1; i++)
        {
            for (int j = 1; j < mapSize - 1; j++)
            {
                if (!visited[i, j] && Random.Range(0, 3) < 2)
                {
                    visited[i, j] = true;
                    preWallMap[i, j] = false;
                }
            }
        }

        // プレハブを配置

        //まずは壁
        for (int r = 0; r < 4; r++)
        {
            for (int i = 2; i < mapSize + 2; i += 4)
            {
                float _i = 0;
                float _j = 0;

                //左
                if (r == 0)
                {
                    _i = (i - 0.5f) * l;
                    _j = 0.5f * l;
                }
                //上
                else if (r == 1)
                {
                    _i = 0.5f * l;
                    _j = (i - 0.5f) * l;
                }
                //右
                else if (r == 2)
                {
                    _i = (i - 0.5f) * l;
                    _j = (0.5f + mapSize - 2) * l;
                }
                //下
                else if (r == 3)
                {
                    _i = (0.5f + mapSize - 2) * l;
                    _j = (i - 0.5f) * l;
                }
                Vector3 pos = new Vector3(_i, 0, _j);
                Instantiate(wallPrefabs[(Random.Range(0, 5)) % 3], pos, Quaternion.Euler(0, 90 * r, 0));
            }
        }
        //*wallMap更新・壁*
        for (int k = 0; k < GameManager.instance.wallMap.GetLength(0); k++)
        {
            //左右上下
            GameManager.instance.wallMap[k, 0] = true;
            GameManager.instance.wallMap[k, GameManager.instance.wallMap.GetLength(0) - 1] = true;
            GameManager.instance.wallMap[0, k] = true;
            GameManager.instance.wallMap[GameManager.instance.wallMap.GetLength(0) - 1, k] = true;
        }


        //壁以外
        bool[,] furnitureCheck = new bool[mapSize, mapSize];

        // furnitreCheck初期化
        for (int i = 0; i < mapSize; i++)
            for (int j = 0; j < mapSize; j++)
                furnitureCheck[i, j] = false;

        for (int i = 1; i < mapSize - 1; i++)
        {
            for (int j = 1; j < mapSize - 1; j++)
            {
                //障害物のサイズに合わせて座標を調整
                float _i = i * l;
                float _j = j * l;

                //床・天井を張る
                Vector3 pos = new Vector3(_i, 0, _j);
                Instantiate(groundPrefab, pos, Quaternion.identity, transform);

                Vector3 ceilingPos = new Vector3(_i, 3f, _j);
                Quaternion ceilingQt = ceilingPrefab.transform.rotation;
                Instantiate(ceilingPrefab, ceilingPos, ceilingQt, transform);

                // ネコ・ネズミエリア：右上(1,1)・左下(mapSize-2, mapSize-2)は空けておく
                if ((i == 1 && j == 1) || (i == mapSize - 2 && j == mapSize - 2))
                {
                    continue;
                }

                if (preWallMap[i, j] == true && !furnitureCheck[i, j])
                {
                    Vector3 wallPos = new Vector3(_i, 0, _j);
                    //隣まで続いてる？
                    //縦
                    if (i != mapSize - 2 && preWallMap[i + 1, j] && !furnitureCheck[i + 1, j])
                    {
                        wallPos.x += l / 2;
                        furnitureCheck[i + 1, j] = true;
                        int rnd = Random.Range(0, 2);
                        Instantiate(furniturePrefabsL[rnd], wallPos, Quaternion.identity, transform);
                        for (int m = 0; m < 4; m++)
                        {
                            for (int n = 0; n < 4; n++)
                            {
                                bool[,] furnitureShape = getFurnitureShape(furniturePrefabsL[rnd]);
                                GameManager.instance.wallMap[1 + 4 * (i - 1) + m, 1 + 4 * (j - 1) + n] = furnitureShape[m, n];
                                GameManager.instance.wallMap[1 + 4 * i + m, 1 + 4 * (j - 1) + n] = furnitureShape[3 - m, n];
                            }
                        }
                    }
                    //横
                    else if (j != mapSize - 2 && preWallMap[i, j + 1] && !furnitureCheck[i, j + 1])
                    {
                        wallPos.z += l / 2;
                        furnitureCheck[i, j + 1] = true;
                        int rnd = Random.Range(0, 2);
                        Instantiate(furniturePrefabsL[rnd], wallPos, Quaternion.Euler(0, 90, 0), transform);
                        for (int m = 0; m < 4; m++)
                        {
                            for (int n = 0; n < 4; n++)
                            {
                                bool[,] furnitureShape = getFurnitureShape(furniturePrefabsL[rnd]);
                                GameManager.instance.wallMap[1 + 4 * (i - 1) + m, 1 + 4 * (j - 1) + n] = furnitureShape[n, m];
                                GameManager.instance.wallMap[1 + 4 * (i - 1) + m, 1 + 4 * j + n] = furnitureShape[3 - n, m];
                            }
                        }
                    }
                    //一マス
                    else
                    {
                        int rnd = Random.Range(0, 2);
                        Instantiate(furniturePrefabsS[rnd], wallPos, Quaternion.Euler(0, 90 * Random.Range(0, 4), 0), transform);
                        for (int m = 0; m < 4; m++)
                        {
                            for (int n = 0; n < 4; n++)
                            {
                                bool[,] furnitureShape = getFurnitureShape(furniturePrefabsS[rnd]);
                                GameManager.instance.wallMap[1 + 4 * (i - 1) + m, 1 + 4 * (j - 1) + n] = furnitureShape[m, n];
                            }
                        }
                    }
                }
                else
                {
                    //チーズ生成
                    int baseX = 1 + 4 * (i - 1);
                    int baseY = 1 + 4 * (j - 1);
                    bool canPlaceCheese = true;

                    for (int m = 0; m < 4; m++)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            if (GameManager.instance.wallMap[baseX + m, baseY + n])
                            {
                                canPlaceCheese = false;
                                break;
                            }
                        }
                    }

                    if (canPlaceCheese && Random.Range(0, 3) == 0)
                    {
                        Vector3 cheesePos = new Vector3(_i, l * 0.2f, _j);
                        Instantiate(cheesePrefab, cheesePos, Quaternion.identity, transform);
                        GameManager.instance.cheeseSum++;
                    }

                    //wallMap更新
                    /*
                    for (int m = 0; m < 4; m++)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            Debug.Log(1 + 4 * i + m);
                            Debug.Log(1 + 4 * j + n);
                            GameManager.instance.wallMap[1 + 4 * (i - 1) + m, 1 + 4 * (j - 1) + n] = false;
                        }
                    }
                    */
                }

                //furnitureCheck更新
                furnitureCheck[i, j] = true;
            }
        }

        //ライト
        if (GameManager.instance.gameMode != 0)
        {
            Vector3 lightPos = new Vector3(l * (mapSize / 2), 3, l * (mapSize / 2));
            Instantiate(lightPrefab, lightPos, Quaternion.identity, transform);
        }

        printWallMap();

        cat.startDFS();

        //タイマー開始
        GameManager.instance.timer = 0;
    }

    private bool[,] getFurnitureShape(GameObject gbj)
    {
        bool[,] rt = new bool[4, 4];

        if (gbj == null)
        {
            Debug.Log("objectが得られてないよ");

        }
        else if (gbj == furniturePrefabsS[0])
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rt[i, j] = true;
                }
            }
        }
        else if (gbj == furniturePrefabsS[1])
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rt[i, j] = false;
                }
            }
            rt[0, 0] = true;
            rt[0, 3] = true;
            rt[3, 0] = true;
            rt[3, 3] = true;
        }
        else if (gbj == furniturePrefabsL[0])
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rt[i, j] = true;
                }
            }
        }
        else if (gbj == furniturePrefabsL[1])
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rt[i, j] = false;
                }
            }
            rt[0, 0] = true;
            rt[0, 3] = true;
        }
        return rt;
    }

    void printWallMap()
    {
        for(int i = 0;i < GameManager.instance.wallMap.GetLength(0); i++)
        {
            string x = null;
            for(int j = 0; j < GameManager.instance.wallMap.GetLength(1); j++)
            {
                if (GameManager.instance.wallMap[i, j])
                {
                    x += "T";
                }
                else x += "F";
            }
            Debug.Log(x);
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}