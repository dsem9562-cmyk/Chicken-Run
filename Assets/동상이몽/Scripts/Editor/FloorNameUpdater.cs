using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// 바닥 이름을 챕터별로 변경하고 플레이어가 인식하도록 설정하는 유틸리티
/// </summary>
public class FloorNameUpdater : EditorWindow
{
    [MenuItem("Tools/바닥 설정/바닥 이름을 챕터별로 변경")]
    public static void UpdateFloorNames()
    {
        // 현재 열려있는 씬들 확인
        Scene activeScene = EditorSceneManager.GetActiveScene();
        
        if (!activeScene.IsValid())
        {
            Debug.LogError("활성 씬이 없습니다!");
            return;
        }
        
        // 모든 바닥 오브젝트 찾기
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        System.Collections.Generic.List<GameObject> floors = new System.Collections.Generic.List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene)
            {
                // Ground 태그가 있거나 바닥 이름을 가진 오브젝트 찾기
                if (obj.CompareTag("Ground") || 
                    obj.name == "바닥" || 
                    obj.name.Contains("바닥"))
                {
                    floors.Add(obj);
                }
            }
        }
        
        if (floors.Count == 0)
        {
            EditorUtility.DisplayDialog("바닥 없음", 
                "씬에서 바닥 오브젝트를 찾을 수 없습니다.\n" +
                "바닥 오브젝트에 'Ground' 태그가 설정되어 있는지 확인하세요.",
                "확인");
            return;
        }
        
        // 사용자에게 챕터 번호 입력 받기
        int chapterNumber = EditorUtility.DisplayDialogComplex("챕터 번호 선택",
            $"총 {floors.Count}개의 바닥을 찾았습니다.\n\n" +
            "모든 바닥을 같은 챕터 번호로 설정하시겠습니까?\n" +
            "(예: 모두 '바닥(챕터1)'로 설정)",
            "챕터1", "챕터2", "챕터3");
        
        if (chapterNumber < 0) return; // 취소
        
        int selectedChapter = chapterNumber + 1;
        int renamedCount = 0;
        
        foreach (GameObject floor in floors)
        {
            // 이미 올바른 이름이면 스킵
            if (floor.name == $"바닥(챕터{selectedChapter})")
                continue;
            
            floor.name = $"바닥(챕터{selectedChapter})";
            
            // Ground 태그 설정 (없으면)
            if (!floor.CompareTag("Ground"))
            {
                floor.tag = "Ground";
            }
            
            renamedCount++;
            Debug.Log($"바닥 이름 변경: {floor.name}");
        }
        
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorUtility.DisplayDialog("바닥 이름 변경 완료", 
            $"{renamedCount}개의 바닥 이름이 '바닥(챕터{selectedChapter})'로 변경되었습니다.\n\n" +
            "모든 바닥에 'Ground' 태그가 설정되어 있어\n" +
            "플레이어가 정상적으로 인식할 수 있습니다.",
            "확인");
    }
    
    [MenuItem("Tools/바닥 설정/모든 바닥에 Ground 태그 설정")]
    public static void SetGroundTagToAllFloors()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        
        if (!activeScene.IsValid())
        {
            Debug.LogError("활성 씬이 없습니다!");
            return;
        }
        
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int taggedCount = 0;
        System.Collections.Generic.List<string> floorNames = new System.Collections.Generic.List<string>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene && 
                (obj.name.Contains("바닥") || obj.name.Contains("챕터")))
            {
                if (!obj.CompareTag("Ground"))
                {
                    obj.tag = "Ground";
                    taggedCount++;
                    floorNames.Add(obj.name);
                    Debug.Log($"Ground 태그 설정: {obj.name}");
                }
            }
        }
        
        EditorSceneManager.MarkSceneDirty(activeScene);
        
        string floorList = "";
        foreach (string name in floorNames)
        {
            floorList += $"- {name}\n";
        }
        
        EditorUtility.DisplayDialog("태그 설정 완료", 
            $"{taggedCount}개의 바닥 오브젝트에 Ground 태그가 설정되었습니다.\n\n" +
            (floorNames.Count > 0 ? $"설정된 바닥:\n{floorList}" : ""),
            "확인");
    }
    
    [MenuItem("Tools/바닥 설정/씬의 모든 바닥 확인")]
    public static void CheckAllFloors()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        
        if (!activeScene.IsValid())
        {
            Debug.LogError("활성 씬이 없습니다!");
            return;
        }
        
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        System.Collections.Generic.List<GameObject> floors = new System.Collections.Generic.List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene && 
                (obj.CompareTag("Ground") || obj.name.Contains("바닥")))
            {
                floors.Add(obj);
            }
        }
        
        string report = $"=== 바닥 오브젝트 확인 ===\n\n";
        report += $"발견된 바닥 수: {floors.Count}\n\n";
        
        if (floors.Count == 0)
        {
            report += "바닥 오브젝트를 찾을 수 없습니다!\n";
        }
        else
        {
            for (int i = 0; i < floors.Count; i++)
            {
                GameObject floor = floors[i];
                report += $"바닥 {i + 1}:\n";
                report += $"  - 이름: {floor.name}\n";
                report += $"  - 태그: {floor.tag}\n";
                report += $"  - 위치: {floor.transform.position}\n";
                
                if (!floor.CompareTag("Ground"))
                {
                    report += $"  ⚠️ Ground 태그가 없습니다!\n";
                }
                report += "\n";
            }
        }
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("바닥 확인", report, "확인");
        
        // 첫 번째 바닥 선택
        if (floors.Count > 0)
        {
            Selection.activeGameObject = floors[0];
            SceneView.FrameLastActiveSceneView();
        }
    }
}
