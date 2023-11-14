using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanMovable : MonoBehaviour
{
    [SerializeField] bool DebugMode = false;
    [Range(0f, 360f)][SerializeField] float ViewAngle = 0f;
    [SerializeField] float ViewRadius = 1f;
    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;

    public Transform[] points;

    // 길을 찾아서 이동할 에이전트
    NavMeshAgent agent;
    Animator anim;

    float velocity;

    // 에이전트의 목적지
    [SerializeField]
    Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        agent.SetDestination(target.position);
        velocity = agent.velocity.magnitude;
        anim.SetFloat("velocity", velocity);
        Debug.Log("Human Velocity: " + velocity);
    }
}
