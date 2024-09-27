﻿var dataTable;

$(document).ready(function () {
    cargarDataTable();
});


function cargarDataTable() {


    var groupColumn = 0;

    dataTable = $("#tblRegistros").DataTable({
        "ajax": {
            "url": "/admin/RegistrosMovimientos/GetAll",
            "type": "GET",
            "datatype": "json"
        },

        columnDefs: [{ visible: false, targets: groupColumn }],
        order: [[groupColumn, 'asc']],
        displayLength: 25,

        drawCallback: function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;

            api.column(groupColumn, { page: 'current' })
                .data()
                .each(function (group, i) {
                    if (last !== group) {
                        $(rows)
                            .eq(i)
                            .before(
                                '<tr class="group"><td colspan="5"> Registros del cajero/a: ' +
                                group +
                                '</td></tr>'
                            );

                        last = group;
                    }
                });
        },


        "columns": [
            { "data": "cajeroUser.nombreCompleto", "width": "30%" },
            { "data": "fechaCreacion", "width": "40%" },
            //{
            //    "data": "fechaCreacion",
            //    "render": function (data) {
            //        const date = new Date(data);

            //        let fecha = date.toISOString().split('T')[0];
            //        let arr = fecha.split('-');
            //        let tiempo = date.toISOString().split('T')[1];
            //        let arrTiempo = tiempo.split(':');

            //        // const formattedDate = arr[2] + '/' + arr[1] + '/' + arr[0] + " - " + arrTiempo[0] + ':' + arrTiempo[1];
            //        const formattedDate = date.toISOString().split('T')[0] + " - " + date.toISOString("").split('T')[1];
            //       // const formattedDate = date.toString("yyyy-MM-dd HH:mm");
            //        return formattedDate;
            //    },
            //    "width": "30%"
            //},

            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                 <a href="/Admin/RegistrosMovimientos/Details/${data}" class="btn btn-info text-white" style="cursor:pointer; width:140px;">
                                <i class="fa-solid fa-circle-info"></i> Detalles
                                </a>
                                &nbsp;
                                <a href="/Admin/RegistrosMovimientos/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                                <a onclick=Delete("/Admin/RegistrosMovimientos/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-trash-alt"></i> Borrar  
                                </a>                                
                               
                          </div>
                         `;
                }, "width": "40%"
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

    // Order by the grouping
    $('#tblRegistros tbody').on('click', 'tr.group', function () {
        var currentOrder = table.order()[0];
        if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
            table.order([groupColumn, 'desc']).draw();
        }
        else {
            table.order([groupColumn, 'asc']).draw();
        }
    });
}

//function cargarDataTable() {
//    dataTable = $("#tblRegistros").DataTable({
//        "ajax": {
//            "url": "/admin/RegistrosMovimientos/GetAll",
//            "type": "GET",
//            "datatype": "json"
//        },
//        "columns": [
//            { "data": "cajeroUser.nombreCompleto", "width": "30%" },            
//            { "data": "fechaCreacion", "width": "40%" },
//            {
//                "data": "fechaCreacion",
//                "render": function (data) {
//                    const date = new Date(data);

//                    let fecha = date.toISOString().split('T')[0];
//                    let arr = fecha.split('-');
//                    let tiempo = date.toISOString().split('T')[1];
//                    let arrTiempo = tiempo.split(':');

//                   // const formattedDate = arr[2] + '/' + arr[1] + '/' + arr[0] + " - " + arrTiempo[0] + ':' + arrTiempo[1];
//                    const formattedDate = date.toISOString().split('T')[0] + " - " + date.toISOString("").split('T')[1];
//                    //const formattedDate = date.toString();
//                    return formattedDate;
//                },
//                "width": "40%"
//            },
            
//            {
//                "data": "id",
//                "render": function (data) {
//                    return `<div class="text-center">
//                                <a href="/Admin/RegistrosMovimientos/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
//                                <i class="far fa-edit"></i> Editar
//                                </a>
//                                &nbsp;
//                                <a onclick=Delete("/Admin/RegistrosMovimientos/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
//                                <i class="far fa-trash-alt"></i> Borrar  
//                                </a>                                
                               
//                          </div>
//                         `;
//                }, "width": "40%"
//            }
//        ],
//        "language": {
//            "decimal": "",
//            "emptyTable": "No hay registros",
//            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
//            "infoEmpty": "Mostrando 0 de 0 de un total de 0 Entradas",
//            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
//            "infoPostFix": "",
//            "thousands": ",",
//            "lengthMenu": "Mostrar _MENU_ Entradas",
//            "loadingRecords": "Cargando...",
//            "processing": "Procesando...",
//            "search": "Buscar:",
//            "zeroRecords": "Sin resultados encontrados",
//            "paginate": {
//                "first": "Primero",
//                "last": "Ultimo",
//                "next": "Siguiente",
//                "previous": "Anterior"
//            }
//        },
//        "width": "100%"
//    });
//}

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
    });
}


