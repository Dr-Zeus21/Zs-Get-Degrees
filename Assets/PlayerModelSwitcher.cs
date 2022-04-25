using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelSwitcher : MonoBehaviour
{
   public Transform ModelParent;
    public int modelNumber = 0;
    private void OnValidate()
    {
        foreach (Transform child in ModelParent)
        {
            child.gameObject.SetActive(false);
        }
        ModelParent.GetChild(modelNumber).gameObject.SetActive(true);
    }
}
