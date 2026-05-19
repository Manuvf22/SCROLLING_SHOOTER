// PlayerShooter.cs
// Maneja los dos tipos de disparo del jugador.
// El crosshair/mira se maneja ocultando el cursor y dibujando uno custom.

using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Puntos de disparo")]
    [SerializeField] private Transform puntoDeDisparo;

    [Header("Prefabs de proyectiles")]
    [SerializeField] private GameObject prefabBalaNormal;
    [SerializeField] private GameObject prefabBalaPotente;

    [Header("Disparo normal")]
    [SerializeField] private float cadenciaNormal = 0.2f;      // segundos entre disparos
    [SerializeField] private float cadenciaBoost = 0.08f;      // con power-up activo

    [Header("Disparo potente")]
    [SerializeField] private float cooldownPotente = 3f;
    [SerializeField] private int maxDisparosPotentes = 1;      // se suman con power-up

    // Estado interno
    private float timerNormal = 0f;
    private float timerCooldownPotente = 0f;
    private int disparosPotentesDisponibles;
    private bool cadenciaBoostActiva = false;
    private float timerBoost = 0f;

    // Referencia al input
    private PlayerController playerController;

    // Eventos para el HUD
    public System.Action<int> OnDisparosPotentesChanged;
    public System.Action<float> OnCooldownChanged; // 0 a 1

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        disparosPotentesDisponibles = maxDisparosPotentes;

        // Ocultar cursor y mostrar crosshair custom
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        timerNormal -= Time.deltaTime;
        ActualizarCooldownPotente();
        ActualizarBoostCadencia();

        var playerInput = playerController.Input;

        // Disparo normal — mientras se mantiene presionado
        if (playerInput.Player.FireNormal.IsPressed() && timerNormal <= 0f)
        {
            DisparoNormal();
        }

        // Disparo potente — al presionar
        if (playerInput.Player.FireHeavy.WasPressedThisFrame())
        {
            DisparoPotente();
        }
    }

    private void DisparoNormal()
    {
        float cadencia = cadenciaBoostActiva ? cadenciaBoost : cadenciaNormal;
        timerNormal = cadencia;

        Vector3 dir = ObtenerDireccionDisparo();
        SpawnProyectil(prefabBalaNormal, dir);
    }

    private void DisparoPotente()
    {
        if (disparosPotentesDisponibles <= 0) return;
        if (timerCooldownPotente > 0f) return;

        disparosPotentesDisponibles--;
        timerCooldownPotente = cooldownPotente;

        Vector3 dir = ObtenerDireccionDisparo();
        SpawnProyectil(prefabBalaPotente, dir);

        OnDisparosPotentesChanged?.Invoke(disparosPotentesDisponibles);
    }

    private Vector3 ObtenerDireccionDisparo()
    {
        // Apunta hacia donde está el mouse en el mundo 3D
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return (hit.point - puntoDeDisparo.position).normalized;

        // Si no impacta nada, dispara hacia adelante según modo cámara
        return PlayerController.EsModoTopDown ? Vector3.forward : Vector3.right;
    }

    private void SpawnProyectil(GameObject prefab, Vector3 direccion)
    {
        if (prefab == null) return;
        GameObject p = Instantiate(prefab, puntoDeDisparo.position, Quaternion.LookRotation(direccion));

        // Pasarle la dirección al proyectil
        if (p.TryGetComponent<Projectile>(out var proj))
            proj.Inicializar(direccion, esDelJugador: true);
    }

    private void ActualizarCooldownPotente()
    {
        if (timerCooldownPotente > 0f)
        {
            timerCooldownPotente -= Time.deltaTime;
            // Notificar al HUD (valor entre 0 y 1)
            OnCooldownChanged?.Invoke(1f - (timerCooldownPotente / cooldownPotente));

            // Al terminar el cooldown, restaurar 1 disparo si llegó a 0
            if (timerCooldownPotente <= 0f && disparosPotentesDisponibles == 0)
            {
                disparosPotentesDisponibles = 1;
                OnDisparosPotentesChanged?.Invoke(disparosPotentesDisponibles);
            }
        }
    }

    // ── Power-ups ───────────────────────────────────────────────
    public void AgregarDisparosPotentes(int cantidad)
    {
        disparosPotentesDisponibles += cantidad;
        OnDisparosPotentesChanged?.Invoke(disparosPotentesDisponibles);
    }

    public void ActivarBoostCadencia(float duracion)
    {
        cadenciaBoostActiva = true;
        timerBoost = duracion;
    }

    private void ActualizarBoostCadencia()
    {
        if (!cadenciaBoostActiva) return;
        timerBoost -= Time.deltaTime;
        if (timerBoost <= 0f)
            cadenciaBoostActiva = false;
    }
}