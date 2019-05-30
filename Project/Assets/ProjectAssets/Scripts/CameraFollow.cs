using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow inst;

    private Camera cam;

    public Transform target = null;
    public float dist = 5f;
    public Vector3 offset = Vector3.zero;
    public float smoothSpeed = 5f;
    public float scrollSensitivity = 5f;
    public float maxDist = 15;
    public float minDist = 5;

    private Vector3 targetPosition;
    public LayerMask layerMask;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (inst == null) inst = this;
        else throw new System.Exception("instance가 이미 존재합니다!");
    }

    private void Update()
    {
        UpdateTargetPosition();
    }

    private void LateUpdate()
    {
        if (!target) return;
        else
        {
            float num = Input.GetAxis("Mouse ScrollWheel");
            dist -= num * scrollSensitivity;
            dist = Mathf.Clamp(dist, minDist, maxDist);

            Vector3 pos = target.position + offset;
            pos -= transform.forward * dist;

            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * smoothSpeed);
        }
    }

    private void UpdateTargetPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            targetPosition = hit.point;
        }
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
}