// PlayerDodge.cs
// Barrel roll al presionar Shift. Consume y recarga barra de energía.
// Evento para que el HUD muestre la barra.

using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [Header("Esquive")]
    [SerializeField] private float duracionRoll = 0.4f;
    [SerializeField] private float velocidadRoll = 720f;   // grados por segundo
    [SerializeField] private float costoEnergia = 30f;

    [Header("Energía")]
    [SerializeField] private float energiaMax = 100f;
    [SerializeField] private float recargaEnergia = 20f;   // por segundo

    private float energia;
    private bool estaEsquivando = false;
    private float timerRoll = 0f;
    private float gradosRotados = 0f;
    private Vector3 ejeRoll;

    private PlayerController playerController;

    // Evento para HUD (0 a 1)
    public System.Action<float> OnEnergiaChanged;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        energia = energiaMax;
    }

    private void Update()
    {
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        RecargarEnergia();

        if (estaEsquivando)
        {
            EjecutarRoll();
            return;
        }

        var input = playerController.Input;
        if (input.Player.Dodge.WasPressedThisFrame() && energia >= costoEnergia)
            IniciarRoll();
    }

    private void IniciarRoll()
    {
        estaEsquivando = true;
        timerRoll = duracionRoll;
        gradosRotados = 0f;
        energia -= costoEnergia;
        OnEnergiaChanged?.Invoke(energia / energiaMax);

        // Eje de rotación según dirección del input y modo cámara
        Vector2 moveInput = playerController.Input.Player.Move.ReadValue<Vector2>();

        if (PlayerController.EsModoTopDown)
            // Roll hacia el costado en top-down → rota en Z
            ejeRoll = moveInput.x >= 0 ? Vector3.forward : -Vector3.forward;
        else
            // Roll hacia arriba/abajo en side → rota en X
            ejeRoll = moveInput.y >= 0 ? Vector3.right : -Vector3.right;
    }

    private void EjecutarRoll()
    {
        float rotacionFrame = velocidadRoll * Time.deltaTime;
        transform.Rotate(ejeRoll, rotacionFrame, Space.Self);
        gradosRotados += rotacionFrame;

        timerRoll -= Time.deltaTime;
        if (timerRoll <= 0f)
        {
            estaEsquivando = false;
            // Resetear rotación suavemente (el PlayerController la retoma)
        }
    }

    private void RecargarEnergia()
    {
        if (energia >= energiaMax) return;
        energia = Mathf.Min(energiaMax, energia + recargaEnergia * Time.deltaTime);
        OnEnergiaChanged?.Invoke(energia / energiaMax);
    }

    // Devuelve si está esquivando (para invulnerabilidad si querés agregarla)
    public bool EstaEsquivando => estaEsquivando;
}