// WorldScroller.cs
// Mueve el suelo/fondo continuamente según el modo de cámara activo.
// Recicla los tiles cuando salen de pantalla para simular nivel infinito.

using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    [Header("Tiles del nivel")]
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private float velocidadScroll = 5f;
    [SerializeField] private float largoDeTile = 20f;    // largo de cada sección

    private CameraTransitionManager camManager;

    private void Start()
    {
        camManager = FindFirstObjectByType<CameraTransitionManager>();
    }

    private void Update()
    {
        if (GameManager.Instance?.Estado != GameState.Playing) return;

        MoverTiles();
        ReciclarTiles();
    }

    private void MoverTiles()
    {
        Vector3 direccion;

        if (camManager != null && camManager.EsModoTopDown)
            // TopDown: el mundo avanza en Z negativo (el jugador "avanza" en Z)
            direccion = Vector3.back;
        else
            // SideScroll: el mundo avanza en X negativo
            direccion = Vector3.left;

        foreach (var tile in tiles)
        {
            if (tile != null)
                tile.transform.Translate(direccion * velocidadScroll * Time.deltaTime);
        }
    }

    private void ReciclarTiles()
    {
        // Detecta qué tile quedó más atrás y lo mueve al frente
        foreach (var tile in tiles)
        {
            if (tile == null) continue;

            if (camManager != null && camManager.EsModoTopDown)
            {
                // En top-down: reciclar en eje Z
                if (tile.transform.position.z < -largoDeTile)
                {
                    float maxZ = ObtenerMaxPosicion(ejeZ: true);
                    tile.transform.position = new Vector3(
                        tile.transform.position.x,
                        tile.transform.position.y,
                        maxZ + largoDeTile
                    );
                }
            }
            else
            {
                // En side-scroll: reciclar en eje X
                if (tile.transform.position.x < -largoDeTile)
                {
                    float maxX = ObtenerMaxPosicion(ejeZ: false);
                    tile.transform.position = new Vector3(
                        maxX + largoDeTile,
                        tile.transform.position.y,
                        tile.transform.position.z
                    );
                }
            }
        }
    }

    private float ObtenerMaxPosicion(bool ejeZ)
    {
        float max = float.MinValue;
        foreach (var tile in tiles)
        {
            if (tile == null) continue;
            float val = ejeZ ? tile.transform.position.z : tile.transform.position.x;
            if (val > max) max = val;
        }
        return max;
    }
}