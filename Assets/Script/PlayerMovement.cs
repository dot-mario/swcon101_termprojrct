using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//wasd와 화살표로 움직이는 TPS방식으로 고양이의 움직임을 담당하는 스크립트
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

    void GetInput() // 키보드 값 받기
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
            anim.SetBool("isWalk", moveVec != Vector3.zero); // Walk 애니메이션 true
        }
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 자연스럽게 회전
    }

    void Jump()
    {
        // Jump하고 있는 상황에서 Jump 하지 않도록 방지
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            anim.SetTrigger("doJump"); // Jump Trigger true 설정
            isJump = true;
        }
    }
    void Run()
    {

    }
}
