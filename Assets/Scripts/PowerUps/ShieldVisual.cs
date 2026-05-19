// ShieldVisual.cs
// Muestra una esfera visual cuando el escudo está activo.
// Se suscribe al evento del PlayerHealth para activarse/desactivarse.

using UnityEngine;

public class ShieldVisual : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Renderer shieldRenderer;

    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        shieldRenderer = GetComponent<Renderer>();

        // Empezar oculto
        shieldRenderer.enabled = false;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnEscudoCambio += ActualizarVisual;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnEscudoCambio -= ActualizarVisual;
    }

    private void ActualizarVisual()
    {
        shieldRenderer.enabled = playerHealth.EscudoActivo;
    }
}