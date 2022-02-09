using UnityEngine;

public class Intersection : MonoBehaviour
{
    private Player player;
    private bool playerInside = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        // if the player tries to turn the camera they are set to the middle of the intersection they are in
        if (playerInside && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2)))
        {
            player.transform.position = transform.position;
        }
    }

    // entering an intersection allows the player to turn
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.turnable = true;
            playerInside = true;
        }
    }

    // exiting an intersection blocks the player from turning
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.turnable = false;
            playerInside = false;
        }
    }
}
