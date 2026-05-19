// PlayerHealth.cs
// Maneja las vidas, el escudo y la invulnerabilidad temporal tras recibir daño.

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invulnerabilidad tras daño")]
    [SerializeField] private float tiempoInvulnerable = 1.5f;

    private bool esInvulnerable = false;
    private float timerInvulnerable = 0f;
    private bool escudoActivo = false;
    private float timerEscudo = 0f;

    // Evento para el HUD
    public System.Action OnEscudoCambio;

    private void Update()
    {
        // Countdown invulnerabilidad
        if (esInvulnerable)
        {
            timerInvulnerable -= Time.deltaTime;
            if (timerInvulnerable <= 0f)
                esInvulnerable = false;
        }

        // Countdown escudo
        if (escudoActivo)
        {
            timerEscudo -= Time.deltaTime;
            if (timerEscudo <= 0f)
            {
                escudoActivo = false;
                OnEscudoCambio?.Invoke();
            }
        }
    }

    public void RecibirDano()
    {
        if (esInvulnerable) return;

        // El escudo absorbe el primer golpe
        if (escudoActivo)
        {
            escudoActivo = false;
            OnEscudoCambio?.Invoke();
            ActivarInvulnerabilidad();
            return;
        }

        GameManager.Instance?.PerderVida();
        ActivarInvulnerabilidad();
    }

    private void ActivarInvulnerabilidad()
    {
        esInvulnerable = true;
        timerInvulnerable = tiempoInvulnerable;
    }

    // ── Power-ups ───────────────────────────────────────────────
    public void ActivarEscudo(float duracion)
    {
        escudoActivo = true;
        timerEscudo = duracion;
        OnEscudoCambio?.Invoke();
    }

    public bool EscudoActivo => escudoActivo;

    // Colisiones con enemigos y proyectiles
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyProjectile") || other.CompareTag("Enemy"))
        {
            RecibirDano();
        }
    }
}