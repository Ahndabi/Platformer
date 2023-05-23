using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] float jumpPower;
	[SerializeField] float maxSpeed;

	[SerializeField] LayerMask groundLayer;		// ���̾� ����ũ

	SpriteRenderer renderer;
	Vector2 dir;
	Animator anim;
	Rigidbody2D rb;

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

	private void FixedUpdate()		// ���� ���õ� �����
	{
		GroundCheck();		// �� �ɷ��� �ٴ�üũ �� �� �ֵ���
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

	// �̰� �� ���� RayCast �� ����
	/*
	private void OnCollisionEnter2D(Collision2D collision)
	{
		anim.SetBool("IsGround", true);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		anim.SetBool("IsGround", false);
	}*/


	// Ray Cast
	void GroundCheck()		// �ٴ� üũ
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
		// transform ��ġ���� �Ʒ� �������� 1.5���͸�ŭ �������� ��ٴ� ��
		// �������� ���̾� ����ũ �� �־���� ��. Player���� �� �������� Player�� �浹���� �ʵ���
		// LayerMask.GetMask("Ground") ��ſ� groundLayer�� �ְ� �ν����Ϳ��� ���̾��ũ�� Ground���� ǥ���ص� ����!


		if(hit.collider != null)                // �ε��� �� ���� ���
		{
			anim.SetBool("IsGround", true);     // ���� �ִ�
			Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.red);
			// Ray�� �׷��� Ray�κ��� �ٴڿ� ��Ҵ����� Ȯ��

			// Debug.Log(hit.collider.gameObject.name);
		}
		else                                    // ���ٴڿ� �� ��Ҵ�
		{
			anim.SetBool("IsGround", false);    // �ϴÿ� �ִ�
			// Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.green);
		}
	}

	/*
	 * ���̾� ����ũ �浹 ����~!
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.layer == LayerMask.GetMask("Monster"))
			// ���Ͷ� �浹���� ��
		if (collision.gameObject.layer == LayerMask.GetMask("Item"))
			// �������̶� �浹���� ��
	}*/
}