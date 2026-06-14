using UnityEngine;

namespace SyntaxSultan.ComputerSystem.FileSystem
{
    [CreateAssetMenu(menuName = "PSSGame/Computer/Folder Icon Config")]
    public class FolderIconConfig : ScriptableObject
    {
        [System.Serializable]
        public struct FolderIconEntry
        {
            public string folderName;   // "Downloads", "Documents" vb. (case-insensitive)
            public Sprite icon;
        }

        public Sprite defaultFolderIcon;
        public Sprite defaultFileIcon;
        public Sprite driveIcon;
        public FolderIconEntry[] specialFolders;

        public Sprite GetFolderIcon(string folderName)
        {
            foreach (var entry in specialFolders)
                if (string.Equals(entry.folderName, folderName, System.StringComparison.OrdinalIgnoreCase))
                    return entry.icon;
            return defaultFolderIcon;
        }
    }
}