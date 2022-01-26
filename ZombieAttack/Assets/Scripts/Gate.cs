using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private GameObject player;
    private Vector3 directionToPlayer;
    private float distanceToPlayer;
    private bool isMoving = false;
    private float movementSpeed = 0.1f;
    private float secondsInterval = 0.01f;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(gameObject.transform.position, player.transform.position);
        directionToPlayer = player.transform.position - transform.position;

        Physics.Raycast(transform.position + new Vector3(0.0f, 4.0f, 0.0f), directionToPlayer + new Vector3(0.0f, 4.0f, 0.0f), out hit, Mathf.Infinity);

        // Keep the gate opened while the player is less than 30 units away, and keep it closed otherwise
        if (distanceToPlayer < 30.0f && hit.collider.gameObject.CompareTag("Player"))
        {
            StopCoroutine(CloseGate());
            if (!isMoving) StartCoroutine(OpenGate());
        }
        else
        {
            StopCoroutine(OpenGate());
            if (!isMoving) StartCoroutine(CloseGate());
        }
    }

    IEnumerator OpenGate()
    {
        isMoving = true;

        while (transform.position.y < 20)
        {
            transform.Translate(new Vector3(0.0f, movementSpeed, 0.0f), Space.Self);
            yield return new WaitForSeconds(secondsInterval); // Take 2 seconds to fully open the gate
        }

        isMoving = false;
    }

    IEnumerator CloseGate()
    {
        isMoving = true;

        while (transform.position.y > 0)
        {
            transform.Translate(new Vector3(0.0f, -movementSpeed, 0.0f), Space.Self);
            yield return new WaitForSeconds(secondsInterval); // Take 2 seconds to fully close the gate
        }

        isMoving = false;
    }
}
