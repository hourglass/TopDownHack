using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.PlayerSettings;

public class SwordController : MonoBehaviour
{
    [SerializeField]
    private GameObject sword; // �� ������ (���� ������)

    [SerializeField]
    private Vector3 swordPos; // �� ���� ��ġ (���� ���� ��ġ)

    // TODO:::�÷��̾� ��ũ��Ʈ���� �Է��� �޵��� ����
    private PlayerInput playerControls;
    private SpriteRenderer swordSpriteRenderer;

    // TODO:::�÷��̾� ��ũ��Ʈ���� �ø��� �ϵ��� ���� 
    private bool isRight;
    private float filpDistance;
    private float flipAngle;


    [SerializeField]
    private float rotSpeed; // �� ���콺 ���� ȸ�� �ӵ�

    [SerializeField]
    private float swingSpeed; // �� �ֵθ��� ȸ�� �ӵ�

    [SerializeField]
    private float swingDegree; // �ֵθ��� ���� ����

    [SerializeField]
    private float swordDegree; // �ֵθ� �� ���� ���� ����

    private bool isSwing; // ���� ������ üũ
    private bool isReverse; // ���� �ѹ� �ֵθ� �������� üũ
    private float swingDirection; // ���� �ֵθ� ����


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
    }

    private void FixedUpdate()
    {
        CheckFlip();
        LookAtMouse();
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
                flipAngle = -180f;
                sword.transform.localPosition = new Vector3(-swordPos.x, swordPos.y, swordPos.z);

                swordSpriteRenderer.flipX = !swordSpriteRenderer.flipX;
                swingDirection = -swingDirection;
                if(isReverse)
                {
                    sword.transform.localRotation = Quaternion.Euler(0f, 0f, swingDirection * -swordDegree);
                }
            }
        }
        else
        {
            // ���콺�� ĳ���ͺ��� �����ʿ� ���� ������
            if (mouseScreenPoint.x > playerScreenPoint.x + filpDistance)
            {
                isRight = true;
                flipAngle = 0f;
                sword.transform.localPosition = new Vector3(swordPos.x, swordPos.y, swordPos.z);

                swordSpriteRenderer.flipX = !swordSpriteRenderer.flipX;
                swingDirection = -swingDirection;
                if (isReverse)
                {
                    sword.transform.localRotation = Quaternion.Euler(0f, 0f, swingDirection * -swordDegree);
                }
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
        float progress = 0f;

        while (progress < 1f)
        {
            // time / duration�� ����Ͽ� progress ���� ���
            progress = time / duration;
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
