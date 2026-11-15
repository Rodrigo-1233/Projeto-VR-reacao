using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

public static class BuildFallingBarsScene
{
    [MenuItem("ReflexLab/Build FallingBars Scene")]
    public static void GenerateScene()
    {
        Debug.Log("Building FallingBars VR Evaluation Scene...");

        string scenePath = "Assets/_Project/Scenes/FallingBars.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Environment: Estação de testes acolhedora
        GameObject envRoot = new GameObject("Environment");

        // Materiais
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Hospital_Floor.mat");
        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Hospital_Walls.mat");
        Material metalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Hospital_SurgicalSteel.mat");
        Material accentMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Hospital_AccentBlue.mat");

        // Piso
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.parent = envRoot.transform;
        floor.transform.position = new Vector3(0, -0.1f, 0);
        floor.transform.localScale = new Vector3(6, 0.2f, 6);
        if (floorMat != null) floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        // Paredes de fundo
        GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWall.name = "BackWall";
        backWall.transform.parent = envRoot.transform;
        backWall.transform.position = new Vector3(0, 1.6f, 3.2f);
        backWall.transform.localScale = new Vector3(6, 3.2f, 0.2f);
        if (wallMat != null) backWall.GetComponent<Renderer>().sharedMaterial = wallMat;

        // Iluminação
        GameObject sun = new GameObject("DirectionalLight");
        sun.transform.parent = envRoot.transform;
        Light l = sun.AddComponent<Light>();
        l.type = LightType.Directional;
        l.intensity = 1.0f;
        l.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(45, -25, 0);

        // 2. Coluna/Aparato de Queda da Barra
        GameObject apparatus = new GameObject("Falling_Apparatus");
        apparatus.transform.parent = envRoot.transform;
        apparatus.transform.position = new Vector3(0, 0, 2.2f);

        // Guias verticais metálicas
        GameObject railLeft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        railLeft.name = "Rail_Left";
        railLeft.transform.parent = apparatus.transform;
        railLeft.transform.localPosition = new Vector3(-0.35f, 1.6f, 0);
        railLeft.transform.localScale = new Vector3(0.04f, 1.1f, 0.04f);
        if (metalMat != null) railLeft.GetComponent<Renderer>().sharedMaterial = metalMat;

        GameObject railRight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        railRight.name = "Rail_Right";
        railRight.transform.parent = apparatus.transform;
        railRight.transform.localPosition = new Vector3(0.35f, 1.6f, 0);
        railRight.transform.localScale = new Vector3(0.04f, 1.1f, 0.04f);
        if (metalMat != null) railRight.GetComponent<Renderer>().sharedMaterial = metalMat;

        // Barra que cai
        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bar.name = "Falling_Bar";
        bar.transform.parent = apparatus.transform;
        bar.transform.localPosition = new Vector3(0, 2.3f, 0);
        bar.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        bar.transform.localScale = new Vector3(0.06f, 0.35f, 0.06f);
        Renderer barRenderer = bar.GetComponent<Renderer>();
        if (accentMat != null) barRenderer.sharedMaterial = accentMat;

        // Zona de captura (Catch Target Line)
        GameObject catchZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        catchZone.name = "Catch_Zone_Marker";
        catchZone.transform.parent = apparatus.transform;
        catchZone.transform.localPosition = new Vector3(0, 1.1f, 0);
        catchZone.transform.localScale = new Vector3(0.8f, 0.02f, 0.04f);
        Material zoneMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Mat_Button_FallingBars.mat");
        if (zoneMat != null) catchZone.GetComponent<Renderer>().sharedMaterial = zoneMat;

        // 3. Painel de UI / Feedback do Paciente
        GameObject uiPanel = new GameObject("UI_Feedback_Board");
        uiPanel.transform.parent = apparatus.transform;
        uiPanel.transform.localPosition = new Vector3(0, 1.7f, 0.45f);

        GameObject statusObj = new GameObject("Text_Status");
        statusObj.transform.parent = uiPanel.transform;
        statusObj.transform.localPosition = new Vector3(0, 0.4f, 0);
        TextMeshPro tmpStatus = statusObj.AddComponent<TextMeshPro>();
        tmpStatus.text = "Barras Caindo";
        tmpStatus.fontSize = 2.8f;
        tmpStatus.alignment = TextAlignmentOptions.Center;
        tmpStatus.color = Color.white;
        tmpStatus.GetComponent<RectTransform>().sizeDelta = new Vector2(2f, 0.4f);

        GameObject counterObj = new GameObject("Text_TrialCounter");
        counterObj.transform.parent = uiPanel.transform;
        counterObj.transform.localPosition = new Vector3(0, 0.15f, 0);
        TextMeshPro tmpCounter = counterObj.AddComponent<TextMeshPro>();
        tmpCounter.text = "Tentativa: 0/5";
        tmpCounter.fontSize = 1.8f;
        tmpCounter.alignment = TextAlignmentOptions.Center;
        tmpCounter.color = new Color(0.8f, 0.9f, 1f);
        tmpCounter.GetComponent<RectTransform>().sizeDelta = new Vector2(2f, 0.3f);

        GameObject resultObj = new GameObject("Text_ReactionResult");
        resultObj.transform.parent = uiPanel.transform;
        resultObj.transform.localPosition = new Vector3(0, -0.1f, 0);
        TextMeshPro tmpResult = resultObj.AddComponent<TextMeshPro>();
        tmpResult.text = "";
        tmpResult.fontSize = 2.2f;
        tmpResult.alignment = TextAlignmentOptions.Center;
        tmpResult.color = new Color(0.4f, 1f, 0.6f);
        tmpResult.GetComponent<RectTransform>().sizeDelta = new Vector2(2.5f, 0.4f);

        GameObject instrObj = new GameObject("Text_Instruction");
        instrObj.transform.parent = uiPanel.transform;
        instrObj.transform.localPosition = new Vector3(0, -0.4f, 0);
        TextMeshPro tmpInstr = instrObj.AddComponent<TextMeshPro>();
        tmpInstr.text = "Pressione [Espaço] ou o gatilho do controle para começar.";
        tmpInstr.fontSize = 1.6f;
        tmpInstr.alignment = TextAlignmentOptions.Center;
        tmpInstr.color = new Color(0.9f, 0.95f, 1f);
        tmpInstr.GetComponent<RectTransform>().sizeDelta = new Vector2(2.5f, 0.5f);

        // 4. Controller script
        var controller = apparatus.AddComponent<ReflexLab.Activities.FallingBars.FallingBarsController>();
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("barTransform").objectReferenceValue = bar.transform;
        so.FindProperty("barRenderer").objectReferenceValue = barRenderer;
        so.FindProperty("barSpawnPosition").vector3Value = new Vector3(0, 2.3f, 2.2f);
        so.FindProperty("barCatchTargetPosition").vector3Value = new Vector3(0, 1.1f, 2.2f);
        so.FindProperty("statusText").objectReferenceValue = tmpStatus;
        so.FindProperty("trialCounterText").objectReferenceValue = tmpCounter;
        so.FindProperty("reactionTimeResultText").objectReferenceValue = tmpResult;
        so.FindProperty("instructionText").objectReferenceValue = tmpInstr;
        so.ApplyModifiedProperties();

        // 5. XR Origin e Simulador
        GameObject xrRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/StarterAssets/Prefabs/XR Origin (XR Rig).prefab");
        GameObject xrSimulatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/XRDeviceSimulator/XR Device Simulator.prefab");

        if (xrRigPrefab != null)
        {
            GameObject rig = (GameObject)PrefabUtility.InstantiatePrefab(xrRigPrefab);
            rig.name = "XR Origin (VR Rig)";
            rig.transform.position = Vector3.zero;
        }

        if (xrSimulatorPrefab != null)
        {
            GameObject sim = (GameObject)PrefabUtility.InstantiatePrefab(xrSimulatorPrefab);
            sim.name = "XR Device Simulator";
        }

        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("FallingBars scene built successfully!");
    }
}
