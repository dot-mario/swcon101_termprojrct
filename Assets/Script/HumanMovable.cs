using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public enum HumanState { Idle = 0, Walk, Chase, Attack }

public class HumanMovable : MonoBehaviour
{
    // 길을 찾아서 이동할 에이전트
    NavMeshAgent agent;
    Animator anim;

    // 에이전트의 타겟
    [SerializeField] Transform target;

    // 에이전트의 위치
    [SerializeField] Transform head;


    //[SerializeField] Transform[] points;

    [SerializeField] bool DebugMode = false;
    [Range(0f, 360f)][SerializeField] float ViewAngle = 0f;

    //캐릭터가 위 아래를 바라보는 각도
    [Range(-90f, 90f)]public float ViewHeight = 0f;
    [SerializeField] float ViewRadius = 1f;
    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;

    Vector3 hitTargetVector;

    bool isChase = false;
    bool isDetect = false;

    private HumanState humanState;

    public HumanTeskPoints humanTeskPoints;

    public GameObject gameOverPanel; // 패널에 대한 참조

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        ChangeState(HumanState.Walk);
    }

    private void Update()
    {
        isDetect = DetectTarget();
        if (isDetect && isChase == false) ChangeState(HumanState.Chase);
        RotationHead();
    }

    private void ChangeState(HumanState newState)
    {
        StopCoroutine(humanState.ToSafeString());
        humanState = newState;
        StartCoroutine(humanState.ToSafeString());
    }

    protected bool pathComplete()
    {
        if (Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator Idle()
    {
        //처음 실행
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Look Around") == false)
            anim.Play("Look Around", 0, 0);

        float time = 0;

        //반복 실행
        while (true)
        {
            time += Time.deltaTime;
            if (time > 3f)
                break;
            yield return null;
        }

        //마지막 실행
        ChangeState(HumanState.Walk);
    }
    private IEnumerator Walk()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") == false)
            anim.Play("Walk", 0, 0);

        agent.SetDestination(humanTeskPoints.GetRandomPointVector());
        while (true)
        {
            if (pathComplete())
                break;
            yield return null;
        }

        ChangeState(HumanState.Walk);
    }

    private IEnumerator Chase()
    {
        isChase = true;

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("RunForward") == false)
            anim.Play("RunForward", 0, 0);

        while (true)
        {
            agent.SetDestination(hitTargetVector);
            if (isDetect == false && agent.remainingDistance == 0) break;
            if (isDetect == true && agent.remainingDistance < 0.5f)
            {
                gameOverPanel.SetActive(true);
                break;
            }
            yield return null;
        }

        isChase = false;
        ChangeState(HumanState.Idle);
    }

    bool DetectTarget()
    {
        Vector3 myPos = head.transform.position;
        Vector3 lookDir = head.transform.forward;

        Collider[] Targets = Physics.OverlapSphere(myPos, ViewRadius, TargetMask);

        if (Targets.Length == 0) return false;
        foreach (Collider EnemyColli in Targets)
        {
            Vector3 targetPos = EnemyColli.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

            if (targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(myPos, targetDir, Vector3.Distance(myPos, targetPos), ObstacleMask))
            {
                hitTargetVector = targetPos;
                return true;
            }
        }
        return false;
    }

    private void RotationHead()
    {
        // 현재 재생 중인 애니메이션 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 클립의 회전 값을 가져와서 물체를 회전
        float normalizedTime = stateInfo.normalizedTime; // 0부터 1까지의 정규화된 시간
        Quaternion rotation = anim.GetBoneTransform(HumanBodyBones.Head).rotation;

        // 물체 회전
        head.transform.rotation = rotation * Quaternion.Euler(-90f, 0f, 0f) * Quaternion.Euler(0f, ViewHeight, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(head.transform.position, head.transform.position + head.transform.forward * 3f);

        // 구체의 중심 좌표
        Vector3 center = head.transform.position;
        Vector3 lookDir = head.transform.forward;

        Collider[] Targets = Physics.OverlapSphere(center, ViewRadius, TargetMask);

        if (Targets.Length != 0)
        {
            foreach (Collider EnemyColli in Targets)
            {
                Vector3 targetPos = EnemyColli.transform.position;
                Vector3 targetDir = (targetPos - center).normalized;
                float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;


                if (targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(center, targetDir, Vector3.Distance(center,targetPos), ObstacleMask))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(center, targetPos);
                }
            }
        }

        if (DebugMode == false) return;

        // 기즈모의 색상 설정
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(center, ViewRadius);

        // 구체의 겉면 좌표를 그리는 반복문
        int segments = Mathf.CeilToInt(ViewAngle * 10);  // ViewAngle에 따라 세그먼트 수를 결정
        for (int i = 0; i <= segments; i++)
        {
            float theta = Mathf.Lerp(-ViewAngle * 0.5f, ViewAngle * 0.5f, i / (float)segments);

            for (float phi = -Mathf.PI; phi <= Mathf.PI; phi += 0.1f)
            {
                float x = center.x + ViewRadius * Mathf.Sin(theta) * Mathf.Cos(phi);
                float y = center.y + ViewRadius * Mathf.Sin(theta) * Mathf.Sin(phi);
                float z = center.z + ViewRadius * Mathf.Cos(theta);

                Vector3 targetPos = new Vector3(x, y, z);
                Vector3 targetDir = (targetPos - center).normalized;
                float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;

                if (targetAngle <= ViewAngle * 0.5f)
                {
                    float radius = ViewRadius;
                    RaycastHit raycastHit;
                    if (Physics.Raycast(center, targetDir, out raycastHit, radius, ObstacleMask))
                    {
                        radius = raycastHit.distance;
                    }
                    Gizmos.DrawLine(center, center + targetDir * radius);
                }
            }
        }
    }
}
