var dataTable;

$(document).ready(function () {
    cargarDataTable();
});


function cargarDataTable() {


    var groupColumn = 0;

    dataTable = $("#tblRegistros").DataTable({
        "ajax": {
            "url": "/admin/RegistrosMovimientos/GetAllCajero",
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

          
            //SUPUESTAMENTE, DESCOMENTADO ESTE PEDAZO Y REEEMPLAZANDOLO POR EL OTRO FUNCIONA AGRUPAR POR OTRA COLUMNAS.. PERO NO ANDA, ME DA ERROR.
            //ME PARECE QUE FALTA ALGUNA LLAVE O PARENTESIS, PERO NO LOGRO DARME CUENTA DONDE

            //segun los comentarios de esto >>> https://datatables.net/examples/advanced_init/row_grouping.html

            //api.columns(groupColumn, { page: 'current' }).every(function () {
            //        this.data().each(function (group, i) {
            api.column(groupColumn, { page: 'current' }) //REEMPLAZAR
                .data()//REEMPLAZAR
                .each(function (group, i) {//REEMPLAZAR
                    if (last !== group) {
                        $(rows)
                            .eq(i)
                            .before(
                                '<tr class="group"><td colspan="5">' +
                                group +
                                '</td></tr>'
                            );

                        last = group;
                    }
                });
        },

        "columns": [
            {
                "data": "esIngresoFichas",

                //DESCOMENTANDO ESTA PARTE, QUISIERA QUE EN VEZ DE TRUE/FALSE DIGA COMO MOVIMIENTO FICHAS O DINERO

                //"render": function (data) {

                //    const texto = ""
                //    if (data === true) {
                //        texto = 'Fichas'
                //    }

                //    else {
                //        texto = 'Dinero'
                //    }

                //    return texto;
                //},


                "width": "30%"
            },
            { "data": "fechaCreacion", "width": "40%" },
            { "data": "fichasCargadas", "width": "15%" },
            { "data": "pesosEntregados", "width": "15%" },
            { "data": "pesosDevueltos", "width": "15%" },
            { "data": "comision", "width": "15%" },
            { "data": "cajeroUser.deudaPesosActual", "width": "20%" },
            
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
        responsive: true,
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


