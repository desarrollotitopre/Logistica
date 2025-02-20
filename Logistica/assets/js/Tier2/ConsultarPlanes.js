document.addEventListener('DOMContentLoaded', function () {
    function cargarLineas(planSeleccionado) {
        console.log("Valor del plan seleccionado: " + planSeleccionado);
        $.ajax({
            url: '/Tier2/consultarListaLineas',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ planSeleccionado: planSeleccionado }),
            success: function (response) {
                console.log("Datos de líneas recibidos:", response);
                const selectLineas = $('#linea');
                selectLineas.empty();
                if (response.success && response.data) {
                    selectLineas.append($('<option>', {
                        value: "",
                        text: "Selecciona la línea",
                        selected: true,
                        disabled: true
                    }));
                    response.data.forEach(linea => {
                        selectLineas.append($('<option>', {
                            value: linea,
                            text: linea
                        }));
                    });
                    selectLineas.prop('disabled', false);
                    $('#planCompleto').prop('disabled', false);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error en la petición:", error);
                $('#errorContainer').text('Error al cargar las líneas: ' + error).show();
            },
            complete: function () {
                $('#spinner').hide();
            }
        });
    }

    document.getElementById('lineaColumn').style.display = 'none';

    $('#plan').change(function () {
        const planSeleccionado = $(this).val();
        console.log("Plan seleccionado:", planSeleccionado);
        const tabla = $('#tablaResumen tbody');
        tabla.empty();
        if (planSeleccionado && planSeleccionado !== "Selecciona") {
            cargarLineas(planSeleccionado);
        } else {
            $('#linea').empty().prop('disabled', true);
            $('#planCompleto').prop('disabled', true).prop('checked', false);
            $('#startDate').prop('disabled', true);
            $('#btnCargarDatos').prop('disabled', true);
            console.log("No se seleccionó plan");
        }
    });

    $('#linea').change(function () {
        const lineaSeleccionada = $(this).val();
        console.log("Línea seleccionada:", lineaSeleccionada);
        const tabla = $('#tablaResumen tbody');
        tabla.empty();
        if (lineaSeleccionada) {
            $('#startDate').prop('disabled', false);
        } else {
            $('#startDate').prop('disabled', true);
            $('#btnCargarDatos').prop('disabled', true);
        }
    });

    $('#planCompleto').change(function () {
        const tabla = $('#tablaResumen tbody');
        tabla.empty();
        if ($(this).is(':checked')) {
            $('#linea').prop('disabled', true);
            $('#startDate').prop('disabled', false);
            document.getElementById('lineaColumn').style.display = '';
        } else {
            document.getElementById('lineaColumn').style.display = 'none';
            $('#linea').prop('disabled', false);
            if (!$('#linea').val()) {
                $('#startDate').prop('disabled', true);
            }
        }
    });

    $('#startDate').change(function () {
        const fechaSeleccionada = $(this).val();
        console.log("Fecha seleccionada:", fechaSeleccionada);
        if (fechaSeleccionada) {
            $('#btnCargarDatos').prop('disabled', false);
        } else {
            $('#btnCargarDatos').prop('disabled', true);
        }
    });

    $('#modalDetalle .close, #modalDetalle .btn-secondary').on('click', function () {
        console.log("Botón de cerrar clickeado");
        $('#modalDetalle').modal('hide'); // Cierra el modal manualmente
    });
});

function obtenerDetalleEtiquetas(noParte, planSeleccionado, fechaSeleccionada) {
    const datos = {
        noParte: noParte,
        proceso: planSeleccionado,
        fecha: fechaSeleccionada
    };

    $.ajax({
        url: '/Tier2/obtenerDetalleEtiquetas',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(datos),
        success: function (response) {
            console.log("Detalle de etiquetas recibido:", response);
            if (response.success && response.data) {
                llenarTablaDetalle(response.data); // Llenar la tabla en el modal
            } else {
                alert("No se encontraron detalles para este No. Parte.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error en la petición:", error);
            alert("Error al obtener el detalle de las etiquetas: " + error);
        }
    });
}

function llenarTablaDetalle(datos) {
    const tabla = $('#tablaDetalleEtiquetas tbody');
    tabla.empty(); // Limpiar la tabla antes de llenarla

    datos.forEach(item => {
        const fila = `<tr>
        <td>${item}</td>
    </tr>`;
        tabla.append(fila);
    });    
}

function showResume() {
    const planSeleccionado = $('#plan').val();
    const lineaSeleccionada = $('#linea').val();
    const fechaSeleccionada = $('#startDate').val();
    const planCompleto = $('#planCompleto').is(':checked');

    if (!planSeleccionado || !fechaSeleccionada) {
        alert("Por favor, selecciona un plan, una línea y una fecha.");
        return;
    }

    const datos = {
        plan: planSeleccionado,
        linea: planCompleto ? null : lineaSeleccionada,
        fecha: fechaSeleccionada,
        planCompleto: planCompleto
    };

    $.ajax({
        url: '/Tier2/obtenerResumen',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(datos),
        success: function (response) {
            console.log("Datos recibidos:", response);
            if (response.success && response.data) {
                const planCompleto = $('#planCompleto').is(':checked');
                response.data.forEach(item => {
                    console.log(`Linea: ${item.Linea}, NoParte: ${item.NoParte}, CantidadNormal: ${item.CantidadEtiquetas}, CantidadParciales: ${item.EtiquetasParciales}, Usuario: ${item.UsuarioRecibio}, FechaEntregado: ${item.FechaEntrega}`);
                });
                llenarTabla(response.data, planCompleto);
            } else {
                alert("No se encontraron datos para mostrar.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error en la petición:", error);
            alert("Error al obtener el resumen: " + error);
        }
    });
}

function llenarTabla(datos, mostrarLinea) {
    const tabla = $('#tablaResumen tbody');
    tabla.empty();
    datos.forEach((item, index) => {
        const linea = item.Linea || 'N/A';
        const noParte = item.NoParte || 'N/A';
        const cantidadEtiquetas = item.CantidadEtiquetas || 0;
        const etiquetasParciales = item.EtiquetasParciales || '';
        const usuarioRecibido = item.UsuarioRecibio;
        const fechaEntrega = item.FechaEntrega;

        let tieneUsuario = false;

        if (usuarioRecibido !== null &&
            usuarioRecibido !== undefined &&
            typeof usuarioRecibido === 'string' &&
            usuarioRecibido.trim().length > 0) {
            tieneUsuario = true;
        }

        const fila = `<tr id="fila-${index}" 
                          data-no-parte="${noParte}">
            ${mostrarLinea ? `<td>${linea}</td>` : ''}
            <td style="font-size: 14px; font-weight: bold;">${noParte}</td>
            <td style="font-size: 14px; font-weight: bold;">${cantidadEtiquetas}</td>
            <td style="font-size: 14px; font-weight: bold;">${etiquetasParciales}</td>
            <td>${tieneUsuario ? `<i class="fas fa-check"></i> ${usuarioRecibido}` : `<i class="fas fa-times-circle"></i> No entregado aun`}</td>
            <td>${fechaEntrega}</td>
            </tr>`;

        tabla.append(fila);
    });

    $('#tablaResumen tbody').on('click', 'tr', function () {
        console.log("Clic en la fila :)");
        const noParte = $(this).data('no-parte');
        const planSeleccionado = $('#plan').val(); 
        const fechaSeleccionada = $('#startDate').val(); 

        $('#modalNoParte').text(noParte);
        obtenerDetalleEtiquetas(noParte, planSeleccionado, fechaSeleccionada); 
        $('#modalDetalle').modal('show');
    });
}