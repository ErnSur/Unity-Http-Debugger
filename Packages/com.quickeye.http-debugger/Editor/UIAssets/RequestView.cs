using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuickEye.UIToolkit;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace QuickEye.WebTools.Editor
{
    internal partial class RequestView
    {
        private readonly VisualElement _root;
        private RequestData _target;
        private readonly HeadersView _headersViewController;
        private readonly Dictionary<string, (string filePath, int line)> _linksByLinkText = new();

        public RequestView(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            _headersViewController = new HeadersView(headersView);
            stackTraceView.RegisterCallback<PointerUpLinkTagEvent>(evt =>
            {
                var (filePath, line) = _linksByLinkText[evt.linkText];
                InternalEditorUtility.OpenFileAtLineExternal(filePath, line);
            });
            stackTraceLabel.selection.isSelectable = true;
            InitTabs();
        }

        public void ToggleReadOnlyMode(bool value)
        {
            foreach (var textField in _root.Query<TextField>().Build())
            {
                textField.isReadOnly = value;
            }

            _headersViewController.ToggleReadOnlyMode(value);
        }

        public void Setup(SerializedObject serializedObject)
        {
            if (serializedObject is null)
                return;
            _target = (RequestData)serializedObject.targetObject;
            reqBodyField.BindProperty(serializedObject.FindProperty(nameof(RequestData.body)));
            _headersViewController.Setup(serializedObject.FindProperty(nameof(RequestData.headers)));
            if (_target is ConsoleRequestData consoleRequestData)
            {
                stackTraceTab.ToggleDisplayStyle(true);
                var stackTrace = StacktraceWithHyperlinks(consoleRequestData.stackTrace);
                stackTraceLabel.text = stackTrace;
            }
            else
            {
                stackTraceTab.ToggleDisplayStyle(false);
            }
        }

        private void InitTabs()
        {
            bodyTab.TabContent = reqBodyField;
            headersTab.TabContent = headersView;
            stackTraceTab.TabContent = stackTraceView;
            authTab.BeforeMenuShow += menu =>
            {
                menu.AddItem("Basic Auth", false, null);
                menu.AddItem("Digest Auth", false, null);
                menu.AddItem("OAuth 1.0", false, null);
                menu.AddItem("OAuth 2.0", false, null);
                menu.AddItem("Bearer Token", false, null);
                menu.AddSeparator("");
                menu.AddItem("No Authentication", true, null);
            };

            bodyTab.value = true;
        }


        // TODO: make it nicer to look at?
        private string StacktraceWithHyperlinks(string stacktraceText)
        {
            _linksByLinkText.Clear();
            var textWithHyperlinks = new StringBuilder();
            textWithHyperlinks.Append(stacktraceText);
            var lines = stacktraceText.Split(new[] { "\n" }, StringSplitOptions.None);
            for (var i = 0; i < lines.Length; ++i)
            {
                const string textBeforeFilePath = ") (at ";
                var filePathIndex = lines[i].IndexOf(textBeforeFilePath, StringComparison.Ordinal);
                if (filePathIndex > 0)
                {
                    filePathIndex += textBeforeFilePath.Length;
                    // sometimes no url is given, just an id between <>, we can't do an hyperlink
                    if (lines[i][filePathIndex] != '<')
                    {
                        var filePathPart = lines[i].Substring(filePathIndex);
                        var lineIndex =
                            filePathPart.LastIndexOf(":",
                                StringComparison.Ordinal); // LastIndex because the url can contain ':' ex:"C:"
                        if (lineIndex > 0)
                        {
                            var endLineIndex =
                                filePathPart.LastIndexOf(")",
                                    StringComparison
                                        .Ordinal); // LastIndex because files or folder in the url can contain ')'
                            if (endLineIndex > 0)
                            {
                                var lineString =
                                    filePathPart.Substring(lineIndex + 1, (endLineIndex) - (lineIndex + 1));
                                var filePath = filePathPart.Substring(0, lineIndex);
                                var linkText = $"{filePath}:{lineString}";
                                _linksByLinkText[linkText] = (filePath, int.Parse(lineString));
                                textWithHyperlinks.Append(lines[i][..filePathIndex]);
                                textWithHyperlinks.Append($"<link=\"{filePath}\"><color=#40a0ff><u>");
                                textWithHyperlinks.Append(linkText);
                                textWithHyperlinks.Append("</u></color></link>)\n");

                                continue; // continue to evade the default case
                            }
                        }
                    }
                }

                // default case if no hyperlink : we just write the line
                textWithHyperlinks.Append(lines[i] + "\n");
            }

            // Remove the last \n
            if (textWithHyperlinks.Length > 0) // textWithHyperlinks always ends with \n if it is not empty
                textWithHyperlinks.Remove(textWithHyperlinks.Length - 1, 1);

            return textWithHyperlinks.ToString();
        }
    }
}