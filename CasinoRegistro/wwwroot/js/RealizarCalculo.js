function AccionBloqueo(url) {

    $.ajax({
        type: 'RealizarCalculos2',
        url: url,
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });

    toastr.options = {
        //primeras opciones
        "closeButton": false, //boton cerrar
        "debug": false,
        "newestOnTop": false, //notificaciones mas nuevas van en la parte superior
        "progressBar": false, //barra de progreso hasta que se oculta la notificacion
        "preventDuplicates": false, //para prevenir mensajes duplicados

        "onclick": null,


        //Posición de la notificación
        //toast-bottom-left, toast-bottom-right, toast-bottom-left, toast-top-full-width, toast-top-center
        "positionClass": "toast-top-center",

        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false,
    };

}