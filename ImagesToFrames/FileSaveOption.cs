using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	class FileSaveOption : IControlMenuOption
	{
		private string name = "Save";
		private IFileManager fileManager;

		public FileSaveOption(IFileManager fileManager)
		{
			this.fileManager = fileManager;
		}

		public void Execute()
		{
			fileManager.Save();
		}

		public string GetName()
		{
			return name;
		}
	}
}
