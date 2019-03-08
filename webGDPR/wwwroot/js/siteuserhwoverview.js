function initBaseCharts() {
  var chartDisconnection = new Chart(document.getElementById("horizontalBar"), {
    "type": "horizontalBar",
    "data": {
      "labels": names,
      "datasets": [{
        "label": "Time disconnected in minutes",
        "data": disconnectedTimes,
        "fill": false,
        "backgroundColor": "rgba(201, 203, 207, 0.5)",
        "borderColor": "rgb(201, 203, 207)",
        "borderWidth": 2
      }]
    },
    "options": {
      "scales": {
        "xAxes": [{
          "ticks": {
            "beginAtZero": true
          }
        }]
      }
    }
  });

  var ctxB = document.getElementById("verticalBarChart").getContext('2d');
  var myBarChart = new Chart(ctxB, {
    type: 'bar',
    data: {
      labels: names,
      datasets: [{
        label: 'Average Radio Strength',
        data: radios,
        backgroundColor: ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        borderColor: ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        borderWidth: 1
      }]
    },
    options: {
      scales: {
        yAxes: [{

          ticks: {
            beginAtZero: true, max: 100,
            min: 0
          }
        }]
      }
    }
  });
}

function initCollarCharts() {
  var chartDisconnection2 = new Chart(document.getElementById("horizontalBar2"), {
    "type": "horizontalBar",
    "data": {
      "labels": namesCollars,
      "datasets": [{
        "label": "Time disconnected in minutes",
        "data": disconnectedTimesCollars,
        "fill": false,
        "backgroundColor": ["rgba(201, 203, 207, 0.2)", "rgba(201, 203, 207, 0.2)",
          "rgba(201, 203, 207, 0.2)", "rgba(201, 203, 207, 0.2)"],
        "borderColor": ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        "borderWidth": 1
      },
      {
        "label": "Time GPS disconnected in minutes",
        "data": gpsDisconnectedTimesCollars,
        "fill": false,
        "backgroundColor": ["rgba(201, 203, 207, 0.2)", "rgba(201, 203, 207, 0.2)",
          "rgba(201, 203, 207, 0.2)", "rgba(201, 203, 207, 0.2)"],
        "borderColor": ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        "borderWidth": 1
      }
      ]
    },
    "options": {
      "scales": {
        "xAxes": [{
          "ticks": {
            "beginAtZero": true
          }
        }]
      }
    }
  });

  var ctxB2 = document.getElementById("verticalBarChart2").getContext('2d');
  var myBarChart2 = new Chart(ctxB2, {
    type: 'bar',
    data: {
      labels: namesCollars,
      datasets: [{
        label: 'Average Radio Strength',
        data: radiosCollars,
        backgroundColor: ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        borderColor: ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        borderWidth: 1
      },
      {
        label: 'Average Battery Power',
        data: batteryCollars,
        backgroundColor: ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        borderColor: ["rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)", "rgb(201, 203, 207)"],
        borderWidth: 1
      }
      ]
    },
    options: {
      scales: {
        yAxes: [{

          ticks: {
            beginAtZero: true, max: 100,
            min: 0
          }
        }]
      }
    }
  });
}

function initCollarMap() {
  try {
    if ($('#service-map').length > 0) {
      var mapService = $('#service-map');
      if (mapService.data('geoLat') && mapService.data('geoLng')) {
        var centerLocation = {
          lat: mapService.data('geoLat'),
          lng: mapService.data('geoLng')
        };

        var map = new google.maps.Map(document.getElementById('service-map'), {
          zoom: 17,
          center: centerLocation,
          mapTypeId: 'terrain'
        });

        var infowindow = new google.maps.InfoWindow({
          //content: contentString
        });

        //Markers and MarkersCluster
        // https://medium.com/@letian1997/how-to-change-javascript-google-map-marker-color-8a72131d1207
        /*          
         * http://maps.google.com/mapfiles/ms/icons/purple-dot.png
         * http://maps.google.com/mapfiles/ms/icons/red-dot.png
         * http://maps.google.com/mapfiles/ms/icons/orange-dot.png
         * http://maps.google.com/mapfiles/ms/icons/yellow-dot.png
         * http://maps.google.com/mapfiles/ms/icons/green-dot.png
         * http://maps.google.com/mapfiles/ms/icons/blue-dot.png
         * */
        var markers = coordinates.map(function (location) {
          return new google.maps.Marker({
            position: {
              lat: location.lat,
              lng: location.lng
            },
            icon: {
              url: "https://test.whereisfinder.com/images/" + location.color + "-dot.png"
            },
            gps: location.gps,
            radio: location.radio
          });
        });

        var myArray = [];

        for (var i = 0; i < markers.length; i += 2) {
          myArray.push(markers.slice(i, 2));
        }

        // Add a marker clusterer to manage the markers.
        var markerCluster = new MarkerClusterer(map, markers,
          { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });



        // Add a marker clusterer to manage the markers.
        var markerCluster1 = new MarkerClusterer(map, markers,
          { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

        //https://localhost:44392/images/home.png to scale to 30 x 30
        var image = {
          url: 'https://test.whereisfinder.com/images/home.png',
          // This marker is 20 pixels wide by 32 pixels high.
          size: new google.maps.Size(30, 30),
          // The origin for this image is (0, 0).
          origin: new google.maps.Point(0, 0),
          // The anchor for this image is the base of the flagpole at (0, 32).
          anchor: new google.maps.Point(0, 32)
        };

        var marker = new google.maps.Marker({
          position: centerLocation,
          map: map,
          icon: image

        });

        marker.addListener('click', function () {
          var gps = markers[i].getAttribute("gps");
          var radio = markers[i].getAttribute("radio");
          var html = '<div id="content">' +
            '<div id="siteNotice">' +
            '</div>' +
            '<h3 id="firstHeading" class="firstHeading"><b> GPS:" + gps + "</b> <br/> Radio:" + radio</h3>' +
            '</div>';
          // var html = "<b> GPS:" + gps + "</b> <br/> Radio:" + radio;
          infowindow.setContent(html);
          infowindow.open(map, marker);
        });
      }
    }
  }
  catch (err) {
    console.log(err);
  }
}

initBaseCharts();
initCollarCharts();
initCollarMap();

$('#btnHWWeek, #btnHWMonth, #btnHWSemester').click(function () {
  if ($(this).id != 'btnHWWeek') {
    $('#btnHWWeek').removeClass('active');
  }
  $.ajax({
    url: '/user/HWPeriod',
    type: 'GET',
    data: {
      'period': $(this).attr('data-period')
    },
    contentType: 'application/json; charset=utf-8',
    success: function (data) {
      $('#HWStats').html(data);
      initBaseCharts();
      initCollarCharts();
      initCollarMap();
    },
    error: function () {
      alert("error");
    }
  });
});

$(function () {
  $('.chart').easyPieChart({
    //your options goes here
    barColor: '#ef1e25',
    trackColor: '#f2f2f2',
    scaleColor:	'#dfe0e0',
    scaleLength: 5,
    lineCap: 'round', // butt, round and square.
    lineWidth: 3,
    size:	110,
    rotate:	0
  });
});

$(function () {
  $('.chart1').easyPieChart({
    easing: 'easeOutBounce',
    onStep: function (from, to, percent) {
      $(this.el).find('.percent').text(Math.round(percent));
    }
  });
  var chart = window.chart = $('.chart1').data('easyPieChart');
  $('.js_update').on('click', function () {
    chart.update(Math.random() * 200 - 100);
  });
});
