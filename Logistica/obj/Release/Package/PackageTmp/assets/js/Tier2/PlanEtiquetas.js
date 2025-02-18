
const state = {
    dataTableInitialized: false,
    fileName: "",
    tableData: [],
    fechasDias: {
        fechaLunes: '',
        fechaMartes: '',
        fechaMiercoles: '',
        fechaJueves: '',
        fechaViernes: '',
        fechaSabado: '',
        fechaDomingo: ''
    }
};

document.addEventListener('DOMContentLoaded', function () {
    initializeEventListeners();
});

function initializeEventListeners() {
    const procesoSelect = document.getElementById("proceso");
    procesoSelect.addEventListener('change', handleProcesoChange);

    $('#xlsxPreviewTable th:nth-child(10)').css('background-color', '#000000');

}

function handleProcesoChange() {
    limpiarTodo();
    const procesoSelect = document.getElementById("proceso");
    if (procesoSelect.value === 'PRENSAS') {
        document.getElementById('domingoCol').style.display = 'none';
    } else {
        document.getElementById('domingoCol').style.display = '';
    }
}

function setValue() {
    const process = document.getElementById("proceso");
    const value = process.options[process.selectedIndex].text;
    document.getElementById("spanProceso").textContent = value;
}

function formatValue(value) {
    if (typeof value === 'object' && value !== null) {
        value = value.v;
    }
    if (typeof value === 'number' && !Number.isInteger(value)) {
        const entero = Math.floor(value);
        return { cantidad: entero, esParcial: value !== entero };
    }
    return {
        cantidad: (value || 0),
        esParcial: false
    };
}

function formatValuePrensas(value) {
    if (typeof value === 'number' && !Number.isInteger(value)) {
        const entero = Math.ceil(value);
        return { cantidad: entero, esParcial: false };
    }
    return { cantidad: value || 0, esParcial: false };
}

function convertExcelDate(cell, format = 'short', dateFormat = 'ymd') {
    try {
        if (!cell || cell.v === undefined) return '';

        const excelDate = cell.v;
        if (typeof excelDate !== 'number') return '';

        const jsDate = new Date((excelDate - 25568 - 1) * 86400 * 1000);
        if (isNaN(jsDate.getTime())) return '';

        const day = jsDate.getDate().toString().padStart(2, '0');
        const month = (jsDate.getMonth() + 1).toString().padStart(2, '0');
        const year = jsDate.getFullYear();

        return `${year}/${month}/${day}`;
    } catch (error) {
        console.error('Error en convertExcelDate:', error);
        return '';
    }
}

function limpiarArchivo() {
    document.getElementById('excelFile').value = '';
    document.getElementById('fileName').innerText = '';
}

function limpiarTabla() {
    if (state.dataTableInitialized) {
        $('#xlsxPreviewTable').DataTable().clear().destroy();
        state.dataTableInitialized = false;
    }
    $('#xlsxPreviewTable tbody').empty();
    $('#errorContainer').hide();
    $('#btnInsertar').hide();
}

function limpiarDatos() {
    state.tableData = [];
    Object.keys(state.fechasDias).forEach(key => {
        state.fechasDias[key] = '';
    });
}

function limpiarTodo() {
    limpiarArchivo();
    limpiarTabla();
    limpiarDatos();
}

