using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion : Character
{
    [HideInInspector]
    public Skill Q;
    [HideInInspector]
    public Skill W;
    [HideInInspector]
    public Skill E;
    [HideInInspector]
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
        if (Status != eCharacterStatus.Acting && Status != eCharacterStatus.Damaged)
            Q.StartSkill();
    }

    public virtual void UseW()
    {
        if (Status != eCharacterStatus.Acting && Status != eCharacterStatus.Damaged)
            W.StartSkill();
    }

    public virtual void UseE()
    {
        if (Status != eCharacterStatus.Acting && Status != eCharacterStatus.Damaged)
            E.StartSkill();
    }

    public virtual void UseR()
    {
        if (Status != eCharacterStatus.Acting && Status != eCharacterStatus.Damaged)
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
        motor.EnablePhysicColliders();
        motor.EnableRigidbody();
    }
}
