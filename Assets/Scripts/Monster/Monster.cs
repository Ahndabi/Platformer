using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] Transform groundCheckPoint;
	[SerializeField] LayerMask groundMask;

	Rigidbody2D rb;
	Animator anim;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		Move();
		if (!IsGroundExist())
		{
			Turn();
		}
	}

	public void Move()     // �Է��� �޾Ƽ� �����̴� �� �ƴ�, �׳� ��~�� ������
	{
		rb.velocity = new Vector2(transform.right.x * -moveSpeed, rb.velocity.y);   // ���ʹ������� ������
	}

	public void Turn()
	{
		transform.Rotate(Vector3.up, 180);
	}

	bool IsGroundExist()
	{
		Debug.DrawRay(groundCheckPoint.position, Vector2.down, Color.red);
		return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, 1f, groundMask);
	}
}