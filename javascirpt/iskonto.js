var iIskontoYuzdeOrani = 0;

$(function () {
    if ($("#iKodMusteri4") != null) {
        $("#iKodMusteri4").change(function () {
            if ($("#iKodMusteri4").val() != null && $("#iKodMusteri4").val() > 0) {
                IskontoHesapla($("#iKodMusteri4").val());
            } else {
                iIskontoYuzdeOrani = 0;
            }
        });
    }
});

function IskontoHesapla(iKodMusteri) {
    if (iKodMusteri > 0) {
        $("#pageLoading").displayBlock();

        $.ajax({
            type: "GET",
            url: "/api/iskontoapi/" + iKodMusteri + "/?" + new Date(),
            contentType: "json",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    iIskontoYuzdeOrani = data;
                    $("#iIskontoYuzdeOrani").val(data);
                    $("#pageLoading").displayNone();
                }
                else {
                    $("#pageLoading").displayNone();
                }
            },
            error: function (xhr) {
                alert(xhr.responseText);
                $.ajax({
                    type: "PUT",
                    url: "/api/logapi/",
                    data: JSON.stringify({
                        "iType": 1,
                        "cControllerName": "Java Script Ajax Hatası",
                        "cActionName": "iskonto.js",
                        "cHata": xhr.responseText,
                        "cIslem": "",
                        "iKodKullanici": 0,
                        "iKodKayit": 0
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    processData: true
                });
                $("#pageLoading").displayNone();
            }
        });
    }
}