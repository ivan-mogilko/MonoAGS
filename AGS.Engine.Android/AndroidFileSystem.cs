﻿using System;
using System.Collections.Generic;
using System.IO;

namespace AGS.Engine.Android
{
	public class AndroidFileSystem : IFileSystem
	{
		#region IFileSystem implementation

		public IEnumerable<string> GetFiles(string folder)
		{
			return Directory.GetFiles(folder);
		}

		public Stream Open(string path)
		{
			return File.OpenRead(path);
		}

		public Stream Create(string path)
		{
			return File.Create(path);
		}

		#endregion
	}
}

