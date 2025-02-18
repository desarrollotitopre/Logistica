

function validarCampos(input_FechaPlanM, input_HrllegadaPlanM, input_HrSalidaPlanM) {
    var errorMessages = [];
    var isValid = true;

    input_FechaPlanM.css('border', '');
    input_HrllegadaPlanM.css('border', '');
    input_HrSalidaPlanM.css('border', '');


    var fechaPlaneada = moment(input_FechaPlanM.val(), 'DD/MM/YYYY');
    var horaLlegada = moment(input_HrllegadaPlanM.val(), 'DD/MM/YYYY HH:mm');
    var horaSalida = moment(input_HrSalidaPlanM.val(), 'DD/MM/YYYY HH:mm');
    var fechaActual = moment(); 
    var fechaMaxima = moment().add(1, 'year');


    if (!fechaPlaneada.isValid()) {
        input_FechaPlanM.css('border', '2px solid red');
        errorMessages.push('Fecha planeda no válida.');
        isValid = false;
    }


    if (fechaPlaneada.isBefore(fechaActual, 'day')) {
        input_FechaPlanM.css('border', '2px solid red');
        errorMessages.push('La fecha planeada no puede ser anterior a la fecha actual.');
        isValid = false;
    }


    if (horaLlegada.isAfter(fechaMaxima, 'day')) {
        input_HrllegadaPlanM.css('border', '2px solid red');
        errorMessages.push('La fecha planeada no puede superar un año desde la fecha actual.');
        errorFound = true;
    }

    if (!horaLlegada.isValid()) {
        input_HrllegadaPlanM.css('border', '2px solid red');
        errorMessages.push('Hora de llegada no válida.');
        isValid = false;
    }
    if (!horaSalida.isValid()) {
        input_HrSalidaPlanM.css('border', '2px solid red');
        errorMessages.push('Hora de salida no válida.');
        isValid = false;
    }


    if (!fechaPlaneada.isSame(horaLlegada, 'day')) {
        input_HrllegadaPlanM.css('border', '2px solid red');
        errorMessages.push('La fecha de la Hora de Llegada no coincide con la Fecha Planeada.');
        isValid = false;
    }


    if (!horaLlegada.isSameOrBefore(horaSalida)) {
        input_HrSalidaPlanM.css('border', '2px solid red');
        errorMessages.push('La hora de salida no puede ser igual o antes de la hora de llegada.');
        isValid = false;
    }

    if (!isValid) {
        $('#errorContainerM').html(errorMessages.join('<br>')).show();

    } else {

        $('#errorContainerM').hide();
    }


    return { isValid: isValid, errorMessages: errorMessages };
}



function agregarFilaManual(e) {

    let isValid = true;
    let modal = $(e.target).closest('.modal');
    let formFields = modal.find('.form-control[required]');
    formFields.each(function () {
        if ($(this).val().trim() === "") {
            $(this).css('border', '2px solid red');
            isValid = false;
        } else {
            $(this).css('border', '');
        }
    });

    var input_FechaPlanM = $('#input_FechaPlanM');
    var input_HrllegadaPlanM = $('#input_HrllegadaPlanM');
    var input_HrSalidaPlanM = $('#input_HrSalidaPlanM');
    var input_ASNM = $('#input_ASNM').val();
    var input_ProveedorM = $('#input_ProveedorM').val();
    var input_ProyectoM = $('#input_ProyectoM').val();
    var input_LineaTransM = $('#input_LineaTransM').val();
    var input_UnidadM = $('#input_UnidadM').val();
    var input_CargaM = $('#input_CargaM').val();
    var select_AreaM = $('#select_AreaM').val();
    var input_ProcesoM = $('#input_ProcesoM').val();

    // Llama a la función de validación
    var validation = validarCampos(input_FechaPlanM, input_HrllegadaPlanM, input_HrSalidaPlanM);

    if (isValid && validation.isValid === true) {
        var nuevaFila = `<tr>
            <td>${input_FechaPlanM.val()}</td>
            <td>${input_ProveedorM}</td>
            <td>${input_ProyectoM}</td>
            <td>${input_UnidadM}</td>
            <td>${input_LineaTransM}</td>
            <td>${input_CargaM}</td>
  
            <td>${input_HrllegadaPlanM.val() }</td>
            <td>${input_HrSalidaPlanM.val() }</td>
            <td>${input_ProcesoM}</td>
            <td>${input_ASNM}</td>

            <td>${select_AreaM}</td>
        </tr>`;

        $('#csvPreviewTable tbody').append(nuevaFila);
        $('#modalCargaManual').modal('hide');
        $('#btnInsertar').show();

    } else {
        toastr.warning('Por favor, complete todos los campos requeridos.');
    }
}







