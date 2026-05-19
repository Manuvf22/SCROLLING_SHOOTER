// EnemyTypeA.cs — Kamikaze
// Se mueve directo hacia el jugador. Al chocar quita vida y muere.
// Muere con 2 disparos comunes.

using UnityEngine;

public class EnemyTypeA : EnemyBase
{
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (estaMuerto || playerTransform == null) return;
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        // Moverse directo hacia el jugador
        Vector3 direccion = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direccion * Velocidad;

        // Rotar hacia el jugador
        if (direccion != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    protected override void AlChocarConJugador()
    {
        // Al chocar con el jugador, muere solo
        Morir();
    }
}