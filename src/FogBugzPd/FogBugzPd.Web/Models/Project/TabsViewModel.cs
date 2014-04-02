using System.Web.Routing;

namespace FogBugzPd.Web.Models.Project
{
	public class TabsViewModel
	{
		public RouteValueDictionary RouteValues { get; set; }

		public void Setup(RouteData routeData)
		{
			var milestoneId = routeData.Values["milestoneId"];
			var projectId = routeData.Values["projectId"];
			var subProjectParentCaseId = routeData.Values["subProjectParentCaseId"];

			RouteValues = new RouteValueDictionary();

			RouteValues.Add("milestoneId", milestoneId);
			RouteValues.Add("projectId", projectId);
			
			if (subProjectParentCaseId != null)
				RouteValues.Add("subProjectParentCaseId", subProjectParentCaseId);

		}
	}
}
