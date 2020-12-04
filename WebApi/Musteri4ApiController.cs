using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace otoparkyonetim.Controllers
{
    public class Musteri4ApiController : ApiController
    {
        public IHttpActionResult Put(Models.Musteri4Modal musteri4Modal)
        {
            try
            {
                if (musteri4Modal != null)
                {
                    using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                    {
                        var kontrol = (from table in dc.Musteri4s
                                       where
                                           (table.iMusteriTedarikciTipi == 1 && table.cAdi == musteri4Modal.cAdiModal && table.cSoyadi == musteri4Modal.cSoyadiModal) ||
                                           (table.iMusteriTedarikciTipi == 2 && table.cFirmaAdi == musteri4Modal.cFirmaAdiModal) &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                       select table).FirstOrDefault();

                        if (kontrol == null)
                        {
                            string cResimListesi = string.Empty;
                            if (!String.IsNullOrEmpty(musteri4Modal.cResimListesi))
                            {
                                string[] cResimler = musteri4Modal.cResimListesi.Split('|');
                                if (cResimler.Length > 0)
                                {
                                    for (int i = 0; i < cResimler.Length; i++)
                                    {
                                        if (!String.IsNullOrEmpty(cResimListesi))
                                        {
                                            cResimListesi += ",";
                                        }

                                        cResimListesi += "{\"cKucukResim\":\"th-" + cResimler[i] + "\",\"cBuyukResim\":\"bg-" + cResimler[i] + "\"}";
                                    }

                                    cResimListesi = "[" + cResimListesi + "]";
                                }

                                if (!String.IsNullOrEmpty(cResimListesi))
                                {
                                    musteri4Modal.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(cResimListesi);
                                }
                            }

                            Data.Musteri4 yenikayit = new Data.Musteri4();
                            if (musteri4Modal.iMusteriTedarikciTipiModal == 1)
                            {
                                yenikayit.cAdi = musteri4Modal.cAdiModal;
                                yenikayit.cSoyadi = musteri4Modal.cSoyadiModal;
                                yenikayit.cGSM = musteri4Modal.cGSMModal;
                                yenikayit.cTelefon = musteri4Modal.cTelefonModal;
                                yenikayit.cEMail = musteri4Modal.cEMailModal;
                                yenikayit.cVergiDairesi = musteri4Modal.cVergiDairesiModal;
                                yenikayit.cTCKimlikNo = musteri4Modal.cTCKimlikNoModal;
                                yenikayit.cFirmaAdi = string.Empty;
                                yenikayit.cTelefonKurumsal = string.Empty;
                                yenikayit.cFaks = string.Empty;
                                yenikayit.cVergiDairesiKurumsal = string.Empty;
                                yenikayit.cVergiNumarasi = string.Empty;
                                yenikayit.cEMailKurumsal = string.Empty;
                                yenikayit.cWebAdresi = string.Empty;
                                yenikayit.cAdiKurumsal = string.Empty;
                                yenikayit.cSoyadiKurumsal = string.Empty;
                                yenikayit.cGSMKurumsal = string.Empty;
                            }
                            else if (musteri4Modal.iMusteriTedarikciTipiModal == 2)
                            {
                                yenikayit.cAdi = string.Empty;
                                yenikayit.cSoyadi = string.Empty;
                                yenikayit.cGSM = string.Empty;
                                yenikayit.cTelefon = string.Empty;
                                yenikayit.cEMail = string.Empty;
                                yenikayit.cVergiDairesi = string.Empty;
                                yenikayit.cTCKimlikNo = string.Empty;
                                yenikayit.cFirmaAdi = musteri4Modal.cFirmaAdiModal;
                                yenikayit.cTelefonKurumsal = musteri4Modal.cTelefonKurumsalModal;
                                yenikayit.cFaks = musteri4Modal.cFaksModal;
                                yenikayit.cVergiDairesiKurumsal = musteri4Modal.cVergiDairesiKurumsalModal;
                                yenikayit.cVergiNumarasi = musteri4Modal.cVergiNumarasiModal;
                                yenikayit.cEMailKurumsal = musteri4Modal.cEMailKurumsalModal;
                                yenikayit.cWebAdresi = musteri4Modal.cWebModal;
                                yenikayit.cAdiKurumsal = musteri4Modal.cAdiKurumsalModal;
                                yenikayit.cSoyadiKurumsal = musteri4Modal.cSoyadiKurumsalModal;
                                yenikayit.cGSMKurumsal = musteri4Modal.cGSMKurumsalModal;
                            }

                            yenikayit.iMusteriTedarikciTipi = musteri4Modal.iMusteriTedarikciTipiModal;
                            yenikayit.cLogo = cResimListesi;
                            yenikayit.iIskontoYuzdeOrani = musteri4Modal.iIskontoYuzdeOraniModal;
                            yenikayit.iAktifMi = 1;
                            yenikayit.dTarih = DateTime.Now;
                            yenikayit.iSonGuncelleyenKullanici = musteri4Modal.iKodKullaniciLoginModal;
                            dc.Musteri4s.InsertOnSubmit(yenikayit);
                            dc.SubmitChanges();

                            Models.Musteri4Modal musteriGonder = new Models.Musteri4Modal();
                            musteriGonder.iKodMusteri4Modal = yenikayit.iKodMusteri4;
                            musteriGonder.cAdiModal = new Models.Musteri4Modal().GonderAdiModal((int)yenikayit.iKodMusteri4);
                            return Json(musteriGonder);
                        }
                        else
                        {
                            return Json(-3);
                        }
                    }
                }
                else
                {
                    return Json(-2);
                }
            }
            catch
            {
                return Json(-2);
            }
        }
    }
}
