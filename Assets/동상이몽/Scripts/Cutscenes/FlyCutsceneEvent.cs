using UnityEngine;

public class FlyCutsceneController : MonoBehaviour
{
    public void OnFlyCutsceneEnd()
    {
        Camera.main.GetComponent<CameraController>().SetSection(4);
    }
}
