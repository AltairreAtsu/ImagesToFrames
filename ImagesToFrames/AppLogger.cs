using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	class AppLogger
	{
		FileStream outputStream;
		StreamWriter writer;
		TextWriter oldOut = Console.Out;
		public void BeginFileLogging()
		{
			string path = Path.Combine(Environment.CurrentDirectory, "log.txt");
			outputStream = new FileStream(path, FileMode.Create, FileAccess.Write);
			writer = new StreamWriter(outputStream);
			Console.SetOut(writer);
		}

		public void EndLogging()
		{
			Console.SetOut(oldOut);
			writer.Close();
			outputStream.Close();
		}
	}
}
