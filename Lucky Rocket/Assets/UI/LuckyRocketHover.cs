using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LuckyRocketHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Curve")]
    [Range(-100f, 100f)]
    public float curveAmount = 20f;

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
        ApplyCurve();

        Vector3 targetScale = isHovering
            ? originalScale * hoverScale
            : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        Color targetColor = isHovering
            ? hoverColor
            : normalColor;

        text.color = Color.Lerp(
            text.color,
            targetColor,
            Time.deltaTime * animationSpeed
        );
    }

    void ApplyCurve()
    {
        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;

        float centerX = text.bounds.center.x;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            Vector3[] vertices =
                textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 pos = vertices[vertexIndex + j];

                float x = pos.x - centerX;

                pos.y += (x * x) * curveAmount * 0.001f;

                vertices[vertexIndex + j] = pos;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices =
                textInfo.meshInfo[i].vertices;

            text.UpdateGeometry(
                textInfo.meshInfo[i].mesh,
                i
            );
        }
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