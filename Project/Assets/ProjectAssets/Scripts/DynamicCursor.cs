using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCursor : MonoBehaviour
{
    public Champion champion;

    private void Start()
    {
        MovePosition(CameraFollow.inst.GetTargetPosition());
    }

    public void MovePosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
