using UnityEngine;

public class Spirit_Patrol : cState
{
    public float curTime;
    public Vector3 targetPos;
    public bool isArrival = false;

    public override void EnterState(Enemy script)
    {
        base.EnterState(script);
        if (!isArrival)
        {
            MoveToNextWayPoint();
            me.navAgent.speed = me.status.moveSpd;
            me.animCtrl.SetBool("isWalk", true);
            me.animCtrl.SetBool("isPatrol", true);
        }
    }

    public override void UpdateState()
    {
        if (((Spirit)me).isReturn) me.SetState((int)Enums.eSpiritState.Return);

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

        if (me.combatState == eCombatState.Alert) me.SetState((int)Enums.eSpiritState.Equipt);
        else
        {
            if (Vector3.Distance(me.transform.position, targetPos) < 1f)
            {
                isArrival = true;
                me.MoveStop();
                me.animCtrl.SetBool("isWalk", false);
            }
            else isArrival = false;
        }
        if (isArrival) me.SetState((int)Enums.eSpiritState.Idle);
        else
        {
            Spirit_PatrolMoveOder();
        }
    }
    public override void LateUpdateState()
    {
        ((Spirit)me).boneRotation(((Spirit)me).RightShoulder, new Vector3(0, -10f, 0));
        ((Spirit)me).boneRotation(((Spirit)me).RightElbow, new Vector3(0, -25f, 0));
        ((Spirit)me).boneRotation(((Spirit)me).RightHand, new Vector3(0f, -15f, 90f));
    }


    public void Spirit_PatrolMoveOder()
    {
        if(((Spirit)me).stepWait) me.MoveStop();
        else me.MoveOrder(targetPos);
    }

    public override void ExitState()
    {
        isArrival = false;
        me.animCtrl.SetBool("isPatrol", false);
        me.animCtrl.SetBool("isWalk", false);
        me.MoveStop();
    }

    public void MoveToNextWayPoint()
    {
        int ranIndex = Random.Range(0, ((Spirit)me).PatrolPos.Length);
        bool isfalse = false;
        if (ranIndex < 0 || ranIndex == ((Spirit)me).curPatrol_Index || ranIndex > ((Spirit)me).PatrolPos.Length) isfalse = true;

        if(!isfalse)
        {
            ((Spirit)me).prePatrol_Index = ((Spirit)me).curPatrol_Index;
            ((Spirit)me).curPatrol_Index = ranIndex;

            targetPos = ((Spirit)me).PatrolPos[ranIndex].position;
            return;
        }
        else MoveToNextWayPoint();
    }
}
