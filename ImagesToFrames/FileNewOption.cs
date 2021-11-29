using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	class FileNewOption : IControlMenuOption
	{
		private string name = "New";
		private IFileManager fileManager;

		public FileNewOption(IFileManager fileManager)
		{
			this.fileManager = fileManager;
		}
		
		public void Execute()
		{
			fileManager.NewFile();
		}

		public string GetName()
		{
			return name;
		}
	}
}
