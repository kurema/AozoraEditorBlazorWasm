using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models.Projects
{
	public class AozoraProject
	{
		public List<IFileEntry> Files { get; } = new List<IFileEntry>();
		public Project.Project Project { get; set; } = new();

		public Notes.notes Notes { get; set; } = new();
	}

	public interface IFileEntry
	{
		string FileName { get; init; }
		string Text { get; }
	}
}
