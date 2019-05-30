using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rg;

    public Vector3 TargetPosition { get; private set; }
    public bool ShouldLookTargetPosition = true;
    private float rotationSpeed = 9;
    private float minDistance = 0.25f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rg = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        LookTargetPosition();
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        TargetPosition = position;
    }

    #region Agent
    private void EnableAgent()
    {
        if (!agent.enabled)
        {
            agent.enabled = true;
        }
    }

    private void DisableAgent()
    {
        if (agent.enabled)
        {
            agent.enabled = false;
        }
    }
    #endregion

    public void Stop()
    {
        DisableAgent();
    }

    public void Continue()
    {
        MoveToDestination();
        EnableAgent();
    }

    public void MoveToDestination()
    {
        EnableAgent();
        agent.SetDestination(TargetPosition);
    }

    private void LookTargetPosition()
    {
        if (!ShouldLookTargetPosition) return;
        if (Vector3.Distance(TargetPosition, transform.position) <= minDistance) return;
        
        Quaternion lookRotation = Quaternion.LookRotation(TargetPosition - transform.position, Vector3.up);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = Vector3.up * newRotation.eulerAngles.y;
    }

    private void LookTargetPositionInstant()
    {
        if (!ShouldLookTargetPosition) return;
        if (TargetPosition - transform.position == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(TargetPosition - transform.position, Vector3.up);
        Quaternion newRotation = lookRotation;
        transform.eulerAngles = Vector3.up * newRotation.eulerAngles.y;
    }
}
