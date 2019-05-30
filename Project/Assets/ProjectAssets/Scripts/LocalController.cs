using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalController : MonoBehaviour
{
    private Champion champion;
    private CameraFollow cam;

    private void Awake()
    {
        champion = GetComponent<Champion>();
    }

    private void Start()
    {
        cam = CameraFollow.inst;
    }

    private void Update()
    {
        AcceptInputs();
    }

    private void AcceptInputs()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 targetPosition = cam.GetTargetPosition();
            champion.UpdateTargetPosition(targetPosition);

            champion.Move();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            champion.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Q)) champion.UseQ();
    }
}
