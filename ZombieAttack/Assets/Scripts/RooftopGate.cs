using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RooftopGate : MonoBehaviour
{
    public GameObject player;
    
    private float distanceToPlayer;
    bool open = false;
    STATE mState = STATE.IDLE;

    enum STATE
    {
        IDLE,
        OPENING,
        CLOSING
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < 15.0f && Input.GetKeyDown(KeyCode.E)) ChangeState();
    }

    // Change the state of the gate from open to close and vice versa
    void ChangeState()
    {
        if (open && (mState == STATE.IDLE || mState == STATE.OPENING))
        {
            StopAllCoroutines();
            StartCoroutine(Close());
        }
        else if (mState == STATE.IDLE || mState == STATE.CLOSING)
        {
            StopAllCoroutines();
            StartCoroutine(Open());
        }
    }

    // Keep rotating until the gate is rotated -90 degrees to open
    IEnumerator Open()
    {
        mState = STATE.OPENING;
        while (transform.localEulerAngles.x > 270.0f || transform.localEulerAngles.x < 2.0f) // Rotation is done from 360 to 270 degrees
        {
            transform.Rotate(Vector3.right, -1.0f);
            yield return new WaitForSeconds(0.04f);
        }
        transform.Rotate(Vector3.right, 270.0f - transform.localEulerAngles.x); // If rotation is slightly below 270 degrees, set it to 270
        open = true;
        mState = STATE.IDLE;
    }

    // Keep rotating until the gate is rotated 0 degrees to close
    IEnumerator Close()
    {
        mState = STATE.CLOSING;
        while (transform.localEulerAngles.x < 360.0f && transform.localEulerAngles.x >= 270.0f) // Rotation is done from 270 to 360 degrees
        {
            transform.Rotate(Vector3.right, 1.0f);
            yield return new WaitForSeconds(0.04f);
        }
        transform.Rotate(Vector3.right, -transform.localEulerAngles.x); // If end rotation is slightly above 0 degrees, nullify it
        open = false;
        mState = STATE.IDLE;
    }
}
