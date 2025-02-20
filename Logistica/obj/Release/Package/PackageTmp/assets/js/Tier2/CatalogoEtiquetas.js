
document.addEventListener('DOMContentLoaded', function () {
    function cargarEtiquetas() {
        $.ajax({
            url: '/Tier2/consultarEtiquetas',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                console.log("Datos recibidos:", response);

                if (response.success) {

                    $('#tablaEtiquetas tbody').empty();
                    response.data.forEach(function (item) {
                        var row = `<tr data-no-parte="${item.NoParte}"> <!-- Agregar data-no-parte -->
                                       <td style="font-size: 16px; font-weight: bold;">${item.NoParte}</td>
                                       <td style="font-size: 16px; font-weight: bold;">${item.Total}</td>
                                   </tr>`;
                        $('#tablaEtiquetas tbody').append(row);
                    });
                    
                    $('#tablaEtiquetas tbody tr').on('click', function () {
                        var noParte = $(this).attr('data-no-parte');

                        $('#modalNoParte').text(noParte); 
                        cargarDetalleEtiquetas(noParte); 
                        $('#modalDetalle').modal('show'); 
                    });

                } else {
                    console.error("Error en la respuesta:", response.message);
                    $('#errorContainer').text('Error: ' + response.message).show();
                }
            },
            error: function (xhr, status, error) {
                console.error("Error en la petición:", error);
                $('#errorContainer').text('Error al cargar los datos: ' + error).show();
            },
            complete: function () {
                $('#spinner').hide();
            }
        });

    }
    function cargarDetalleEtiquetas(noParte) {
        console.log("Entrando en consultarDetalleEtiquetas");
        $.ajax({
            url: '/Tier2/consultarDetalleEtiquetas', 
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ noParte: noParte }), 
            success: function (response) {
                console.log("Detalle de etiquetas recibido:", response);
                if (response.success && response.data) {
                    $('#tablaDetalleEtiquetas tbody').empty(); 
                    response.data.forEach(function (item) {
                        var estadoTexto = item.Estado === 1 ? "En uso" : "En gaveta"; 
                        var estadoColor = item.Estado === 1 ? "red" : "green"; 
                        var row = `<tr>
                                       <td>${item.NoEtiqueta}</td>
                                       <td style="color: ${estadoColor};">${estadoTexto}</td>
                                       <td>${item.UltimoUso}</td>
                                   </tr>`;
                        $('#tablaDetalleEtiquetas tbody').append(row);
                    });
                } else {
                    console.error("Error en la respuesta:", response.message);
                    $('#errorContainer').text('Error: ' + response.message).show();
                }
            },
            error: function (xhr, status, error) {
                console.error("Error en la petición:", error);
                $('#errorContainer').text('Error al cargar el detalle: ' + error).show();
            }
        });
    }

    $('#modalDetalle .close, #modalDetalle .btn-secondary').on('click', function () {
        $('#modalDetalle').modal('hide'); 
    });

    cargarEtiquetas();
});
