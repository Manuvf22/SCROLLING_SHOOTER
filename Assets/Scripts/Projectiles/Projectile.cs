// Projectile.cs
// Script único para todos los proyectiles (jugador y enemigos).
// Se inicializa con dirección y origen al spawnearse.

using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private int dano = 1;
    [SerializeField] private float tiempoVida = 4f;

    private bool esDelJugador;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Auto-destruir si no impacta nada
        Destroy(gameObject, tiempoVida);
    }

    // Llamado al instanciar desde PlayerShooter o EnemyBase
    public void Inicializar(Vector3 direccion, bool esDelJugador)
    {
        this.esDelJugador = esDelJugador;
        rb.linearVelocity = direccion.normalized * velocidad;

        // Tag según origen (para que PlayerHealth y EnemyBase sepan qué los golpeó)
        gameObject.tag = esDelJugador ? "PlayerProjectile" : "EnemyProjectile";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (esDelJugador)
        {
            // Daña enemigos
            if (other.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.RecibirDano(dano);
                Destroy(gameObject);
            }
        }
        else
        {
            // Daña al jugador
            if (other.TryGetComponent<PlayerHealth>(out var player))
            {
                player.RecibirDano();
                Destroy(gameObject);
            }
        }

        // Destruir si impacta el escenario
        if (other.CompareTag("Scenery"))
            Destroy(gameObject);
    }

    public int Dano => dano;
}