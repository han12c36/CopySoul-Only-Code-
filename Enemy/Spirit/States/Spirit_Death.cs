using UnityEngine;

public enum DeathPattern
{
    Front,
    Back,
    End
}

public class Spirit_Death : cState
{
    public int DeathIndex;

    public override void EnterState(Enemy script)
    {
        base.EnterState(script);


        if (!me.isRouting)
        {
            me.isRouting = true;
            Inventory.Instance.Routing(me.transform.position);
        }

        if (!me.ragdoll.gameObject.activeSelf)
        {
            me.status.isDead = true;
            DeathIndex = Random.Range((int)DeathPattern.Front, (int)DeathPattern.End);
            me.animCtrl.SetBool("isDeath", true);
            me.animCtrl.SetFloat("AttIndex", DeathIndex);
        }
        me.navAgent.enabled = false;
        me.GetComponent<FieldOfView>().enabled = false;
        if(me.preState_i != 8) InGameManager.Instance.TimeStopEffect();

    }

    public override void UpdateState()
    {
        if (!me.ragdoll.gameObject.activeSelf)
        {
            if (me.animCtrl.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                if (((Spirit)me).isCurrentAnimationOver(me.animCtrl, 0.5f))
                {
                    ((Spirit)me).ChangeToRagDoll();
                    ((Spirit)me).DeathReset();
                }
            }
        }

    }
    public override void ExitState()
    {
        
    }
}
