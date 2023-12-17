using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTeskPoints : MonoBehaviour
{
    private Cover[] covers;

    private void Awake()
    {
        covers = GetComponentsInChildren<Cover>();
    }

    public Vector3 GetRandomPointVector()
    {
        return covers[Random.Range(0, covers.Length - 1)].transform.position;
    }
}
