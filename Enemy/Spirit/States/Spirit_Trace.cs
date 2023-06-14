using UnityEngine;

//Trace -> Unquipt
//Trace -> Att

public class Spirit_Trace : cState
{
    public override void EnterState(Enemy script)
    {
        base.EnterState(script);
        me.animCtrl.SetBool("isTrace", true);
        me.animCtrl.SetBool("isRun", true);
        me.navAgent.speed = me.status.runSpd;
        me.MoveOrder(me.targetObj.transform.position);
    }

    public override void UpdateState()
    {
        if (((Spirit)me).isReturn) me.SetState((int)Enums.eSpiritState.Return);

        me.SetDestination(me.targetObj.transform.position);

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

        if (me.status.isBackHold)
        {
            me.SetState((int)Enums.eSpiritState.Hold);
        }

        if (me.combatState == eCombatState.Alert)
        {
            if (me.distToTarget <= me.status.atkRange)
            {
                me.SetState((int)Enums.eSpiritState.Atk);
            }
            else if (me.distToTarget > me.status.ricognitionRange)
            {
                me.SetState((int)Enums.eSpiritState.Unequipt);
            }
        }
        else me.SetState((int)Enums.eSpiritState.Unequipt);
    }

    public override void LateUpdateState()
    {
        ((Spirit)me).boneRotation(HumanBodyBones.Head, ((Spirit)me).targetHeadPos, new Vector3(0, 0, 0));

        ((Spirit)me).boneRotation(((Spirit)me).RightShoulder, new Vector3(0, -10f, 45f));
        ((Spirit)me).boneRotation(((Spirit)me).RightElbow, new Vector3(0, -15f, 0));
        ((Spirit)me).boneRotation(((Spirit)me).RightHand, new Vector3(45f, -15f, 45f));
    }

    public override void ExitState()
    {
        me.animCtrl.SetBool("isTrace", false);
        me.animCtrl.SetBool("isRun", false);
        me.MoveStop();
    }
}
