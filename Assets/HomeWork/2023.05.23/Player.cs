using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] float jumpPower;
	[SerializeField] float maxSpeed;

	[SerializeField] LayerMask layerMask;	// 레이어 마스크 만들기
	Rigidbody2D rb;
	SpriteRenderer renderer;
	Vector2 dir;
	Animator anim;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		renderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		Move();
	}

	// 실습!
	// 1. 충돌체로 땅바닥 체크하기
	
	private void OnCollisionEnter2D(Collision2D collision)
	{
		anim.SetBool("IsGround", true);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		anim.SetBool("IsGround", false);
	}

	private void FixedUpdate()
	{
		//GroundCheck();		// 물리와 관련된 함수이기 때문에 FixedUpdate에서 구현
	}


	// 2. 레이캐스트를 이용해서 바닥 체크하기
	void GroundCheck()		
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
		// transform 위치에서 아래 방향으로 1.5미터만큼 레이저를 쏜다.
		// LayerMask.GetMask("Ground") 대신에 layerMask를 넣고 인스펙터에서 레이어마스크를 Ground만 표시해도 된다.

		if(hit.collider != null)				// 부딪힌 게 있을 경우
		{
			anim.SetBool("IsGround", true);		// 땅에 있다.
			Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.red);
			// Ray를 그려서 딸바닥에 닿았는지를 확인한다.
		}
		else									// 땅바닥에 안 닿았다.
			anim.SetBool("IsGround", false);	// 하늘에 있다.
	}







	void OnMove(InputValue value)
	{
		dir = value.Get<Vector2>();
	}

	public void Move()
	{
		// 속도가 끝없이 빨라지는 것을 방지하기 위해 maxSpeed보다 빨라지지 않도록 최고속도를 정해준다.
		if (dir.x < 0 && rb.velocity.x > -maxSpeed)
			rb.AddForce(Vector2.right * dir.x * moveSpeed, ForceMode2D.Force);
		else if (dir.x > 0 && rb.velocity.x < maxSpeed)
			rb.AddForce(Vector2.right * dir.x * moveSpeed);

		// MoveSpeed를 x로 수정한다. 절댓값을 쓰는 이유는, 왼쪽으로 갈 때도 애니메이션이 작동하도록 하기 위해서이다.
		anim.SetFloat("MoveSpeed", Mathf.Abs(dir.x));

		// 플레이어가 왼쪽으로 갈 때 좌우반전을 하도록 구현한다.
		if (dir.x > 0)
			renderer.flipX = false;
		else if (dir.x < 0)
			renderer.flipX = true;
	}

	void OnJump(InputValue value)
	{
		Jump();
	}

	public void Jump()
	{
		rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
	}

}
