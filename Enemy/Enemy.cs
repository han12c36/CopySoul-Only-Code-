using System;
using UnityEngine;
using UnityEngine.AI;
using Enums;
using Structs;

public enum eCombatState
{
    Idle, 
    Alert, 
    Combat, 
    End
}

public enum eEquipState
{
    UnEquip, 
    Equip, 
    End
}

/// <summary>
/// 몬스터 최상단 부모
/// 행동들을 상태패턴으로 관리
/// 공통적으로 사용되는 기능을 가짐
/// </summary>

public abstract class Enemy : MonoBehaviour
{
    public Structs.EnemyStatus status;
    public Collider DamagedCollider;

    [HideInInspector]
    public Vector3 initPos;
    public Vector3 initForward;

    public GameObject targetObj;
    public Player targetScript;
    public float distToTarget;
    public Vector3 dirToTarget;


    public Transform headTr;
    
    public int HitCount;

    public eCombatState combatState = eCombatState.Idle;
    public eEquipState weaponEquipState = eEquipState.UnEquip;

    public Transform[] PatrolPos;

    public Enemy_Ragdoll ragdoll;
    public HpBar hpBar = null;

    ////FSM
    public cState[] fsm;
    public cState preState = null;
    public int preState_i = -1;
    public cState curState = null;
    public int curState_i = -1;
    ////FSM

    ////default Components
    public Animator animCtrl;
    public Collider col;
    public Rigidbody rd;
    public NavMeshAgent navAgent;
    ////default Components

    public bool isRouting;

    public void SetInitTr(Vector3 pos, Vector3 forward)
    {
        initPos = pos;
        initForward = forward;
    }
    public virtual void DeathReset()
    {
        UnitManager.Instance.EraseDeathEnemy(this);
    }

    public virtual void ResetEnemy()
    {
        status.curHp = status.maxHp;
        status.curMp = status.maxMp;
        status.curStamina = status.maxStamina;
    }
    


    public void HoldTransPos_Enemy(Transform dir,Vector3 forwardVec)
    {
        transform.position = forwardVec;
        if(status.isBackHold) transform.forward = dir.forward;
        else transform.forward = -dir.forward;
    }

    public void ResetAllAnimTrigger(string[] triggerStrArr)
    {
        for (int i = 0; i < triggerStrArr.Length; ++i)
            animCtrl.ResetTrigger(triggerStrArr[i]);
    }

    public void CalcAboutTarget()
    {
        //if (targetObj == null)
        //{ return; }
        //
        //distToTarget = Vector3.Distance(transform.position, targetObj.transform.position);
        //dirToTarget = (targetObj.transform.position - transform.position).normalized;
    }

	public Quaternion LookAtSlow_Rotation(Transform me, Transform target, float spd)
	{
        Vector3 tempDir = dirToTarget;
        tempDir.y = 0;

        Quaternion angle = Quaternion.LookRotation(tempDir);

        return Quaternion.Lerp(me.rotation, angle, Time.deltaTime * spd);
    }
    public Quaternion LookAtSlow_Rotation(Transform me, Vector3 targetPos, float spd)
    {
        Vector3 tempDir = (targetPos - me.position).normalized;
        tempDir.y = 0;

        Quaternion angle = Quaternion.LookRotation(tempDir);

        return Quaternion.Lerp(me.rotation, angle, Time.deltaTime * spd);
    }

    public void LookAtSlow(Transform me, Transform target, float spd)
    {
        Vector3 tempDir = dirToTarget;
        tempDir.y = 0;

        Quaternion angle = Quaternion.LookRotation(tempDir);
        
        transform.rotation = Quaternion.Lerp(me.rotation, angle, Time.deltaTime * spd);
    }


    #region LookAt_Animation Bone

    public void LookAtSpecificBone(HumanBodyBones boneName, Transform targetTr, Vector3 offsetEulerRotate)
    {
        Transform boneTr = animCtrl.GetBoneTransform(boneName);
        boneTr.LookAt(targetTr);
        boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
    }

    public void LookAtSpecificBone(Transform boneTr, Transform targetTr, eGizmoDirection boneDir)
    {
        Vector3 lookDir = (targetTr.position - boneTr.position).normalized;

        switch (boneDir)
        {
            case eGizmoDirection.Foward:
                {
                    boneTr.forward = lookDir;
                }
                break;
            case eGizmoDirection.Back:
                {
                    boneTr.forward = -lookDir;
                }
                break;
            case eGizmoDirection.Right:
                {
                    boneTr.right = lookDir;
                }
                break;
            case eGizmoDirection.Left:
                {
                    boneTr.right = -lookDir;
                }
                break;
            case eGizmoDirection.Up:
                {
                    boneTr.up = lookDir;
                }
                break;
            case eGizmoDirection.Down:
                {
                    boneTr.up = -lookDir;
                }
                break;

            default:
                {
                    Debug.Log("Enemy bone LookAt Error");
                }
                break;
        }
    }

