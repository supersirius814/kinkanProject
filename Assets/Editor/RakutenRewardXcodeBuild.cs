using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build;
using UnityEditor.iOS.Xcode;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
#if UNITY_2018_1_OR_NEWER
public class RakutenRewardXcodeBuild : IPostprocessBuildWithReport
#elif UNITY_2017
public class RakutenRewardXcodeBuild : IPostprocessBuild
#else
public class RakutenRewardXcodeBuild
#endif
{
    public int callbackOrder { get { return 10; } }
#if UNITY_2017_1_OR_NEWER
    public void updateRewardSDKSetting(string path)
#else 
    public static void updateRewardSDKSetting(string path)
#endif
    {
        string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);
        string target = pbxProject.TargetGuidByName("Unity-iPhone");
        pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        pbxProject.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
        const string defaultLocationInProj = "Plugins/iOS";
        const string coreFrameworkName = "RakutenRewardSDK.framework";
        string framework = Path.Combine(defaultLocationInProj, coreFrameworkName);
        string fileGuid = pbxProject.AddFile(framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
#if UNITY_2017_1_OR_NEWER
        PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, target, fileGuid);
        pbxProject.SetBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");

#endif
        pbxProject.WriteToFile(projectPath);
    }
#if UNITY_2018_1_OR_NEWER
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("MyCustomBuildProcessor.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
        BuildTarget buildTarget = report.summary.platform;
        string path = report.summary.outputPath;
        if (buildTarget == BuildTarget.iOS)
        {
            updateRewardSDKSetting(path);
        }

    }
#elif UNITY_2017
    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        if (target.Equals(BuildTarget.iOS))
        {
            updateRewardSDKSetting(path);
        }
    }
#else
    [PostProcessBuild]
    public static void OnPostprocessBuild(UnityEditor.BuildTarget target, string path)
    {
        if (target.Equals(BuildTarget.iOS))
        {
            updateRewardSDKSetting(path);
        }
    }
#endif
}