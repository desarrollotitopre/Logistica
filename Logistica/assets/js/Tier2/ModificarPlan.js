document.getElementById('proceso').addEventListener('change', function () {
    const selectElement = document.getElementById('proceso');
    const dateInput = document.getElementById('startDate');
    const alertButton = document.getElementById('btnCargarDatos');

    if (selectElement.value !== "Selecciona") {
        dateInput.disabled = false;
        alertButton.disabled = !dateInput.value;

    } else {
        dateInput.disabled = true;
        dateInput.value = "";
        alertButton.disabled = true;
        var tituloDiv = document.querySelector("#titleDiv");
        tituloDiv.className = '';
        tituloDiv.style.display = 'none;';
        tituloDiv.innerHTML = '';
        const tbody = document.querySelector('#xlsxPreviewTable tbody');
        tbody.innerHTML = '';
    }
});

document.getElementById('startDate').addEventListener('change', function () {
    const selectElement = document.getElementById('proceso');
    const dateInput = document.getElementById('startDate');
    const alertButton = document.getElementById('btnCargarDatos');
    alertButton.disabled = !(selectElement.value !== "Selecciona" && dateInput.value);
});

document.getElementById('btnCargarDatos').addEventListener('click', mostrarAlertas);

document.getElementById('saveAllChanges').addEventListener('click', saveAllChanges);
function mostrarAlertas() {
    const selectElement = document.getElementById('proceso');
    const dateInput = document.getElementById('startDate');
    const area = selectElement.value;
    const date = dateInput.value;
    const tbody = document.querySelector('#xlsxPreviewTable tbody');
    tbody.innerHTML = '';
    if (area !== "Selecciona" && date) {

        document.getElementById('spinner').style.display = 'block';

        $.ajax({
            url: '/Tier2/consultarPlan',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                proceso: area,
                fecha: date
            }),
            success: function (response) {
                if (response.success && response.data) {
                    response.data.forEach(item => {
                        if (item.FechaPlan) {
                            const timestamp = parseInt(item.FechaPlan.match(/\d+/)[0]);
                            item.FechaPlan = new Date(timestamp).toISOString().split('T')[0];
                        }
                    });

                    var titleDiv = document.getElementById('titleDiv');
                    titleDiv.style.display = 'block';
                    titleDiv.innerHTML = `<strong>Plan actual:</strong> ${response.data[0].TituloPlan}`;
                    //document.querySelector('.table-responsive').before(titleDiv);

                    const tbody = document.querySelector('#xlsxPreviewTable tbody');
                    tbody.innerHTML = '';

                    response.data.forEach((item, index) => {
                        const tr = document.createElement('tr');
                        tr.innerHTML = `
                            <td style="font-size: 16px; font-weight: bold;">${item.Linea}</td>
                            <td style="font-size: 16px; font-weight: bold;">${item.NoParte}</td>
                            <td style="font-size: 16px; font-weight: bold;">${item.CantEtqSoli}</td>
                            <td style="font-size: 16px; font-weight: bold;">
                                <span class="cantidad-display">${item.CantidadEtiquetas}</span>
                                <input type="number" class="form-control cantidad-edit" style="display:none" value="${item.CantidadEtiquetas}">
                            </td>
                            <td style="font-size: 16px; font-weight: bold;">
                                <span class="parcial-display">${item.EtiquetaParcial}</span>
                                <input type="number" class="form-control parcial-edit" style="display:none" value="${item.EtiquetaParcial}">
                            </td>
                            <td style="font-size: 16px; font-weight: bold;">
                                <button class="btn btn-edit" data-row="${index}">
                                    <i class="fas fa-pen-square" style="color: #0d6efd"></i>
                                </button>
                                <button class="btn btn-save" data-row="${index}" style="display:none">
                                    <i class="fas fa-save" style="color: #198754"></i>
                                </button>
                                <button class="btn btn-cancel" data-row="${index}" style="display:none">
                                    <i class="fas fa-times" style="color: #dc3545"></i>
                                </button>
                            </td>
                        `;
                        tbody.appendChild(tr);
                    });

                    setupEditButtons();
                }
            },
            error: function (xhr, status, error) {
                console.error("Error en la petición:", error);
                document.getElementById('errorContainer').style.display = 'block';
                document.getElementById('errorContainer').textContent = 'Error al cargar los datos: ' + error;
            },
            complete: function () {
                document.getElementById('spinner').style.display = 'none';
            }
        });
    } else {
        alert("Por favor selecciona un área y una fecha.");
    }
}

