using UnityEngine;

public class Test : MonoBehaviour
{
    public bool PrintRotation = false;

    private void OnValidate()
    {
        if (PrintRotation)
        {
            PrintRotation = false;
            print(transform.rotation);
            print(transform.localRotation);
        }
    }
}
