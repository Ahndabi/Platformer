using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] float jumpPower;
	[SerializeField] float maxSpeed;

	SpriteRenderer renderer;
	Vector2 dir;
	Rigidbody2D rb;
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
		else if(dir.x < 0)
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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		anim.SetBool("IsGround", true);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		anim.SetBool("IsGround", false);
	}
}
