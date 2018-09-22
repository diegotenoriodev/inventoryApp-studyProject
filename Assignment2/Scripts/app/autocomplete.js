var autoCompleteLabel = '';

$(function () {
    $('#Name').autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.ajax({
                url: "/Inventory/GetProducts",
                type: "POST",
                dataType: "json",
                data: { txt: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        $('#IdProduct').val(0);
                        return { label: item.Name, value: item.Name, id: item.Id };
                    }))

                }
            })
        },
        select: function (event, ui) {
            $('#IdProduct').val(ui.item.id);
            autoCompleteLabel = ui.item.label;

            $.ajax({
                url: "/Inventory/GetProductDetails",
                type: "POST",
                dataType: "json",
                data: { id: ui.item.id },
                success: function (data) {
                    if (data != null) {
                        $('#DatePurchase').val(data.DatePurchaseStr);
                        $('#PriceAcquisition').val(data.PriceAcquisition);
                        $('#Price').val(data.Price);
                        $('#Qtd').val(data.Qtd);
                    } else {
                        $('#DatePurchase').val(null);
                        $('#Price').val(null);
                        $('#PriceAcquisition').val(null);
                        $('#Qtd').val(0);
                    }
                },
                error: function () {
                    $('#DatePurchase').val(null);
                    $('#Price').val(null);
                    $('#Qtd').val(0);
                }
            })
        }
    });

    $('#Name').focus(function () {
        $(this).data("uiAutocomplete").search($(this).val());
    });

    $('#Name').focusout(function () {
        // In case the user changes the name after selected the auto complete
        if ($(this).val() != autoCompleteLabel) {
            $('#IdProduct').val(0);
        }
    });
});