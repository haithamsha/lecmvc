$(function () {

    $("#btnSave").click(function () {
        var product = { Name: 'prod3', Price: 40, Quantity: 20, CategoryId: 1 };

        $("#spLoading").html('loading....');
        $.post('/Products/InsertProduct', { product: product })
        .done(function (data) {
            console.log(data);
            $("#spLoading").html('');
        })
        .error(function (err) {
            console.log(err);
        })
    })

});