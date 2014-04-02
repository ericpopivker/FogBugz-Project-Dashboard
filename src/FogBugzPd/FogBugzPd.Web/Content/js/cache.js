function clearCache(type, projectId, milestoneId, subProjectParentCaseId) {
    $("#loader").height($("#project-content").height()); clearCache
	if ($("#loader").height() == 0) $("#loader").height(200);
	$("#loader").show();

	$.post(resolveUrl("~/Home/ClearMsCache"),
	{
		type: type,
		projectId: projectId,
		milestoneId: milestoneId,
		subProjectParentCaseId: subProjectParentCaseId
	},
    function (result) {
    	if (result == 0) location.reload();
    }, "HTML");

}