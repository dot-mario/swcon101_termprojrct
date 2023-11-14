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
    bool jDowned = true;
    bool jUp;
    bool isJump = false;
    bool sRun;
    //bool isForced = false;
    bool isFly = false; // ���߿� �ִ��� ���θ� ��Ÿ���� ����
    bool jumpFocusing = false; //���� ��¡ ����
    bool isGrounded;
    float jumpForce = 0f; // ���� ��
    float jumpKeyStartTime = 0.0f;
    //float jumpStartTime;  // ������ ������ �ð�
    //float jumpDuration;   // ���� �ð� (���� �� �������� �ɸ��� �ð�)
    float jumpAnimLength;
    //float gravity; // �߷� ���ӵ�
    float currentFlyTime = 0.0f; // ���� ���߿� �ִ� �ð�
    float currentForcedTime = 0.0f;
    float currentRunningTime = 0.0f;
    Vector3 totalForce;

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
            if (clip.name == "Jump")
            {
                // �ִϸ��̼��� ����(��)�� ����ϴ�.
                jumpAnimLength = clip.length;
                Debug.Log("�ִϸ��̼� '" + "Jump" + "'�� ����: " + jumpAnimLength + "��");
            }
            if (clip.name == "Fear (1)")
            {
                // �ִϸ��̼��� ����(��)�� ����ϴ�.
                jumpAnimLength = clip.length;
                Debug.Log("�ִϸ��̼� '" + "Fear (1)" + "'�� ����: " + jumpAnimLength + "��");
            }
        }

        //�ڽ� ������Ʈ
        //Transform childTransform = transform.Find("Cat");
        //if (childTransform != null)
        //{
        //    // �ڽ� ������Ʈ�� ã���� �� ���ϴ� �۾� ����
        //    cat = childTransform.gameObject;
        //    Debug.Log("�ڽ� ������Ʈ�� ã�ҽ��ϴ�: " + cat.name);
        //}
        //else
        //{
        //    Debug.Log("�ڽ� ������Ʈ�� ã�� ���߽��ϴ�.");
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
        //if (isForced)
        //    return;

        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;
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
            anim.SetBool("isWalk", moveVec != Vector3.zero); // Walk �ִϸ��̼� true
        }
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
        {
            // �̵� ������ ������� �÷��̾ ȸ����ŵ�ϴ�.
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



            //float currentHeight = transform.position.y - rigid.position.y;
            ////float maxHeight = (jumpForce * jumpForce) / (2 * gravity * mass); // �ִ� ���� ���

            ////jumpDuration = Mathf.Sqrt((2 * maxHeight) / gravity) * 2; // ���� �ð� ���
            ////Debug.Log("�̷��� ��� �ð�: " + jumpDuration);
            ////anim.SetFloat("JumpSpeed", jumpAnimLength / jumpDuration); // ���� �ð��� �̿��Ͽ� �ִϸ��̼� �ӵ� ����

            //if (currentHeight >= maxHeight)
            //{
            //    Debug.Log("�̷��� ����");
            //    // ���� �� ���̰� �ִ� ���̿� �����ϸ� ���� ����
            //    //isJump = false;
            //    //jumpDuration = Mathf.Sqrt((2 * maxHeight) / gravity); // ���� �ð� ���
            //    //anim.SetFloat("JumpSpeed", jumpDuration); // ���� �ð��� �̿��Ͽ� �ִϸ��̼� �ӵ� ����
            //}
        }
    }
    void GroundCheck()
    {
        // �÷��̾� �ֺ��� Raycast�� ��� ������ ǥ�鿡 �ִ����� �Ǵ�
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.1f)) // -transform.up�� �Ʒ� ������ ��Ÿ���ϴ�.
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < 10f) // ǥ���� ���� ���Ͱ� ���� ���� ���Ϸ� ������ ������ ������ ǥ������ ����
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
        //    Debug.Log("not Force 1 / ��������");
        //    return;
        //}
        //else
        //{
        //    totalForce = rigid.velocity * rigid.mass / Time.fixedDeltaTime;
        //    //Debug.Log($"Force check: {totalForce.magnitude}");
        //    if (totalForce.magnitude >= minForceMagnitude)
        //    {
        //        isForced = true;
        //        Debug.Log("Force 1 / ���� ���� ����");
        //    }
        //    if (isForced)
        //    {
        //        currentForcedTime += Time.fixedDeltaTime;
        //        if (currentForcedTime > 3.0f)
        //        {
        //            isForced = false;
        //            Debug.Log("not Force 2 / ���� ������ 3�ʰ� ����");
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

    // OnCollisionEnter�� ����Ͽ� ĳ���Ͱ� ���� ��Ҵ��� Ȯ��
    void OnCollisionEnter(Collision collision)
    {
        //isFly = false;
        //anim.SetBool("isFly", false);
        jDowned = true;
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    isJump = false;
        //    jDowned = true;
        //    //Debug.Log("��� �ð�: " + (Time.time - jumpStartTime));
        //}
    }

    // OnCollisionExit�� ����Ͽ� ĳ���Ͱ� ������ ������� Ȯ��
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
