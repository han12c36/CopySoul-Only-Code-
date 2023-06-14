using UnityEngine;

public class Spirit_Idle : cState
{
    public float curTime = 0;
    public float patrolWaitTime = 7f;

    public override void EnterState(Enemy script)
    {
        base.EnterState(script);
        me.MoveStop();
        me.animCtrl.SetBool("isIdle", true);
    }

    public override void UpdateState()
    {
        curTime += Time.deltaTime;
        if (me.status.curHp < me.status.maxHp)
        {
            me.status.curHp++;
        }
        if (me.combatState == eCombatState.Alert) me.SetState((int)Enums.eSpiritState.Equipt);
        if (curTime >= patrolWaitTime)
        {
            curTime = 0;
            me.SetState((int)Enums.eSpiritState.Patrol);
        }

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

        if(((Spirit)me).isReturn)
        {
        }
    }
     
    public override void LateUpdateState()
    {
        ((Spirit)me).boneRotation(((Spirit)me).RightShoulder, new Vector3(0, -20f, 0));
        ((Spirit)me).boneRotation(((Spirit)me).RightElbow, new Vector3(0, -90f, 0));
        ((Spirit)me).boneRotation(((Spirit)me).RightHand, new Vector3(0, 15f, 0));
    }

    public override void ExitState()
    {
        me.animCtrl.SetBool("isIdle", false);
    }
}
