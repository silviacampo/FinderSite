$("#deviceConnectionMoreBtn").click(function () {
  $("#loader").show();
  $('.overlay').show();
  var page = $(this).attr('data-page');
  var id = $(this).attr('data-id');
  $(this).attr('data-page', parseInt(page) + 1);
  $.ajax({
    url: '/Device/ConnectionTimelineMore/',
    type: 'GET',
    data: { page: page, id: id },
    contentType: 'application/json; charset=utf-8',
    success: function (data) {
      for (var i = 0, l = data.length; i < l; i++) {
        var localTime = new Date(data[i].itemDate);
        localTimeStr = localTime.toLocaleString();
        var newDate = new Date(localTimeStr);
        if (oldDate.getFullYear() != newDate.getFullYear() || oldDate.getMonth() != newDate.getMonth() || oldDate.getDate() != newDate.getDate()) {
          oldDate = newDate;
          $(".timeline:last").append('<div class="dateSeparator">' + localTime.toLocaleDateString() + '</div>');
        }
        var orientationClass = "";
        if (data[i].orientation == 0) {
          orientationClass = "left";
        }
        else {
          orientationClass = "right";
        }
        $(".timeline:last").append('<div class="timelineitemcontainer ' + orientationClass + '"><div class="content"><h5 style="display:inline;">' + localTime.toLocaleTimeString() + '</script></h5><p style="display: inline;float:right;">' + data[i].itemLeftTitle + '</p><p style="clear: both;">' + data[i].itemMessage + '<i class="fa fa-plus-circle fa-fw" aria-hidden="true" title="' + data[i].itemMore + '"></i></p></div></div>');
      }
      $("#loader").hide();
      $('.overlay').hide();
    },
    error: function () {
      alert("error");
    }
  });
});
