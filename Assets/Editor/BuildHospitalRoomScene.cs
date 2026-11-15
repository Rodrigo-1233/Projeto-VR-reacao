using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

public static class BuildHospitalRoomScene
{
    public static void GenerateScene()
    {
        Debug.Log("Building Hospital Room 3D Virtual Reality Scene for Meta Quest...");

        // Ensure materials directory exists
        string matPath = "Assets/_Project/Materials";
        if (!AssetDatabase.IsValidFolder(matPath))
        {
            AssetDatabase.CreateFolder("Assets/_Project", "Materials");
        }

        // 1. Create or load cohesive, professional hospital palette materials
        Material floorMat = GetOrCreateMaterial("Mat_Hospital_Floor", new Color(0.85f, 0.87f, 0.88f), 0.1f, 0.35f);
        Material wallMat = GetOrCreateMaterial("Mat_Hospital_Walls", new Color(0.92f, 0.94f, 0.95f), 0.0f, 0.15f);
        Material ceilingMat = GetOrCreateMaterial("Mat_Hospital_Ceiling", new Color(0.96f, 0.97f, 0.98f), 0.0f, 0.1f);
        Material accentMat = GetOrCreateMaterial("Mat_Hospital_AccentBlue", new Color(0.18f, 0.45f, 0.65f), 0.1f, 0.4f);
        Material woodMat = GetOrCreateMaterial("Mat_Hospital_WoodWarm", new Color(0.72f, 0.53f, 0.38f), 0.0f, 0.3f);
        Material metalMat = GetOrCreateMaterial("Mat_Hospital_SurgicalSteel", new Color(0.82f, 0.84f, 0.86f), 0.8f, 0.75f);
        Material glassMat = GetOrCreateMaterial("Mat_Hospital_FrostedGlass", new Color(0.75f, 0.88f, 0.92f, 0.6f), 0.2f, 0.85f);
        Material lightMat = GetOrCreateMaterial("Mat_Hospital_LightEmission", new Color(0.98f, 0.98f, 0.95f), 0.0f, 0.2f);
        lightMat.EnableKeyword("_EMISSION");
        lightMat.SetColor("_EmissionColor", new Color(0.98f, 0.98f, 0.95f) * 1.5f);

        // Open or Create Scene
        string scenePath = "Assets/_Project/Scenes/HospitalRoom.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 2. Room Hierarchy Parent
        GameObject envRoot = new GameObject("Environment_HospitalRoom");

        // Dimensions of room: 8m wide x 9m long x 3.2m high (spacious, non-claustrophobic for VR)
        float roomW = 8.0f;
        float roomL = 9.0f;
        float roomH = 3.2f;

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.parent = envRoot.transform;
        floor.transform.position = new Vector3(0, -0.1f, 0);
        floor.transform.localScale = new Vector3(roomW, 0.2f, roomL);
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        // Ceiling
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.parent = envRoot.transform;
        ceiling.transform.position = new Vector3(0, roomH + 0.1f, 0);
        ceiling.transform.localScale = new Vector3(roomW, 0.2f, roomL);
        ceiling.GetComponent<Renderer>().sharedMaterial = ceilingMat;

        // Walls
        CreateWall(envRoot.transform, "Wall_North", new Vector3(0, roomH / 2, roomL / 2), new Vector3(roomW, roomH, 0.2f), wallMat);
        CreateWall(envRoot.transform, "Wall_South", new Vector3(0, roomH / 2, -roomL / 2), new Vector3(roomW, roomH, 0.2f), wallMat);
        CreateWall(envRoot.transform, "Wall_East", new Vector3(roomW / 2, roomH / 2, 0), new Vector3(0.2f, roomH, roomL), wallMat);
        CreateWall(envRoot.transform, "Wall_West", new Vector3(-roomW / 2, roomH / 2, 0), new Vector3(0.2f, roomH, roomL), wallMat);

        // Decorative Hospital Wall Baseboards & Accent Strip (gives architectural depth)
        GameObject accentStrip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        accentStrip.name = "Wall_AccentStripe_North";
        accentStrip.transform.parent = envRoot.transform;
        accentStrip.transform.position = new Vector3(0, 1.1f, (roomL / 2) - 0.08f);
        accentStrip.transform.localScale = new Vector3(roomW - 0.4f, 0.15f, 0.05f);
        accentStrip.GetComponent<Renderer>().sharedMaterial = accentMat;

        // 3. Furniture and Hospital Equipment
        GameObject furnRoot = new GameObject("Furniture_HospitalEquipment");
        furnRoot.transform.parent = envRoot.transform;

        // Patient Comfortable Ergonomic Armchair (Positioned at 0, 0, 0 where patient sits)
        CreatePatientChair(furnRoot.transform, new Vector3(0, 0, -0.4f), accentMat, metalMat);

        // Doctor/Therapist Consultation Desk on the side (West wall)
        CreateDesk(furnRoot.transform, new Vector3(-3.0f, 0, 1.5f), woodMat, metalMat);

        // Hospital Storage Cabinet / Credenza (East wall)
        CreateStorageCabinet(furnRoot.transform, new Vector3(3.2f, 0, 0), wallMat, metalMat);

        // Medical Monitor Stand on Wheels
        CreateMedicalMonitor(furnRoot.transform, new Vector3(-2.8f, 0, -0.5f), metalMat, accentMat);

        // IV Drip Pole (discreet corner hospital element)
        CreateIVPole(furnRoot.transform, new Vector3(2.5f, 0, -2.5f), metalMat);

        // Indoor Plant for soothing calming environment (therapeutic setting)
        CreateCalmingPlant(furnRoot.transform, new Vector3(3.2f, 0, 3.5f), woodMat);

        // 4. Lighting (Soft, diffuse, non-glare for VR visual comfort)
        GameObject lightRoot = new GameObject("Lighting_Setup");
        lightRoot.transform.parent = envRoot.transform;

        // Main soft directional ceiling bounce light
        GameObject sun = new GameObject("Directional_SoftLight");
        sun.transform.parent = lightRoot.transform;
        Light dirLight = sun.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(1.0f, 0.98f, 0.95f);
        dirLight.intensity = 0.9f;
        dirLight.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Ceiling Recessed Light Panels
        CreateCeilingLightPanel(lightRoot.transform, new Vector3(-1.8f, roomH - 0.02f, 1.5f), lightMat);
        CreateCeilingLightPanel(lightRoot.transform, new Vector3(1.8f, roomH - 0.02f, 1.5f), lightMat);
        CreateCeilingLightPanel(lightRoot.transform, new Vector3(-1.8f, roomH - 0.02f, -1.5f), lightMat);
        CreateCeilingLightPanel(lightRoot.transform, new Vector3(1.8f, roomH - 0.02f, -1.5f), lightMat);

        // 5. Activity Portals (Interactive Selection Hub for the 3 Evaluations)
        // Positioned in an ergonomic semicircle 2.5m in front of the patient
        GameObject portalsRoot = new GameObject("Activity_Portals");

        CreatePortal(portalsRoot.transform, 
            new Vector3(-1.8f, 1.3f, 2.6f), 
            Quaternion.Euler(0, 25f, 0),
            ReflexLab.Core.ActivityType.FallingBars,
            "FallingBars",
            "1. Barras Caindo",
            "Avaliação de tempo de reação visual.\nPressione o controle assim que a barra iniciar o movimento de queda.",
            "Duração estimada: 2 a 3 min\nFoco: Tempo de Reação Simples",
            accentMat, metalMat);

        CreatePortal(portalsRoot.transform, 
            new Vector3(0f, 1.35f, 2.85f), 
            Quaternion.Euler(0, 0f, 0),
            ReflexLab.Core.ActivityType.TargetShooting,
            "TargetShooting",
            "2. Tiro ao Alvo",
            "Avaliação de coordenação e precisão motora.\nUse o feixe do controle para apontar e selecionar os alvos circulares.",
            "Duração estimada: 3 min\nFoco: Precisão Motora Fina",
            accentMat, metalMat);

        CreatePortal(portalsRoot.transform, 
            new Vector3(1.8f, 1.3f, 2.6f), 
            Quaternion.Euler(0, -25f, 0),
            ReflexLab.Core.ActivityType.ObjectSelection,
            "ObjectSelection",
            "3. Seleção de Objetos",
            "Avaliação de reconhecimento e velocidade cognitiva.\nIdentifique e aponte para o objeto instruído na bancada.",
            "Duração estimada: 3 a 4 min\nFoco: Reconhecimento e Decisão",
            accentMat, metalMat);

        // Welcome / Instruction Banner above the portals
        CreateWelcomeBanner(envRoot.transform, new Vector3(0, 2.35f, 3.4f), "ReflexLab — Centro de Reabilitação Virtual");

        // 6. XR Origin & VR Rig Setup
        GameObject xrRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/StarterAssets/Prefabs/XR Origin (XR Rig).prefab");
        GameObject xrSimulatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/XRDeviceSimulator/XR Device Simulator.prefab");

        if (xrRigPrefab != null)
        {
            GameObject rigInstance = (GameObject)PrefabUtility.InstantiatePrefab(xrRigPrefab);
            rigInstance.name = "XR Origin (VR Rig)";
            rigInstance.transform.position = Vector3.zero;
            rigInstance.transform.rotation = Quaternion.identity;
            Debug.Log("Instantiated XR Origin prefab successfully.");
        }
        else
        {
            // Fallback camera if prefab isn't present
            GameObject cam = new GameObject("Main Camera");
            Camera c = cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0, 1.35f, 0); // Comfortable seated height
            cam.AddComponent<AudioListener>();
        }

