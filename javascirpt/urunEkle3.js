var iUrunId = 0;

$(function () {

    $("#btnSave").click(function () {
        $("#form").submit();
    });

    $("#btnAdd").click(function () {
        ProductAdd();
    });

    $("#btnAllAdd").click(function () {
        ProductAllCode();
    });

    $('#cokluUrunEkleModal').on('shown.bs.modal', function () {
        $('#cBarkodlar').focus();
    });

    $(".cboUrunAdi").change(function () {
        Product(this);
    });
});

function ProductAdd() {
    $("#pageLoading").displayBlock();
    $("#cUrun3ListesiValidation").text("");

    var lSecimYap = true;
    for (var i = 0; i < iUrunId + 1; i++) {
        if ($("#iKodUrun-" + i).val() == 0 || $("#iAdet-" + i).val() == 0 || $("#cBirimFiyati-" + i).val() == '' || $("#cFiyat-" + i).val() == '' || $("#cBirimFiyati-" + i).val() == "0.00" || $("#cFiyat-" + i).val() == "0.00") {
            lSecimYap = false;
        }
    }

    if (lSecimYap == false) {
        $("#cUrun3ListesiValidation").text("Lütfen bu alanı doldurun!");
        $("#pageLoading").displayNone();
    } else {
        ProductAdd2(0);
    }
}

