using BeeState;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// using static UnityEngine.RuleTile.TilingRuleOutput;		이거 하면 Transform, Transform[]이 오류남

public class Bee : MonoBehaviour
{
	// 1. 플레이어가 멀리 있을 땐 가만히 있기
	// 2. 플레이어가 어느정도 가까워지면, 플레이어를 공격하도록 추적
	// 2-1 추적 중에 너무 멀어져서 쫒아가기 어려워지면 다시 제자리
	// 2-2 추적 중에 공격범위 안에 들어오면 공격

	public enum State { Idle, Trace, Return, Attack, Patrol, Size }


	StateBase<Bee>[] states;      // 상태들을 들고 있음 (딕셔너리 형태로도 가지고 있기도 함)
	[SerializeField] State curState;

	[SerializeField] public float detectRange;     // 추적할 수 있는 범위
	[SerializeField] public float moveSpeed;       // 속도
	[SerializeField] public float attackRange;     // 공격할 수 있는 범위
	[SerializeField] public float LastAttackTime;      // 마지막 공격 시간(?)
	[SerializeField] public Transform[] patrolPoints;      // 순찰할 수 있는 포인트들

	public Transform player;           // 플레이어 위치
	public Vector3 returnPosition;     // 돌아갈 위치
	public int patrolIndex = 0;        // 내가 지금 순찰하고 있는 곳?


	private void Awake()
	{
		states = new StateBase<Bee>[(int)State.Size];   // Size는 열거형으로 쳤을 때 5번째(인덱스개념)니까, 5가 됨
		states[(int)State.Idle] = new IdleState(this);		// states[0]
		states[(int)State.Trace] = new TraceState(this);    // states[1]	
		states[(int)State.Return] = new ReturnState(this);  // states[2]
		states[(int)State.Attack] = new AttackState(this);  // states[3]
		states[(int)State.Patrol] = new PatrolState(this);  // states[4]
	}

	private void Start()
	{
		curState = State.Idle;      // 처음 시작 상태는 가만히 있는 거로 시작.
		states[(int)curState].Enter();

		player = GameObject.FindGameObjectWithTag("Player").transform;
		returnPosition = transform.position;    // 돌아갈 위치는 시작했을 때의 그 위치로 설정
	}

	private void Update()
	{
		states[(int)curState].Update();
	}

	public void ChangeState(State state)		// 현재 상태로 바꾸어줌
	{
		curState = state;
	}
}


namespace BeeState
{
	public class IdleState : StateBase<Bee>
	{
		Bee bee;
		float idleTime = 0;

		
		public IdleState(Bee bee) : base(bee)   // 생성할 당시 Bee,가 bee를 매개변수로 넣었으니 IdleState도 Bee를 이용가능
		{
			this.bee = bee;
		}

		/*
		public IdleState(Bee owner) : base(owner)
		{
		}*/

		public override void SetUp()
		{

		}

		public override void Enter()
		{
			idleTime = 0;		// 진입했을 땐 idle시간을 0부터 시작
		}

		public override void Exit()
		{

		}

		public override void Update()		// 상태가 진행 중일 때 할 일
		{
			idleTime += Time.deltaTime;
			
			// 아무것도 안 하기

			if (idleTime > 2)       // idle 상태가 2초 이상 지나면 Patrol 상태로 변경
			{
				// idleTime = 0;
				//bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length; 이건 Patrol에 Enter로 옮김
				bee.ChangeState(Bee.State.Patrol);
			}

			// 만약 플레이어와 가까워 지면,
			if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
			{
				bee.ChangeState(Bee.State.Trace);		// 추적상태로 변환
			}
		}
	}

	public class TraceState : StateBase<Bee>
	{
		Bee bee;

		public TraceState(Bee bee) : base(bee)
		{
			this.bee = bee;
		}

		public override void Enter()
		{
		}

		public override void Exit()
		{
		}

		public override void SetUp()
		{
		}

