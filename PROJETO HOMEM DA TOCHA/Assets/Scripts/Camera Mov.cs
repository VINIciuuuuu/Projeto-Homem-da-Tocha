using UnityEngine;

public class CameraMov : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("O objeto que a câmera vai seguir (normalmente o jogador).")]
    public Transform target;

    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade de suavização da câmera.")]
    [Range(0.1f, 10f)]
    public float smoothSpeed = 5f;

    [Tooltip("Deslocamento da câmera em relação ao alvo.")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Limites da Câmera (opcional)")]
    [Tooltip("Ativar limites de movimentação da câmera.")]
    public bool useLimits = false;

    public Vector2 minLimits; // limite inferior esquerdo
    public Vector2 maxLimits; // limite superior direito

    private void LateUpdate()
    {
        if (target == null) return;

        // Posição desejada com offset
        Vector3 desiredPosition = target.position + offset;

        // Suavização usando Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Se limites estiverem ativos, aplica clamp
        if (useLimits)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minLimits.x, maxLimits.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minLimits.y, maxLimits.y);
        }

        transform.position = smoothedPosition;
    }
}
