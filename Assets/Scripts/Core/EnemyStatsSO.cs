// EnemyStatsSO.cs
// ScriptableObject con los stats de cada tipo de enemigo.
// Crear 3 assets: EnemyStats_A, EnemyStats_B, EnemyStats_C

using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Combate")]
    public int vidaMaxima = 2;
    public int danoAlJugador = 1;
    public int puntosAlMorir = 100;

    [Header("Movimiento")]
    public float velocidad = 4f;

    [Header("Drop")]
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.2f; // 20% de chance de dropear power-up
}