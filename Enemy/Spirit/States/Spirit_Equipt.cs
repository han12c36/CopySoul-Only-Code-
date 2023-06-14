using UnityEngine;

//Equipt -> Unequipt(범위벗어남) -> Idle
//Equipt -> Trace(장비후 추격)
//Equipt -> Att

public class Spirit_Equipt : cState
{

    public override void EnterState(Enemy script)
    {
        base.EnterState(script);
        me.MoveStop();
        ((Spirit)me).isEquipt = true;
        me.weaponEquipState = eEquipState.Equip;
        me.GetComponent<FieldOfView>().viewAngle = 360f;
        me.animCtrl.SetBool("isEquipt", true);
    }

    public override void UpdateState()
    {
        me.MoveStop();

        if (me.status.isBackHold)
        {
            me.SetState((int)Enums.eSpiritState.Hold);
        }

        if (!me.status.isBackHold && !me.status.isFrontHold)
        {
            if (me.HitCount > 0)
            {
                if (((Spirit)me).curState_e != Enums.eSpiritState.Damaged)
                {
                    me.SetState((int)Enums.eSpiritState.Damaged);
                }
            }
        }

        if (((Spirit)me).complete_Equipt)
        {
            if (((Spirit)me).isReturn) me.SetState((int)Enums.eSpiritState.Return);

            if (me.combatState == eCombatState.Alert)
            {
                me.animCtrl.SetBool("isEquipt", false);
                if (me.distToTarget <= me.status.atkRange)
                {
                    me.SetState((int)Enums.eSpiritState.Atk);
                }
                else if (me.distToTarget > me.status.atkRange && me.distToTarget <= me.status.ricognitionRange)
                {
                    me.SetState((int)Enums.eSpiritState.Trace);
                }
            }
            else me.SetState((int)Enums.eSpiritState.Unequipt);
        }
        else
        {
        }
    }

    public override void LateUpdateState()
    {
        ((Spirit)me).boneRotation(((Spirit)me).RightHand, new Vector3(0f, 180f, 0f));
    }


    public override void ExitState()
    {
        ((Spirit)me).complete_Equipt = false;
        me.animCtrl.SetBool("isEquipt", false);
    }
}
