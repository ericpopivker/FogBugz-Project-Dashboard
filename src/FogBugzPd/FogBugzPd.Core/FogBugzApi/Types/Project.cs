namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class Project
	{

		public int Id { get; set; }

		public string Name { get; set; }

		public string WikiPageId { get; set; }


		public Project(FogLampz.Model.Project parentProject)
		{
			Id = parentProject.Index.Value;
			Name = parentProject.Name;
		}
	}
}