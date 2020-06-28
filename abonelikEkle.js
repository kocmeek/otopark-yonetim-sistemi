$(function () {
    if ($("#iKodAbonelikTipi").val() > 0 && iKodLokasyonLogin > 0) {
        AbonelikTipiFiyat();
    }

    $("#iKodAbonelikTipi").change(function () {
        if ($("#iKodAbonelikTipi").val() > 0 && iKodLokasyonLogin > 0) {
            AbonelikTipiFiyat();
        }
    });

    $("#iDuzeltmeTipi").change(function () {
        Hesapla();
    });

    $("#cDuzeltme").change(function () {
        Hesapla();
    });

    $("#cDuzeltme").keyup(function () {
        Hesapla();
    });
});

var fTutar = 0;
function AbonelikTipiFiyat() {
    $("#pageLoading").displayBlock();
    $.ajax({
        type: "GET",
        url: "/api/aboneliktipiapi/" + iKodLokasyonLogin + "/" + $("#iKodAbonelikTipi").val() + "?" + new Date(),
        contentType: "json",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                fTutar = data.replace(",", ".");
                Hesapla();
            }
            $("#pageLoading").displayNone();
        },
        error: function (xhr) {
            alert(xhr.responseText);
            $.ajax({
                type: "PUT",
                url: "/api/logapi/",
                data: JSON.stringify({
                    "iType": 1,
                    "cControllerName": "Java Script Ajax Hatası",
                    "cActionName": "abonelikEkle.js",
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

function Hesapla() {
    var fGenelTutar = OndalikCevir(fTutar);
    var fDuzeltme = 0;
    if ($("#iDuzeltmeTipi") != null && $("#iDuzeltmeTipi").val() != null && $("#iDuzeltmeTipi").val() != 0 && $("#cDuzeltme").val() != null && $("#cDuzeltme").val() != "0") {
        fDuzeltme = OndalikCevir($("#cDuzeltme").val().replace(",", ""));
        if ($("#iDuzeltmeTipi").val() == 1) {
            fGenelTutar = fGenelTutar - fDuzeltme;
        } else if ($("#iDuzeltmeTipi").val() == 2) {
            fGenelTutar = fGenelTutar + fDuzeltme;

        }
    } else if (($("#iDuzeltmeTipi") == null || $("#iDuzeltmeTipi").val() == null) && $("#cDuzeltme").val() != null) {
        fDuzeltme = OndalikCevir($("#cDuzeltme").val().replace(",", ""));
        fGenelTutar = fGenelTutar - fDuzeltme;
    }

    $('#cDuzeltme').priceFormat(optionPriceFormat);
    $('#cGenelTutar').val(fGenelTutar.toFixed(2));
    $('#cOdemeTutar').val(fGenelTutar.toFixed(2));
    $('#cOdemeTutar').priceFormat(optionPriceFormat);
    $('#cGenelTutar').priceFormat(optionPriceFormat);
}

function OndalikCevir(deger) {
    var dec = 2;
    var result = Math.round(deger * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
}