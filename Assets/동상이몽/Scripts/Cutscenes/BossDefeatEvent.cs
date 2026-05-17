using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// UFO 보스에 부착합니다. 보스가 파괴(Destroy)될 때 엔딩 컷신을 재생합니다.
/// Inspector에서 엔딩(일반).mp4를 할당하세요.
/// </summary>
public class BossDeathEndingTrigger : MonoBehaviour
{
    [Header("엔딩 영상")]
    [Tooltip("컷씬 폴더의 엔딩(일반).mp4를 여기에 드래그하세요")]
    public VideoClip endingVideo;

    [Header("비디오 설정")]
    public bool pauseGameDuringVideo = true;
    public bool resumeGameAfterVideo = true;

    void OnDestroy()
    {
        if (endingVideo == null)
        {
            Debug.LogWarning("BossDeathEndingTrigger: 엔딩 영상(endingVideo)이 할당되지 않았습니다. Inspector에서 컷씬/엔딩(일반).mp4를 지정하세요.");
            return;
        }

        EndingCutscenePlayer.Play(endingVideo, pauseGameDuringVideo, resumeGameAfterVideo);
    }
}
