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

    private PlayerState currentState;

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

    private bool isRight;
    private float filpDistance;


    private void Awake()
    {
        playerControls = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = SpriteObject.GetComponent<Animator>();
        mySpriteRenderer = SpriteObject.GetComponent<SpriteRenderer>();

        filpDistance = 50f;
        isRight = true;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void FixedUpdate()
    {
        CheckFlip();
        Move();
    }

    private void CheckFlip()
    {
        // 마우스와 플레이어의 스크린 좌표 가져오기
        Vector3 mouseScreenPoint = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (isRight)
        {
            // 마우스가 캐릭터보다 왼쪽에 오면 뒤집기 
            if (mouseScreenPoint.x < playerScreenPoint.x - filpDistance)
            {
                isRight = false;
                mySpriteRenderer.flipX = true;
            }
        }
        else
        {
            // 마우스가 캐릭터보다 오른쪽에 오면 뒤집기
            if (mouseScreenPoint.x > playerScreenPoint.x + filpDistance)
            {
                isRight = true;
                mySpriteRenderer.flipX = false;
            }
        }
    }

    private void Move()
    {
        // 마우스 커서의 위치를 받아오기
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = rb.position;

        // 마우스 방향으로 향하는 벡터 구하기
        Vector2 forward = mousePos - playerPos;
        forward.Normalize();

        if (Vector2.Distance(mousePos, playerPos) > stopDistance)
        {
            if (currentState != PlayerState.Run) 
            { 
                ChangeState(PlayerState.Run); 
            }

            // 마우스 방향으로 이동
            rb.velocity = forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            ChangeState(PlayerState.Idle);

            // 마우스가 플레이어와 일정 거리만큼 가까워지면 정지
            rb.velocity = Vector2.zero;
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
