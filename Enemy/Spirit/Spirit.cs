using UnityEngine;
using Enums;
using Structs;

/// <summary>
/// Enemy 파생 클래스
/// 근접 몬스터
/// Pool에 Return되고 Lent되는 시점을 기준으로 작성
/// </summary>
public class Spirit : Enemy
{
    public Material hitMaterial;
    public Material material;
    public GameObject trail;

    public Vector3 respawnPos;

    public Transform targetHeadPos;
    public Transform headPos;
    public Transform head;
    public GameObject model;

    public Transform RightHand;
    public Transform RightElbow;
    public Transform RightShoulder;


    //public GameObject ragdollModel;
    public GameObject remainderWeapon;
    public Spirit_Weapon weapon;
    public LayerMask player_Hitbox;
    public int preHp;
    public int prePatrol_Index;
    public eSpiritState curState_e;
    public float distToRespawnPos;

    public bool isGetUp;
    public float HoldCoolTime = 1.5f;
    public float Timer;

    public float initFOVAngle;
    public int curPatrol_Index;
    public bool Arrival;
    public bool complete_Equipt;
    public bool complete_Unequipt;
    public bool complete_Atk;
    public bool complete_AttReturn;
    public bool complete_Damaged;
    public bool complete_Groggy;
    public bool isEquipt;
    public bool atting;
    public bool existRemainderWeapon;
    public bool preChangeWeaponPos;
    public bool doubleAttCheck;
    public bool isReturn;
    public bool stepWait;
    public bool isBoneChanged;
    public bool timeSlow;

    //public bool isReset;

    /// <summary>
    /// 상태패턴의 State 생성
    /// </summary>
    public override void InitializeState()
	{
        fsm = new cState[(int)Enums.eSpiritState.End];

        fsm[(int)Enums.eSpiritState.Idle] = new Spirit_Idle();
        fsm[(int)Enums.eSpiritState.Patrol] = new Spirit_Patrol();
        fsm[(int)Enums.eSpiritState.Equipt] = new Spirit_Equipt();
        fsm[(int)Enums.eSpiritState.Unequipt] = new Spirit_Unequipt();
        fsm[(int)Enums.eSpiritState.Trace] = new Spirit_Trace();
        fsm[(int)Enums.eSpiritState.Atk] = new Spirit_Atk();
        fsm[(int)Enums.eSpiritState.Damaged] = new Spirit_Damaged();
        fsm[(int)Enums.eSpiritState.Groggy] = new Spirit_Groggy();
        fsm[(int)Enums.eSpiritState.Hold] = new Spirit_Hold();
        fsm[(int)Enums.eSpiritState.Death] = new Spirit_Death();
        fsm[(int)Enums.eSpiritState.Return] = new Spirit_Return();

        SetState((int)Enums.eSpiritState.Idle);
	}
	protected override void Awake()
    {
        base.Awake();

        Spirit_TrailOnOff();

        initFOVAngle = GetComponent<FieldOfView>().viewAngle;

        weapon = GetComponentInChildren<Spirit_Weapon>();
        if (weapon != null)
        { weapon.owner = gameObject; }

    }

    protected override void Start()
    {
        base.Start();

        respawnPos = transform.localPosition;

        targetObj = UnitManager.Instance.GetPlayerObj;
        if (targetObj != null) targetScript = UnitManager.Instance.GetPlayerScript;

        player_Hitbox = 1 << LayerMask.NameToLayer("Player_Hitbox");
        head = animCtrl.GetBoneTransform(HumanBodyBones.Head);
        RightHand = animCtrl.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        RightElbow = animCtrl.GetBoneTransform(HumanBodyBones.RightLowerArm);
        RightShoulder = animCtrl.GetBoneTransform(HumanBodyBones.RightUpperArm);
    }

