$(".sendChatroomMessage").click(function () {
  var connectionid = $(this).attr('data-id');
  var message = $(this).prev().val();
  $.ajax({
    url: '/monitoring/SendChatroomMessage',
    type: 'POST',
    data: {
      'id': connectionid,
      'message' : message
    },
    success: function (data) {
      var currentrow = $(".sendChatroomMessage[data-id='" + connectionid + "']").parent().parent();
      //currentrow.pre().
    },
    error: function () {
      alert("error");
    }
  });
});