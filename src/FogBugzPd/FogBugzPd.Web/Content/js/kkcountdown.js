/**
 * Plugin kkcountdown counts down to specific dates in the future.
 *
 * @example
 * $(".come-class").kkcountdown();
 *
 * @type jQuery
 *
 * @name kkcountdown
 * @author Krzysztof Furtak http://krzysztof-furtak.pl/
 * @version 1.3
 * 
 * Documentation: http://krzysztof-furtak.pl/2010/05/kk-countdown-jquery-plugin/
 * 
 */

(function ($) {
	$.fn.kkcountdown = function (options) {

		var defaults = {
			dayText: 'day ',
			daysText: 'days ',
			hoursText: ':',
			minutesText: ':',
			secondsText: '',
			textAfterCount: '---',
			oneDayClass: false,
			displayDays: true,
			displayZeroDays: true,
			addClass: false,
			callback: false,
			useLocalTimezone:false
		};

		var options = $.extend(defaults, options);

		this.each(function () {
			var _this = $(this);

			var box = $(document.createElement('span')).addClass('kkcountdown-box');
			var boxDni = $(document.createElement('span')).addClass('kkc-dni');
			var boxGodz = $(document.createElement('span')).addClass('kkc-godz');
			var boxMin = $(document.createElement('span')).addClass('kkc-min');
			var boxSec = $(document.createElement('span')).addClass('kkc-sec');
			var boxDniText = $(document.createElement('span')).addClass('kkc-dni-text');
			var boxGodzText = $(document.createElement('span')).addClass('kkc-godz-text');
			var boxMinText = $(document.createElement('span')).addClass('kkc-min-text');
			var boxSecText = $(document.createElement('span')).addClass('kkc-sec-text');

			if (options.addClass != false) {
				box.addClass(options.addClass);
			}

			box.append(boxDni).append(boxDniText).append(boxGodz).append(boxGodzText).append(boxMin).append(boxMinText).append(boxSec).append(boxSecText);
			_this.append(box);

			var now = new Date();

			//now.setHours(now.getUTCHours());
			if (options.useLocalTimezone)
				now = Math.round((((new Date()).getTime() - (new Date()).getTimezoneOffset() * 60 * 1000)) / 1000);
			else now = Math.floor(now.getTime() / 1000);
			var event = _this.attr('time');
			var count = event - now;

			kkCountdownInit(_this, count);

			boxGodzText.html(options.hoursText);
			boxMinText.html(options.minutesText);
			boxSecText.html(options.secondsText);
		});

		function kkCountdownInit(_this, count) {

			if (count <= 0) {
				_this.html(options.textAfterCount);
				if (options.callback) {
					options.callback();
				}
			} else if (count <= 24 * 60 * 60) {
				kkCountDown(true, _this, count);
				setTimeout(function () {
					kkCountDown(true, _this, count);
					kkCountdownInit(_this, count - 1);
				}, 1000);

			} else {
				kkCountDown(false, _this, count);
				setTimeout(function () {
					kkCountDown(false, _this, count);
					kkCountdownInit(_this, count - 1);
				}, 1000);
			}
		}

		function kkCountDown(oneDay, obj, count) {
			var sekundy = naprawaCzasu(count % 60);
			count = Math.floor(count / 60);
			var minuty = naprawaCzasu(count % 60);
			count = Math.floor(count / 60);
			var godziny = naprawaCzasu(count % 24);
			count = Math.floor(count / 24);
			var dni = count;

			if (oneDay && options.oneDayClass != false) {
				obj.addClass(options.oneDayClass);
			}

			if (dni == 0 && !options.displayZeroDays) {

			} else if (dni == 1) {
				obj.find('.kkc-dni').html(dni);
				obj.find('.kkc-dni-text').html(options.dayText);
			} else {
				obj.find('.kkc-dni').html(dni);
				obj.find('.kkc-dni-text').html(options.daysText);
			}

			obj.find('.kkc-godz').html(godziny);
			obj.find('.kkc-min').html(minuty);
			obj.find('.kkc-sec').html(sekundy);
		}

		function naprawaCzasu(obj) {
			s = '';
			if (obj < 10) {
				obj = '0' + obj;
			}
			return obj;
		}
	}
})(jQuery);