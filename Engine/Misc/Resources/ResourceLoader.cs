﻿using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using AGS.API;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public class ResourceLoader : IResourceLoader
	{
		private readonly Assembly _assembly;
		private Dictionary<string, List<string>> _folders;

		public ResourceLoader()
		{
			_assembly = Assembly.GetEntryAssembly();
		}

		public static string CustomAssemblyName;
		public static string AssetsFolder = "Assets";

		public IResource LoadResource(string path)
		{
			if (shouldIgnoreFile(path)) return null;

			string resourcePath = getResourceName(path);

			IResource resource = loadResource(resourcePath);

			if (resource != null)
			{
				return resource;
			}

			return loadFile(path);
		}

		public List<IResource> LoadResources(params string[] paths)
		{
			List<IResource> resources = new List<IResource> (paths.Length);
			foreach (string path in paths)
			{
				if (shouldIgnoreFile(path)) continue;

				IResource resource = LoadResource(path);
				if (resource == null)
				{
					Debug.WriteLine("Failed to load resource {0}.", path);
					continue;
				}
				resources.Add(resource);
			}
			return resources;
		}

		public List<IResource> LoadResources(string folder)
		{
			loadFolders();

			HashSet<Asset> assets = new HashSet<Asset> ();
			foreach (var file in Directory.GetFiles(folder))
			{
				if (shouldIgnoreFile(file)) continue;
				Asset asset = new Asset (getResourceName(file), file);
				assets.Add(asset);
			}

			string folderResource = getResourceName(folder);
			List<string> embeddedResources;
			if (_folders.TryGetValue(folderResource, out embeddedResources))
			{
				foreach (string resource in embeddedResources)
				{
					Asset asset = new Asset (resource, null);
					assets.Add(asset);
				}
			}
			List<IResource> resources = new List<IResource> (assets.Count);
			foreach (var asset in assets.OrderBy(r => r.ResourceName))
			{
				IResource resource = loadResource(asset.ResourceName);
				if (resource == null && asset.Filename != null)
					resource = loadFile(asset.Filename);
				if (resource == null)
				{
					Debug.WriteLine("Failed to load resource {0} from folder.", asset.ResourceName);
					continue;
				}
				resources.Add(resource);
			}
			return resources;
		}

		private bool shouldIgnoreFile(string path)
		{
			return path == null || 
				   path.EndsWith(".DS_Store"); //Mac OS file
		}

		private IResource loadResource(string resourceName)
		{
			Stream stream = _assembly.GetManifestResourceStream(resourceName);
			if (stream == null) return null;
			return new AGSResource (resourceName, stream);
		}

		private IResource loadFile(string path)
		{
			return new AGSResource(path, new FileStream (path, FileMode.Open, FileAccess.Read));
		}

		private string getResourceName(string path)
		{
			try
			{
				string assemblyName = CustomAssemblyName ?? _assembly.GetName().Name;
				string resourcePath = path.Substring(path.IndexOf(AssetsFolder));
				resourcePath = resourcePath.Replace('/', '.').Replace('\\', '.');
				resourcePath = string.Format("{0}.{1}", assemblyName, resourcePath);
				return resourcePath;
			}
			catch (Exception e)
			{
				throw new ArgumentException ("Invalid resource name: " + path ?? "null", e);
			}
		}

		private string getFolderName(string resource)
		{
			string folder = resource.Substring(0, resource.LastIndexOf('.'));
			folder = folder.Substring(0, folder.LastIndexOf('.'));
			return folder;
		}

		private void loadFolders()
		{
			if (Repeat.OnceOnly("LoadResourceFolders"))
			{
				_folders = new Dictionary<string, List<string>> (50);
				foreach (string name in _assembly.GetManifestResourceNames())
				{
					string folder = getFolderName(name);
					_folders.GetOrAdd(folder, () => new List<string> (20)).Add(name);
				}
			}
		}

		private class Asset : IEquatable<Asset>
		{
			public Asset(string resourceName, string filename)
			{
				ResourceName = resourceName;
				Filename = filename;
			}

			public string ResourceName { get; private set; }
			public string Filename { get; private set; }

			public override bool Equals(object obj)
			{
				return Equals(obj as Asset);
			}

			#region IEquatable implementation

			public bool Equals(Asset other)
			{
				if (other == null) return false;
				return ResourceName == other.ResourceName;
			}

			public override int GetHashCode()
			{
				return ResourceName.GetHashCode();
			}

			#endregion
		}
	}
}

