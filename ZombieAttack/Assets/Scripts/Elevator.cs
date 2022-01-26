using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject mPlayer;
    public Collider mArea;

    private Player playerScript;

    private bool isMoving = false;
    private float distanceToPlayer;
    private DOOR_STATE mState = DOOR_STATE.CLOSE;
    private int floor = 1;
    private bool travelReady = true;

    // Which door is going to be affected
    public enum DOOR_SIDE
    {
        LEFT = 1,
        RIGHT = -1
    }

    // Open or Close the door
    public enum DOOR_STATE
    {
        OPEN = 1,
        CLOSE = -1
    }

    public enum VERTICAL_DIRECTION
    {
        DOWN = -1,
        UP = 1
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = mPlayer.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInElevator() && travelReady)
        {
            ChangeFloor();
        }
        else
        {
            distanceToPlayer = Vector3.Distance(transform.position, mPlayer.transform.position);
            if (!IsPlayerInElevator() && distanceToPlayer > 20.0f) travelReady = true;

            if (Input.GetKey(KeyCode.E) && distanceToPlayer < 15.0f && NoWallsObstructing() && mState == DOOR_STATE.CLOSE)
                ChangeDoorStates(DOOR_STATE.OPEN);

            if (mState == DOOR_STATE.OPEN && distanceToPlayer > 40.0f)
                ChangeDoorStates(DOOR_STATE.CLOSE);
        }
    }

    // Changes the states of the doors from open to close and vice versa. Can only work when the doors are not already moving
    public void ChangeDoorStates(DOOR_STATE state)
    {
        if (!isMoving) StartCoroutine(DoorAnimation(state));
    }

    // Perform the actual movement of the doors as they're changing their states
    IEnumerator DoorAnimation(DOOR_STATE state)
    {
        isMoving = true;
        for (int i = 0; i < 120; ++i)
        {
            leftDoor.transform.Translate(new Vector3(0.0f, 0.0f, (int)state * 0.1f * (int)DOOR_SIDE.LEFT), Space.Self);
            rightDoor.transform.Translate(new Vector3(0.0f, 0.0f, (int)state * 0.1f * (int)DOOR_SIDE.RIGHT), Space.Self);
            yield return new WaitForSeconds(0.01f);
        }
        isMoving = false;
        mState = state;
    }

    // Returns true if there are no walls between the player and the elevator, false otherwise
    private bool NoWallsObstructing()
    {
        RaycastHit hit;
        Vector3 direction = Vector3.Normalize(transform.position - mPlayer.transform.position);

        Physics.Raycast(mPlayer.transform.position, direction, out hit, Mathf.Infinity);

        if (hit.collider.gameObject.CompareTag("Elevator")) return true;
        return false;
    }

    // Returns true if the player is inside the elevator area, and false otherwise
    private bool IsPlayerInElevator()
    {
        if (mArea.bounds.Contains(mPlayer.transform.position)) return true;
        return false;
    }

    // Go to floor 1 if the elevator is on floor 2, and to floor 2 if it's on floor 1
    private void ChangeFloor()
    {
        if (!isMoving && mState == DOOR_STATE.OPEN)
        {
            ChangeDoorStates(DOOR_STATE.CLOSE);
        }
        else if (!isMoving && mState == DOOR_STATE.CLOSE && travelReady)
        {
            if (floor == 1)  StartCoroutine(Move(VERTICAL_DIRECTION.UP));
            else if (floor == 2) StartCoroutine(Move(VERTICAL_DIRECTION.DOWN));
        }
        else if (!isMoving && mState == DOOR_STATE.CLOSE)
        {
            ChangeDoorStates(DOOR_STATE.OPEN);
        }
    }

    // Perform the movement of the elevator as it's going up/down, depending on the given direction
    IEnumerator Move(VERTICAL_DIRECTION dir)
    {
        isMoving = true;
        travelReady = false;

        // TO DO: Open/Close doors, then execute moving up/down behaviour

        for (int i = 0; i < 21; ++i)
        {
            playerScript.MovePlayer(new Vector3(0.0f, 10.0f * (int)dir, 0.0f));
            transform.Translate(new Vector3(0.0f, 10.0f * (int)dir, 0.0f), Space.Self);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.0f);
        isMoving = false;
        floor = floor == 1 ? 2 : 1;
    }
}
