@using System.Linq
@using FogBugzPd.Core
@using FogBugzPd.Application.Extensions
@using FogBugzPd.Application.Utils
@using FogBugzPd.Core.ProjectStatus
@using FogBugzPd.Web.Utils
@{
	var keys = Model.Statuses.Keys.ToArray();

	var veryBehindStyle = "color: #ffffff;text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25);font-size: 13px;margin: 1px 0 1px 0;background-color: #FD5531;*background-color: #CC3300;background-image: -moz-linear-gradient(top, #FD5531, #CC3300);background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#FD5531), to(#CC3300));background-image: -webkit-linear-gradient(top, #FD5531, #CC3300);background-image: linear-gradient(to bottom, #FD5531, #CC3300);background-repeat: repeat-x;border-color: #FD5531 #CC3300 #387038;border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff62c462', endColorstr='#ff51a351', GradientType=0);filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);";
	var littleBehindStyle= "color: #ffffff;text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25);font-size: 13px;margin: 1px 0 1px 0;background-color: #FFA500;*background-color: #FF8800;background-image: -moz-linear-gradient(top, #FFA500 ,#FF8800);background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#FFA500), to(#FF8800));background-image: -webkit-linear-gradient(top, #FFA500, #FF8800);background-image: linear-gradient(to bottom, #FFA500, #FF8800);background-repeat: repeat-x;border-color: #FFA500 #FF8800 #387038;border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff62c462', endColorstr='#ff51a351', GradientType=0);filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);";
	var onTimeStyle= "color: #ffffff;font-size: 13px;margin: 1px 0 1px 0;background-color: #0BFA51;*background-color: #0BE651;background-image: -moz-linear-gradient(top, #0BFA51, #0BE651);background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#0BFA51), to(#0BE651));background-image: -webkit-linear-gradient(top, #0BFA51, #0BE651);background-image: linear-gradient(to bottom, #0BFA51, #0BE651);background-repeat: repeat-x;border-color: #0BE651 #0BFA51 #387038;border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);";
	var notstartedStyle = "color: #000000;font-size: 13px;margin: 1px 0 1px 0;background-color: #EEEEEE;*background-color: #DDDDDD;background-image: -moz-linear-gradient(top, #EEEEEE, #DDDDDD);background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#EEEEEE), to(#DDDDDD));background-image: -webkit-linear-gradient(top, #EEEEEE, #DDDDDD);background-color:#F5F5F5;background-image: linear-gradient(to bottom, #FFFFFF, #E6E6E6);;background-repeat: repeat-x;border-color: #EEEEEE #DDDDDD #387038;border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff62c462', endColorstr='#ff51a351', GradientType=0);filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);";
	var littleAheadStyle= "color: #ffffff;text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25);font-size: 13px;margin: 1px 0 1px 0;background-color: #58D658;*background-color: #33CC33;background-image: -moz-linear-gradient(top, #58D658, #33CC33);background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#58D658), to(#33CC33));background-image: -webkit-linear-gradient(top, #58D658, #33CC33);background-image: linear-gradient(to bottom, #58D658, #33CC33);background-repeat: repeat-x;border-color: #58D658 #33CC33 #387038;border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff62c462', endColorstr='#ff51a351', GradientType=0);filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);";
	var veryAheadStyle = "color: #ffffff;text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25);font-size: 13px;margin: 1px 0 1px 0;background-color: #00B000;*background-color: #008C00;background-image: -moz-linear-gradient(top, #00B000, #008C00);background-image: -webkit-gradient(linear, 0 0, 0 100%, from(#00B000), to(#008C00));background-image: -webkit-linear-gradient(top, #00B000, #008C00);background-image: linear-gradient(to bottom, #00B000, #008C00);background-repeat: repeat-x;border-color: #00B000 #008C00 #387038;border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff62c462', endColorstr='#ff51a351', GradientType=0);filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);";

	var btnStyle= "-moz-border-bottom-colors: none;-moz-border-left-colors: none;-moz-border-right-colors: none;-moz-border-top-colors: none;background-repeat: repeat-x; border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) #A2A2A2;border-image: none;border-radius: 4px;border-style: solid;border-width: 1px;box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);cursor: pointer;display: inline-block;font-size: 14px;line-height: 20px;margin-bottom: 0;padding: 4px 12px;text-align: center; vertical-align: middle;";

	Func<FogbugzProjectStatus, string> statusStyleFunc = delegate(FogbugzProjectStatus status) 
		{
			switch(status)
			{
				 case FogbugzProjectStatus.ALittleAhead:
					return littleAheadStyle;
				case FogbugzProjectStatus.LittleBehind:
					return littleBehindStyle;
				case FogbugzProjectStatus.NotStarted:
					return notstartedStyle;
				case FogbugzProjectStatus.OnTime:
					return onTimeStyle;
				case FogbugzProjectStatus.VeryAhead:
					return veryAheadStyle;
				case FogbugzProjectStatus.VeryBehind:
					return veryBehindStyle;
			}

			return notstartedStyle;
		};
}
<table class="table" style="width: 100%; border-collapse: collapse;">
	<tr>
		<th style="width: 25%; text-align:left;">Project</th>
		<th style="width: 70%; text-align:left;">
			@if (Model.ShowSubProjects)
			{
				<table class="table" style="width:900px;">
					<tr>
						<td style="width: 50%; text-align:left;">Milestone</td>
						<td style="width: 50%; text-align:left;">Sub-Project</td>
					</tr>
				</table>
			}
			else
			{
				<div style="padding-bottom: 10px;">Milestone</div>
			}
		</th>
	</tr>

	@foreach (var item in Model.ProjectMilestoneList.Items)
	{
		<tr style="border-bottom: solid lightgray 1px;vertical-align:top;">
			<td>
				@item.Project.Name
			</td>
			<td style="padding-top: 0">
				@if (Model.ShowSubProjects)
				{
					<table class="table">
						@foreach (var milestoneListItem in item.MilestoneListItems)
						{
							var statusKey = keys.First(k=>k.MileStoneId == milestoneListItem.Milestone.Index && k.ProjectId == item.Project.Index && k.SubProjectParentCaseId == null);
							var status = Model.Statuses[statusKey];
							var milestoneUrl = @EnvironmentConfig.GetFrontEndWebRootUrl() + "/Project/Dashboard/" + @item.Project.Index + "/" + @milestoneListItem.Milestone.Index;
							
							<tr>
								<td style="width: 50%;">
									<a href=@milestoneUrl style="color: #0088cc;text-decoration: none">@milestoneListItem.Milestone.Name</a>
									<span style="@btnStyle @statusStyleFunc(status.Status)" onclick="location.href=@milestoneUrl">
										@status.Status.GetStringValue()
										@if (status.Status != FogbugzProjectStatus.NotStarted)
										{
											@:( @FogBugzPd.Application.Utils.StringUtils.FormatSignDecimal(status.StatusValue, 1) )
										}

									</span>
									<br />
								</td>
								<td style="width: 50%;">
									@foreach (var subProjectParentCase in milestoneListItem.SubProjectParentCases)
									{
										var subProjectStatusKey = keys.Single(k=>k.MileStoneId == milestoneListItem.Milestone.Index && k.ProjectId == item.Project.Index && k.SubProjectParentCaseId == subProjectParentCase.Index);
										var subProjectstatus = Model.Statuses[subProjectStatusKey];
										var subProjectUrl = @EnvironmentConfig.GetFrontEndWebRootUrl() + "/Project/Dashboard/" + @item.Project.Index + "/" + @milestoneListItem.Milestone.Index + "/" + @subProjectParentCase.Index;

										<a href=@subProjectUrl style="color: #0088cc;text-decoration: none">@subProjectParentCase.Title</a>
										<span style="@btnStyle @statusStyleFunc(subProjectstatus.Status)" onclick="location.href=@subProjectUrl">
										@subProjectstatus.Status.GetStringValue()
										@if (subProjectstatus.Status != FogbugzProjectStatus.NotStarted)
										{
											@:( @FogBugzPd.Application.Utils.StringUtils.FormatSignDecimal(subProjectstatus.StatusValue, 1) )
										}

									</span>
										<br />
									}
								</td>
							</tr>

						}
					</table>
				}
			else
			{
				foreach (var milestoneListItem in item.MilestoneListItems)
				{
					var statusKey = keys.Single(k=>k.MileStoneId == milestoneListItem.Milestone.Index && k.ProjectId == item.Project.Index);
					var status = Model.Statuses[statusKey];
					var url = @EnvironmentConfig.GetFrontEndWebRootUrl() + "/Project/Dashboard/" + @item.Project.Index + "/" + @milestoneListItem.Milestone.Index;

					<a href=@url style="color: #0088cc;text-decoration: none">@milestoneListItem.Milestone.Name
					<span style="@statusStyleFunc(status.Status) @btnStyle">
						@status.Status.GetStringValue()
						@if (status.Status != FogbugzProjectStatus.NotStarted)
						{
							@:( @FogBugzPd.Application.Utils.StringUtils.FormatSignDecimal(status.StatusValue, 1) )
						}
					</span>
					</a>
					<br />
				}
					<br />
			}
			</td>
		</tr>
	}
</table>