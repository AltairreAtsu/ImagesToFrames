using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ImagesToFrames
{
	class SettingsManager
	{
		public Settings settings;

		public bool LoadConfiguration()
		{
			string configFilePath = Path.Combine(Environment.CurrentDirectory, "config.json");
			if (!File.Exists(configFilePath))
			{
				settings = new Settings();
				string jsonString = JsonConvert.SerializeObject(settings);
				File.WriteAllText(configFilePath, jsonString);
				return true;
			}
			else
			{
				try
				{
					string jsonString = File.ReadAllText(configFilePath);
					settings = JsonConvert.DeserializeObject<Settings>(jsonString);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
	}

	public class Settings
	{
		public Settings()
		{
			defaultExportPath = "";
		}
		public String defaultExportPath { get; set; }
	}
}
