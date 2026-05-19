// EnemyCleanup.cs
// Elimina enemigos que salen demasiado lejos del área jugable.
// Evita que se acumulen objetos fuera de cámara consumiendo recursos.

using UnityEngine;

public class EnemyCleanup : MonoBehaviour
{
    [SerializeField] private float limiteDistancia = 30f;
    [SerializeField] private float intervaloChequeo = 3f;

    private Transform playerTransform;
    private float timer = 0f;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = intervaloChequeo;
            LimpiarEnemigosLejanos();
        }
    }

    private void LimpiarEnemigosLejanos()
    {
        EnemyBase[] enemigos = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (var enemigo in enemigos)
        {
            float distancia = Vector3.Distance(
                playerTransform.position,
                enemigo.transform.position
            );

            if (distancia > limiteDistancia)
                Destroy(enemigo.gameObject);
        }
    }
}