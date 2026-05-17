using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        FollowPlayer, // S1~S3
        AutoScroll,   // S4~S7
        Cutscene      // S3.5, S8
    }

    [Header("State")]
    public CameraMode mode = CameraMode.FollowPlayer;

    [Header("Player Follow")]
    public Transform player;
    public float offsetX = 0f;

    [Header("Section Settings")]
    public float sectionWidth = 20f;
    public float fixedY = 0f;
    [Tooltip("챕터 1 최소 X (왼쪽 이동 허용). -6 = 시작 구역에서 왼쪽 이동 가능")]
    public float section1MinX = -6f;
    [Tooltip("챕터 2 최소 X. SectionTrigger가 자동 전달, 없으면 이 값")]
    public float section2MinXDefault = 20f;
    [Tooltip("챕터 3 최소 X. SectionTrigger가 자동 전달, 없으면 이 값")]
    public float section3MinXDefault = 50f;

    [Header("Auto Scroll")]
    public float scrollSpeed = 5f;

    private float minX = 0f;
    private int currentSection = 1;
    private float section2MinX;
    private float section3MinX;

    void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) p = GameObject.Find("플레이어");
            if (p != null) player = p.transform;
        }
        section2MinX = section2MinXDefault;
        section3MinX = section3MinXDefault;
    }

    void LateUpdate()
    {
        if (mode == CameraMode.FollowPlayer)
            FollowPlayer();
        else if (mode == CameraMode.AutoScroll)
            AutoScroll();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float targetX = player.position.x + offsetX;

        // 섹션 1~3: 카메라는 자유롭게 따라감 (이전 챕터도 볼 수 있음), 이동 제한은 플레이어 쪽에서
        // 섹션 4+: AutoScroll 모드 (자동 이동, 뒤로 안 감)

        transform.position = new Vector3(
            targetX,
            fixedY,
            transform.position.z
        );
    }

    void AutoScroll()
    {
        minX += scrollSpeed * Time.deltaTime;

        transform.position = new Vector3(
            minX,
            fixedY,
            transform.position.z
        );
    }

    /// <summary>현재 섹션에서 플레이어가 갈 수 있는 최소 X (이전 챕터로 이동 방지용)</summary>
    public float GetSectionMinX()
    {
        if (currentSection == 1) return section1MinX;
        if (currentSection == 2) return section2MinX;
        if (currentSection == 3) return section3MinX;
        // S4~S7 AutoScroll: 카메라 왼쪽 가장자리(흰 벽)가 경계
        if (currentSection >= 4 && mode == CameraMode.AutoScroll)
            return transform.position.x - 9f;
        return float.NegativeInfinity;
    }

    public void SetSection(int section) => SetSection(section, float.NaN);

    public void SetSection(int section, float boundaryMinX)
    {
        // 섹션 4 이상(AutoScroll)에 진입했으면 이전 섹션 트리거로 되돌아가지 않음
        if (currentSection >= 4 && section < 4)
        {
            Debug.Log($"SetSection: 이미 섹션 {currentSection} (AutoScroll) - 섹션 {section}으로 되돌아감 무시");
            return;
        }

        Debug.Log("SetSection: " + section);
        currentSection = section;

        // SectionTrigger에서 경계 전달 (챕터 1, 2, 3)
        // 챕터 1: 내부 트리거가 경계를 더 오른쪽으로 밀어넣지 않게 (Min 사용)
        // 챕터 2, 3: 진입 시 해당 트리거 왼쪽 가장자리가 경계
        if (!float.IsNaN(boundaryMinX))
        {
            if (section == 1)
                section1MinX = Mathf.Min(section1MinX, boundaryMinX); // 더 제한하지 않음
            else if (section == 2)
                section2MinX = boundaryMinX;
            else if (section == 3)
                section3MinX = boundaryMinX;
        }

        // AutoScroll 전환 (S4~S7) - 챕터 4 이상에서만 카메라 자동 이동
        if (section >= 4)
        {
            mode = CameraMode.AutoScroll;
            minX = transform.position.x;
        }
        else
        {
            mode = CameraMode.FollowPlayer;
        }
    }
}
