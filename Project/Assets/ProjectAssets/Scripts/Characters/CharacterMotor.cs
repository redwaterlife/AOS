using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour
{
    private NavMeshAgent agent;
    public Rigidbody rg { get; private set; }

    public Collider[] colliders;
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

    #region Colliders
    public void DisablePhysicColliders()
    {
        foreach(Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void EnablePhysicColliders()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }
    #endregion 

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

    #region Rigidbody
    public void EnableRigidbody()
    {
        if (rg.isKinematic)
        {
            rg.isKinematic = false;
            rg.useGravity = true;
        }
    }

    public void DisableRigidbody()
    {
        if (!rg.isKinematic)
        {
            rg.isKinematic = true;
            rg.velocity = Vector3.zero;
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

    public void LookTargetPositionInstant()
    {
        if (!ShouldLookTargetPosition) return;
        if (TargetPosition - transform.position == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(TargetPosition - transform.position, Vector3.up);
        Quaternion newRotation = lookRotation;
        transform.eulerAngles = Vector3.up * newRotation.eulerAngles.y;
    }
}
