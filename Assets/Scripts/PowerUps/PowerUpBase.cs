// PowerUpBase.cs
// Clase base para todos los power-ups.
// Flota en el lugar, rota visualmente, y al tocarlo aplica su efecto al jugador.

using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] protected PowerUpStatsSO stats;
    [SerializeField] private float velocidadRotacion = 90f;
    [SerializeField] private float amplitudFlotacion = 0.3f;
    [SerializeField] private float velocidadFlotacion = 2f;
    [SerializeField] private float tiempoVida = 10f; // se destruye si nadie lo recoge

    private Vector3 posicionBase;

    private void Start()
    {
        posicionBase = transform.position;
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        // Rotación continua
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime);

        // Flotación suave arriba y abajo
        float offsetY = Mathf.Sin(Time.time * velocidadFlotacion) * amplitudFlotacion;
        transform.position = posicionBase + new Vector3(0f, offsetY, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Aplicar efecto según la clase hija
        AplicarEfecto(other.gameObject);
        Destroy(gameObject);
    }

    // Cada power-up implementa su propio efecto
    protected abstract void AplicarEfecto(GameObject jugador);
}