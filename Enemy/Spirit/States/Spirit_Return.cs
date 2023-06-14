using UnityEngine;

public class Spirit_Return : cState
{
    public bool Complete_Schouting;

    public override void EnterState(Enemy script)
    {
        base.EnterState(script);
        me.MoveStop();
        me.animCtrl.SetBool("isReturn", true);
        me.GetComponent<FieldOfView>().viewAngle = ((Spirit)me).initFOVAngle;
        me.col.enabled = false;
    }

    public override void UpdateState()
    {
        if (me.animCtrl.GetCurrentAnimatorStateInfo(0).IsName("ReturnStart"))
        {
            if (((Spirit)me).isCurrentAnimationOver(me.animCtrl, 1f))
            {
                if (!me.animCtrl.GetBool("isSchouting"))
                {
                    me.animCtrl.SetBool("isSchouting", true);
                    Complete_Schouting = true;
                }
            }
        }

        if(Complete_Schouting)
        {
            if(Vector3.Distance(((Spirit)me).respawnPos, me.transform.position) > 1f)
            {
                me.MoveOrder(((Spirit)me).respawnPos);
            }
            else if(Vector3.Distance(((Spirit)me).respawnPos, me.transform.position) <= 1f || me.transform.position == ((Spirit)me).respawnPos)
            {
                me.MoveStop();
                me.animCtrl.SetBool("isSchouting", false);
                me.animCtrl.SetBool("isReturn", false);
                Complete_Schouting = false;
                me.SetState((int)Enums.eSpiritState.Idle);
            }
        }
    }
    public override void ExitState()
    {
        me.animCtrl.SetBool("isReturn", false);
        Complete_Schouting = false;
        ((Spirit)me).isReturn = false;
        me.col.enabled = true;
    }
}
