var dataTable;

$(document).ready(function () {
    cargarDataTable();
});


function cargarDataTable() {


    var groupColumn = 0;   

    dataTable = $("#tblRegistros").DataTable({
        "ajax": {
            "url": "/admin/RegistrosMovimientos/GetAll",
            "type": "POST",
            "datatype": "json"
        },
        "processing": true,
        "serverSide": true,
        "pageLength": 10,
        "filter": true,
        "data": null,
        "responsive": true,
        displayLength: 25,
        //columnDefs: [{ visible: false, targets: groupColumn }],
        //order: [[groupColumn, 'asc']],

        order: [
                [0, 'asc'],
                [1, 'asc']            
               ],
        rowGroup: {
                dataSrc: ['cajeroUser.nombreCompleto', 'fechaCreacion']
                },
        columnDefs: [
                {
                    targets: [0, 1],
                    visible: false
                //targets: [0], visible: false,
                //targets: [1], visible: false,
                    //  targets: '_all', visible: true 

                }
            ],

  

        //drawCallback: function (settings) {
        //    var api = this.api();
        //    var rows = api.rows({ page: 'current' }).nodes();
        //    var last = null;

        //    api.column(groupColumn, { page: 'current' })
        //        .data()
        //        .each(function (group, i) {
        //        /*.each(function (group, i) {*/
        //            if (last !== group) {
        //                $(rows)
        //                    .eq(i)
        //                    .before(
        //                        '<tr class="group"><td colspan="5" style="background-color:#f9f6f0"> Registros del cajero/a: ' +
        //                        group +
        //                        '</td></tr>'
        //                    );

        //                last = group;
        //            }
        //        });
        //},

        //rowGroup: {
        //    dataSrc: [0,1]
        //},


        "columns": [
            //{ "data": "cajeroUser.nombreCompleto", "width": "30%" },      
            //{ "data": "fechaCreacion", "width": "40%" },
            { "data": "cajeroUser.nombreCompleto", "autowidth": true },
            { "data": "fechaCreacion", "autowidth": true },
            {
                "data": "esIngresoFichas",
                "render": function (data, type, row) {
                    // Aquí obtenemos el estado desde la fila
                    const estado = row.esIngresoFichas;
                    let tipo;

                    if (estado == true) {
                        tipo = "Fichas";
                    } else {
                        tipo = "Dinero";
                    }

                    // return `<div class>`  tipo;
                    return `<div class="text-center">
                    ${tipo}
                </div>`
                },
                /*"width": "10%"*/
                "autowidth": true 
            },        
         



            




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
                },
                "width": "40%"             
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
    //$('#tblRegistros tbody').on('click', 'tr.group', function () {
    //    var currentOrder = table.order()[0];
    //    if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
    //        table.order([groupColumn, 'desc']).draw();
    //    }
    //    else {
    //        table.order([groupColumn, 'asc']).draw();
    //    }
    //});
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


