using UnityEngine;

public class BuildPanelToggle : MonoBehaviour
{
    public Animator animator;

    private bool isOpen = false;

    public void TogglePanel()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }
}