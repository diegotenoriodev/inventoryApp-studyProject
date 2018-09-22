$(function () {
    $('#addNewItem').click(function () {
        $('#Name').focus();
    });

    $('#Name').autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.ajax({
                url: "/Sale/GetInventoryProducts",
                type: "POST",
                dataType: "json",
                data: { txt: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        $('#IdProduct').val(0);
                        $('#IdProduct').trigger('change');
                        return { label: item.Name, value: item.Name, id: item.Id };
                    }))

                }
            })
        },
        select: function (event, ui) {
            $('#IdProduct').val(ui.item.id);
            $('#IdProduct').trigger('change');
            $('#Name').val(ui.item.label);
            $('#Name').trigger('change');
         
            $.ajax({
                url: "/Sale/GetInventoryProductDetails",
                type: "POST",
                dataType: "json",
                data: { id: ui.item.id },
                success: function (data) {
                    if (data != null) {
                        $('#Price').val(data.Price);
                        $('#Price').trigger('change');
                        $('#QtdAvailable').val(data.QtdAvailable);
                        $('#QtdAvailable').trigger('change');
                    } else {
                        $('#Price').val(null);
                        $('#Price').trigger('change');
                        $('#QtdAvailable').val(null);
                        $('#QtdAvailable').trigger('change');
                    }
                },
                error: function () {
                    $('#Price').val(null);
                    $('#Price').trigger('change');
                }
            })
        }
    }).click(function () {
        $('#Name').val('');
        $(this).data("uiAutocomplete").search($(this).val());
    });
});