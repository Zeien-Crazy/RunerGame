using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 dir;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject particle;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera maincamera;
    [SerializeField] private AudioSource fon;

    private Animator anim;

    private int lineToMove = 1;
    private int lineOld = 1;
    public float lineDistance = 4;
    [SerializeField] private float maxSpeed = 30;
    private bool boost = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        losePanel.SetActive(false);
        particle.SetActive(false);
        //playerCamera.enabled = !playerCamera.enabled;
        //StartCoroutine(SpeedIncrease());
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private States State
    {
        get { return (States)anim.GetInteger("State"); }
        set { anim.SetInteger("State", (int)value); }
    }

    private void Update()
    {
        if (controller.isGrounded && !boost)
            State = States.Run;

        if (!controller.isGrounded)
            State = States.Jump;

        if (SwipeController.swipeRight)
        {
            if (lineToMove < 2)
            {
                lineToMove++;
            }
        }

        if (SwipeController.swipeLeft)
        {
            if (lineToMove > 0)
            {
                lineToMove--;
            }
        }

        if (SwipeController.swipeUp)
        {
            if (controller.isGrounded)
            {
                Jump();
            }
        }

        Vector3 targetPosition = transform.position.z * Vector3.forward + transform.position.y * transform.up;
        if (lineToMove == 2)
        {
            targetPosition += Vector3.left * lineDistance;
        }
        else if (lineToMove == 0)
        {
            targetPosition += Vector3.right * lineDistance;

        }

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * speed * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
        {
            if (lineToMove == 0  || lineToMove == 1)
            {
                State = States.RunRight;
            }

            if (lineToMove == 2 || lineToMove == 1)
            {
                State = States.RunLeft;
            }
            controller.Move(moveDir);
        }
        else
        {
            controller.Move(diff);
        }
    }

    private void Jump()
    {
        dir.y = jumpForce;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dir.z = speed * (-1);
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
        if (speed < maxSpeed)
            speed += 0.1f * Time.deltaTime;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "obstacle")
        {
            losePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    /*public IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(4);
        if (speed < maxSpeed)
        {
            speed += 1;
            StartCoroutine(SpeedIncrease());
        }
    }*/

    public void Boost()
    {
        StartCoroutine(BoostStart());
    }

    public IEnumerator BoostStart()
    {
        float speedDev = speed;
        yield return new WaitForSeconds(2);
        fon.Pause();
        speed = 50;
        boost = true;
        playerCamera.enabled = !playerCamera.enabled;
        maincamera.enabled = !maincamera.enabled;
        //particle.SetActive(true);
        State = States.Boost;
        yield return new WaitForSeconds(16);
        fon.Play();
        speed = speedDev;
        boost = false;
        playerCamera.enabled = !playerCamera.enabled;
        maincamera.enabled = !maincamera.enabled;
        //particle.SetActive(false);
        State = States.Run;
    }
}

public enum States
{
    Run,
    Jump,
    RunLeft,
    RunRight,
    Boost
}
