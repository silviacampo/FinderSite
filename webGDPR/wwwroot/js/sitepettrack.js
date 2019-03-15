function initPetTrackMap() {
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
          icons: [//{
          //  icon: lineSymbolCat,
          //  offset: '100%'
          //},
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

        var imgPaw = {
          url: domain + '/images/82-dog-paw-iconsm.png',
          size: new google.maps.Size(30, 30),
          origin: new google.maps.Point(0, 0),
          anchor: new google.maps.Point(0, 32)
        };

        var markers = coordinates.map(function (location, i) {
          return new google.maps.Marker({
            position: {
              lat: location.lat,
              lng: location.lng
            },
            label: i.toString(),
            icon: {
              url: domain + '/images/blue-dot.png'
            },
            time: location.time,
            title: 'at ' + location.time,
            map: map
          });
        });

        var imgDog = {
          url: domain + '/images/22214-dog-face-iconsm.png',
          size: new google.maps.Size(30, 30),
          origin: new google.maps.Point(0, 0),
          anchor: new google.maps.Point(5, 10)
        };

        var imgCat = {
          url: domain + '/images/cat-iconsm.png',
          size: new google.maps.Size(30, 30),
          origin: new google.maps.Point(0, 0),
          anchor: new google.maps.Point(5, 10)
        };

        var imgPet;
        if (type == "Cat") {
          imgPet = imgCat;
        }
        else {
          imgPet = imgDog;
        }

        var marker = new google.maps.Marker({
          position: deviceLocation,
          map: map,
          title: contentString,
          icon: imgPet
        });

        marker.addListener('click', function () {
          infowindow.setContent(contentHtmlString);
          infowindow.open(map, marker);
        });

        markers.forEach(function (item) {
          item.addListener('click', function () {
            var time = item.time;
            var html = '<div id="content">' +
              '<div id="siteNotice">' +
              '</div>' +
              '<h5 id="firstHeading" class="firstHeading">at ' + time + '</h5>' +
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

initPetTrackMap();