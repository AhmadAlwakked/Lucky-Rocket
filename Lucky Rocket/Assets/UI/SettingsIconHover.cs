using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SettingsIconHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private RectTransform leftIcon;
    [SerializeField] private RectTransform rightIcon;
    [SerializeField] private float rotationDuration = 0.5f;

    private bool isRotating = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateIcons());
        }
    }

    private IEnumerator RotateIcons()
    {
        isRotating = true;

        float leftStart = leftIcon.localEulerAngles.z;
        float rightStart = rightIcon.localEulerAngles.z;

        float leftEnd = leftStart - 360f;
        float rightEnd = rightStart + 360f;

        float time = 0f;

        while (time < rotationDuration)
        {
            time += Time.deltaTime;

            float t = time / rotationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float leftRotation = Mathf.Lerp(leftStart, leftEnd, t);
            float rightRotation = Mathf.Lerp(rightStart, rightEnd, t);

            leftIcon.localEulerAngles = new Vector3(0f, 0f, leftRotation);
            rightIcon.localEulerAngles = new Vector3(0f, 0f, rightRotation);

            yield return null;
        }

        leftIcon.localEulerAngles = new Vector3(0f, 0f, leftEnd);
        rightIcon.localEulerAngles = new Vector3(0f, 0f, rightEnd);

        isRotating = false;
    }
}