function abrirModalManual() {
    $('#modalCargaManual').modal('show');
    $('#errorContainer2').hide();


    $("#input_FechaPlanM").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker('setDate', new Date());



    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '&#x3c;Ant',
        nextText: 'Sig&#x3e;',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
            'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
            'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['es']);


    $('#input_HrllegadaPlanM').datetimepicker({
        dateFormat: 'dd/mm/yy',
        timeFormat: 'HH:mm',
        showSecond: true,
        hourGrid: 4,
        minuteGrid: 10,
        secondGrid: 10,
        stepHour: 1,
        stepMinute: 1,
        stepSecond: 1,
        showMinute: false,
        showSecond: false,
        beforeShow: function (input, inst) {
            var offset = $(input).offset();
            var height = $(input).outerHeight();
            window.setTimeout(function () {
                inst.dpDiv.css({ top: (offset.top + height) + 'px', left: offset.left + 'px' });
            }, 1);
        }
    });

    $('#input_HrSalidaPlanM').datetimepicker({
        dateFormat: 'dd/mm/yy',
        timeFormat: 'HH:mm',
        showSecond: true,
        hourGrid: 4,
        minuteGrid: 10,
        secondGrid: 10,
        stepHour: 1,
        stepMinute: 1,
        stepSecond: 1,
        showMinute: false,
        showSecond: false,
        beforeShow: function (input, inst) {
            var offset = $(input).offset();
            var height = $(input).outerHeight();
            window.setTimeout(function () {
                inst.dpDiv.css({ top: (offset.top + height) + 'px', left: offset.left + 'px' });
            }, 1);
        }
    });
}

var dataTableInitialized = false;

