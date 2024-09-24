var dataTable;

$(document).ready(function () {
    cargarDataTable();
});


function cargarDataTable() {
    dataTable = $("#tblCajeros").DataTable({
        "ajax": {
            "url": "/admin/cajeros/GetAll",
            "type": "GET",
            "datatype": "json"

        },


        "columns": [
            { "data": "nombre", "width": "5%" },
            { "data": "apellido", "width": "40%" },
            { "data": "email", "width": "10%" },
            { "data": "deudaPesosActual", "width": "30%" },

            {
                "data": "id",
                "render": function (data, type, row) {
                    // Aquí obtenemos el estado desde la fila
                    //const estado = row.lockoutEnd;
                    const estado = row.estado;
                    let botonAccion;

                    if (estado == false) {
                        botonAccion = `<a onclick=AccionBloqueo("/Admin/Cajeros/BloquearDesloquearCajero/${data}") class="btn btn-success text-white" style="cursor:pointer; width:150px;">
                                        <i class="fa-solid fa-lock-open"></i> Desbloquear
                                       </a>`;
                    } else {
                        botonAccion = `<a onclick=AccionBloqueo("/Admin/Cajeros/BloquearDesloquearCajero/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                        <i class="fa-solid fa-user-lock"></i> Bloquear
                                       </a>`;
                    }

                    return `<div class="text-center">
                                <a href="/Admin/Cajeros/Edit/${data}" class="btn btn-warning text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                                ${botonAccion}
                            </div>`;
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
}

function AccionBloqueo(url) {

    $.ajax({
        type: 'BloquearDesloquearCajero',
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

}
