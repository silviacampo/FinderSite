using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.ViewModels
{
    public class UploadViewModel
    {
		public List<FilesDirectory> DownloadDirectories { get; set; }
		public int Type { get; set; }

		public IEnumerable<SelectListItem> Types { get; set; }

		public string Version { get; set; }

		public string FileName { get; set; }
	}

	public class FilesDirectory {
		public string DirectoryName { get; set; }
		public List<string> FileNames { get; set; }
		//public List<FilesDirectory> SubDirectories { get; set; }

		public FilesDirectory(){
			FileNames = new List<string>();
			//SubDirectories = new List<FilesDirectory>();
		}
	}
}
