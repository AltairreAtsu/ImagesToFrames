using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;


namespace ImagesToFrames
{
	class FrameGenerator
	{
		public static bool GenerateFramesFile(StandardFrame frame, string path)
		{
			JsonSerializer jsonSerializer = new JsonSerializer();
			jsonSerializer.Formatting = Formatting.Indented;
			jsonSerializer.NullValueHandling = NullValueHandling.Ignore;

			try
			{
				StreamWriter sw = new StreamWriter(path);
				JsonTextWriter jsonWriter = new JsonTextWriter(sw);

				jsonSerializer.Serialize(jsonWriter, frame);
				sw.Flush();
				sw.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static StandardFrame LoadFrameFromFile(string path)
		{
			try
			{
				string fileContents = File.ReadAllText(path);
				StandardFrame frame = JsonConvert.DeserializeObject<StandardFrame>(fileContents);
				return frame;
			}
			catch
			{
				return null;
			}
		}

		public static int[] LoadDimensionsFromImage(string imagePath)
		{
			try
			{
				Image img = Image.FromFile(imagePath);
				return new int[] { img.Width, img.Height };
			}
			catch
			{
				return null;
			}
		}
	}
}
