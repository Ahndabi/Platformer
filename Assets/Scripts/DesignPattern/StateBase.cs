using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase<TOwner> where TOwner : MonoBehaviour 
{
	// 어떤 컴포넌트의 상태인지를 일반화시켜 갖고있어야 함
	protected TOwner owner;

	public StateBase(TOwner owner)
	{
		this.owner = owner;		// owner를 통해서 얻고 싶은 정보를 얻을 수 있음
	}

	public abstract void SetUp();		// 초기 셋팅
	public abstract void Enter();		// 그 상태에 진입했을 때
	public abstract void Update();		// 동작 중일 때
	public abstract void Exit();		// 그 상태에서 벗어났을 때
}