function initPetsMap() {
  try {
    if ($('.card-map').length > 0) {
      var map1 = $('#pets-map');
      if (map1.data('geoLat') && map1.data('geoLng')) {
        var centerLocation = {
          lat: map1.data('geoLat'),
          lng: map1.data('geoLng')
        };

        var map = new google.maps.Map(document.getElementById('pets-map'), {
          zoom: 15,
          center: centerLocation,
          mapTypeId: 'terrain'
        });

        var infowindow = new google.maps.InfoWindow({
          });

        var panorama = new google.maps.StreetViewPanorama(
          document.getElementById('panoDashboard'), {
            position: centerLocation,
            pov: {
              heading: 34,
              pitch: 10
            }
          });
        map.setStreetView(panorama);

        //http://www.iconarchive.com/search?q=paw
        var imgDog = {
          url: domain + '/images/22214-dog-face-iconsm.png',
          size: new google.maps.Size(30, 30),
          origin: new google.maps.Point(0, 0),
          anchor: new google.maps.Point(0, 32)
        };

        var imgCat = {
          url: domain + '/images/cat-iconsm.png',
          size: new google.maps.Size(30, 30),
          origin: new google.maps.Point(0, 0),
          anchor: new google.maps.Point(0, 32)
        };

        //Markers and MarkersCluster
        //https://medium.com/@letian1997/how-to-change-javascript-google-map-marker-color-8a72131d1207
        var markers = coordinates.map(function (location, i) {
          var imgPet;
          if (location.type == "Cat") {
            imgPet = imgCat;
          }
          else {
            imgPet = imgDog;
          }

          return new google.maps.Marker({
            position: {
              lat: location.lat,
              lng: location.lng
            },
            //label: "test",
            icon: imgPet,
            name: location.name,
            time: location.time
          });
        });

        // Add a marker clusterer to manage the markers.
        var markerCluster = new MarkerClusterer(map, markers,
          { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

        var imghome = {
          url: domain + '/images/home.png',
          size: new google.maps.Size(30, 30),
          origin: new google.maps.Point(0, 0),
          anchor: new google.maps.Point(0, 32)
        };

        var marker = new google.maps.Marker({
          position: centerLocation,
          map: map,
          icon: imghome

        });

        markers.forEach(function (item) {
          item.addListener('click', function () {
            var name = item.name;
            var time = item.time;
            var html = '<div id="content">' +
              '<div id="siteNotice">' +
              '</div>' +
              '<h5 id="firstHeading" class="firstHeading"><b>' + name + '</b> at<br/>' + time + '</h5>' +
              '</div>';
            infowindow.setContent(html);
            infowindow.open(map, item);
          });
        });
      }
    }
  }
  catch (err) {
    console.log(err);
  }

}

initPetsMap();