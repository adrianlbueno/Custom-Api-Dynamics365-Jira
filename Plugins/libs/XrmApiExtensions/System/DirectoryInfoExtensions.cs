using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Copies a Folder with all its subfolders and content to another location.
        /// </summary>
        /// <param name="source">The source folder</param>
        /// <param name="targetDirectory">The new destination</param>
        public static void CopyTo(this DirectoryInfo source, string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyTo(source, diTarget);
        }

        /// <summary>
        /// Copies a Folder with all its subfolders and content to another location.
        /// </summary>
        /// <param name="source">The source folder</param>
        /// <param name="target">The new destination</param>
        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyTo(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
