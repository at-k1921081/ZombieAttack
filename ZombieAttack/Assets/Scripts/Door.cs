using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private float distanceToPlayer;
    private GameObject playerRef;
    private Camera playerCamera;
    private bool isClosed = true;
    private bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.Find("Player");
        playerCamera = playerRef.transform.Find("PlayerViewCamera").gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = GetDistanceFromPlayerView();

        if (distanceToPlayer < 10.0f && distanceToPlayer >= 0)
        {
            if (Input.GetKey(KeyCode.E) && !isRotating) StartCoroutine(ChangeDoorState());
        }
    }

    float GetDistanceFromPlayerView()
    {
        RaycastHit hit;
        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, Mathf.Infinity);
        if (hit.collider.gameObject == gameObject) return hit.distance;
        foreach (Transform t in transform)
            if (t.gameObject.name == hit.collider.gameObject.name) return hit.distance;
        return -1.0f;
    }

    // Open/Close the door, depending on its current state
    IEnumerator ChangeDoorState()
    {
        isRotating = true;
        float rot = isClosed ? -1.0f : 1.0f;

        for (int i = 0; i < 90; ++i)
        {
            transform.Rotate(new Vector3(0.0f, rot, 0.0f), Space.Self);
            yield return new WaitForSeconds(0.01f);
        }
        isClosed = !isClosed;
        isRotating = false;
    }
}
