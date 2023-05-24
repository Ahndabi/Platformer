using BeeState;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// using static UnityEngine.RuleTile.TilingRuleOutput;		�̰� �ϸ� Transform, Transform[]�� ������

public class Bee : MonoBehaviour
{
	// 1. �÷��̾ �ָ� ���� �� ������ �ֱ�
	// 2. �÷��̾ ������� ���������, �÷��̾ �����ϵ��� ����
	// 2-1 ���� �߿� �ʹ� �־����� �i�ư��� ��������� �ٽ� ���ڸ�
	// 2-2 ���� �߿� ���ݹ��� �ȿ� ������ ����

	public enum State { Idle, Trace, Return, Attack, Patrol, Size }


	StateBase<Bee>[] states;      // ���µ��� ��� ���� (��ųʸ� ���·ε� ������ �ֱ⵵ ��)
	[SerializeField] State curState;

	[SerializeField] public float detectRange;     // ������ �� �ִ� ����
	[SerializeField] public float moveSpeed;       // �ӵ�
	[SerializeField] public float attackRange;     // ������ �� �ִ� ����
	[SerializeField] public float LastAttackTime;      // ������ ���� �ð�(?)
	[SerializeField] public Transform[] patrolPoints;      // ������ �� �ִ� ����Ʈ��

	public Transform player;           // �÷��̾� ��ġ
	public Vector3 returnPosition;     // ���ư� ��ġ
	public int patrolIndex = 0;        // ���� ���� �����ϰ� �ִ� ��?


	private void Awake()
	{
		states = new StateBase<Bee>[(int)State.Size];   // Size�� ���������� ���� �� 5��°(�ε�������)�ϱ�, 5�� ��
		states[(int)State.Idle] = new IdleState(this);		// states[0]
		states[(int)State.Trace] = new TraceState(this);    // states[1]	
		states[(int)State.Return] = new ReturnState(this);  // states[2]
		states[(int)State.Attack] = new AttackState(this);  // states[3]
		states[(int)State.Patrol] = new PatrolState(this);  // states[4]
	}

	private void Start()
	{
		curState = State.Idle;      // ó�� ���� ���´� ������ �ִ� �ŷ� ����.
		states[(int)curState].Enter();

		player = GameObject.FindGameObjectWithTag("Player").transform;
		returnPosition = transform.position;    // ���ư� ��ġ�� �������� ���� �� ��ġ�� ����
	}

	private void Update()
	{
		states[(int)curState].Update();
	}

