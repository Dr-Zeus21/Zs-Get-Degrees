using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform other;
    public bool printOut = false;

    // Update is called once per frame
    void Update()
    {
        if (printOut)
        {
            printOut = false;
            Vector3 targetDir = other.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            print(angle);
        }
    }
}
