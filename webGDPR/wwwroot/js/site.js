// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
var domain = 'https://test.whereisfinder.com';
// Write your JavaScript code.
$(document).ready(function () {
    try {
        $('#rootwizard').bootstrapWizard({
            onNext: function (tab, navigation, index) {
                if (index == 1) {
                    // Make sure we entered the name
                    if (!$('#name').val()) {
                        alert('You must enter your name');
                        $('#name').focus();
                        return false;
                    }
                }

                // Set the name for the next tab
                $('#tab3').html('Hello, ' + $('#name').val());

            }, onTabShow: function (tab, navigation, index) {
                var $total = navigation.find('li').length;
                var $current = index + 1;
                var $percent = ($current / $total) * 100;
                $('#rootwizard .progress-bar').css({ width: $percent + '%' });
            }
        });
    }
    catch (err) {
        console.log(err);
    }

    var sheet = document.createElement('style'),
        $rangeInput = $('.range input'),
        prefs = ['webkit-slider-runnable-track', 'moz-range-track', 'ms-track'];

    document.body.appendChild(sheet);

    var getTrackStyle = function (el) {
        var curVal = el.value,
            val = 10 + (curVal - 1) * 20,
            style = '';

        // Set active label
        $('.range-labels li').removeClass('active selected');

        var curLabel = $('.range-labels').find('li:nth-child(' + curVal + ')');

        curLabel.addClass('active selected');
        curLabel.prevAll().addClass('selected');

        // Change background gradient
        for (var i = 0; i < prefs.length; i++) {
            style += '.range {background: linear-gradient(to right, #37adbf 0%, #37adbf ' + val + '%, #fff ' + val + '%, #fff 100%)}';
            style += '.range input::-' + prefs[i] + '{background: linear-gradient(to right, #37adbf 0%, #37adbf ' + val + '%, #b2b2b2 ' + val + '%, #b2b2b2 100%)}';
        }

        return style;
    }

    $rangeInput.on('input', function () {
        sheet.textContent = getTrackStyle(this);
    });

    // Change input value on label click
    $('.range-labels li').on('click', function () {
        var index = $(this).index();

        $rangeInput.val(index + 2).trigger('input');

    });

  var coll = document.getElementsByClassName("collapsible");
  var i;

  for (i = 0; i < coll.length; i++) {
    coll[i].addEventListener("click", function () {
      this.classList.toggle("collapsibleactive");
      var content = this.nextElementSibling;
      if (content.style.maxHeight) {
        content.style.maxHeight = null;
      } else {
        content.style.maxHeight = content.scrollHeight + "px";
      }
    });
  }

  $("#moreBtn").click(function () {
    var page = $(this).attr('data-page');
    $(this).attr('data-page', parseInt(page) + 1);
    $.ajax({
      url: '/User/ConnectionTimelineMore/',
      type: 'GET',
      data: { page: page},
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        for (var i = 0, l = data.length; i < l; i++) {

        }
      },
      error: function () {
        alert("error");
      }
    });
  });

});



//  var ctxSc = document.getElementById('scatterChart').getContext('2d');
//	var scatterData = {
//    datasets: [{
//    borderColor: 'rgba(99,0,125, .2)',
//  backgroundColor: 'rgba(99,0,125, .5)',
//  label: 'km walked',
//			data: [{
//    x: 0,
//  y: 1.711,
//			}, {
//    x: 1,
//  y: 2.708,
//			}, {
//    x: 2,
//  y: 4.285,
//			}, {
//    x: 3,
//  y: 6.772,
//			}, {
//    x: 4,
//  y: 1.068,
//			}, {
//    x: 5,
//  y: 1.681,
//			}, {
//    x: 6,
//  y: 2.635,
//			}, {
//    x: 7,
//  y:4.106,
//			}, {
//    x: 8,
//  y: 6.339,
//			}, {
//    x: 9,
//  y: 9.659,
//			}, {
//    x: 10,
//  y: 1.445,
//			}, {
//    x: 11,
//  y: 2.110,
//			}, {
//    x: 12,
//  y: 2.992,
//			}, {
//    x: 13,
//  y: 4.102,
//			}, {
//    x: 14,
//  y: 5.429,
//			}, {
//    x: 15,
//  y: 6.944,
//			}, {
//    x: 16,
//  y: 8.607,
//			}, {
//    x: 17,
//  y: 1.038,
//			}, {
//    x: 18,
//  y: 1.223,
//			}, {
//    x: 19,
//  y: 1.413,
//			}, {
//    x: 20,
//  y: 1.607,
//			}, {
//    x: 21,
//  y: 0,
//			}, {
//    x: 22,
//  y: 2.1,
//			}, {
//    x: 23,
//  y: 2.199,
//			}]
//}]
//}

//	var config1 = new Chart.Scatter(ctxSc, {
//    data: scatterData,
//		options: {
//    title: {
//    display: true,
//  text: 'Pet Activity'
//},
//			scales: {
//    xAxes: [{
//    type: 'linear',
//  position: 'bottom',
//					ticks: {
//    userCallback: function (tick) {
//								return tick.toString() + 'H';
//},
//},
//					scaleLabel: {
//    labelString: 'Day hours',
//  display: true,
//}
//}],
//				yAxes: [{
//    type: 'linear',
//					ticks: {
//    userCallback: function (tick) {
//							return tick.toString() + 'Km';
//}
//},
//					scaleLabel: {
//    labelString: 'Distance',
//  display: true
//}
//}]
//}
//}
//});



