using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	interface IFileManager
	{
		void NewFile();
		void Save();
		void SaveAs();
		void Load();
	}
}
