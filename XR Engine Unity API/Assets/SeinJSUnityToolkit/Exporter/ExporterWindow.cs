﻿/**
 * @File   : ExportorWindow.cs
 * @Author : dtysky (dtysky@outlook.com)
 * @Link   : dtysky.moe
 * @Date   : 2019/09/09 0:00:00PM
 */
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace SeinJS
{

    public class ExporterWindow : EditorWindow
    {

        [MenuItem("SeinJS/Export to GlTF", priority = 0)]
        static void Init()
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            ExporterWindow window = (ExporterWindow)GetWindow(typeof(ExporterWindow));
            window.titleContent.text = "Export to GlTF";
            window.Show();
#else // and error dialog if not standalone
		    EditorUtility.DisplayDialog("Error", "Your build target must be set to standalone", "Okay");
#endif
        }

        public static GameObject tempGoForSein = null;

        // UI dimensions (to be cleaned)
        [SerializeField]
        Vector2 fullSize = new Vector2(520, 580);

        // Exporter UI: static elements
        int SPACE_SIZE = 10;
        Vector2 scrollPos;

        private Exporter _exporter;

        void OnFocus()
        {
            if (_exporter == null)
            {
                _exporter = new Exporter();
            }

            Config.Load();
            if (!Utils.inited)
            {
                Utils.Init();
            }

            ExporterSettings.Export.UpdateFolder(Config.GetExportPath());

            if (!Directory.Exists(ExporterSettings.Export.folder))
            {
                Directory.CreateDirectory(ExporterSettings.Export.folder);
            }

            ExtensionManager.Init();
        }

        void OnEnable()
        {
            // Pre-fill model name with scene name if empty
            if (ExporterSettings.Export.name.Length == 0)
            {
                ExporterSettings.Export.name = EditorSceneManager.GetActiveScene().name;
            }
        }

        // Update is called once per frame
        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            
            GUILayout.Label(Config.header);
            GUILayout.Label("Version: " + Config.Version, EditorStyles.boldLabel);

            GUILayout.Space(SPACE_SIZE);

            GUILayout.Label("Export Settings", EditorStyles.boldLabel);

            GUILayout.Label("Name;");
            ExporterSettings.Export.name = EditorGUILayout.TextField(ExporterSettings.Export.name);
            GUILayout.Space(SPACE_SIZE);

            GUILayout.Label("Folder");
            GUILayout.BeginHorizontal();
            GUILayout.TextField(ExporterSettings.Export.folder, GUILayout.MinWidth(350), GUILayout.Height(21));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Select Folder", GUILayout.Height(21), GUILayout.Width(150)))
            {
                var tmp = EditorUtility.OpenFolderPanel("Choose a folder", ExporterSettings.Export.folder, "");
                if (tmp != "")
                {
                    ExporterSettings.Export.UpdateFolder(tmp);
                }
            }
            GUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(fullSize.y - 330));

            GUILayout.BeginHorizontal();
            ExporterSettings.Export.splitChunks = EditorGUILayout.Toggle("Split chunks", ExporterSettings.Export.splitChunks);
            GUILayout.FlexibleSpace();
            ExporterSettings.Export.clear = EditorGUILayout.Toggle("Clear folder", ExporterSettings.Export.clear);
            GUILayout.FlexibleSpace();
            ExporterSettings.Export.checkEmpty = EditorGUILayout.Toggle("Check empty", ExporterSettings.Export.checkEmpty);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ExporterSettings.Export.skybox = EditorGUILayout.Toggle("Export skybox", ExporterSettings.Export.skybox);
            GUILayout.FlexibleSpace();
            ExporterSettings.Export.noVertexColor = EditorGUILayout.Toggle("No VertexColor", ExporterSettings.Export.noVertexColor);
            GUILayout.FlexibleSpace();
            ExporterSettings.Export.unlit = EditorGUILayout.Toggle("Unlit(No Normals)", ExporterSettings.Export.unlit);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 240;
            ExporterSettings.Animation.forceLinear = EditorGUILayout.Toggle("Force Linear Interpolation", ExporterSettings.Animation.forceLinear);

            GUILayout.Space(12);
            GUILayout.Label("Normal Texture Settings", EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 200;
            ExporterSettings.NormalTexture.opaqueType = (ENormalTextureType)EditorGUILayout.EnumPopup("Opaque image type", ExporterSettings.NormalTexture.opaqueType);
            ExporterSettings.NormalTexture.transparentType = (ENormalTextureType)EditorGUILayout.EnumPopup("Transparent image type", ExporterSettings.NormalTexture.transparentType);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture max size");
            GUILayout.FlexibleSpace();
            ExporterSettings.NormalTexture.maxSize = EditorGUILayout.IntField(ExporterSettings.NormalTexture.maxSize);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture jpg quality(JPG)");
            GUILayout.FlexibleSpace();
            ExporterSettings.NormalTexture.jpgQulity = EditorGUILayout.IntSlider(ExporterSettings.NormalTexture.jpgQulity, 0, 100);
            GUILayout.EndHorizontal();
            ExporterSettings.NormalTexture.pngFormat = (EPNGTextureFormat)EditorGUILayout.EnumPopup("Texture png format(PNG)", ExporterSettings.NormalTexture.pngFormat);

            GUILayout.Space(12);
            GUILayout.Label("HDR Settings(HDR)", EditorStyles.boldLabel);
            ExporterSettings.HDR.type = (EHDRTextureType)EditorGUILayout.EnumPopup("HDR", ExporterSettings.HDR.type);

            GUILayout.Space(12);
            GUILayout.Label("CubeTexture Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture max size");
            GUILayout.FlexibleSpace();
            ExporterSettings.CubeTexture.maxSize = EditorGUILayout.IntField(ExporterSettings.CubeTexture.maxSize);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUILayout.Label("Lighting Settings", EditorStyles.boldLabel);
            ExporterSettings.Lighting.ambient = EditorGUILayout.Toggle("Export ambient light", ExporterSettings.Lighting.ambient);

            ExporterSettings.Lighting.lightMap = EditorGUILayout.Toggle("Export light map", ExporterSettings.Lighting.lightMap);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Light map max size");
            GUILayout.FlexibleSpace();
            ExporterSettings.Lighting.lightMapSize = EditorGUILayout.IntField(ExporterSettings.Lighting.lightMapSize);
            GUILayout.EndHorizontal();

            ExporterSettings.Lighting.reflection = EditorGUILayout.Toggle("Export reflection", ExporterSettings.Lighting.reflection);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Reflection map max size");
            GUILayout.FlexibleSpace();
            ExporterSettings.Lighting.reflectionSize = EditorGUILayout.IntField(ExporterSettings.Lighting.reflectionSize);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview", GUILayout.Width(240), GUILayout.Height(40)))
            {
                var origFolder = ExporterSettings.Export.folder;
                var origSplit = ExporterSettings.Export.splitChunks;
                var origName = ExporterSettings.Export.name;
                ExporterSettings.Export.UpdateFolderTemp("../sein-previewer-temp-assets");
                ExporterSettings.Export.splitChunks = false;
                ExporterSettings.Export.name = "scene";
                var folder = ExporterSettings.Export.folder;

                try
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    DirectoryInfo directory = new DirectoryInfo(folder);
                    var files = directory.GetFiles();
                    var subDirectories = directory.GetDirectories();

                    StartExport(files, subDirectories, false);

                    Previewer.StartPreview();
                }
                finally
                {
                    ExporterSettings.Export.UpdateFolderTemp(origFolder);
                    ExporterSettings.Export.splitChunks = origSplit;
                    ExporterSettings.Export.name = origName;
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export", GUILayout.Width(240), GUILayout.Height(40)))
            {
                var folder = ExporterSettings.Export.folder;
                if (!Directory.Exists(folder))
                {
                    EditorUtility.DisplayDialog("Error", "Folder for exporting is not existed: \"" + folder + "\"", "OK");
                    return;
                }

                DirectoryInfo directory = new DirectoryInfo(folder);
                var files = directory.GetFiles();
                var subDirectories = directory.GetDirectories();

                if (!IsEmpty())
                {
                    if (EditorUtility.DisplayDialog(
                        "This folder is not empty",
                        "If you export gltf here, all files and sub directories will be deleted !",
                        "Continue",
                        "Cancel"
                    ))
                    {
                        StartExport(files, subDirectories);
                    }

                    return;
                }

                StartExport(files, subDirectories);
            }

            GUILayout.EndHorizontal();
        }

        bool IsEmpty()
        {
            var folder = ExporterSettings.Export.folder;
            if (
                !ExporterSettings.Export.checkEmpty
                && Config.PathIsInProject(folder)
            )
            {
                return true;
            }

            DirectoryInfo directory = new DirectoryInfo(folder);
            var files = directory.GetFiles();
            var subDirectories = directory.GetDirectories();

            return Utils.MakeRelativePath(Config.AppDataPath, folder) == Config.DefaultExportFolder || !(files.Length > 0 || subDirectories.Length > 0);

        }
        void StartExport(FileInfo[] files, DirectoryInfo[] subDirectories, bool openFolder = true)
        {
            if (ExporterSettings.Export.clear || ExporterSettings.Export.folder == Config.DefaultExportFolder)
            {
                //delete files:
                foreach (FileInfo file in files)
                    file.Delete();
                //delete directories in this directory:
                foreach (DirectoryInfo subDirectory in subDirectories)
                    subDirectory.Delete(true);
            }

            _exporter.Export();

            if (openFolder)
            {
                OpenInFileBrowser.Open(Path.GetDirectoryName(ExporterSettings.Export.GetExportPath()));
            }
        }

        void OnDestroy()
        {
            _exporter = null;
            Config.Save();
        }

        void OnLostFocus()
        {
            Config.Save();
        }
    }
}

#endif
