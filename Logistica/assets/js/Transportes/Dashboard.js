var tablePlan;
var intervalId;
var currentData = [];
var modalIsOpen = false;

$("#fechaPlaneado").datepicker({
    dateFormat: 'yy-mm-dd',
    onSelect: function (dateText, inst) {
        var formattedDate = $.datepicker.formatDate('yy-mm-dd', $(this).datepicker('getDate'));
        getPlanTransporte(formattedDate);
        getFilterCustomers(formattedDate);
    }
}).datepicker('setDate', new Date());
$.datepicker.setDefaults($.datepicker.regional['es']);

function startInterval() {
    intervalId = setInterval(function () {
        if (!modalIsOpen) {
            getPlanTransporte($("#fechaPlaneado").val());
        }
    }, 5000);
}

function stopInterval() {
    clearInterval(intervalId);
}
function getPlanTransporte(Fecha) {
    console.log("Real Time");
    jQuery.ajax({
        url: "/Transportes/getTablePlan",
        type: "POST",
        data: JSON.stringify({ dateFind: Fecha }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var newData = data.data;
            if (!data.SessionActive) {
                document.getElementById("spinner").style.display = "block";
                window.location.href = signoutUrl;
            } else {
                // Procesar los datos recibidos normalmente
                if (!arraysEqual(currentData, newData)) {
                    currentData = newData;
                    updateTable(newData);
                }
            }
        },
        error: function (xhr, status, error) {
            toastr.error(
                "Error al intentar obtener datos en tiempo real. Posibles causas:" +
                "<ul>" +
                "<li>Sesión Expirada</li>" +
                "<li>Desconexión de red</li>" +
                "<li>Error en el servidor</li>" +
                "</ul>" + error
            );
        }
    });
}


