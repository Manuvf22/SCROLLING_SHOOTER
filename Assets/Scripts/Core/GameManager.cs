// GameManager.cs
// Controla el estado global del juego, puntuación, vidas y records.
// Patrón Singleton para acceso global.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración inicial")]
    [SerializeField] private int vidasIniciales = 3;
    [SerializeField] private int maxVidas = 4;

    // Estado
    public GameState Estado { get; private set; } = GameState.Playing;

    // Stats de la run actual
    public int Vidas { get; private set; }
    public int Puntos { get; private set; }
    public float TiempoRun { get; private set; }

    // Records guardados
    public int MejorPuntaje { get; private set; }
    public float MejorTiempo { get; private set; }

    // Eventos para que otros sistemas reaccionen sin acoplamiento
    public System.Action<int> OnVidasCambiaron;
    public System.Action<int> OnPuntosCambiaron;
    public System.Action OnGameOver;
    public System.Action OnPausa;
    public System.Action OnReanudacion;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        CargarRecords();
        IniciarRun();
    }

    private void Update()
    {
        // Acumular tiempo solo mientras se juega
        if (Estado == GameState.Playing)
            TiempoRun += Time.deltaTime;
    }

    // ── Inicialización ──────────────────────────────────────────
    private void IniciarRun()
    {
        Vidas = vidasIniciales;
        Puntos = 0;
        TiempoRun = 0f;
        CambiarEstado(GameState.Playing);
    }

    // ── Estado ──────────────────────────────────────────────────
    public void CambiarEstado(GameState nuevoEstado)
    {
        Estado = nuevoEstado;

        switch (nuevoEstado)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                OnPausa?.Invoke();
                break;
            case GameState.CameraTransition:
                // El CameraTransitionManager maneja el timeScale aquí
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                GuardarRecords();
                OnGameOver?.Invoke();
                break;
        }
    }

    // ── Vidas ───────────────────────────────────────────────────
    public void PerderVida()
    {
        if (Estado != GameState.Playing) return;

        Vidas--;
        OnVidasCambiaron?.Invoke(Vidas);

        if (Vidas <= 0)
            CambiarEstado(GameState.GameOver);
    }

    public void GanarVida()
    {
        if (Vidas >= maxVidas) return;
        Vidas++;
        OnVidasCambiaron?.Invoke(Vidas);
    }

    // ── Puntos ──────────────────────────────────────────────────
    public void SumarPuntos(int cantidad)
    {
        Puntos += cantidad;
        OnPuntosCambiaron?.Invoke(Puntos);
    }

    // ── Pausa ───────────────────────────────────────────────────
    public void TogglePausa()
    {
        if (Estado == GameState.Playing)
            CambiarEstado(GameState.Paused);
        else if (Estado == GameState.Paused)
        {
            CambiarEstado(GameState.Playing);
            OnReanudacion?.Invoke();
        }
    }

    // ── Escenas ─────────────────────────────────────────────────
    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ── Records ─────────────────────────────────────────────────
    private void GuardarRecords()
    {
        if (Puntos > MejorPuntaje)
        {
            MejorPuntaje = Puntos;
            PlayerPrefs.SetInt("MejorPuntaje", MejorPuntaje);
        }
        if (TiempoRun > MejorTiempo)
        {
            MejorTiempo = TiempoRun;
            PlayerPrefs.SetFloat("MejorTiempo", MejorTiempo);
        }
        PlayerPrefs.Save();
    }

    private void CargarRecords()
    {
        MejorPuntaje = PlayerPrefs.GetInt("MejorPuntaje", 0);
        MejorTiempo = PlayerPrefs.GetFloat("MejorTiempo", 0f);
    }
}