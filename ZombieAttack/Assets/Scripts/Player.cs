using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Camera mCamera;
    private CharacterController mController;
    private GameObject mGunArm;
    private GameObject mGunBarrel;
    private float mouseSensitivity = 1.0f;
    private float speed = 25.0f;
    private float verticalSpeed = 0.0f;
    private float gravity = 80.0f;
    private float jumpForce = 40.0f;
    private float groundDistance = 0.1f;
    private float health = 100.0f;
    public float Health { get { return health; } }
    private float mDamage = 50.0f;
    private bool isAirborne = false;

    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        mController = gameObject.GetComponent<CharacterController>();
        mCamera = GameObject.Find("PlayerViewCamera").GetComponent<Camera>();
        mGunArm = transform.Find("PlayerGunArm").gameObject;
        mGunBarrel = mGunBarrel = transform.Find("PlayerGunArm").gameObject.transform.Find("PlayerGun").gameObject.transform.Find("GunBarrel").gameObject;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovement();
        HandleShooting();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Exit")) GameManager.Instance.LoadNextLevel();
    }

    // Handles the movement of the camera with mouse input
    void HandleCameraMovement()
    {
        // Handle player rotation according to mouse movement
        gameObject.transform.Rotate(new Vector3(0.0f, Input.GetAxis("Mouse X") * mouseSensitivity, 0.0f), Space.Self);

        // Handle camera rotation according to vertical mouse movement
        // Camera can only be rotated in the range of [0; 90] u [270; 360] degrees
        // This means that the range (90; 270) degrees is illegal
        // So when doing a rotation, first check if the rotation will put the object in the illegal range. If so, set the rotation to 90 or 270 accordingly
        float yMovement = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate up
        if (yMovement < 0.0f)
        {
            // Check if the rotation is within the illegal range
            if (mCamera.transform.eulerAngles.x + yMovement < 270.0f && mCamera.transform.eulerAngles.x + yMovement > 180.0f)
            {
                // If the rotation is illegal, set it back to the legal range
                mCamera.transform.eulerAngles = new Vector3(270.0f, mCamera.transform.eulerAngles.y, mCamera.transform.eulerAngles.z);
            }
            else
            {
                // If the rotation is legal, do it
                mCamera.transform.Rotate(new Vector3(yMovement, 0.0f, 0.0f), Space.Self);
            }
        }

        // Rotate down
        if (yMovement > 0.0f)
        {
            // Check illegal range
            if (mCamera.transform.eulerAngles.x + yMovement > 90.0f && mCamera.transform.eulerAngles.x + yMovement <= 180.0f)
            {
                // Correct illegal rotation
                mCamera.transform.eulerAngles = new Vector3(90.0f, mCamera.transform.eulerAngles.y, mCamera.transform.eulerAngles.z);
            }
            else
            {
                // Rotate if rotation is legal
                mCamera.transform.Rotate(new Vector3(yMovement, 0.0f, 0.0f), Space.Self);
            }
        }

        // Rotate gun and arm along with the camera
        mGunArm.transform.eulerAngles = mCamera.transform.eulerAngles;
    }

    // Handles all movement of the player, with both mouse and keyboard
    void HandlePlayerMovement()
    {
        // Handle mouse input to move the camera
        HandleCameraMovement();

        // Handle WASD input to move the character
        Vector3 directionVector = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
        directionVector.Normalize();

        mController.Move(directionVector * speed * Time.deltaTime);

        // Handle jumping
        HandleJumping();
    }

    // Handles jumping input
    void HandleJumping()
    {
        // Check to see if the player is on the ground or on a platform
        isAirborne = !Physics.CheckSphere(transform.position, groundDistance, groundMask);

        // If the Space key is pressed and the player is not in the air, set vertical velocity to the jump force
        if (Input.GetKey(KeyCode.Space) && !isAirborne)
        {
            verticalSpeed = jumpForce;
        }

        // Apply movement
        mController.Move(new Vector3(0.0f, verticalSpeed * Time.deltaTime, 0.0f));

        // If the player is in the air, gradually decrease the vertical velocity until the ground is reached, to implement gravity
        if (isAirborne) verticalSpeed -= gravity * Time.deltaTime;
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(mCamera.transform.position, mCamera.transform.forward, out hit, Mathf.Infinity))
            {
                switch (hit.collider.gameObject.tag)
                {
                    case "RegularZombie": hit.collider.gameObject.GetComponentInParent<RegularZombie>().TakeDamage(mDamage); break;
                    case "WeaponZombie": hit.collider.gameObject.GetComponentInParent<WeaponZombie>().TakeDamage(mDamage); break;
                    case "GunZombie": hit.collider.gameObject.GetComponentInParent<GunZombie>().TakeDamage(mDamage); break;
                }
            }
        }
    }

    // Returns the distance to the player. Uses raycasting, which starts from the player camera origin and goes in the direction of its view
    public float GetDistanceFromPlayer(GameObject obj)
    {
        RaycastHit hit;
        int layerMask = 1 << 6;
        
        if (Physics.Raycast(mCamera.transform.position, mCamera.transform.forward, out hit, Mathf.Infinity, layerMask))
            return hit.distance;
        return -1.0f;
    }

    public void MovePlayer(Vector3 dir)
    {
        mController.Move(dir);
    }

    public void MovePlayerTransform(Vector3 dir)
    {
        transform.Translate(dir, Space.Self);
    }

    public void Damage(float dmg)
    {
        health -= dmg;

        if (health <= 0.0f) GameManager.Instance.GameOver();
    }

    public bool PlayerInSight(GameObject obj, int layerMask)
    {
        RaycastHit hit;

        if (!Physics.Raycast(obj.transform.position, transform.position - obj.transform.position, out hit, Mathf.Infinity, layerMask)) return false;

        if (hit.collider.gameObject.CompareTag("Player")) return true;
        return false;
    }
}
