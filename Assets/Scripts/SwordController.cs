using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.PlayerSettings;

public class SwordController : MonoBehaviour
{
    [SerializeField]
    private GameObject sword;

    [SerializeField]
    private Vector3 swordPos;

    [SerializeField]
    private float rotSpeed;

    [SerializeField]
    private float swingSpeed;

    [SerializeField]
    private float swingDegree;

    [SerializeField]
    private float swordDegree;

    private bool isRight;
    private float filpDistance;
    private float flipAngle;

    private bool isSwing;
    private float swingDirection;

    private PlayerInput playerControls;
    private SpriteRenderer mySpriteRenderer;


    private void Awake()
    {
        playerControls = new PlayerInput();
        mySpriteRenderer = sword.GetComponent<SpriteRenderer>();

        isRight = true;
        filpDistance = 50f;
        flipAngle = 0f;

        isSwing = false;
        swingDirection = 1f;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => Attack();
    }

    private void FixedUpdate()
    {
        CheckFlip();
        LookAtMouse();
    }

    void CheckFlip()
    {
        // 마우스와 플레이어의 스크린 좌표 가져오기
        Vector3 mouseScreenPoint = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.parent.position);

        if (isRight)
        {
            // 마우스가 캐릭터보다 왼쪽에 오면 뒤집기
            if (mouseScreenPoint.x < playerScreenPoint.x - filpDistance)
            {
                isRight = false;
                mySpriteRenderer.flipX = false;
                sword.transform.localPosition = new Vector3(-swordPos.x, swordPos.y, swordPos.z);
                swingDirection = -1f;
                flipAngle = -180f;
            }
        }
        else
        {
            // 마우스가 캐릭터보다 오른쪽에 오면 뒤집기
            if (mouseScreenPoint.x > playerScreenPoint.x + filpDistance)
            {
                isRight = true;
                mySpriteRenderer.flipX = true;
                sword.transform.localPosition = new Vector3(swordPos.x, swordPos.y, swordPos.z);
                swingDirection = 1f;
                flipAngle = 0f;
            }
        }
    }

    private void LookAtMouse()
    {
        if (isSwing) { return; }

        // 마우스 커서의 위치를 받아오기
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스와 물체 사이의 거리 구하기
        Vector2 distance = mousePos - (Vector2)transform.position;

        // 아크탄젠트에 높이(Y)와 밑변을(X)를 넣어 각도를 구하기
        float angle = (Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg);

        // 회전 값 적용
        // 바라보는 방향 = angle + 스프라이트 각도(-90f) + 들고 있는 각도(+90f)
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + flipAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }

    void Attack()
    {
        if (isSwing) { return; }

        isSwing = true;
        StartCoroutine(SwingCoroutine());
    }

    IEnumerator SwingCoroutine()
    {
        float startAngle = transform.eulerAngles.z + (25f * swingDirection); ;
        float targetAngle = startAngle + (swingDegree * swingDirection);

        float startSwordAngle = sword.transform.localEulerAngles.z;
        float targetSwordAngle = startSwordAngle + (swordDegree * swingDirection);

        float duration = 0.5f;
        float time = 0f;

        while (time < duration)
        {
            // time / duration을 사용하여 progress 값을 계산
            float progress = time / duration;
            float progressPow = Mathf.Pow(progress, 3);

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, swingSpeed * progressPow);

            Quaternion targetSwordRotation = Quaternion.Euler(0f, 0f, targetSwordAngle);
            sword.transform.localRotation = Quaternion.Slerp(sword.transform.localRotation, targetSwordRotation, swingSpeed * 0.02f * progress);

            time += Time.deltaTime;
            yield return null;
        }

        // 마지막 각도 설정
        transform.eulerAngles = new Vector3(0f, 0f, targetAngle);
        sword.transform.localEulerAngles = new Vector3(0f, 0f, startSwordAngle);

        isSwing = false;
    }
}
