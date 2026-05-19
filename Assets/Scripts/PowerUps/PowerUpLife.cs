// PowerUpLife.cs
// Suma una vida al jugador (máximo 4).

using UnityEngine;

public class PowerUpLife : PowerUpBase
{
    protected override void AplicarEfecto(GameObject jugador)
    {
        GameManager.Instance?.GanarVida();
        Debug.Log("[PowerUp] Vida extra obtenida");
    }
}