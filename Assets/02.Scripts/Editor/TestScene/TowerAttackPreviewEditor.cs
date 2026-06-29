using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerAttackPreview))]
public class TowerAttackPreviewEditor : Editor
{
    private bool isPlaying;
    private double previousTime;
    private float elapsedTime;

    private TowerAttackPreview Preview => (TowerAttackPreview)target;

    private void OnDisable()
    {
        StopPreview();
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Tower"))
        {
            Preview.ApplyTower();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Play Attack"))
        {
            StartPreview();
        }

        if (GUILayout.Button("Stop"))
        {
            StopPreview();
        }
    }

    private void StartPreview()
    {
        StopPreview();

        Preview.ApplyTower();
        Preview.PlayAttack();

        elapsedTime = 0f;
        previousTime = EditorApplication.timeSinceStartup;
        isPlaying = true;

        EditorApplication.update += UpdatePreview;
    }

    private void UpdatePreview()
    {
        if (!isPlaying || Preview == null)
        {
            StopPreview();
            return;
        }

        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - previousTime);
        previousTime = currentTime;

        Preview.UpdatePreview(deltaTime);
        elapsedTime += deltaTime;

        if (elapsedTime >= 1f / Preview.AttackSpeed)
        {
            StopPreview();
            return;
        }

        EditorApplication.QueuePlayerLoopUpdate();
        SceneView.RepaintAll();
        Repaint();
    }

    private void StopPreview()
    {
        if (isPlaying)
            EditorApplication.update -= UpdatePreview;

        isPlaying = false;
        elapsedTime = 0f;

        if (target is TowerAttackPreview preview)
            preview.StopPreview();

        SceneView.RepaintAll();
    }
}
