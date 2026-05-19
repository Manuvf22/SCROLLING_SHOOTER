// CameraTransitionManager.cs
// Controla cuándo y cómo se cambia entre las dos cámaras.
// Detecta si hay pocos enemigos cerca y ejecuta la transición con pausa suave.


using Unity.Cinemachine;
using UnityEngine;

public class CameraTransitionManager : MonoBehaviour
{
    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineCamera vcamTopDown;
    [SerializeField] private CinemachineCamera vcamSideScroll;

    [Header("Condición de transición")]
    [SerializeField] private float intervaloChequeo = 8f;      // cada cuántos segundos evalúa
    [SerializeField] private float radioDeteccionEnemigos = 20f;
    [SerializeField] private int maxEnemigosPermitidos = 2;    // umbral para transicionar

    [Header("Pausa suave")]
    [SerializeField] private float duracionSlowMo = 1.8f;
    [SerializeField] private float timeScaleSlowMo = 0.15f;
    [SerializeField] private GameObject overlayTransicion;     // Panel UI "¡Cambio de perspectiva!"

    // Estado
    private bool esModoTopDown = true;
    private float timerChequeo = 0f;
    private bool transicionEnCurso = false;
    private float timerTransicion = 0f;

    // Referencia al player para detectar enemigos cercanos
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Estado inicial: TopDown activa
        AplicarCamara(topDown: true, conTransicion: false);

        // Empezar con overlay oculto
        if (overlayTransicion != null)
            overlayTransicion.SetActive(false);

        // Dar tiempo inicial antes del primer chequeo
        timerChequeo = intervaloChequeo;
    }

    private void Update()
    {
        if (GameManager.Instance?.Estado == GameState.GameOver) return;
        if (transicionEnCurso) { ManejarSlowMo(); return; }
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        timerChequeo -= Time.deltaTime;
        if (timerChequeo <= 0f)
        {
            timerChequeo = intervaloChequeo;
            EvaluarTransicion();
        }
    }

    // ── Evaluación ───────────────────────────────────────────────
    private void EvaluarTransicion()
    {
        if (playerTransform == null) return;

        int enemigasCercanos = ContarEnemigosCercanos();

        if (enemigasCercanos <= maxEnemigosPermitidos)
            IniciarTransicion();
    }

    private int ContarEnemigosCercanos()
    {
        // Busca todos los enemigos activos en escena
        EnemyBase[] enemigos = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var e in enemigos)
        {
            float distancia = Vector3.Distance(playerTransform.position, e.transform.position);
            if (distancia <= radioDeteccionEnemigos)
                count++;
        }

        return count;
    }

    // ── Transición ───────────────────────────────────────────────
    private void IniciarTransicion()
    {
        transicionEnCurso = true;
        timerTransicion = duracionSlowMo;

        // Cambiar estado del juego
        GameManager.Instance?.CambiarEstado(GameState.CameraTransition);

        // Slow motion
        Time.timeScale = timeScaleSlowMo;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Mostrar overlay UI
        if (overlayTransicion != null)
            overlayTransicion.SetActive(true);

        // Cambiar la cámara activa
        esModoTopDown = !esModoTopDown;
        AplicarCamara(esModoTopDown, conTransicion: true);

        // Actualizar el modo en PlayerController
        PlayerController.EsModoTopDown = esModoTopDown;
    }

    private void ManejarSlowMo()
    {
        // Usar unscaled delta porque timeScale está alterado
        timerTransicion -= Time.unscaledDeltaTime;

        if (timerTransicion <= 0f)
            FinalizarTransicion();
    }

    private void FinalizarTransicion()
    {
        transicionEnCurso = false;

        // Restaurar tiempo normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Ocultar overlay
        if (overlayTransicion != null)
            overlayTransicion.SetActive(false);

        // Volver a estado playing
        GameManager.Instance?.CambiarEstado(GameState.Playing);

        // Resetear timer para no transicionar inmediatamente
        timerChequeo = intervaloChequeo;
    }

    // ── Cámaras ──────────────────────────────────────────────────
    private void AplicarCamara(bool topDown, bool conTransicion)
    {
        if (topDown)
        {
            vcamTopDown.Priority = 10;
            vcamSideScroll.Priority = 9;
        }
        else
        {
            vcamTopDown.Priority = 9;
            vcamSideScroll.Priority = 10;
        }
    }

    // Acceso público para el SpawnManager
    public bool EsModoTopDown => esModoTopDown;
}