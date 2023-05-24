using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase<TOwner> where TOwner : MonoBehaviour 
{
	// � ������Ʈ�� ���������� �Ϲ�ȭ���� �����־�� ��
	protected TOwner owner;

	public StateBase(TOwner owner)
	{
		this.owner = owner;		// owner�� ���ؼ� ��� ���� ������ ���� �� ����
	}

	public abstract void SetUp();		// �ʱ� ����
	public abstract void Enter();		// �� ���¿� �������� ��
	public abstract void Update();		// ���� ���� ��
	public abstract void Exit();		// �� ���¿��� ����� ��
}