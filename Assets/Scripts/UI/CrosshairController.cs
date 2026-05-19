// CrosshairController.cs
// Mueve una imagen UI para que siga al mouse, reemplazando el cursor del sistema.

using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairRect;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        // Convertir posición del mouse a coordenadas del canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            UnityEngine.Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            crosshairRect.localPosition = localPoint;
        }
    }
}