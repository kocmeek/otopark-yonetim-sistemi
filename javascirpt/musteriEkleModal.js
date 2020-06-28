$(function () {
    
    $("#musteriEkleModal #btnEkleMusteri").click(function () {

        if ($.trim($('#musteriEkleModal #iMusteriTedarikciTipiModal').val()) == 0) {
            $("#musteriEkleModal #iMusteriTedarikciTipiModalValidation").text("Lütfen bu alanı doldurun!");
            $("#musteriEkleModal #iMusteriTedarikciTipiModalValidation").displayBlock();
            $("#musteriEkleModal #alert-1").displayBlock();
            return;
        }

        if ($('#musteriEkleModal #iMusteriTedarikciTipiModal').val() == 1) {
            if ($.trim($('#musteriEkleModal #cAdiModal').val()) == "") {
                $("#musteriEkleModal #cAdiModalValidation").text("Lütfen bu alanı doldurun!");
                $("#musteriEkleModal #cAdiModalValidation").displayBlock();
                $("#musteriEkleModal #alert-1").displayBlock();
                return;
            }

            if ($.trim($('#musteriEkleModal #cSoyadiModal').val()) == "") {
                $("#musteriEkleModal #cSoyadiModalValidation").text("Lütfen bu alanı doldurun!");
                $("#musteriEkleModal #cSoyadiModalValidation").displayBlock();
                $("#musteriEkleModal #alert-1").displayBlock();
                return;
            }
        } else if ($('#musteriEkleModal #iMusteriTedarikciTipiModal').val() == 2) {
            if ($.trim($('#musteriEkleModal #cFirmaAdi').val()) == "") {
                $("#musteriEkleModal #cFirmaAdiValidation").text("Lütfen bu alanı doldurun!");
                $("#musteriEkleModal #cFirmaAdiValidation").displayBlock();
                $("#musteriEkleModal #alert-1").displayBlock();
                return;
            }
        }

        var data = {
            "iMusteriTedarikciTipiModal": $('#musteriEkleModal #iMusteriTedarikciTipiModal').val(),
            "cAdiModal": $('#musteriEkleModal #cAdiModal').val(),
            "cSoyadiModal": $('#musteriEkleModal #cSoyadiModal').val(),
            "cTelefonModal": $('#musteriEkleModal #cTelefonModal').val(),
            "cGSMModal": $('#musteriEkleModal #cGSM').val(),
            "cEMailModal": $('#musteriEkleModal #cEMailModal').val(),
            "cVergiDairesiModal": $('#musteriEkleModal #cVergiDairesi').val(),
            "cTCKimlikNoModal": $('#musteriEkleModal #cTCKimlikNoModal').val(),
            "cFirmaAdiModal": $('#musteriEkleModal #cFirmaAdiModal').val(),
            "cAdiKurumsalModal": $('#musteriEkleModal #cAdiKurumsalModal').val(),
            "cSoyadiKurumsalModal": $("#musteriEkleModal #cSoyadiKurumsalModal").val(),
            "cTelefonKurumsalModal": $("#musteriEkleModal #cTelefonKurumsalModal").val(),
            "cFaksModal": $("#musteriEkleModal #cFaksModal").val(),
            "cGSMKurumsalModal": $("#musteriEkleModal #cGSMKurumsalModal").val(),
            "cEMailKurumsalModal": $("#musteriEkleModal #cEMailKurumsalModal").val(),
            "cWebModal": $("#musteriEkleModal #cWebModal").val(),
            "cVergiDairesiKurumsalModal": $("#musteriEkleModal #cVergiDairesiKurumsalModal").val(),
            "cVergiNumarasiModal": $("#musteriEkleModal #cVergiNumarasiModal").val(),
            "cResimListesi": $('#musteriEkleModal #cResimListesi').val(),
            "iKodKullaniciLoginModal": iKodKullaniciLogin
        };

        $.ajax({
            type: "PUT",
            url: "/api/musteri4api/",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            processData: true,
            success: function (result, status, jqXHR) {
                if (result != null) {
                    $('#iKodMusteri4').append($('<option>', {
                        value: result.iKodMusteri4Modal,
                        text: result.cAdiModal
                    }));

                    $('#iKodMusteri4').val(result.iKodMusteri4Modal).trigger('change');

                    $("#musteriEkleModal").modal('hide');
                } else if (data == -2) {
                    $("#musteriEkleModal #alert-2").displayBlock();
                }
                else if (data == -3) {
                    $("#musteriEkleModal #alert-3").displayBlock();
                }
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });

    $('#musteriEkleModal').on('show.bs.modal', function () {

        $("#musteriEkleModal #sahisModal").displayNone();
        $("#musteriEkleModal #kurumModal").displayNone();

        $('#musteriEkleModal #iMusteriTedarikciTipiModal').on('change', function () {
            if ($("#musteriEkleModal #iMusteriTedarikciTipiModal").val() == 1) {
                $("#musteriEkleModal #sahisModal").displayBlock();
                $("#musteriEkleModal #kurumModal").displayNone();
                $("#musteriEkleModal #iSehirAnahtarModal").select2();
                $("#musteriEkleModal #iIlceAnahtarModal").select2();
                $("#musteriEkleModal #iMahalleAnahtarModal").select2();
            }
            else if ($("#musteriEkleModal #iMusteriTedarikciTipiModal").val() == 2) {
                $("#musteriEkleModal #kurumModal").displayBlock();
                $("#musteriEkleModal #sahisModal").displayNone();
                $("#musteriEkleModal #iSehirAnahtarModal").select2();
                $("#musteriEkleModal #iIlceAnahtarModal").select2();
                $("#musteriEkleModal #iMahalleAnahtarModal").select2();
            }
        });

        $("#musteriEkleModal #iMusteriTedarikciTipiModal").select2();
        $("#musteriEkleModal #iMusteriTedarikciTipiModal").val("").trigger('change');
        $('#musteriEkleModal #cResimListesi').val("");
        $("#musteriEkleModal #cAdiModal").text("");
        $("#musteriEkleModal #cSoyadiModal").text("");
        $("#musteriEkleModal #cTelefonModal").text("");
        $("#musteriEkleModal #cGSMModal").text("");
        $("#musteriEkleModal #cEMailModal").text("");
        $("#musteriEkleModal #cVergiDairesiModal").text("");
        $("#musteriEkleModal #cTCKimlikNoModal").text("");
        $("#musteriEkleModal #cFirmaAdiModal").text("");
        $("#musteriEkleModal #cAdiKurumsalModal").text("");
        $("#musteriEkleModal #cSoyadiKurumsalModal").text("");
        $("#musteriEkleModal #cTelefonKurumsalModal").text("");
        $("#musteriEkleModal #cFaksModal").text("");
        $("#musteriEkleModal #cGSMKurumsalModal").text("");
        $("#musteriEkleModal #cEMailKurumsalModal").text("");
        $("#musteriEkleModal #cWebModal").text("");
        $("#musteriEkleModal #cVergiDairesiKurumsalModal").text("");
        $("#musteriEkleModal #cVergiNumarasiModal").text("");
        $("#musteriEkleModal #cTelefonModal").mask("9 (999) 999 99 99");
        $("#musteriEkleModal #cFaksModal").mask("9 (999) 999 99 99");
        $("#musteriEkleModal #cGSMModal").mask("9 (999) 999 99 99");
        $("#musteriEkleModal #cTelefonKurumsalModal").mask("9 (999) 999 99 99");
        $("#musteriEkleModal #cGSMKurumsalModal").mask("9 (999) 999 99 99");
    });
});