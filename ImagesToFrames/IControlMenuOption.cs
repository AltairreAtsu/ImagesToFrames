using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToFrames
{
	public interface IControlMenuOption
	{
		string GetName();
		void Execute();
	}
}
