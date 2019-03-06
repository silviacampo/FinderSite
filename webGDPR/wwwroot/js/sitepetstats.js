function initActivityChart() {
  try {
    if (document.getElementById("activityChart") != null) {
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
              'rgba(153, 102, 255, 0.2)', //0 Purple
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
              'rgba(153, 102, 255, 1)', //0 Purple
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
  } catch (err) {
    console.log(err);
  }
}

initActivityChart();

function initActivityMap() {
  try {
    if ($('#stat-map').length > 0) {
      var mapPetStats = $('#stat-map');
      if (mapPetStats.data('geoLat') && mapPetStats.data('geoLng')) {
        var centerLocation = {
          lat: mapPetStats.data('geoLat'),
          lng: mapPetStats.data('geoLng')
        };

        var map = new google.maps.Map(document.getElementById('stat-map'), {
          zoom: 17,
          center: centerLocation,
          mapTypeId: 'terrain'
        });

        var infowindow = new google.maps.InfoWindow({
          content: contentString
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
            }
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
          infowindow.open(map, marker);
        });
      }
    }
  }
  catch (err) {
    console.log(err);
  }
}

initActivityMap();

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
      initActivityMap();
    },
    error: function () {
      alert("error");
    }
  });
});