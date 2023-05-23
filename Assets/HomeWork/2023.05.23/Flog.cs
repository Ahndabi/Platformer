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
		if (!IsGroundExist())		// IsGround�� groundMask�� �ƴ϶��
			Turn();					// �ݴ�������� ����.
	}

	void Move()
	{
		rb.velocity = new Vector2(transform.right.x * -moveSpeed, rb.velocity.y);   // ���ʹ������� ������
	}

	void Turn()
	{
		transform.Rotate(Vector3.up, 180);	// Y���� �������� 180�� ȸ��(�¿����)
	}

	bool IsGroundExist()	// ����ĳ��Ʈ�� �̿��Ͽ� ������ �������� ����� �� �ٴ����� üũ�Ѵ�.
	{
		return Physics2D.Raycast(checkPoint.position, Vector2.down, 1f, groundMask);
	}
}