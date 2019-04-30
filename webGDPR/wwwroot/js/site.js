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



$("#UserSelect").change(function () {
  var dID = $('#UserSelect option:selected').val();
  $.getJSON("/Monitoring/GetUserSubTypes", { id: dID },
    function (data) {

      //Device
      var selectDevice = $("#DeviceSelect");
      selectDevice.empty();
      selectDevice.append($('<option />', {
        value: 0,
        text: "Select a device"
      }));
      $.each(data.devicesItems, function (index, itemData) {
        selectDevice.append($('<option/>', {
          value: itemData.value,
          text: itemData.text
        }));
      });

      //Base
      var selectBase = $("#BaseSelect");
      selectBase.empty();
      selectBase.append($('<option />', {
        value: 0,
        text: "Select a base"
      }));
      $.each(data.basesItems, function (index, itemData) {
        selectBase.append($('<option/>', {
          value: itemData.value,
          text: itemData.text
        }));
      });

      //Collar
      var selectCollar = $("#CollarSelect");
      selectCollar.empty();
      selectCollar.append($('<option />', {
        value: 0,
        text: "Select a collar"
      }));
      $.each(data.collarsItems, function (index, itemData) {
        selectCollar.append($('<option/>', {
          value: itemData.value,
          text: itemData.text
        }));
      });

    });
});

$("#CollarSelect").change(function () {
  var dID = $('#CollarSelect option:selected').val();
  if (dID == 127)
  {
    $("#collarNumberInput").val(dID);
  }
  else {
    var dText = $('#CollarSelect option:selected').text();
    dText = dText.substring(0, dText.indexOf("-")).trim();
    $("#collarNumberInput").val(parseInt(dText));
  }
});

function showToast(message) {
  var x = document.getElementById("toast");
  x.textContent = message;
  x.className = "show";
  setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}

$("#chatModal").draggable({
  handle: ".modal-content"
});

$("#chatModal").resizable({
  //alsoResize: ".modal-dialog",
  minHeight: 300,
  minWidth: 300
});

$('#chatModal').on('show.bs.modal', function (e) {
  alert("I want this to appear after the modal has opened!");
  var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
  connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    $(".conversation-body").append(li);
  });
  connection.start().then(function () {
    $("#chat-input").keyup(function (event) {
      if (event.keyCode === 13) {
        $(".conversation-body").append('<li class="even read"><span class="user">You</span> <p>' + $("#chat-input").val() + '</p> <span class="time">' + new Date().getHours() + ':' + new Date().getMinutes() + '</span></li>');
        $('.conversation-body').animate({ scrollTop: $('.conversation-body').prop("scrollHeight") }, 500);
        connection.invoke("SendMessage", "test", $("#chat-input").val()).catch(function (err) {
          return console.error(err.toString());
        });
        $("#chat-input").val("");
      }
    });
  }).catch(function (err) {
    return console.error(err.toString());
  });
});

$('#chatModal').on('hide.bs.modal', function (e) {
  alert("I want this to appear after the modal has opened!");
});


 

