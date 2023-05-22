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
		// �ӵ��� ������ �������� ���� �����ϱ� ���� maxSpeed���� �������� �ʵ��� �ְ�ӵ��� �����ش�.
		if (dir.x < 0 && rb.velocity.x > -maxSpeed)
			rb.AddForce(Vector2.right * dir.x * moveSpeed, ForceMode2D.Force);
		else if (dir.x > 0 && rb.velocity.x < maxSpeed)
			rb.AddForce(Vector2.right * dir.x * moveSpeed);
		
		// MoveSpeed�� x�� �����Ѵ�. ������ ���� ������, �������� �� ���� �ִϸ��̼��� �۵��ϵ��� �ϱ� ���ؼ��̴�.
		anim.SetFloat("MoveSpeed", Mathf.Abs(dir.x));

		// �÷��̾ �������� �� �� �¿������ �ϵ��� �����Ѵ�.
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
