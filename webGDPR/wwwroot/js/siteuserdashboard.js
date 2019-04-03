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
      type: 'POST',
      data: {
        'id': id
      },
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
      type: 'POST',
      data: {
        'id': caller.data('id')
      },
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
      type: 'POST',
      data: {
        'id': id
      },
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
      type: 'POST',
      data: {
        'id': caller.data('id')
      },
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
    
    var device = JSON.parse($(this).attr('data-device'));
    $("#deleteDeviceModalConfirmBtn").attr('data-deviceId', device.DeviceId);
    $("#deviceType").html(device.Type);
    $("#devicePlatform").html(device.Platform);
    $("#deviceName").html(device.Name);
    $("#deviceModel").html(device.Model);
    $("#deviceManufacturer").html(device.Manufacturer);
    $("#deviceOSVersion").html(device.OSVersion);
    $("#deviceAliasName").html(device.AliasName);    
    $("#deleteDeviceModal").modal("show");
  });

  $("#deleteDeviceModalConfirmBtn").click(function () {
    var id = $(this).attr('data-deviceId');
    $.ajax({
      url: '/device/Delete',
      type: 'POST',
      data: {
        'id': id
      },
      success: function (data) {
          $(".openDeleteDeviceDialog[data-id='" + id + "']").parent().parent().hide();
        $("#deleteDeviceModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });
  });

  //Delete base

  $(".openDeleteBaseDialog").click(function () {

    var base = JSON.parse($(this).attr('data-base'));
    $("#deleteBaseModalConfirmBtn").attr('data-baseId', base.BaseId);
    $("#baseHWId").html(base.HWId);
    $("#baseName").html(base.Name);
    if (base.LastStatus != null && base.LastStatus.DeviceConnectedTo != null) {
      $("#connectedToDT").attr('style', 'display:block;');
      $("#connectedToDD").attr('style', 'display:block;');
      $("#connectedToDD").html(base.LastStatus.DeviceConnectedTo.GetName);
    }
    else {
      $("#connectedToDT").attr('style', 'display:none;');
      $("#connectedToDD").attr('style', 'display:none;');
      $("#connectedToDD").html("");
    }
    $("#deleteBaseModal").modal("show");
  });

  $("#deleteBaseModalConfirmBtn").click(function () {
    var id = $(this).attr('data-baseId');
    $.ajax({
      url: '/base/Delete',
      type: 'POST',
      data: {
        'id': id
      },
      success: function (data) {
        $(".openDeleteBaseDialog[data-id='" + id + "']").parent().parent().hide();
        $("#deleteBaseModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });
  });

  //Delete collar

  $(".openDeleteCollarDialog").click(function () {

    var collar = JSON.parse($(this).attr('data-collar'));
    var pet = $(this).attr('data-pet');
    $("#deleteCollarModalConfirmBtn").attr('data-collarId', collar.CollarId);
    $("#collarHWId").html(collar.HWId);
    $("#collarName").html(collar.Name);
    if (collar.LastStatus != null) {
      $("#connectedToDT").attr('style', 'display:block;');
      $("#connectedToDD").attr('style', 'display:block;');
      $("#connectedToDD").html(collar.LastStatus.BaseConnectedTo.Name);
    }
    else {
      $("#connectedToDT").attr('style', 'display:none;');
      $("#connectedToDD").attr('style', 'display:none;');
      $("#connectedToDD").html("");
    }
    if (pet != "") {
      $("#petNameDT").attr('style', 'display:block;');
      $("#petNameDD").attr('style', 'display:block;');
      $("#petNameDD").html(pet);
    }
    else {
      $("#petNameDT").attr('style', 'display:none;');
      $("#petNameDD").attr('style', 'display:none;');
      $("#petNameDD").html("");
    }
    $("#deleteCollarModal").modal("show");
  });

  $("#deleteCollarModalConfirmBtn").click(function () {
    var id = $(this).attr('data-collarId');
    $.ajax({
      url: '/collar/Delete',
      type: 'POST',
      data: {
        'id': id
      },
      success: function (data) {
        var currentrow = $(".openDeleteCollarDialog[data-id='" + id + "']").parent().parent();
        currentrow.addClass("missing");        
        currentrow.html('<td><span style="color:red">Missing Collar</span></td>');
       if (currentrow.next().hasClass("missing")) {
          currentrow.hide();
          currentrow.next().hide();
        } 

        $("#deleteCollarModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });
  });

  //Delete pet

  $(".openDeletePetDialog").click(function () {

    var pet = JSON.parse($(this).attr('data-pet'));
    var collar = $(this).attr('data-collar');
    $("#deletePetModalConfirmBtn").attr('data-petId', pet.PetId);
    $("#petName1").html(pet.Name);
    $("#petType").html(pet.Type);
    $("#petBreeding").html(pet.Breeding);
    $("#petColor").html(pet.Color);
    $("#petBirthdate").html(pet.Birthdate);
    $("#petGender").html(pet.Gender);
    $("#petWeight").html(pet.Weigth);
    $("#petHealthComments").html(pet.HealthComments);
    if (collar != "") {
      $("#collarNameDT").attr('style', 'display:block;');
      $("#collarNameDD").attr('style', 'display:block;');
      $("#collarNameDD").html(collar);
    }
    else {
      $("#collarNameDT").attr('style', 'display:none;');
      $("#collarNameDD").attr('style', 'display:none;');
      $("#collarNameDD").html("");
    }
    $("#deletePetModal").modal("show");
  });

  $("#deletePetModalConfirmBtn").click(function () {
    var id = $(this).attr('data-petId');
    $.ajax({
      url: '/pet/Delete',
      type: 'POST',
      data: {
        'id': id
      },
      success: function (data) {
        var currentrow = $(".openDeletePetDialog[data-id='" + id + "']").parent().parent();
        currentrow.addClass("missing");
        currentrow.html('<td><span style="color:red">Missing Pet</span></td>');
        if (currentrow.prev().hasClass("missing")) {
          currentrow.hide();
          currentrow.prev().hide();
        }        
        $("#deletePetModal").modal("hide");
      },
      error: function () {
        alert("error");
      }
    });
  });


});

initPetsMap();