function uploadExcel() {
    const fileInput = document.getElementById('excelFile');
    fileInput.value = '';
    fileInput.click();

    fileInput.onchange = async function (event) {
        const file = event.target.files[0];
        if (!file) return;

        try {
            document.getElementById("spinner").style.display = "block";
            const nameWithoutExtension = file.name.split('.')[0];
            document.getElementById('fileName').innerText = nameWithoutExtension;
            state.fileName = nameWithoutExtension;

            const arrayBuffer = await file.arrayBuffer();
            const data = new Uint8Array(arrayBuffer);
            const workbook = XLSX.read(data, { type: 'array' });

            const proceso = document.getElementById("proceso").value;
            if (proceso === "PRENSAS") {
                processPrensasExcelData(workbook);
            } else {
                processEnsambleExcelData(workbook);
            }
        } catch (error) {
            alert('Hubo un error al procesar el archivo');
        } finally {
            document.getElementById("spinner").style.display = "none";
        }
    };
}
////////////////////////////
////      ENSAMBLE     /////
////////////////////////////
function processEnsambleExcelData(workbook) {
    console.log(workbook);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    console.log(worksheet);
    state.tableData = [];

    Object.keys(state.fechasDias).forEach((key, index) => {
        const column = String.fromCharCode(72 + index);
        state.fechasDias[key] = convertExcelDate(worksheet[`${column}2`]);
    });
    console.log(state.fechasDias);
    let rowIndex = 4;
    while (true) {
        const cell = worksheet[`B${rowIndex}`];
        const numeroParte = cell ? cell.v : undefined;
        if (!numeroParte) break;

        const row = processEnsambleRow(worksheet, rowIndex);
        console.log("Procesando fila: ", row);
        if (row) state.tableData.push(row);
        rowIndex++;
    }

    updateTable();
}
////////////////////////////
////      PRENSAS      /////
////////////////////////////
function processPrensasExcelData(workbook) {
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    state.tableData = [];

    const firstDateCell = worksheet['H2'];
    console.log('Contenido de celda H2:', firstDateCell);
    let fechaPlan = null;

    if (firstDateCell && firstDateCell.t === 'n') {
        fechaPlan = excelDateToFormattedDate(firstDateCell.v);
        console.log('Primera fecha procesada:', fechaPlan);
    } else if (firstDateCell && firstDateCell.t === 's') {
        fechaPlan = firstDateCell.v;
        console.log('Primera fecha procesada desde texto:', fechaPlan);
    } else {
        console.log('No se pudo obtener la fecha del lunes');
    }

    if (fechaPlan) {
        const [day, month, year] = fechaPlan.split('/');
        const baseDate = new Date(year, parseInt(month) - 1, parseInt(day));

        state.fechasDias.fechaLunes = formatDate(baseDate, 1);
        state.fechasDias.fechaMartes = formatDate(baseDate, 2);
        state.fechasDias.fechaMiercoles = formatDate(baseDate, 3);
        state.fechasDias.fechaJueves = formatDate(baseDate, 4);
        state.fechasDias.fechaViernes = formatDate(baseDate, 5);
        state.fechasDias.fechaSabado = formatDate(baseDate, 6);

        console.log('Fechas de la semana:', state.fechasDias);

    } else {
        console.error('No se pudo calcular las fechas de la semana');
    }

    let rowIndex = 4;
    while (true) {
        const numeroParte = worksheet[`B${rowIndex}`]?.v;
        if (!numeroParte) break;
        const row = processPrensasRow(worksheet, rowIndex, fechaPlan);
        if (row) state.tableData.push(row);
        rowIndex++;
    }

    updateTablePrensas();
}

function excelDateToFormattedDate(excelDate) {

    const utc_days = Math.floor(excelDate - 25569);
    const utc_value = utc_days * 86400;
    const date_info = new Date(utc_value * 1000);

    const year = date_info.getFullYear();
    const month = String(date_info.getMonth() + 1).padStart(2, '0');
    const day = String(date_info.getUTCDate()).padStart(2, '0');

    return `${day}/${month}/${year}`;
}

function processPrensasRow(worksheet, rowIndex, fechaPlan) {
    const data = {
        Linea: worksheet[`A${rowIndex}`]?.v,
        NoParte: worksheet[`B${rowIndex}`]?.v,
        TipoEtiqueta: worksheet[`F${rowIndex}`]?.v,
        FechaPlan: fechaPlan
    };

    const dias = ['H', 'I', 'J', 'K', 'L', 'M'];
    const nombresDias = ['Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado'];
    let totalEtiquetas = 0;

    dias.forEach((col, index) => {
        const valorDia = formatValuePrensas(worksheet[`${col}${rowIndex}`]?.v).cantidad;
        data[nombresDias[index]] = valorDia;
        totalEtiquetas += valorDia;
    });

    const totalColumnaN = formatValuePrensas(worksheet[`N${rowIndex}`]?.v).cantidad;
    data.Total = totalColumnaN || totalEtiquetas;

    return data.Total > 0 ? data : null;

}

//////////////////////////////////////////////////////////////

function formatDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}/${month}/${day}`; // Formato yyyy/MM/dd
}


///////////////////////
function processEnsambleRow(worksheet, rowIndex) {
    const dias = ['G', 'H', 'I', 'J', 'K', 'L', 'M'];
    const data = {
        Linea: worksheet[`A${rowIndex}`]?.v,
        NoParte: worksheet[`B${rowIndex}`]?.v
    };

    let hasData = false;
    dias.forEach((col, index) => {
        const dayName = ['Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado', 'Domingo'][index];
        const cell = worksheet[`${col}${rowIndex}`];
        const cellValue = cell ? cell.v : undefined; //formatValue(worksheet[`${col}${rowIndex}`]?.v);

        const value = formatValue(cellValue);
        data[dayName] = value.cantidad;
        data[`${dayName}Parcial`] = value.esParcial ? 1 : 0;

        if (value.cantidad != 0 || value.esParcial) hasData = true;
    });
    const totalCell = worksheet[`N${rowIndex}`];
    const totalValue = totalCell ? totalCell.v : 0;
    const formattedTotal = formatValue(totalValue);
    data.Total = formattedTotal.cantidad + (formattedTotal.esParcial ? 1 : 0);

    return hasData ? data : null;
}

function updateTable() {
    console.log("updateTable: " + state.tableData);
    const tableBody = document.querySelector('#xlsxPreviewTable tbody');
    tableBody.innerHTML = '';

    state.tableData.forEach(dataRow => {
        const tr = document.createElement('tr');
        tr.appendChild(createCell(dataRow.Linea));
        tr.appendChild(createCell(dataRow.NoParte));

        ['Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado', 'Domingo'].forEach(dia => {
            const td = createCell(
                dataRow[`${dia}Parcial`] ?
                    `${dataRow[dia]} + 1 parcial` :
                    dataRow[dia] || ''
            );
            tr.appendChild(td);
        });

        const total = dataRow.Total || 0;
        tr.appendChild(createCell(total));
        tableBody.appendChild(tr);
    });

    document.getElementById('btnInsertar').style.display = 'block';
}

function updateTablePrensas() {
    const tableBody = document.querySelector('#xlsxPreviewTable tbody');
    tableBody.innerHTML = '';

    state.tableData.forEach(dataRow => {
        const tr = document.createElement('tr');
        tr.appendChild(createCell(dataRow.Linea));
        tr.appendChild(createCell(dataRow.NoParte));

        ['Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado'].forEach(dia => {
            const valorDia = dataRow[dia];
            let cellValue = '';

            if (valorDia != null && valorDia != '0') {
                const cantidadRedondeada = Math.ceil(valorDia);
                cellValue = cantidadRedondeada.toString();
            } else {
                cellValue = '';
            }

            const td = createCell(cellValue);
            tr.appendChild(td);

        });

        tr.appendChild(createCell(dataRow.Total || ''));
        tableBody.appendChild(tr);
    });

    document.getElementById('btnInsertar').style.display = 'block';
}

function createCell(text) {
    const td = document.createElement('td');
    td.textContent = text;
    return td;
}

function uploadExcelData() {
    const proceso = document.getElementById("proceso").value;

    if (proceso == 'PRENSAS') {
        console.log('Estado antes de enviar:', {
            tableData: state.tableData,
            fechasDias: state.fechasDias,
            fileName: state.fileName
        });

        if (!state.fechasDias) {
            console.error('fechasDias está vacío o no definido');
            return;
        }
        const datosParaBackend = state.tableData.map(plan => ({
            Linea: plan.Linea,
            NoParte: plan.NoParte,
            CantidadEtiquetas: plan.Total,
            EsParcial: plan.TipoEtiqueta === "PARCIAL",
            UsuarioCreacion: nombreUsuario,
            Proceso: proceso,
            FechaPlan: state.fechasDias.fechaLunes
        }));

        console.log('Datos transformados para el backend:', datosParaBackend);
        console.log('Datos de fechas: ', state.fechasDias.fechaLunes)

        $.ajax({
            url: '/Tier2/datosPlanSemanalPrensas',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                datosXLSX: datosParaBackend,
                nombreUsuario: nombreUsuario,
                fechasDias: state.fechasDias,
                nombrePlan: state.fileName,
                proceso: proceso
            }),
            success: function (response) {
                if (response.success == true) {
                    alert('Datos guardados exitosamente');
                } else {
                    alert('Los datos no se pudieron guardar');
                    console.log("Error: " + response.Message)
                }
                limpiarTodo();
            },
            error: function (xhr, status, error) {
                alert('Error al guardar los datos: ' + error);
                console.error(xhr.responseText);
            }
        });
    } else if (proceso == 'ENSAMBLE') {
        console.log("Iniciando AJAX de 'ENSAMBLE'");
        $.ajax({
            url: '/Tier2/datosPlanSemanal',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                datosXLSX: state.tableData,
                nombreUsuario: nombreUsuario,
                fechasDias: state.fechasDias,
                nombrePlan: state.fileName,
                proceso: proceso
            }),
            success: function (response) {
                if (response.success == true) {
                    alert('Datos guardados exitosamente');
                } else {
                    alert('Los datos no se pudieron guardar');
                    console.log("Error: " + response.Message)
                }
                limpiarTodo();
            },
            error: function (xhr, status, error) {
                alert('Error al guardar los datos: ' + error + ", " + xhr + ", " + status);
                console.error(xhr.responseText);
            }
        });
    }
}

function processExcelDate(cell) {
    if (!cell) return null;

    // Si es una cadena de texto (como "PRODUCCIÓN")
    if (cell.t === 's') {
        return null;
    }

    // Si es una fecha numérica de Excel
    if (cell.t === 'n') {
        return convertExcelDateToISO(cell.v);
    }

    // Si es una fecha en formato texto (dd/mm/yyyy)
    if (cell.t === 's' && cell.v.includes('/')) {
        const [day, month, year] = cell.v.split('/');
        const date = new Date(year, month - 1, day);
        return date.toISOString().split('T')[0];
    }

    return null;
}

function convertExcelDateToISO(excelDate) {
    const millisecondsPerDay = 24 * 60 * 60 * 1000;
    const excelEpoch = new Date(Date.UTC(1900, 0, -1));
    const javascriptDate = new Date(excelEpoch.getTime() + (excelDate - 1) * millisecondsPerDay);
    return javascriptDate.toISOString().split('T')[0];
}
