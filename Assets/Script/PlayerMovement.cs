using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

//wasd�� ȭ��ǥ�� �����̴� TPS������� ������� �������� ����ϴ� ��ũ��Ʈ
public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody rigid;
    GameObject cat;

    public CinemachineFreeLook freeLookCamera;

    float hAxis;
    float vAxis;
    bool jDown;
    bool jUp;
    bool isJump = false;
    bool sRun;
    //bool isForced = false;
    bool jumpFocusing = false; //���� ��¡ ����
    bool isGrounded;
    float jumpForce = 0f; // ���� ��
    float jumpKeyStartTime = 0.0f;
    //float jumpStartTime;  // ������ ������ �ð�
    //float jumpDuration;   // ���� �ð� (���� �� �������� �ɸ��� �ð�)
    float jumpAnimLength;
    //float gravity; // �߷� ���ӵ�
    float currentFlyTime = 0.0f; // ���� ���߿� �ִ� �ð�
    float currentRunningTime = 0.0f;

    public float walk_speed = 3;
    public float additional_run_speed = 3;
    public float additional_roll_speed = 3;
    public float rollingTime = 3.0f;
    public float mass = 1.0f; // �÷��̾� ĳ������ ����
    public float minJumpForce = 5.0f;
    public float maxJumpForce = 20.0f;
    public float minForceMagnitude = 10.0f; // �ּ������� Ȯ���Ϸ��� ���� ũ��
    public float flyTimeThreshold = 1.0f; // �÷��̾ ���߿� �ִ� �ð� �Ӱ谪
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

        // Animator ������Ʈ���� �ִϸ��̼� ������ �����ɴϴ�.
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        // �ִϸ��̼� �̸��� ��ġ�ϴ� �ִϸ��̼��� ã���ϴ�.
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Fear (1)")
            {
                // �ִϸ��̼��� ����(��)�� ����ϴ�.
                jumpAnimLength = clip.length;
                Debug.Log("�ִϸ��̼� '" + "Fear (1)" + "'�� ����: " + jumpAnimLength + "��");
                anim.SetFloat("JumpSpeed", (float)(jumpAnimLength / maxJumpTime));
                Debug.Log("�ִϸ��̼� '" + "Fear (1)" + "'�� ����: " + (float)(jumpAnimLength / maxJumpTime) + "��");
            }
        }
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
        GroundCheck();
        FlyCheck();
    }

    void GetInput() // Ű���� �� �ޱ�
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButton("Jump");
        jUp = Input.GetButtonUp("Jump");
        sRun = Input.GetKey(KeyCode.LeftShift);
    }

    void Move()
    {
        // ī�޶��� ������ �������� �̵� ������ ����
        // Cinemachine FreeLook ī�޶��� X Axis�� Y Axis ���� �����ͼ� �̵� ������ ����
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

        if (sRun) //�� ��
        {
            anim.SetBool("isWalk", false);
            currentRunningTime += Time.deltaTime;
            if (currentRunningTime > rollingTime) //���� �ٸ� ������
            {
                transform.position += moveVec * (walk_speed + additional_run_speed + additional_roll_speed) * focusDebuff * Time.deltaTime;
                anim.SetBool("isRoll", true);
            }
            else
            {
                transform.position += moveVec * (walk_speed + additional_run_speed) * focusDebuff * Time.deltaTime;
                anim.SetBool("isRoll", false);
                anim.SetBool("isRun", true);
            }
        }
        else //���� ��
        {
            rigid.angularVelocity = Vector3.zero;
            currentRunningTime = 0;
            transform.position += moveVec * (walk_speed) * focusDebuff * Time.deltaTime;
            anim.SetBool("isRun", false);
            anim.SetBool("isRoll", false);
            anim.SetBool("isWalk", moveVec != Vector3.zero); // Walk �ִϸ��̼� true
        }
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void Jump()
    {
        //if (isForced)
        //    return;

        // ������ �Է��� �޾� jumpForce�� ���� (�����̽��ٸ� ���� �� ������ ���� ���� jumpForce�� ����)
        if (jDown && !isJump && isGrounded)
        {
            jumpKeyStartTime += Time.deltaTime;
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
            }
        }
        else if (jUp && !isJump && isGrounded) // Jump�ϰ� �ִ� ��Ȳ���� Jump ���� �ʵ��� ����        
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

            Debug.Log("���� ����: " + jumpForce);

            // �÷��̾��� ������ ����Ͽ� jumpForce�� ����
            float adjustedJumpForce = jumpForce / mass;

            // jumpForce�� ����Ͽ� ���� ���� ����
            rigid.AddForce(Vector3.up * adjustedJumpForce, ForceMode.Impulse);

            Debug.Log("����");
        }
    }
    void GroundCheck()
    {
        // �÷��̾� �ֺ��� Raycast�� ��� ������ ǥ�鿡 �ִ����� �Ǵ�
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.1f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < 10f) // ǥ���� ���� ���Ͱ� ���� ���� ���Ϸ� ������ ������ ������ ǥ������ ����
            {
                isGrounded = true;
                anim.SetBool("isFly", false);
                isJump = false;
                return;
            }
        }
        isGrounded = false;
    }
    void FlyCheck()
    {
        if(!isGrounded)
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
}
