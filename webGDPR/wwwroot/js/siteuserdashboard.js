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
$(function () {
  //Lost pet

  $(".openReportLostDialog").click(function () {
    $("#petLostModalConfirmBtn").attr('data-petId',$(this).data('id'));
    $("#petName").html($(this).data('name'));
    $("#petLostModal").modal("show");
  });

  $("#petLostModalConfirmBtn").click(function () {
    var id = $(this).attr('data-petId');
    $.ajax({
      url: '/pet/EmergencyOn',
      type: 'GET',
      data: {
        'id': id
      },
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        $(".openReportLostDialog[data-id='"+id+"']").hide();
        $(".openReportLostDialog[data-id='"+id+"']").siblings(".reportFound").show();
        $("#petLostModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });   
  });

  $(".reportFound").click(function () {
    var caller = $(this);
    $.ajax({
      url: '/pet/EmergencyOff',
      type: 'GET',
      data: {
        'id': caller.data('id')
      },
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        caller.hide();
        caller.siblings(".openReportLostDialog").show();
      },
      error: function () {
        alert("error");
      }
    });
  });

  //Ban device

  $(".openBanDialog").click(function () {
    $("#banModalConfirmBtn").attr('data-deviceId', $(this).data('id'));
    $("#banName").html($(this).data('name'));
    $("#banModal").modal("show");
  });

  $("#banModalConfirmBtn").click(function () {
    var id = $(this).attr('data-deviceId');
    $.ajax({
      url: '/device/BanOn',
      type: 'GET',
      data: {
        'id': id
      },
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        $(".openBanDialog[data-id='" + id + "']").hide();
        $(".openBanDialog[data-id='" + id + "']").siblings(".unBan").show();
        $("#banModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });
  });

  $(".unBan").click(function () {
    var caller = $(this);
    $.ajax({
      url: '/device/BanOff',
      type: 'GET',
      data: {
        'id': caller.data('id')
      },
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        caller.hide();
        caller.siblings(".openBanDialog").show();
      },
      error: function () {
        alert("error");
      }
    });
  });

  //Delete device

  $(".openDeleteDeviceDialog").click(function () {
    $("#deleteDeviceModalConfirmBtn").attr('data-deviceId', $(this).data('id'));
   // $("#banName").html($(this).data('name'));
    $("#deleteDeviceModal").modal("show");
  });

  $("#deleteDeviceModalConfirmBtn").click(function () {
    var id = $(this).attr('data-deviceId');
    $.ajax({
      url: '/device/Delete',
      type: 'GET',
      data: {
        'id': id
      },
      contentType: 'application/json; charset=utf-8',
      success: function (data) {
        $(".openDeleteDeviceDialog[data-id='" + id + "']").parent().hide();
        $("#deleteDeviceModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });
  });
});

initPetsMap();