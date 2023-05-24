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

	[SerializeField] LayerMask layerMask;	// ���̾� ����ũ �����
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

	// �ǽ�!
	// 1. �浹ü�� ���ٴ� üũ�ϱ�
	
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
		//GroundCheck();		// ������ ���õ� �Լ��̱� ������ FixedUpdate���� ����
	}


	// 2. ����ĳ��Ʈ�� �̿��ؼ� �ٴ� üũ�ϱ�
	void GroundCheck()		
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
		// transform ��ġ���� �Ʒ� �������� 1.5���͸�ŭ �������� ���.
		// LayerMask.GetMask("Ground") ��ſ� layerMask�� �ְ� �ν����Ϳ��� ���̾��ũ�� Ground�� ǥ���ص� �ȴ�.

		if(hit.collider != null)				// �ε��� �� ���� ���
		{
			anim.SetBool("IsGround", true);		// ���� �ִ�.
			Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.red);
			// Ray�� �׷��� ���ٴڿ� ��Ҵ����� Ȯ���Ѵ�.
		}
		else									// ���ٴڿ� �� ��Ҵ�.
			anim.SetBool("IsGround", false);	// �ϴÿ� �ִ�.
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
