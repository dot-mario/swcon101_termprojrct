using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//wasd�� ȭ��ǥ�� �����̴� TPS������� ������� �������� ����ϴ� ��ũ��Ʈ
public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody rigid;

    float hAxis;
    float vAxis;
    bool jDown;
    bool isJump = false;
    bool sRun;

    public float walk_speed;
    public float additional_run_speed;

    Vector3 moveVec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput() // Ű���� �� �ޱ�
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButton("Jump");
        sRun = Input.GetKey(KeyCode.LeftShift);
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (sRun)
        {
            transform.position += moveVec * (walk_speed + additional_run_speed) * Time.deltaTime;
            anim.SetBool("isRun", true);
        }
        else
        {
            transform.position += moveVec * walk_speed * Time.deltaTime;
            anim.SetBool("isRun", false);
            anim.SetBool("isWalk", moveVec != Vector3.zero); // Walk �ִϸ��̼� true
        }
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // �ڿ������� ȸ��
    }

    void Jump()
    {
        // Jump�ϰ� �ִ� ��Ȳ���� Jump ���� �ʵ��� ����
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            anim.SetTrigger("doJump"); // Jump Trigger true ����
            isJump = true;
        }
    }
    void Run()
    {

    }
}
