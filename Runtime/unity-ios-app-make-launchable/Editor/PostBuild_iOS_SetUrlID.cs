using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

namespace BeatThat.PostBuild.iOS
{
    /// <summary>
    /// Makes your Unity iOS app launchable via a url scheme. 
    /// By default, sets the url scheme to be the fully qualified product name, e.g. com.yourcompany.yourapp.
    /// 
    /// So if you're using, say, the App Launcher plugin from the Unity Asset store 
    /// (https://assetstore.unity.com/packages/tools/integration/app-launcher-20454)
    /// 
    /// ...then you would use this line of code to launch your app from another unity app:
    /// 
    /// AppLauncher.LaunchApp("com.yourcompany.yourapp://", "NameOfYourGameObjectForOnSuccessOnErrorCallbacks");
    /// </summary>
	public static class PostBuild_iOS_SetUrlID
	{
        [PostProcessBuild]
        public static void PostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
#if UNITY_EDITOR_OSX
                // Get plist
                string plistPath = pathToBuiltProject + "/Info.plist";
                var plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root
                PlistElementDict rootDict = plist.root;

                if (rootDict == null)
                {
                    Debug.LogError("ROOT DICT IS NULL");
                    return;
                }

                var id = rootDict["CFBundleIdentifier"].AsString();

                SetiOSUrlId(id, rootDict);

                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
            }
#endif
        }

#if UNITY_EDITOR_OSX
        public static void SetiOSUrlId(string urlId, PlistElementDict rootDict)
        {

            PlistElementArray urlTypesArray =
                rootDict["CFBundleURLTypes"] != null ? rootDict["CFBundleURLTypes"].AsArray()
                                                                                   : rootDict.CreateArray("CFBundleURLTypes");


            PlistElementDict urlTypes = urlTypesArray.AddDict();

            urlTypes.SetString("CFBundleTypeRole", "Editor");
            urlTypes.SetString("CFBundleURLName", urlId);

            PlistElementArray urlSchemes = urlTypes.CreateArray("CFBundleURLSchemes");
            urlSchemes.AddString(urlId);
		}
#endif
	}
}