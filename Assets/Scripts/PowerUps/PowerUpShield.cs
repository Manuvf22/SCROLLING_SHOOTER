// PowerUpShield.cs
// Activa un escudo temporal que absorbe 1 golpe.

using UnityEngine;

public class PowerUpShield : PowerUpBase
{
    protected override void AplicarEfecto(GameObject jugador)
    {
        if (jugador.TryGetComponent<PlayerHealth>(out var health))
        {
            float duracion = stats != null ? stats.duracion : 6f;
            health.ActivarEscudo(duracion);
            Debug.Log($"[PowerUp] Escudo activado por {duracion} segundos");
        }
    }
}