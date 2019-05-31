using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yasuo : Champion
{
    public int qStack { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        qStack = 0;

        Q = GetComponent<Skill_Yasuo_Q>();
        E = GetComponent<Skill_Yasuo_E>();
    }

    public override void UseQ()
    {
        if (qStack != 2) Q.ProjectilePrefabs["Q"] = Q.ProjectilePrefabs["Q1"];
        else Q.ProjectilePrefabs["Q"] = Q.ProjectilePrefabs["Q2"];
        Q.StartSkill();
        AddQStack();
    }

    public void AddQStack()
    {
        if (qStack > 1) qStack = 0;
        else qStack++;
    }
}
