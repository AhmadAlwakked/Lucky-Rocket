using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LuckyRocketHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Effect")]
    public float hoverScale = 1.08f;
    public float animationSpeed = 8f;

    [Header("Text Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private TMP_Text text;
    private Vector3 originalScale;
    private bool isHovering;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        originalScale = transform.localScale;

        text.color = normalColor;
    }

    void Update()
    {
        // Scale effect
        Vector3 targetScale = isHovering
            ? originalScale * hoverScale
            : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        // Color effect
        Color targetColor = isHovering
            ? hoverColor
            : normalColor;

        text.color = Color.Lerp(
            text.color,
            targetColor,
            Time.deltaTime * animationSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}