function cargarCSV(file) {
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var tableBody = $('#csvPreviewTable tbody');
            tableBody.empty();

            var rows = e.target.result.split('\n');
            var header = rows[0].split(','); // Obtener la cabecera del CSV

            var errorFound = false; // Bandera para indicar si se encuentra un error
            var errorMessages = []; // Lista de mensajes de error

            // Iterar sobre cada fila del CSV
            for (var i = 1; i < rows.length; i++) {
                var rowData = rows[i].trim(); // Eliminar espacios adicionales al inicio y al final
                if (rowData !== '') { // Verificar si la fila no está vacía
                    var columns = rowData.split(',');
                    var allColumnsEmpty = columns.every(function (column) {
                        return column.trim() === '';
                    });

                    if (allColumnsEmpty) {
                        continue; // Saltar esta fila si todas las columnas están vacías
                    }
                    // Verificar la cantidad de columnas
                    if (columns.length !== header.length || header.length !== $('#csvPreviewTable thead th').length) {
                        errorMessages.push('Error: La fila ' + (i) + ' tiene un número incorrecto de columnas.');
                        errorFound = true;
                        continue; // Saltar a la siguiente fila si hay un error
                    }

                    var fechaIndex = header.map(function (column) {
                        return column.trim();
                    }).indexOf("FECHA");

                    var horaLlegadaIndex = header.map(function (column) {
                        return column.trim();
                    }).indexOf("HR_LLEGADA");

                    var horaSalidaIndex = header.map(function (column) {
                        return column.trim();
                    }).indexOf("HR_SALIDA");

                    var areaIndex = header.map(function (column) {
                        return column.trim();
                    }).indexOf("AREA");

                    var asnIndex = header.map(function (column) {
                        return column.trim();
                    }).indexOf("ASN");

                    // Validar la hora de llegada y hora de salida
                    var horaLlegadaPlan = columns[horaLlegadaIndex].trim().replace('a.m.', '').replace('p.m.', '');
                    var horaSalidaPlan = columns[horaSalidaIndex].trim().replace('a.m.', '').replace('p.m.', '');
                    var fechaPlaneadaStr = columns[fechaIndex].trim();

                    var fechaPlaneadaStr = columns[fechaIndex].trim();

                    var timeLlegada = moment(horaLlegadaPlan, 'DD/MM/YYYY HH:mm');
                    var timeSalida = moment(horaSalidaPlan, 'DD/MM/YYYY HH:mm');
                    console.log(columns[horaLlegadaIndex].trim());
                    console.log(columns[horaSalidaIndex].trim());
                    console.log("=================================");
                    console.log(timeLlegada);
                    console.log(timeSalida);
                    var fechaPlaneada = moment(fechaPlaneadaStr, 'DD/MM/YYYY');

                    // Validar formato y consistencia de las horas
                    if (!timeLlegada.isValid() || !timeSalida.isValid() || !fechaPlaneada.isValid()) {
                        errorMessages.push('Error: En la fila ' + (i) + ', la fecha o la hora tiene un formato incorrecto.');
                        errorFound = true;
                    } else {

                        var fechaActual = moment(); // Fecha y hora actual
                        var fechaMaxima = moment().add(1, 'year'); // 1 año desde la fecha actual

                        if (fechaPlaneada.isBefore(fechaActual, 'day')) {
                            errorMessages.push('Error: En la fila ' + (i) + ', la fecha planeada no puede ser anterior a la fecha actual.');
                            errorFound = true;
                        }

                        if (fechaPlaneada.isAfter(fechaMaxima, 'day')) {
                            errorMessages.push('Error: En la fila ' + (i) + ', la fecha planeada no puede superar un año desde la fecha actual.');
                            errorFound = true;
                        }

                        if (!timeLlegada.isSameOrBefore(timeSalida)) {
                            errorMessages.push('Error: En la fila ' + (i) + ', la Hora y fecha de Salida planificada no puede ser menor que la fecha Hora de Salida Planificada.');
                            errorFound = true;
                        }

                        // Validar que la fecha en hora de llegada sea igual a la fecha planeada
                        if (!fechaPlaneada.isSame(timeLlegada, 'day')) {
                            errorMessages.push('Error: En la fila ' + (i) + ', la fecha en la Hora de Llegada no coincide con la Fecha Planeada.');
                            errorFound = true;
                        }
                    }

                    var areaValue = columns[areaIndex].trim().toUpperCase()

                    if (areaValue !== "LOGISTICA" && areaValue !== "ROLLOS" && areaValue !== "INDIRECTOS") {
                        errorMessages.push('Error: En la fila ' + (i) + ', el valor en la columna "Área" debe ser LOGISTICA, ROLLOS o INDIRECTOS.');
                        errorFound = true;
                    }

                    // Crear una nueva fila para esta entrada de CSV
                    var $row = $('<tr>');
                    // Verificar si alguna columna está en blanco
                    var missingFields = [];
                    columns.forEach(function (column, index) {

                        if (column.trim() === '' && index !== asnIndex) {
                            missingFields.push(header[index]);
                            errorFound = true;
                            // Resaltar el campo problemático en la fila actual
                            $row.append('<td class="campo-invalido">' + column + '</td>');
                        } else if (index === areaIndex) {
                            var areaValue = column.trim().toUpperCase();
                            if (areaValue !== "LOGISTICA" && areaValue !== "ROLLOS" && areaValue !== "INDIRECTOS") {
                                $row.append('<td class="campo-invalido">' + column + '</td>');
                            } else {
                                $row.append('<td>' + column + '</td>');
                            }
                        } else if (index === fechaIndex) {
                            if (!fechaPlaneada.isValid() || fechaPlaneada.isBefore(moment(), 'day') || fechaPlaneada.isAfter(moment().add(1, 'year'), 'day')) {
                                $row.append('<td class="campo-invalido">' + column + '</td>');
                            } else {
                                $row.append('<td>' + column + '</td>');
                            }

                        } else if (index === horaLlegadaIndex) {
                            if (!timeLlegada.isValid() || !fechaPlaneada.isSame(timeLlegada, 'day')) {
                                $row.append('<td class="campo-invalido">' + column + '</td>');
                            } else {
                                $row.append('<td>' + column.replace('a.m.', '').replace('p.m.', '') + '</td>');
                            }

                        } else if (index === horaSalidaIndex) {
                            if (!timeSalida.isValid() || !timeLlegada.isSameOrBefore(timeSalida)) {
                                $row.append('<td class="campo-invalido">' + column + '</td>');
                            } else {
                                $row.append('<td>' + column.replace('a.m.', '').replace('p.m.', '') + '</td>');
                            }
                        } else {
                            $row.append('<td>' + column + '</td>');
                        }
                    });

                    if (missingFields.length > 0) {
                        errorMessages.push('Error: En la fila ' + (i) + ' faltan los siguientes campos: ' + missingFields.join(', '));
                    }
                    if (areaValue !== "LOGISTICA" && areaValue !== "ROLLOS" && areaValue !== "INDIRECTOS") {
                        $row.find('td').eq(areaIndex).addClass('campo-invalido');
                    }
                    tableBody.append($row);
                }
            }

            if (errorFound) {
                $('#errorContainer').html(errorMessages.join('<br>')).show();
                $('#btnInsertar').hide();
            } else {
                $('#errorContainer').hide();
                $('#btnInsertar').show();
            }

            if (dataTableInitialized) {
                $('#csvPreviewTable').DataTable().clear().destroy();
            }
            dataTableInitialized = true;

            $('#csvPreviewTable').DataTable({
                paging: false,
                "destroy": true,
                responsive: true,
                ordering: false,
                lengthChange: false,
                searching: false,
            });
            $('#fileInput').val('');
        };
        reader.readAsText(file);
    }
}

