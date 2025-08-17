using System.IO;
using Erogemy.BlockBreaker.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Erogemy.BlockBreaker.Editor
{
    public class BlockBreakerBuilderWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset m_VisualTreeAsset;

        int currentPhase = 1;
        BockBreakerSettings settings = new();

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
            CheckTMProResources();

            AssetDatabase.importPackageCompleted += (_) => CheckTMProResources();
        }

        void CheckTMProResources()
        {
            var isVisible = !File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");

            // TextMeshProのリソースがない場合は、インポートするように促す
            var messageContainer = rootVisualElement.Q<VisualElement>("tmp-message-container");
            if (messageContainer != null)
            {
                messageContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        void SetupUIBindings(VisualElement root)
        {
            var updatePackageBtn = root.Q<Button>("update-package-button");
            if (updatePackageBtn != null)
            {
                updatePackageBtn.clicked += UpdatePackage;
            }

            var createSceneBtn = root.Q<Button>("create-scene-button");
            if (createSceneBtn != null)
            {
                createSceneBtn.clicked += () =>
                {
                    var blockSizeWPx = root.Q<IntegerField>("block-width-size-field").value;
                    var blockSizeHPx = root.Q<IntegerField>("block-height-size-field").value;

                    if (blockSizeWPx < 1 || blockSizeHPx < 1)
                    {
                        SetShowMessage(true);
                        SetSystemMessage("ブロックのサイズは1px以上を指定してください");
                        return;
                    }

                    SetShowMessage(false);
                    BlockBreakerBuilder.Build(new Vector2Int(blockSizeWPx, blockSizeHPx), settings);
                };
            }

            var buildForWebGLBtn = root.Q<Button>("build-for-webgl-button");
            if (buildForWebGLBtn != null)
            {
                buildForWebGLBtn.clicked += BlockBreakerBuilder.BuildScene;
            }

            var reloadPreviewBtn = root.Q<Button>("reload-preview-button");
            if (reloadPreviewBtn != null)
            {
                reloadPreviewBtn.clicked += UpdatePreview;
            }

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

            var ballCountField = root.Q<IntegerField>("ball-count-field");
            ballCountField.value = settings.ballCount;
            ballCountField.RegisterValueChangedCallback(evt => settings.ballCount = evt.newValue);

            var ballSpeedField = root.Q<IntegerField>("ball-speed-field");
            ballSpeedField.value = settings.ballMoveSpeed;
            ballSpeedField.RegisterValueChangedCallback(evt => settings.ballMoveSpeed = evt.newValue);

            var skipPhaseThresholdField = root.Q<IntegerField>("skip-threshold-field");
            skipPhaseThresholdField.value = settings.skipPhaseThreshold;
            skipPhaseThresholdField.RegisterValueChangedCallback(evt => settings.skipPhaseThreshold = evt.newValue);

            var reflectionAngleField = root.Q<FloatField>("reflection-angle-field");
            reflectionAngleField.value = settings.ballReflectionAngle;
            reflectionAngleField.RegisterValueChangedCallback(evt => settings.ballReflectionAngle = evt.newValue);

            var isResetBallField = root.Q<Toggle>("reset-ball-toggle");
            isResetBallField.value = settings.recoverBallOnPhaseClear;
            isResetBallField.RegisterValueChangedCallback(evt => settings.recoverBallOnPhaseClear = evt.newValue);

            var tmpImportBtn = rootVisualElement.Q<Button>("import-tmp-button");
            if (tmpImportBtn != null)
            {
                tmpImportBtn.clicked += () =>
                {
                    AssetDatabase.ImportPackage("Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage", false);
                    EditorApplication.delayCall += CheckTMProResources;
                };
            }
        }

        void UpdatePackage()
        {
            UnityEditor.PackageManager.Client.Add("https://github.com//erogemy/BlockBreakerTemplate.git?path=/Assets/#main");
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
}
