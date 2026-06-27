using JESUIS.Editor.Elements.Common.Layout;
using JESUIS.Editor.UIBuilder.Panels.Views;
using UnityEditor;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels
{
    public class UIEditorLayoutManager
    {
        [System.Serializable]
        class SplittablePanelState
        {
            [SerializeField] public bool IsSplit = false;
            [SerializeField] public bool IsSplitVertically = false;
            [SerializeField] public float SplitPosition = 0;
            [SerializeReference] public object ElementOne = null;
            [SerializeReference] public object ElementTwo = null;
        }

        [System.Serializable]
        class UIEditorPanelState 
        {
            [SerializeField] public EditorViews.Views CurrentView;
        }

        bool editorViewIsDirty = false;
        SplittablePanel rootElement;

        public void SetRootElement(SplittablePanel panel)
        {
            rootElement = panel;
        }

        public void QueueEditorPreferenceUpdate()
        {
            if (rootElement == null)
            {
                return;
            }

            editorViewIsDirty = true; 
        }

        // Saving Editor State
        public void SaveLayout()
        {
            if (editorViewIsDirty)
            {
                SplittablePanelState state = new SplittablePanelState();
                BuildSplittablePanelState(state, rootElement);

                string serializedValue = JsonUtility.ToJson(state);
                EditorPrefs.SetString("JESUIS_UIEditorLayout", serializedValue);
            }
            editorViewIsDirty = false;
        }

        void BuildSplittablePanelState(SplittablePanelState splittablePanelState, SplittablePanel splittablePanel)
        {
            splittablePanelState.IsSplit = splittablePanel.IsSplit;
            splittablePanelState.IsSplitVertically = splittablePanel.IsSplitVertically;
            if (splittablePanel.IsSplit)
            {
                splittablePanelState.SplitPosition = splittablePanel.GetDragBarPosition();
            }

            if (splittablePanel.elementOne is SplittablePanel splitOne)
            {
                splittablePanelState.ElementOne = new SplittablePanelState();
                BuildSplittablePanelState((SplittablePanelState)splittablePanelState.ElementOne, splitOne);
            }
            else if (splittablePanel.elementOne is UIEditorPanel panelOne)
            {
                splittablePanelState.ElementOne = new UIEditorPanelState();
                BuildUIEditorState((UIEditorPanelState)splittablePanelState.ElementOne, panelOne);
            }

            if (splittablePanel.elementTwo is SplittablePanel splitTwo)
            {
                splittablePanelState.ElementTwo = new SplittablePanelState();
                BuildSplittablePanelState((SplittablePanelState)splittablePanelState.ElementTwo, splitTwo);
            }
            else if (splittablePanel.elementTwo is UIEditorPanel panelTwo)
            {
                splittablePanelState.ElementTwo = new UIEditorPanelState();
                BuildUIEditorState((UIEditorPanelState)splittablePanelState.ElementTwo, panelTwo);
            }
        }

        void BuildUIEditorState(UIEditorPanelState state, UIEditorPanel panel)
        {
            state.CurrentView = panel.GetView();
        }
        // Saving Editor State

        // Loading Editor State
        public bool HasSavedLayout()
        {
            return EditorPrefs.HasKey("JESUIS_UIEditorLayout");
        }

        public bool LoadLayout(ViewManager viewManager)
        {
            string json = EditorPrefs.GetString("JESUIS_UIEditorLayout"); 
            
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            SplittablePanelState state = JsonUtility.FromJson<SplittablePanelState>(json);

            try
            {
                if (!RegenerateSplittablePanelFromState(state, rootElement, viewManager))
                {
                    EditorPrefs.DeleteKey("JESUIS_UIEditorLayout");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to load UI Editor Preferences: {e.Message}");
                EditorPrefs.DeleteKey("JESUIS_UIEditorLayout");
                return false;
            }

            Debug.Log("Loaded UI Editor layout from preferences");
            return true;
        }

        bool RegenerateSplittablePanelFromState(SplittablePanelState state, SplittablePanel panel, ViewManager viewManager)
        {
            if (state.ElementOne is UIEditorPanelState uiState)
            {
                UIEditorPanel newPanel = new UIEditorPanel(viewManager, this);
                newPanel.SetView(uiState.CurrentView);
                panel.SetToInitialState(newPanel);
            }
            else if (state.ElementOne is SplittablePanelState splitState)
            {
                SplittablePanel newPanel = new SplittablePanel();
                panel.SetToInitialState(newPanel);
                RegenerateSplittablePanelFromState(splitState, newPanel, viewManager);
            }
            else
            {
                Debug.LogError("Failed to loaded UI Editor Preferences: Invalid state");
                return false;
            } 

            if (state.IsSplit)
            {
                if (state.ElementTwo is UIEditorPanelState uiState2)
                {
                    UIEditorPanel newPanel = new UIEditorPanel(viewManager, this); 
                    newPanel.SetView(uiState2.CurrentView);

                    if (state.IsSplitVertically) 
                    {
                        panel.SplitVertically(newPanel);
                    }
                    else
                    {
                        panel.SplitHorizontally(newPanel);
                    }

                }
                else if (state.ElementTwo is SplittablePanelState splitState2)
                {
                    SplittablePanel newPanel = new SplittablePanel();
                    if (state.IsSplitVertically)
                    {
                        panel.SplitVertically(newPanel);
                    }
                    else
                    {
                        panel.SplitHorizontally(newPanel);
                    }
                    RegenerateSplittablePanelFromState(splitState2, newPanel, viewManager);
                }
                else
                {
                    Debug.LogError("Failed to loaded UI Editor Preferences: Invalid state");
                    return false; 
                }

                panel.SetDragBarPosition(state.SplitPosition); 
            }
            return true;
        }
        // Loading Editor State
    }
}
