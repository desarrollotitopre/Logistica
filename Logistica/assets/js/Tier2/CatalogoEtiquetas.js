
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
                        var row = `<tr>
                                       <td style="font-size: 16px; font-weight: bold;">${item.NoParte}</td>
                                       <td style="font-size: 16px; font-weight: bold;">${item.Total}</td>
                                   </tr>`;
                        $('#tablaEtiquetas tbody').append(row);
                    });

                    $('#tablaEtiquetas tbody tr').on('click', function () {
                        var noParte = $(this).data('no-parte');
                        console.log("Número de parte seleccionado:", noParte);
                        cargarDetalleEtiquetas(noParte);
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

        function cargarDetalleEtiquetas(noParte) {
            console.log("Número de parte recibido:", noParte);
            console.log("Enviando datos:", JSON.stringify({ NoParte: noParte }));
            $.ajax({
                url: '/Tier2/consultarDetalleEtiquetas',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ NoParte: noParte }),

                success: function (response) {
                    console.log("Datos recibidos:", response);
                    if (response.success) {
                        $('#tablaDetalle tbody').empty();
                        response.data.forEach(function (item) {
                            var row = `<tr>
                                             <td>${item.NoParte}</td>
                                             <td>${item.NoEtiqueta}</td>
                                             <td>${item.Estado = 1 ? `En uso` : `En gaveta`}</td>
                                          </tr>`;
                            $('#tablaDetalle tbody').append(row);
                        });

                        $('#tablaDetalle tbody tr').show();
                    } else {
                        console.error("Error en la respuesta de detalle:", response.message);
                        $('#errorContainer').text('Error: ' + response.message).show();
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error en la petición de detalle:", error);
                    $('#errorContainer').text('Error al cargar los datos: ' + error).show();
                },
            });
        }
    }
    cargarEtiquetas();
});
