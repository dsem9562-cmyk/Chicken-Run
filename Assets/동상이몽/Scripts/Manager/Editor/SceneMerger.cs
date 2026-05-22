using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Chicken Run 씬의 플레이어, 바닥, 아이템을 GameScene으로 복사하는 유틸리티
/// </summary>
public class SceneMerger : EditorWindow
{
    [MenuItem("Tools/씬 통합/Chicken Run → GameScene 복사 (중복 체크)")]
    public static void CopyObjectsToGameScene()
    {
        // GameScene 열기
        string gameScenePath = "Assets/GameScene.unity";
        Scene gameScene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        if (!gameScene.IsValid())
        {
            Debug.LogError("GameScene을 찾을 수 없습니다!");
            return;
        }
        
        EditorSceneManager.SetActiveScene(gameScene);
        
        // 기존 오브젝트 확인
        GameObject existingPlayer = GameObject.Find("플레이어");
        GameObject existingFloor = GameObject.Find("바닥");
        
        if (existingPlayer != null || existingFloor != null)
        {
            bool proceed = EditorUtility.DisplayDialog("중복 오브젝트 발견",
                "GameScene에 이미 다음 오브젝트가 있습니다:\n" +
                (existingPlayer != null ? "- 플레이어\n" : "") +
                (existingFloor != null ? "- 바닥\n" : "") +
                "\n기존 오브젝트를 삭제하고 새로 복사하시겠습니까?",
                "예, 삭제 후 복사", "취소");
            
            if (!proceed) return;
            
            // 기존 오브젝트 삭제
            if (existingPlayer != null) DestroyImmediate(existingPlayer);
            if (existingFloor != null) DestroyImmediate(existingFloor);
        }
        
        // Chicken Run 씬 열기
        string chickenRunScenePath = "Assets/Chicken Run (공동작업_01).unity";
        Scene chickenRunScene = EditorSceneManager.OpenScene(chickenRunScenePath, OpenSceneMode.Additive);
        
        if (!chickenRunScene.IsValid())
        {
            Debug.LogError("Chicken Run 씬을 찾을 수 없습니다!");
            return;
        }
        
        // 활성 씬을 GameScene으로 설정
        EditorSceneManager.SetActiveScene(gameScene);
        
        // 모든 오브젝트 찾기 (플레이어, 바닥, 모든 아이템)
        GameObject player = GameObject.Find("플레이어");
        GameObject floor = GameObject.Find("바닥");
        
        // 모든 아이템 찾기 (이름으로 검색)
        List<GameObject> allItems = new List<GameObject>();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == chickenRunScene && 
                (obj.name.Contains("아이템") || obj.name.Contains("Item")))
            {
                allItems.Add(obj);
            }
        }
        
        // 특정 아이템들도 확인
        GameObject itemWorldTab = GameObject.Find("아이템_세계탭");
        GameObject itemChicken = GameObject.Find("아이템_치킨");
        GameObject itemBuldak = GameObject.Find("아이템_불닭");
        GameObject itemSamgyetang = GameObject.Find("아이템_삼계탕");
        
        if (itemWorldTab != null && !allItems.Contains(itemWorldTab)) allItems.Add(itemWorldTab);
        if (itemChicken != null && !allItems.Contains(itemChicken)) allItems.Add(itemChicken);
        if (itemBuldak != null && !allItems.Contains(itemBuldak)) allItems.Add(itemBuldak);
        if (itemSamgyetang != null && !allItems.Contains(itemSamgyetang)) allItems.Add(itemSamgyetang);
        
        if (player == null && floor == null && allItems.Count == 0)
        {
            Debug.LogWarning("복사할 오브젝트를 찾을 수 없습니다. 오브젝트 이름을 확인해주세요.");
            EditorSceneManager.CloseScene(chickenRunScene, true);
            return;
        }
        
        int copiedCount = 0;
        List<string> copiedNames = new List<string>();
        
        // 플레이어 복사
        if (player != null)
        {
            GameObject copiedPlayer = Instantiate(player);
            copiedPlayer.name = "플레이어";
            // 플레이어 위치를 배경 안쪽으로 조정
            copiedPlayer.transform.position = new Vector3(0, -1.5f, 0);
            copiedCount++;
            copiedNames.Add("플레이어");
            Debug.Log($"플레이어 복사 완료 (위치: {copiedPlayer.transform.position})");
        }
        
        // 바닥 복사
        if (floor != null)
        {
            GameObject copiedFloor = Instantiate(floor);
            copiedFloor.name = "바닥";
            copiedCount++;
            copiedNames.Add("바닥");
            Debug.Log("바닥 복사 완료");
        }
        
        // 모든 아이템 복사
        foreach (GameObject item in allItems)
        {
            if (item != null)
            {
                GameObject copiedItem = Instantiate(item);
                copiedItem.name = item.name; // 원본 이름 유지
                copiedCount++;
                copiedNames.Add(item.name);
                Debug.Log($"{item.name} 복사 완료");
            }
        }
        
        // CameraController 업데이트
        CameraController camController = Camera.main?.GetComponent<CameraController>();
        if (camController != null)
        {
            GameObject newPlayer = GameObject.Find("플레이어");
            if (newPlayer != null)
            {
                camController.player = newPlayer.transform;
                Debug.Log("CameraController가 새 플레이어를 참조하도록 업데이트됨");
            }
            else
            {
                Debug.LogWarning("CameraController를 업데이트할 플레이어를 찾을 수 없습니다!");
            }
        }
        
        // GameStartInitializer 업데이트
        GameStartInitializer initializer = Object.FindFirstObjectByType<GameStartInitializer>();
        if (initializer != null)
        {
            GameObject newPlayer = GameObject.Find("플레이어");
            if (newPlayer != null)
            {
                initializer.player = newPlayer.transform;
                initializer.spawnPositions = new Vector3[] { new Vector3(0, -1.5f, 0) };
                Debug.Log("GameStartInitializer가 새 플레이어를 참조하도록 업데이트됨");
            }
        }
        
        // AutoScrollDeathChecker가 플레이어에 있는지 확인
        if (player != null)
        {
            GameObject newPlayer = GameObject.Find("플레이어");
            if (newPlayer != null && newPlayer.GetComponent<AutoScrollDeathChecker>() == null)
            {
                newPlayer.AddComponent<AutoScrollDeathChecker>();
                Debug.Log("AutoScrollDeathChecker가 플레이어에 추가됨");
            }
        }
        
        // 씬 저장
        EditorSceneManager.SaveScene(gameScene);
        EditorSceneManager.CloseScene(chickenRunScene, true);
        
        // 복사된 오브젝트 목록 생성
        string copiedList = "";
        foreach (string name in copiedNames)
        {
            copiedList += $"- {name}\n";
        }
        
        Debug.Log($"총 {copiedCount}개의 오브젝트가 GameScene에 복사되었습니다!");
        EditorUtility.DisplayDialog("씬 통합 완료", 
            $"{copiedCount}개의 오브젝트가 GameScene에 복사되었습니다.\n\n" +
            "복사된 오브젝트:\n" + copiedList +
            "\n✅ 자동 설정 완료:\n" +
            "- CameraController가 플레이어를 참조하도록 업데이트됨\n" +
            "- GameStartInitializer가 플레이어를 참조하도록 업데이트됨\n" +
            "- AutoScrollDeathChecker가 플레이어에 추가됨\n" +
            "\n플레이어 위치를 확인하고 필요시 조정하세요.",
            "확인");
    }
    
    [MenuItem("Tools/씬 통합/GameScene에서 Chicken Run 오브젝트 제거")]
    public static void RemoveChickenRunObjectsFromGameScene()
    {
        string gameScenePath = "Assets/GameScene.unity";
        Scene gameScene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        if (!gameScene.IsValid())
        {
            Debug.LogError("GameScene을 찾을 수 없습니다!");
            return;
        }
        
        EditorSceneManager.SetActiveScene(gameScene);
        
        int removedCount = 0;
        
        // 오브젝트 찾기 및 삭제
        GameObject[] objectsToRemove = {
            GameObject.Find("플레이어"),
            GameObject.Find("바닥"),
            GameObject.Find("아이템_세계탭"),
            GameObject.Find("아이템_치킨"),
            GameObject.Find("아이템_불닭")
        };
        
        foreach (GameObject obj in objectsToRemove)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
                removedCount++;
            }
        }
        
        EditorSceneManager.SaveScene(gameScene);
        
        Debug.Log($"총 {removedCount}개의 오브젝트가 GameScene에서 제거되었습니다!");
        EditorUtility.DisplayDialog("오브젝트 제거 완료", 
            $"{removedCount}개의 오브젝트가 GameScene에서 제거되었습니다.",
            "확인");
    }
    
    [MenuItem("Tools/씬 통합/플레이어 위치 및 충돌 확인")]
    public static void CheckPlayerPositionAndCollisions()
    {
        string gameScenePath = "Assets/GameScene.unity";
        Scene gameScene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        if (!gameScene.IsValid())
        {
            Debug.LogError("GameScene을 찾을 수 없습니다!");
            return;
        }
        
        EditorSceneManager.SetActiveScene(gameScene);
        
        // 모든 플레이어 오브젝트 찾기
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] playersByName = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(go => go.name.Contains("플레이어") || go.name.Contains("Player"))
            .ToArray();
        
        List<GameObject> allPlayerObjects = new List<GameObject>();
        allPlayerObjects.AddRange(allPlayers);
        foreach (var p in playersByName)
        {
            if (!allPlayerObjects.Contains(p))
                allPlayerObjects.Add(p);
        }
        
        string report = "=== 플레이어 충돌 확인 리포트 ===\n\n";
        report += $"발견된 플레이어 오브젝트 수: {allPlayerObjects.Count}\n\n";
        
        if (allPlayerObjects.Count == 0)
        {
            report += "플레이어 오브젝트를 찾을 수 없습니다!\n";
        }
        else if (allPlayerObjects.Count > 1)
        {
            report += "⚠️ 경고: 여러 개의 플레이어가 발견되었습니다!\n";
            report += "이것이 충돌의 원인일 수 있습니다.\n\n";
        }
        
        for (int i = 0; i < allPlayerObjects.Count; i++)
        {
            GameObject player = allPlayerObjects[i];
            report += $"플레이어 {i + 1}:\n";
            report += $"  - 이름: {player.name}\n";
            report += $"  - 위치: {player.transform.position}\n";
            report += $"  - 활성화: {player.activeSelf}\n";
            
            // CameraController 확인
            CameraController camController = Camera.main?.GetComponent<CameraController>();
            if (camController != null && camController.player != null)
            {
                if (camController.player == player.transform)
                {
                    report += $"  - 카메라가 이 플레이어를 추적 중 ✓\n";
                }
                else
                {
                    report += $"  - 카메라는 다른 플레이어를 추적 중\n";
                }
            }
            report += "\n";
        }
        
        // 배경 오브젝트 확인
        GameObject floor = GameObject.Find("바닥");
        if (floor != null)
        {
            report += $"바닥 오브젝트:\n";
            report += $"  - 위치: {floor.transform.position}\n";
            report += $"  - 크기: {floor.transform.localScale}\n\n";
        }
        else
        {
            report += "바닥 오브젝트를 찾을 수 없습니다.\n\n";
        }
        
        // 권장 사항
        report += "=== 권장 사항 ===\n";
        if (allPlayerObjects.Count > 1)
        {
            report += "1. 중복된 플레이어를 제거하세요.\n";
            report += "2. CameraController가 올바른 플레이어를 참조하는지 확인하세요.\n";
        }
        if (allPlayerObjects.Count > 0)
        {
            GameObject firstPlayer = allPlayerObjects[0];
            if (firstPlayer.transform.position.y < -5 || firstPlayer.transform.position.x < -10)
            {
                report += "3. 플레이어의 초기 위치가 배경 범위 밖에 있습니다.\n";
                report += "   플레이어 위치를 (0, 0, 0) 근처로 조정하세요.\n";
            }
        }
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("플레이어 충돌 확인", report, "확인");
        
        // 첫 번째 플레이어 선택
        if (allPlayerObjects.Count > 0)
        {
            Selection.activeGameObject = allPlayerObjects[0];
            SceneView.FrameLastActiveSceneView();
        }
    }
}
