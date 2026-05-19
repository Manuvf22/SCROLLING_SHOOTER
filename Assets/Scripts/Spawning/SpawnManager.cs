// SpawnManager.cs
// Spawnea enemigos en los bordes visibles según la cámara activa.
// La dificultad aumenta progresivamente con el tiempo de juego.

using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    [SerializeField] private GameObject prefabEnemyA;
    [SerializeField] private GameObject prefabEnemyB;
    [SerializeField] private GameObject prefabEnemyC;

    [Header("Configuración de spawn")]
    [SerializeField] private float intervaloInicial = 2.5f;   // segundos entre spawns al inicio
    [SerializeField] private float intervaloMinimo = 0.6f;    // límite inferior de velocidad
    [SerializeField] private float reduccionPorMinuto = 0.3f; // cuánto baja el intervalo por minuto

    [Header("Límites del área de juego")]
    [SerializeField] private float limiteX = 8f;   // ancho del área visible
    [SerializeField] private float limiteY = 5f;   // alto (para side-scroll)
    [SerializeField] private float limiteZ = 8f;   // profundidad (para top-down)
    [SerializeField] private float alturaTopDown = 0f;   // Y fija en top-down
    [SerializeField] private float profundidadSide = 0f; // Z fija en side-scroll

    [Header("Referencias")]
    [SerializeField] private CameraTransitionManager cameraManager;

    // Runtime
    private float timerSpawn = 0f;
    private float tiempoJuego = 0f;

    // Probabilidades acumuladas por dificultad (A, B, C)
    // Al inicio spawnea casi solo tipo A, con el tiempo aparecen B y C
    private float probA = 1f;
    private float probB = 0f;
    private float probC = 0f;

    private void Start()
    {
        if (cameraManager == null)
            cameraManager = FindFirstObjectByType<CameraTransitionManager>();
    }

    private void Update()
    {
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        tiempoJuego += Time.deltaTime;
        timerSpawn -= Time.deltaTime;

        ActualizarDificultad();

        if (timerSpawn <= 0f)
        {
            SpawnEnemigo();
            timerSpawn = ObtenerIntervaloActual();
        }
    }

    // ── Dificultad ───────────────────────────────────────────────
    private void ActualizarDificultad()
    {
        // Cada minuto que pasa, aumenta la proporción de enemigos difíciles
        float minutos = tiempoJuego / 60f;

        // Tipo A baja de 100% a 40% en los primeros 3 minutos
        probA = Mathf.Clamp(1f - (minutos * 0.2f), 0.4f, 1f);

        // Tipo B aparece desde el minuto 1
        probB = Mathf.Clamp((minutos - 0.5f) * 0.25f, 0f, 0.4f);

        // Tipo C aparece desde el minuto 2
        probC = Mathf.Clamp((minutos - 1.5f) * 0.15f, 0f, 0.3f);

        // Normalizar para que sumen 1
        float total = probA + probB + probC;
        if (total > 0)
        {
            probA /= total;
            probB /= total;
            probC /= total;
        }
    }

    private float ObtenerIntervaloActual()
    {
        float minutos = tiempoJuego / 60f;
        float intervalo = intervaloInicial - (minutos * reduccionPorMinuto);
        return Mathf.Max(intervalo, intervaloMinimo);
    }

    // ── Spawn ────────────────────────────────────────────────────
    private void SpawnEnemigo()
    {
        GameObject prefab = ElegirPrefab();
        if (prefab == null) return;

        Vector3 posicion = ObtenerPosicionSpawn();
        Instantiate(prefab, posicion, Quaternion.identity);
    }

    private GameObject ElegirPrefab()
    {
        float roll = Random.value;

        if (roll < probA) return prefabEnemyA;
        if (roll < probA + probB) return prefabEnemyB;
        return prefabEnemyC;
    }

    private Vector3 ObtenerPosicionSpawn()
    {
        bool esTopDown = cameraManager != null && cameraManager.EsModoTopDown;

        if (esTopDown)
            return SpawnBordeTopDown();
        else
            return SpawnBordeSideScroll();
    }

    // Spawnea en los 4 bordes del plano XZ (vista top-down)
    private Vector3 SpawnBordeTopDown()
    {
        int borde = Random.Range(0, 4);
        float x, z;

        switch (borde)
        {
            case 0: // borde superior (adelante)
                x = Random.Range(-limiteX, limiteX);
                z = limiteZ;
                break;
            case 1: // borde inferior (atrás)
                x = Random.Range(-limiteX, limiteX);
                z = -limiteZ;
                break;
            case 2: // borde derecho
                x = limiteX;
                z = Random.Range(-limiteZ, limiteZ);
                break;
            default: // borde izquierdo
                x = -limiteX;
                z = Random.Range(-limiteZ, limiteZ);
                break;
        }

        return new Vector3(x, alturaTopDown, z);
    }

    // Spawnea en los 4 bordes del plano XY (vista side-scroll)
    private Vector3 SpawnBordeSideScroll()
    {
        int borde = Random.Range(0, 4);
        float x, y;

        switch (borde)
        {
            case 0: // borde derecho (adelante del scroll)
                x = limiteX;
                y = Random.Range(-limiteY, limiteY);
                break;
            case 1: // borde izquierdo
                x = -limiteX;
                y = Random.Range(-limiteY, limiteY);
                break;
            case 2: // borde superior
                x = Random.Range(-limiteX, limiteX);
                y = limiteY;
                break;
            default: // borde inferior
                x = Random.Range(-limiteX, limiteX);
                y = -limiteY;
                break;
        }

        return new Vector3(x, y, profundidadSide);
    }

    // ── Utilidad pública ─────────────────────────────────────────
    // El CameraTransitionManager consulta esto para decidir si transicionar
    public int ContarEnemigosVivos()
    {
        return FindObjectsByType<EnemyBase>(FindObjectsSortMode.None).Length;
    }
}