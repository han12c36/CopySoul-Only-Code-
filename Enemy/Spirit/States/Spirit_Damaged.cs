using UnityEngine;

public class Spirit_Damaged : cState
{
    public override void EnterState(Enemy script)
    {
        base.EnterState(script);

        me.animCtrl.SetBool("isDamaged", true);
        me.animCtrl.SetBool("ChangeDamaged", true);
        ((Spirit)me).GetComponentInChildren<SkinnedMeshRenderer>().material = ((Spirit)me).hitMaterial;
        me.GetComponent<FieldOfView>().viewAngle = 360f;

        UiManager.Instance.ppController.DoBloom(10f, 0.6f, 0.1f);

        ((Spirit)me).HitCount--;
    }

    public override void UpdateState()
    {
        if (me.status.isBackHold)
        {
            me.SetState((int)Enums.eSpiritState.Hold);
        }

        if(((Spirit)me).HitCount > 1)
        {
            if (me.animCtrl.GetBool("isDamaged")) me.animCtrl.SetBool("isDamaged", false);
            else me.animCtrl.SetBool("isDamaged", true);
            ((Spirit)me).HitCount--;
        }
        else
        {
            if (((Spirit)me).complete_Damaged)
            {
                //((Spirit)me).HitCount--;
                //me.animCtrl.SetBool("ChangeDamaged", false);

                if (((Spirit)me).isReturn) me.SetState((int)Enums.eSpiritState.Return);

                if (me.combatState == eCombatState.Alert)
                {
                    if (me.weaponEquipState == eEquipState.Equip)
                    {
                        if (me.distToTarget <= me.status.atkRange)
                        {
                            me.SetState((int)Enums.eSpiritState.Atk);
                        }
                        else if (me.distToTarget > me.status.atkRange && me.distToTarget <= me.status.ricognitionRange)
                        {
                            me.SetState((int)Enums.eSpiritState.Trace);
                        }
                    }
                    else me.SetState((int)Enums.eSpiritState.Equipt);
                }
                else me.SetState((int)Enums.eSpiritState.Unequipt);
            }
        }
    }

    public override void ExitState()
    {
        me.animCtrl.SetBool("ChangeDamaged", false);
        me.animCtrl.SetBool("isDamaged", false);
        ((Spirit)me).complete_Damaged = false;
        ((Spirit)me).HitCount = 0;
        ((Spirit)me).GetComponentInChildren<SkinnedMeshRenderer>().material = ((Spirit)me).material;
    }
}
