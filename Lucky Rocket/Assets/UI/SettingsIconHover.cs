using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SettingsIconHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private RectTransform settingsIcon;
    [SerializeField] private float rotationDuration = 0.5f;

    private bool isRotating = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateIcon());
        }
    }

    private IEnumerator RotateIcon()
    {
        isRotating = true;

        float startRotation = settingsIcon.localEulerAngles.z;
        float endRotation = startRotation + 360f;
        float time = 0f;

        while (time < rotationDuration)
        {
            time += Time.deltaTime;

            float t = time / rotationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float rotation = Mathf.Lerp(startRotation, endRotation, t);
            settingsIcon.localEulerAngles = new Vector3(0f, 0f, rotation);

            yield return null;
        }

        settingsIcon.localEulerAngles = new Vector3(0f, 0f, endRotation);

        isRotating = false;
    }
}