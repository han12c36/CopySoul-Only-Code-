using UnityEngine;

public class Spirit_Weapon : Weapon
{
    public Spirit me;
    public Transform initPos;
    public Transform transPos;
    public bool att_close;

    protected override void weaponInitialize()
    {
        type = eWeaponType.Melee;
        Dmg = 1;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        if (owner != null) me = owner.GetComponent<Spirit>();
    }

    protected override void Update()
    {
        base.Update();

        if(me.curState_e == Enums.eSpiritState.Atk)
        {
            if(!att_close)
            {
                if (me.atting) me.weapon.WeaponColliderOnOff(true);
                else me.weapon.WeaponColliderOnOff(false);
            }
            else
            {
                me.weapon.WeaponColliderOnOff(false);
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.transform.root.GetComponent<Player>() != null)
        {
            if (!att_close)
            {
                att_close = true;
            }
        }
    }

    public void TransWeaponPos()
    {
        if (transPos == null) return;
        if(!me.preChangeWeaponPos)
        {
            TransPos();
            me.preChangeWeaponPos = true;
        }
    }

    public void ReturnWeaponPos()
    {
        if (initPos == null) return;

        if (me.preChangeWeaponPos)
        {
            ReturnPos();
            me.preChangeWeaponPos = false;
        }
    }

    public void TransPos()
    {
        transform.rotation = transPos.rotation;
    }

    public void ReturnPos()
    {
        transform.rotation = initPos.rotation;
    }
}

