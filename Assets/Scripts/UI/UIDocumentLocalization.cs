using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public class UIDocumentLocalization : MonoBehaviour
{
	private Dictionary<VisualElement, string> _originalTexts = new Dictionary<VisualElement, string>();

	[SerializeField] LocalizedStringTable _table = null;
	UIDocument _uiDocument;

	/// <summary> Executed after hierarchy is cloned fresh and translated. </summary>
	public event System.Action onCompleted = () => {  };
	void OnEnable()
	{
		if (_uiDocument == null)
			_uiDocument = GetComponent<UIDocument>();
		_table.TableChanged += OnTableChanged;
	}

	void OnDisable()
	{
		_table.TableChanged -= OnTableChanged;
	}

	void OnTableChanged(StringTable table)
	{
//		ConvenientLogger.Log(nameof(UIDocumentLocalization), GlobalLogConstant.IsLocalizeLogEnabled, $"{nameof(StringTable)} changed, {nameof(VisualTreeAsset)} has been cloned anew",
	//		_uiDocument);

		var op = _table.GetTableAsync();
		if (op.IsDone)
		{
			OnTableLoaded(op);
		}
		else
		{
			op.Completed -= OnTableLoaded;
			op.Completed += OnTableLoaded;
		}
	}

	void OnTableLoaded(AsyncOperationHandle<StringTable> op)
	{
		StringTable table = op.Result;
		LocalizeChildrenRecursively(_uiDocument.rootVisualElement, table);
		_uiDocument.rootVisualElement.MarkDirtyRepaint();
		onCompleted();
	}

	void LocalizeChildrenRecursively(VisualElement element, StringTable table)
	{
		VisualElement.Hierarchy elementHierarchy = element.hierarchy;
		int numChildren = elementHierarchy.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			VisualElement child = elementHierarchy.ElementAt(i);
			Localize(child, table);
		}

		for (int i = 0; i < numChildren; i++)
		{
			VisualElement child = elementHierarchy.ElementAt(i);
			VisualElement.Hierarchy childHierarchy = child.hierarchy;
			int numGrandChildren = childHierarchy.childCount;
			if (numGrandChildren != 0)
				LocalizeChildrenRecursively(child, table);
		}
	}

	void Localize(VisualElement next, StringTable table)
	{
		if (_originalTexts.TryGetValue(next, out var text))
		{
			((TextElement)next).text = text;
		}

		if (typeof(TextElement).IsInstanceOfType(next))
		{
			TextElement textElement = (TextElement)next;
			string key = textElement.text;
			if (!string.IsNullOrEmpty(key) && key[0] == '#')
			{
				if (!_originalTexts.ContainsKey(textElement))
					_originalTexts.Add(textElement, textElement.text);

				key = key.TrimStart('#');
				StringTableEntry entry = table[key];
				if (entry != null)
					textElement.text = entry.LocalizedValue;
			}
		}
	}

	public void LocalizeManually(VisualElement element)
    {
		StringTable table = _table.GetTable();
		
		if (typeof(TextElement).IsInstanceOfType(element))
		{
			TextElement textElement = (TextElement)element;
			string key = textElement.text;
			if (!string.IsNullOrEmpty(key) && key[0] == '#')
			{
				if (!_originalTexts.ContainsKey(textElement))
					_originalTexts.Add(textElement, textElement.text);

				key = key.TrimStart('#');
				StringTableEntry entry = table[key];
				if (entry != null)
					textElement.text = entry.LocalizedValue;
			}
		}
	}
}


//// NOTE: this class assumes that you designate StringTable keys in label fields (as seen in Label, Button, etc)
//// and start them all with '#' char (so other labels will be left be)
//// example: https://i.imgur.com/H5RUIej.gif

//[HelpURL("https://gist.github.com/andrew-raphael-lukasik/72a4d3d14dd547a1d61ae9dc4c4513da")]
//[DisallowMultipleComponent]
//[RequireComponent(typeof(UIDocument))]
//public class UIDocumentLocalization : MonoBehaviour
//{

//    [SerializeField] LocalizedStringTable _table = null;
//    UIDocument _uiDocument;

//    /// <summary> Executed after hierarchy is cloned fresh and translated. </summary>
//    public event System.Action<VisualElement> onCompleted;


//    void OnEnable()
//    {
//        if (_uiDocument == null)
//            _uiDocument = GetComponent<UIDocument>();
//        _table.TableChanged += OnTableChanged;
//    }

//    void OnDisable()
//    {
//        _table.TableChanged -= OnTableChanged;
//    }


//    void OnTableChanged(StringTable table)
//    {
//        _uiDocument.rootVisualElement.Clear();
//        _uiDocument.visualTreeAsset.CloneTree(_uiDocument.rootVisualElement);

//        var op = _table.GetTableAsync();
//        if (op.IsDone)
//        {
//            OnTableLoaded(op);
//        }
//        else
//        {
//            op.Completed -= OnTableLoaded;
//            op.Completed += OnTableLoaded;
//        }
//    }

//    void OnTableLoaded(AsyncOperationHandle<StringTable> op)
//    {
//        StringTable table = op.Result;
//        LocalizeChildrenRecursively(_uiDocument.rootVisualElement, table);
//        _uiDocument.rootVisualElement.MarkDirtyRepaint();
//        onCompleted?.Invoke(_uiDocument.rootVisualElement);
//    }

//    void LocalizeChildrenRecursively(VisualElement element, StringTable table)
//    {
//        VisualElement.Hierarchy elementHierarchy = element.hierarchy;
//        int numChildren = elementHierarchy.childCount;
//        for (int i = 0; i < numChildren; i++)
//        {
//            VisualElement child = elementHierarchy.ElementAt(i);
//            Localize(child, table);
//        }
//        for (int i = 0; i < numChildren; i++)
//        {
//            VisualElement child = elementHierarchy.ElementAt(i);
//            VisualElement.Hierarchy childHierarchy = child.hierarchy;
//            int numGrandChildren = childHierarchy.childCount;
//            if (numGrandChildren != 0)
//                LocalizeChildrenRecursively(child, table);
//        }
//    }

//    void Localize(VisualElement next, StringTable table)
//    {
//        if (typeof(TextElement).IsInstanceOfType(next))
//        {
//            TextElement textElement = (TextElement)next;
//            string key = textElement.text;
//            if (!string.IsNullOrEmpty(key) && key[0] == '#')
//            {
//                key = key.TrimStart('#');
//                StringTableEntry entry = table[key];
//                if (entry != null)
//                    textElement.text = entry.LocalizedValue;
//                else
//                    Debug.LogWarning($"No {table.LocaleIdentifier.Code} translation for key: '{key}'");
//            }
//        }
//    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(UIDocumentLocalization))]
//    public class MyEditor : Editor
//    {
//        public override VisualElement CreateInspectorGUI()
//        {
//            var ROOT = new VisualElement();

//            var LABEL = new Label($"- Remember -<br> Use <color=\"yellow\">{nameof(onCompleted)}</color> event instead of <color=\"yellow\">OnEnable()</color><br>to localize and bind this document correctly.");
//            {
//                var style = LABEL.style;
//                style.minHeight = EditorGUIUtility.singleLineHeight * 3;
//                style.backgroundColor = new Color(1f, 0.121f, 0, 0.2f);
//                style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopLeftRadius = style.borderTopRightRadius = 6;
//                style.unityTextAlign = TextAnchor.MiddleCenter;
//            }
//            ROOT.Add(LABEL);

//            InspectorElement.FillDefaultInspector(ROOT, this.serializedObject, this);

//            return ROOT;
//        }
//    }
//#endif

//}