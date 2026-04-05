using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeedIn;//プレイヤーの移動速度を入力
    [SerializeField]
    GameObject playerCamera;


    Rigidbody playerRb;//プレイヤーのRigidbody

    Vector3 moveSpeed;//プレイヤーの移動速度

    Vector3 currentPos;//プレイヤーの現在の位置
    Vector3 pastPos;//プレイヤーの過去の位置

    Vector3 delta;//プレイヤーの移動量

    Quaternion playerRot;//プレイヤーの進行方向を向くクォータニオン

    float currentAngularVelocity;//現在の回転各速度

    [SerializeField]
    float maxAngularVelocity = Mathf.Infinity;//最大の回転角速度[deg/s]

    [SerializeField]
    float smoothTime = 0.1f;//進行方向にかかるおおよその時間[s]

    float diffAngle;//現在の向きと進行方向の角度

    float rotAngle;//現在の回転する角度

    private Animator catAnim;
    private bool isMoving;
    private float catTimer;

    Quaternion nextRot;//どんくらい回転するか
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        pastPos = transform.position;

        if (gameObject.name == "Cat")
        {
            catAnim = GetComponent<Animator>();
            catAnim.SetBool("isSearching", false);
        }
    }

    void Update()
    {
        catTimer += Time.deltaTime;
        isMoving = false;
        Vector3 rawForward = playerCamera.transform.forward;
        Vector3 cameraForward = new Vector3(rawForward.x, 0, rawForward.z);

        // カメラの前方向がほぼゼロなら代替ベクトルをセット
        if (cameraForward.sqrMagnitude < 0.01f)
        {
            cameraForward = Vector3.forward; // ワールド座標の前方向を代わりに使う
        }
        else
        {
            cameraForward.Normalize();
        }

        Vector3 rawRight = playerCamera.transform.right;
        Vector3 cameraRight = new Vector3(rawRight.x, 0, rawRight.z).normalized;

        moveSpeed = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveSpeed += moveSpeedIn * cameraForward;
            isMoving = true;
            catTimer = 0;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveSpeed += -moveSpeedIn * cameraRight;
            isMoving = true;
            catTimer = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveSpeed += -moveSpeedIn * cameraForward;
            isMoving = true;
            catTimer = 0;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveSpeed += moveSpeedIn * cameraRight;
            isMoving = true;
            catTimer = 0;
        }

        Move();

        if (gameObject.name == "Cat")
        {
            catAnim.SetBool("isSearching", isMoving);
            catAnim.SetBool("isFindMouse", isMoving);

            if (catTimer > 5 && !GameManager.instance.isFinished)
            {
                catAnim.SetBool("isBored", true);
            }
            else
            {
                catAnim.SetBool("isBored", false);
            }
        }

        //慣性を消す
        /*
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            //playerRb.velocity = Vector3.zero;
            // playerRb.angularVelocity = Vector3.zero;
        }
        */

        //------プレイヤーの回転------

        //現在の位置
        currentPos = transform.position;

        //移動量計算
        delta = currentPos - pastPos;
        delta.y = 0;

        //過去の位置の更新
        pastPos = currentPos;

        if (delta.magnitude < 0.01f || moveSpeed.magnitude < 0.5f)
            return;

        playerRot = Quaternion.LookRotation(delta, Vector3.up);

        diffAngle = Vector3.Angle(transform.forward, delta);

        //Vector3.SmoothDampはVector3型の値を徐々に変化させる
        //Vector3.SmoothDamp (現在地, 目的地, ref 現在の速度, 遷移時間, 最高速度);
        rotAngle = Mathf.SmoothDampAngle(0, diffAngle, ref currentAngularVelocity, smoothTime, maxAngularVelocity);

        nextRot = Quaternion.RotateTowards(transform.rotation, playerRot, rotAngle);

        transform.rotation = nextRot;

    }

    /// <summary>
    /// 移動方向に力を加える
    /// </summary>
    private void Move()
    {
        //playerRb.AddForce(moveSpeed, ForceMode.Force);

        playerRb.velocity = moveSpeed;
    }

}



