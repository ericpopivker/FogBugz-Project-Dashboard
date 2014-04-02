var fogbugzpd = {
	web : {
		closeAlert: function () {

			$(".alert").animate({
				opacity: 0
			}, 150, function () { $(".alert").alert('close'); });

		}

	}
};
jQuery.extend(fogbugzpd.web, {
});

