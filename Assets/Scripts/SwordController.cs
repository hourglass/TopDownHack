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

    private float swingDirection;
    private bool isSwing;

    private PlayerInput playerControls;
    private SpriteRenderer mySpriteRenderer;


    private void Awake()
    {
        playerControls = new PlayerInput();
        mySpriteRenderer = sword.GetComponent<SpriteRenderer>();
        isSwing = false;
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
        LookAtMouse();
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
        angle = angle % 360;

        // ��������Ʈ �ø�
        Vector3 mouseScreenPoint = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.parent.position);

        if (mouseScreenPoint.x > playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
            sword.transform.localPosition = new Vector3(swordPos.x, swordPos.y, swordPos.z);

            swingDirection = 1f;
        }
        else
        {
            mySpriteRenderer.flipX = false;
            sword.transform.localPosition = new Vector3(-swordPos.x, swordPos.y, swordPos.z);

            swingDirection = -1f;
            angle = angle - 180f;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
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

        // ������ ���� ����
        transform.eulerAngles = new Vector3(0f, 0f, targetAngle);
        sword.transform.localEulerAngles = new Vector3(0f, 0f, startSwordAngle);

        isSwing = false;
    }
}
