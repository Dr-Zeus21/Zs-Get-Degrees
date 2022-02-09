using UnityEngine;

public class Building : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;

        }
    }

    // exiting an intersection blocks the player from turning
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
