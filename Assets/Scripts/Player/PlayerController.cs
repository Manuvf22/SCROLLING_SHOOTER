// PlayerController.cs
// Maneja el movimiento del jugador adaptado según la cámara activa.
// Recibe la dirección de CameraTransitionManager para saber en qué plano moverse.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float limiteMovimiento = 4f; // rango máximo desde centro

    private Rigidbody rb;
    private PlayerInputActions input;
    private Vector2 moveInput;

    // El CameraTransitionManager setea esto
    public static bool EsModoTopDown { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        input = new PlayerInputActions();
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private void Update()
    {
        // Leer input cada frame
        moveInput = input.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.Estado != GameState.Playing) return;

        Mover();
    }

    private void Mover()
    {
        Vector3 direccion;

        if (EsModoTopDown)
        {
            // Vista desde arriba: WASD mueve en plano XZ
            direccion = new Vector3(moveInput.x, 0f, moveInput.y);
        }
        else
        {
            // Vista de costado: WASD mueve en plano XY
            direccion = new Vector3(moveInput.x, moveInput.y, 0f);
        }

        // Aplicar velocidad
        rb.linearVelocity = direccion * velocidad;

        // Inclinar levemente el avión según el movimiento (efecto visual)
        AplicarInclinacion(direccion);
    }

    private void AplicarInclinacion(Vector3 direccion)
    {
        if (EsModoTopDown)
        {
            // Inclina en Z al moverse lateralmente
            float targetRoll = -direccion.x * 25f;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetRoll);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }
        else
        {
            // Inclina en Z al moverse arriba/abajo
            float targetRoll = -direccion.y * 20f;
            Quaternion targetRot = Quaternion.Euler(targetRoll, 0f, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }
    }

    // Acceso al input para otros scripts del player
    public PlayerInputActions Input => input;
}