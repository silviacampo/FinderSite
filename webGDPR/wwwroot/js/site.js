// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
try {
  CKEDITOR.replace("FullDescription");
}
catch (err) {  
}

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

if ($('.card-map').length > 0) {
  var map = $('#card-map');
  if (map.data('geoLat') && map.data('geoLng')) {
    var deviceLocation = {
      lat: map.data('geoLat'),
      lng: map.data('geoLng')
    };

    var map = new google.maps.Map(document.getElementById('card-map'), {
      zoom: 13,
      center: deviceLocation,
      mapTypeId: 'terrain'
    });

    var contentString = '<div id="content">' +
      '<div id="siteNotice">' +
      '</div>' +
      '<h1 id="firstHeading" class="firstHeading">Uluru</h1>' +
      '<div id="bodyContent">' +
      '<p><b>Uluru</b>, also referred to as <b>Ayers Rock</b>, is a large ' +
      'sandstone rock formation in the southern part of the ' +
      'Northern Territory, central Australia. It lies 335&#160;km (208&#160;mi) ' +
      'south west of the nearest large town, Alice Springs; 450&#160;km ' +
      '(280&#160;mi) by road. Kata Tjuta and Uluru are the two major ' +
      'features of the Uluru - Kata Tjuta National Park. Uluru is ' +
      'sacred to the Pitjantjatjara and Yankunytjatjara, the ' +
      'Aboriginal people of the area. It has many springs, waterholes, ' +
      'rock caves and ancient paintings. Uluru is listed as a World ' +
      'Heritage Site.</p>' +
      '<p>Attribution: Uluru, <a href="https://en.wikipedia.org/w/index.php?title=Uluru&oldid=297882194">' +
      'https://en.wikipedia.org/w/index.php?title=Uluru</a> ' +
      '(last visited June 22, 2009).</p>' +
      '</div>' +
      '</div>';

    var infowindow = new google.maps.InfoWindow({
      content: contentString
    });
    var fenway = { lat: 42.345573, lng: -71.098326 };
    var panorama = new google.maps.StreetViewPanorama(
      document.getElementById('pano'), {
        position: fenway,
        pov: {
          heading: 34,
          pitch: 10
        }
      });
    map.setStreetView(panorama);

    var marker = new google.maps.Marker({
      position: deviceLocation,
      map: map
    });

    marker.addListener('click', function () {
      infowindow.open(map, marker);
    });

    var flightPlanCoordinates = [
      { lat: 45.5273677, lng: -73.6839115 },
      { lat: 45.5273298, lng: -73.6836115 },
      { lat: 45.5270227, lng: -73.6833668 },
      { lat: 45.5268926, lng: -73.6832776 }
    ];

    var lineSymbol = {
      path: google.maps.SymbolPath.FORWARD_OPEN_ARROW
    };

    var lineSymbolCat = {
      path: google.maps.SymbolPath.CIRCLE,
      scale: 4,
      strokeColor: '#393'
    };

    var flightPath = new google.maps.Polyline({
      path: flightPlanCoordinates,
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
      strokeColor: '#FF0000',
      strokeOpacity: 1.0,
      strokeWeight: 2
    });

    flightPath.setMap(map);

    // animateCircle(flightPath);

    function animateCircle(line) {
      var count = 0;
      window.setInterval(function () {
        count = (count + 1) % 200;

        var icons = line.get('icons');
        icons[0].offset = (count / 2) + '%';
        line.set('icons', icons);
      }, 20);
    }
    //mirar overlays

    // Create an array of alphabetical characters used to label the markers.
    var labels = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';

    var locations = [
      { lat: 45.4125436, lng: -73.7253372 },
      { lat: 45.3125436, lng: -73.7353372 },
      { lat: 45.2125436, lng: -73.7453372 },
      { lat: 45.1125436, lng: -73.7553372 },
      { lat: 45.0125436, lng: -73.7653372 },
      { lat: 45.9125436, lng: -73.7753372 },
      { lat: 45.8125436, lng: -73.7853372 },
      { lat: 45.7125436, lng: -73.7953372 },
      { lat: 45.6125436, lng: -73.7153372 },
      { lat: 45.5125436, lng: -73.7253372 },
      { lat: 45.5225436, lng: -73.7263372 },
      { lat: 45.5325436, lng: -73.7273372 },
      { lat: 45.5425436, lng: -73.7283372 },
      { lat: 45.5525436, lng: -73.7293372 },
      { lat: 45.5625436, lng: -73.7213372 },
      { lat: 45.5725436, lng: -73.7223372 },
      { lat: 45.5825436, lng: -73.7233372 },
      { lat: 45.5925436, lng: -73.7243372 },
      { lat: 45.5125436, lng: -73.7253372 },
      { lat: 45.5135436, lng: -73.7254372 },
      { lat: 45.5145436, lng: -73.7255372 },
      { lat: 45.5155436, lng: -73.7256372 },
      { lat: 45.5165436, lng: -73.7257372 }
    ]

    // Add some markers to the map.
    // Note: The code uses the JavaScript Array.prototype.map() method to
    // create an array of markers based on a given "locations" array.
    // The map() method here has nothing to do with the Google Maps API.
    var markers = locations.map(function (location, i) {
      return new google.maps.Marker({
        position: location,
        label: labels[i % labels.length]
      });
    });

    // Add a marker clusterer to manage the markers.
    var markerCluster = new MarkerClusterer(map, markers,
      { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });


  }
}
