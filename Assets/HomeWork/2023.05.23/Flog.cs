using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flog : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] Transform checkPoint;
	[SerializeField] LayerMask groundMask;
	Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		Move();
		if (!IsGroundExist())		// IsGround가 groundMask가 아니라면
			Turn();					// 반대방향으로 돈다.
	}

	void Move()
	{
		rb.velocity = new Vector2(transform.right.x * -moveSpeed, rb.velocity.y);   // 왼쪽방향으로 움직임
	}

	void Turn()
	{
		transform.Rotate(Vector3.up, 180);	// Y축을 기준으로 180도 회전(좌우반전)
	}

	bool IsGroundExist()	// 레이캐스트를 이용하여 밑으로 레이저를 쏘았을 때 바닥인지 체크한다.
	{
		return Physics2D.Raycast(checkPoint.position, Vector2.down, 1f, groundMask);
	}
}