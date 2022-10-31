
function inicializaFormularioTransacciones(urlObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const valueSelect = $(this).val();

        const resp = await fetch(urlCategorias, {
            method: 'POST',
            body: valueSelect,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await resp.json();
        const opciones = result.map(categoria =>
            `<option value="${categoria.value}">${categoria.text}</option>`
        );

        $("#CategoriaID").html(opciones);
    });
}