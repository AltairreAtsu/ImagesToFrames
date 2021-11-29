using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	class FileSaveAsOption : IControlMenuOption
	{
		private string name = "Save As";
		private IFileManager fileManager;
		
		public FileSaveAsOption(IFileManager fileManager)
		{
			this.fileManager = fileManager;
		}

		public void Execute()
		{
			fileManager.SaveAs();
		}

		public string GetName()
		{
			return name;
		}
	}
}