        // Add XR Device Simulator for in-editor VR testing without physical headset
        if (xrSimulatorPrefab != null)
        {
            GameObject simInstance = (GameObject)PrefabUtility.InstantiatePrefab(xrSimulatorPrefab);
            simInstance.name = "XR Device Simulator";
            Debug.Log("Instantiated XR Device Simulator for in-editor PC testing.");
        }

        // Save Scene
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("HospitalRoom VR Scene generated and saved successfully!");
    }

    private static Material GetOrCreateMaterial(string name, Color color, float metallic = 0f, float smoothness = 0.5f)
    {
        string path = $"Assets/_Project/Materials/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader uLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (uLitShader == null) uLitShader = Shader.Find("Standard");
            mat = new Material(uLitShader);
            mat.color = color;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    private static void CreateWall(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = parent;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void CreatePatientChair(Transform parent, Vector3 pos, Material seatMat, Material frameMat)
    {
        GameObject chair = new GameObject("Patient_Ergonomic_Armchair");
        chair.transform.parent = parent;
        chair.transform.position = pos;

        // Seat
        GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seat.name = "Seat_Cushion";
        seat.transform.parent = chair.transform;
        seat.transform.localPosition = new Vector3(0, 0.45f, 0);
        seat.transform.localScale = new Vector3(0.65f, 0.12f, 0.6f);
        seat.GetComponent<Renderer>().sharedMaterial = seatMat;

        // Backrest
        GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.name = "Backrest";
        back.transform.parent = chair.transform;
        back.transform.localPosition = new Vector3(0, 0.85f, -0.27f);
        back.transform.localScale = new Vector3(0.62f, 0.7f, 0.1f);
        back.GetComponent<Renderer>().sharedMaterial = seatMat;

        // Armrests
        CreateArmrest(chair.transform, new Vector3(-0.35f, 0.65f, 0), seatMat, frameMat);
        CreateArmrest(chair.transform, new Vector3(0.35f, 0.65f, 0), seatMat, frameMat);

        // Metal base
        GameObject basePole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        basePole.name = "Base_Support";
        basePole.transform.parent = chair.transform;
        basePole.transform.localPosition = new Vector3(0, 0.2f, 0);
        basePole.transform.localScale = new Vector3(0.12f, 0.2f, 0.12f);
        basePole.GetComponent<Renderer>().sharedMaterial = frameMat;
    }

    private static void CreateArmrest(Transform parent, Vector3 pos, Material cushionMat, Material frameMat)
    {
        GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        arm.name = "Armrest";
        arm.transform.parent = parent;
        arm.transform.localPosition = pos;
        arm.transform.localScale = new Vector3(0.08f, 0.06f, 0.45f);
        arm.GetComponent<Renderer>().sharedMaterial = cushionMat;
    }

    private static void CreateDesk(Transform parent, Vector3 pos, Material topMat, Material legMat)
    {
        GameObject desk = new GameObject("Consultation_Desk");
        desk.transform.parent = parent;
        desk.transform.position = pos;

        // Top
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "Desk_Top";
        top.transform.parent = desk.transform;
        top.transform.localPosition = new Vector3(0, 0.75f, 0);
        top.transform.localScale = new Vector3(1.6f, 0.06f, 0.85f);
        top.GetComponent<Renderer>().sharedMaterial = topMat;

        // Legs
        CreateLeg(desk.transform, new Vector3(-0.7f, 0.36f, -0.35f), legMat);
        CreateLeg(desk.transform, new Vector3(0.7f, 0.36f, -0.35f), legMat);
        CreateLeg(desk.transform, new Vector3(-0.7f, 0.36f, 0.35f), legMat);
        CreateLeg(desk.transform, new Vector3(0.7f, 0.36f, 0.35f), legMat);
    }

    private static void CreateLeg(Transform parent, Vector3 pos, Material mat)
    {
        GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leg.name = "Leg";
        leg.transform.parent = parent;
        leg.transform.localPosition = pos;
        leg.transform.localScale = new Vector3(0.05f, 0.36f, 0.05f);
        leg.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void CreateStorageCabinet(Transform parent, Vector3 pos, Material bodyMat, Material handleMat)
    {
        GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cab.name = "Hospital_Cabinet";
        cab.transform.parent = parent;
        cab.transform.position = pos + new Vector3(0, 1.0f, 0);
        cab.transform.localScale = new Vector3(0.7f, 2.0f, 1.8f);
        cab.GetComponent<Renderer>().sharedMaterial = bodyMat;
    }

    private static void CreateMedicalMonitor(Transform parent, Vector3 pos, Material standMat, Material screenMat)
    {
        GameObject mon = new GameObject("Medical_Monitor_Station");
        mon.transform.parent = parent;
        mon.transform.position = pos;

        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Stand_Pole";
        pole.transform.parent = mon.transform;
        pole.transform.localPosition = new Vector3(0, 0.65f, 0);
        pole.transform.localScale = new Vector3(0.06f, 0.65f, 0.06f);
        pole.GetComponent<Renderer>().sharedMaterial = standMat;

        GameObject display = GameObject.CreatePrimitive(PrimitiveType.Cube);
        display.name = "Display_Unit";
        display.transform.parent = mon.transform;
        display.transform.localPosition = new Vector3(0, 1.35f, 0);
        display.transform.localRotation = Quaternion.Euler(0, 35f, 0);
        display.transform.localScale = new Vector3(0.55f, 0.35f, 0.08f);
        display.GetComponent<Renderer>().sharedMaterial = screenMat;
    }

    private static void CreateIVPole(Transform parent, Vector3 pos, Material mat)
    {
        GameObject iv = new GameObject("IV_Drip_Pole");
        iv.transform.parent = parent;
        iv.transform.position = pos;

        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Pole";
        pole.transform.parent = iv.transform;
        pole.transform.localPosition = new Vector3(0, 0.9f, 0);
        pole.transform.localScale = new Vector3(0.03f, 0.9f, 0.03f);
        pole.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static void CreateCalmingPlant(Transform parent, Vector3 pos, Material potMat)
    {
        GameObject plant = new GameObject("Therapeutic_Plant");
        plant.transform.parent = parent;
        plant.transform.position = pos;

        GameObject pot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pot.name = "Pot";
        pot.transform.parent = plant.transform;
        pot.transform.localPosition = new Vector3(0, 0.25f, 0);
        pot.transform.localScale = new Vector3(0.4f, 0.25f, 0.4f);
        pot.GetComponent<Renderer>().sharedMaterial = potMat;

        GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.name = "Foliage";
        foliage.transform.parent = plant.transform;
        foliage.transform.localPosition = new Vector3(0, 0.7f, 0);
        foliage.transform.localScale = new Vector3(0.55f, 0.65f, 0.55f);
        Material leafMat = GetOrCreateMaterial("Mat_Plant_Leaves", new Color(0.22f, 0.48f, 0.25f), 0.0f, 0.2f);
        foliage.GetComponent<Renderer>().sharedMaterial = leafMat;
    }

    private static void CreateCeilingLightPanel(Transform parent, Vector3 pos, Material lightMat)
    {
        GameObject fixture = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fixture.name = "Ceiling_LightPanel";
        fixture.transform.parent = parent;
        fixture.transform.position = pos;
        fixture.transform.localScale = new Vector3(1.2f, 0.04f, 0.6f);
        fixture.GetComponent<Renderer>().sharedMaterial = lightMat;

        GameObject lightObj = new GameObject("Panel_PointLight");
        lightObj.transform.parent = fixture.transform;
        lightObj.transform.localPosition = new Vector3(0, -0.1f, 0);
        Light pl = lightObj.AddComponent<Light>();
        pl.type = LightType.Point;
        pl.color = new Color(0.96f, 0.97f, 0.98f);
        pl.intensity = 1.2f;
        pl.range = 6.0f;
    }

    private static void CreatePortal(Transform parent, Vector3 pos, Quaternion rot, ReflexLab.Core.ActivityType type, string scene, string title, string desc, string details, Material frameMat, Material metalMat)
    {
        GameObject portal = new GameObject($"Portal_{type}");
        portal.transform.parent = parent;
        portal.transform.position = pos;
        portal.transform.rotation = rot;

        // Pedestal
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestal.name = "Pedestal_Base";
        pedestal.transform.parent = portal.transform;
        pedestal.transform.localPosition = new Vector3(0, -0.65f, 0);
        pedestal.transform.localScale = new Vector3(0.2f, 0.65f, 0.2f);
        pedestal.GetComponent<Renderer>().sharedMaterial = metalMat;

        // Panel Board
        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "Panel_Board";
        board.transform.parent = portal.transform;
        board.transform.localPosition = Vector3.zero;
        board.transform.localScale = new Vector3(1.15f, 0.75f, 0.06f);
        board.GetComponent<Renderer>().sharedMaterial = frameMat;

        // Indicator Light / Button
        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        button.name = "Start_Button";
        button.transform.parent = portal.transform;
        button.transform.localPosition = new Vector3(0, -0.22f, -0.04f);
        button.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        button.transform.localScale = new Vector3(0.22f, 0.03f, 0.22f);
        Material btnMat = GetOrCreateMaterial($"Mat_Button_{type}", new Color(0.18f, 0.54f, 0.74f), 0.2f, 0.6f);
        button.GetComponent<Renderer>().sharedMaterial = btnMat;

        // Add Box Collider & Interactivity
        var portalComp = portal.AddComponent<ReflexLab.UI.ActivityPortal>();

        // Texts using TextMeshPro
        GameObject titleObj = new GameObject("Text_Title");
        titleObj.transform.parent = portal.transform;
        titleObj.transform.localPosition = new Vector3(0, 0.22f, -0.04f);
        TextMeshPro tmpTitle = titleObj.AddComponent<TextMeshPro>();
        tmpTitle.text = title;
        tmpTitle.fontSize = 2.4f;
        tmpTitle.alignment = TextAlignmentOptions.Center;
        tmpTitle.color = Color.white;
        RectTransform rtTitle = tmpTitle.GetComponent<RectTransform>();
        rtTitle.sizeDelta = new Vector2(1.1f, 0.3f);

        GameObject descObj = new GameObject("Text_Description");
        descObj.transform.parent = portal.transform;
        descObj.transform.localPosition = new Vector3(0, 0.02f, -0.04f);
        TextMeshPro tmpDesc = descObj.AddComponent<TextMeshPro>();
        tmpDesc.text = desc;
        tmpDesc.fontSize = 1.35f;
        tmpDesc.alignment = TextAlignmentOptions.Center;
        tmpDesc.color = new Color(0.92f, 0.94f, 0.96f);
        RectTransform rtDesc = tmpDesc.GetComponent<RectTransform>();
        rtDesc.sizeDelta = new Vector2(1.05f, 0.35f);

        // Wire references via SerializedObject
        SerializedObject so = new SerializedObject(portalComp);
        so.FindProperty("activityType").enumValueIndex = (int)type;
        so.FindProperty("targetSceneName").stringValue = scene;
        so.FindProperty("activityTitle").stringValue = title;
        so.FindProperty("activityDescription").stringValue = desc;
        so.FindProperty("durationOrTrialsInfo").stringValue = details;
        so.FindProperty("titleText").objectReferenceValue = tmpTitle;
        so.FindProperty("descriptionText").objectReferenceValue = tmpDesc;
        so.FindProperty("indicatorLight").objectReferenceValue = button.GetComponent<Renderer>();
        so.ApplyModifiedProperties();

        // Add XR Simple Interactable for VR Controller pointer/selection
        var interactable = portal.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        BoxCollider boxCol = portal.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(1.2f, 0.8f, 0.2f);
    }

    private static void CreateWelcomeBanner(Transform parent, Vector3 pos, string text)
    {
        GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        banner.name = "Welcome_Banner";
        banner.transform.parent = parent;
        banner.transform.position = pos;
        banner.transform.localScale = new Vector3(4.5f, 0.5f, 0.04f);
        Material bannerMat = GetOrCreateMaterial("Mat_Banner_Backing", new Color(0.12f, 0.28f, 0.42f), 0.1f, 0.4f);
        banner.GetComponent<Renderer>().sharedMaterial = bannerMat;

        GameObject textObj = new GameObject("Banner_Text");
        textObj.transform.parent = banner.transform;
        textObj.transform.localPosition = new Vector3(0, 0, -0.55f);
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 3.2f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        RectTransform rt = tmp.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(4.4f, 0.45f);
    }
}
