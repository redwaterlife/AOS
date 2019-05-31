using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCharacterStatus { Idle, Moving, Acting, Damaged }

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(CharacterStats))]
public class Character : MonoBehaviour
{
    public eCharacterStatus Status { get; protected set; }

    public CharacterMotor motor { get; private set; }
    protected CharacterStats stats;

    public Vector3 TargetPosition { get; private set; }

    protected virtual void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        stats = GetComponent<CharacterStats>();

        Status = eCharacterStatus.Idle;
    }

    protected virtual void Start()
    {
        //
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        TargetPosition = position;
    }

    public void Move()
    {
        if (Status == eCharacterStatus.Acting) return;

        motor.UpdateTargetPosition(TargetPosition);
        motor.MoveToDestination();
    }

    public void Stop()
    {
        if (Status == eCharacterStatus.Acting) return;

        motor.Stop();
    }

}
