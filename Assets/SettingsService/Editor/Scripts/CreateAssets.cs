using UnityEditor;
using System.IO;

namespace CoreTeamGamesSDK.SettingsService.Editor
{
public class CreateSettingsValue
{
    [MenuItem(itemName: "Assets/Create/SettingsValue", isValidateFunction: false, priority: 51)]
    public static void CreateScriptFromTemplate()
    {
        string path = Directory.GetFiles(UnityEngine.Application.dataPath, "SettingsValueTemplate.cs.txt",SearchOption.AllDirectories)[0];
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(Path.GetFullPath(path), "New SettingsValue.cs");
    }
}

    public class CreateSettingsValueUIHandler
    {
        [MenuItem(itemName: "Assets/Create/SettingsValueUIHandler", isValidateFunction: false, priority: 51)]
        public static void CreateScriptFromTemplate()
        {
            string path = Directory.GetFiles(UnityEngine.Application.dataPath, "SettingsValueUIHandlerTemplate.cs.txt", SearchOption.AllDirectories)[0];
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(Path.GetFullPath(path), "New SettingsValueUIHandler.cs");
        }
    }
}