using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] UIDocument doc;
    [SerializeField] PanelManager panelManager;
    UIDocumentLocalization localization;
    Button done;

    VisualElement tutorImage;
    Label moveDescription;
    Label perkDescription;

    void OnEnable() => GetComponent<UIDocumentLocalization>().onCompleted += Bind;

    void Bind ()
    {
        localization = GetComponent<UIDocumentLocalization>();
        done = (Button)doc.rootVisualElement.Query("Done");
        done.clicked += Done_clicked;

        tutorImage = doc.rootVisualElement.Query("TutorImage");
        moveDescription = (Label)doc.rootVisualElement.Query("moveDescLabel");
        perkDescription = (Label)doc.rootVisualElement.Query("Description");

        if (Application.platform == RuntimePlatform.Android)
        {
            tutorImage.AddToClassList("tutor-image-android");
            moveDescription.text = "#movement-android-description";
            perkDescription.text = "#perks-description-android";
            localization.LocalizeManually(moveDescription);
            localization.LocalizeManually(perkDescription);
        }
    }

    private void Done_clicked()
    {
        panelManager.SetMainMenuState(true);
        panelManager.SetTutorialPanelState(false);
    }
}
