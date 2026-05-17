using UnityEngine;

/// <summary>
/// 배경을 카메라와 함께 스크롤하는 스크립트
/// 카메라가 이동할 때 배경도 함께 이동합니다.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    [Header("스크롤 설정")]
    [Tooltip("카메라 이동 속도에 대한 배경 이동 비율 (1.0 = 카메라와 동일한 속도)")]
    public float scrollRatio = 1.0f;
    
    [Tooltip("패럴랙스 효과를 위한 오프셋 (0 = 카메라와 완전히 동기화)")]
    public float parallaxOffset = 0f;
    
    [Header("참조")]
    [Tooltip("카메라 컨트롤러 (자동으로 찾습니다)")]
    private CameraController cameraController;
    
    private Vector3 initialPosition;
    private float lastCameraX;

    void Start()
    {
        // CameraController 찾기
        cameraController = Camera.main?.GetComponent<CameraController>();
        
        if (cameraController == null)
        {
            Debug.LogWarning("BackgroundScroller: CameraController를 찾을 수 없습니다!");
        }
        
        // 초기 위치 저장
        initialPosition = transform.position;
        lastCameraX = Camera.main.transform.position.x;
    }

    void LateUpdate()
    {
        if (cameraController == null || Camera.main == null) return;
        
        float currentCameraX = Camera.main.transform.position.x;
        float cameraDeltaX = currentCameraX - lastCameraX;
        
        // 배경을 카메라 이동량에 비례하여 이동
        if (Mathf.Abs(cameraDeltaX) > 0.001f)
        {
            float backgroundDeltaX = cameraDeltaX * scrollRatio;
            transform.position = new Vector3(
                transform.position.x + backgroundDeltaX,
                initialPosition.y + parallaxOffset,
                initialPosition.z
            );
        }
        
        lastCameraX = currentCameraX;
    }
}
