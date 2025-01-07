using UnityEngine;

public class EnemyPatrol2D : MonoBehaviour
{
    public Transform[] patrolPoints; // Array para m�ltiples puntos de patrulla
    public float speed = 0.5f;         // Velocidad del enemigo
    private int currentPointIndex = 0; // �ndice del punto de patrulla actual
    public SpriteRenderer spr;

    
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
}
