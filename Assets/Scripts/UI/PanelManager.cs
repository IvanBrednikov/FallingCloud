using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class PanelManager : MonoBehaviour
{
    [SerializeField] UIDocument mainMenu;
    [SerializeField] UIDocument levelScreen;
    [SerializeField] UIDocument options;
    [SerializeField] UIDocument result;
    [SerializeField] UIDocument pauseMenu;
    [SerializeField] UIDocument tutorial;
    [SerializeField] PlayerInput input;

    UIDocument activePanel; //for gameplay video

    void SetPanelEnableState(UIDocument panel, bool state)
    {
        if(state)
        {
            panel.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            panel.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    private void OnEnable()
    {
        SetResultPanelState(false);
        SetLevelScreenState(false);
        SetOptionsState(false);
        SetPausePanelState(false);
        SetTutorialPanelState(false);
        activePanel = mainMenu;
    }

    public void SetMainMenuState(bool state)
    {
        SetPanelEnableState(mainMenu, state);
        if(state)
        {
            MenuPanel docHandler = FindObjectOfType<MenuPanel>();
            if (docHandler != null)
                docHandler.UpdateAttempts();
        }
    }

    public void SetLevelScreenState(bool state)
    {
        SetPanelEnableState(levelScreen, state);
        input.enabled = state;
    }

    public void SetOptionsState(bool state)
    {
        SetPanelEnableState(options, state);
        OptionsPanel docHandler = FindObjectOfType<OptionsPanel>();
        if (docHandler != null)
            docHandler.UpdateSettings();
    }

    public void SetResultPanelState(bool state)
    {
        SetPanelEnableState(result, state);
    }

    public void SetPausePanelState(bool state)
    {
        SetPanelEnableState(pauseMenu, state);
        PausePanel docHandler = FindObjectOfType<PausePanel>();
        if(docHandler != null)
            docHandler.UpdateInfo();
    }

    public void SetTutorialPanelState(bool state)
    {
        SetPanelEnableState(tutorial, state);
    }

    public void HideCurrentPanel()
    {
        UIDocument[] panels = FindObjectsOfType<UIDocument>();
        foreach (UIDocument panel in panels)
            if (panel.rootVisualElement.style.display == DisplayStyle.Flex)
            {
                activePanel = panel;
                break;
            }
        
        if (activePanel != null)
            SetPanelEnableState(activePanel, false);
    }

    public void UnhideCurrentPanel()
    {
        if (activePanel != null)
            SetPanelEnableState(activePanel, true);
        activePanel = null;
    }
}
