using System.Collections.Generic;
using UnityEngine;
using Structs;

public enum eWeaponType
{
    None,
    Melee,
    Sheild,
    Arrow,
    Range,

    End
}


//owner 연결 해줍시당
//weapon 상속받은거 각자 필요한 유닛에게 적용시키기

public abstract class Weapon : MonoBehaviour
{
    List<GameObject> hitObjs;

    public Collider col;
    public eWeaponType type;
    public int Dmg;
    public Enums.eAttackType atkType = Enums.eAttackType.End;
    public GameObject owner;

    public LayerMask PlayerLayer;
    public LayerMask EnemyLayer;

    protected abstract void weaponInitialize();

    protected virtual void Awake()
    {
        weaponInitialize();
        col = GetComponent<Collider>();
        PlayerLayer = LayerMask.GetMask("Player_Hitbox");
        EnemyLayer = LayerMask.GetMask("Enemy");

        hitObjs = new List<GameObject>();
	}

    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void LateUpdate()
    {
    }

    public void WeaponColliderOnOff(bool value)
    {
        if (owner != null)
        {
            col.enabled = value;
            hitObjs.Clear();
        }
    }

    public void WeaponColliderOnOff(int value)
    {
        if (owner != null)
        {
            col.enabled = Funcs.I2B(value);
            hitObjs.Clear();
        }
    }
    //===============================================================================================================================


    /// <summary>
    /// 패링시 -> 스턴 + 데미지
    /// 압잡뒤잡조건 -> 스턴이 걸린상태에서 딜을 넣는 기술
    /// 몬스터 강공처리
    /// dmgStruct.atkType = Enums.eAttackType.Strong;
    /// </summary>

    //===============================================================================================================================
    // 데미지 주고받기
    public void Att(GameObject HittedObj, Collider other)
    {
        //맞은 놈 : player
        if (HittedObj.transform.root.GetComponent<Player>() != null)
        {
            Structs.DamagedStruct dmgStruct = new DamagedStruct();

            dmgStruct.dmg = Dmg;
            dmgStruct.attackObj = owner;
            dmgStruct.atkType = atkType;

            PlayerActionTable temp = HittedObj.transform.root.GetComponent<Player>().playerAt;

            if (temp != null)
            {
                if(Player.instance.status.isParrying == true)
                {
                    ParryingToEnemy(owner.GetComponent<Enemy>());
                    GameObject effect = ObjectPoolingCenter.Instance.LentalObj("EtherealHit", 1);
                    SoundManager.Instance.PlaySound("Shield_Guard", this.gameObject);
                    effect.transform.position = other.ClosestPoint(HittedObj.transform.position);
                    effect.GetComponent<ParticleSystem>().Play();
                    Debug.Log("Parrying");

                }
                else if(Player.instance.status.isInvincible)
                {

                }
                else
                {
                    GameObject effect = ObjectPoolingCenter.Instance.LentalObj("ScifiTris 1", 1);
                    effect.transform.position = other.ClosestPoint(HittedObj.transform.position);
                    temp.Hit(dmgStruct);
                    CameraEffect.instance.PlayShake("Player_Damaged");
                    Debug.Log("Hit");
                }
            }
        }

        //맞은 놈 : enemy 
        else if (HittedObj.GetComponent<Enemy>() != null)
        {
            if (!HittedObj.GetComponent<Enemy>().status.isDead)
            {
                Structs.DamagedStruct dmgStruct = new DamagedStruct();

                dmgStruct.dmg = Dmg;
                dmgStruct.attackObj = owner;
                dmgStruct.atkType = atkType;

                PlayerActionTable temp = HittedObj.transform.root.GetComponent<Player>().playerAt;

                temp.Hit(dmgStruct);


                //HittedObj.GetComponent<Enemy>().status.curHp -= Dmg;
            }
            else
            {
                return;
            }
        }
    }

    //===============================================================================================================================

    //===============================================================================================================================
    // ParryingToSpirit
    //===============================================================================================================================
    public void ParryingToEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (enemy.GetComponent<Spirit>() == null) return;
        if (enemy.GetCurState<Enums.eSpiritState>() == Enums.eSpiritState.Atk)
        {
            if (enemy.GetComponent<Spirit>().atting && !enemy.GetComponent<Spirit>().status.isGroggy)
            {
                enemy.GetComponent<Spirit>().status.isGroggy = true;
            }
        }
    }
    //===============================================================================================================================


    //===============================================================================================================================
    // trigger

    public virtual void OnTriggerEnter(Collider other)
    {
        if (hitObjs.Find(x => x == other.transform.root.gameObject))
        {
            return;    
        }

        //if(owner.GetComponent<Spirit>() != null)
        //{
        //    if (other.gameObject.layer == 9)
        //    {
        //        CameraEffect.instance.PlayShake("FrictionToEnvironment");
        //    }
        //}    
        

        //Enemy -> Player
        if (owner.gameObject.GetComponent<Enemy>() != null)
        {
            if(!owner.gameObject.GetComponent<Enemy>().status.isDead)
            {
                if(other.transform.root.GetComponent<Player>() != null)
                {
					Att(other.gameObject, other);
					hitObjs.Add(other.transform.root.gameObject);
				}
            }
        }

    }
    //===============================================================================================================================

}

