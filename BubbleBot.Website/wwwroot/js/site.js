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