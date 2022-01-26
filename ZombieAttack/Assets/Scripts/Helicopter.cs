using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Collider area;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    
    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (area.bounds.Contains(player.transform.position) && !gameOver)
        {
            StartCoroutine(WinGame());
            gameOver = true;
        }
    }

    // Close the helicopter doors then win the game
    IEnumerator WinGame()
    {
        yield return StartCoroutine(CloseHeliDoors());
        GameManager.Instance.LoadNextLevel();
    }

    // Apply animations for closing the helicopter doors. First close them, then bring them in
    IEnumerator CloseHeliDoors()
    {
        yield return StartCoroutine(CloseDoors());
        yield return StartCoroutine(BringDoorsIn());
    }

    // Close the doors of the helicopter
    IEnumerator CloseDoors()
    {
        for (int i = 0; i < 30; ++i)
        {
            leftDoor.transform.Translate(0.0f, 0.0f, 0.1f);
            rightDoor.transform.Translate(0.0f, 0.0f, -0.1f);

            yield return new WaitForSeconds(0.1f);
        }
    }

    // When the doors are already closed, bring them back in towards the helicopter
    IEnumerator BringDoorsIn()
    {
        for (int i = 0; i < 10; ++i)
        {
            leftDoor.transform.Translate(0.1f, 0.0f, 0.0f);
            rightDoor.transform.Translate(0.1f, 0.0f, 0.0f);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
