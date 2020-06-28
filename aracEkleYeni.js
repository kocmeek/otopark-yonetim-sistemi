var iUrunId = 0;
$(function () {
    $('#cPlaka').keyup(function () {
        this.value = this.value.toLocaleUpperCase()
            .replace(/\İ/g, 'I')
            .replace(/\Ş/g, 'S')
            .replace(/\Ğ/g, 'G')
            .replace(/\Ü/g, 'U')
            .replace(/\Ö/g, 'O')
            .replace(/\Ç/g, 'C');
    });
    setTimeout(function () { $('#cPlaka').focus() }, 500);
    document.addEventListener("keydown", keyDownTextField, false);
    function keyDownTextField(e) {
        var keyCode = e.keyCode;
        if (keyCode == 113) {
            $('#iPostTipi').val(1);
            $('#form').submit();
        }
    }

    $("#btnAdd").click(function () {
        UrunEkle();
    });

    $("#cDuzeltme").ForceNumericOnly();

    $('#cDuzeltme').keyup(function () {
        setTimeout(UrunYaz(), 200);
    });
    
});

function UrunEkle() {
    $("#pageLoading").displayBlock();
    $("#cUrun2ListesiValidation").text("");

    var lSecimYap = true;
    for (var i = 0; i < iUrunId + 1; i++) {
        if ($("#iKodUrun-" + i).val() == 0 || $("#iAdet-" + i).val() == 0 || $("#cBirimFiyati-" + i).val() == '' || $("#cFiyat-" + i).val() == '' || $("#cBirimFiyati-" + i).val() == "0.00" || $("#cFiyat-" + i).val() == "0.00") {
            lSecimYap = false;
        }
    }

    if (lSecimYap == false) {
        $("#cUrun2ListesiValidation").text("Lütfen bu alanı doldurun!");
        $("#pageLoading").displayNone();
    } else {
        UrunEkle2(0);
    }
}

