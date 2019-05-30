using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yasuo : Champion
{
    protected override void Awake()
    {
        base.Awake();
        Q = GetComponent<Skill_Yasuo_Q>();
    }
}
