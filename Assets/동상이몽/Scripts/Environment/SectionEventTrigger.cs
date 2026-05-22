using UnityEngine;
using UnityEngine.Video;

public class SectionTrigger : MonoBehaviour
{
    public int sectionIndex;

    [Header("이전 섹션 배경 변경")]
    [Tooltip("이 섹션에 진입할 때 이전 섹션 배경을 '부서진' 버전으로 바꿀 ChapterController. 없으면 EnterTrigger에서 처리")]
    public ChapterController previousChapterToLock;

    [Header("섹션 4 진입 컷신")]
    [Tooltip("섹션 4 진입 시 재생할 영상 (닭이 날아가는 영상 등). 비워두면 건너뜁니다.")]
    public VideoClip section4CutsceneVideo;

    private bool _triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_triggered) return;

        if (Camera.main == null)
        {
            Debug.LogWarning($"SectionTrigger ({gameObject.name}): Camera.main이 null입니다!");
            return;
        }

        CameraController cam = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
        if (cam == null)
            cam = Object.FindAnyObjectByType<CameraController>();
        if (cam == null)
        {
            Debug.LogWarning($"SectionTrigger ({gameObject.name}): CameraController를 찾을 수 없습니다! Main Camera에 CameraController 컴포넌트를 추가하세요.");
            return;
        }

        // 섹션 4 진입 시 컷신 영상 재생
        if (sectionIndex == 4)
        {
            Debug.Log("SectionTrigger: 섹션 4 트리거 진입 (컷신 시작)");
            if (section4CutsceneVideo != null)
            {
                _triggered = true;
                StartCoroutine(PlaySection4CutsceneCoroutine(cam, section4CutsceneVideo));
                return;
            }
            Debug.LogWarning("SectionTrigger: 섹션 4 컷신 영상이 할당되지 않았습니다. Section_04 선택 후 Inspector에서 '닭이 날아가는 영상 (완).mp4'를 지정해주세요.");
        }

        ApplySection(cam);
    }

    System.Collections.IEnumerator PlaySection4CutsceneCoroutine(CameraController cam, VideoClip clip)
    {
        if (Camera.main == null)
        {
            Debug.LogError("SectionTrigger: Camera.main이 null입니다.");
            ApplySection(cam);
            yield break;
        }

        // ====== 1. 이전 챕터 잠금 ======
        if (previousChapterToLock != null)
            previousChapterToLock.LockSection();

        // ====== 2. 게임 일시정지 ======
        float savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // ====== 3. 플레이어 완전 정지 ======
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D playerRb = player != null ? player.GetComponent<Rigidbody2D>() : null;
        Animator playerAnim = player != null ? player.GetComponent<Animator>() : null;
        MonoBehaviour[] playerScripts = player != null ? player.GetComponents<MonoBehaviour>() : null;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.simulated = false;
        }
        if (playerAnim != null)
            playerAnim.speed = 0f;

        // ====== 4. 비디오 플레이어 생성 ======
        GameObject go = new GameObject("Section4CutsceneVideoPlayer");
        VideoPlayer vp = go.AddComponent<VideoPlayer>();
        vp.clip = clip;
        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.targetCamera = Camera.main;
        vp.renderMode = VideoRenderMode.CameraNearPlane;
        vp.targetCameraAlpha = 1f;
        vp.skipOnDrop = true;

        // ====== 5. 비디오 준비 ======
        vp.Prepare();
        float timeout = 5f;
        float t = 0f;
        while (!vp.isPrepared && t < timeout)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // ====== 6. 재생 ======
        vp.Play();
        Debug.Log("SectionTrigger: 섹션 4 컷신 영상 재생 시작");

        // ====== 7. 영상 끝날 때까지 대기 (스킵 가능) ======
        float duration = (float)clip.length;
        if (duration <= 0f) duration = 10f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            // 스페이스 / 클릭 / ESC로 스킵 가능
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
            {
                Debug.Log("SectionTrigger: 컷신 스킵!");
                break;
            }
            yield return null;
        }

        // ====== 8. 정리 ======
        if (go != null)
        {
            vp.Stop();
            Destroy(go);
        }

        // 한 프레임 대기 (Destroy 반영)
        yield return null;

        // ====== 9. 플레이어 복원 ======
        if (playerRb != null)
            playerRb.simulated = true;
        if (playerAnim != null)
            playerAnim.speed = 1f;

        // ====== 10. 게임 재개 ======
        Time.timeScale = savedTimeScale;

        // ====== 11. 섹션 전환 (영상 끝난 후!) ======
        ApplySection(cam);

        Debug.Log("SectionTrigger: 섹션 4 컷신 종료, 섹션 4로 전환 완료");
    }

    void ApplySection(CameraController cam)
    {
        _triggered = true;
        Debug.Log($"SectionTrigger: 섹션 {sectionIndex}로 이동 (트리거: {gameObject.name})");
        float boundaryX = 0f;
        if (sectionIndex >= 1 && sectionIndex <= 3)
        {
            var col = GetComponent<Collider2D>();
            boundaryX = (col != null && col.enabled) ? col.bounds.min.x : transform.position.x;
        }
        cam.SetSection(sectionIndex, boundaryX);

        // 이전 섹션 배경을 '부서진' 버전으로 변경 (EnterTrigger 없을 때 대체)
        if (previousChapterToLock != null)
            previousChapterToLock.LockSection();
    }
}
