// PowerUpHeavyShot.cs
// Agrega 2 disparos potentes inmediatos al contador del jugador.

using UnityEngine;

public class PowerUpHeavyShot : PowerUpBase
{
    protected override void AplicarEfecto(GameObject jugador)
    {
        if (jugador.TryGetComponent<PlayerShooter>(out var shooter))
        {
            int cantidad = stats != null ? stats.valorEntero : 2;
            shooter.AgregarDisparosPotentes(cantidad);
            Debug.Log($"[PowerUp] +{cantidad} disparos potentes");
        }
    }
}