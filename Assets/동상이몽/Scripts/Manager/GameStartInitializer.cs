using UnityEngine;

/// <summary>
/// 게임 시작 시 플레이어 위치 초기화 및 카메라 설정.
/// Awake에서 위치 설정 → 프롤로그 이미지가 나오기 전에 플레이어가 올바른 위치에 있음.
/// </summary>
[DefaultExecutionOrder(-100)] // PrologueImageController보다 먼저 실행
public class GameStartInitializer : MonoBehaviour
{
    [Header("플레이어 설정")]
    [Tooltip("초기화할 플레이어 오브젝트 (자동으로 찾을 수도 있음)")]
    public Transform player;

    [Tooltip("스폰 후보 위치들 (2개 중 하나 랜덤 선택). 벽/경계에서 떨어진 빈 공간에 배치")]
    public Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-5.5f, 2.16f, 7.736767f),
        new Vector3(-5.5f, 0.53f, 7.736767f)
    };

    [Header("카메라 설정")]
    [Tooltip("시작 섹션 번호 (1-3 = 플레이어 추적, 4-7 = 자동 스크롤)")]
    public int startSection = 1;

    void Awake()
    {
        // 플레이어가 설정되지 않았으면 자동으로 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                playerObj = GameObject.Find("플레이어");
            }
            // "플레이어 움직임"의 자식인 플레이어도 찾기
            if (playerObj == null)
            {
                var parent = GameObject.Find("플레이어 움직임");
                if (parent != null && parent.transform.childCount > 0)
                {
                    playerObj = parent.transform.GetChild(0).gameObject;
                }
            }

            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"GameStartInitializer: 플레이어를 자동으로 찾았습니다 - {playerObj.name}");
                // 아이템 먹었을 때 스킨 바꾸기: 프리팹에 없어도 런타임에 추가
                if (playerObj.GetComponent<PlayerSkinController>() == null)
                {
                    playerObj.AddComponent<PlayerSkinController>();
                    Debug.Log("GameStartInitializer: 플레이어에 PlayerSkinController를 추가했습니다.");
                }
                if (playerObj.GetComponent<PlayerSectionBoundary>() == null)
                {
                    playerObj.AddComponent<PlayerSectionBoundary>();
                    Debug.Log("GameStartInitializer: 플레이어에 PlayerSectionBoundary를 추가했습니다.");
                }
                if (playerObj.GetComponent<AutoScrollDeathChecker>() == null)
                {
                    playerObj.AddComponent<AutoScrollDeathChecker>();
                    Debug.Log("GameStartInitializer: 플레이어에 AutoScrollDeathChecker를 추가했습니다.");
                }
            }
            else
            {
                Debug.LogWarning("GameStartInitializer: 플레이어를 찾을 수 없습니다!");
                return;
            }
        }

        // 플레이어 위치 설정 + 스킨 컨트롤러 보강 (player가 Inspector에서 할당된 경우)
        if (player != null)
        {
            if (player.GetComponent<PlayerSkinController>() == null)
            {
                player.gameObject.AddComponent<PlayerSkinController>();
                Debug.Log("GameStartInitializer: 플레이어에 PlayerSkinController를 추가했습니다.");
            }
            if (player.GetComponent<PlayerSectionBoundary>() == null)
            {
                player.gameObject.AddComponent<PlayerSectionBoundary>();
                Debug.Log("GameStartInitializer: 플레이어에 PlayerSectionBoundary를 추가했습니다.");
            }
            if (player.GetComponent<AutoScrollDeathChecker>() == null)
            {
                player.gameObject.AddComponent<AutoScrollDeathChecker>();
                Debug.Log("GameStartInitializer: 플레이어에 AutoScrollDeathChecker를 추가했습니다.");
            }
            Vector3 spawnPos = spawnPositions.Length > 0
                ? spawnPositions[Random.Range(0, spawnPositions.Length)]
                : new Vector3(-5.5f, 2.16f, 7.736767f);
            // 부모가 있으면 월드 좌표로 확실히 설정 (로컬 좌표 간섭 방지)
            player.position = spawnPos;
            // Rigidbody2D가 있으면 rb.position도 동기화 (물리 엔진이 transform.position 덮어쓰는 것 방지)
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = new Vector2(spawnPos.x, spawnPos.y);
                rb.linearVelocity = Vector2.zero; // 떨림 방지: 스폰 시 속도 초기화
            }
            Debug.Log($"GameStartInitializer: 플레이어 위치를 {spawnPos}로 설정했습니다. (현재 월드: {player.position})");
        }

        // 카메라 설정
        CameraController camController = Camera.main?.GetComponent<CameraController>();
        if (camController != null)
        {
            if (player != null && camController.player == null)
            {
                camController.player = player;
                Debug.Log("GameStartInitializer: 카메라에 플레이어를 연결했습니다.");
            }
            camController.SetSection(startSection);
            Debug.Log($"GameStartInitializer: 카메라 섹션을 {startSection}으로 설정했습니다.");
        }
        else
        {
            Debug.LogWarning("GameStartInitializer: CameraController를 찾을 수 없습니다!");
        }

        CreateSection01LeftWall();
    }

    void Start()
    {
        // AudioManager가 충분히 초기화된 시점인 Start에서 배경음악 실행
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(0);
            Debug.Log("GameStartInitializer: 배경음악 0번 재생을 요청했습니다.");
        }
        else
        {
            Debug.LogWarning("GameStartInitializer: AudioManager.Instance를 찾을 수 없습니다!");
        }
    }

    void CreateSection01LeftWall()
    {
        var section01 = GameObject.Find("Section_01");
        if (section01 == null) return;

        const string wallName = "Section01_LeftWall";
        var existing = section01.transform.Find(wallName);
        GameObject wall;
        if (existing != null)
        {
            wall = existing.gameObject;
        }
        else
        {
            wall = new GameObject(wallName);
            wall.transform.SetParent(section01.transform, false);
            wall.transform.localPosition = new Vector3(-6.5f, 0f, 0f);
            var box = wall.AddComponent<BoxCollider2D>();
            box.isTrigger = false;
            box.size = new Vector2(1f, 20f);
            box.offset = Vector2.zero;
        }
        wall.SetActive(true);
        var col = wall.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    /// <summary>
    /// 프롤로그 종료 후 등에서 호출. 플레이어를 스폰 위치로 재설정합니다.
    /// </summary>
    public void ApplySpawnPosition()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) p = GameObject.Find("플레이어");
            if (p == null)
            {
                var parent = GameObject.Find("플레이어 움직임");
                if (parent != null && parent.transform.childCount > 0)
                    p = parent.transform.GetChild(0).gameObject;
            }
            if (p != null) player = p.transform;
        }
        if (player == null) return;

        Vector3 spawnPos = spawnPositions.Length > 0
            ? spawnPositions[Random.Range(0, spawnPositions.Length)]
            : new Vector3(-5.5f, 2.16f, 7.736767f);
        player.position = spawnPos;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = new Vector2(spawnPos.x, spawnPos.y);
            rb.linearVelocity = Vector2.zero;
        }
    }
}