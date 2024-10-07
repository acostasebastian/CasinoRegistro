
var dataTable;

$(document).ready(function () {
    cargarDataTable();
});

function cargarDataTable() {
    var groupColumn = 0;
    var deudaTotal; // Variable para almacenar el valor de deudaPesosActual

    dataTable = $("#tblRegistros").DataTable({
        "ajax": {
            "url": "/admin/RegistrosMovimientos/GetAllCajero",
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

        order: [
            [0, 'asc'],
            /*[1, 'asc']*/
        ],
        rowGroup: {
           // dataSrc: ['fechaCreacion', 'esIngresoFichas']
                        dataSrc: ['fechaCreacion']
        },
        columnDefs: [
            {
                targets: [0], visible: false,
                //targets: [0, 1],
                //visible: false
               // targets: [1], className: 'dt-right'
            }
        ],

        "columns": [
            { "data": "fechaCreacion", "width": "25%" },
            {

                "data": "esIngresoFichas",
                "render": function (data) {
                    return data ? 'Fichas' : 'Dinero';
                },
                "width": "30%",
              

            },
            
            { "data": "fichasCargadas", "width": "15%" },
            { "data": "pesosEntregados", "width": "15%" },
            { "data": "pesosDevueltos", "width": "15%" },
            { "data": "comision", "width": "15%" },
            /*{ "data": "cajeroUser.deudaPesosActual", "width": "35%" }*/
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 de 0 de un total de 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
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

        "initComplete": function (settings, json) {
            // Solo se ejecuta una vez que DataTables ha terminado de inicializar
            if (json.data && json.data.length > 0) {
                deudaTotal = Math.abs(json.data[0].cajeroUser.deudaPesosActual); // Obtener el valor absoluto
                document.getElementById("deudaTotal").innerText = deudaTotal; // Actualizar el HTML
            }
        },

        "width": "100%"
    });

    $('#tblRegistros tbody').on('click', 'tr.group', function () {
        var currentOrder = dataTable.order()[0];
        if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
            dataTable.order([groupColumn, 'desc']).draw();
        }
        else {
            dataTable.order([groupColumn, 'asc']).draw();
        }
    });
}