    public void LookAtSpecificBone(Transform boneTr, Transform targetTr, eGizmoDirection boneDir, Vector3 offsetEulerRotate)
    {
        Vector3 lookDir = (targetTr.position - boneTr.position).normalized;

        switch (boneDir)
		{
			case eGizmoDirection.Foward:
                {
                    boneTr.forward = lookDir;
                }
				break;
			case eGizmoDirection.Back:
                {
                    boneTr.forward = -lookDir;
                }
				break;
			case eGizmoDirection.Right:
                {
                    boneTr.right = lookDir;
                }
				break;
			case eGizmoDirection.Left:
                {
                    boneTr.right = -lookDir;
                }
				break;
			case eGizmoDirection.Up:
                {
                    boneTr.up = lookDir;
                }
				break;
			case eGizmoDirection.Down:
                {
                    boneTr.up = -lookDir;
                }
				break;

            default:
                {
                    Debug.Log("Enemy bone LookAt Error");
                }
				break;
		}

		boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
    }

    public void LookAtSpecificBone(Transform boneTr, Vector3 targetPos, eGizmoDirection boneDir, Vector3 offsetEulerRotate)
    {

        Vector3 lookDir = (targetPos - boneTr.position).normalized;

        switch (boneDir)
        {
            case eGizmoDirection.Foward:
                {
                    boneTr.forward = lookDir;
                }
                break;
            case eGizmoDirection.Back:
                {
                    boneTr.forward = -lookDir;
                }
                break;
            case eGizmoDirection.Right:
                {
                    boneTr.right = lookDir;
                }
                break;
            case eGizmoDirection.Left:
                {
                    boneTr.right = -lookDir;
                }
                break;
            case eGizmoDirection.Up:
                {
                    boneTr.up = lookDir;
                }
                break;
            case eGizmoDirection.Down:
                {
                    boneTr.up = -lookDir;
                }
                break;

            default:
                {
                    Debug.Log("Enemy bone LookAt Error");
                }
                break;
        }

        boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
    }
    #endregion

    public void SetDestination(Vector3 dest)
    {
        if (navAgent == null || navAgent.isStopped) return;
        navAgent.destination = dest;
    }

    public void MoveOrder(Vector3 dest)
    {
        if (navAgent == null) return;

        navAgent.isStopped = true;
        navAgent.destination = dest;
        navAgent.isStopped = false;
    }

    public void MoveStop()
    {
        if (navAgent == null) return;

        navAgent.isStopped = true;
        navAgent.SetDestination(transform.position);
    }

 


    public abstract void InitializeState();

	public T GetCurState<T>() where T : Enum
    {
        int index = System.Array.IndexOf(fsm, curState);

        if (curState == null)
        {
            Debug.Log("현재 state가 null입니다!!\nAt GetCurState Funcs");
        }

        return (T)(object)index;
    }


    public void SetState(cState state)
    {
        
        int index = System.Array.IndexOf(fsm, state);

        if (index == -1 || curState == state) return;

        if (curState != null) curState.ExitState();

        int curIndex = System.Array.IndexOf(fsm, curState);

        preState = curState;
        preState_i = curIndex;

        curState = state;
        curState_i = index;

        curState.EnterState(this);
    }


    public void SetState(int state)
    {
        if (fsm[state] == null || curState == fsm[state]) return;

        if (curState != null) curState.ExitState();

        cState nextState = fsm[state];
        int curIndex = System.Array.IndexOf(fsm, curState);

        preState = curState;
        preState_i = curIndex;

        curState = nextState;
        curState_i = state;

        curState.EnterState(this);
    }

    public void RestartCurState()
    {
        curState.ExitState();

        preState = curState;
        
        int curIndex = System.Array.IndexOf(fsm, curState);
        preState_i = curIndex;

        curState.EnterState(this);
    }

    public void stop()
    {
        StopAllCoroutines();
    }

    protected virtual void Awake()
    {
        SetInitTr(transform.position, transform.forward);

        col = GetComponent<Collider>();
        animCtrl = GetComponent<Animator>();
        rd = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        InitializeState();
    }

    protected virtual void Start()
    {
		if (status.name_e != eEnemyName.Golem)
		{
			hpBar = UiManager.Instance.InstantiateHpBar(this);
		}
	}

    public void GetPlayerState()
    {

    }

    protected virtual void Update()
    {
        if (curState != null)
        { curState.UpdateState(); }

        CalcAboutTarget();
    }

    protected virtual void FixedUpdate()
    {
        if (curState != null)
        { curState.FixedUpdateState(); }
    }

    protected virtual void LateUpdate()
    {
        if (curState != null)
        { curState.LateUpdateState(); }
    }

    protected virtual void OnTriggerEnter(Collider other) { }

    //=============================================================

    public virtual void Hit(DamagedStruct dmgStruct)
    {
        status.curHp -= (int)dmgStruct.dmg;

        if (dmgStruct.isBackstab)
        {
            if(!status.isBackHold) status.isBackHold = true;

        }
        else if(dmgStruct.isRiposte)
        {
            if (!status.isFrontHold) status.isFrontHold = true;
        }

    }

    protected virtual void OnDrawGizmosSelected()
	{
        ////Dir to Target
        if(targetObj != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, targetObj.transform.position);
        }
        ////Dir to Target
        Color temp = Color.yellow;
        temp.a = 0.4f;
        //Gizmos.color = Color.yellow;
        Gizmos.color = temp;
        Gizmos.DrawSphere(transform.position, status.ricognitionRange);
        ////인식범위

        
        ////공격 사정거리
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, status.atkRange);
		////공격 사정거리
	}
}