function limpiarArchivo() {
    $('#fileInput').val('');
}

function limpiarTabla() {
    if (dataTableInitialized) {
        $('#csvPreviewTable').DataTable().clear().destroy();
        dataTableInitialized = false;
    }
    $('#csvPreviewTable tbody').empty();
    $('#errorContainer').hide();
    $('#btnInsertar').hide();
}

$('#fileInput').change(function () {
    cargarCSV(this.files[0]);
});

function insertarDatos() {
    var data = [];
    $('#csvPreviewTable tbody tr').each(function () {
        var row = [];
        $(this).find('td').each(function () {
            row.push($(this).text());
        });
        data.push(row);
    });


    $.ajax({
        url: "/Transportes/InsertarDatosCSV",
        type: 'POST',
        contentType: 'application/json', // Asegúrate de definir el tipo de contenido como JSON
        data: JSON.stringify({ datosCSV: data, nombreUsuario: nombreUsuario }),
        success: function (response) {
            if (response.insertados >= 0 && response.total === response.insertados) {
                toastr.success("Se cargaron " + response.insertados + " de " + response.total  + " registros correctamente");
                limpiarTabla();
            } else {
                toastr.error("Error al cargar CSV: " + response.Message + "\nSe cargaron " + response.insertados + " de " + response.total + " registros correctamente");
            }
        },
        error: function () {
            toastr.error("Ha ocurrido un error al cargar los datos del CSV.");
        }
    });
}