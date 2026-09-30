using UnityEngine;
using UnityEngine.UI;

public class BuildPanelToggle : MonoBehaviour
{
    public Button openPanel;
    public RectTransform panel;

    public float moveAmount = 25f;
    public float moveSpeed = 5f;

    public bool open = false;
    private Vector2 closedPosition;
    private Vector2 targetPosition;

    void Start()
    {
        closedPosition = panel.anchoredPosition;
        targetPosition = closedPosition;

        openPanel.onClick.AddListener(TogglePanel);
    }

    void Update()
    {
        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    public void TogglePanel()
    {
        open = !open;

        if (open)
        {
            // 25 pixels naar links
            targetPosition = closedPosition + Vector2.left * moveAmount;
        }
        else
        {
            // Terug naar de oorspronkelijke positie
            targetPosition = closedPosition;
        }
    }
}