using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    Rigidbody rg;
    Transform target;
    bool air = false;
    public float rotateSpeed = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (air)
        {
            other.GetComponent<Enemy>().Airbone();
        }
        else
        {
            other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * 4, ForceMode.Impulse);
        }
    }

    public void Fire(float power, float lifeTime)
    {
        rg = GetComponent<Rigidbody>();
        rg.AddForce(transform.forward * power * 100);
        Destroy(gameObject, lifeTime);
    }

    public void Track(Transform target, float lifeTime)
    {
        this.target = target;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target != null) transform.position = target.position;
        if (rotateSpeed > 0) transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    public void AddAirbone()
    {
        air = true;
    }
}
