﻿namespace Mapbox.Unity.Editor
{

	using System;
	using System.IO;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Build;
	using System.Text;

	public class PreBuildChecksEditor : IPreprocessBuild
	{
		public int callbackOrder { get { return 0; } }
		public void OnPreprocessBuild(BuildTarget target, string path)
		{

			if (BuildTarget.Android != target)
			{
				return;
			}

			Debug.Log("Mapbox prebuild checks for target " + target + " at path " + path);

			List<LibInfo> libInfo = new List<LibInfo>();
			foreach (var file in Directory.GetFiles(Application.dataPath, "*.jar", SearchOption.AllDirectories))
			{
				libInfo.Add(new LibInfo(file));
			}
			foreach (var file in Directory.GetFiles(Application.dataPath, "*.aar", SearchOption.AllDirectories))
			{
				libInfo.Add(new LibInfo(file));
			}

			var stats = libInfo.GroupBy(li => li.BaseFileName).OrderBy(g => g.Key);

			StringBuilder sb = new StringBuilder();
			foreach (var s in stats)
			{
				if (s.Count() > 1)
				{
					sb.AppendLine(string.Format(
						"{0}:{1}{2}"
						, s.Key
						, Environment.NewLine
						, string.Join(Environment.NewLine, s.Select(li => "\t" + li.AssetPath).ToArray())
					));
				}
			}
			if (sb.Length > 0)
			{
				Debug.LogErrorFormat("DUPLICATE ANDROID PLUGINS FOUND - BUILD WILL MOST LIKELY FAIL!!!{0}Resolve to continue.{0}{1}", Environment.NewLine, sb);
			}
		}



		private class LibInfo
		{
			public LibInfo(string fullPath)
			{
				FullPath = fullPath;
				FullFileName = Path.GetFileName(fullPath);
				BaseFileName = FullFileName.Substring(0, FullFileName.LastIndexOf("-"));
				AssetPath = fullPath.Replace(Application.dataPath.Replace("Assets", ""), "");
			}

			public string FullPath { get; private set; }
			public string FullFileName { get; private set; }
			public string BaseFileName { get; private set; }
			public string AssetPath { get; private set; }
		}

	}
}