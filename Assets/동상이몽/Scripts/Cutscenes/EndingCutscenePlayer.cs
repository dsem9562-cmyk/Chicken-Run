using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 엔딩 컷신 영상을 재생하는 유틸리티.
/// 보스 사망 시 등에서 호출합니다.
/// </summary>
public static class EndingCutscenePlayer
{
    private static float _savedTimeScale = 1f;

    /// <summary>
    /// 엔딩 영상을 재생합니다. 게임을 일시정지하고 영상 종료 후 재개합니다.
    /// </summary>
    public static void Play(VideoClip clip, bool pauseGame = true, bool resumeAfter = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("EndingCutscenePlayer: VideoClip이 null입니다.");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("EndingCutscenePlayer: Main Camera를 찾을 수 없습니다.");
            return;
        }

        if (pauseGame)
        {
            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        var go = new GameObject("EndingVideoPlayer");
        var vp = go.AddComponent<VideoPlayer>();
        vp.clip = clip;
        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.targetCamera = Camera.main;
        vp.renderMode = VideoRenderMode.CameraNearPlane;
        vp.targetCameraAlpha = 1f;

        void OnVideoEnded(VideoPlayer v)
        {
            v.loopPointReached -= OnVideoEnded;
            if (resumeAfter && pauseGame)
            {
                Time.timeScale = _savedTimeScale;
            }
            if (v != null && v.gameObject != null)
            {
                Object.Destroy(v.gameObject);
            }
            Application.Quit();
        }
        vp.loopPointReached += OnVideoEnded;

        vp.Play();
        Debug.Log("EndingCutscenePlayer: 엔딩 영상 재생 시작");
    }
}
