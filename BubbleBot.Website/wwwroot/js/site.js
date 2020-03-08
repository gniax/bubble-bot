
type = ['primary', 'info', 'success', 'warning', 'danger'];
icone = ['nc-icon nc-app', 'fa fa-info', 'fa fa-check', 'fa fa-exclamation-circle', 'fa fa-exclamation-triangle'];

site = {
    showNotification: function(msg, color = 0, from, align) {
        $.notify({
            icon: icone[color],
            message: msg

        }, {
            type: type[color],
            timer: 8000,
            placement: {
                from: from,
                align: align
            }
        });
    }
}

$(document).on('click', '.hidebtn', function() {
    document.getElementById("custom-sidebar").style.width = "30px";
    document.getElementById("custom-sidebar").style.height = "272px";
    var all = document.getElementsByClassName("custom-nav");
    for (var i = 0; i < all.length; i++) {
      all[i].style.opacity = 0;
      all[i].style.pointerEvents = "none";
      all[i].style.cursor = "default";
    }
    document.getElementById("navcontent").style.transition = "0.2s";
    document.getElementById("navcontent").style.opacity = 0;
    document.getElementById("togglebtn").className = "showbtn";
    document.getElementById("toggle-icon").className = "fa fa-plus-square";
});

$(document).on('click', '.showbtn', function() {
    document.getElementById("custom-sidebar").style.width = "250px";
    document.getElementById("custom-sidebar").style.height = "auto";

    var all = document.getElementsByClassName("custom-nav");
    for (var i = 0; i < all.length; i++) {
      all[i].style.opacity = 1;
      all[i].style.pointerEvents = "all";
      all[i].style.cursor = "pointer";
    }
    document.getElementById("navcontent").style.transition = "1s";
    document.getElementById("navcontent").style.opacity = 1;
    document.getElementById("togglebtn").className = "hidebtn";
    document.getElementById("toggle-icon").className = "fa fa-minus-square";
});

$(document).on('click', '.hidebtn-right', function() {
    document.getElementById("custom-sidebar-right").style.width = "30px";
    document.getElementById("custom-sidebar-right").style.height = "306px";
    var all = document.getElementsByClassName("custom-nav-right");
    for (var i = 0; i < all.length; i++) {
      all[i].style.opacity = 0;
      all[i].style.pointerEvents = "none";
      all[i].style.cursor = "default";
    }
    document.getElementById("navcontent-right").style.display = "none";
    document.getElementById("classe_logo").style.width = "0px";
    document.getElementById("togglebtn-right").className = "showbtn-right";
    document.getElementById("toggle-icon-right").className = "fa fa-plus-square";
});

$(document).on('click', '.showbtn-right', function() {
    document.getElementById("custom-sidebar-right").style.width = "250px";
    document.getElementById("custom-sidebar-right").style.height = "auto";

    var all = document.getElementsByClassName("custom-nav-right");
    for (var i = 0; i < all.length; i++) {
      all[i].style.opacity = 1;
      all[i].style.pointerEvents = "all";
      all[i].style.cursor = "pointer";
    }
    document.getElementById("navcontent-right").style.display = "block";
    document.getElementById("classe_logo").style.width = "auto";
    document.getElementById("togglebtn-right").className = "hidebtn-right";
    document.getElementById("toggle-icon-right").className = "fa fa-minus-square";
});