    protected override void Update()
    {
        base.Update();

        distToRespawnPos = Vector3.Distance(respawnPos, transform.position);
        if (distToRespawnPos > status.moveMileage) isReturn = true;
        curState_e = GetCurState<Enums.eSpiritState>();

        if (status.curHp <= 0 && curState_e != eSpiritState.Hold)
        {
            status.isDead = true;
            SetState((int)Enums.eSpiritState.Death);
        }
        else if(status.curHp > 0 && !status.isDead)
        {
            if (status.isBackHold || status.isFrontHold )
            {
                //잡기
                if (status.isBackHold && status.isFrontHold)
                {
                    Debug.Log("뒤잡앞잡 동시 발동 : 판정 error");
                }
            }
        }

        //모종의 이유로 무기해제시
        if (curState_e == eSpiritState.Atk || curState_e == eSpiritState.Trace)
        {
            if (weaponEquipState == eEquipState.UnEquip) SetState((int)Enums.eSpiritState.Idle);
        }
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void ResetEnemy()
    {
        base.ResetEnemy();

        curState.ExitState();

        complete_Equipt = false;
        complete_Unequipt = false;
        complete_Atk = false;
        complete_AttReturn = false;
        complete_Damaged = false;
        complete_Groggy = false;
        isEquipt = false;
        atting = false;
        existRemainderWeapon = false;
        preChangeWeaponPos = false;
        doubleAttCheck = false;
        isReturn = false;
        stepWait = false;
        isBoneChanged = false;
        timeSlow = false;

        if (status.isDead)
        {
            //래그돌 없애기
            ragdoll.gameObject.SetActive(false);
            gameObject.SetActive(true);
            status.isDead = false;
            status.isFrontHold = false;
            status.isBackHold = false;
        }
        Debug.Log("spirit리셋");
        
        
        navAgent.enabled = true;
        navAgent.isStopped = true;
        transform.position = initPos;
        transform.forward = initForward;
        navAgent.isStopped = false;

        boneRotation(RightShoulder, Vector3.zero);
        boneRotation(RightElbow, Vector3.zero);
        boneRotation(RightHand, Vector3.zero);

        SetState((int)Enums.eSpiritState.Idle);

    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(other.tag == "ReturnBoundary" && curState_e != eSpiritState.Return && !status.isDead)
        {
            isReturn = true;
        }
    }


    public override void Hit(DamagedStruct dmgStruct)
    {
        base.Hit(dmgStruct);
        if (!status.isBackHold && !status.isFrontHold) HitCount++;
        else
        {
            if(HitCount > 0) HitCount = 0;
        }
    }



    //=============================================================================
    // 렉돌교체함수
    //=============================================================================
    public void ChangeToRagDoll()
    {
        if (model.activeSelf)
        {
            model.SetActive(false);
            GameObject ragdollObj = ragdoll.gameObject;
            ragdollObj.SetActive(true);
            ragdollObj.transform.position = transform.position;
            ragdollObj.transform.rotation = transform.rotation;
            Funcs.RagdollObjTransformSetting(transform, ragdollObj.transform);
        }
    }

    private void CopyCharacterTransfoemRoRagdoll(Transform origin, Transform ragdoll)
    {
        for(int i = 0; i < origin.childCount; i++)
        {
            if(origin.childCount != 0)
            {
                CopyCharacterTransfoemRoRagdoll(origin.GetChild(i), ragdoll.GetChild(i));
            }
            ragdoll.GetChild(i).localPosition = origin.GetChild(i).localPosition;
            ragdoll.GetChild(i).localRotation = origin.GetChild(i).localRotation;
        }
    }

    /// <summary>
    /// 상체 본만 회전하는 함수
    /// Mixamo Animation의 연계를 자연스럽게 하기 위함
    /// </summary>
    public void boneRotation(HumanBodyBones boneName, Transform targetTr, Vector3 offsetEulerRotate)
    {
        Transform boneTr = animCtrl.GetBoneTransform(boneName);
        boneTr.LookAt(targetTr);
        boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
    }

    public void boneRotation(Transform boneTr, Vector3 offsetEulerRotate)
    {
        boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
    }


    //=============================================================================

    //=============================================================================
    //instance
    //=============================================================================
    public void CreateRemainderWeapon(Transform trans)
    {
        if (!existRemainderWeapon)
        {
            existRemainderWeapon = true;
            GameObject obj = Instantiate(remainderWeapon);
            obj.transform.position = trans.position;
            obj.transform.rotation = trans.rotation;
        }
        else return;
    }

    //=============================================================================

    /// <summary>
    /// Animation Event
    /// </summary>
    public void Spirit_StepWait() { if (!stepWait) stepWait = true; }
    public void Spirit_StepStart() { if (stepWait) stepWait = false; }
    public void Spirit_Melee_CompleteEquiptment() { complete_Equipt = true; }
    public void Spirit_Melee_CompleteUnequiptment() { complete_Unequipt = true; }
    public void Spirit_Melee_CompleAtk() { complete_Atk = true; }
    public void Spirit_Melee_DoubleAttCheck() { doubleAttCheck = true; }
    public void PlayTimeSlow() { timeSlow = true; }
    public void Spirit_Melee_DoZoom_FrontHold(float speed)
    {
        CameraEffect.instance.PlayZoom(ZoomDir.Front, speed, true);
    }
    public void Spirit_Melee_StopZoom_FrontHold(float speed)
    {
        CameraEffect.instance.PlayZoom(ZoomDir.Front, speed, false);
    }

    public void Spirit_Melee_DoShake()
    {
        CameraEffect.instance.PlayShake("Enemy_FrontHold");
    }


    public void Spirit_Damaged() { complete_Damaged = true; }
    public void Spirit_Groggy() { complete_Groggy = true; }
    public void Spirit_Atting()
    {
        if(curState_e == Enums.eSpiritState.Atk)
        {
            if (!atting)
            {
                atting = true;
            }
            else if(atting)
            {
                atting = false;
            }
        }
    }

    public enum ShakeIndex
    {
        Spirit_Scream,
        Spirit_Damaged,
        End
    }

    public void DoShake(int index)
    {
        if(index == (int)ShakeIndex.Spirit_Scream)
        {
            CameraEffect.instance.PlayShake("Spirit_Scream");
        }
    }

    /// <summary>
    /// 각 Unit마다 적용되는 Sound는 본인 Script에서 처리
    /// </summary>
    public enum SoundIndex
    {
        Spirit_Scream,
        Spirit_Damaged,
        Spirit_Death,
        Spirit_NormalAtt,
        Spirit_SwingAtt,
        Spirit_DoubleAtt1,
        Spirit_DoubleAtt2,
        End
    }

    public void Spirit_Sound(int index)
    {
        if (index == (int)SoundIndex.Spirit_Scream)
        {
            SoundManager.Instance.PlaySound("Spirit_Scream", gameObject, 1f);
        }
        else if (index == (int)SoundIndex.Spirit_Death)
        {
            SoundManager.Instance.PlaySound("Spirit_Death", gameObject, 10f);
        }
        else if (index == (int)SoundIndex.Spirit_Damaged)
        {
            int i = Random.Range(1, 4);
            if(i == 1) SoundManager.Instance.PlaySound("Enemy_Damaged1", gameObject, 1f);
            else if(i == 2) SoundManager.Instance.PlaySound("Enemy_Damaged2", gameObject, 1f);
            else if (i == 3) SoundManager.Instance.PlaySound("Enemy_Damaged3", gameObject, 1f);
        }
        else if (index == (int)SoundIndex.Spirit_NormalAtt)
        {
            SoundManager.Instance.PlaySound("Spirit_NormalAtt", gameObject, 1f);
        }
        else if (index == (int)SoundIndex.Spirit_SwingAtt)
        {
            SoundManager.Instance.PlaySound("Spirit_SwingAtt", gameObject, 1f);
        }
        else if (index == (int)SoundIndex.Spirit_DoubleAtt1)
        {
            SoundManager.Instance.PlaySound("Spirit_DoubleAtt1", gameObject, 1f);
        }
        else if (index == (int)SoundIndex.Spirit_DoubleAtt2)
        {
            SoundManager.Instance.PlaySound("Spirit_DoubleAtt2", gameObject, 1f);
        }
    }


    public bool isCurrentAnimationOver(Animator animator,float time)
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > time;
    }

    //=============================================================================

    public void Spirit_TrailOnOff()
    {
        if(!trail.activeSelf) trail.SetActive(true);
        else trail.SetActive(false);
    }
}



