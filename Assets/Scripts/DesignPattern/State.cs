using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//========================================
//##		디자인 패턴 State			##
//========================================
/*
	상태 패턴 :
	객체에게 한 번에 하나의 상태만을 가지게 하며
	객체는 현재 상태에 해당하는 행동만을 진행함

	가만히 있는 상태, 순찰 중인 상태, 점프 상태, 맞는 상태 등,, 각 상태들을 만들어줌

	구현 :
	1. 열거형 자료형으로 객체가 가질 수 있는 상태들을 정의
	2. 현재 상태를 저장하는 변수에 초기 상태를 지정
	3. 객체는 행동에 있어서 현재 상태만의 행동을 진행
	4. 객체는 현재 상태의 행동을 진행 후 상태 변화에 대해 판단
	5. 상태 변화가 있어야 하는 경우 현재 상태를 대상 상태로 지정
	6. 상태가 변경된 경우 다음 행동에 있어서 바뀐 상태만의 행동을 진행

	장점 :
	1. 객체가 진행할 행동을 복잡한 조건문을 상태로 처리가 가능하므로, 조건처리에 대한 부담이 적음 (가장 큰 장점)
	2. 객체가 가지는 여러상태에 대한 연산없이 현재상태만을 처리하므로, 연산속도가 뛰어남
	3. 객체와 관련된 모든 동작을 각각의 상태에 분산시키므로, 코드가 간결하고 가독성이 좋음

	단점 :
	1. 상태의 구분이 명확하지 않거나 갯수가 많은 경우, 상태 변경 코드가 복잡해질 수 있음
	2. 상태를 클래스로 캡슐화 시키지 않은 경우 상태간 간섭이 가능하므로, 개방폐쇄원칙이 준수되지 않음
	3. 간단한 동작의 객체에 상태패턴을 적용하는 경우, 오히려 관리할 상태 코드량이 증가하게 됨
*/

namespace DesignPattern
{
	public class State
	{
		public class Mobile
		{
			public enum State { Off, Normal, Charge, FullCharged }
			// 꺼진 상태, 소비 중, 충전 중, 풀충전 됐을 때

			// 초기 상태들 지정
			private State state = State.Normal;     // 현재 상태
			private bool charging = false;          // 충전 여부
			private float battery = 50.0f;          // 잔여 배터리

			private void Update()
			{
				// 현재 상태에서 할 업데이트들을 진행. (OffUpdate, NormalUpdate, ChargeUpdate, FullChargedUpdate)
				switch (state)
				{
					case State.Off:
						OffUpdate();
						break;
					case State.Normal:
						NormalUpdate();
						break;
					case State.Charge:
						ChargeUpdate();
						break;
					case State.FullCharged:
						FullChargedUpdate();
						break;
				}
			}

			private void OffUpdate()
			{
				// 충전도 안 하고 있고, 배터리도 없다.
				// Off work
				// Do nothing

				if (charging)               // 그와중에 만약 충전이 됐다?
				{
					state = State.Charge;   // 상태를 충전상태로 전환만 해주면 됨
				}
			}

			private void NormalUpdate()
			{
				// 충전은 안 하는데 배터리는 있다.

				// Normal work
				battery -= 1.5f * Time.deltaTime;       // 배터리는 계속 소모 중,,

				if (charging)               // 만약 충전을 시작했다?
				{
					state = State.Charge;   // Charge 상태로 전환만 해주면 됨
				}
				else if (battery <= 0)      // 만약 배터리 다 썼다?
				{
					state = State.Off;      // Off 상태로 전환만 해주면 됨
				}
			}

			private void ChargeUpdate()
			{
				// 배터리 충전 중

				// Charge work
				battery += 25f * Time.deltaTime;    // 배터리 차는 중

				if (!charging)                  // 충전을 더이상 안 한다면
				{
					state = State.Normal;       // Normal 상태로 변경만
				}
				else if (battery >= 100)        // 배터리가 100 이상이면
				{
					state = State.FullCharged;  // FullCharged 상태로 변경
				}
			}

			private void FullChargedUpdate()
			{
				// FullCharged work

				if (!charging)
				{
					state = State.Normal;
				}
			}

			public void ConnectCharger()
			{
				charging = true;
			}

			public void DisConnectCharger()
			{
				charging = false;
			}
		}
	}
}