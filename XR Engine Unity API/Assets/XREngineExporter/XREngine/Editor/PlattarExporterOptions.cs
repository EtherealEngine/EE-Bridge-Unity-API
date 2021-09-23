#if UNITY_EDITOR

using System;
using System.IO;
using Ionic.Zip;

namespace XREngine {
	public class XREngineExporterOptions {
		public static readonly string ZipExtension = ".zip";
		public static bool IsZip = true;
		public static string LastEditorPath = "";
		public static bool ExportAnimations = true;

		public static void CompressDirectory(string source, string destination) {
			// delete any files that exist
			XREngineExporterOptions.DeleteFile(destination + ZipExtension);
			// Compress a folder.
			using(ZipFile zip = new ZipFile()) {
				zip.AddDirectory(source);
				zip.Save(destination + ZipExtension);
			}
		}

		public static void DeleteFile(string target_file) {
			if (File.Exists(target_file)) {
				File.SetAttributes(target_file, FileAttributes.Normal);
				File.Delete(target_file);
			}
		}

		public static void DeleteDirectory(string target_dir) {
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			foreach (string file in files) {
				XREngineExporterOptions.DeleteFile(file);
			}

			foreach (string dir in dirs) {
				DeleteDirectory(dir);
			}

			Directory.Delete(target_dir, false);
		}
	}
}

#endif