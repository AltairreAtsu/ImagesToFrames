using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	class StandardFrame
	{
		public FrameGrid frameGrid { get; set; }
		public Dictionary<String, String> aliases { get; set; }

		public StandardFrame()
		{
			frameGrid = new FrameGrid();
		}

	}

	class FrameGrid
	{
		public int[] size { get; set; }
		public int[] dimensions { get; set; }
		public string[,] names { get; set; }

		public FrameGrid()
		{
			size = new int[2] { 0, 0 };
			dimensions = new int[2] { 1, 1 };
			names = new string[1, 1] { { "default"} };
		}
		public FrameGrid(int[] size, int[] dimensions, string[,] names)
		{
			this.size = size;
			this.dimensions = dimensions;
			this.names = names;
		}
	}
}
