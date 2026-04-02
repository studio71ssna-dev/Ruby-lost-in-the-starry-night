using UnityEngine;

public class EnableChildren : MonoBehaviour
{
    // Adding ContextMenu lets you right-click this script in the Inspector 
    // and click "Enable All Children" to run it in the editor.
    [ContextMenu("Enable All Children")]
    public void ActivateAllChildren()
    {
        // Loop through every direct child of the GameObject this script is attached to
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Optional: If you want it to happen automatically the moment this object wakes up
    private void OnEnable()
    {
        ActivateAllChildren();
    }
}