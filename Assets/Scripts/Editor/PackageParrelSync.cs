#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Game
{

    public static class PackageParrelSync
    {

        const string MANIFEST = "manifest.json";
        const string PACKAGES_LOCK = "packages-lock.json";

        [MenuItem("ParrelSync/PackageSync")]
        public static void Sync()
        {

            string dir = Environment.CurrentDirectory;
            string dstDir0 = Path.Combine($"{dir}_clone_0", "Packages");
            string dstDir1 = Path.Combine($"{dir}_clone_1", "Packages");
            string dstDir2 = Path.Combine($"{dir}_clone_2", "Packages");
            string dstDir3 = Path.Combine($"{dir}_clone_3", "Packages");

            // MANIFEST SYNC
            string src = Path.Combine(dir, "Packages", MANIFEST);
            SyncFile(src, dstDir0, MANIFEST);
            SyncFile(src, dstDir1, MANIFEST);
            SyncFile(src, dstDir2, MANIFEST);
            SyncFile(src, dstDir3, MANIFEST);
            Debug.Log("------------------------manifest.json sync completed.");

            // PACKAGESLOCK SYNC
            src = Path.Combine(dir, "Packages", PACKAGES_LOCK);
            SyncFile(src, dstDir0, PACKAGES_LOCK);
            SyncFile(src, dstDir1, PACKAGES_LOCK);
            SyncFile(src, dstDir2, PACKAGES_LOCK);
            SyncFile(src, dstDir3, PACKAGES_LOCK);
            Debug.Log("------------------------packages-lock.json sync completed.");
        }

        static void SyncFile(string src, string dstDir, string dstFileName)
        {
            if (Directory.Exists(dstDir))
            {
                string dstFilePath = Path.Combine(dstDir, dstFileName);
                if (File.Exists(dstFilePath)) File.Delete(dstFilePath);
                File.Copy(src, dstFilePath);
                Debug.Log(dstDir);
            }
        }
    }

}
#endif