using System.IO;
using UnityEditor;

namespace Project.Editor
{
	/**
	 * A Unity Editor tool for toggling the visibility of samples directories in UPM packages.
	 * Note the reason for using System.IO rather than AssetDatabase to move directories is
	 * because Unity generates a .meta file for the Samples~ directory which will throw a warning
	 * when the published package is installed.  When renaming the Samples directory to Samples~
	 * Unity seems to have cached the Samples directory and Samples.meta so they reappear which is why
	 * I've added a call to delete them after renaming to Samples~
	 */
	public static class SampleFolderUtility
	{
		private const string SampleFolderName = "Samples";
		private const string HiddenSampleFolderName = "Samples~";
		private const string PackagePath = "Assets/Package";
		private const string MetaExt = ".meta";

		[MenuItem("Tools/Toggle Sample Folder Visibility")]
		public static void ToggleSampleFolderVisibility()
		{
			var sampleDirPath = Path.Combine(PackagePath, SampleFolderName);
			var hiddenDirPath = Path.Combine(PackagePath, HiddenSampleFolderName);
			var sampleMetaPath = sampleDirPath + MetaExt;
			
			if (Directory.Exists(sampleDirPath))
			{
				Directory.Move(sampleDirPath, hiddenDirPath);
				if (File.Exists(sampleMetaPath)) File.Delete(sampleMetaPath);
				if (File.Exists(sampleDirPath)) File.Delete(sampleDirPath);
				EditorApplication.delayCall += AssetDatabase.Refresh;
			}
			else if (Directory.Exists(hiddenDirPath))
			{
				if (File.Exists(sampleMetaPath)) File.Delete(sampleMetaPath);
				EditorApplication.delayCall += MoveHiddenDir;
			}
		}

		private static void MoveHiddenDir()
		{
			var sampleDirPath = Path.Combine(PackagePath, SampleFolderName);
			var hiddenDirPath = Path.Combine(PackagePath, HiddenSampleFolderName);
			
			if (Directory.Exists(hiddenDirPath))
			{
				Directory.Move(hiddenDirPath, sampleDirPath);
			}
			
			EditorApplication.delayCall += AssetDatabase.Refresh;
		}
	}
}