function ProductAdd2(iKodUrun) {
    console.log("girdi");
    $.ajax({
        type: "GET",
        url: "/api/urun3api/?" + new Date(),
        contentType: "json",
        dataType: "json",
        success: function (data) {
            
            if (data != null) {
                for (var i = 0; i < iUrunId + 1; i++) {
                    if ($("#iKodUrun-" + i).val() == 0 || $("#iAdet-" + i).val() == 0 || $("#cBirimFiyati-" + i).val() == '' || $("#cFiyat-" + i).val() == '' || $("#cBirimFiyati-" + i).val() == "0.00" || $("#cFiyat-" + i).val() == "0.00") {
                        $("#iKodUrun-" + i).parent().parent().remove();
                    }
                }

                iUrunId = iUrunId + 1;

                if (data.length > 0) {
                    var cHTML = '';

                    $.each(data, function (i, item) {
                        if (item && item.iKodUrun3 && item.cAdi) {
                            cHTML += "<option value=\"" + item.iKodUrun3 + "\">" + item.cAdi + "</option>";
                        }
                    });

                    if (cHTML != '') {
                        cHTML =
                            "<div class=\"row\">" +
                            "<div class=\"form-group col-md-2\">" +
                            "<label for=\"cKodu\">Ürün Kodu</label>" +
                            "<input onkeypress=\"ProductCode(this)\" autocomplete=\"off\" class=\"form-control txtUrunKodu\" id=\"cKodu-" + iUrunId + "\" maxlength=\"100\" name=\"cKodu\" placeholder=\"Lütfen ürün kodu girin ...\" tabindex=\"3\">" +
                            "</div>" +
                            "<div class= \"form-group col-md-3\" >" +
                            "<label for=\"iKodUrun\">Ürün</label>" +
                            "<select onchange=\"Product(this)\" class=\"form-control cboUrunAdi\" id=\"iKodUrun-" + iUrunId + "\" tabindex=\"2\">" +
                            "<option value =\"0\">Lütfen ürün seçin ...</option>" +
                            cHTML +
                            "</select>" +
                            "</div>" +
                            "<div class=\"form-group col-md-1\">" +
                            "<label for=\"iAdet\">Adet</label>" +
                            "<input onchange=\"ProductWrite()\" autocomplete=\"off\" class=\"form-control txtUrunAdet\" id=\"iAdet-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen adet girin ...\" type=\"number\" value=\"1\" tabindex=\"3\">" +
                            "</div>" +
                            "<div class=\"form-group col-md-2\">" +
                            "<label for=\"iAdet\">Birim Fiyatı</label>" +
                            "<input onchange=\"ProductWrite()\" autocomplete=\"off\" class=\"form-control txtUrunBirimFiyati\" id=\"cBirimFiyati-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen birim fiyatı girin ...\" value=\"0.00\" tabindex=\"4\">" +
                            "</div>" +
                            "<div class=\"form-group col-md-2\">" +
                            "<label for=\"iAdet\">Fiyat</label>" +
                        "<input onchange=\"ProductWrite()\" autocomplete=\"off\" class=\"form-control txtUrunFiyat\" id=\"cFiyat-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen fiyat girin ...\" value=\"0.00\" tabindex=\"5\" readonly=\"readonly\">" +
                            "</div>" +
                            "<div class=\"col-md-1\">" +
                            "<button onclick=\"ProductPrint(this)\" type=\"button\" class=\"btn btn-secondary btn-block btn-delete\" tabindex=\"6\">Yazdır</button>" +
                            "</div>" +
                            "<div class=\"col-md-1\">" +
                            "<button onclick=\"ProductDelete(this)\" type=\"button\" class=\"btn btn-secondary btn-block btn-delete\" tabindex=\"7\">Sil</button>" +
                            "</div>" +
                            "</div>";

                        

                        $("#urun3Listesi").append(cHTML);

                        $("#iKodTedarikci").select2();
                        $("#iKodUrun-0").select2();

                        if (iKodUrun > 0) {
                            $("#iKodUrun-" + iUrunId).val(iKodUrun);
                            Product($("#iKodUrun-" + iUrunId));
                        }

                        $("#iKodUrun-" + iUrunId).select2();
                        $("#cBirimFiyati-" + iUrunId).priceFormat(optionPriceFormat);
                        $("#cFiyat-" + iUrunId).priceFormat(optionPriceFormat);
                        $("#cBirimFiyati-" + iUrunId).val("0.00");
                        $("#cFiyat-" + iUrunId).val("0.00");
                        $("#cKodu-" + iUrunId).focus();

                        ProductWrite();
                        $("#pageLoading").displayNone();
                    }
                    else {
                        $("#pageLoading").displayNone();
                    }
                }
                else {
                    $("#pageLoading").displayNone();
                }
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
                    "cActionName": "urunEkle2.js",
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

function ProductAllCode() {
    $("#cokluUrunEkleModal").modal('hide');
    $("#pageLoading").displayBlock();
    $("#cUrun3ListesiValidation").text("");

    var cBarkodlar = $("#cBarkodlar").val().split('\n');

    $.ajax({
        type: "GET",
        url: "/api/urun3api/?" + new Date(),
        contentType: "json",
        dataType: "json",
        success: function (data) {
            for (var i = 0; i < iUrunId + 1; i++) {
                if ($("#iKodUrun-" + i).val() == 0 || $("#iAdet-" + i).val() == 0 || $("#cBirimFiyati-" + i).val() == '' || $("#cFiyat-" + i).val() == '' || $("#cBirimFiyati-" + i).val() == "0.00" || $("#cFiyat-" + i).val() == "0.00") {
                    $("#iKodUrun-" + i).parent().parent().remove();
                }
            }

            if (data != null) {

                if (data.length > 0) {
                    var cHTMLOption = '';
                    $.each(data, function (i, item) {
                        if (item && item.iKodUrun3 && item.cAdi) {
                            cHTMLOption += "<option value=\"" + item.iKodUrun3 + "\">" + item.cAdi + "</option>";
                        }
                    });

                    if (cHTMLOption != '') {
                        for (var i = 0; i < cBarkodlar.length; i++) {
                            if (cBarkodlar[i] != '') {

                                var iKodUrun = 0;
                                var iAdet = 0;
                                var cBirimFiyati = '';
                                var cFiyat = '';
                                $.each(data, function (j, item) {
                                    if (item.cKodu.toLowerCase() == cBarkodlar[i].toLowerCase()) {
                                        iKodUrun = item.iKodUrun3;
                                        iAdet = item.iAdet;
                                    }
                                });

                                iUrunId = iUrunId + 1;

                                cHTML =
                                    "<div class=\"row\">" +
                                    "<div class=\"form-group col-md-2\">" +
                                    "<label for=\"cKodu\">Ürün Kodu</label>" +
                                    "<input value=\"" + cBarkodlar[i].toUpperCase() + "\" onkeypress=\"ProductCode(this)\" autocomplete=\"off\" class=\"form-control txtUrunKodu\" id=\"cKodu-" + iUrunId + "\" maxlength=\"100\" name=\"cKodu\" placeholder=\"Lütfen ürün kodu girin ...\" tabindex=\"3\">" +
                                    "</div>" +
                                    "<div class= \"form-group col-md-3\" >" +
                                    "<label for=\"iKodUrun\">Ürün</label>" +
                                    "<select onchange=\"Product(this)\" class=\"form-control cboUrunAdi\" id=\"iKodUrun-" + iUrunId + "\" tabindex=\"4\">" +
                                    "<option value =\"0\">Lütfen ürün seçin ...</option>" +
                                    cHTMLOption +
                                    "</select>" +
                                    "</div>" +
                                    "<div class=\"form-group col-md-1\">" +
                                    "<label for=\"iAdet\">Adet</label>" +
                                    "<input onchange=\"ProductWrite()\" autocomplete=\"off\" class=\"form-control txtUrunAdet\" id=\"iAdet-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen adet girin ...\" type=\"number\" value=\"1\"  tabindex=\"3\">" +
                                    "</div>" +
                                    "<div class=\"form-group col-md-2\">" +
                                    "<label for=\"iAdet\">Birim Fiyatı</label>" +
                                    "<input onchange=\"ProductWrite()\" autocomplete=\"off\" class=\"form-control txtUrunBirimFiyati\" id=\"cBirimFiyati-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen birim fiyatı girin ...\" value=\"0.00\" tabindex=\"4\">" +
                                    "</div>" +
                                    "<div class=\"form-group col-md-2\">" +
                                    "<label for=\"iAdet\">Fiyat</label>" +
                                    "<input onchange=\"ProductWrite()\" autocomplete=\"off\" class=\"form-control txtUrunFiyat\" id=\"cFiyat-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen fiyat girin ...\" value=\"0.00\" tabindex=\"5\" readonly=\"readonly\">" +
                                    "</div>" +
                                    "<div class=\"col-md-1\">" +
                                    "<button onclick=\"ProductPrint(this)\" type=\"button\" class=\"btn btn-secondary btn-block btn-delete\" tabindex=\"7\">Yazdır</button>" +
                                    "</div>" +
                                    "<div class=\"col-md-1\">" +
                                    "<button onclick=\"ProductDelete(this)\" type=\"button\" class=\"btn btn-secondary btn-block btn-delete\" tabindex=\"8\">Sil</button>" +
                                    "</div>" +
                                    "</div>";

                                $("#urun3Listesi").append(cHTML);
                                $("#iKodUrun-" + iUrunId).val(iKodUrun);
                                $("#iKodUrun-" + iUrunId).select2();
                                $("#cBirimFiyati-" + iUrunId).priceFormat(optionPriceFormat);
                                $("#cFiyat-" + iUrunId).priceFormat(optionPriceFormat);
                            }
                        }

                        ProductWrite();
                        $("#cBarkodlar").val('');

                        $("#urun3Listesi .row").each(function () {
                            if ($(this).find(".txtUrunKodu").val() == '') {
                                $(this).remove();
                            }
                        });

                        $("#pageLoading").displayNone();
                    }
                    else {
                        $("#pageLoading").displayNone();
                    }
                }
                else {
                    $("#pageLoading").displayNone();
                }
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
                    "cActionName": "urunEkle2.js",
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

function ProductCode(txtUrunKodu) {
    var keycode = event.keyCode;
    if (keycode == '13') {
        $("#pageLoading").displayBlock();
        $("#cUrun3ListesiValidation").text("");

        var cboUrunAdi = $(txtUrunKodu).parent().parent().find(".cboUrunAdi");
        var txtUrunAdet = $(txtUrunKodu).parent().parent().find(".txtUrunAdet");
        var txtUrunBirimFiyati = $(txtUrunKodu).parent().parent().find(".txtUrunBirimFiyati");
        var txtUrunFiyat = $(txtUrunKodu).parent().parent().find(".txtUrunFiyat");

        $.ajax({
            type: "GET",
            url: "/api/urunkodu2api/" + $(txtUrunKodu).val() + "/?" + new Date(),
            contentType: "json",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    var lVarMi = false;

                    if (lVarMi == false) {

                        $(cboUrunAdi).val(data.iKodUrun3).trigger('change');
                        $(txtUrunAdet).val(1);
                        $(txtUrunBirimFiyati).val("0.00");
                        $(txtUrunFiyat).val("0.00");

                        ProductAdd(false);

                        $('#cUrun3Listesi').val('');
                        $('#urun3Listesi').find('.row').each(function () {
                            if ($('#cUrun3Listesi').val() != '') {
                                $('#cUrun3Listesi').val($('#cUrun3Listesi').val() + "|");
                            }

                            $('#cUrun3Listesi').val($('#cUrun3Listesi').val() + $(this).find('.txtUrunKodu').val() + "*" + $(this).find('.cboUrunAdi').val() + "*" + $(this).find('.txtUrunAdet').val() + "*" + $(this).find('.txtUrunBirimFiyati').val() + "*" + $(this).find('.txtUrunFiyat').val())
                        });
                    }

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
                        "cActionName": "urunEkle2.js",
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

function ProductWrite() {
    var fTutar = 0;
    $('#cUrun3Listesi').val('');
    $('#urun3Listesi').find('.row').each(function () {
        var fFiyat = OndalikCevir($(this).find('.txtUrunAdet').val() * $(this).find('.txtUrunBirimFiyati').val().replace(",", ""));
        fTutar += fFiyat;
        $(this).find('.txtUrunFiyat').val(fFiyat.toFixed(2));
        $(this).find('.txtUrunFiyat').priceFormat(optionPriceFormat);

        if ($('#cUrun3Listesi').val() != '') {
            $('#cUrun3Listesi').val($('#cUrun3Listesi').val() + "|");
        }

        $('#cUrun3Listesi').val($('#cUrun3Listesi').val() + $(this).find('.txtUrunKodu').val() + "*" + $(this).find('.cboUrunAdi').val() + "*" + $(this).find('.txtUrunAdet').val() + "*" + $(this).find('.txtUrunBirimFiyati').val().replace(",", "") + "*" + $(this).find('.txtUrunFiyat').val().replace(",", ""))
    });

    $('#cTutar').val(fTutar.toFixed(2));
    $('#cGenelTutar').val(fTutar.toFixed(2));
    
    if ($('#iKDVTuru').val() == 1) {
        if ($('#iKDVOrani').val() == 1) {
            $('#cGenelTutar').val(fTutar.toFixed(2));
        } else if ($('#iKDVOrani').val() == 2) {
            $('#cGenelTutar').val((fTutar * 1.01).toFixed(2));
        } else if ($('#iKDVOrani').val() == 3) {
            $('#cGenelTutar').val((fTutar * 1.08).toFixed(2));
        } else if ($('#iKDVOrani').val() == 4) {
            $('#cGenelTutar').val((fTutar * 1.10).toFixed(2));
        } else if ($('#iKDVOrani').val() == 5) {
            $('#cGenelTutar').val((fTutar * 1.18).toFixed(2));
        }
    } else {
        $('#cGenelTutar').val(fTutar.toFixed(2));
    }

    $("#cTutar").priceFormat(optionPriceFormat);
    $("#cGenelTutar").priceFormat(optionPriceFormat);
}

function OndalikCevir(deger) {
    var dec = 2;
    var result = Math.round(deger * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
}

function Product(cboUrunAdi) {
    $("#pageLoading").displayBlock();
    $("#cUrun3ListesiValidation").text("");
    var txtUrunKodu = $(cboUrunAdi).parent().parent().find(".txtUrunKodu");
    var txtUrunAdet = $(cboUrunAdi).parent().parent().find(".txtUrunAdet");
    var txtUrunBirimFiyati = $(txtUrunKodu).parent().parent().find(".txtUrunBirimFiyati");
    var txtUrunFiyat = $(txtUrunKodu).parent().parent().find(".txtUrunFiyat");

    $.ajax({
        type: "GET",
        url: "/api/urun3api/" + $(cboUrunAdi).val() + "/?" + new Date(),
        contentType: "json",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $(txtUrunKodu).val(data.cKodu);
                $(txtUrunAdet).val(1);
                $(txtUrunBirimFiyati).val("0.00");
                $(txtUrunFiyat).val("0.00");

                ProductWrite();

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
                    "cActionName": "urunEkle2.js",
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

function ProductDelete(button) {
    if (button) {
        $(button).parent().parent().remove();
        ProductWrite();
    }
}

function ProductPrint(button) {

    setTimeout(
        function () {
            var txtUrunKodu = $(button).parent().parent().find(".txtUrunKodu");
            var cboUrunAdi = $(button).parent().parent().find(".cboUrunAdi");

            var res = $(cboUrunAdi).find('option:selected').text().replace("Kodu : ", "").replace("Adı : ", "").replace("Raf : ", "").split(" - ");

            var print = window.open('', 'Yazdır');
            print.document.open();
            print.document.write(
                '<html>' +
                '<head>' +
                '<title>Yazdır</title>' +
                '<link href="/Content/print.min.css?date=20062019-1" rel="stylesheet" />' +
                '</head>' +
                '<body class="print" onload="window.print()">' +
                '<div class="bilgi">' +
                '<img src="/Images/barkod-logo.jpg?date=20062019-1"/>' +
                '<p class="barkod-no">' + res[0] + "</p>" +
                '<p>' + res[1] + "</p>" +
                '</div>' +
                '<div class="barkod">' +
                '<p>(' + $(txtUrunKodu).val() + ")</p>" +
                '</div>' +
                '</body>' +
                '</html>');
            print.document.close();
            setTimeout(function () { newWin.close(); }, 1);
        }, 2000);


}