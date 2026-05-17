using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Visual Scripting 불안정성 문제 해결 유틸리티
/// Unity 설정과 Visual Scripting 그래프 연결 상태를 확인합니다.
/// </summary>
public class VisualScriptingStabilityFixer : EditorWindow
{
    [MenuItem("Tools/플레이어 설정/Visual Scripting 안정성 확인 및 수정")]
    public static void CheckVisualScriptingStability()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        
        if (!activeScene.IsValid())
        {
            Debug.LogError("활성 씬이 없습니다!");
            EditorUtility.DisplayDialog("오류", "활성 씬이 없습니다!", "확인");
            return;
        }
        
        // 모든 오브젝트 찾기
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        System.Collections.Generic.List<GameObject> players = new System.Collections.Generic.List<GameObject>();
        System.Collections.Generic.List<GameObject> floors = new System.Collections.Generic.List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene)
            {
                // 플레이어 오브젝트 찾기
                if (obj.CompareTag("Player") || obj.name.Contains("플레이어"))
                {
                    players.Add(obj);
                }
                
                // 바닥 오브젝트 찾기
                if (obj.CompareTag("Ground") || obj.name.Contains("바닥"))
                {
                    floors.Add(obj);
                }
            }
        }
        
        string report = "=== Visual Scripting 안정성 진단 ===\n\n";
        
        // Unity 설정 확인
        report += "⚠️ Unity 설정 경고:\n";
        report += "Unity Preferences > General > Script Changes While Playing\n";
        report += "이 설정이 'Recompile And Continue Playing'로 되어 있으면\n";
        report += "Visual Scripting 플러그인에 불안정성을 일으킬 수 있습니다.\n\n";
        report += "권장 설정: 'Stop Playing and Recompile'\n\n";
        
        // 플레이어 확인
        report += $"플레이어 오브젝트: {players.Count}개\n";
        if (players.Count == 0)
        {
            report += "⚠️ 플레이어 오브젝트를 찾을 수 없습니다!\n\n";
        }
        else
        {
            foreach (var player in players)
            {
                report += $"  - {player.name} (태그: {player.tag})\n";
                
                // Visual Scripting 컴포넌트 확인
                var scriptMachine = player.GetComponent<Unity.VisualScripting.ScriptMachine>();
                var stateGraph = player.GetComponent<Unity.VisualScripting.StateGraph>();
                
                if (scriptMachine == null && stateGraph == null)
                {
                    report += "    ⚠️ Visual Scripting 컴포넌트가 없습니다!\n";
                }
                else
                {
                    if (scriptMachine != null)
                    {
                        report += $"    ✓ ScriptMachine 컴포넌트 발견\n";
                        if (scriptMachine.graph != null)
                        {
                            report += "      그래프 연결됨\n";
                        }
                        else
                        {
                            report += $"      ⚠️ 그래프가 연결되지 않았습니다!\n";
                        }
                    }
                    if (stateGraph != null)
                    {
                        report += $"    ✓ StateGraph 컴포넌트 발견\n";
                    }
                }
                
                // Collider2D 확인
                var collider = player.GetComponent<Collider2D>();
                if (collider == null)
                {
                    report += "    ⚠️ Collider2D가 없습니다!\n";
                }
                else
                {
                    report += $"    ✓ Collider2D 발견 (활성화: {collider.enabled})\n";
                    if (!collider.enabled)
                    {
                        report += "      ⚠️ Collider2D가 비활성화되어 있습니다!\n";
                    }
                }
                
                // Rigidbody2D 확인
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    report += "    ⚠️ Rigidbody2D가 없습니다!\n";
                }
                else
                {
                    report += $"    ✓ Rigidbody2D 발견\n";
                }
                
                report += "\n";
            }
        }
        
        // 바닥 확인
        report += $"바닥 오브젝트: {floors.Count}개\n";
        if (floors.Count == 0)
        {
            report += "⚠️ 바닥 오브젝트를 찾을 수 없습니다!\n\n";
        }
        else
        {
            int groundTaggedCount = 0;
            int missingTagCount = 0;
            int disabledColliderCount = 0;
            
            foreach (var floor in floors)
            {
                if (floor.CompareTag("Ground"))
                {
                    groundTaggedCount++;
                }
                else
                {
                    missingTagCount++;
                }
                
                var collider = floor.GetComponent<Collider2D>();
                if (collider != null && !collider.enabled)
                {
                    disabledColliderCount++;
                    report += $"  ⚠️ {floor.name} - Collider2D 비활성화\n";
                }
            }
            
            report += $"\nGround 태그 설정됨: {groundTaggedCount}개\n";
            report += $"Ground 태그 없음: {missingTagCount}개\n";
            report += $"Collider2D 비활성화: {disabledColliderCount}개\n\n";
            
            if (missingTagCount > 0 || disabledColliderCount > 0)
            {
                bool fix = EditorUtility.DisplayDialog("바닥 설정 문제 발견",
                    $"바닥 오브젝트에 문제가 있습니다:\n\n" +
                    $"- Ground 태그 없음: {missingTagCount}개\n" +
                    $"- Collider2D 비활성화: {disabledColliderCount}개\n\n" +
                    "자동으로 수정하시겠습니까?",
                    "수정", "취소");
                
                if (fix)
                {
                    int fixedTagCount = 0;
                    int fixedColliderCount = 0;
                    
                    foreach (var floor in floors)
                    {
                        if (!floor.CompareTag("Ground"))
                        {
                            floor.tag = "Ground";
                            fixedTagCount++;
                        }
                        
                        var collider = floor.GetComponent<Collider2D>();
                        if (collider != null && !collider.enabled)
                        {
                            collider.enabled = true;
                            fixedColliderCount++;
                        }
                    }
                    
                    EditorSceneManager.MarkSceneDirty(activeScene);
                    
                    EditorUtility.DisplayDialog("수정 완료",
                        $"수정된 항목:\n" +
                        $"- Ground 태그 설정: {fixedTagCount}개\n" +
                        $"- Collider2D 활성화: {fixedColliderCount}개\n\n" +
                        "이제 플레이어가 바닥을 인식할 수 있습니다!",
                        "확인");
                    
                    return;
                }
            }
        }
        
        // 해결 방법 안내
        report += "=== 해결 방법 ===\n\n";
        report += "1. Unity 설정 변경:\n";
        report += "   Edit > Preferences > General\n";
        report += "   Script Changes While Playing: 'Stop Playing and Recompile' 선택\n\n";
        report += "2. Visual Scripting 그래프 확인:\n";
        report += "   플레이어 오브젝트 선택 > Inspector > Script Machine\n";
        report += "   '플레이어 움직임' 그래프가 연결되어 있는지 확인\n";
        report += "   'Edit Graph' 버튼으로 그래프 열기\n\n";
        report += "3. 바닥 인식 확인:\n";
        report += "   Visual Scripting 그래프에서 OnCollisionStay2D 이벤트 확인\n";
        report += "   CompareTag('Ground') 노드가 제대로 작동하는지 확인\n\n";
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("진단 완료", report, "확인");
    }
}