function UrunEkle2(iKodUrun) {
    $.ajax({
        type: "GET",
        url: "/api/urun2api/" + iKodLokasyonLogin + "?" + new Date(),
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
                        if (item && item.iKodUrun2 && item.cAdi) {
                            cHTML += "<option value=\"" + item.iKodUrun2 + "\">" + item.cAdi + "</option>";
                        }
                    });

                    if (cHTML != '') {
                        cHTML =
                            "<div class=\"row\">" +
                            "<div class=\"form-group col-md-2 pr-0\">" +
                            "<label for=\"cKodu\">Ürün Kodu</label>" +
                            "<input onkeypress=\"UrunCode(this)\" autocomplete=\"off\" class=\"form-control txtUrunKodu\" id=\"cKodu-" + iUrunId + "\" maxlength=\"100\" name=\"cKodu\" placeholder=\"Lütfen ürün kodu girin ...\" tabindex=\"1\" readonly=\"readonly\">" +
                            "</div>" +
                            "<div class= \"form-group col-md-4 pr-0\" >" +
                            "<label for=\"iKodUrun\">Ürün</label>" +
                            "<select onchange=\"Urun(this)\" class=\"form-control cboUrunAdi\" id=\"iKodUrun-" + iUrunId + "\" tabindex=\"2\">" +
                            "<option value =\"0\">Lütfen ürün seçin ...</option>" +
                            cHTML +
                            "</select>" +
                            "</div>" +
                            "<div class=\"form-group col-md-1 pr-0\">" +
                            "<label for=\"iAdet\">Adet</label>" +
                            "<input onkeyup=\"UrunYaz()\" onchange=\"UrunYaz()\" autocomplete=\"off\" class=\"form-control txtUrunAdet\" id=\"iAdet-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen adet girin ...\" type=\"number\" value=\"1\" tabindex=\"3\">" +
                            "</div>" +
                            "<div class=\"form-group col-md-2 pr-0\">" +
                            "<label for=\"iAdet\">Birim Fiyatı</label>" +
                            "<div class=\"input-group\">" +
                            "<input onchange=\"UrunYaz()\" autocomplete=\"off\" class=\"form-control txtUrunBirimFiyati\" id=\"cBirimFiyati-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen birim fiyatı girin ...\" value=\"0.00\" tabindex=\"4\" readonly=\"readonly\">" +
                            "<div class=\"input-group-append\"><span class=\"input-group-text\">TL</span></div>" +
                            "</div>" +
                            "</div>" +
                            "<div class=\"form-group col-md-2 pr-0\">" +
                            "<label for=\"iAdet\">Fiyat</label>" +
                            "<div class=\"input-group\">" +
                            "<input onchange=\"UrunYaz()\" autocomplete=\"off\" class=\"form-control txtUrunFiyat\" id=\"cFiyat-" + iUrunId + "\" maxlength=\"100\" name=\"iAdet\" placeholder=\"Lütfen fiyat girin ...\" value=\"0.00\" tabindex=\"5\" readonly=\"readonly\">" +
                            "<div class=\"input-group-append\"><span class=\"input-group-text\">TL</span></div>" +
                            "</div>" +
                            "</div>" +
                            "<div class=\"col-md-1\">" +
                            "<button onclick=\"UrunSil(this)\" type=\"button\" class=\"btn btn-secondary btn-block btn-delete\" tabindex=\"7\">Sil</button>" +
                            "</div>" +
                            "</div>";

                        $("#urun2Listesi").append(cHTML);

                        if (iKodUrun > 0) {
                            $("#iKodUrun-" + iUrunId).val(iKodUrun);
                            Urun($("#iKodUrun-" + iUrunId));
                        }

                        $("#cBirimFiyati-" + iUrunId).priceFormat(optionPriceFormat);
                        $("#cFiyat-" + iUrunId).priceFormat(optionPriceFormat);
                        $("#cBirimFiyati-" + iUrunId).val("0.00");
                        $("#cFiyat-" + iUrunId).val("0.00");
                        $("#cKodu-" + iUrunId).focus();

                        UrunYaz();
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

function Urun(cboUrunAdi) {
    $("#pageLoading").displayBlock();
    $("#cUrun2ListesiValidation").text("");
    var txtUrunKodu = $(cboUrunAdi).parent().parent().find(".txtUrunKodu");
    var txtUrunAdet = $(cboUrunAdi).parent().parent().find(".txtUrunAdet");
    var txtUrunBirimFiyati = $(txtUrunKodu).parent().parent().find(".txtUrunBirimFiyati");
    var txtUrunFiyat = $(txtUrunKodu).parent().parent().find(".txtUrunFiyat");

    if ($(cboUrunAdi).val() && $(cboUrunAdi).val() > 0) {
        $.ajax({
            type: "GET",
            url: "/api/urun2api/" + iKodLokasyonLogin + "/" + $(cboUrunAdi).val() + "/?" + new Date(),
            contentType: "json",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $(txtUrunKodu).val(data.cKodu);
                    $(txtUrunAdet).val(1);
                    $(txtUrunBirimFiyati).val(data.cSatisFiyati);
                    $(txtUrunFiyat).val(data.cSatisFiyati);
                    $(txtUrunBirimFiyati).priceFormat(optionPriceFormat);
                    $(txtUrunFiyat).priceFormat(optionPriceFormat);
                    UrunYaz();

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
    } else {
        $(txtUrunKodu).val('');
        $(txtUrunAdet).val(1);
        $(txtUrunBirimFiyati).val(0);
        $(txtUrunFiyat).val(0);
        $(txtUrunBirimFiyati).priceFormat(optionPriceFormat);
        $(txtUrunFiyat).priceFormat(optionPriceFormat);
        UrunYaz();

        $("#pageLoading").displayNone();
    }
}

function OndalikCevir(deger) {
    var dec = 2;
    var result = Math.round(deger * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
}

function UrunYaz() {
    $("#pageLoading").displayBlock();

    $('#cUrun2Listesi').val('');
    $('#urun2Listesi').find('.row').each(function () {
        var fFiyat = OndalikCevir($(this).find('.txtUrunAdet').val() * $(this).find('.txtUrunBirimFiyati').val().replace(",", ""));
        $(this).find('.txtUrunFiyat').val(fFiyat.toFixed(2));
        if ($('#cUrun2Listesi').val() != '') {
            $('#cUrun2Listesi').val($('#cUrun2Listesi').val() + "|");
        }
        $('#cUrun2Listesi').val($('#cUrun2Listesi').val() + $(this).find('.txtUrunKodu').val() + "*" + $(this).find('.cboUrunAdi').val() + "*" + $(this).find('.txtUrunAdet').val() + "*" + $(this).find('.txtUrunBirimFiyati').val() + "*" + $(this).find('.txtUrunFiyat').val())
    });

    if ($('#cGenelTutar').val()) {
        var id = $('#cUrun2Listesi').val().replace(/\*/g, '$');
        if (id == '') {
            id = "yok";
        }
        var id2 = 0;
        if ($('#iDuzeltmeTipi').val()) {
            id2 = $('#iDuzeltmeTipi').val();
        }
        var id3 = $('#cDuzeltme').val().replace(/\,/g, '').replace(/\./g, ',');
        if (id3 == '') {
            id3 = 0;
        }
        var id4 = $('#cGirisTarihi').val().replace(/\:/g, '$').replace(/\,/g, '');
        var id5 = $('#cCikisTarihi').val().replace(/\:/g, '$').replace(/\,/g, '');
        var id6 = iKodLokasyonLogin;
        var id7 = 0;
        if ($('#cAboneMi').val() == 'Evet') {
            id7 = 1;
        } else if ($('#cAboneMi').val() == 'Hayır') {
            id7 = 2;
        }
        var id8 = $('#iKodAracTipi').val();

        $.ajax({
            type: "GET",
            url: "/api/fiyat2yeniapi/" + id + "/" + id2 + "/" + id3 + "/" + id4 + "/" + id5 + "/" + id6 + "/" + id7 + "/" + id8,
            contentType: "json",
            dataType: "json",
            success: function (data) {
                $('#cGenelTutar').val(data);
                $("#pageLoading").displayNone();
            },
            error: function (xhr) {
                $("#pageLoading").displayNone();
            }
        });
    } else {
        $("#pageLoading").displayNone();
    }
}

function UrunSil(button) {
    if (button) {
        $(button).parent().parent().remove();
        UrunYaz();
    }
}

$.fn.ForceNumericOnly =
    function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;
                // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
                // home, end, period, and numpad decimal
                return (
                    key == 8 ||
                    key == 9 ||
                    key == 13 ||
                    key == 46 ||
                    key == 110 ||
                    key == 190 ||
                    (key >= 35 && key <= 40) ||
                    (key >= 48 && key <= 57) ||
                    (key >= 96 && key <= 105));
            });
        });
    };