﻿var dataTable;

$(document).ready(function () {
    cargarDataTable();
});


function cargarDataTable() {
    dataTable = $("#tblPlataformas").DataTable({
        "ajax": {
            "url": "/admin/plataformas/GetAll",
            "type": "POST",
            "datatype": "json"
        },


        "processing": true,
        "serverSide": true,
        "pageLength": 10,
        "filter": true,
        "data": null,
        "responsive": true,

        "columns": [
            //{ "data": "id", "width": "5%" },
            //{ "data": "url", "width": "40%" },
            //{ "data": "descripcion", "width": "10%" },

            { "data": "id", "autoWidth": true },
            { "data": "url", "autoWidth": true },
            { "data": "descripcion", "autowidth":true },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Plataformas/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                                <a onclick=Delete("/Admin/Plataformas/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-trash-alt"></i> Borrar  
                                </a>                                
                               
                          </div>
                         `;
                },
                /*"width": "40%"*/
                "autoWidth": true
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 de 0 de un total de 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        
        "width": "100%"
    });
}

function Delete(url) { /*ESTE METODO ES EL QUE SE LLAMA DESDE EL BOTON DE BORRAR DEL DATATABLE (QUE ESTÁ MÁS ARRIBA)*/
    swal({
        title: "¿Está seguro de borrar?",
        text: "¡Este contenido no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, ¡borrar!",
        closeOnconfirm: true
    }, function () {
        $.ajax({
            type: 'DELETE', /*esto es la llamada al metodo que está en el controller*/
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

    });
}


