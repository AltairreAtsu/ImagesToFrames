using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	class FileLoadOption : IControlMenuOption
	{
		private string name = "Open";
		private IFileManager fileManager;

		public FileLoadOption(IFileManager fileManager)
		{
			this.fileManager = fileManager;
		}

		public void Execute()
		{
			fileManager.Load();
		}

		public string GetName()
		{
			return name;
		}
	}
}
