// PowerUpRapidFire.cs
// Aumenta la cadencia de disparo común por X segundos.

using UnityEngine;

public class PowerUpRapidFire : PowerUpBase
{
    protected override void AplicarEfecto(GameObject jugador)
    {
        if (jugador.TryGetComponent<PlayerShooter>(out var shooter))
        {
            float duracion = stats != null ? stats.duracion : 5f;
            shooter.ActivarBoostCadencia(duracion);
            Debug.Log($"[PowerUp] Cadencia aumentada por {duracion} segundos");
        }
    }
}