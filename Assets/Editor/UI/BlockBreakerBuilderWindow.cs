using Erogemy.BlockBreaker.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockBreakerBuilderWindow : EditorWindow
{
    [SerializeField] VisualTreeAsset m_VisualTreeAsset;

    int currentPhase = 1;

    [MenuItem("Tools/Erogemy/BlockBreaker/BlockBreakerBuilderWindow")]
    public static void ShowExample()
    {
        var wnd = GetWindow<BlockBreakerBuilderWindow>();
        wnd.titleContent = new GUIContent("BlockBreakerBuilderWindow");
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        SetupUIBindings(root);
        UpdatePreview();
    }

    void SetupUIBindings(VisualElement root)
    {
        var createSceneBtn = root.Q<Button>("create-scene-button");
        if (createSceneBtn != null)
        {
            // TODO
            createSceneBtn.clicked += () => BlockBreakerBuilder.Build(16);
        }

        var createPrefabsBtn = root.Q<Button>("create-prefabs-button");
        if (createPrefabsBtn != null)
        {
            // TODO
            createPrefabsBtn.clicked += () => Debug.Log("Create Prefabs button clicked");
        }

        var reloadPreviewBtn = root.Q<Button>("reload-preview-button");
        if (reloadPreviewBtn != null)
        {
            reloadPreviewBtn.clicked += UpdatePreview;
        };

        var phasePrevBtn = root.Q<Button>("phase-prev-button");
        if (phasePrevBtn != null)
        {
            phasePrevBtn.clicked += () =>
            {
                currentPhase--;
                UpdatePreview();
            };
        }

        var phaseNextBtn = root.Q<Button>("phase-next-button");
        if (phaseNextBtn != null)
        {
            phaseNextBtn.clicked += () =>
            {
                currentPhase++;
                UpdatePreview();
            };
        }
    }

    void SetShowMessage(bool isVisible)
    {
        var messageContainer = rootVisualElement.Q<VisualElement>("system-message-container");
        if (messageContainer != null)
        {
            messageContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    void SetSystemMessage(string message)
    {
        var messageLabel = rootVisualElement.Q<Label>("system-message-label");
        if (messageLabel != null)
        {
            messageLabel.text = message;
        }
    }

    void UpdatePreview()
    {
        // フォルダを見てcurrentPhaseをクリップ
        currentPhase = Mathf.Clamp(currentPhase, 1, ResourcesValidator.ValidateAndGetPhaseCount(out var message));
        if (message != null)
        {
            SetShowMessage(true);
            SetSystemMessage(message);
            return;
        }
        SetShowMessage(false);

        var previewArea = rootVisualElement.Q<VisualElement>("PreviewArea");

        var baseImage = AssetDatabase.LoadAssetAtPath<Texture2D>($"{EditorConsts.ImagesPath}Phase_{currentPhase}/{EditorConsts.BaseImageName}");
        AddPreviewImageLayer(previewArea, baseImage);

        var blockImage = AssetDatabase.LoadAssetAtPath<Texture2D>($"{EditorConsts.ImagesPath}Phase_{currentPhase}/{EditorConsts.BlockImageName}");
        AddPreviewImageLayer(previewArea, blockImage);

        // ラベルの更新
        var label = rootVisualElement.Q<Label>("current-phase-label");
        if (label != null)
        {
            label.text = $"Phase {currentPhase}";
        }
    }

    void AddPreviewImageLayer(VisualElement previewArea, Texture2D image)
    {
        var heightPx = 320f;
        var widthPx = heightPx * image.width / image.height;

        var backgroundImage = new VisualElement
        {
            style =
            {
                backgroundImage = new StyleBackground(image),
                width = widthPx,
                height = heightPx,
                position = Position.Absolute,
            }
        };
        previewArea.Add(backgroundImage);
    }
}
