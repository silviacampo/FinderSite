// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {

  try {
    CKEDITOR.replace("FullDescription");
  }
  catch (err) {
    console.log(err);
  }

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

  try {
    if ($('.card-map').length > 0) {
      var map1 = $('#card-map');
      if (map1.data('geoLat') && map1.data('geoLng')) {
        var deviceLocation = {
          lat: map1.data('geoLat'),
          lng: map1.data('geoLng')
        };

        var map = new google.maps.Map(document.getElementById('card-map'), {
          zoom: 17,
          center: deviceLocation,
          mapTypeId: 'terrain'
        });

        var infowindow = new google.maps.InfoWindow({
          content: contentString
        });

        var panorama = new google.maps.StreetViewPanorama(
          document.getElementById('pano'), {
            position: deviceLocation,
            pov: {
              heading: 34,
              pitch: 10
            }
          });
        map.setStreetView(panorama);

        var lineSymbol = {
          path: google.maps.SymbolPath.FORWARD_OPEN_ARROW
        };

        var lineSymbolCat = {
          path: google.maps.SymbolPath.CIRCLE,
          scale: 4,
          strokeColor: '#FF0000'
        };

        var path = new google.maps.Polyline({
          path: coordinates.concat([deviceLocation]),
          geodesic: true,
          icons: [{
            icon: lineSymbolCat,
            offset: '100%'
          },
          {
            icon: lineSymbol,
            offset: '30%',
            repeat: '50px'
          }],
          strokeColor: '#000000',
          strokeOpacity: 1.0,
          strokeWeight: 2
        });

        path.setMap(map);

        //Markers and MarkersCluster
        //http://www.lass.it/Web/viewer.aspx?id=4
        var flag = {
          url: 'https://test.whereisfinder.com/images/blue.png',
          // This marker is 20 pixels wide by 32 pixels high.
          size: new google.maps.Size(32, 25)
        };

        var markers = coordinates.map(function (location, i) {
          return new google.maps.Marker({
            position: location,
            label: i.toString(),
            icon: flag
          });
        });

        // Add a marker clusterer to manage the markers.
        var markerCluster = new MarkerClusterer(map, markers,
          { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

        var marker = new google.maps.Marker({
          position: deviceLocation,
          map: map
        });

        marker.addListener('click', function () {
          infowindow.open(map, marker);
        });
      }
    }
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


//bar
function initActivityChart() {
  var ctxB = document.getElementById("activityChart").getContext('2d');
  var myBarChart = new Chart(ctxB, {
    type: 'bar',
    data: {
      labels: ["00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"],
      datasets: [{
        label: 'kms walked',
        data: petActivity,
        backgroundColor: [
          'rgba(153, 102, 255, 0.2)', //0 Black
          'rgba(180, 101, 225, 0.2)', //1
          'rgba(205, 100, 195, 0.2)', //2
          'rgba(230, 100, 165, 0.2)', //3
          'rgba(255, 99, 132, 0.2)', //4 Red
          'rgba(255, 114, 115, 0.2)', //5
          'rgba(255, 129, 98, 0.2)', //6
          'rgba(255, 144, 81, 0.2)', //7
          'rgba(255, 159, 64, 0.2)', //8 Orange
          'rgba(255, 171, 69, 0.2)', //9
          'rgba(255, 182, 75, 0.2)', //10
          'rgba(255, 195, 81, 0.2)', //11
          'rgba(255, 206, 86, 0.2)', //12 Yellow
          'rgba(210, 202, 112, 0.2)', //13
          'rgba(165, 199, 139, 0.2)', //14
          'rgba(120, 195, 165, 0.2)', //15
          'rgba(75, 192, 192, 0.2)', //16 Green
          'rgba(70, 185, 203, 0.2)', //17
          'rgba(65, 177, 213, 0.2)', //18
          'rgba(60, 170, 224, 0.2)', //19
          'rgba(54, 162, 235, 0.2)', //20 Blue
          'rgba(79, 147, 240, 0.2)', //21
          'rgba(104, 132, 245, 0.2)', //22
          'rgba(128, 117, 250, 0.2)' //23
        ],
        borderColor: [
          'rgba(153, 102, 255, 1)', //0 Black
          'rgba(180, 101, 225, 1)', //1
          'rgba(205, 100, 195, 1)', //2
          'rgba(230, 100, 165, 1)', //3
          'rgba(255, 99, 132, 1)', //4 Red
          'rgba(255, 114, 115, 1)', //5
          'rgba(255, 129, 98, 1)', //6
          'rgba(255, 144, 81, 1)', //7
          'rgba(255, 159, 64, 1)', //8 Orange
          'rgba(255, 171, 69, 1)', //9
          'rgba(255, 182, 75, 1)', //10
          'rgba(255, 195, 81, 1)', //11
          'rgba(255, 206, 86, 1)', //12 Yellow
          'rgba(210, 202, 112, 1)', //13
          'rgba(165, 199, 139, 1)', //14
          'rgba(120, 195, 165, 1)', //15
          'rgba(75, 192, 192, 1)', //16 Green
          'rgba(70, 185, 203, 1)', //17
          'rgba(65, 177, 213, 1)', //18
          'rgba(60, 170, 224, 1)', //19
          'rgba(54, 162, 235, 1)', //20 Blue
          'rgba(79, 147, 240, 1)', //21
          'rgba(104, 132, 245, 1)', //22
          'rgba(128, 117, 250, 1)' //23
        ],
        borderWidth: 3
      },
      {
        label: 'medium kms walked',
        data: avgActivity,
        backgroundColor: [
          'rgba(153, 102, 255, 0.2)', //0 Black
          'rgba(180, 101, 225, 0.2)', //1
          'rgba(205, 100, 195, 0.2)', //2
          'rgba(230, 100, 165, 0.2)', //3
          'rgba(255, 99, 132, 0.2)', //4 Red
          'rgba(255, 114, 115, 0.2)', //5
          'rgba(255, 129, 98, 0.2)', //6
          'rgba(255, 144, 81, 0.2)', //7
          'rgba(255, 159, 64, 0.2)', //8 Orange
          'rgba(255, 171, 69, 0.2)', //9
          'rgba(255, 182, 75, 0.2)', //10
          'rgba(255, 195, 81, 0.2)', //11
          'rgba(255, 206, 86, 0.2)', //12 Yellow
          'rgba(210, 202, 112, 0.2)', //13
          'rgba(165, 199, 139, 0.2)', //14
          'rgba(120, 195, 165, 0.2)', //15
          'rgba(75, 192, 192, 0.2)', //16 Green
          'rgba(70, 185, 203, 0.2)', //17
          'rgba(65, 177, 213, 0.2)', //18
          'rgba(60, 170, 224, 0.2)', //19
          'rgba(54, 162, 235, 0.2)', //20 Blue
          'rgba(79, 147, 240, 0.2)', //21
          'rgba(104, 132, 245, 0.2)', //22
          'rgba(128, 117, 250, 0.2)' //23
        ],
        borderColor: [
          'rgba(153, 102, 255, 1)', //0 Black
          'rgba(180, 101, 225, 1)', //1
          'rgba(205, 100, 195, 1)', //2
          'rgba(230, 100, 165, 1)', //3
          'rgba(255, 99, 132, 1)', //4 Red
          'rgba(255, 114, 115, 1)', //5
          'rgba(255, 129, 98, 1)', //6
          'rgba(255, 144, 81, 1)', //7
          'rgba(255, 159, 64, 1)', //8 Orange
          'rgba(255, 171, 69, 1)', //9
          'rgba(255, 182, 75, 1)', //10
          'rgba(255, 195, 81, 1)', //11
          'rgba(255, 206, 86, 1)', //12 Yellow
          'rgba(210, 202, 112, 1)', //13
          'rgba(165, 199, 139, 1)', //14
          'rgba(120, 195, 165, 1)', //15
          'rgba(75, 192, 192, 1)', //16 Green
          'rgba(70, 185, 203, 1)', //17
          'rgba(65, 177, 213, 1)', //18
          'rgba(60, 170, 224, 1)', //19
          'rgba(54, 162, 235, 1)', //20 Blue
          'rgba(79, 147, 240, 1)', //21
          'rgba(104, 132, 245, 1)', //22
          'rgba(128, 117, 250, 1)' //23
        ],
        borderWidth: 1
      }
      ]
    },
    options: {
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true
          }
        }]
      }
    }
  });
}
initActivityChart();

$('#btnWeek, #btnMonth, #btnSemester').click(function () {
  if ($(this).id != 'btnWeek') {
    $('#btnWeek').removeClass('active');
  }
    $.ajax({
      url: '/pet/StatsPeriod',
      type: 'GET',
      data: {
        'id': $(this).attr('data-petId'), 'period': $(this).attr('data-period')
      },
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        $('#activityStats').html(data);
        initActivityChart();
      },
      error: function () {
        alert("error");
      }
    });
  });
