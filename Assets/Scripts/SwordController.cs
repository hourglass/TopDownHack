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
    private bool isReverse;
    private float swingDirection;

    private PlayerInput playerControls;
    private SpriteRenderer swordSpriteRenderer;


    private void Awake()
    {
        playerControls = new PlayerInput();
        swordSpriteRenderer = sword.GetComponent<SpriteRenderer>();
        sword.transform.localPosition = swordPos;

        isRight = true;
        filpDistance = 50f;
        flipAngle = 0f;

        isSwing = false;
        isReverse = false;
        swingDirection = 1f;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => Attack();

        isPause = false;
    }

    private bool isPause;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPause = !isPause;
            Debug.Log(isPause);
        }
    }

    private void FixedUpdate()
    {
        if (!isPause) 
        {
            CheckFlip();
            LookAtMouse();
        }
    }

    void CheckFlip()
    {
        // ���콺�� �÷��̾��� ��ũ�� ��ǥ ��������
        Vector3 mouseScreenPoint = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.parent.position);

        if (isRight)
        {
            // ���콺�� ĳ���ͺ��� ���ʿ� ���� ������
            if (mouseScreenPoint.x < playerScreenPoint.x - filpDistance)
            {
                isRight = false;
                sword.transform.localPosition = new Vector3(-swordPos.x, swordPos.y, swordPos.z);
                flipAngle = -180f;

                swordSpriteRenderer.flipX = !swordSpriteRenderer.flipX;
                swingDirection = -swingDirection;
                if (isReverse)
                { 
                    sword.transform.localRotation = 
                }
            }
        }
        else
        {
            // ���콺�� ĳ���ͺ��� �����ʿ� ���� ������
            if (mouseScreenPoint.x > playerScreenPoint.x + filpDistance)
            {
                isRight = true;
                sword.transform.localPosition = new Vector3(swordPos.x, swordPos.y, swordPos.z);
                flipAngle = 0f;

                swordSpriteRenderer.flipX = !swordSpriteRenderer.flipX;
                swingDirection = -swingDirection;

            }
        }
    }

    private void LookAtMouse()
    {
        if (isSwing) { return; }

        // ���콺 Ŀ���� ��ġ�� �޾ƿ���
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ���콺�� ��ü ������ �Ÿ� ���ϱ�
        Vector2 distance = mousePos - (Vector2)transform.position;

        // ��ũź��Ʈ�� ����(Y)�� �غ���(X)�� �־� ������ ���ϱ�
        float angle = (Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg);

        angle = angle + flipAngle;
        if (angle < -180f)
        {
            angle = angle + 360f;
        }

        if (isRight)
        {
            angle = Mathf.Clamp(angle, -25f, 90f);
        }
        else
        {
            angle = Mathf.Clamp(angle, -90f, 25f);
        }

        // ȸ�� �� ����
        // �ٶ󺸴� ���� = angle + ��������Ʈ ����(-90f) + ��� �ִ� ����(+90f)
        Quaternion targetRotation;
        if (!isReverse)
        {
            targetRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else 
        {
            targetRotation = Quaternion.Euler(0f, 0f, angle + (swingDegree * -swingDirection));
        }

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
        float startAngle = transform.eulerAngles.z;
        float targetAngle = startAngle + (swingDegree * swingDirection);

        float startSwordAngle = sword.transform.localEulerAngles.z;
        float targetSwordAngle = startSwordAngle + (swordDegree * swingDirection);

        float duration = 0.5f;
        float time = 0f;

        while (time < duration)
        {
            // time / duration�� ����Ͽ� progress ���� ���
            float progress = time / duration;
            float progressPow = Mathf.Pow(progress, 3);

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, swingSpeed * progressPow);

            Quaternion targetSwordRotation = Quaternion.Euler(0f, 0f, targetSwordAngle);
            sword.transform.localRotation = Quaternion.Slerp(sword.transform.localRotation, targetSwordRotation, swingSpeed * 0.02f * progress);

            time += Time.deltaTime;
            yield return null;
        }

        swordSpriteRenderer.flipX = !swordSpriteRenderer.flipX;
        swingDirection = -swingDirection;

        // ������ ���� ����
        transform.eulerAngles = new Vector3(0f, 0f, targetAngle);
        sword.transform.localEulerAngles = new Vector3(0f, 0f, targetSwordAngle);

        isSwing = false;
        isReverse = !isReverse;
    }
}
