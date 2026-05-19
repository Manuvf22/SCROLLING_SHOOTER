// EnemyTypeB.cs — Shooter
// Mantiene distancia del jugador y dispara proyectiles físicos.
// Muere con 3 disparos comunes.

using UnityEngine;

public class EnemyTypeB : EnemyBase
{
    [Header("Disparo")]
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float cadenciaDisparo = 2f;
    [SerializeField] private float distanciaIdeal = 8f;   // distancia que intenta mantener

    private Rigidbody rb;
    private float timerDisparo = 0f;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        if (estaMuerto || playerTransform == null) return;
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        timerDisparo -= Time.deltaTime;
        if (timerDisparo <= 0f)
        {
            Disparar();
            timerDisparo = cadenciaDisparo;
        }
    }

    private void FixedUpdate()
    {
        if (estaMuerto || playerTransform == null) return;
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        MantenerDistancia();
    }

    private void MantenerDistancia()
    {
        float distancia = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 direccion = (playerTransform.position - transform.position).normalized;

        if (distancia > distanciaIdeal + 1f)
            // Acercarse
            rb.linearVelocity = direccion * Velocidad;
        else if (distancia < distanciaIdeal - 1f)
            // Alejarse
            rb.linearVelocity = -direccion * Velocidad;
        else
            rb.linearVelocity = Vector3.zero;

        // Siempre mirar al jugador
        if (direccion != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    private void Disparar()
    {
        if (prefabProyectil == null || puntoDisparo == null) return;

        Vector3 dir = (playerTransform.position - puntoDisparo.position).normalized;
        GameObject p = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.LookRotation(dir));

        if (p.TryGetComponent<Projectile>(out var proj))
            proj.Inicializar(dir, esDelJugador: false);
    }
}