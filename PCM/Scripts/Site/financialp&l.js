$('.btnStep3').click(function () {
    var s3data = [$("#data-affectedl12cases").val().toString(),
    $("#data-affectedl12price").val().toString(),
    $("#data-unaffectedl12cases").val().toString(),
    $("#data-unaffectedl12price").val().toString(),
    $("#data-affectedn12cases").val().toString(),
    $("#data-affectedn12price").val().toString(),
    $("#data-unaffectedn12cases").val().toString(),
    $("#data-unaffectedn12price").val().toString()];
    var url = window.location.href.split('?')[0];
    var modelid = url.substring(url.lastIndexOf('/') + 1);
    $.ajax({
        type: "POST",
        url: '@Url.Action("Step3Data", "Home")',
        dataType: 'JSON',
        data: {
            arr: s3data, modelid: modelid
        },
        success: function (result) {
            if (result.url) {
                // if the server returned a JSON object containing an url
                // property we redirect the browser to that url
                window.location.href = result.url;
            }
        },
        error: function (request, status, error) {
            alert('Error occured');
            alert(request.responseText);
        }
    });
})
function Step3Submit() {
    var findata = {
        affectedl12cases: $("#val-affectedl12cases").val().toString(),
        affectedn12cases: $("#val-affectedn12cases").val().toString(),
        unaffectedl12cases: $("#val-unaffectedl12cases").val().toString(),
        unaffectedn12cases: $("#val-unaffectedn12cases").val().toString(),
        affectedl12price: $("#val-affectedl12price").val().toString(),
        affectedn12price: $("#val-affectedn12price").val().toString(),
        unaffectedl12price: $("#val-unaffectedl12price").val().toString(),
        unaffectedn12price: $("#val-unaffectedn12price").val().toString(),
        affectedl12customerprograms: $("#val-affectedl12customerprograms").val().toString(),
        affectedn12customerprograms: $("#val-affectedn12customerprograms").val().toString(),
        unaffectedl12customerprograms: $("#val-unaffectedl12customerprograms").val().toString(),
        unaffectedn12customerprograms: $("#val-unaffectedn12customerprograms").val().toString(),
        affectedl12offinvoice: $("#val-affectedl12offinvoice").val().toString(),
        affectedn12offinvoice: $("#val-affectedn12offinvoice").val().toString(),
        unaffectedl12offinvoice: $("#val-unaffectedl12offinvoice").val().toString(),
        unaffectedn12offinvoice: $("#val-unaffectedn12offinvoice").val().toString(),
        affectedl12credits: $("#val-affectedl12credits").val().toString(),
        affectedn12credits: $("#val-affectedn12credits").val().toString(),
        unaffectedl12credits: $("#val-unaffectedl12credits").val().toString(),
        unaffectedn12credits: $("#val-unaffectedn12credits").val().toString(),
        affectedl12marketingspend: $("#val-affectedl12marketingspend").val().toString(),
        affectedn12marketingspend: $("#val-affectedn12marketingspend").val().toString(),
        unaffectedl12marketingspend: $("#val-unaffectedl12marketingspend").val().toString(),
        unaffectedn12marketingspend: $("#val-unaffectedn12marketingspend").val().toString(),
        affectedl12varopex: $("#val-affectedl12varopex").val().toString(),
        affectedn12varopex: $("#val-affectedn12varopex").val().toString(),
        unaffectedl12varopex: $("#val-unaffectedl12varopex").val().toString(),
        unaffectedn12varopex: $("#val-unaffectedn12varopex").val().toString(),
        l12directopex: $("#val-L12directopex").val().toString(),
        n12directopex: $("#val-N12directopex").val().toString(),
    }
    var url = window.location.href.split('?')[0];
    var modelid = url.substring(url.lastIndexOf('/') + 1);
    $.ajax({
        type: "POST",
        url: '@Url.Action("Step3Data", "Home")',
        dataType: 'JSON',
        data: {
            arr: findata, modelid: modelid
        },
        success: function (result) {
            if (result.url) {
                // if the server returned a JSON object containing an url
                // property we redirect the browser to that url
                window.location.href = result.url;
            }
        },
        error: function (request, status, error) {
            alert('Error occured');
            alert(request.responseText);
        }
    });
}