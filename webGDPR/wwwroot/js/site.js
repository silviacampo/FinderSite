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
      path: coordinates,
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

    //Markers and MarkersCluster
    // Create an array of alphabetical characters used to label the markers.
    var labels = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';

     var markers = coordinates.map(function (location, i) {
      return new google.maps.Marker({
        position: location,
        label: labels[i % labels.length]
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
