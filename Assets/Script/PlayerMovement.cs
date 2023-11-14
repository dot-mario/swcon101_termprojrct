using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

//wasd와 화살표로 움직이는 TPS방식으로 고양이의 움직임을 담당하는 스크립트
public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody rigid;
    GameObject cat;

    public CinemachineFreeLook freeLookCamera;

    float hAxis;
    float vAxis;
    bool jDown;
    bool jDowned = true;
    bool jUp;
    bool isJump = false;
    bool sRun;
    //bool isForced = false;
    bool isFly = false; // 공중에 있는지 여부를 나타내는 변수
    bool jumpFocusing = false; //점프 차징 여부
    bool isGrounded;
    float jumpForce = 0f; // 점프 힘
    float jumpKeyStartTime = 0.0f;
    //float jumpStartTime;  // 점프를 시작한 시간
    //float jumpDuration;   // 점프 시간 (점프 후 착지까지 걸리는 시간)
    float jumpAnimLength;
    //float gravity; // 중력 가속도
    float currentFlyTime = 0.0f; // 현재 공중에 있는 시간
    float currentForcedTime = 0.0f;
    float currentRunningTime = 0.0f;
    Vector3 totalForce;

    public float walk_speed = 3;
    public float additional_run_speed = 3;
    public float additional_roll_speed = 3;
    public float rollingTime = 3.0f;
    public float mass = 1.0f; // 플레이어 캐릭터의 질량
    public float minJumpForce = 5.0f;
    public float maxJumpForce = 20.0f;
    public float minForceMagnitude = 10.0f; // 최소한으로 확인하려는 힘의 크기
    public float flyTimeThreshold = 1.0f; // 플레이어가 공중에 있는 시간 임계값
    public float rotationSpeed = 90.0f;
    public float maxJumpTime = 5.0f;
    public bool isJumpDeduff = true;

    Vector3 moveVec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //gravity = Physics.gravity.magnitude;     
        mass = rigid.mass;

        // Animator 컴포넌트에서 애니메이션 정보를 가져옵니다.
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        // 애니메이션 이름과 일치하는 애니메이션을 찾습니다.
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Jump")
            {
                // 애니메이션의 길이(초)를 얻습니다.
                jumpAnimLength = clip.length;
                Debug.Log("애니메이션 '" + "Jump" + "'의 길이: " + jumpAnimLength + "초");
            }
            if (clip.name == "Fear (1)")
            {
                // 애니메이션의 길이(초)를 얻습니다.
                jumpAnimLength = clip.length;
                Debug.Log("애니메이션 '" + "Fear (1)" + "'의 길이: " + jumpAnimLength + "초");
            }
        }

        //자식 오브젝트
        //Transform childTransform = transform.Find("Cat");
        //if (childTransform != null)
        //{
        //    // 자식 오브젝트를 찾았을 때 원하는 작업 수행
        //    cat = childTransform.gameObject;
        //    Debug.Log("자식 오브젝트를 찾았습니다: " + cat.name);
        //}
        //else
        //{
        //    Debug.Log("자식 오브젝트를 찾지 못했습니다.");
        //}
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }
    private void FixedUpdate()
    {
        ForceCheck();
        GroundCheck();
        FlyCheck();
    }

    void GetInput() // 키보드 값 받기
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButton("Jump");
        jUp = Input.GetButtonUp("Jump");
        sRun = Input.GetKey(KeyCode.LeftShift);
    }

    void Move()
    {
        //if (isForced)
        //    return;

        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        // 카메라의 방향을 기준으로 이동 방향을 설정
        // Cinemachine FreeLook 카메라의 X Axis와 Y Axis 값을 가져와서 이동 방향을 설정
        float cameraXAxis = freeLookCamera.m_XAxis.Value;
        float cameraYAxis = freeLookCamera.m_YAxis.Value;

        Vector3 cameraForward = Quaternion.Euler(0, cameraXAxis, 0) * Vector3.forward;
        Vector3 cameraRight = Quaternion.Euler(0, cameraXAxis, 0) * Vector3.right;

        moveVec = (vAxis * cameraForward + hAxis * cameraRight).normalized;

        float focusDebuff = 1;

        if (isJumpDeduff && jumpFocusing)
        {
            focusDebuff = 0;
        }

        if (sRun)
        {
            anim.SetBool("isWalk", false);
            currentRunningTime += Time.deltaTime;
            //Debug.Log("Roll: " + currentRunningTime);
            if (currentRunningTime > rollingTime)
            {
                transform.position += moveVec * (walk_speed + additional_run_speed + additional_roll_speed) * focusDebuff * Time.deltaTime;
                //cat.transform.rotation *= Quaternion.Euler(rotationSpeed, 0, 0);
                //rigid.AddTorque(transform.right * rotationSpeed);
                //cat.GetComponent<Rigidbody>().AddTorque(transform.right * rotationSpeed);
                anim.SetBool("isRoll", true);
            }
            else
            {
                transform.position += moveVec * (walk_speed + additional_run_speed) * focusDebuff * Time.deltaTime;
                anim.SetBool("isRoll", false);
                anim.SetBool("isRun", true);
            }
        }
        else
        {
            rigid.angularVelocity = Vector3.zero;
            currentRunningTime = 0;
            transform.position += moveVec * (walk_speed) * focusDebuff * Time.deltaTime;
            anim.SetBool("isRun", false);
            anim.SetBool("isRoll", false);
            anim.SetBool("isWalk", moveVec != Vector3.zero); // Walk 애니메이션 true
        }
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
        {
            // 이동 방향을 기반으로 플레이어를 회전시킵니다.
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void Jump()
    {
        //if (isForced)
        //    return;

        // 게이지 입력을 받아 jumpForce를 설정 (스페이스바를 누를 때 게이지 값에 따라 jumpForce를 설정)
        if (jDown && !isJump && isGrounded)
        {
            jumpKeyStartTime += Time.deltaTime;
            anim.SetFloat("JumpSpeed", (float)(jumpAnimLength / maxJumpTime));
            //Debug.Log("Speed: " + 0.154 / jumpKeyStartTime);
            if (jumpKeyStartTime > maxJumpTime)
            {
                anim.SetBool("isJumpingBegin", true);
                jumpFocusing = true;
            }
            else if (jumpKeyStartTime > 1.0f)
            {
                anim.SetTrigger("doJumpBegin");
                jumpFocusing = true;
                //jDowned = false;
            }
        }
        else if (jUp && !isJump && isGrounded) // Jump하고 있는 상황에서 Jump 하지 않도록 방지        
        {
            isJump = true;
            jumpFocusing = false;
            if (jumpKeyStartTime < 1.0f)
            {
                anim.SetTrigger("doJump");
                jumpForce = minJumpForce;
            }
            else if (jumpKeyStartTime < maxJumpTime)
            {
                float normalizedGauge = Mathf.Clamp01(jumpKeyStartTime / maxJumpTime);
                jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, normalizedGauge);
                anim.SetTrigger("doJump");
            }
            else
            {
                float normalizedGauge = Mathf.Clamp01(1.0f);
                jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, normalizedGauge);
                anim.SetBool("isJumpingBegin", false);
            }
            jumpKeyStartTime = 0.0f;

            Debug.Log("점프 강도: " + jumpForce);

            // 플레이어의 질량을 고려하여 jumpForce를 적용
            float adjustedJumpForce = jumpForce / mass;

            // jumpForce를 사용하여 점프 힘을 설정
            rigid.AddForce(Vector3.up * adjustedJumpForce, ForceMode.Impulse);

            Debug.Log("점프");



            //float currentHeight = transform.position.y - rigid.position.y;
            ////float maxHeight = (jumpForce * jumpForce) / (2 * gravity * mass); // 최대 높이 계산

            ////jumpDuration = Mathf.Sqrt((2 * maxHeight) / gravity) * 2; // 점프 시간 계산
            ////Debug.Log("이론적 경과 시간: " + jumpDuration);
            ////anim.SetFloat("JumpSpeed", jumpAnimLength / jumpDuration); // 점프 시간을 이용하여 애니메이션 속도 조절

            //if (currentHeight >= maxHeight)
            //{
            //    Debug.Log("이론적 고점");
            //    // 점프 중 높이가 최대 높이에 도달하면 점프 종료
            //    //isJump = false;
            //    //jumpDuration = Mathf.Sqrt((2 * maxHeight) / gravity); // 점프 시간 계산
            //    //anim.SetFloat("JumpSpeed", jumpDuration); // 점프 시간을 이용하여 애니메이션 속도 조절
            //}
        }
    }
    void GroundCheck()
    {
        // 플레이어 주변에 Raycast를 쏘아 안정된 표면에 있는지를 판단
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.1f)) // -transform.up은 아래 방향을 나타냅니다.
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < 10f) // 표면의 법선 벡터가 일정 각도 이하로 기울어져 있으면 안정된 표면으로 간주
            {
                isGrounded = true;
                isFly = false;
                anim.SetBool("isFly", false);
                isJump = false;
                return;
            }
        }
        isGrounded = false;
        isFly = true;
    }
    void ForceCheck()
    {
        //if (isJump)
        //{
        //    isForced = false;
        //    Debug.Log("not Force 1 / 점프중임");
        //    return;
        //}
        //else
        //{
        //    totalForce = rigid.velocity * rigid.mass / Time.fixedDeltaTime;
        //    //Debug.Log($"Force check: {totalForce.magnitude}");
        //    if (totalForce.magnitude >= minForceMagnitude)
        //    {
        //        isForced = true;
        //        Debug.Log("Force 1 / 많은 힘을 받음");
        //    }
        //    if (isForced)
        //    {
        //        currentForcedTime += Time.fixedDeltaTime;
        //        if (currentForcedTime > 3.0f)
        //        {
        //            isForced = false;
        //            Debug.Log("not Force 2 / 힘을 받은지 3초가 지남");
        //            currentForcedTime = 0.0f;
        //            RestForce();
        //        }
        //        else
        //        {
        //            isForced = true;
        //        }
        //    }
        //}
    }
    void FlyCheck()
    {
        if(isFly)
        {
            currentFlyTime += Time.fixedDeltaTime;
            if (currentFlyTime >= flyTimeThreshold)
            {
                anim.SetBool("isFly", true);
            }
        } else
        {
            currentFlyTime = 0;
        }
    }

    // OnCollisionEnter를 사용하여 캐릭터가 땅에 닿았는지 확인
    void OnCollisionEnter(Collision collision)
    {
        //isFly = false;
        //anim.SetBool("isFly", false);
        jDowned = true;
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    isJump = false;
        //    jDowned = true;
        //    //Debug.Log("경과 시간: " + (Time.time - jumpStartTime));
        //}
    }

    // OnCollisionExit를 사용하여 캐릭터가 땅에서 벗어났는지 확인
    void OnCollisionExit(Collision collision)
    {
        //isFly = true;
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    //isGrounded = false;
        //}
    }
    void RestForce()
    {
        Debug.Log("Rest Force");
        rigid.velocity = Vector3.zero;
    }
}
