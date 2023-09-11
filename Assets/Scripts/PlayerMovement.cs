using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;

    [SerializeField] private float speed;
    [SerializeField] private float blendTime;
    [SerializeField] private float rotationDamping = 5;

    private Rigidbody playerRB;

    private Animator animator;

    private CinemachinePOV povCam;
    private Camera cam;


    private Vector3 movement;
    private Quaternion newRotation;

    private float horizontal, vertical;
    private float xvelDif;
    private float zvelDif;

    private int xVelHash;
    private int zVelHash;

    private bool gameEnded;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody>();

        xVelHash = Animator.StringToHash("xVel");
        zVelHash = Animator.StringToHash("zVel");


        povCam = vCam.GetCinemachineComponent<CinemachinePOV>();
        cam = Camera.main;

        gameEnded = false;
    }

    private void Start() => GetRotation();
    private void OnEnable() => MyEvents.OnPlayerDeath += GameEnded;
    private void OnDisable() => MyEvents.OnPlayerDeath -= GameEnded;
    private bool Run => Input.GetKey(KeyCode.LeftShift);

    private void GameEnded()
    {
        gameEnded = true;
        povCam.enabled = false;
    }

    private void Update()
    {
        if (gameEnded)
            return;

        StartRuning();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        if (gameEnded)
            return;

        MoveCharacter();

        playerRB.MoveRotation(newRotation);
    }

    private void LateUpdate()
    {
        if (gameEnded)
            return;

        GetRotation();
    }

    private void MoveCharacter()
    {
        movement.x = Mathf.Lerp(movement.x, horizontal * speed, blendTime * Time.deltaTime);
        movement.z = Mathf.Lerp(movement.z, vertical * speed, blendTime * Time.deltaTime);

        xvelDif = movement.x - playerRB.velocity.x;
        zvelDif = movement.z - playerRB.velocity.z;

        Vector3 move = new Vector3(xvelDif, 0, zvelDif);

        playerRB.AddForce(transform.TransformVector(move), ForceMode.VelocityChange);

        animator.SetFloat(xVelHash, movement.x);
        animator.SetFloat(zVelHash, movement.z);
    }

    private void GetRotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.right);
        targetRotation.x = 0;
        targetRotation.z = 0;
        newRotation = Quaternion.Lerp(playerRB.rotation, targetRotation, rotationDamping * Time.fixedDeltaTime);
    }
    private void StartRuning()
    {
        speed = Run ? 2 : 1;
    }
}
