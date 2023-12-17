using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanViewPartGizmo : MonoBehaviour
{
    public HumanMovable humanMovable;

    private void OnDrawGizmos()
    {
        //transform.rotation = Quaternion.Euler(0, 180, 0);
        //Gizmos.color = Color.blue;
        ////transform.rotation = transform.rotation * Quaternion.Euler(humanMovable.ViewHeight, 0f, 0f);
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);
    }
}
