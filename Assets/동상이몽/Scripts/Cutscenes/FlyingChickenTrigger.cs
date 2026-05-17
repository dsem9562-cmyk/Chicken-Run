using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 플레이어가 "상하좌우 전환" 오브젝트에 닿으면 닭이 날아가는 영상을 컷신으로 재생합니다.
/// 상하좌우 전환 프리팹에 부착하고, Inspector에서 "닭이 날아가는 영상 (완).mp4"를 할당하세요.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FlyingChickenCutsceneTrigger : MonoBehaviour
{
    [Header("닭이 날아가는 영상")]
    [Tooltip("컷씬 폴더의 닭이 날아가는 영상 (완).mp4를 여기에 드래그하세요")]
    public VideoClip flyingChickenVideo;

    [Header("비디오 설정")]
    [Tooltip("영상 재생 시 게임 일시정지")]
    public bool pauseGameDuringVideo = true;
    [Tooltip("영상 종료 후 게임 재개")]
    public bool resumeGameAfterVideo = true;

    private bool _triggered;
    private VideoPlayer _videoPlayer;
    private static float _savedTimeScale = 1f;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log("FlyingChickenCutsceneTrigger: Collider2D를 Is Trigger로 설정했습니다.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;
        if (flyingChickenVideo == null)
        {
            Debug.LogWarning("FlyingChickenCutsceneTrigger: 닭이 날아가는 영상(flyingChickenVideo)이 할당되지 않았습니다. Inspector에서 컷씬/닭이 날아가는 영상 (완).mp4를 지정하세요.");
            return;
        }

        _triggered = true;
        PlayFlyingChickenVideo();
    }

    void PlayFlyingChickenVideo()
    {
        if (Camera.main == null)
        {
            Debug.LogError("FlyingChickenCutsceneTrigger: Main Camera를 찾을 수 없습니다.");
            return;
        }

        if (pauseGameDuringVideo)
        {
            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        var go = new GameObject("FlyingChickenVideoPlayer");
        _videoPlayer = go.AddComponent<VideoPlayer>();

        _videoPlayer.clip = flyingChickenVideo;
        _videoPlayer.playOnAwake = false;
        _videoPlayer.isLooping = false;
        _videoPlayer.targetCamera = Camera.main;
        _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        _videoPlayer.targetCameraAlpha = 1f;

        _videoPlayer.loopPointReached += OnVideoEnded;
        _videoPlayer.Play();
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoEnded;

        if (resumeGameAfterVideo && pauseGameDuringVideo)
        {
            Time.timeScale = _savedTimeScale;
        }

        if (_videoPlayer != null && _videoPlayer.gameObject != null)
        {
            Destroy(_videoPlayer.gameObject);
        }
    }
}
