// PowerUpStatsSO.cs
// ScriptableObject con la configuración de cada power-up.
// Crear 4 assets: PU_Shield, PU_Life, PU_HeavyShot, PU_RapidFire

using UnityEngine;

public enum PowerUpTipo
{
    Escudo,
    VidaExtra,
    DisparosPotentes,
    CadenciaAumentada
}

[CreateAssetMenu(fileName = "PowerUpStats", menuName = "ScriptableObjects/PowerUpStats")]
public class PowerUpStatsSO : ScriptableObject
{
    public PowerUpTipo tipo;

    [Header("Duración (0 = instantáneo)")]
    public float duracion = 5f;

    [Header("Valor (disparos extra, etc.)")]
    public int valorEntero = 2;
}