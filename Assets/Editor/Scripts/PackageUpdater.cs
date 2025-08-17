using UnityEditor;
using UnityEngine.Events;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Erogemy.BlockBreaker.Editor
{
    public class PackageUpdater
    {
        public class PackageInfoSerializable {
            public string version;
        }

        UnityAction onUpdate;
        UnityAction onComplete;

        static RemoveRequest removeRequest;
        static AddRequest addRequest;

        const string packageRepository = "https://github.com//erogemy/BlockBreakerTemplate.git?path=/Assets/#main";

        public PackageUpdater(UnityAction onUpdate, UnityAction onComplete)
        {
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
        }

        public string GetPackageVersion()
        {
            // PackagePathのpackage.json直に読んじゃう
            var packageJsonPath = $"{EditorConsts.PackagePath}package.json";
            var jsonContent = AssetDatabase.LoadAssetAtPath<TextAsset>(packageJsonPath)?.text;
            var json = JsonUtility.FromJson<PackageInfoSerializable>(jsonContent);
            return json.version;
        }

        public void UpdatePackage()
        {
            onUpdate?.Invoke();
            removeRequest = Client.Remove(packageRepository);
            EditorApplication.update += RemoveProgress;
        }

        void RemoveProgress()
        {
            if (!removeRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= RemoveProgress;
            addRequest = Client.Add(packageRepository);
            EditorApplication.update += AddProgress;
        }

        void AddProgress()
        {
            if (!addRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= AddProgress;
            onComplete?.Invoke();
        }
    }
}
