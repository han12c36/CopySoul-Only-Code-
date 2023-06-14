using UnityEngine;

[System.Serializable]
public enum eSpirit_AtkPattern
{
    NormalAtk,
    DoubleAtk,
    TurnAtt,
    End
}
public class Spirit_Atk : cState
{

    public eSpirit_AtkPattern CurPattern;
    public bool startPattern;
    public int AttIndex;

    public override void EnterState(Enemy script)
    {
        base.EnterState(script);
        ((Spirit)me).complete_Atk = true;
        me.animCtrl.SetBool("isAtk", true);
    }

    public override void UpdateState()
    {
        if (((Spirit)me).weapon == null) return;

        if (me.status.isGroggy)
        {
            me.SetState((int)Enums.eSpiritState.Groggy);
        }
        else if (me.status.isBackHold)
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

        if (!((Spirit)me).complete_Atk)
        {
            if (((Spirit)me).isReturn) me.SetState((int)Enums.eSpiritState.Return);

            if (CurPattern == eSpirit_AtkPattern.DoubleAtk)
            {
                if (((Spirit)me).doubleAttCheck)
                {
                    ((Spirit)me).doubleAttCheck = false;
                    if (((Spirit)me).weapon.att_close)
                    {
                        ((Spirit)me).weapon.att_close = false;
                    }
                }
            }

            Play(CurPattern);
        }
        else if(((Spirit)me).complete_Atk)
        {
            stop(CurPattern);
            ((Spirit)me).trail.SetActive(false);
            ((Spirit)me).complete_Atk = false;
            ((Spirit)me).weapon.att_close = false;
            if (me.combatState == eCombatState.Alert)
            {
                if (me.distToTarget <= me.status.atkRange) Select();
                else if (me.distToTarget > me.status.atkRange) me.SetState((int)Enums.eSpiritState.Trace);
            }
            else me.SetState((int)Enums.eSpiritState.Unequipt);
        }


    }

    public override void ExitState()
    {
        stop(CurPattern);
        if (((Spirit)me).complete_Atk) ((Spirit)me).complete_Atk = false;
        startPattern = false;
        me.animCtrl.SetBool("isAtk", false);
        if(((Spirit)me).atting) ((Spirit)me).atting = false;
        ((Spirit)me).weapon.att_close = false;
        if (((Spirit)me).weapon.col.enabled == true) ((Spirit)me).weapon.WeaponColliderOnOff(false);
        ((Spirit)me).weapon.ReturnWeaponPos();
    }

    public void Select()
    {
        if (!startPattern)
        {
            int AttPatternIndex;
            AttPatternIndex = Random.Range(((int)eSpirit_AtkPattern.NormalAtk), ((int)eSpirit_AtkPattern.End));
            //AttPatternIndex = (int)eSpirit_AtkPattern.NormalAtk;
            AttIndex = AttPatternIndex;

            CurPattern = (eSpirit_AtkPattern)AttPatternIndex;
            me.transform.LookAt(me.targetObj.transform);

            if (CurPattern == eSpirit_AtkPattern.DoubleAtk || CurPattern == eSpirit_AtkPattern.TurnAtt)
            {
                ((Spirit)me).weapon.TransWeaponPos();
            }
            startPattern = true;
        }
    }
   
    public void Play(eSpirit_AtkPattern CurPattern)
    {

        switch (CurPattern)
       {
           case eSpirit_AtkPattern.NormalAtk:
               PlayNormalAtk();
               break;
           case eSpirit_AtkPattern.DoubleAtk:
               PlayDoubleAtk();
               break;
           case eSpirit_AtkPattern.TurnAtt:
               PlayTurnAtk();
               break;
           case eSpirit_AtkPattern.End:
               break;
           default:
               break;
       }
    }

    public void stop(eSpirit_AtkPattern curPattern)
    {
        if (startPattern) startPattern = false;


        switch (CurPattern)
        {
            case eSpirit_AtkPattern.NormalAtk:
                StopNormalAtk();
                break;
            case eSpirit_AtkPattern.DoubleAtk:
                StopDoubleAtk();
                break;
            case eSpirit_AtkPattern.TurnAtt:
                StopTurnAtk();
                break;
            case eSpirit_AtkPattern.End:
                break;
            default:
                break;
        }
    }

    public void PlayNormalAtk()
    {
        if (!me.animCtrl.GetBool("isNormalAtk"))
        {
            me.animCtrl.SetFloat("AttIndex", AttIndex);
            me.animCtrl.SetBool("isNormalAtk", true);
        }
    }

    public void StopNormalAtk()
    {
        if (me.animCtrl.GetBool("isNormalAtk"))
        {
            me.animCtrl.SetBool("isNormalAtk", false);
        }
    }

    public void PlayDoubleAtk()
    {
        if (!me.animCtrl.GetBool("isDoubleAtk"))
        {
            me.animCtrl.SetFloat("AttIndex", AttIndex);
            me.animCtrl.SetBool("isDoubleAtk", true);
        }
    }

    public void StopDoubleAtk()
    {
        if (me.animCtrl.GetBool("isDoubleAtk"))
        {
            me.animCtrl.SetBool("isDoubleAtk", false);
            ((Spirit)me).doubleAttCheck = false;
        }
    }

    public void PlayTurnAtk()
    {
        if (!me.animCtrl.GetBool("isTurnAtk"))
        {
            me.animCtrl.SetFloat("AttIndex", AttIndex);
            me.animCtrl.SetBool("isTurnAtk", true);
        }
    }

    public void StopTurnAtk()
    {
        if (me.animCtrl.GetBool("isTurnAtk"))
        {
            me.animCtrl.SetBool("isTurnAtk", false);
        }
    }
}
 