function getFilterCustomers(Fecha) {

    jQuery.ajax({
        url: "/Transportes/getFilterCustomers",
        type: "POST",
        data: JSON.stringify({ dateFind: Fecha }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var newData = data.data;

            var select = $('#cliente'); // El select donde se llenarán los proveedores
            select.empty(); // Limpiamos el select antes de llenarlo
            var option = $('<option></option>').val("ALL").text("ALL");
            select.append(option);
            // Recorrer el array de proveedores
            $.each(data.data, function (index, proveedor) {
                // Crear un nuevo option por cada proveedor
                var option = $('<option></option>').val(proveedor.Proveedor).text(proveedor.Proveedor);
                select.append(option); // Agregar el option al select
            });

            if (tablePlan) {
                tablePlan.column(5).search("").draw();
                tablePlan.column(6).search("").draw();
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al intentar obtener filtro clientes");
        }
    });
}

function arraysEqual(a, b) {
    if (a.length !== b.length) return false;
    for (var i = 0; i < a.length; i++) {
        if (JSON.stringify(a[i]) !== JSON.stringify(b[i])) return false;
    }
    return true;
}

function updateTable(data) {
    //console.log(data);
    if (Tipo === 4) {

        data = data.filter(function (item) {

            return item.Area === "LOGISTICA" && item.Proveedor.trim() == "INSITE";
        });
    }


    if (!$.fn.DataTable.isDataTable('#tablePlan')) {
        tablePlan = $('#tablePlan').DataTable({
            data: data,
            responsive: true,
            ordering: true,
            lengthChange: false,
            searching: true,
            paging: false, // Desactiva la paginación
            pageLength: data.length,
            columns: [
                { "data": "Folio" },
                { "data": "ASN" },
                { "data": "FechaPlan" },
                {
                    "data": "HrLlegadaPlan",
                    "render": function (data, type, row) {
                        if (data != "") {
                            var formattedDate = moment(data, "DD/MM/YYYY h:mm:ss a").format("DD/MM/YYYY HH:mm");
                            return formattedDate;
                        } else {
                            return "";
                        }
                    }
                },
                {
                    "data": "HrSalidaPlan", "render": function (data, type, row) {
                        var formattedDate = moment(data, "DD/MM/YYYY h:mm:ss a").format("DD/MM/YYYY HH:mm");
                        return formattedDate;
                    }
                },
                { "data": "Proveedor", "createdCell": function (cell) { $(cell).addClass('proveedor'); } },
                { "data": "Area" },
                { "data": "Proceso" },
                { "data": "LineaTrans" },
                {
                    "data": "LlegadaReal", "render": function (data, type, row) {
                        if (data != "") {
                            var formattedDate = moment(data, "DD/MM/YYYY h:mm:ss a").format("DD/MM/YYYY HH:mm");
                            return formattedDate;
                        } else {
                            return "";
                        }
                    }
                },
                { "data": "Rampa" },
                {
                    "data": "EntRampaReal", "render": function (data, type, row) {
                        if (data != "") {
                            var formattedDate = moment(data, "DD/MM/YYYY h:mm:ss a").format("DD/MM/YYYY HH:mm");
                            return formattedDate;
                        } else {
                            return "";
                        }
                    }
                },
                {
                    "data": "SalRampaReal", "render": function (data, type, row) {
                        if (data != "") {
                            var formattedDate = moment(data, "DD/MM/YYYY h:mm:ss a").format("DD/MM/YYYY HH:mm");
                            return formattedDate;
                        } else {
                            return "";
                        }
                    }
                },
                {
                    "data": "SalidaReal", "render": function (data, type, row) {
                        if (data != "") {
                            var formattedDate = moment(data, "DD/MM/YYYY h:mm:ss a").format("DD/MM/YYYY HH:mm");
                            return formattedDate;
                        } else {
                            return "";
                        }
                    }
                },
                {
                    "data": "Status",
                    "render": function (data, type, row, meta) {
                        var buttons = '<div style="display: flex; align-items: center; justify-content: center; gap: 10px;">';
                        if (Tipo === 1) {

                            if (data == 6) {
                                buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Ver" title="Ver info del Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/ver.png" alt="Truck Icon" class="truck-icon"></button>' +
                                    '<button type="button" class="btnCustom btn-transport btn-Limpiar" title="Limpiar Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/reiniciar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                    '</div>';
                            } else {

                                if (data != 0 && data != 6) {
                                    buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Transportista" title="Ver info del Transportista" data-row-index="' + meta.row + '"><img src="/assets/Imgs/camion.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Ver" title="Ver info del Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/ver.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Editar" title="Editar Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/editar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Regresar" title="Regresar estatus anterior" data-row-index="' + meta.row + '"><img src="/assets/Imgs/regresar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Limpiar" title="Limpiar Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/reiniciar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Cancelar" title="Cancelar Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/cancelar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '</div>';
                                } else {
                                    buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Ver" title="Ver info del Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/ver.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Editar" title="Editar Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/editar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '<button type="button" class="btnCustom btn-transport btn-Cancelar" title="Cancelar Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/cancelar.png" alt="Truck Icon" class="truck-icon"></button>' +
                                        '</div>';
                                }
                            }

                            return buttons;

                        } else if (Tipo === 2 || Tipo === 3) {
                            buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Ver" title="Ver info del Folio" data-row-index="' + meta.row + '"><img src="/assets/Imgs/ver.png" alt="Truck Icon" class="truck-icon"></button>';
                            if (data != 0 && data != 6) {

                                buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Transportista" title="Ver info del Transportista" data-row-index="' + meta.row + '"<img src="/assets/Imgs/camion.png" alt="Truck Icon" class="truck-icon"></button>';
                                if ((data != 2 && data != 3 && data != 4) && Tipo === 3) {
                                    buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Regresar" title="Regresar estatus anterior" data-row-index="' + meta.row + '"><img src="/assets/Imgs/regresar.png" alt="Truck Icon" class="truck-icon"></button>';
                                }

                                if ((data != 1 && data != 5) && Tipo === 2) {
                                    buttons = buttons + '<button type="button" class="btnCustom btn-transport btn-Regresar" title="Regresar estatus anterior" data-row-index="' + meta.row + '"><img src="/assets/Imgs/regresar.png" alt="Truck Icon" class="truck-icon"></button>';
                                }

                            }
                            return buttons + '</div>';
                        } else {
                            return '';
                        }
                    }
                }
            ],
            "columnDefs": [
                { "searchable": true, "targets": [0] },
                { "searchable": false, "targets": [1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14] },
                { "orderable": true, "targets": [0, 5] },
                { "orderable": false, "targets": [1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14] }
            ],
            searchPlaceholder: 'Folio',
            "createdRow": function (row, data, dataIndex) {
                if (data.Status == 0) {
                    $(row).addClass('upload');
                }
                if (data.Status == 1) {
                    $(row).addClass('register');
                }
                if (data.Status == 2) {
                    $(row).addClass('assignedRamp');
                }
                if (data.Status == 3) {
                    $(row).addClass('rampEntrance');
                }
                if (data.Status == 4) {
                    $(row).addClass('rampExit');
                }
                if (data.Status == 5) {
                    $(row).addClass('plantExit');
                }
                if (data.Status == 6) {
                    $(row).addClass('cancelled');
                }
            }

        });
        // Aplicar clases a las filas secundarias cuando se muestran o se redimensiona la tabla
        $('#tablePlan').on('responsive-resize', function (e, datatable, columns) {
            tablePlan.rows().every(function () {
                var row = this.node();
                var data = this.data();
                applyRowClasses(row, data); // Aplicar clases CSS a las filas secundarias
            });
        });
    } else {
        tablePlan.clear().rows.add(data).draw();
    }
}


function applyRowClasses(row, data) {
    $(row).removeClass('upload register assignedRamp rampEntrance rampExit plantExit cancelled'); // Limpiar clases anteriores
    if (data.Status == 0) {
        $(row).addClass('upload');
    }
    if (data.Status == 1) {
        $(row).addClass('register');
    }
    if (data.Status == 2) {
        $(row).addClass('assignedRamp');
    }
    if (data.Status == 3) {
        $(row).addClass('rampEntrance');
    }
    if (data.Status == 4) {
        $(row).addClass('rampExit');
    }
    if (data.Status == 5) {
        $(row).addClass('plantExit');
    }
    if (data.Status == 6) {
        $(row).addClass('cancelled');
    }
}


getPlanTransporte($("#fechaPlaneado").val());
getFilterCustomers($("#fechaPlaneado").val());

$('#area').change(function () {
    var selectedArea = $(this).val();
    if (tablePlan) {
        if (selectedArea === "GENERAL") {
            tablePlan.column(6).search("").draw();
        } else {
            tablePlan.column(6).search(selectedArea.trim().toUpperCase()).draw();
        }
    }
});

$('#cliente').change(function () {
    var selectedCustomer = $(this).val();
    if (tablePlan) {
        if (selectedCustomer === "ALL") {
            tablePlan.column(5).search("").draw();
        } else {
            tablePlan.column(5).search(selectedCustomer.trim().toUpperCase()).draw();
        }
    }
});


document.getElementById('searchFolio').addEventListener('input', function () {
    var folio = this.value;
    if (tablePlan) {
        if (folio === "") {
            tablePlan.column(0).search("").draw();
        } else {
            tablePlan.column(0).search(folio.trim().toUpperCase()).draw();
        }
    }
});


$('#tablePlan tbody').on('click', 'td:nth-child(1)', function () {
    console.log("clic");
    //$('#tablePlan tbody').on('click', 'tr', function () {
    document.getElementById("datosRampa").style.display = "none";

    var rowData = tablePlan.row(this).data();
    var folio = rowData.Folio;
    var status = rowData.Status;

    $('#modalFolio').text(folio);

    var nextStepLabel = "";
    var btnColor = "";
    switch (status) {
        case 0:
            nextStepLabel = "Registrar";
            btnColor = "#d3d3d3";
            break;
        case 1:
            nextStepLabel = "Asignar Rampa";
            btnColor = "#c96e9f";

            document.getElementById("datosRampa").style.display = "block";
            break;
        case 2:
            nextStepLabel = "Entrada Rampa";
            btnColor = "#0573b5";
            break;
        case 3:
            nextStepLabel = "Salida Rampa";
            btnColor = "#F5B514";
            break;
        case 4:
            nextStepLabel = "Salida Planta";
            btnColor = "#005d00";
            break;
    }
    $('#nextStepBtn').text(nextStepLabel);
    $('#nextStepBtn').css('background-color', btnColor);
    $('#nextStepBtn').data('Folio', folio);
    $('#nextStepBtn').data('Status', status);

    var tipoUsuario = Tipo;

    if (Tipo === 4) {
        toastr.warning("Solo tiene permisos de lectura");
        return;
    }

    if (status == 5) {
        toastr.success("El folio " + folio + " ya ha terminado su proceso");
        return;
    }
    if (status == 6) {
        toastr.error("El folio " + folio + " ha sido marcado como cancelado");
        return;
    } else {
        if (tipoUsuario == 1 || tipoUsuario == 2) {
            if (status == 0 && tipoUsuario == 1) {
                $('#nextStepBtn2').data('Folio', folio);
                $('#nextStepBtn2').data('Status', status);
                $('#nextStepBtn2').text(nextStepLabel);
                $('#nextStepBtn2').css('background-color', btnColor);
                $('#modalFolio2').text(folio);
                $('#statusModal2').modal('show');
            } else {
                if ((status == 0 || status == 2 || status == 4) && tipoUsuario == 2) {
                    toastr.warning("Solo tiene permisos para asignar y registrar salidas de rampa");
                } else {
                    $('#statusModal').modal('show');
                }
            }

        } else {
            if (tipoUsuario == 3) {
                if (status == 0 || status == 2 || status == 4) {
                    if (status == 0) {
                        $('#nextStepBtn2').data('Folio', folio);
                        $('#nextStepBtn2').data('Status', status);
                        $('#nextStepBtn2').text(nextStepLabel);
                        $('#nextStepBtn2').css('background-color', btnColor);
                        $('#modalFolio2').text(folio);
                        $('#statusModal2').modal('show');
                    } else {
                        $('#statusModal').modal('show');
                    }
                }
                else {
                    toastr.warning("Solo tiene permisos para registrar entradas a rampa así como entradas y salidas de transporte");
                    return;
                }
            } else {
                toastr.warning("Solo tiene permisos de lectura");
                return;
            }
        }
    }
});


function validateRamp() {
    var rampaAsignado = document.getElementById("rampaAsignado").value.trim();
    var Status = $('#nextStepBtn').data('Status');
    if (rampaAsignado === "" && Status == 1) {
        toastr.warning("Por favor ingresa la rampa");
    } else {
        changeStatusPlan();
    }
}

function changeStatusPlan() {
    var Folio = $('#nextStepBtn').data('Folio');
    var Status = $('#nextStepBtn').data('Status');
    var Rampa = $('#rampaAsignado').val() ? $('#rampaAsignado').val() : "";

    $('#rampaAsignado').val("");
    document.getElementById("datosRampa").style.display = "none";
    jQuery.ajax({
        url: "/Transportes/changeStatusPlan",
        type: "POST",
        data: JSON.stringify({ Folio: Folio, Status: Status, Rampa: Rampa, AutorizedBy: AutorizedBy }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.result == true) {
                switch (Status) {
                    case 0:
                        toastr.success("Transporte registrado correctamente");
                        break;
                    case 1:
                        toastr.success('Asignación de rampa correctamente');
                        break;
                    case 2:
                        toastr.success('Entrada de rampa correctamente');
                        break;
                    case 3:
                        toastr.success('Salida de rampa correctamente');
                        break;
                    default:
                        toastr.success('Registro de salida de planta correctamente');
                        break;
                }
                getPlanTransporte($("#fechaPlaneado").val());
            } else {
                toastr.error("Error al actulizar el estatus del Folio");
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud: " + error);
        }
    });
    $('#statusModal').modal('hide');
}

function validateFormTrans() {
    var transportista = document.getElementById("transportista").value.trim();
    var empresa = document.getElementById("empresa").value.trim();
    var placas = document.getElementById("placas").value.trim();
    var origen = document.getElementById("origen").value.trim();
    var gafete = document.getElementById("gafete").value.trim();
    var contenedor = document.getElementById("contenedor").value.trim();
    var tracto = document.getElementById("tracto").value.trim();

    if (transportista === "" || empresa === "" || placas === "" || origen === "" || gafete === "" || contenedor === "" || tracto === "") {
        toastr.warning("Por favor completa todos los campos para poder continuar.!");
    } else {
        insertDataTrans();
    }
}

function insertDataTrans() {
    var Folio = $('#nextStepBtn2').data('Folio');

    var transportista = $('#transportista').val();
    var Empresa = $('#empresa').val();
    var placas = $('#placas').val();
    var origen = $('#origen').val();
    var gafete = $('#gafete').val();
    var contenedor = $('#contenedor').val();
    var tracto = $('#tracto').val();

    jQuery.ajax({
        url: "/Transportes/inserDataRegsProvsT2",
        type: "POST",
        data: JSON.stringify({ Folio: Folio, AutorizedBy: AutorizedBy, transportista: transportista, Empresa: Empresa, placas: placas, origen: origen, gafete: gafete, contenedor: contenedor, tracto: tracto }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.result == true) {
                transportista = $('#transportista').val("");
                compania = $('#empresa').val("");
                placas = $('#placas').val("");
                origen = $('#origen').val("");
                gafete = $('#gafete').val("");
                contenedor = $('#contenedor').val("");
                tracto = $('#tracto').val("");

                toastr.success("Transporte registrado correctamente");
                getPlanTransporte($("#fechaPlaneado").val());
            } else {
                toastr.error("Error al actulizar al status 2");
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud: " + error);
        }
    });

    $('#statusModal2').modal('hide');
}

$("#tablePlan tbody").on("click", '.btn-Transportista', function () {
    var rowIndex = $(this).data('row-index');
    var dataFolio = tablePlan.row(rowIndex).data();

    $.ajax({
        url: "/Transportes/getDataTransportista",
        type: 'POST',
        data: JSON.stringify({ Folio: dataFolio.Folio }), // Pasar el nombre de usuario como un parámetro adicional
        contentType: 'application/json',
        success: function (response) {
            var data = response.data;
            console.log(data);
            if (data && data.length > 0) {
                var transportistaData = data[0];

                $('#modalFolio3').text(dataFolio.Folio);
                $('#transportistaR').val(transportistaData.Nombre);
                $('#empresaR').val(transportistaData.Empresa);
                $('#placasR').val(transportistaData.Placas);
                $('#origenR').val(transportistaData.Origen);
                $('#gafeteR').val(transportistaData.Gafete);
                $('#contenedorR').val(transportistaData.Contenedor);
                $('#tractoR').val(transportistaData.Tracto);

                $('#statusModal3').modal('show');
            } else {
                toastr.error("El Folio " + dataFolio.Folio + " no tiene registrado datos del transportista");
            }
        },
        error: function () {
            toastr.error("Ha ocurrido realizar la peticion ajax para obtener datos del transportista");

        }
    });

});



$("#tablePlan tbody").on("click", '.btn-Regresar', function () {

    var rowIndex = $(this).data('row-index');
    var dataFolio = tablePlan.row(rowIndex).data();


    $('#confirmActionModal').modal('show');
    document.getElementById('Message').textContent = 'Desea regresar a un paso anterior el Folio ' + dataFolio.Folio + '?';
    $('#confirmBtn').data('Folio', dataFolio.Folio);
    var confirmBtn = document.getElementById("confirmBtn");
    confirmBtn.setAttribute('onclick', 'crudFolio(1)');
});

$("#tablePlan tbody").on("click", '.btn-Limpiar', function () {
    var rowIndex = $(this).data('row-index');
    var dataFolio = tablePlan.row(rowIndex).data();
    $('#confirmActionModal').modal('show');
    document.getElementById('Message').textContent = 'Esta seguro que desea limpiar completamente el Folio ' + dataFolio.Folio + '?';
    $('#confirmBtn').data('Folio', dataFolio.Folio);
    var confirmBtn = document.getElementById("confirmBtn");
    confirmBtn.setAttribute('onclick', 'crudFolio(2)');
});

$("#tablePlan tbody").on("click", '.btn-Cancelar', function () {
    var rowIndex = $(this).data('row-index');
    var dataFolio = tablePlan.row(rowIndex).data();
    $('#confirmActionModal').modal('show');
    document.getElementById('Message').textContent = 'Esta seguro que desea cancelar el Folio ' + dataFolio.Folio + '?';
    $('#confirmBtn').data('Folio', dataFolio.Folio);
    var confirmBtn = document.getElementById("confirmBtn");
    confirmBtn.setAttribute('onclick', 'crudFolio(3)');
});



$("#tablePlan tbody").on("click", '.btn-Editar', function () {
    var rowIndex = $(this).data('row-index');
    var dataFolio = tablePlan.row(rowIndex).data();
    $('#getDetailFolio').modal('show');
    $('#titleModalInfoFolio').text(dataFolio.Folio);

    if (!document.getElementById("actualizarFolio")) {
        var nuevoBoton = Object.assign(document.createElement("button"), {
            type: "button",
            className: "btn btn-outline-success",
            id: "actualizarFolio",
            textContent: "ACTUALIZAR",
            onclick: validar
        });
        nuevoBoton.style.width = "100% !important";

        document.getElementById("buttonContainer").appendChild(nuevoBoton);
    }

    getDetailFolio(dataFolio.Folio, 1);

});

$("#tablePlan tbody").on("click", '.btn-Ver', function () {
    var rowIndex = $(this).data('row-index');
    var dataFolio = tablePlan.row(rowIndex).data();

    $('#getDetailFolio').modal('show');
    $('#titleModalInfoFolio').text(dataFolio.Folio);
    var boton = document.getElementById("actualizarFolio");
    if (boton) {
        boton.remove();
    }
    getDetailFolio(dataFolio.Folio, 2);

});

function crudFolio(type) {
    if (type == 1) {
        resetearStepFolio();
    } else if (type == 2) {
        resetearFullFolio();
    } else {
        cancelarFolio();
    }
}

function resetearStepFolio() {

    jQuery.ajax({
        url: "/Transportes/resetStepFolio",
        type: "POST",
        data: JSON.stringify({ Folio: $('#confirmBtn').data('Folio'), Tipo: Tipo, AutorizedBy: AutorizedBy }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.result > 0) {
                toastr.success(data.Message);
                getPlanTransporte($("#fechaPlaneado").val());
            } else {
                toastr.error('ERROR: ' + data.Message);
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud de reseteo de folio: " + error);
        }

    });
    $('#confirmActionModal').modal('hide');
}

function resetearFullFolio() {

    jQuery.ajax({
        url: "/Transportes/resetFullFolio",
        type: "POST",
        data: JSON.stringify({ Folio: $('#confirmBtn').data('Folio'), AutorizedBy: AutorizedBy }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.result == true) {
                $('#folio').val("");
                toastr.success("Folio  " + $('#confirmBtn').data('Folio') + " reseteado correctamente");
                getPlanTransporte($("#fechaPlaneado").val());
            } else {
                toastr.error("Error al resetar el folio " + $('#confirmBtn').data('Folio'));
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud de reseteo de folio: " + error);
        }
    });
    $('#confirmActionModal').modal('hide');
}

function cancelarFolio() {

    jQuery.ajax({
        url: "/Transportes/cancelFolio",
        type: "POST",
        data: JSON.stringify({ Folio: $('#confirmBtn').data('Folio'), AutorizedBy: AutorizedBy }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.result == true) {
                $('#folio').val("");
                toastr.success("Folio  " + $('#confirmBtn').data('Folio') + " cancelado correctamente");
                getPlanTransporte($("#fechaPlaneado").val());
            } else {
                toastr.error("Error al cancelar el folio " + $('#confirmBtn').data('Folio'));
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud de cancelacion de folio: " + error);
        }
    });
    $('#confirmActionModal').modal('hide');
}


function getDetailFolio(folio, type) {

    jQuery.ajax({
        url: "/Transportes/getDetailFolio",
        type: "POST",
        data: JSON.stringify({ Folio: folio }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var detailFolio = data.data;
            console.log(detailFolio)
            if (detailFolio && detailFolio.length > 0) {
                detailFolio = data.data[0];

                const fechaHora = moment(detailFolio.HrLlegadaPlan, "DD/MM/YYYY h:mm:ss a");
                const HrLlegadaPlan = fechaHora.format("DD/MM/YYYY HH:mm:ss");

                const fechaHora2 = moment(detailFolio.HrSalidaPlan, "DD/MM/YYYY h:mm:ss a");
                const HrSalidaPlan = fechaHora2.format("DD/MM/YYYY HH:mm:ss");

                const fechaHora3 = moment(detailFolio.FechaCarga, "DD/MM/YYYY h:mm:ss a");
                const FechaCarga = fechaHora3.format("DD/MM/YYYY HH:mm:ss");

                if (type == 1) {
                    var FechaPlan = moment(detailFolio.FechaPlan, 'DD/MM/YYYY').toDate();
                    $('#FPlan').val(FechaPlan).removeAttr("readonly");;
                    $("#FPlan").datepicker({
                        dateFormat: 'yy-mm-dd'
                    }).datepicker('setDate', FechaPlan);



                    $('#HrllegadaPlan').val(HrLlegadaPlan).removeAttr("readonly");;

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


                    $('#HrllegadaPlan').datetimepicker({
                        dateFormat: 'dd/mm/yy',
                        timeFormat: 'HH:mm:ss',
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

                    $('#HrSalidaPlan').val(HrSalidaPlan).removeAttr("readonly");;
                    $('#HrSalidaPlan').datetimepicker({
                        dateFormat: 'dd/mm/yy',
                        timeFormat: 'HH:mm:ss',
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


                    $('#ASN').val(detailFolio.ASN).removeAttr("readonly");
                    $('#Proveedor').val(detailFolio.Proveedor).removeAttr("readonly");
                    $('#Proyecto').val(detailFolio.Proyecto).removeAttr("readonly");
                    $('#Proceso').val(detailFolio.Proceso).removeAttr("readonly");
                    $('#Unidad').val(detailFolio.Unidad).removeAttr("readonly");
                    $('#Carga').val(detailFolio.Carga).removeAttr("readonly");
                    $('#LineaTrans').val(detailFolio.LineaTrans).removeAttr("readonly");
                    $('#FechaCarga').val(FechaCarga).removeAttr("readonly");

                    $('#actualizarFolio').data('Folio', folio);


                } else {
                    $('#FPlan').val(detailFolio.FechaPlan).attr("readonly", true);
                    $('#HrllegadaPlan').val(HrLlegadaPlan).attr("readonly", true);
                    $('#HrSalidaPlan').val(HrSalidaPlan).attr("readonly", true);
                    $('#ASN').val(detailFolio.ASN).attr("readonly", true);
                    $('#Proveedor').val(detailFolio.Proveedor).attr("readonly", true);
                    $('#Proyecto').val(detailFolio.Proyecto).attr("readonly", true);
                    $('#Proceso').val(detailFolio.Proceso).attr("readonly", true);
                    $('#Unidad').val(detailFolio.Unidad).attr("readonly", true);
                    $('#Carga').val(detailFolio.Carga).attr("readonly", true);
                    $('#LineaTrans').val(detailFolio.LineaTrans).attr("readonly", true);
                    $('#FechaCarga').val(FechaCarga).attr("readonly", true);

                    $('#FPlan').timepicker('destroy').removeClass('dtapicker');
                    $('#HrllegadaPlan').timepicker('destroy').removeClass('dtapicker');
                    $('#HrSalidaPlan').timepicker('destroy').removeClass('dtapicker');
                }
            } else {
                limpiarFormulario();
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud de busqueda de folio: " + error);
        }
    });
}


function validar() {
    // Objeto para almacenar los datos del formulario
    var data = {};
    data.Folio = $('#actualizarFolio').data('Folio');
    data.AutorizedBy = AutorizedBy;

    var camposVacios = [];
    var camposInvalidos = [];

    function esValorValido(valor) {
        return valor.trim() !== '';
    }

    var valorASN = $('#ASN').val().trim();
    if (esValorValido(valorASN)) {
        data.ASN = valorASN;
    } else {
        camposVacios.push(' ASN');
    }

    var valorFPlan = $('#FPlan').val().trim();
    if (esValorValido(valorFPlan)) {
        data.FechaPlan = valorFPlan;
    } else {
        camposVacios.push(' Fecha Plan');
    }

    var valorHrllegadaPlan = $('#HrllegadaPlan').val().trim();
    if (esValorValido(valorHrllegadaPlan)) {
        data.HrLlegadaPlan = valorHrllegadaPlan;
    } else {
        camposVacios.push(' Hora Llegada Plan');
    }

    var valorHrSalidaPlan = $('#HrSalidaPlan').val().trim();
    if (esValorValido(valorHrSalidaPlan)) {
        data.HrSalidaPlan = valorHrSalidaPlan;
    } else {
        camposVacios.push(' Hora Salida Plan');
    }


    // Validación de fechas y horas
    if (data.FechaPlan && data.HrLlegadaPlan && data.HrSalidaPlan) {
        var fechaPlan = new Date(data.FechaPlan);

        // Crear DateTime para llegada y salida
        var timeLlegada = moment(data.HrLlegadaPlan, 'DD/MM/YYYY HH:mm:ss');
        var timeSalida = moment(data.HrSalidaPlan, 'DD/MM/YYYY HH:mm:ss');

        //console.log("timeLlegada  --> ", timeLlegada.format('DD/MM/YYYY HH:mm:ss'));
        //console.log("timeSalida   --> ", timeSalida.format('DD/MM/YYYY HH:mm:ss'));

        if (timeLlegada.isSameOrAfter(timeSalida)) {
            camposInvalidos.push(' La hora de llegada planeada no puede ser mayor o igual que la hora de salida planeada a menos que cruce la medianoche.');
        }

        function getDateWithoutTime(date) {
            return new Date(date.getFullYear(), date.getMonth(), date.getDate());
        }

        // Validar que la fecha de llegada no sea mayor que la fecha de salida
        var today = new Date();
        today = getDateWithoutTime(today);
        fechaPlan.setDate(fechaPlan.getDate() + 1);
        fechaPlan = getDateWithoutTime(fechaPlan);

        if (fechaPlan < today) {
            camposInvalidos.push('La Fecha Plan no puede ser anterior a la fecha actual.');
        }

    }

    var valorProveedor = $('#Proveedor').val().trim();
    if (esValorValido(valorProveedor)) {
        data.Proveedor = valorProveedor;
    } else {
        camposVacios.push(' Proveedor');
    }

    var valorProyecto = $('#Proyecto').val().trim();
    if (esValorValido(valorProyecto)) {
        data.Proyecto = valorProyecto;
    } else {
        camposVacios.push(' Proyecto');
    }

    var valorUnidad = $('#Unidad').val().trim();
    if (esValorValido(valorUnidad)) {
        data.Unidad = valorUnidad;
    } else {
        camposVacios.push(' Unidad');
    }

    var valorCarga = $('#Carga').val().trim();
    if (esValorValido(valorCarga)) {
        data.Carga = valorCarga;
    } else {
        camposVacios.push(' Carga');
    }

    var valorLineaTrans = $('#LineaTrans').val().trim();
    if (esValorValido(valorLineaTrans)) {
        data.LineaTrans = valorLineaTrans;
    } else {
        camposVacios.push(' Línea Transporte');
    }

    if (camposVacios.length > 0) {
        toastr.error('Los siguientes campos están vacíos: ' + camposVacios);
    } else if (camposInvalidos.length > 0) {
        toastr.error('Error en los siguientes campos: ' + camposInvalidos);

    } else {
        actualizarFolio(data)
    }

}
function actualizarFolio(data) {
    var dtFolio = data.Folio;
    jQuery.ajax({
        url: "/Transportes/updateFolio",
        type: "POST",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.result == true) {
                $('#titleModalInfoFolio').val("");
                toastr.success("Datos del folio  " + dtFolio + " actualizados correctamente");
                getPlanTransporte($("#fechaPlaneado").val());
            } else {
                toastr.error("Error al actualizar los datos  del folio " + dtFolio);
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Error al hacer la solicitud de actualización de folio: " + error);
        }
    });
    $('#getDetailFolio').modal('hide');
}


$('#statusModal').on('show.bs.modal', function () {
    modalIsOpen = true;
    stopInterval();
});

$('#statusModal').on('hide.bs.modal', function () {
    modalIsOpen = false;
    startInterval();
});

$('#statusModal2').on('show.bs.modal', function () {
    modalIsOpen = true;
    stopInterval();
});

$('#statusModal2').on('hide.bs.modal', function () {
    modalIsOpen = false;
    startInterval();
});
$('#statusModal3').on('show.bs.modal', function () {
    modalIsOpen = true;
    stopInterval();
});

$('#statusModal3').on('hide.bs.modal', function () {
    modalIsOpen = false;
    startInterval();
});


$('#confirmActionModal').on('show.bs.modal', function () {
    modalIsOpen = true;
    stopInterval();
});

$('#confirmActionModal').on('hide.bs.modal', function () {
    modalIsOpen = false;
    startInterval();
});


$('#getDetailFolio').on('show.bs.modal', function () {
    modalIsOpen = true;
    stopInterval();
});

$('#getDetailFolio').on('hide.bs.modal', function () {
    modalIsOpen = false;
    startInterval();
});


$(document).ready(function () {
    startInterval();
});