	public void ChangeState(State state)		// ���� ���·� �ٲپ���
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

		
		public IdleState(Bee bee) : base(bee)   // ������ ��� Bee,�� bee�� �Ű������� �־����� IdleState�� Bee�� �̿밡��
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
			idleTime = 0;		// �������� �� idle�ð��� 0���� ����
		}

		public override void Exit()
		{

		}

		public override void Update()		// ���°� ���� ���� �� �� ��
		{
			idleTime += Time.deltaTime;
			
			// �ƹ��͵� �� �ϱ�

			if (idleTime > 2)       // idle ���°� 2�� �̻� ������ Patrol ���·� ����
			{
				// idleTime = 0;
				//bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length; �̰� Patrol�� Enter�� �ű�
				bee.ChangeState(Bee.State.Patrol);
			}

			// ���� �÷��̾�� ����� ����,
			if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
			{
				bee.ChangeState(Bee.State.Trace);		// �������·� ��ȯ
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
			// �÷��̾� �Ѿư���
			Vector2 dir = (bee.player.position - bee.transform.position).normalized;
			// normalized : ���Ͱ� ũ�� �۵�, ���͸� 1�� ������� (������ ����ȭ ������)
			bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

			// ���� �÷��̾ �־�����,
			if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
			{
				// ���� ���·� ��ȯ��Ŵ
				bee.ChangeState(Bee.State.Return);
			}

			// ���ݹ��� �ȿ����� ��
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
			// �����ϱ�
			// 1�ʸ��� �����ϰԲ� ����
			if (bee.LastAttackTime > 1)
			{
				Debug.Log("����");
				bee.LastAttackTime = 0;
			}

			bee.LastAttackTime += Time.deltaTime;

			// ���� �߿� ���ݹ����� �����
			if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.attackRange)
			{
				bee.ChangeState(Bee.State.Trace);		// �ٽ� ��������
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
			// ���� �ڸ��� ���ư���
			Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
			// normalized : ���Ͱ� ũ�� �۵�, ���͸� 1�� ������� (������ ����ȭ ������)
			bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

			// ���� �ڸ��� ����������,
			// ����Ƽ float�� �Ҽ��� �����̶� ����� transform.position == returnposition�� �ٸ��ٰ� ������ ������
			// < 0.02f ó�� ������ �������ִ� ���� ����
			if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.02f)
			{
				bee.ChangeState(Bee.State.Idle);
			}
			// ���ư��� �߿��� �÷��̾ ������ �Ǹ� Trace�� �ǰԲ�
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
			// ó���� ���� Ž�� ������ ������� ��!
			bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;      // ���� ��ġ�� ��
			// patrolIndex + 1 �ϴٰ� �ε��� ���� �Ѿ���
		}

		public override void Exit()
		{
		}

		public override void SetUp()
		{
		}

		public override void Update()
		{
			// ���� ����
			//bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;      // ���� ��ġ�� ��

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
		// �ƹ��͵� �� �ϱ�
		
		if(idleTime > 2)		// idle ���°� 2�� �̻� ������ Patrol ���·� ����
		{
			idleTime = 0;
			patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
			curState = State.Patrol;
		}
		idleTime += Time.deltaTime;

		// ���� �÷��̾�� ����� ����,
		if(Vector2.Distance(player.position, transform.position) < detectRange)
		{
			// ���� ���·� ��ȯ��Ŵ
			curState = State.Trace;
		}
	}

	void TraceUpdate()
	{
		// �÷��̾� �Ѿư���
		Vector2 dir = (player.position - transform.position).normalized;
		// normalized : ���Ͱ� ũ�� �۵�, ���͸� 1�� ������� (������ ����ȭ ������)
		transform.Translate(dir * moveSpeed * Time.deltaTime);

		// ���� �÷��̾ �־�����,
		if (Vector2.Distance(player.position, transform.position) > detectRange)
		{
			// ���� ���·� ��ȯ��Ŵ
			curState = State.Return;
		}

		// ���ݹ��� �ȿ����� ��
		else if(Vector2.Distance(player.position, transform.position) < attackRange)
		{
			curState = State.Attack;
		}
	}

	void ReturnUpdate()
	{
		// ���� �ڸ��� ���ư���
		Vector2 dir = (returnPosition - transform.position).normalized;
		// normalized : ���Ͱ� ũ�� �۵�, ���͸� 1�� ������� (������ ����ȭ ������)
		transform.Translate(dir * moveSpeed * Time.deltaTime);

		// ���� �ڸ��� ����������,
		// ����Ƽ float�� �Ҽ��� �����̶� ����� transform.position == returnposition�� �ٸ��ٰ� ������ ������
		// < 0.02f ó�� ������ �������ִ� ���� ����
		if (Vector2.Distance(transform.position, returnPosition) < 0.02f )
		{
			curState = State.Idle;
		}
		// ���ư��� �߿��� �÷��̾ ������ �Ǹ� Trace�� �ǰԲ�
		else if(Vector2.Distance(player.position, transform.position) < detectRange)
		{
			curState = State.Trace;
		}

	}
	
	void AttackUpdate()
	{
		// �����ϱ�
		// 1�ʸ��� �����ϰԲ� ����
		if(LastAttackTime > 1)
		{
			Debug.Log("����");
			LastAttackTime = 0;
		}

		LastAttackTime += Time.deltaTime;

		// ���� �߿� ���ݹ����� �����
		if ( Vector2.Distance(player.position, transform.position) > attackRange)
		{
			curState = State.Trace;		// �ٽ� ��������
		}
	}

	void PatrolUpdate()
	{
		// ���� ����
		// patrolIndex = (patrolIndex + 1) % patrolPoints.Length;      // ���� ��ġ�� ��
																	// patrolIndex + 1 �ϴٰ� �ε��� ���� �Ѿ��ä�
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