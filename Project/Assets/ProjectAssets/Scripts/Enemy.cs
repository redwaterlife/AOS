using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    NavMeshAgent agent;
    Vector3 pos;

    bool c = false;
    bool hc = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pos = transform.position;
        StartCoroutine(a());

    }

    IEnumerator a()
    {
        if (!c && !hc && Vector3.Distance(transform.position, pos) >= 3)
        {
            agent.enabled = true;
            agent.SetDestination(pos);
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(a());
    }

    public void Airbone()
    {
        agent.enabled = false;
        c = true;
        GetComponent<Rigidbody>().AddForce(Vector3.up * 6, ForceMode.Impulse);
        StopCoroutine(b());
        StartCoroutine(b());
    }

    public void Ice()
    {
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        hc = true; 
    }

    public void StopIce()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        hc = false;
    }

    IEnumerator b()
    {
        yield return new WaitForSeconds(2f);
        c = false;
    }
}
