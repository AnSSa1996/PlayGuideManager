using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayGuideFraemwork.PlayGuide
{
    public class PlayGuideEditor : EditorWindow
    {
        public string NAME = string.Empty;
        public PlayGuideScriptableObject currentPlayGuide_ScriptableObject = null;
        private ObjectField currentPlayGuideObjectField = null;
        private TextField _nameTextField = null;

        private int _selectedStepIndex = 0;
        private string _selectedStepName = string.Empty;

        private TemplateContainer _stepParentTemplateContainer = null;
        private TemplateContainer _stepListTemplateContainer = null;
        private TemplateContainer _stepParentButtonTemplateContainer = null;
        private TemplateContainer _stepMoveButtonTemplateContainer = null;
        private ListView _stepListView = null;
        private List<string> _stepNames = new List<string>();
        private ScrollView _stepDetailScrollView = null;

        [MenuItem("PlayGuide/Play Guide Editor", false, 1)]
        public static void OpenWindow()
        {
            PlayGuideEditor NewWindow = GetWindow<PlayGuideEditor>();
            if (NewWindow != null)
            {
                NewWindow.titleContent = new GUIContent("Play Guide Editor");
                NewWindow.minSize = new Vector2(1000f, 800f);
                NewWindow.autoRepaintOnSceneChange = true;
                NewWindow.Show();
            }
        }

        public void CreateGUI()
        {
            if (currentPlayGuide_ScriptableObject == null) currentPlayGuide_ScriptableObject = CreateInstance<PlayGuideScriptableObject>();
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/PlayGuide/Editor/PlayGuide_Properties_Style.uss"));

            var fileProperty = new VisualElement();

            currentPlayGuideObjectField = new ObjectField("현재 선택된 PlayGuide");
            currentPlayGuideObjectField.labelElement.AddToClassList("currentObjectField-label");
            currentPlayGuideObjectField.AddToClassList("current-objectField");
            currentPlayGuideObjectField.RegisterValueChangedCallback(evt => StepRefresh());
            rootVisualElement.Add(currentPlayGuideObjectField);

            var nameProperty = new VisualElement();
            var nameLabel = new Label("PlayGuide 이름");
            nameLabel.AddToClassList("name-label");
            nameProperty.Add(nameLabel);
            _nameTextField = new TextField();
            _nameTextField.AddToClassList("nameTextField-textField");
            nameProperty.Add(_nameTextField);
            var nameChangeButton = new Button(NameChange);
            nameChangeButton.text = "이름 변경";
            nameChangeButton.AddToClassList("nameChangeButton-button");
            nameProperty.Add(nameChangeButton);
            nameProperty.AddToClassList("horizontalDontArrange-container");
            rootVisualElement.Add(nameProperty);

            var loadFileButton = new Button(LoadFile);
            loadFileButton.text = $"LoadFile";
            loadFileButton.AddToClassList("loadFile-button");
            fileProperty.Add(loadFileButton);
            
            var saveFileButton = new Button(SaveFile);
            saveFileButton.text = $"SaveFile";
            saveFileButton.AddToClassList("saveFile-button");
            fileProperty.Add(saveFileButton);
            
            fileProperty.AddToClassList("horizontal-container");
            rootVisualElement.Add(fileProperty);

            var stepScrollTitleLabel = new Label("스탭 목록");
            stepScrollTitleLabel.AddToClassList("title-label");
            rootVisualElement.Add(stepScrollTitleLabel);

            _stepParentTemplateContainer = new TemplateContainer();
            _stepListTemplateContainer = new TemplateContainer();
            _stepParentButtonTemplateContainer = new TemplateContainer();
            _stepMoveButtonTemplateContainer = new TemplateContainer();

            CreateStepListView();
            CreateStepDetailScrollView();
            _stepListTemplateContainer.Add(_stepListView);

            var upButton = new Button(StepMoveUP);
            upButton.text = "<";
            upButton.AddToClassList("playGuideStepButton-button");
            var downButton = new Button(StepMoveDown);
            downButton.text = ">";
            downButton.AddToClassList("playGuideStepButton-button");
            _stepMoveButtonTemplateContainer.Add(upButton);
            _stepMoveButtonTemplateContainer.Add(downButton);
            _stepMoveButtonTemplateContainer.AddToClassList("playGuideStepButton-templateContainer");

            var cleanAllStepButton = new Button(CleanStepName);
            cleanAllStepButton.text = "스탭 이름 정리";
            cleanAllStepButton.AddToClassList("playGuideStepCleanButton-button");

            _stepParentButtonTemplateContainer.Add(cleanAllStepButton);
            _stepParentButtonTemplateContainer.Add(_stepMoveButtonTemplateContainer);

            _stepParentButtonTemplateContainer.AddToClassList("playGuideStepParentButton-templateContainer");
            _stepListTemplateContainer.Add(_stepParentButtonTemplateContainer);
            _stepListTemplateContainer.AddToClassList("playGuideStepList-templateContainer");

            _stepParentTemplateContainer.Add(_stepListTemplateContainer);
            _stepParentTemplateContainer.Add(_stepDetailScrollView);
            _stepParentTemplateContainer.AddToClassList("playGuideParentStep-templateContainer");
            rootVisualElement.Add(_stepParentTemplateContainer);

            var stepProperty = new VisualElement();
            var stepAddButton = new Button(StepAdd);
            stepAddButton.text = $"스텝 추가";
            stepAddButton.AddToClassList("stepAddButton-button");
            stepProperty.Add(stepAddButton);
            var stepRemoveButton = new Button(StepRemove);
            stepRemoveButton.text = $"스텝 제거";
            stepRemoveButton.AddToClassList("stepRemoveButton-button");
            stepProperty.Add(stepRemoveButton);
            stepProperty.AddToClassList("horizontal-container");
            rootVisualElement.Add(stepProperty);
        }

        public void LoadFile()
        {
            string path = EditorUtility.OpenFilePanel("Select PlayGuide File", $"Assets/ScriptableObjects/PlayGuideData", "asset");
            path = path.Replace(Application.dataPath, "Assets");
            var scriptableObject = AssetDatabase.LoadAssetAtPath<PlayGuideScriptableObject>($"{path}");
            if (scriptableObject.IsUnityNull()) return;
            currentPlayGuide_ScriptableObject = scriptableObject;
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            currentPlayGuideObjectField.SetValueWithoutNotify(currentPlayGuide_ScriptableObject);
            _nameTextField.value = currentPlayGuide_ScriptableObject.name;
            CleanAllStep();
            StepRefresh();
        }

        public void SaveFile()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            if (string.IsNullOrEmpty(_nameTextField.value))
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름을 설정해 주시기 바랍니다.", this.position);
                return;
            }

            if (string.IsNullOrEmpty(currentPlayGuide_ScriptableObject.name) == false && currentPlayGuide_ScriptableObject.name != _nameTextField.value)
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름 변경을 눌러주시기 바랍니다.", this.position);
                return;
            }

            ShowPlayGuideEditor playguide = CreateInstance<ShowPlayGuideEditor>();

            playguide.Init($"파일을 저장하시겠습니까?",
                () =>
                {
                    if (!AssetDatabase.IsValidFolder($"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}"))
                    {
                        AssetDatabase.CreateFolder($"Assets/ScriptableObjects/PlayGuideData", _nameTextField.value);
                    }

                    if (!AssetDatabase.Contains(currentPlayGuide_ScriptableObject))
                        AssetDatabase.CreateAsset(currentPlayGuide_ScriptableObject, $"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}/{_nameTextField.value}.asset");

                    currentPlayGuideObjectField.SetValueWithoutNotify(currentPlayGuide_ScriptableObject);
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                    CleanAllStep();
                    playguide.Close();
                },
                this.position
            );
        }

        private void NameChange()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            string assetPath = AssetDatabase.GetAssetPath(currentPlayGuide_ScriptableObject);
            string folderPath = assetPath.Replace($"/{currentPlayGuide_ScriptableObject.name}.asset", "");
            AssetDatabase.RenameAsset(assetPath, _nameTextField.value);
            AssetDatabase.RenameAsset(folderPath, _nameTextField.value);
            AssetDatabase.Refresh();
            CleanAllStep();
        }

        private void StepMoveUP()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            if (currentPlayGuide_ScriptableObject.PlayGuideSteps.IsUnityNull()) return;

            if (string.IsNullOrEmpty(currentPlayGuide_ScriptableObject.name))
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("파일을 먼저 저장해주시기 바랍니다.", this.position);
                return;
            }

            if (currentPlayGuide_ScriptableObject.name != _nameTextField.value)
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름 변경을 눌러주시기 바랍니다.", this.position);
                return;
            }

            var currentIndex = _selectedStepIndex;
            var upIndex = currentIndex - 1;
            if (upIndex < 0) return;
            _selectedStepIndex = upIndex;
            var currentStep = currentPlayGuide_ScriptableObject.PlayGuideSteps[currentIndex];
            var upStep = currentPlayGuide_ScriptableObject.PlayGuideSteps[upIndex];
            (currentStep.Order, upStep.Order) = (upStep.Order, currentStep.Order);
            CleanAllStep();
        }

        private void StepMoveDown()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            if (currentPlayGuide_ScriptableObject.PlayGuideSteps.IsUnityNull()) return;

            if (string.IsNullOrEmpty(currentPlayGuide_ScriptableObject.name))
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("파일을 먼저 저장해주시기 바랍니다.", this.position);
                return;
            }

            if (currentPlayGuide_ScriptableObject.name != _nameTextField.value)
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름 변경을 눌러주시기 바랍니다.", this.position);
                return;
            }

            var currentIndex = _selectedStepIndex;
            var downIndex = currentIndex + 1;
            if (downIndex >= currentPlayGuide_ScriptableObject.PlayGuideSteps.Count) return;
            _selectedStepIndex = downIndex;
            var currentStep = currentPlayGuide_ScriptableObject.PlayGuideSteps[currentIndex];
            var downStep = currentPlayGuide_ScriptableObject.PlayGuideSteps[downIndex];
            (currentStep.Order, downStep.Order) = (downStep.Order, currentStep.Order);
            CleanAllStep();
        }

        private void StepAdd()
        {
            StepAddChosePopupEditor popup = CreateInstance<StepAddChosePopupEditor>();

            if (string.IsNullOrEmpty(_nameTextField.value))
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름을 설정해 주시기 바랍니다.", this.position);
                return;
            }

            if (string.IsNullOrEmpty(currentPlayGuide_ScriptableObject.name) == false && currentPlayGuide_ScriptableObject.name != _nameTextField.value)
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름 변경을 눌러주시기 바랍니다.", this.position);
                return;
            }

            popup.Init($"어떤 스텝을 추가하시겠습니까?",
                () =>
                {
                    int order = currentPlayGuide_ScriptableObject.PlayGuideSteps?.Count() ?? 0;
                    popup.OnClickAdd();
                    if (StepAddChosePopupEditor.SelectedPlayGuideStep.IsUnityNull()) return;
                    if (!AssetDatabase.IsValidFolder($"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}"))
                    {
                        AssetDatabase.CreateFolder($"Assets/ScriptableObjects/PlayGuideData", _nameTextField.value);
                    }

                    if (!AssetDatabase.IsValidFolder($"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}/Steps"))
                    {
                        AssetDatabase.CreateFolder($"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}", "Steps");
                    }

                    if (!AssetDatabase.Contains(StepAddChosePopupEditor.SelectedPlayGuideStep))
                        AssetDatabase.CreateAsset(StepAddChosePopupEditor.SelectedPlayGuideStep,
                            $"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}/Steps/{StepAddChosePopupEditor.SelectedPlayGuideStep.GetType().Name}_{order}.asset");
                    StepAddChosePopupEditor.SelectedPlayGuideStep.Order = order;
                    AssetDatabase.SaveAssets();
                    currentPlayGuide_ScriptableObject.PlayGuideSteps.Add(StepAddChosePopupEditor.SelectedPlayGuideStep);
                    EditorUtility.FocusProjectWindow();
                    StepRefresh();
                    RefreshStepListView();
                },
                this.position
            );
        }

        private void StepRemove()
        {
            if (string.IsNullOrEmpty(currentPlayGuide_ScriptableObject.name) == false && currentPlayGuide_ScriptableObject.name != _nameTextField.value)
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름 변경을 눌러주시기 바랍니다.", this.position);
                return;
            }

            AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/PlayGuideData/{_nameTextField.value}/Steps/{_selectedStepName}.asset");
            currentPlayGuide_ScriptableObject.CleanEmptyList();
            AssetDatabase.Refresh();
            StepRefresh();
            RefreshStepListView();
        }
        
        private void CleanStepName()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            if (currentPlayGuide_ScriptableObject.PlayGuideSteps.IsUnityNull()) return;

            if (string.IsNullOrEmpty(currentPlayGuide_ScriptableObject.name))
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("파일을 먼저 저장해주시기 바랍니다.", this.position);
                return;
            }

            if (currentPlayGuide_ScriptableObject.name != _nameTextField.value)
            {
                AlertPopupEditor alertPopup = CreateInstance<AlertPopupEditor>();
                alertPopup.Init("이름 변경을 눌러주시기 바랍니다.", this.position);
                return;
            }

            StepAllRename();
            StepRefresh();
            RefreshStepListView();
        }

        private void StepAllRename()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            if (currentPlayGuide_ScriptableObject.PlayGuideSteps.IsUnityNull()) return;
            currentPlayGuide_ScriptableObject.PlayGuideSteps.Sort((a, b) => a.Order.CompareTo(b.Order));
            int order = 0;

            foreach (var step in currentPlayGuide_ScriptableObject.PlayGuideSteps)
            {
                string assetPath = AssetDatabase.GetAssetPath(step);
                Debug.Log(AssetDatabase.RenameAsset(assetPath, $"{step.GetType().Name}_{order}_Change"));
                step.Order = order++;
            }

            AssetDatabase.Refresh();
            order = 0;
            foreach (var step in currentPlayGuide_ScriptableObject.PlayGuideSteps)
            {
                string assetPath = AssetDatabase.GetAssetPath(step);
                Debug.Log(AssetDatabase.RenameAsset(assetPath, $"{step.GetType().Name}_{order}"));
                step.Order = order++;
            }

            AssetDatabase.Refresh();
        }

        private void CleanAllStep()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            string assetPath = AssetDatabase.GetAssetPath(currentPlayGuide_ScriptableObject);
            string folderPath = assetPath.Replace($"/{currentPlayGuide_ScriptableObject.name}.asset", "");
            var stepInFolder_List = new List<PlayGuideStep>();
            string[] guids = AssetDatabase.FindAssets("", new[] { $"{folderPath}/Steps" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                PlayGuideStep step = AssetDatabase.LoadAssetAtPath<PlayGuideStep>(path);
                if (step.IsUnityNull()) continue;
                stepInFolder_List.Add(step);
            }

            stepInFolder_List.Sort((a, b) => a.Order.CompareTo(b.Order));
            currentPlayGuide_ScriptableObject.PlayGuideSteps.Clear();
            currentPlayGuide_ScriptableObject.PlayGuideSteps.AddRange(stepInFolder_List);

            StepRefresh();
            RefreshStepListView();
        }

        private void StepRefresh()
        {
            if (currentPlayGuide_ScriptableObject.IsUnityNull()) return;
            if (currentPlayGuide_ScriptableObject.PlayGuideSteps.IsUnityNull()) return;

            _stepParentTemplateContainer.Clear();
            _stepListTemplateContainer.Clear();
            _stepListView.Clear();
            _stepDetailScrollView.Clear();
            _stepNames.Clear();
            currentPlayGuide_ScriptableObject.CleanEmptyList();
            
            foreach (var step in currentPlayGuide_ScriptableObject.PlayGuideSteps)
            {
                _stepNames.Add(step.name);
            }
            
            CreateStepListView();
            _stepListTemplateContainer.Add(_stepListView);
            RefreshStepListButton();
            _stepParentTemplateContainer.Add(_stepListTemplateContainer);
            _stepParentTemplateContainer.Add(_stepDetailScrollView);
            _stepParentTemplateContainer.MarkDirtyRepaint();
            if (_selectedStepIndex < currentPlayGuide_ScriptableObject.PlayGuideSteps.Count) _stepListView.SetSelection(_selectedStepIndex);
            _stepListView.MarkDirtyRepaint();
            Repaint();
            EditorUtility.SetDirty(currentPlayGuide_ScriptableObject);
            foreach (var step in currentPlayGuide_ScriptableObject.PlayGuideSteps)
            {
                EditorUtility.SetDirty(step);
            }

            _stepListView.ScrollToItem(_stepListView.selectedIndex);
        }
        
        private void RefreshStepListButton()
        {
            _stepParentButtonTemplateContainer.Clear();
            _stepMoveButtonTemplateContainer.Clear();
            var upButton = new Button(StepMoveUP);
            upButton.text = "<";
            upButton.AddToClassList("playGuideStepButton-button");
            var downButton = new Button(StepMoveDown);
            downButton.text = ">";
            downButton.AddToClassList("playGuideStepButton-button");
            _stepMoveButtonTemplateContainer.Add(upButton);
            _stepMoveButtonTemplateContainer.Add(downButton);
            var cleanAllStepButton = new Button(CleanStepName);
            cleanAllStepButton.text = "스탭 이름 정리";
            cleanAllStepButton.AddToClassList("playGuideStepCleanButton-button");

            _stepParentButtonTemplateContainer.Add(cleanAllStepButton);
            _stepParentButtonTemplateContainer.Add(_stepMoveButtonTemplateContainer);
            _stepListTemplateContainer.Add(_stepParentButtonTemplateContainer);
        }

        private VisualElement CreateStepListView()
        {
            _stepListView = new ListView(_stepNames, 20)
            {
                name = "step-list",
                selectionType = SelectionType.Single,

                style =
                {
                    minHeight = 300f,
                    flexGrow = 1f,
                }
            };

            _stepListView.onSelectionChange -= SelectStepItem;
            _stepListView.onSelectionChange += SelectStepItem;
            _stepListView.AddToClassList("playGuideStepName-listview");
            _stepListView.focusable = true;

            return _stepListView;
        }

        private VisualElement CreateStepDetailScrollView()
        {
            _stepDetailScrollView = new ScrollView();
            _stepDetailScrollView.AddToClassList("playGuideStepDetail-scrollview");
            return _stepDetailScrollView;
        }

        private void SelectStepItem(object InObj)
        {
            _selectedStepIndex = _stepListView.selectedIndex;
            _selectedStepName = _stepNames[_selectedStepIndex];
            _stepDetailScrollView.Clear();

            var SelectStepData = currentPlayGuide_ScriptableObject.PlayGuideSteps[_selectedStepIndex];
            if (SelectStepData != null)
            {
                var serializeData = new SerializedObject(SelectStepData);
                if (serializeData == null)
                {
                    return;
                }

                var property = serializeData.GetIterator();
                property.NextVisible(true);

                do
                {
                    var DefaultProperty = new PropertyField(property);

                    if (DefaultProperty.bindingPath.Contains("m_Script"))
                    {
                        continue;
                    }

                    _stepDetailScrollView.Add(DefaultProperty);
                } while (property.NextVisible(false));

                _stepDetailScrollView.Bind(serializeData);
            }
        }

        private async void RefreshStepListView()
        {
            _stepListView?.Rebuild();
            await Task.Delay(1);
            _stepListView?.ScrollToItem(_stepListView.selectedIndex);
        }
    }

    public class ShowPlayGuideEditor : EditorWindow
    {
        private string Title = string.Empty;
        private Action OnAgreeAction = null;

        public void Init(string title, Action onAgreeAction, Rect parentRect)
        {
            Title = title;
            OnAgreeAction = onAgreeAction;

            this.titleContent = new GUIContent("OK");
            this.position = new Rect(parentRect.position.x, parentRect.position.y, 150, 100);
            this.minSize = new Vector2(200f, 100f);
            this.Show();
            this.Focus();
        }

        private void CreateGUI()
        {
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/PlayGuide/Editor/PlayGuide_Properties_Style.uss"));

            var titleContainer = new VisualElement();
            var titleLabel = new Label(Title);
            titleContainer.Add(titleLabel);
            rootVisualElement.Add(titleContainer);

            var buttonContainer = new VisualElement();
            var AgreeButton = new Button(OnAgreeAction);
            AgreeButton.text = "YES";
            AgreeButton.AddToClassList("YESNO-button");
            var DisAgreeButton = new Button(this.Close);
            DisAgreeButton.text = "NO";
            DisAgreeButton.AddToClassList("YESNO-button");

            buttonContainer.Add(AgreeButton);
            buttonContainer.Add(DisAgreeButton);
            buttonContainer.AddToClassList("horizontal-container");
            rootVisualElement.Add(buttonContainer);
        }
    }

    public class StepAddChosePopupEditor : EditorWindow
    {
        public static PlayGuideStep SelectedPlayGuideStep = null;
        private int _selectedGuideStepIndex = 0;

        private List<Type> _playGuideStepTypeList = new List<Type>();
        private ListView _addGuideStepListView = null;
        private string Title = string.Empty;
        private Action OnAgreeAction = null;

        public void Init(string title, Action onAgreeAction, Rect parentRect)
        {
            Title = title;
            OnAgreeAction = onAgreeAction;

            this.titleContent = new GUIContent("Alert");
            this.position = new Rect(parentRect.position.x, parentRect.position.y, 150, 100);
            this.minSize = new Vector2(400f, 400f);
            this.Show();
            this.Focus();
        }

        private void CreateGUI()
        {
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/PlayGuide/Editor/PlayGuide_Properties_Style.uss"));

            var titleContainer = new VisualElement();
            var titleLabel = new Label(Title);
            titleContainer.Add(titleLabel);
            rootVisualElement.Add(titleContainer);

            _playGuideStepTypeList = Assembly
                .GetAssembly(typeof(PlayGuideStep))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(PlayGuideStep))).ToList();
            _addGuideStepListView = new ListView(_playGuideStepTypeList, 20f, MakeItem, BindItem);
            _addGuideStepListView.onSelectionChange -= OnSelectStepChange;
            _addGuideStepListView.onSelectionChange += OnSelectStepChange;
            _addGuideStepListView.AddToClassList("addPlayGuideStepListView-listView");
            rootVisualElement.Add(_addGuideStepListView);

            var buttonContainer = new VisualElement();
            var AgreeButton = new Button(OnAgreeAction);
            AgreeButton.text = "추가";
            AgreeButton.AddToClassList("YESNO-button");
            var DisAgreeButton = new Button(this.Close);
            DisAgreeButton.text = "취소";
            DisAgreeButton.AddToClassList("YESNO-button");

            buttonContainer.Add(AgreeButton);
            buttonContainer.Add(DisAgreeButton);
            buttonContainer.AddToClassList("horizontal-container");
            rootVisualElement.Add(buttonContainer);
        }

        public void OnClickAdd()
        {
            SelectedPlayGuideStep = Activator.CreateInstance(_playGuideStepTypeList[_selectedGuideStepIndex]) as PlayGuideStep;
        }

        private void OnSelectStepChange(object InObj)
        {
            _selectedGuideStepIndex = _addGuideStepListView.selectedIndex;
        }

        private VisualElement MakeItem()
        {
            var Item = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1f,
                    flexShrink = 0f,
                    flexBasis = 0f
                }
            };
            Item.Add(new Label());
            Item.AddToClassList("addPlayGuideStep-label");

            return Item;
        }

        private void BindItem(VisualElement InVE, int InIndex)
        {
            var Elem = InVE.ElementAt(0) as Label;
            if (Elem != null && _playGuideStepTypeList.Count > InIndex)
            {
                Elem.text = _playGuideStepTypeList[InIndex].Name;
            }
        }
    }

    public class AlertPopupEditor : EditorWindow
    {
        private string _content = string.Empty;

        public void Init(string content, Rect parentRect)
        {
            _content = content;
            this.titleContent = new GUIContent("OK");
            this.position = new Rect(parentRect.position.x, parentRect.position.y, 150, 100);
            this.minSize = new Vector2(200f, 200f);
            this.Show();
            this.Focus();
        }

        private void CreateGUI()
        {
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/PlayGuide/Editor/PlayGuide_Properties_Style.uss"));

            var titleContainer = new VisualElement();
            var titleLabel = new Label(_content);
            titleContainer.Add(titleLabel);
            rootVisualElement.Add(titleContainer);


            var buttonContainer = new VisualElement();
            var checkButton = new Button(this.Close);
            checkButton.text = "확인";
            checkButton.AddToClassList("checkButton-button");
            buttonContainer.Add(checkButton);
            buttonContainer.AddToClassList("horizontal-container");
            rootVisualElement.Add(buttonContainer);
        }
    }
}