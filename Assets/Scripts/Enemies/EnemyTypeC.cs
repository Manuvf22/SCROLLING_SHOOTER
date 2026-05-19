// EnemyTypeC.cs — Dasher
// Se queda quieto temblando, luego hace dash veloz hacia el jugador.
// Muere con 4 disparos comunes.

using UnityEngine;

public class EnemyTypeC : EnemyBase
{
    [Header("Dash")]
    [SerializeField] private float tiempoEspera = 2f;      // tiempo temblando
    [SerializeField] private float velocidadDash = 18f;
    [SerializeField] private float duracionDash = 0.4f;
    [SerializeField] private float intensidadTemblor = 0.05f;

    private Rigidbody rb;

    // Máquina de estados simple
    private enum EstadoDasher { Esperando, Temblando, Dash }
    private EstadoDasher estado = EstadoDasher.Esperando;

    private float timerEstado = 0f;
    private Vector3 posicionBase;
    private Vector3 direccionDash;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    protected override void Start()
    {
        base.Start();
        // Empezar con una pequeña espera antes de temblar
        timerEstado = 0.5f;
        estado = EstadoDasher.Esperando;
    }

    private void Update()
    {
        if (estaMuerto || playerTransform == null) return;
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        timerEstado -= Time.deltaTime;

        switch (estado)
        {
            case EstadoDasher.Esperando:
                if (timerEstado <= 0f)
                    IniciarTemblor();
                break;

            case EstadoDasher.Temblando:
                AplicarTemblor();
                if (timerEstado <= 0f)
                    IniciarDash();
                break;

            case EstadoDasher.Dash:
                if (timerEstado <= 0f)
                    FinalizarDash();
                break;
        }
    }

    private void IniciarTemblor()
    {
        estado = EstadoDasher.Temblando;
        timerEstado = tiempoEspera;
        posicionBase = transform.position;
        rb.linearVelocity = Vector3.zero;

        // Guardar dirección al jugador para el dash
        direccionDash = (playerTransform.position - transform.position).normalized;
    }

    private void AplicarTemblor()
    {
        // Oscilación rápida alrededor de la posición base
        float offsetX = Mathf.Sin(Time.time * 40f) * intensidadTemblor;
        float offsetY = Mathf.Cos(Time.time * 35f) * intensidadTemblor;
        transform.position = posicionBase + new Vector3(offsetX, offsetY, 0f);
    }

    private void IniciarDash()
    {
        estado = EstadoDasher.Dash;
        timerEstado = duracionDash;

        // Actualizar dirección justo antes del dash
        direccionDash = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direccionDash * velocidadDash;
    }

    private void FinalizarDash()
    {
        rb.linearVelocity = Vector3.zero;
        estado = EstadoDasher.Esperando;
        timerEstado = 0.8f; // pequeña pausa antes del próximo ciclo
    }

    protected override void AlChocarConJugador()
    {
        // El Dasher no muere al chocar, solo frena
        FinalizarDash();
    }
}