using UnityEngine;

/// <summary>
/// 챕터 1, 2, 3에서 플레이어가 이전 챕터로 이동하지 못하게 X 경계를 적용합니다.
/// 카메라는 이전 챕터를 볼 수 있지만, 플레이어는 경계를 넘지 못합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSectionBoundary : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Camera.main == null) return;

        var camController = Camera.main.GetComponent<CameraController>();
        if (camController == null) return;

        float minX = camController.GetSectionMinX();
        if (float.IsNegativeInfinity(minX)) return;

        var rb = GetComponent<Rigidbody2D>();
        var pos = rb.position;
        if (pos.x < minX)
        {
            pos.x = minX;
            rb.position = pos;
            rb.linearVelocity = new Vector2(Mathf.Max(0, rb.linearVelocity.x), rb.linearVelocity.y);
        }
    }
}