function setupEditButtons() {
    let hasChanges = false;

    document.querySelectorAll('.btn-edit').forEach(button => {
        button.addEventListener('click', function () {
            const row = this.closest('tr');

            // Guardar valores actuales para cancelar
            row.dataset.oldCantidad = row.querySelector('.cantidad-display').textContent;
            row.dataset.oldParcial = row.querySelector('.parcial-display').textContent;

            // Mostrar campos de edición
            row.querySelector('.cantidad-display').style.display = 'none';
            row.querySelector('.cantidad-edit').style.display = 'block';
            row.querySelector('.parcial-display').style.display = 'none';
            row.querySelector('.parcial-edit').style.display = 'block';

            // Cambiar botones
            this.style.display = 'none';
            row.querySelector('.btn-save').style.display = 'inline-block';
            row.querySelector('.btn-cancel').style.display = 'inline-block';
        });
    });

    document.querySelectorAll('.btn-save').forEach(button => {
        button.addEventListener('click', function () {
            const row = this.closest('tr');
            const newCantidad = row.querySelector('.cantidad-edit').value;
            const newParcial = row.querySelector('.parcial-edit').value;

            if (newCantidad < 0 || newParcial < 0) {
                alert('Los valores no pueden ser negativos');
                return;
            }

            row.dataset.modified = 'true';
            row.dataset.newCantidad = newCantidad;
            row.dataset.newParcial = newParcial;

            row.querySelector('.cantidad-display').textContent = newCantidad;
            row.querySelector('.parcial-display').textContent = newParcial;
            toggleEditMode(row, false);

            hasChanges = true;
            document.getElementById('saveAllChanges').style.display = 'block';
        });
    });

    document.querySelectorAll('.btn-cancel').forEach(button => {
        button.addEventListener('click', function () {
            const row = this.closest('tr');

            // Restaurar valores originales
            row.querySelector('.cantidad-edit').value = row.dataset.oldCantidad;
            row.querySelector('.parcial-edit').value = row.dataset.oldParcial;

            // Restaurar vista normal
            toggleEditMode(row, false);
        });
    });
}

function saveAllChanges() {
    const changedRows = document.querySelectorAll('tr[data-modified="true"]');
    const changes = Array.from(changedRows).map(row => ({
        noParte: row.querySelector('td:nth-child(2)').textContent,
        cantidadEtiquetas: row.dataset.newCantidad,
        etiquetaParcial: row.dataset.newParcial
    }));

    const proceso = document.getElementById('proceso').value;
    const fecha = document.getElementById('startDate').value;

    document.getElementById('spinner').style.display = 'block';
    $.ajax({
        url: '/Tier2/actualizarPlan',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            proceso: proceso,
            fecha: fecha,
            registros: changes
        }),
        success: (response) => {
            if (response.success) {
                alert('Todos los cambios guardados correctamente', 'success');
                changedRows.forEach(row => {
                    delete row.dataset.modified;
                    delete row.dataset.newCantidad;
                    delete row.dataset.newParcial;
                });
                document.getElementById('saveAllChanges').style.display = 'none';
            } else {
                alert('Error al guardar los cambios: ' + response.message, 'error');
            }
        },
        error: (xhr, status, error) => {
            alert('Error al guardar los cambios. Error: ' + error);
            alert('Error al guardar los cambios. Status: ' + status);
            alert('Error al guardar los cambios. XHR: ' + xhr);
        },
        complete: () => {
            document.getElementById('spinner').style.display = 'none';
        }
    });
}

function toggleEditMode(row, editing) {
    row.querySelector('.cantidad-display').style.display = editing ? 'none' : 'block';
    row.querySelector('.cantidad-edit').style.display = editing ? 'block' : 'none';
    row.querySelector('.parcial-display').style.display = editing ? 'none' : 'block';
    row.querySelector('.parcial-edit').style.display = editing ? 'block' : 'none';
    row.querySelector('.btn-edit').style.display = editing ? 'none' : 'inline-block';
    row.querySelector('.btn-save').style.display = editing ? 'inline-block' : 'none';
    row.querySelector('.btn-cancel').style.display = editing ? 'inline-block' : 'none';
}