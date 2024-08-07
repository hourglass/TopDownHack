using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Run
    }

    [SerializeField]
    private GameObject SpriteObject;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float stopDistance;

    private PlayerInput playerControls;
    private Rigidbody2D rb;

    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    PlayerState currentState;


    private void Awake()
    {
        playerControls = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = SpriteObject.GetComponent<Animator>();
        mySpriteRenderer = SpriteObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        LookAtMouse();
        Move();
    }

    private void PlayerInput()
    {
        //Vector2 movement = playerControls.Movement.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        // ���콺 Ŀ���� ��ġ�� �޾ƿ���
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = rb.position;

        // ���콺 �������� ���ϴ� ���� ���ϱ�
        Vector2 forward = mousePos - playerPos;
        forward.Normalize();

        if (Vector2.Distance(mousePos, playerPos) > stopDistance)
        {
            if (currentState != PlayerState.Run) 
            { 
                ChangeState(PlayerState.Run); 
            }

            // ���콺 �������� �̵�
            rb.velocity = forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            ChangeState(PlayerState.Idle);

            // ���콺�� �÷��̾�� ���� �Ÿ���ŭ ��������� ����
            rb.velocity = Vector2.zero;
        }
    }

    private void LookAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
        }
        else
        {
            mySpriteRenderer.flipX = false;
        }
    }

    private void ChangeState(PlayerState state)
    {
        /*
        switch (state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                break;
        }
        */

        myAnimator.SetInteger("State", (int)state);
        currentState = state;
    }
}