		public override void Update()
		{
			// 플레이어 쫓아가기
			Vector2 dir = (bee.player.position - bee.transform.position).normalized;
			// normalized : 벡터가 크든 작든, 벡터를 1로 만들어줌 (방향을 정규화 시켜줌)
			bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

			// 만약 플레이어가 멀어지면,
			if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
			{
				// 추적 상태로 변환시킴
				bee.ChangeState(Bee.State.Return);
			}

			// 공격범위 안에있을 때
			else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.attackRange)
			{
				bee.ChangeState(Bee.State.Attack);
			}
		}
	}

	public class AttackState : StateBase<Bee>
	{
		Bee bee;

		public AttackState(Bee bee) : base(bee)
		{
			this.bee = bee;
		}

		public override void Enter()
		{
		}

		public override void Exit()
		{
		}

		public override void SetUp()
		{
		}

		public override void Update()
		{
			// 공격하기
			// 1초마다 공격하게끔 해줌
			if (bee.LastAttackTime > 1)
			{
				Debug.Log("공격");
				bee.LastAttackTime = 0;
			}

			bee.LastAttackTime += Time.deltaTime;

			// 공격 중에 공격범위를 벗어나면
			if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.attackRange)
			{
				bee.ChangeState(Bee.State.Trace);		// 다시 추적모드로
			}
		}
	}

	public class ReturnState : StateBase<Bee>
	{
		Bee bee;

		public ReturnState(Bee bee) : base(bee)
		{
			this.bee = bee;
		}

		public override void Enter()
		{
		}

		public override void Exit()
		{
		}

		public override void SetUp()
		{
		}

		public override void Update()
		{
			// 원래 자리로 돌아가기
			Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
			// normalized : 벡터가 크든 작든, 벡터를 1로 만들어줌 (방향을 정규화 시켜줌)
			bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

			// 원래 자리에 도착했으면,
			// 유니티 float는 소수점 조금이라도 벗어나면 transform.position == returnposition이 다르다고 나오기 떄문에
			// < 0.02f 처럼 범위를 지정해주는 것이 좋음
			if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.02f)
			{
				bee.ChangeState(Bee.State.Idle);
			}
			// 돌아가는 중에도 플레이어가 포착이 되면 Trace로 되게끔
			else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
			{
				bee.ChangeState(Bee.State.Trace);
			}
		}
	}

	public class PatrolState : StateBase<Bee>
	{
		Bee bee;

		public PatrolState(Bee bee) : base(bee)
		{
			this.bee = bee;
		}

		public override void Enter()
		{
			// 처음에 다음 탐색 지역이 어디인지 봄!
			bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;      // 다음 위치로 감
			// patrolIndex + 1 하다가 인덱스 범위 넘어갈까봐
		}

		public override void Exit()
		{
		}

		public override void SetUp()
		{
		}

		public override void Update()
		{
			// 순찰 진행
			//bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;      // 다음 위치로 감

			Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
			bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

			if (Vector2.Distance(bee.transform.position, bee.patrolPoints[bee.patrolIndex].position) < 0.02f)
			{
				bee.ChangeState(Bee.State.Idle);

			}
			else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
			{
				bee.ChangeState(Bee.State.Trace);
			}

		}
	}
}








	/*
	private void Update()
	{
		switch (curState)
		{
			case State.Idle:
				IdleUpdate();
				break;
			case State.Trace:
				TraceUpdate();
				break;
			case State.Return:
				ReturnUpdate();
				break;
			case State.Attack:
				AttackUpdate();
				break;
			case State.Patrol:
				PatrolUpdate();
				break;
		}
	}


	float idleTime = 0;

	void IdleUpdate()
	{
		// 아무것도 안 하기
		
		if(idleTime > 2)		// idle 상태가 2초 이상 지나면 Patrol 상태로 변경
		{
			idleTime = 0;
			patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
			curState = State.Patrol;
		}
		idleTime += Time.deltaTime;

		// 만약 플레이어와 가까워 지면,
		if(Vector2.Distance(player.position, transform.position) < detectRange)
		{
			// 추적 상태로 변환시킴
			curState = State.Trace;
		}
	}

	void TraceUpdate()
	{
		// 플레이어 쫓아가기
		Vector2 dir = (player.position - transform.position).normalized;
		// normalized : 벡터가 크든 작든, 벡터를 1로 만들어줌 (방향을 정규화 시켜줌)
		transform.Translate(dir * moveSpeed * Time.deltaTime);

		// 만약 플레이어가 멀어지면,
		if (Vector2.Distance(player.position, transform.position) > detectRange)
		{
			// 추적 상태로 변환시킴
			curState = State.Return;
		}

		// 공격범위 안에있을 때
		else if(Vector2.Distance(player.position, transform.position) < attackRange)
		{
			curState = State.Attack;
		}
	}

	void ReturnUpdate()
	{
		// 원래 자리로 돌아가기
		Vector2 dir = (returnPosition - transform.position).normalized;
		// normalized : 벡터가 크든 작든, 벡터를 1로 만들어줌 (방향을 정규화 시켜줌)
		transform.Translate(dir * moveSpeed * Time.deltaTime);

		// 원래 자리에 도착했으면,
		// 유니티 float는 소수점 조금이라도 벗어나면 transform.position == returnposition이 다르다고 나오기 떄문에
		// < 0.02f 처럼 범위를 지정해주는 것이 좋음
		if (Vector2.Distance(transform.position, returnPosition) < 0.02f )
		{
			curState = State.Idle;
		}
		// 돌아가는 중에도 플레이어가 포착이 되면 Trace로 되게끔
		else if(Vector2.Distance(player.position, transform.position) < detectRange)
		{
			curState = State.Trace;
		}

	}
	
	void AttackUpdate()
	{
		// 공격하기
		// 1초마다 공격하게끔 해줌
		if(LastAttackTime > 1)
		{
			Debug.Log("공격");
			LastAttackTime = 0;
		}

		LastAttackTime += Time.deltaTime;

		// 공격 중에 공격범위를 벗어나면
		if ( Vector2.Distance(player.position, transform.position) > attackRange)
		{
			curState = State.Trace;		// 다시 추적모드로
		}
	}

	void PatrolUpdate()
	{
		// 순찰 진행
		// patrolIndex = (patrolIndex + 1) % patrolPoints.Length;      // 다음 위치로 감
																	// patrolIndex + 1 하다가 인덱스 범위 넘어갈까보ㅓㅏ
		Vector2 dir = (patrolPoints[patrolIndex].position - transform.position).normalized;
		transform.Translate(dir * moveSpeed * Time.deltaTime);

		if (Vector2.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.02f )
		{
			curState = State.Idle;
		}
		else if (Vector2.Distance(player.position, transform.position) < detectRange)
		{
			curState= State.Trace;
		}

	}
	*/