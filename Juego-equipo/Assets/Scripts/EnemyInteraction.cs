using UnityEngine;

public class EnemyInteraction : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] patrolPoints; // Array para m�ltiples puntos de patrulla
    public float speed = 0.5f;         // Velocidad del enemigo
    private int currentPointIndex = 0; // �ndice del punto de patrulla actual
    public SpriteRenderer spr;

    private bool isDead = false;

    [Header("Destroy Settings")]
    public GameObject coinPrefab;    // Prefab de moneda
    public Transform coinSpawnPoint; // Lugar donde aparece la moneda
    public float destroyHeightOffset = 0.2f; // Altura para detectar el salto

    void Update()
    {
        if (patrolPoints.Length == 0) return; // Aseg�rate de que haya puntos asignados

        // Obtener la posici�n del punto actual
        Vector3 targetPosition = patrolPoints[currentPointIndex].position;

        // Moverse hacia el punto actual
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, speed * Time.deltaTime);


        if (spr != null && patrolPoints.Length > 0)
        {
            // Invertir flipX si el objetivo est� a la izquierda del enemigo
            spr.flipX = patrolPoints[currentPointIndex].position.x < transform.position.x;
        }

        // Cambiar al siguiente punto cuando llegue al actual
        if (Vector3.Distance(transform.position, patrolPoints[currentPointIndex].position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length; // Pasar al siguiente punto c�clicamente
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            vidasPlay vida = collider.gameObject.GetComponent<vidasPlay>();
            Rigidbody2D playerRb = collider.gameObject.GetComponent<Rigidbody2D>();

            float playerY = collider.transform.position.y;  // Posici�n 'y' del jugador
            float enemyY = transform.position.y;            // Posici�n 'y' del enemigo

            Debug.Log($"Posici�n del jugador: {playerY}, Posici�n del enemigo: {enemyY}");

            if (playerY > enemyY + destroyHeightOffset)
            {
                Debug.Log("�Golpe desde arriba! Destruyendo enemigo.");
                KillEnemy();

                // Rebote del jugador
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Ajusta la velocidad del rebote
            }
            else if (!isDead)
            {
                Debug.Log("Jugador golpe� al enemigo pero no desde arriba. Infligiendo da�o.");
                vida?.TomarDa�o(1);
            }
        }
    }

    private void KillEnemy()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("El enemigo ha sido destruido.");

        if (coinPrefab != null && coinSpawnPoint != null)
        {
            Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

