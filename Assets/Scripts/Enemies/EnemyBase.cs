// EnemyBase.cs
// Clase base abstracta para todos los enemigos.
// Define comportamiento común: vida, daño, muerte y drop de power-up.

using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats (asignar ScriptableObject)")]
    [SerializeField] protected EnemyStatsSO stats;

    [Header("Drop de Power-ups")]
    [SerializeField] private GameObject[] prefabsPowerUps; // asignar los 4 prefabs

    // Estado en runtime
    protected int vidaActual;
    protected Transform playerTransform;
    protected bool estaMuerto = false;

    protected virtual void Awake()
    {
        // Buscar al jugador al iniciar
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Start()
    {
        if (stats != null)
            vidaActual = stats.vidaMaxima;
    }

    // ── Daño y muerte ────────────────────────────────────────────
    public void RecibirDano(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;

        // Feedback visual de daño
        StartCoroutine(FlashDano());

        if (vidaActual <= 0)
            Morir();
    }

    protected virtual void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        // Sumar puntos al GameManager
        if (stats != null)
            GameManager.Instance?.SumarPuntos(stats.puntosAlMorir);

        // Chance de dropear power-up
        IntentarDrop();

        Destroy(gameObject);
    }

    private void IntentarDrop()
    {
        if (prefabsPowerUps == null || prefabsPowerUps.Length == 0) return;
        if (stats == null) return;

        float roll = Random.value; // 0 a 1
        if (roll <= stats.probabilidadDrop)
        {
            // Elegir power-up aleatorio del array
            int index = Random.Range(0, prefabsPowerUps.Length);
            if (prefabsPowerUps[index] != null)
                Instantiate(prefabsPowerUps[index], transform.position, Quaternion.identity);
        }
    }

    // ── Feedback visual ──────────────────────────────────────────
    private System.Collections.IEnumerator FlashDano()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        // Cambiar a color rojo brevemente
        foreach (var r in renderers)
            r.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        foreach (var r in renderers)
            r.material.color = Color.white;
    }

    // ── Colisión con el jugador ──────────────────────────────────
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.RecibirDano();
            AlChocarConJugador();
        }
    }

    // Las clases hijas definen qué pasa al chocar
    protected virtual void AlChocarConJugador() { }

    // Acceso protegido a stats para las clases hijas
    protected float Velocidad => stats != null ? stats.velocidad : 3f;
}