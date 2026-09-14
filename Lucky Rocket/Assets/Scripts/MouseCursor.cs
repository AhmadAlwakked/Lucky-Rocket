using UnityEngine;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour
{
    [Header("Mouse Cursor")]
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;

    [Header("Flash Text")]
    public TextMeshProUGUI flashText;

    void Start()
    {
        // Custom cursor
        Cursor.visible = true;
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

        // Start text sequence
        StartCoroutine(ShowFlashText());
    }

    IEnumerator ShowFlashText()
    {
        flashText.gameObject.SetActive(true);

        // LOADING
        flashText.text = "LOADING.";
        yield return new WaitForSeconds(1f);

        // LOADING
        flashText.text = "LOADING..";
        yield return new WaitForSeconds(0.5f);

        // LOADING
        flashText.text = "LOADING...";
        yield return new WaitForSeconds(0.4f);

        // LOADING
        flashText.text = "LOADING....";
        yield return new WaitForSeconds(0.3f);

        // SYSTEM READY
        flashText.text = "SYSTEM READY";
        yield return new WaitForSeconds(0.3f);

        // Start smooth flashing
        StartCoroutine(FlashReadyText());
    }

    IEnumerator FlashReadyText()
    {
        float timer = 0f;

        Color color = flashText.color;

        while (timer < 5f)
        {
            // Smooth fade IN
            for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * 3f)
            {
                color.a = alpha;
                flashText.color = color;
                yield return null;
            }

            // Smooth fade OUT
            for (float alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * 3f)
            {
                color.a = alpha;
                flashText.color = color;
                yield return null;
            }

            timer += 0.6f;
        }

        // Smoothly stop flashing
        float currentAlpha = color.a;

        for (float alpha = currentAlpha; alpha < 1f; alpha += Time.deltaTime * 2f)
        {
            color.a = alpha;
            flashText.color = color;
            yield return null;
        }

        // SYSTEM READY stays visible
        color.a = 1f;
        flashText.color = color;
    }
}