using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion : Character
{
    public Skill Q;
    public Skill W;
    public Skill E;
    public Skill R;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }


    public virtual void UseQ()
    {
        Q.StartSkill();
    }

    public virtual void UseW()
    {
        W.StartSkill();
    }

    public virtual void UseE()
    {
        E.StartSkill();
    }

    public virtual void UseR()
    {
        R.StartSkill();
    }

    public virtual void StartSkill()
    {
        Status = eCharacterStatus.Acting;
        motor.Stop();
    }

    public virtual void EndSkill()
    {
        // 중요
        if (Status == eCharacterStatus.Acting) Status = eCharacterStatus.Idle;
        motor.Continue();
    }
}
