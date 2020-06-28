using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;
using OfficeOpenXml;


namespace otoparkcogencomtr.Controllers
{
    public class AracController : Controller
    {
        #region Eski

        [HttpGet]
        public ActionResult Ekle(string id, string id2, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 48))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.KDVTuruListesi = new Models.KDVTuru().Gonder();
                ViewBag.KDVOraniListesi = new Models.KDVOrani().Gonder();
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodArac = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0)
                    {
                        int iSayfaTipi = 0;
                        if (!String.IsNullOrEmpty(id2))
                        {
                            iSayfaTipi = Convert.ToInt32(id2);
                        }

                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           table.iAktifMi == 1 &&
                                           (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                           (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1) &&
                                           (tableKullanici2s != null && tableKullanici2s.iAktifMi == 1)
                                         select new Models.Arac
                                         {
                                             iKodArac = table.iKodArac,
                                             iKodAracTipi = table.iKodAracTipi,
                                             cAracTipi = tableAracTipis.cAdi,
                                             iKodMusteri3 = (int)table.iKodMusteri3,
                                             cMusteri3 = tableMusteri3s.cPlaka,
                                             dGirisTarihi = (DateTime)table.dGirisTarihi,
                                             cGirisTarihi = table.dGirisTarihi.Value.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             dCikisTarihi = DateTime.Now,
                                             cCikisTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             cOtoparkSuresi = new Models.Fiyat2().Fiyat2SureHesapla(Convert.ToDateTime(table.dGirisTarihi), DateTime.Now).ToString(),
                                             cOtoparkUcreti = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(false, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             cUrun = table.cUrun,
                                             cDuzeltme = string.Format("{0:N2}", table.fDuzeltme),
                                             iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                             cAciklama = table.cAciklama,
                                             cTutar = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             iKDVTuru = table.iKDVTuru != null ? (int)table.iKDVTuru : 0,
                                             iKDVOrani = table.iKDVOrani != null ? (int)table.iKDVOrani : 0,
                                             cGenelTutar = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                             iSayfaTipi = iSayfaTipi,
                                             cPersonel = tableKullanici2s.cAdi != null && tableKullanici2s.cSoyadi != null ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-",
                                             iUrunSilindiMi = (table.iUrunSilindiMi != null ? table.iUrunSilindiMi : 0),
                                             lDurum = new Models.Fiyat2().Fiyat2DusurduMu(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)),
                                             cAboneMi = new Models.Abonelik().AboneMiYazi((int)table.iKodMusteri3),
                                             iEkleFormuAcilsinMi = 0
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cUrun))
                                {
                                    okuma.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(okuma.cUrun);
                                    string cUrunler = string.Empty;
                                    for (int i = 0; i < okuma.urun2Listesi.Count; i++)
                                    {
                                        okuma.urun2Listesi[i].iStokTutlacakMi = new Models.Urun2().GonderStokTutulacakMi(okuma.urun2Listesi[i].iKodUrun2, iKodLokasyonLogin);

                                        if (!String.IsNullOrEmpty(cUrunler))
                                        {
                                            cUrunler += "|";
                                        }

                                        cUrunler += okuma.urun2Listesi[i].cKodu + "*" + okuma.urun2Listesi[i].iKodUrun2 + "*" + okuma.urun2Listesi[i].iAdet + "*" + okuma.urun2Listesi[i].cBirimFiyati + "*" + okuma.urun2Listesi[i].cFiyat;
                                    }
                                    okuma.cUrun2Listesi = cUrunler;
                                }
                            }

                            return View(okuma);
                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -2;
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "Ekle_Get", Ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Ekle(Models.Arac arac, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 48))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.KDVTuruListesi = new Models.KDVTuru().Gonder();
                ViewBag.KDVOraniListesi = new Models.KDVOrani().Gonder();
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;

                if (arac != null)
                {
                    string cUrun2Listesi = string.Empty;
                    if (!String.IsNullOrEmpty(arac.cUrun2Listesi))
                    {
                        string[] cUrunler = arac.cUrun2Listesi.Split('|');
                        if (cUrunler.Length > 0)
                        {
                            for (int i = 0; i < cUrunler.Length; i++)
                            {
                                if (!String.IsNullOrEmpty(cUrun2Listesi))
                                {
                                    cUrun2Listesi += ",";
                                }

                                string[] cUrun = cUrunler[i].Split('*');

                                if (String.IsNullOrEmpty(cUrun[0]))
                                {
                                    cUrun[0] = "";
                                }

                                if (String.IsNullOrEmpty(cUrun[1]))
                                {
                                    cUrun[1] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[2] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[3] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[4] = "0";
                                }

                                cUrun2Listesi += "{\"cKodu\":\"" + cUrun[0] + "\",\"iKodUrun2\":\"" + cUrun[1] + "\",\"iAdet\":\"" + cUrun[2] + "\",\"cBirimFiyati\":\"" + cUrun[3] + "\",\"cFiyat\":\"" + cUrun[4] + "\"}";
                            }

                            cUrun2Listesi = "[" + cUrun2Listesi + "]";
                        }

                        if (!String.IsNullOrEmpty(cUrun2Listesi))
                        {
                            arac.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(cUrun2Listesi);

                            if (arac.urun2Listesi != null)
                            {
                                for (int i = 0; i < arac.urun2Listesi.Count; i++)
                                {
                                    if (String.IsNullOrEmpty(arac.urun2Listesi[i].cKodu) || arac.urun2Listesi[i].iKodUrun2 == 0 || arac.urun2Listesi[i].iAdet == 0 || String.IsNullOrEmpty(arac.urun2Listesi[i].cBirimFiyati) || arac.urun2Listesi[i].cBirimFiyati == "0.00" || String.IsNullOrEmpty(arac.urun2Listesi[i].cFiyat) || arac.urun2Listesi[i].cFiyat == "0.00")
                                    {
                                        ModelState.AddModelError("cUrun2Listesi", "Lütfen bu alanı doldurun!");
                                    }
                                }
                            }
                        }
                    }

                    if (arac.iKodArac > 0)
                    {
                        if (((String.IsNullOrEmpty(arac.cDuzeltme) || Convert.ToDouble(arac.cDuzeltme.ToString().Replace(",", "").Replace(".", ",")) == 0) && (arac.iDuzeltmeTipi != null && arac.iDuzeltmeTipi > 0)))
                        {
                            ModelState.AddModelError("cDuzeltme", "Lütfen bu alanı doldurun!");
                        }

                        if (((!String.IsNullOrEmpty(arac.cDuzeltme) && Convert.ToDouble(arac.cDuzeltme.ToString().Replace(",", "").Replace(".", ",")) > 0) && (arac.iDuzeltmeTipi == null || arac.iDuzeltmeTipi == 0)))
                        {
                            ModelState.AddModelError("iDuzeltmeTipi", "Lütfen bu alanı doldurun!");
                        }

                        if (arac.cDuzeltme != null && !String.IsNullOrEmpty(arac.cDuzeltme) &&
                        Convert.ToDouble(arac.cDuzeltme.ToString().Replace(",", "").Replace(".", ",")) > 0 &&
                        String.IsNullOrEmpty(arac.cAciklama))
                        {
                            ModelState.AddModelError("cAciklama", "Lütfen düzeltme yapma nedeninizi yazınız!");
                        }

                        if (arac.iUrunSilindiMi == 1 && String.IsNullOrEmpty(arac.cAciklama))
                        {
                            ModelState.AddModelError("cAciklama", "Lütden ürün silme nedeninizi yazınız!");
                        }

                        if (arac.iKDVTuru == 0)
                        {
                            ModelState.AddModelError("iKDVTuru", "Lütfen bu alanı doldurun!");
                        }

                        if (arac.iKDVOrani == 0)
                        {
                            ModelState.AddModelError("iKDVTuru", "Lütfen bu alanı doldurun!");
                        }

                        if (ModelState.IsValid)
                        {
                            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                            {
                                var guncelleme = (from table in dc.Aracs
                                                  where
                                                    table.iKodArac == arac.iKodArac &&
                                                    table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                                    table.iAktifMi == 1
                                                  select table).FirstOrDefault();

                                if (guncelleme != null)
                                {
                                    List<Models.UrunJson2> oncekiUrun2Listesi = null;
                                    if (!String.IsNullOrEmpty(guncelleme.cUrun))
                                    {
                                        oncekiUrun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(guncelleme.cUrun);

                                        if (oncekiUrun2Listesi != null && oncekiUrun2Listesi.Count > 0 && arac.urun2Listesi != null && arac.urun2Listesi.Count > 0)
                                        {
                                            for (int i = 0; i < arac.urun2Listesi.Count; i++)
                                            {
                                                var resultSuankiUrunAdedi =
                                                    (from table in dc.Urun2s
                                                     where
                                                         table.iKodUrun2 == arac.urun2Listesi[i].iKodUrun2 &&
                                                         table.iStokTutlacakMi == 1 &&
                                                         table.iAktifMi == 1
                                                     select table).FirstOrDefault();

                                                if (resultSuankiUrunAdedi != null)
                                                {
                                                    for (int j = 0; j < oncekiUrun2Listesi.Count; j++)
                                                    {
                                                        if (arac.urun2Listesi[i].iKodUrun2 == oncekiUrun2Listesi[j].iKodUrun2)
                                                        {
                                                            if ((int)resultSuankiUrunAdedi.iAdet + oncekiUrun2Listesi[j].iAdet < arac.urun2Listesi[i].iAdet)
                                                            {
                                                                ModelState.AddModelError("cUrun2Listesi", "Çıkış adedi, adetten büyük olamaz!");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (arac.iSayfaTipi == 1)
                                    {
                                        guncelleme.fOtoparkSuresi = Convert.ToDouble(arac.cOtoparkSuresi.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.fOtoparkUcreti = Convert.ToDouble(arac.cOtoparkUcreti.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.cUrun = cUrun2Listesi;
                                        guncelleme.fDuzeltme = Convert.ToDouble(arac.cDuzeltme.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.iDuzeltmeTipi = arac.iDuzeltmeTipi;
                                        guncelleme.cAciklama = arac.cAciklama;
                                        guncelleme.fTutar = Convert.ToDouble(arac.cTutar.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.iKDVTuru = arac.iKDVTuru;
                                        guncelleme.iKDVOrani = arac.iKDVOrani;
                                        guncelleme.fGenelTutar = Convert.ToDouble(arac.cGenelTutar.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.dCikisTarihi = DateTime.Now;
                                        guncelleme.iUrunSilindiMi = arac.iUrunSilindiMi;
                                        guncelleme.dTarih = DateTime.Now;
                                        guncelleme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                        dc.SubmitChanges();

                                        Data.Gelir2 gelir2 = new Data.Gelir2();
                                        gelir2.cAciklama = guncelleme.cAciklama;
                                        gelir2.dGelirTarihi = guncelleme.dCikisTarihi;
                                        gelir2.dTarih = guncelleme.dTarih;
                                        gelir2.fGenelTutar = guncelleme.fGenelTutar;
                                        gelir2.fTutar = guncelleme.fTutar;
                                        gelir2.iAktifMi = 1;
                                        gelir2.iBaglanti = guncelleme.iKodArac;
                                        gelir2.iKDVOrani = guncelleme.iKDVOrani;
                                        gelir2.iKDVTuru = guncelleme.iKDVTuru;
                                        gelir2.iKodLokasyon = guncelleme.iKodLokasyon;
                                        gelir2.iKodMusteri3 = guncelleme.iKodMusteri3;
                                        gelir2.iSonGuncelleyenKullanici = guncelleme.iSonGuncelleyenKullanici;
                                        dc.Gelir2s.InsertOnSubmit(gelir2);
                                        dc.SubmitChanges();

                                        Data.Tahsilat2 tahsilat2 = new Data.Tahsilat2();
                                        tahsilat2.cAciklama = guncelleme.cAciklama;
                                        tahsilat2.cFotograf = string.Empty;
                                        tahsilat2.dTahsilatTarihi = guncelleme.dCikisTarihi;
                                        tahsilat2.dTarih = guncelleme.dTarih;
                                        tahsilat2.fTutar = guncelleme.fGenelTutar;
                                        tahsilat2.iAktifMi = 1;
                                        tahsilat2.iKodBanka2 = 0;
                                        tahsilat2.iKodGelir2 = gelir2.iKodGelir2;
                                        tahsilat2.iKodLokasyon = guncelleme.iKodLokasyon;
                                        tahsilat2.iKodTahsilatYontemi = 1;
                                        tahsilat2.iSonGuncelleyenKullanici = guncelleme.iSonGuncelleyenKullanici;
                                        dc.Tahsilat2s.InsertOnSubmit(tahsilat2);
                                        dc.SubmitChanges();
                                    }
                                    else if (arac.iSayfaTipi == 2)
                                    {
                                        guncelleme.fOtoparkSuresi = Convert.ToDouble(arac.cOtoparkSuresi.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.fOtoparkUcreti = Convert.ToDouble(arac.cOtoparkUcreti.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.cUrun = cUrun2Listesi;
                                        guncelleme.fDuzeltme = Convert.ToDouble(arac.cDuzeltme.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.iDuzeltmeTipi = arac.iDuzeltmeTipi;
                                        guncelleme.cAciklama = arac.cAciklama;
                                        guncelleme.fTutar = Convert.ToDouble(arac.cTutar.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.iKDVTuru = arac.iKDVTuru;
                                        guncelleme.iKDVOrani = arac.iKDVOrani;
                                        guncelleme.fGenelTutar = Convert.ToDouble(arac.cGenelTutar.ToString().Replace(",", "").Replace(".", ","));
                                        guncelleme.iUrunSilindiMi = arac.iUrunSilindiMi;
                                        guncelleme.dTarih = DateTime.Now;
                                        guncelleme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                        dc.SubmitChanges();
                                    }

                                    if (arac.urun2Listesi != null && arac.urun2Listesi.Count > 0)
                                    {
                                        for (int i = 0; i < arac.urun2Listesi.Count; i++)
                                        {
                                            int iOncekiAdet = 0;
                                            if (oncekiUrun2Listesi != null && oncekiUrun2Listesi.Count > 0)
                                            {
                                                for (int j = 0; j < oncekiUrun2Listesi.Count; j++)
                                                {
                                                    if (oncekiUrun2Listesi[j].iKodUrun2 == arac.urun2Listesi[i].iKodUrun2)
                                                    {
                                                        iOncekiAdet = oncekiUrun2Listesi[j].iAdet;
                                                        break;
                                                    }
                                                }
                                            }

                                            var urunÜrünGuncelle =
                                                (from table in dc.Urun2s
                                                 where
                                                    table.iKodUrun2 == arac.urun2Listesi[i].iKodUrun2 &&
                                                    table.iStokTutlacakMi == 1 &&
                                                    table.iAktifMi == 1
                                                 select table).FirstOrDefault();

                                            if (urunÜrünGuncelle != null)
                                            {
                                                urunÜrünGuncelle.iAdet += iOncekiAdet;
                                                urunÜrünGuncelle.iAdet -= arac.urun2Listesi[i].iAdet;
                                                dc.SubmitChanges();
                                            }
                                        }

                                        if (oncekiUrun2Listesi != null && oncekiUrun2Listesi.Count > 0)
                                        {
                                            for (int j = 0; j < oncekiUrun2Listesi.Count; j++)
                                            {
                                                bool lUrunVarMi = false;
                                                for (int i = 0; i < arac.urun2Listesi.Count; i++)
                                                {
                                                    if (oncekiUrun2Listesi[j].iKodUrun2 == arac.urun2Listesi[i].iKodUrun2)
                                                    {
                                                        lUrunVarMi = true;
                                                        break;
                                                    }
                                                }

                                                if (lUrunVarMi == false)
                                                {
                                                    var urunÜrünGuncelle =
                                                       (from table in dc.Urun2s
                                                        where
                                                           table.iKodUrun2 == oncekiUrun2Listesi[j].iKodUrun2 &&
                                                           table.iStokTutlacakMi == 1 &&
                                                           table.iAktifMi == 1
                                                        select table).FirstOrDefault();

                                                    if (urunÜrünGuncelle != null)
                                                    {
                                                        urunÜrünGuncelle.iAdet += oncekiUrun2Listesi[j].iAdet;
                                                        dc.SubmitChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    ViewBag.iSonuc = 2;
                                    if (arac.iEkleFormuAcilsinMi == 1)
                                    {
                                        return Redirect("/Arac/Ekle");
                                    }
                                    else
                                    {
                                        return Redirect("/Arac/BekleyenListele");
                                    }
                                }
                                else
                                {
                                    ViewBag.iSonuc = -2;
                                }
                            }
                        }
                        else
                        {
                            ViewBag.iSonuc = -5;
                        }
                    }
                    else
                    {
                        if ((arac.iKodMusteri3 == null && String.IsNullOrEmpty(arac.cPlaka)) ||
                        (arac.iKodMusteri3 != null && arac.iKodMusteri3 > 0 && !String.IsNullOrEmpty(arac.cPlaka)))
                        {
                            ModelState.AddModelError("iKodMusteri3", "Lütfen müşteri veya plaka alanlarından birini doldurun!");
                            ModelState.AddModelError("cPlaka", "Lütfen müşteri veya plaka alanlarından birini doldurun!");
                        }

                        if (ModelState.IsValid)
                        {
                            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                            {
                                var kontrol = (from table in dc.Aracs
                                               join tableMusteri3s in dc.Musteri3s
                                                    on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                               from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                               where
                                                 (table.iKodMusteri3 == arac.iKodMusteri3 || tableMusteri3s.cPlaka == arac.cPlaka) &&
                                                 table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                                 table.iAktifMi == 1
                                               select table).FirstOrDefault();

                                if (kontrol == null)
                                {
                                    int iKodMusteri3 = 0;
                                    if (arac.iKodMusteri3 != null)
                                    {
                                        iKodMusteri3 = (int)arac.iKodMusteri3;
                                    }

                                    string cPlaka = string.Empty;
                                    if (!String.IsNullOrEmpty(arac.cPlaka))
                                    {
                                        cPlaka = arac.cPlaka;
                                        var musteri3Kontrol = (from table in dc.Musteri3s
                                                               where
                                                                 table.cPlaka == arac.cPlaka &&
                                                                 table.iAktifMi == 1
                                                               select table).FirstOrDefault();

                                        if (musteri3Kontrol == null)
                                        {
                                            Data.Musteri3 musteri3 = new Data.Musteri3();
                                            musteri3.cPlaka = arac.cPlaka;
                                            musteri3.iAktifMi = 1;
                                            musteri3.dTarih = DateTime.Now;
                                            musteri3.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                            dc.Musteri3s.InsertOnSubmit(musteri3);
                                            dc.SubmitChanges();
                                            iKodMusteri3 = musteri3.iKodMusteri3;
                                        }
                                        else
                                        {
                                            iKodMusteri3 = musteri3Kontrol.iKodMusteri3;
                                        }
                                    }
                                    else
                                    {
                                        var musteri3 = (from table in dc.Musteri3s
                                                        where
                                                            table.iKodMusteri3 == arac.iKodMusteri3 &&
                                                            table.iAktifMi == 1
                                                        select table).FirstOrDefault();

                                        if (musteri3 == null)
                                        {
                                            cPlaka = musteri3.cPlaka;
                                        }
                                    }

                                    Data.Arac yenikayit = new Data.Arac();
                                    yenikayit.iKodLokasyon = iKodLokasyonLogin;
                                    yenikayit.iKodAracTipi = arac.iKodAracTipi;
                                    yenikayit.iKodMusteri3 = iKodMusteri3;
                                    yenikayit.dGirisTarihi = DateTime.Now;
                                    yenikayit.dCikisTarihi = Convert.ToDateTime("1900-01-01");
                                    yenikayit.fOtoparkSuresi = 0;
                                    yenikayit.fOtoparkUcreti = 0;
                                    yenikayit.cUrun = string.Empty;
                                    yenikayit.fDuzeltme = 0;
                                    yenikayit.iDuzeltmeTipi = 0;
                                    yenikayit.fTutar = 0;
                                    yenikayit.iKDVTuru = 0;
                                    yenikayit.iKDVOrani = 0;
                                    yenikayit.fGenelTutar = 0;
                                    yenikayit.cAciklama = string.Empty;
                                    yenikayit.iUrunSilindiMi = 0;
                                    yenikayit.dTarih = DateTime.Now;
                                    yenikayit.iAktifMi = 1;
                                    yenikayit.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                    dc.Aracs.InsertOnSubmit(yenikayit);
                                    dc.SubmitChanges();

                                    BarkodYazdir(yenikayit.iKodArac, cPlaka, (int)yenikayit.iKodAracTipi, Convert.ToDateTime(yenikayit.dGirisTarihi));

                                    ViewBag.iSonuc = 1;
                                    return Redirect("/Arac/BekleyenListele");
                                }
                                else
                                {
                                    ViewBag.iSonuc = -3;
                                }
                            }
                        }
                        else
                        {
                            ViewBag.iSonuc = -1;
                        }
                    }
                }
                else
                {
                    ViewBag.iSonuc = -2;
                }

            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "Ekle_Post", Ex.Message);
            }

            return View(arac);
        }

        [HttpGet]
        public ActionResult BekleyenDetayliBilgi(string id, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 48))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iKodArac = id;
                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.KDVTuruListesi = new Models.KDVTuru().Gonder();
                ViewBag.KDVOraniListesi = new Models.KDVOrani().Gonder();
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodArac = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableLokasyons in dc.Lokasyons
                                            on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                         from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           table.iAktifMi == 1 &&
                                           (tableAracTipis != null && (tableAracTipis.iAktifMi == 0 || tableAracTipis.iAktifMi == 1)) &&
                                           (tableMusteri3s != null && (tableMusteri3s.iAktifMi == 0 || tableMusteri3s.iAktifMi == 1)) &&
                                           (tableLokasyons != null && (tableLokasyons.iAktifMi == 0 || tableLokasyons.iAktifMi == 1)) &&
                                           (tableKullanici2s != null && (tableKullanici2s.iAktifMi == 0 || tableKullanici2s.iAktifMi == 1))
                                         select new Models.Arac
                                         {
                                             iKodArac = table.iKodArac,
                                             iKodAracTipi = table.iKodAracTipi,
                                             cAracTipi = tableAracTipis.cAdi,
                                             iKodMusteri3 = (int)table.iKodMusteri3,
                                             cMusteri3 = tableMusteri3s.cPlaka,
                                             dGirisTarihi = (DateTime)table.dGirisTarihi,
                                             cGirisTarihi = table.dGirisTarihi.Value.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             dCikisTarihi = DateTime.Now,
                                             cCikisTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             cOtoparkSuresi = new Models.Fiyat2().Fiyat2SureHesapla(Convert.ToDateTime(table.dGirisTarihi), DateTime.Now).ToString(),
                                             cOtoparkUcreti = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(false, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             cUrun = table.cUrun,
                                             cDuzeltme = string.Format("{0:N2}", table.fDuzeltme),
                                             iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                             cAciklama = table.cAciklama,
                                             cTutar = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             iKDVTuru = table.iKDVTuru != null ? (int)table.iKDVTuru : 0,
                                             iKDVOrani = table.iKDVOrani != null ? (int)table.iKDVOrani : 0,
                                             cGenelTutar = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                             cPersonel = tableKullanici2s.cAdi != null && tableKullanici2s.cSoyadi != null ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-",
                                             iUrunSilindiMi = (table.iUrunSilindiMi != null ? table.iUrunSilindiMi : 0),
                                             cAboneMi = new Models.Abonelik().AboneMiYazi((int)table.iKodMusteri3)
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cUrun))
                                {
                                    okuma.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(okuma.cUrun);
                                    string cUrunler = string.Empty;
                                    for (int i = 0; i < okuma.urun2Listesi.Count; i++)
                                    {
                                        if (!String.IsNullOrEmpty(cUrunler))
                                        {
                                            cUrunler += "|";
                                        }

                                        cUrunler += okuma.urun2Listesi[i].cKodu + "*" + okuma.urun2Listesi[i].iKodUrun2 + "*" + okuma.urun2Listesi[i].iAdet + "*" + okuma.urun2Listesi[i].cBirimFiyati + "*" + okuma.urun2Listesi[i].cFiyat;
                                    }
                                    okuma.cUrun2Listesi = cUrunler;
                                }
                            }

                            return View(okuma);
                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -2;
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "DetayliBilgi_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult TumunuDetayliBilgi(string id, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi, int? iKodLokasyon, int? iAracDurumu)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 75))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iKodArac = id;
                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
                ViewBag.iKodLokasyon = iKodLokasyon;
                ViewBag.iAracDurumu = iAracDurumu;

                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(0);
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.KDVTuruListesi = new Models.KDVTuru().Gonder();
                ViewBag.KDVOraniListesi = new Models.KDVOrani().Gonder();
                ViewBag.iKodLokasyonLogin = 0;

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodArac = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableLokasyons in dc.Lokasyons
                                            on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                         from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iAktifMi == 1 &&
                                           (tableAracTipis != null && (tableAracTipis.iAktifMi == 0 || tableAracTipis.iAktifMi == 1)) &&
                                           (tableMusteri3s != null && (tableMusteri3s.iAktifMi == 0 || tableMusteri3s.iAktifMi == 1)) &&
                                           (tableLokasyons != null && (tableLokasyons.iAktifMi == 0 || tableLokasyons.iAktifMi == 1)) &&
                                           (tableKullanici2s != null && (tableKullanici2s.iAktifMi == 0 || tableKullanici2s.iAktifMi == 1))
                                         select new Models.Arac
                                         {
                                             iKodArac = table.iKodArac,
                                             iKodAracTipi = table.iKodAracTipi,
                                             cAracTipi = tableAracTipis.cAdi,
                                             iKodMusteri3 = (int)table.iKodMusteri3,
                                             cMusteri3 = tableMusteri3s.cPlaka,
                                             dGirisTarihi = (DateTime)table.dGirisTarihi,
                                             cGirisTarihi = table.dGirisTarihi.Value.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             dCikisTarihi = DateTime.Now,
                                             cCikisTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             cOtoparkSuresi = new Models.Fiyat2().Fiyat2SureHesapla(Convert.ToDateTime(table.dGirisTarihi), DateTime.Now).ToString(),
                                             cOtoparkUcreti = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(false, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             cUrun = table.cUrun,
                                             cDuzeltme = string.Format("{0:N2}", table.fDuzeltme),
                                             iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                             cAciklama = table.cAciklama,
                                             cTutar = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             iKDVTuru = table.iKDVTuru != null ? (int)table.iKDVTuru : 0,
                                             iKDVOrani = table.iKDVOrani != null ? (int)table.iKDVOrani : 0,
                                             cGenelTutar = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                             cPersonel = tableKullanici2s.cAdi != null && tableKullanici2s.cSoyadi != null ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-",
                                             iUrunSilindiMi = (table.iUrunSilindiMi != null ? table.iUrunSilindiMi : 0),
                                             cAboneMi = new Models.Abonelik().AboneMiYazi((int)table.iKodMusteri3)
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cUrun))
                                {
                                    okuma.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(okuma.cUrun);
                                    string cUrunler = string.Empty;
                                    for (int i = 0; i < okuma.urun2Listesi.Count; i++)
                                    {
                                        if (!String.IsNullOrEmpty(cUrunler))
                                        {
                                            cUrunler += "|";
                                        }

                                        cUrunler += okuma.urun2Listesi[i].cKodu + "*" + okuma.urun2Listesi[i].iKodUrun2 + "*" + okuma.urun2Listesi[i].iAdet + "*" + okuma.urun2Listesi[i].cBirimFiyati + "*" + okuma.urun2Listesi[i].cFiyat;
                                    }
                                    okuma.cUrun2Listesi = cUrunler;
                                }
                            }

                            return View(okuma);
                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -2;
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "DetayliBilgi_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult LokasyonDetayliBilgi(string id, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi, int? iAracDurumu)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 131))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iKodArac = id;
                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
                ViewBag.iAracDurumu = iAracDurumu;

                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(0);
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.KDVTuruListesi = new Models.KDVTuru().Gonder();
                ViewBag.KDVOraniListesi = new Models.KDVOrani().Gonder();
                ViewBag.iKodLokasyonLogin = 0;

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodArac = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableLokasyons in dc.Lokasyons
                                            on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                         from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iAktifMi == 1 &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           (tableAracTipis != null && (tableAracTipis.iAktifMi == 0 || tableAracTipis.iAktifMi == 1)) &&
                                           (tableMusteri3s != null && (tableMusteri3s.iAktifMi == 0 || tableMusteri3s.iAktifMi == 1)) &&
                                           (tableLokasyons != null && (tableLokasyons.iAktifMi == 0 || tableLokasyons.iAktifMi == 1)) &&
                                           (tableKullanici2s != null && (tableKullanici2s.iAktifMi == 0 || tableKullanici2s.iAktifMi == 1))
                                         select new Models.Arac
                                         {
                                             iKodArac = table.iKodArac,
                                             iKodAracTipi = table.iKodAracTipi,
                                             cAracTipi = tableAracTipis.cAdi,
                                             iKodMusteri3 = (int)table.iKodMusteri3,
                                             cMusteri3 = tableMusteri3s.cPlaka,
                                             dGirisTarihi = (DateTime)table.dGirisTarihi,
                                             cGirisTarihi = table.dGirisTarihi.Value.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             dCikisTarihi = (DateTime)table.dCikisTarihi,
                                             cCikisTarihi = table.dCikisTarihi.Value.ToString("dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR")),
                                             cOtoparkSuresi = new Models.Fiyat2().Fiyat2SureHesapla(Convert.ToDateTime(table.dGirisTarihi), Convert.ToDateTime(table.dCikisTarihi)).ToString(),
                                             cOtoparkUcreti = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(false, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), Convert.ToDateTime(table.dCikisTarihi), table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             cUrun = table.cUrun,
                                             cDuzeltme = string.Format("{0:N2}", table.fDuzeltme),
                                             iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                             cAciklama = table.cAciklama,
                                             cTutar = string.Format("{0:N2}", new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), Convert.ToDateTime(table.dCikisTarihi), table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0))).Replace(",", "."),
                                             iKDVTuru = table.iKDVTuru != null ? (int)table.iKDVTuru : 0,
                                             iKDVOrani = table.iKDVOrani != null ? (int)table.iKDVOrani : 0,
                                             cGenelTutar = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), Convert.ToDateTime(table.dCikisTarihi), table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                             cPersonel = tableKullanici2s.cAdi != null && tableKullanici2s.cSoyadi != null ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-",
                                             iUrunSilindiMi = (table.iUrunSilindiMi != null ? table.iUrunSilindiMi : 0),
                                             cAboneMi = new Models.Abonelik().AboneMiYazi((int)table.iKodMusteri3)
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cUrun))
                                {
                                    okuma.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(okuma.cUrun);
                                    string cUrunler = string.Empty;
                                    for (int i = 0; i < okuma.urun2Listesi.Count; i++)
                                    {
                                        if (!String.IsNullOrEmpty(cUrunler))
                                        {
                                            cUrunler += "|";
                                        }

                                        cUrunler += okuma.urun2Listesi[i].cKodu + "*" + okuma.urun2Listesi[i].iKodUrun2 + "*" + okuma.urun2Listesi[i].iAdet + "*" + okuma.urun2Listesi[i].cBirimFiyati + "*" + okuma.urun2Listesi[i].cFiyat;
                                    }
                                    okuma.cUrun2Listesi = cUrunler;
                                }
                            }

                            return View(okuma);
                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -2;
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "DetayliBilgi_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult BekleyenListele(int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 48))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     join tableLokasyons in dc.Lokasyons
                                        on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                     from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                     join tableKullanici2s in dc.Kullanici2s
                                        on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                     from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                        table.iAktifMi == 1 &&
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1) &&
                                        (tableKullanici2s != null && tableKullanici2s.iAktifMi == 1)
                                     select new Models.Arac
                                     {
                                         iKodArac = table.iKodArac,
                                         cResim = (tableAracTipis != null && tableAracTipis.cFotograf != null && tableAracTipis.cFotograf.ToString() != string.Empty ? tableAracTipis.cFotograf : string.Empty),
                                         iKodMusteri3 = (int)table.iKodMusteri3,
                                         iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cMusteri3 =
                                            "Firma Adı : " + (tableMusteri3s != null && tableMusteri3s.cFirmaAdi != null && tableMusteri3s.cFirmaAdi != string.Empty ? tableMusteri3s.cFirmaAdi : "-") +
                                            "<br/>Ad Soyad : " + (tableMusteri3s != null && tableMusteri3s.cAdi != null && tableMusteri3s.cAdi != string.Empty && tableMusteri3s.cSoyadi != null && tableMusteri3s.cSoyadi != string.Empty ? tableMusteri3s.cAdi + " " + tableMusteri3s.cSoyadi : "-") +
                                            "<br/>Lokasyon : " + (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-") +
                                            "<br/>Kullanıcı : " + (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cLokasyon = (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-"),
                                         cFirmaAdi = (tableMusteri3s != null && tableMusteri3s.cFirmaAdi != null && tableMusteri3s.cFirmaAdi != string.Empty ? tableMusteri3s.cFirmaAdi : "-"),
                                         cAdSoyad = (tableMusteri3s != null && tableMusteri3s.cAdi != null && tableMusteri3s.cAdi != string.Empty && tableMusteri3s.cSoyadi != null && tableMusteri3s.cSoyadi != string.Empty ? tableMusteri3s.cAdi + " " + tableMusteri3s.cSoyadi : "-"),
                                         cKullanici = (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         dGirisTarihi = Convert.ToDateTime(table.dGirisTarihi),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", table.dGirisTarihi),
                                         dCikisTarihi = Convert.ToDateTime(table.dCikisTarihi),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", table.dCikisTarihi),
                                         cOtoparkUcreti = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                         fOtoparkUcreti = new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                         fGenelTutar = new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0),
                                         lAboneMi = new Models.Abonelik().AboneMi((int)table.iKodMusteri3)
                                     }).OrderByDescending(x => x.iKodArac).ToList();

                    if (!String.IsNullOrEmpty(arama))
                    {
                        if (arama.Length > 0 && arama.Substring(0, 1) == "*")
                        {
                            string cPlaka = arama.Replace("*", "");
                            var aracKoduArama = listeleme.Where(model => model.cPlaka == cPlaka).FirstOrDefault();
                            if (aracKoduArama != null)
                            {
                                return Redirect("/Arac/Ekle/" + aracKoduArama.iKodArac + "/1");
                            }
                        }

                        List<string> cSplitArama = arama.Split(',').Select(p => p.ToLower().Trim()).ToList();
                        for (int i = 0; i < cSplitArama.Count; i++)
                        {
                            if (String.IsNullOrEmpty(cSplitArama[i]))
                            {
                                cSplitArama.Remove(cSplitArama[i]);
                                i--;
                            }
                        }
                        if (cSplitArama.Count > 0)
                        {
                            ViewBag.cSplitArama = cSplitArama;
                            ViewBag.cArama = String.Join(", ", cSplitArama.ToArray());

                            listeleme = listeleme.Where(
                                    model =>
                                    cSplitArama.Any(model.iKodArac.ToString().ToLower().Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cLokasyon).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cFirmaAdi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cAdSoyad).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cKullanici).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cPlaka).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cGirisTarihi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cOtoparkUcreti).Contains)
                                    ).ToList();
                        }
                        else
                        {
                            ViewBag.cSplitArama = null;
                            ViewBag.cArama = string.Empty;
                        }
                    }

                    DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }


                    if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") || dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dBaslangicTarihiLocal.Date &&
                                model.dGirisTarihi.Date <= dBitisTarihiLocal.Date
                                ).ToList();
                        }
                        else if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dBaslangicTarihiLocal.Date
                                ).ToList();
                        }
                        else if (dBaslangicTarihiLocal == Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date <= dBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (listeleme != null && listeleme.Count > 0)
                    {
                        for (int i = 0; i < listeleme.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(listeleme[i].cResim))
                            {
                                listeleme[i].resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(listeleme[i].cResim);
                                if (listeleme[i].resimListesi.Count > 0 && !String.IsNullOrEmpty(listeleme[i].resimListesi[0].cKucukResim))
                                {
                                    listeleme[i].cResim = "/Files/th-" + listeleme[i].resimListesi[0].cKucukResim.Replace("th-", "");
                                }
                                else
                                {
                                    listeleme[i].cResim = "/Images/no-image.jpg";
                                }
                            }
                        }
                    }

                    int iSayfaNo = sayfaNo ?? 1;
                    int iListelemeSayisi = 60;
                    ViewBag.iSayfaNo = iSayfaNo;
                    ViewBag.iTutarKayitSayisi = listeleme.Count;
                    ViewBag.cToplamDuzeltme = listeleme.Sum(x => x.fDuzeltme);
                    ViewBag.cToplamAracSayisi = listeleme.Count;
                    ViewBag.cToplamTutar = listeleme.Sum(x => x.fGenelTutar);
                    ViewBag.iIlkKayit = ((((int)ViewBag.iSayfaNo - 1) * iListelemeSayisi) + 1);
                    ViewBag.iSonKayit = (((int)ViewBag.iSayfaNo * iListelemeSayisi));
                    if (ViewBag.iSonKayit > (int)ViewBag.iTutarKayitSayisi)
                    {
                        ViewBag.iSonKayit = (int)ViewBag.iTutarKayitSayisi;
                    }


                    return View(listeleme.ToPagedList(iSayfaNo, iListelemeSayisi));
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "Listele_Get", Ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult TumunuListele(int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi, int? iKodLokasyon, int? iAracDurumu)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 75))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
                ViewBag.iKodLokasyon = iKodLokasyon;
                ViewBag.iAracDurumu = iAracDurumu;

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     join tableLokasyons in dc.Lokasyons
                                        on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                     from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                     join tableKullanici2s in dc.Kullanici2s
                                        on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                     from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                     where
                                        table.iAktifMi == 1 &&
                                        (tableAracTipis != null && (tableAracTipis.iAktifMi == 0 || tableAracTipis.iAktifMi == 1)) &&
                                        (tableMusteri3s != null && (tableMusteri3s.iAktifMi == 0 || tableMusteri3s.iAktifMi == 1)) &&
                                        (tableLokasyons != null && (tableLokasyons.iAktifMi == 0 || tableLokasyons.iAktifMi == 1)) &&
                                        (tableKullanici2s != null && (tableKullanici2s.iAktifMi == 0 || tableKullanici2s.iAktifMi == 1))
                                     select new Models.Arac
                                     {
                                         iKodArac = table.iKodArac,
                                         cResim = (tableAracTipis != null && tableAracTipis.cFotograf != null && tableAracTipis.cFotograf.ToString() != string.Empty ? tableAracTipis.cFotograf : string.Empty),
                                         iKodMusteri3 = (int)table.iKodMusteri3,
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cMusteri3 =
                                            "Firma Adı : " + (tableMusteri3s != null && tableMusteri3s.cFirmaAdi != null && tableMusteri3s.cFirmaAdi != string.Empty ? tableMusteri3s.cFirmaAdi : "-") +
                                            "<br/>Ad Soyad : " + (tableMusteri3s != null && tableMusteri3s.cAdi != null && tableMusteri3s.cAdi != string.Empty && tableMusteri3s.cSoyadi != null && tableMusteri3s.cSoyadi != string.Empty ? tableMusteri3s.cAdi + " " + tableMusteri3s.cSoyadi : "-") +
                                            "<br/>Lokasyon : " + (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-") +
                                            "<br/>Kullanıcı : " + (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cLokasyon = (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-"),
                                         cFirmaAdi = (tableMusteri3s != null && tableMusteri3s.cFirmaAdi != null && tableMusteri3s.cFirmaAdi != string.Empty ? tableMusteri3s.cFirmaAdi : "-"),
                                         cAdSoyad = (tableMusteri3s != null && tableMusteri3s.cAdi != null && tableMusteri3s.cAdi != string.Empty && tableMusteri3s.cSoyadi != null && tableMusteri3s.cSoyadi != string.Empty ? tableMusteri3s.cAdi + " " + tableMusteri3s.cSoyadi : "-"),
                                         cKullanici = (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         dGirisTarihi = Convert.ToDateTime(table.dGirisTarihi),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", table.dGirisTarihi),
                                         dCikisTarihi = Convert.ToDateTime(table.dCikisTarihi),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", table.dCikisTarihi),
                                         cOtoparkUcreti = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                         fOtoparkUcreti = new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0),
                                         cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)).Replace(",", "."),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                         iKodLokasyon = table.iKodLokasyon != null ? (int)table.iKodLokasyon : 0,
                                         fGenelTutar = (table.fGenelTutar != null && table.fGenelTutar > 0 ? (float)table.fGenelTutar : 0),
                                         lAboneMi = new Models.Abonelik().AboneMi((int)table.iKodMusteri3)
                                     }).ToList();

                    if (!String.IsNullOrEmpty(arama))
                    {
                        List<string> cSplitArama = arama.Split(',').Select(p => p.ToLower().Trim()).ToList();
                        for (int i = 0; i < cSplitArama.Count; i++)
                        {
                            if (String.IsNullOrEmpty(cSplitArama[i]))
                            {
                                cSplitArama.Remove(cSplitArama[i]);
                                i--;
                            }
                        }
                        if (cSplitArama.Count > 0)
                        {
                            ViewBag.cSplitArama = cSplitArama;
                            ViewBag.cArama = String.Join(", ", cSplitArama.ToArray());

                            listeleme = listeleme.Where(
                                    model =>
                                    cSplitArama.Any(model.iKodArac.ToString().ToLower().Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cLokasyon).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cFirmaAdi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cAdSoyad).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cKullanici).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cPlaka).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cGirisTarihi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cCikisTarihi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cOtoparkUcreti).Contains)
                                    ).ToList();
                        }
                        else
                        {
                            ViewBag.cSplitArama = null;
                            ViewBag.cArama = string.Empty;
                        }
                    }

                    DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }

                    if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") ||
                        dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dBaslangicTarihiLocal.Date &&
                                model.dGirisTarihi.Date <= dBitisTarihiLocal.Date
                                ).ToList();
                        }
                        else if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dBaslangicTarihiLocal.Date
                                ).ToList();
                        }
                        else if (dBaslangicTarihiLocal == Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date <= dBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (iAracDurumu != null && iAracDurumu == 1)
                    {
                        ViewBag.cAracDurumu = new Models.AracDurumu().GonderAdi((int)iAracDurumu);
                        listeleme = listeleme.Where(model => model.dCikisTarihi.Date == Convert.ToDateTime("1900-01-01")).ToList();
                    }
                    else if (iAracDurumu != null && iAracDurumu == 2)
                    {
                        ViewBag.cAracDurumu = new Models.AracDurumu().GonderAdi((int)iAracDurumu);
                        listeleme = listeleme.Where(model => model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")).ToList();
                    }

                    if (iKodLokasyon != null && iKodLokasyon > 0)
                    {
                        ViewBag.cLokasyon = new Models.Lokasyon().GonderAdi((int)iKodLokasyon);
                        listeleme = listeleme.Where(model => model.iKodLokasyon == iKodLokasyon).ToList();
                    }

                    if (listeleme != null && listeleme.Count > 0)
                    {
                        for (int i = 0; i < listeleme.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(listeleme[i].cResim))
                            {
                                listeleme[i].resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(listeleme[i].cResim);
                                if (listeleme[i].resimListesi.Count > 0 && !String.IsNullOrEmpty(listeleme[i].resimListesi[0].cKucukResim))
                                {
                                    listeleme[i].cResim = "/Files/th-" + listeleme[i].resimListesi[0].cKucukResim.Replace("th-", "");
                                }
                                else
                                {
                                    listeleme[i].cResim = "/Images/no-image.jpg";
                                }
                            }
                        }
                    }

                    ViewBag.cSiralama = siralamaSekli;
                    ViewBag.Kolon1 = siralamaSekli == "kolon1" ? "kolon1_desc" : "kolon1";
                    ViewBag.Kolon2 = siralamaSekli == "kolon2" ? "kolon2_desc" : "kolon2";
                    ViewBag.Kolon3 = siralamaSekli == "kolon3" ? "kolon3_desc" : "kolon3";
                    ViewBag.Kolon4 = siralamaSekli == "kolon4" ? "kolon4_desc" : "kolon4";
                    ViewBag.Kolon5 = siralamaSekli == "kolon5" ? "kolon5_desc" : "kolon5";
                    ViewBag.Kolon6 = siralamaSekli == "kolon6" ? "kolon6_desc" : "kolon6";

                    int iSayfaNo = sayfaNo ?? 1;
                    int iListelemeSayisi = 60;
                    ViewBag.iSayfaNo = iSayfaNo;
                    ViewBag.iTutarKayitSayisi = listeleme.Count;

                    ViewBag.cCikisYapanToplamTutar = listeleme.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Sum(x => x.fGenelTutar);
                    ViewBag.cCikisYapanToplamDuzeltme = listeleme.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Sum(x => x.fDuzeltme);
                    ViewBag.cCikisYapanToplamAracSayisi = listeleme.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Count();

                    ViewBag.cBekleyenToplamDuzeltme = listeleme.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Sum(x => x.fDuzeltme);
                    ViewBag.cBekleyenToplamTutar = listeleme.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Sum(x => x.fGenelTutar);
                    ViewBag.cBekleyenToplamAracSayisi = listeleme.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Count();

                    ViewBag.cToplamTutar = listeleme.Sum(x => x.fGenelTutar);
                    ViewBag.cToplamDuzelme = listeleme.Sum(x => x.fDuzeltme);
                    ViewBag.cToplamAracSayisi = listeleme.Count();

                    ViewBag.iIlkKayit = ((((int)ViewBag.iSayfaNo - 1) * iListelemeSayisi) + 1);
                    ViewBag.iSonKayit = (((int)ViewBag.iSayfaNo * iListelemeSayisi));
                    if (ViewBag.iSonKayit > (int)ViewBag.iTutarKayitSayisi)
                    {
                        ViewBag.iSonKayit = (int)ViewBag.iTutarKayitSayisi;
                    }

                    return View(listeleme.ToPagedList(iSayfaNo, iListelemeSayisi));
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "Listele_Get", Ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult LokasyonListele(int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi, int? iAracDurumu)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 131))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
                ViewBag.iAracDurumu = iAracDurumu;

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     join tableLokasyons in dc.Lokasyons
                                        on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                     from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                     join tableKullanici2s in dc.Kullanici2s
                                        on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                     from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                     where
                                        table.iAktifMi == 1 &&
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        (tableAracTipis != null && (tableAracTipis.iAktifMi == 0 || tableAracTipis.iAktifMi == 1)) &&
                                        (tableMusteri3s != null && (tableMusteri3s.iAktifMi == 0 || tableMusteri3s.iAktifMi == 1)) &&
                                        (tableLokasyons != null && (tableLokasyons.iAktifMi == 0 || tableLokasyons.iAktifMi == 1)) &&
                                        (tableKullanici2s != null && (tableKullanici2s.iAktifMi == 0 || tableKullanici2s.iAktifMi == 1))
                                     select new Models.Arac
                                     {
                                         iKodArac = table.iKodArac,
                                         cResim = (tableAracTipis != null && tableAracTipis.cFotograf != null && tableAracTipis.cFotograf.ToString() != string.Empty ? tableAracTipis.cFotograf : string.Empty),
                                         iKodMusteri3 = (int)table.iKodMusteri3,
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cMusteri3 =
                                            "Firma Adı : " + (tableMusteri3s != null && tableMusteri3s.cFirmaAdi != null && tableMusteri3s.cFirmaAdi != string.Empty ? tableMusteri3s.cFirmaAdi : "-") +
                                            "<br/>Ad Soyad : " + (tableMusteri3s != null && tableMusteri3s.cAdi != null && tableMusteri3s.cAdi != string.Empty && tableMusteri3s.cSoyadi != null && tableMusteri3s.cSoyadi != string.Empty ? tableMusteri3s.cAdi + " " + tableMusteri3s.cSoyadi : "-") +
                                            "<br/>Lokasyon : " + (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-") +
                                            "<br/>Kullanıcı : " + (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cLokasyon = (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-"),
                                         cFirmaAdi = (tableMusteri3s != null && tableMusteri3s.cFirmaAdi != null && tableMusteri3s.cFirmaAdi != string.Empty ? tableMusteri3s.cFirmaAdi : "-"),
                                         cAdSoyad = (tableMusteri3s != null && tableMusteri3s.cAdi != null && tableMusteri3s.cAdi != string.Empty && tableMusteri3s.cSoyadi != null && tableMusteri3s.cSoyadi != string.Empty ? tableMusteri3s.cAdi + " " + tableMusteri3s.cSoyadi : "-"),
                                         cKullanici = (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         dGirisTarihi = Convert.ToDateTime(table.dGirisTarihi),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", table.dGirisTarihi),
                                         dCikisTarihi = Convert.ToDateTime(table.dCikisTarihi),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", table.dCikisTarihi),
                                         cOtoparkUcreti = string.Format("{0:N2}", new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0)).Replace(",", "."),
                                         fOtoparkUcreti = new Class.KDVHesapla().Gonder(new Models.Fiyat2().Fiyat2Hesapla(true, (int)table.iKodLokasyon, (int)table.iKodAracTipi, Convert.ToDateTime(table.dGirisTarihi), DateTime.Now, table.cUrun, table.fDuzeltme != null ? (float)table.fDuzeltme : 0, (tableMusteri3s != null && tableMusteri3s.iKodMusteri3 > 0 ? tableMusteri3s.iKodMusteri3 : 0)), table.iKDVTuru != null ? (int)table.iKDVTuru : 0, table.iKDVOrani != null ? (int)table.iKDVOrani : 0),
                                         cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)).Replace(",", "."),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                         iKodLokasyon = table.iKodLokasyon != null ? (int)table.iKodLokasyon : 0,
                                         fGenelTutar = (table.fGenelTutar != null && table.fGenelTutar > 0 ? (float)table.fGenelTutar : 0),
                                         lAboneMi = new Models.Abonelik().AboneMi((int)table.iKodMusteri3)
                                     }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    if (!String.IsNullOrEmpty(arama))
                    {
                        List<string> cSplitArama = arama.Split(',').Select(p => p.ToLower().Trim()).ToList();
                        for (int i = 0; i < cSplitArama.Count; i++)
                        {
                            if (String.IsNullOrEmpty(cSplitArama[i]))
                            {
                                cSplitArama.Remove(cSplitArama[i]);
                                i--;
                            }
                        }
                        if (cSplitArama.Count > 0)
                        {
                            ViewBag.cSplitArama = cSplitArama;
                            ViewBag.cArama = String.Join(", ", cSplitArama.ToArray());

                            listeleme = listeleme.Where(
                                    model =>
                                    cSplitArama.Any(model.iKodArac.ToString().ToLower().Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cLokasyon).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cFirmaAdi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cAdSoyad).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cKullanici).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cPlaka).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cGirisTarihi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cCikisTarihi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cOtoparkUcreti).Contains)
                                    ).ToList();
                        }
                        else
                        {
                            ViewBag.cSplitArama = null;
                            ViewBag.cArama = string.Empty;
                        }
                    }

                    DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }

                    if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") || dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dBaslangicTarihiLocal.Date &&
                                model.dGirisTarihi.Date <= dBitisTarihiLocal.Date
                                ).ToList();
                        }
                        else if (dBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dBaslangicTarihiLocal.Date
                                ).ToList();
                        }
                        else if (dBaslangicTarihiLocal == Convert.ToDateTime("1900-01-01") && dBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date <= dBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (iAracDurumu != null && iAracDurumu == 1)
                    {
                        ViewBag.cAracDurumu = new Models.AracDurumu().GonderAdi((int)iAracDurumu);
                        listeleme = listeleme.Where(model => model.dCikisTarihi.Date == Convert.ToDateTime("1900-01-01")).ToList();
                    }
                    else if (iAracDurumu != null && iAracDurumu == 2)
                    {
                        ViewBag.cAracDurumu = new Models.AracDurumu().GonderAdi((int)iAracDurumu);
                        listeleme = listeleme.Where(model => model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")).ToList();
                    }

                    if (listeleme != null && listeleme.Count > 0)
                    {
                        for (int i = 0; i < listeleme.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(listeleme[i].cResim))
                            {
                                listeleme[i].resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(listeleme[i].cResim);
                                if (listeleme[i].resimListesi.Count > 0 && !String.IsNullOrEmpty(listeleme[i].resimListesi[0].cKucukResim))
                                {
                                    listeleme[i].cResim = "/Files/th-" + listeleme[i].resimListesi[0].cKucukResim.Replace("th-", "");
                                }
                                else
                                {
                                    listeleme[i].cResim = "/Images/no-image.jpg";
                                }
                            }
                        }
                    }

                    ViewBag.cSiralama = siralamaSekli;
                    ViewBag.Kolon1 = siralamaSekli == "kolon1" ? "kolon1_desc" : "kolon1";
                    ViewBag.Kolon2 = siralamaSekli == "kolon2" ? "kolon2_desc" : "kolon2";
                    ViewBag.Kolon3 = siralamaSekli == "kolon3" ? "kolon3_desc" : "kolon3";
                    ViewBag.Kolon4 = siralamaSekli == "kolon4" ? "kolon4_desc" : "kolon4";
                    ViewBag.Kolon5 = siralamaSekli == "kolon5" ? "kolon5_desc" : "kolon5";
                    ViewBag.Kolon6 = siralamaSekli == "kolon6" ? "kolon6_desc" : "kolon6";

                    int iSayfaNo = sayfaNo ?? 1;
                    int iListelemeSayisi = 60;
                    ViewBag.iSayfaNo = iSayfaNo;
                    ViewBag.iTutarKayitSayisi = listeleme.Count;

                    ViewBag.cCikisYapanToplamTutar = listeleme.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Sum(x => x.fGenelTutar);
                    ViewBag.cCikisYapanToplamDuzeltme = listeleme.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Sum(x => x.fDuzeltme);
                    ViewBag.cCikisYapanToplamAracSayisi = listeleme.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Count();

                    ViewBag.cBekleyenToplamDuzeltme = listeleme.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Sum(x => x.fDuzeltme);
                    ViewBag.cBekleyenToplamTutar = listeleme.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Sum(x => x.fGenelTutar);
                    ViewBag.cBekleyenToplamAracSayisi = listeleme.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Count();

                    ViewBag.cToplamTutar = listeleme.Sum(x => x.fGenelTutar);
                    ViewBag.cToplamDuzelme = listeleme.Sum(x => x.fDuzeltme);
                    ViewBag.cToplamAracSayisi = listeleme.Count();

                    ViewBag.iIlkKayit = ((((int)ViewBag.iSayfaNo - 1) * iListelemeSayisi) + 1);
                    ViewBag.iSonKayit = (((int)ViewBag.iSayfaNo * iListelemeSayisi));
                    if (ViewBag.iSonKayit > (int)ViewBag.iTutarKayitSayisi)
                    {
                        ViewBag.iSonKayit = (int)ViewBag.iTutarKayitSayisi;
                    }

                    return View(listeleme.ToPagedList(iSayfaNo, iListelemeSayisi));
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "Listele_Get", Ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult BekleyenAra(int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 48))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Arac", "Ara_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult TumunuAra(int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi, int? iKodLokasyon, int? iAracDurumu)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 75))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
                ViewBag.iKodLokasyon = iKodLokasyon;
                ViewBag.iAracDurumu = iAracDurumu;

                ViewBag.LokasyonListesi = new Models.Lokasyon().Gonder();
                ViewBag.AracDurumuListesi = new Models.AracDurumu().Gonder();
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Arac", "Ara_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult LokasyonAra(int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi, int? iAracDurumu)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 131))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;
                ViewBag.iAracDurumu = iAracDurumu;

                ViewBag.AracDurumuListesi = new Models.AracDurumu().Gonder();
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Arac", "Ara_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult BekleyenSil(string id, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 48))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                if (!String.IsNullOrEmpty(id))
                {
                    int iKodArac = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var silme = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           table.iAktifMi == 1
                                         select table).FirstOrDefault();

                            if (silme != null && silme.dGirisTarihi >= DateTime.Now.AddMinutes(-3))
                            {
                                silme.iAktifMi = -1;
                                silme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                silme.dTarih = DateTime.Now;
                                dc.SubmitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Arac", "Sil_Get", Ex.Message);
            }

            return Redirect("/Arac/BekleyenListele?sayfaNo=" + sayfaNo + "&siralamaSekli=" + siralamaSekli + "&arama=" + arama + "&dBaslangicTarihi=" + dBaslangicTarihi + "&dBitisTarihi=" + dBitisTarihi);
        }

        public void BarkodYazdir(int iKodArac, string cPlaka, int iKodAracTipi, DateTime dGirisTarihi)
        {
            cPlakaLocal = cPlaka;
            cAracTipiLocal = new Models.AracTipi().GonderAdi(iKodAracTipi);
            cGirisTarihiLocal = dGirisTarihi.ToString("dd MMMM yyyy, dddd, HH:mm");

            var doc = new PrintDocument();
            doc.PrinterSettings.PrinterName = "Hoin-58-Series";
            doc.PrintPage += new PrintPageEventHandler(ProvideContent);
            doc.Print();
        }

        public ActionResult YenidenBarkodYazdir(int iKodArac, string cPlaka, int iKodAracTipi, DateTime dGirisTarihi)
        {
            cPlakaLocal = cPlaka;
            cAracTipiLocal = new Models.AracTipi().GonderAdi(iKodAracTipi);
            cGirisTarihiLocal = dGirisTarihi.ToString("dd MMMM yyyy, dddd, HH:mm");

            var doc = new PrintDocument();
            doc.PrinterSettings.PrinterName = "Hoin-58-Series";
            doc.PrintPage += new PrintPageEventHandler(ProvideContent);
            doc.Print();

            return Redirect("/Arac/BekleyenListele");
        }

        string cBarkodLocal = string.Empty;
        string cAracTipiLocal = string.Empty;
        string cGirisTarihiLocal = string.Empty;
        string cPlakaLocal = string.Empty;
        public void ProvideContent(object sender, PrintPageEventArgs e)
        {
            generateBarcode(cPlakaLocal);
            Image barcodeImage = Image.FromFile(string.Format("{0}{1}.jpg", Server.MapPath("~/Files/"), "barcode"));

            e.Graphics.DrawString("KOÇYİĞİT", new Font("Arial", 23, FontStyle.Bold), Brushes.Black, 0, 5);
            e.Graphics.DrawString("MEŞRUTİYET GARAJI", new Font("Arial", 11, FontStyle.Bold), Brushes.Black, 3, 43);
            e.Graphics.DrawString("OTOPARK YIKAMA SERVİSİ", new Font("Arial", 9, FontStyle.Bold), Brushes.Black, 3, 60);
            e.Graphics.DrawString("Adres: Meşrutiyet Cad. No: 18/A", new Font("Arial", 8), Brushes.Black, 3, 75);
            e.Graphics.DrawString("Kızılay / Ankara Telefon: 0 (312)", new Font("Arial", 8), Brushes.Black, 3, 88);
            e.Graphics.DrawString("419 45 51 Faks: 0 (312) 418 41 31", new Font("Arial", 8), Brushes.Black, 3, 101);
            e.Graphics.DrawString("Web Adresi: www.kocyigitvale.com", new Font("Arial", 8), Brushes.Black, 3, 114);
            e.Graphics.DrawImage(barcodeImage, 5, 133);
            e.Graphics.DrawString(cPlakaLocal.ToUpper(), new Font("Arial", 18, FontStyle.Bold), Brushes.Black, 0, 200);
            e.Graphics.DrawString(cAracTipiLocal.ToUpper(), new Font("Arial", 11), Brushes.Black, 3, 226);
            e.Graphics.DrawString(cGirisTarihiLocal, new Font("Arial", 8), Brushes.Black, 3, 243);
            e.Graphics.DrawString("UYARILAR", new Font("Arial", 11, FontStyle.Bold), Brushes.Black, 3, 256);
            e.Graphics.DrawString("1- Yıkamaya bırakılan araçlardan 1", new Font("Arial", 8), Brushes.Black, 3, 276);
            e.Graphics.DrawString("(bir) saatten sonra park ücreti alınır.", new Font("Arial", 8), Brushes.Black, 3, 289);
            e.Graphics.DrawString("2- Müdüriyete teslim edilmeyen", new Font("Arial", 8), Brushes.Black, 3, 302);
            e.Graphics.DrawString("kayıp olan eşyalardan mesul değiliz.", new Font("Arial", 8), Brushes.Black, 3, 315);
            e.Graphics.DrawString("3- Aracınızı teslim almaya gelirken", new Font("Arial", 8), Brushes.Black, 3, 328);
            e.Graphics.DrawString("fişi yetkiliye veriniz. Aksi taktirde", new Font("Arial", 8), Brushes.Black, 3, 341);
            e.Graphics.DrawString("ruhsat sahibine teslim edilir.", new Font("Arial", 8), Brushes.Black, 3, 354);
            e.Graphics.DrawString("4- Açılış saati 08:00 kapanış saati", new Font("Arial", 8), Brushes.Black, 3, 367);
            e.Graphics.DrawString("23:00'dır.", new Font("Arial", 8), Brushes.Black, 3, 379);
            e.Graphics.DrawString("-------", new Font("Arial", 8), Brushes.Black, 3, 395);

            barcodeImage.Dispose();
        }

        public void generateBarcode(string id)
        {
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            Image image = barcode.Draw("*" + id + "*", 60);
            image.Save(Server.MapPath("~/Files/") + "barcode.jpg");
        }

        #endregion

        [HttpGet]
        public ActionResult CikisYapanAracListeleYeni(
            int? sayfaNo,
            string cPlaka,
            int? iKodAracTipi,
            string dGirisTarihi,
            string dCikisTarihi,
            int? iAbonelikDurumu,
            string dAboneBaslangicTarihi,
            string dAboneBitisTarihi,
            int? iDuzeltmeTipi,
            int? iKodUrun2)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 134))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cPlaka = cPlaka;
                ViewBag.dGirisTarihi = dGirisTarihi;
                ViewBag.dCikisTarihi = dCikisTarihi;
                ViewBag.iKodAracTipi = iKodAracTipi;
                ViewBag.iKodUrun2 = iKodUrun2;
                ViewBag.iAbonelikDurumu = iAbonelikDurumu;
                ViewBag.iDuzeltmeTipi = iDuzeltmeTipi;
                ViewBag.dAboneBaslangicTarihi = dAboneBaslangicTarihi;
                ViewBag.dAboneBitisTarihi = dAboneBitisTarihi;

                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.dCikisTarihi != Convert.ToDateTime("1900-01-01") &&
                                        table.iAktifMi == 1 &&
                                        table.iKodAracTipi != 4 && // Misafir Dilse
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                     select new Models.AracYeni
                                     {
                                         iSatirNumarasi = 0,
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                         iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                         cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                         cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                         cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                         cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                         lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                         iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                         cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                         iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                         dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                         dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))
                                     }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeIptal = (from table in dc.Aracs
                                          join tableAracTipis in dc.AracTipis
                                             on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                          from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                          join tableMusteri3s in dc.Musteri3s
                                             on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                          from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                          where
                                             table.iKodLokasyon == iKodLokasyonLogin &&
                                             table.iAktifMi == -1 &&
                                             table.iKodAracTipi != 4 && // Misafir Dilse
                                             (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                             (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                          select new Models.AracYeni
                                          {
                                              cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                              cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                              cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                              iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                              cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                              cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                              iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                              cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                              cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                              iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                              cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                              iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                              dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                              fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                              fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                              dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                          }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeBekleyen = (from table in dc.Aracs
                                             join tableAracTipis in dc.AracTipis
                                                on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                             from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                             join tableMusteri3s in dc.Musteri3s
                                                on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                             from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                             where
                                                table.iKodLokasyon == iKodLokasyonLogin &&
                                                table.iAktifMi == 1 &&
                                                table.iKodAracTipi != 4 && // Misafir Dilse
                                                (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                             select new Models.AracYeni
                                             {
                                                 cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                 cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                 cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                 iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                 cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                 cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                 iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                 cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                 cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                 iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                 cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                 iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                 dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                 fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                 fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                 dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                             }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeMisafirArac = (from table in dc.Aracs
                                                join tableAracTipis in dc.AracTipis
                                                   on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                                from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                                join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                                from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                                where
                                                   table.iKodLokasyon == iKodLokasyonLogin &&
                                                   table.iAktifMi == 1 &&
                                                   table.iKodAracTipi == 4 && // Misafir Araç
                                                   (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                   (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                                select new Models.AracYeni
                                                {
                                                    cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                    cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                    cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                    iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                    cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                    cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                    iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                    cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                    cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                    iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                    cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                    iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                    dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                    fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                    fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                    dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var abonelistesi = (from table in dc.Aboneliks
                                        join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                        from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                        where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.iAktifMi == 1
                                        select new Models.Abonelik
                                        {
                                            dKayitTarihi = (table.dKayitTarihi != null ? Convert.ToDateTime(table.dKayitTarihi) : Convert.ToDateTime("1900-01-01")),
                                            cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                            cOdemeString = (table.cOdeme != null && table.cOdeme.ToString() != string.Empty ? table.cOdeme : string.Empty)
                                        }).ToList();

                    var abonelistesi2 = abonelistesi;
                    double dAbonelikUcretiOdeyenlerTutari = 0;
                    int iAbonelikUcretiOdeyenler = 0;

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        listeleme = listeleme.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.cPlaka == cPlaka).ToList();
                    }

                    DateTime dGirisTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dCikisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dGirisTarihi))
                    {
                        dGirisTarihiLocal = Convert.ToDateTime(dGirisTarihi);
                    }
                    if (!String.IsNullOrEmpty(dCikisTarihi))
                    {
                        dCikisTarihiLocal = Convert.ToDateTime(dCikisTarihi);
                    }

                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") || dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }


                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();


                            abonelistesi = abonelistesi.Where(
                                model =>
                                model.dKayitTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();
                        }

                        if (dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                               model =>
                               model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                               ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                               model =>
                               model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                               ).ToList();

                            abonelistesi = abonelistesi.Where(
                               model =>
                               model.dKayitTarihi.Date <= dCikisTarihiLocal.Date
                               ).ToList();
                        }

                        for (int i = 0; i < abonelistesi2.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(abonelistesi2[i].cOdemeString))
                            {
                                abonelistesi2[i].aboneOdemeTakvimis = JsonConvert.DeserializeObject<List<Models.AboneOdemeTakvimi>>(abonelistesi2[i].cOdemeString);

                                for (int j = 0; j < abonelistesi2[i].aboneOdemeTakvimis.Count; j++)
                                {
                                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                                    {
                                        if (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date)
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                    else if (dGirisTarihiLocal == Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                    {
                                        if (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date)
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                    else if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                    {
                                        if ((Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date) && (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date))
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    DateTime dAboneBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dAboneBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dAboneBaslangicTarihi))
                    {
                        dAboneBaslangicTarihiLocal = Convert.ToDateTime(dAboneBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dAboneBitisTarihi))
                    {
                        dAboneBitisTarihiLocal = Convert.ToDateTime(dAboneBitisTarihi);
                    }

                    if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") || dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();
                        }

                        if (dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (iKodAracTipi != null && iKodAracTipi >= 0)
                    {
                        listeleme = listeleme.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                    }

                    if (iAbonelikDurumu != null)
                    {
                        listeleme = listeleme.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                    }

                    if (iDuzeltmeTipi != null)
                    {
                        if (iDuzeltmeTipi == 3)
                        {
                            listeleme = listeleme.Where(model => model.iDuzeltmeTipi > 0).ToList();
                            listelemeIptal = listelemeIptal.Where(model => model.iDuzeltmeTipi > 0).ToList();
                            listelemeBekleyen = listelemeBekleyen.Where(model => model.iDuzeltmeTipi > 0).ToList();
                            listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iDuzeltmeTipi > 0).ToList();
                        }
                        else
                        {
                            listeleme = listeleme.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                            listelemeIptal = listelemeIptal.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                            listelemeBekleyen = listelemeBekleyen.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                            listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        }
                    }

                    int iSayi = 0;
                    List<Models.UrunJson2> toplamUrunler = new List<Models.UrunJson2>();
                    List<Models.AracYeni> listelemeYeni = new List<Models.AracYeni>();
                    for (int i = 0; i < listeleme.Count; i++)
                    {
                        iSayi++;
                        listeleme[i].iSatirNumarasi = iSayi;

                        if (!String.IsNullOrEmpty(listeleme[i].cUrun))
                        {
                            listeleme[i].urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(listeleme[i].cUrun);
                            for (int j = 0; j < listeleme[i].urun2Listesi.Count; j++)
                            {
                                if (iKodUrun2 != null && iKodUrun2 > 0)
                                {
                                    if (iKodUrun2 == listeleme[i].urun2Listesi[j].iKodUrun2)
                                    {
                                        listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                        listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                        toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                        break;
                                    }
                                }
                                else
                                {
                                    listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                    listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                    toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                }
                            }

                            listelemeYeni.Add(listeleme[i]);
                        }
                        else if (iKodUrun2 == null || iKodUrun2 == 0)
                        {
                            listelemeYeni.Add(listeleme[i]);
                        }
                    }
                    listeleme = listelemeYeni;

                    List<Models.Arac.AracYeniToplaUrun> toplamUrunListesi =
                        toplamUrunler.GroupBy(l => l.iKodUrun2)
                            .Select(cl => new Models.Arac.AracYeniToplaUrun
                            {
                                iKodUrun2 = cl.First().iKodUrun2,
                                iToplam = cl.Sum(c => Convert.ToInt32(c.iAdet)),
                                dToplamFiyat = cl.Sum(c => Convert.ToDouble(c.cFiyat.ToString().Replace(".", ""))),
                            }).ToList();

                    ViewBag.ToplamUrunler = toplamUrunListesi;

                    double dToplamUrunler = 0;
                    if (toplamUrunListesi != null)
                    {
                        for (int i = 0; i < toplamUrunListesi.Count; i++)
                        {
                            dToplamUrunler += toplamUrunListesi[i].dToplamFiyat;
                        }
                    }
                    ViewBag.cToplamUrunler = dToplamUrunler;




                    int iListelemeSayisi = 50;
                    int iSayfaNo = sayfaNo ?? 1;
                    ViewBag.iSayfaNo = iSayfaNo;
                    ViewBag.iKayitSayisi = listeleme.Count;
                    ViewBag.iIlkKayit = ((((int)ViewBag.iSayfaNo - 1) * iListelemeSayisi) + 1);
                    ViewBag.iSonKayit = (((int)ViewBag.iSayfaNo * iListelemeSayisi));
                    if (ViewBag.iSonKayit > (int)ViewBag.iKayitSayisi)
                    {
                        ViewBag.iSonKayit = (int)ViewBag.iKayitSayisi;
                    }

                    ViewBag.cToplamAracSayisi = listeleme.Count();
                    ViewBag.cOtoparkToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti);
                    ViewBag.cDuzeltmeEksiltmeTutar = listeleme.Where(x => x.iDuzeltmeTipi == 1).Sum(x => x.fDuzeltme);
                    ViewBag.cDuzeltmeArtirmaTutar = listeleme.Where(x => x.iDuzeltmeTipi == 2).Sum(x => x.fDuzeltme);
                    ViewBag.cOtoparkDuzeltmeliToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti + (x.iDuzeltmeTipi == 1 ? -1 * x.fDuzeltme : 0) + (x.iDuzeltmeTipi == 2 ? x.fDuzeltme : 0));
                    ViewBag.cGenelToplam = (((float)ViewBag.cOtoparkToplamTutar) + ((float)ViewBag.cDuzeltmeArtirmaTutar) + ((float)ViewBag.cToplamUrunler)) - ((float)ViewBag.cDuzeltmeEksiltmeTutar);
                    ViewBag.cVeresiyeToplamTutar = listeleme.Sum(x => x.fVeresiye);

                    List<Models.AracYeni.AracYazdir.UrunListesi> urunListesis = new List<Models.AracYeni.AracYazdir.UrunListesi>();

                    List<Models.Urun2> urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                    for (int i = 0; i < urun2Listesi.Count; i++)
                    {
                        int iToplam = 0;
                        double dToplamFiyat = 0;

                        for (int j = 0; j < toplamUrunListesi.Count; j++)
                        {
                            if (urun2Listesi[i].iKodUrun2 == toplamUrunListesi[j].iKodUrun2)
                            {
                                iToplam = toplamUrunListesi[j].iToplam;
                                dToplamFiyat = toplamUrunListesi[j].dToplamFiyat;
                                break;
                            }
                        }

                        urunListesis.Add(new Models.AracYeni.AracYazdir.UrunListesi
                        {
                            cUrun = urun2Listesi[i].cAdi,
                            iToplam = iToplam,
                            dToplamFiyat = dToplamFiyat,
                            lStokTutlacakMi = urun2Listesi[i].lStokTutlacakMi
                        });
                    }

                    ViewBag.cToplamYikamaSayisi = urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.iToplam);
                    ViewBag.cToplamYikamaUcreti = String.Format("{0:N2}", urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.dToplamFiyat));
                    ViewBag.cToplamUrunSayisi = urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.iToplam);
                    ViewBag.cToplamUrunUcreti = String.Format("{0:N2}", urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.dToplamFiyat));
                    ViewBag.cToplamVeresiyeUcreti = String.Format("{0:N2}", listeleme.Where(x => x.lVeresiye == true).Sum(x => x.fVeresiye));

                    ViewBag.cToplamBekleyenAracSayisi = listelemeBekleyen.Count();

                    ViewBag.cAboneCikisYapanAracSayisi = listeleme.Where(x => x.iAboneMi == 1).Count();
                    ViewBag.cAboneBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iAboneMi == 1).Count();
                    ViewBag.cAboneAracSayisi = ViewBag.cAboneCikisYapanAracSayisi + ViewBag.cAboneBekleyenAracSayisi;

                    ViewBag.cDuzeltemeCikisYapanAracSayisi = listeleme.Where(x => x.iDuzeltmeTipi > 0).Count();
                    ViewBag.cDuzeltemeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Count();
                    ViewBag.cDuzeltemeAracSayisi = ViewBag.cDuzeltemeCikisYapanAracSayisi + ViewBag.cDuzeltemeBekleyenAracSayisi;

                    ViewBag.cMisafirCikisYapanAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Count();
                    ViewBag.cMisafirBekleyenAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Count();
                    ViewBag.cMisafirAracSayisi = ViewBag.cMisafirCikisYapanAracSayisi + ViewBag.cMisafirBekleyenAracSayisi;
                    ViewBag.cVeresiyeAracSayisi = listeleme.Where(x => x.lVeresiye == true).Count();
                    ViewBag.cIptalEdilenAracSayisi = listelemeIptal.Count();
                    ViewBag.fAbonelikUcreti = (float)dAbonelikUcretiOdeyenlerTutari;
                    ViewBag.iAboneArac = iAbonelikUcretiOdeyenler;
                    ViewBag.iYeniAboneArac = abonelistesi.Count;

                    return View(listeleme.ToPagedList(iSayfaNo, iListelemeSayisi));
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                return View();
            }
        }

        [HttpGet]
        public ActionResult CikisYapanAracAraYeni(
            int? sayfaNo,
            string cPlaka,
            int? iKodAracTipi,
            string dGirisTarihi,
            string dCikisTarihi,
            int? iAbonelikDurumu,
            string dAboneBaslangicTarihi,
            string dAboneBitisTarihi,
            int? iDuzeltmeTipi,
            int? iKodUrun2)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 134))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cPlaka = cPlaka;
                ViewBag.dGirisTarihi = dGirisTarihi;
                ViewBag.dCikisTarihi = dCikisTarihi;
                ViewBag.iKodAracTipi = iKodAracTipi;
                ViewBag.iKodUrun2 = iKodUrun2;
                ViewBag.iAbonelikDurumu = iAbonelikDurumu;
                ViewBag.iDuzeltmeTipi = iDuzeltmeTipi;
                ViewBag.dAboneBaslangicTarihi = dAboneBaslangicTarihi;
                ViewBag.dAboneBitisTarihi = dAboneBitisTarihi;

                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.AboneDurumuListesi = new Models.AbonelikDurumu().GonderAra();
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().GonderAra(); ;
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);
                return View();

            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                return View();
            }
        }

        public void CikisYapanAracYazdirYeni(
            int? sayfaNo,
            string cPlaka,
            int? iKodAracTipi,
            string dGirisTarihi,
            string dCikisTarihi,
            int? iAbonelikDurumu,
            string dAboneBaslangicTarihi,
            string dAboneBitisTarihi,
            int? iDuzeltmeTipi,
            int? iKodUrun2)
        {
            try
            {
                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.dCikisTarihi != Convert.ToDateTime("1900-01-01") &&
                                        table.iAktifMi == 1 &&
                                        table.iKodAracTipi != 4 && // Misafir Dilse
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                     select new Models.AracYeni
                                     {
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                         iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                         cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                         cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                         cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                         cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                         iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                         cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                         iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                         dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                         lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                         dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))
                                     }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeIptal = (from table in dc.Aracs
                                          join tableAracTipis in dc.AracTipis
                                             on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                          from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                          join tableMusteri3s in dc.Musteri3s
                                             on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                          from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                          where
                                             table.iKodLokasyon == iKodLokasyonLogin &&
                                             table.iAktifMi == -1 &&
                                             table.iKodAracTipi != 4 && // Misafir Dilse
                                             (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                             (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                          select new Models.AracYeni
                                          {
                                              cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                              cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                              cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                              iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                              cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                              cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                              iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                              cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                              cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                              iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                              cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                              iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                              dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                              fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                              fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                              lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                              dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                          }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeBekleyen = (from table in dc.Aracs
                                             join tableAracTipis in dc.AracTipis
                                                on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                             from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                             join tableMusteri3s in dc.Musteri3s
                                                on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                             from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                             where
                                                table.iKodLokasyon == iKodLokasyonLogin &&
                                                table.iAktifMi == 1 &&
                                                table.iKodAracTipi != 4 && // Misafir Dilse
                                                (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                             select new Models.AracYeni
                                             {
                                                 cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                 cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                 cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                 iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                 cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                 cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                 iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                 cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                 cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                 iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                 cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                 iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                 dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                 fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                 fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                 lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                                 dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                             }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeMisafirArac = (from table in dc.Aracs
                                                join tableAracTipis in dc.AracTipis
                                                   on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                                from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                                join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                                from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                                where
                                                   table.iKodLokasyon == iKodLokasyonLogin &&
                                                   table.iAktifMi == 1 &&
                                                   table.iKodAracTipi == 4 && // Misafir Araç
                                                   (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                   (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                                select new Models.AracYeni
                                                {
                                                    cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                    cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                    cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                    iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                    cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                    cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                    iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                    cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                    cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                    iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                    cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                    iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                    dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                    fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                    fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                    lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                                    dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                }).OrderByDescending(x => x.dCikisTarihi).ToList();



                    var abonelistesi = (from table in dc.Aboneliks
                                        join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                        from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                        where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.iAktifMi == 1
                                        select new Models.Abonelik
                                        {
                                            dKayitTarihi = (table.dKayitTarihi != null ? Convert.ToDateTime(table.dKayitTarihi) : Convert.ToDateTime("1900-01-01")),
                                            cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                            cOdemeString = (table.cOdeme != null && table.cOdeme.ToString() != string.Empty ? table.cOdeme : string.Empty)
                                        }).ToList();

                    var abonelistesi2 = abonelistesi;
                    double dAbonelikUcretiOdeyenlerTutari = 0;
                    int iAbonelikUcretiOdeyenler = 0;

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        listeleme = listeleme.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.cPlaka == cPlaka).ToList();
                    }

                    DateTime dGirisTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dCikisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dGirisTarihi))
                    {
                        dGirisTarihiLocal = Convert.ToDateTime(dGirisTarihi);
                    }
                    if (!String.IsNullOrEmpty(dCikisTarihi))
                    {
                        dCikisTarihiLocal = Convert.ToDateTime(dCikisTarihi);
                    }

                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") || dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            abonelistesi = abonelistesi.Where(
                                model =>
                                model.dKayitTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();
                        }

                        if (dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            abonelistesi = abonelistesi.Where(
                                model =>
                                model.dKayitTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();
                        }

                        for (int i = 0; i < abonelistesi2.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(abonelistesi2[i].cOdemeString))
                            {
                                abonelistesi2[i].aboneOdemeTakvimis = JsonConvert.DeserializeObject<List<Models.AboneOdemeTakvimi>>(abonelistesi2[i].cOdemeString);

                                for (int j = 0; j < abonelistesi2[i].aboneOdemeTakvimis.Count; j++)
                                {
                                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                                    {
                                        if (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date)
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                    else if (dGirisTarihiLocal == Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                    {
                                        if (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date)
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                    else if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                    {
                                        if ((Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date) && (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date))
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    DateTime dAboneBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dAboneBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dAboneBaslangicTarihi))
                    {
                        dAboneBaslangicTarihiLocal = Convert.ToDateTime(dAboneBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dAboneBitisTarihi))
                    {
                        dAboneBitisTarihiLocal = Convert.ToDateTime(dAboneBitisTarihi);
                    }

                    if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") || dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();
                        }

                        if (dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (iKodAracTipi != null && iKodAracTipi >= 0)
                    {
                        listeleme = listeleme.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                    }

                    if (iAbonelikDurumu != null)
                    {
                        listeleme = listeleme.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                    }

                    if (iDuzeltmeTipi != null)
                    {
                        listeleme = listeleme.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                    }

                    List<Models.UrunJson2> toplamUrunler = new List<Models.UrunJson2>();
                    List<Models.AracYeni> listelemeYeni = new List<Models.AracYeni>();
                    for (int i = 0; i < listeleme.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(listeleme[i].cUrun))
                        {
                            listeleme[i].urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(listeleme[i].cUrun);
                            for (int j = 0; j < listeleme[i].urun2Listesi.Count; j++)
                            {
                                if (iKodUrun2 != null && iKodUrun2 > 0)
                                {
                                    if (iKodUrun2 == listeleme[i].urun2Listesi[j].iKodUrun2)
                                    {
                                        listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                        listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                        toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                        break;
                                    }
                                }
                                else
                                {
                                    listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                    listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                    toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                }
                            }

                            listelemeYeni.Add(listeleme[i]);
                        }
                        else if (iKodUrun2 == null || iKodUrun2 == 0)
                        {
                            listelemeYeni.Add(listeleme[i]);
                        }
                    }
                    listeleme = listelemeYeni;

                    List<Models.Arac.AracYeniToplaUrun> toplamUrunListesi =
                        toplamUrunler.GroupBy(l => l.iKodUrun2)
                            .Select(cl => new Models.Arac.AracYeniToplaUrun
                            {
                                iKodUrun2 = cl.First().iKodUrun2,
                                iToplam = cl.Sum(c => Convert.ToInt32(c.iAdet)),
                                dToplamFiyat = cl.Sum(c => Convert.ToDouble(c.cFiyat.ToString().Replace(".", ""))),
                            }).ToList();

                    double dToplamUrunler = 0;
                    if (toplamUrunListesi != null)
                    {
                        for (int i = 0; i < toplamUrunListesi.Count; i++)
                        {
                            dToplamUrunler += toplamUrunListesi[i].dToplamFiyat;
                        }
                    }

                    Models.AracYeni.AracYazdir aracYazdirs = new Models.AracYeni.AracYazdir();
                    aracYazdirs.iToplamAracSayisi = listeleme.Count();
                    aracYazdirs.dUrunFiyatToplam = dToplamUrunler;
                    aracYazdirs.fOtoparkToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti);
                    aracYazdirs.fDuzeltmeEksiltmeTutar = listeleme.Where(x => x.iDuzeltmeTipi == 1).Sum(x => x.fDuzeltme);
                    aracYazdirs.fDuzeltmeArtirmaTutar = listeleme.Where(x => x.iDuzeltmeTipi == 2).Sum(x => x.fDuzeltme);
                    aracYazdirs.fOtoparkDuzeltmeliToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti + (x.iDuzeltmeTipi == 1 ? -1 * x.fDuzeltme : 0) + (x.iDuzeltmeTipi == 2 ? x.fDuzeltme : 0));
                    aracYazdirs.fVeresiyeToplamTutar = listeleme.Sum(x => x.fVeresiye);
                    aracYazdirs.fGenelToplam = (aracYazdirs.fOtoparkToplamTutar + aracYazdirs.fDuzeltmeArtirmaTutar + (float)aracYazdirs.dUrunFiyatToplam) - aracYazdirs.fDuzeltmeEksiltmeTutar;
                    aracYazdirs.fAbonelikUcreti = (float)dAbonelikUcretiOdeyenlerTutari;
                    aracYazdirs.iAboneArac = iAbonelikUcretiOdeyenler;
                    aracYazdirs.iYeniAboneArac = abonelistesi.Count;
                    aracYazdirs.urunListesis = new List<Models.AracYeni.AracYazdir.UrunListesi>();

                    List<Models.Urun2> urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                    for (int i = 0; i < urun2Listesi.Count; i++)
                    {
                        int iToplam = 0;
                        double dToplamFiyat = 0;

                        for (int j = 0; j < toplamUrunListesi.Count; j++)
                        {
                            if (urun2Listesi[i].iKodUrun2 == toplamUrunListesi[j].iKodUrun2)
                            {
                                iToplam = toplamUrunListesi[j].iToplam;
                                dToplamFiyat = toplamUrunListesi[j].dToplamFiyat;
                                break;
                            }
                        }

                        aracYazdirs.urunListesis.Add(new Models.AracYeni.AracYazdir.UrunListesi
                        {
                            cUrun = urun2Listesi[i].cAdi,
                            iToplam = iToplam,
                            dToplamFiyat = dToplamFiyat,
                            lStokTutlacakMi = urun2Listesi[i].lStokTutlacakMi
                        });
                    }

                    aracYazdirs.iToplamBekleyenAracSayisi = listelemeBekleyen.Count();

                    aracYazdirs.iAboneCikisYapanAracSayisi = listeleme.Where(x => x.iAboneMi == 1).Count();
                    aracYazdirs.AboneCikisYapanAraclar = listeleme.Where(x => x.iAboneMi == 1).Select(x => x.cPlaka).ToArray();
                    aracYazdirs.iAboneBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iAboneMi == 1).Count();
                    aracYazdirs.AboneBekleyenAraclar = listelemeBekleyen.Where(x => x.iAboneMi == 1).Select(x => x.cPlaka).ToArray();
                    aracYazdirs.iAboneAracSayisi = aracYazdirs.iAboneCikisYapanAracSayisi + aracYazdirs.iAboneBekleyenAracSayisi;

                    aracYazdirs.iDuzeltmeCikisYapanAracSayisi = listeleme.Where(x => x.iDuzeltmeTipi > 0).Count();
                    aracYazdirs.DuzeltmeCikisYapanAraclar = listeleme.Where(x => x.iDuzeltmeTipi > 0).Select(x => x.cPlaka.ToUpper() + (x.iDuzeltmeTipi == 1 ? " / -" : " / +") + string.Format("{0:N2}", x.fDuzeltme) + " TL\n" + x.cAciklama).ToArray();
                    aracYazdirs.iDuzeltmeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Count();
                    aracYazdirs.DuzeltmeBekleyenAraclar = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Select(x => x.cPlaka.ToUpper() + (x.iDuzeltmeTipi == 1 ? " / -" : " / +") + string.Format("{0:N2}", x.fDuzeltme) + " TL\n" + x.cAciklama).ToArray();
                    aracYazdirs.iDuzeltmeAracSayisi = aracYazdirs.iDuzeltmeCikisYapanAracSayisi + aracYazdirs.iDuzeltmeBekleyenAracSayisi;

                    aracYazdirs.iVeresiyeCikisYapanAracSayisi = listeleme.Where(x => x.lVeresiye == true).Count();
                    aracYazdirs.iVeresiyeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.lVeresiye == true).Count();
                    aracYazdirs.VeresiyeBekleyenAraclar = listelemeBekleyen.Where(x => x.lVeresiye == true).Select(x => x.cPlaka.ToUpper() + (x.lVeresiye == true ? " / -" : " / +") + string.Format("{0:N2}", x.fVeresiye) + " TL\n" + x.cAciklama).ToArray();
                    aracYazdirs.VeresiyeCikisYapanAraclar = listeleme.Where(x => x.lVeresiye == true).Select(x => x.cPlaka.ToUpper() + (x.lVeresiye == true ? " / -" : " / +") + string.Format("{0:N2}", x.fVeresiye) + " TL\n" + x.cAciklama).ToArray();
                    aracYazdirs.iVeresiyeAracSayisi = aracYazdirs.iVeresiyeBekleyenAracSayisi + aracYazdirs.iVeresiyeCikisYapanAracSayisi;

                    aracYazdirs.iMisafirCikisYapanAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Count();
                    aracYazdirs.MisafirCikisYapanAraclar = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Select(x => x.cPlaka.ToUpper() + "\n" + x.cAciklama).ToArray();
                    aracYazdirs.iMisafirBekleyenAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Count();
                    aracYazdirs.MisafirBekleyenAraclar = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Select(x => x.cPlaka.ToUpper() + "\n" + x.cAciklama).ToArray();
                    aracYazdirs.iMisafirAracSayisi = aracYazdirs.iMisafirCikisYapanAracSayisi + aracYazdirs.iMisafirBekleyenAracSayisi;

                    aracYazdirs.YeniAboneAraclar = abonelistesi.Where(x => x.dKayitTarihi.Date == DateTime.Now.Date).Select(x => x.cPlaka.ToUpper()).ToArray();

                    string[] aboneUcretiAlinanAraclar = new string[0];
                    for (int i = 0; i < abonelistesi.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(abonelistesi[i].cOdemeString))
                        {
                            abonelistesi[i].aboneOdemeTakvimis = JsonConvert.DeserializeObject<List<Models.AboneOdemeTakvimi>>(abonelistesi[i].cOdemeString);

                            for (int j = 0; j < abonelistesi[i].aboneOdemeTakvimis.Count; j++)
                            {
                                if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                                {
                                    if (Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date)
                                    {
                                        Array.Resize(ref aboneUcretiAlinanAraclar, (aboneUcretiAlinanAraclar.Length + 1));
                                        aboneUcretiAlinanAraclar[aboneUcretiAlinanAraclar.Length - 1] = abonelistesi[i].cPlaka.ToUpper();
                                    }
                                }
                                else if (dGirisTarihiLocal == Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                {
                                    if (Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date)
                                    {
                                        Array.Resize(ref aboneUcretiAlinanAraclar, (aboneUcretiAlinanAraclar.Length + 1));
                                        aboneUcretiAlinanAraclar[aboneUcretiAlinanAraclar.Length - 1] = abonelistesi[i].cPlaka.ToUpper();
                                    }
                                }
                                else if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                {
                                    if ((Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date) && (Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date))
                                    {
                                        Array.Resize(ref aboneUcretiAlinanAraclar, (aboneUcretiAlinanAraclar.Length + 1));
                                        aboneUcretiAlinanAraclar[aboneUcretiAlinanAraclar.Length - 1] = abonelistesi[i].cPlaka.ToUpper();
                                    }
                                }
                            }
                        }
                    }

                    aracYazdirs.AboneUcretiAlinanAraclar = aboneUcretiAlinanAraclar;

                    aracYazdirs.iIptalEdilenAracSayisi = listelemeIptal.Count();

                    List<string> yazdir = new List<string>();
                    yazdir.Add("Rapor Yazdırma Tarihi");
                    yazdir.Add(String.Format("{0:dd MMMM yyyy, dddd, HH:mm}", DateTime.Now));
                    yazdir.Add("-------");
                    yazdir.Add("Çıkış Yapan Sayısı");
                    yazdir.Add(aracYazdirs.iToplamAracSayisi.ToString());
                    yazdir.Add("Bekleyen Sayısı");
                    yazdir.Add(aracYazdirs.iToplamBekleyenAracSayisi.ToString());
                    yazdir.Add("-------");
                    yazdir.Add("Abone Sayısı");
                    yazdir.Add(aracYazdirs.iAboneAracSayisi.ToString());
                    yazdir.Add("Abone Çıkış Yapan Sayısı");
                    yazdir.Add(aracYazdirs.iAboneCikisYapanAracSayisi.ToString());
                    yazdir.Add("Abone Bekleyen Sayısı");
                    yazdir.Add(aracYazdirs.iAboneBekleyenAracSayisi.ToString());
                    yazdir.Add("-------");
                    yazdir.Add("Düzeltme Sayısı");
                    yazdir.Add(aracYazdirs.iDuzeltmeAracSayisi.ToString());
                    yazdir.Add("Düzeltme Çıkış Yapan Sayısı");
                    yazdir.Add(aracYazdirs.iDuzeltmeCikisYapanAracSayisi.ToString());
                    yazdir.Add("Düzeltme Bekleyen Sayısı");
                    yazdir.Add(aracYazdirs.iDuzeltmeBekleyenAracSayisi.ToString());
                    yazdir.Add("-------");
                    yazdir.Add("Veresiye Sayısı");
                    yazdir.Add(aracYazdirs.iVeresiyeAracSayisi.ToString());
                    yazdir.Add("Veresiye Çıkış Yapan Sayısı");
                    yazdir.Add(aracYazdirs.iVeresiyeCikisYapanAracSayisi.ToString());
                    yazdir.Add("Veresiye Bekleyen Sayısı");
                    yazdir.Add(aracYazdirs.iVeresiyeBekleyenAracSayisi.ToString());
                    yazdir.Add("-------");
                    yazdir.Add("Misafir Sayısı");
                    yazdir.Add(aracYazdirs.iMisafirAracSayisi.ToString());
                    yazdir.Add("Misafir Çıkış Yapan Sayısı");
                    yazdir.Add(aracYazdirs.iMisafirCikisYapanAracSayisi.ToString());
                    yazdir.Add("Misafir Bekleyen Sayısı");
                    yazdir.Add(aracYazdirs.iMisafirBekleyenAracSayisi.ToString());
                    yazdir.Add("-------");
                    yazdir.Add("İptal Edilen Sayısı");
                    yazdir.Add(aracYazdirs.iIptalEdilenAracSayisi.ToString());
                    yazdir.Add("-------");
                    yazdir.Add("Çıkış Yapan Ücret");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.fGenelToplam) + " TL");
                    yazdir.Add("-------");
                    yazdir.Add("Otopark Ücreti (Düzeltmesiz)");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.fOtoparkToplamTutar) + " TL");
                    yazdir.Add("Otopark Ücreti (Düzeltmeli)");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.fOtoparkDuzeltmeliToplamTutar) + " TL");
                    yazdir.Add("-------");
                    yazdir.Add("Düzeltme (Eksiltme) Ücreti");
                    yazdir.Add("-" + String.Format("{0:N2}", aracYazdirs.fDuzeltmeEksiltmeTutar) + " TL");
                    yazdir.Add("Düzeltme (Artırma) Ücreti");
                    yazdir.Add("+" + String.Format("{0:N2}", aracYazdirs.fDuzeltmeArtirmaTutar) + " TL");
                    yazdir.Add("-------");
                    yazdir.Add("Veresiye Ücreti");
                    yazdir.Add("+" + String.Format("{0:N2}", aracYazdirs.fVeresiyeToplamTutar) + " TL");
                    yazdir.Add("-------");
                    yazdir.Add("Yıkama Sayısı");
                    yazdir.Add(aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.iToplam).ToString());
                    yazdir.Add("Yıkama Ücreti");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.dToplamFiyat)) + " TL");
                    yazdir.Add("Ürün Sayısı");
                    yazdir.Add(aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.iToplam).ToString());
                    yazdir.Add("Ürün Ücreti");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.dToplamFiyat)) + " TL");
                    yazdir.Add("-------");
                    yazdir.Add("Yeni Abone Araç Sayısı");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.iYeniAboneArac));
                    yazdir.Add("Abone Ücreti Alınan Araç Sayısı");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.iAboneArac));
                    yazdir.Add("Abone Ücreti Toplam Tutar");
                    yazdir.Add(String.Format("{0:N2}", aracYazdirs.fAbonelikUcreti) + "TL");
                    yazdir.Add("-------");
                    if (aracYazdirs.urunListesis != null && aracYazdirs.urunListesis.Count > 0)
                    {
                        for (int j = 0; j < aracYazdirs.urunListesis.Count; j++)
                        {
                            if (aracYazdirs.urunListesis[j].iToplam > 0)
                            {
                                yazdir.Add(aracYazdirs.urunListesis[j].cUrun + " Sayısı");
                                yazdir.Add(aracYazdirs.urunListesis[j].iToplam.ToString());
                                yazdir.Add(aracYazdirs.urunListesis[j].cUrun + " Ücreti");
                                yazdir.Add(String.Format("{0:N2}", aracYazdirs.urunListesis[j].dToplamFiyat) + " TL");
                            }
                        }
                        yazdir.Add("-------");
                    }
                    if (aracYazdirs.AboneCikisYapanAraclar != null && aracYazdirs.AboneCikisYapanAraclar.Length > 0)
                    {
                        yazdir.Add("Abone Çıkış Yapan Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.AboneCikisYapanAraclar.Length; i++)
                        {
                            yazdir.Add(aracYazdirs.AboneCikisYapanAraclar[i].ToUpper());
                        }
                        yazdir.Add("-------");
                    }
                    if (aracYazdirs.AboneBekleyenAraclar != null && aracYazdirs.AboneBekleyenAraclar.Length > 0)
                    {
                        yazdir.Add("Abone Bekleyen Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.AboneBekleyenAraclar.Length; i++)
                        {
                            yazdir.Add(aracYazdirs.AboneBekleyenAraclar[i].ToUpper());
                        }
                        yazdir.Add("-------");
                    }
                    if (aracYazdirs.DuzeltmeCikisYapanAraclar != null && aracYazdirs.DuzeltmeCikisYapanAraclar.Length > 0)
                    {
                        yazdir.Add("Düzeltme Çıkış Yapan Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.DuzeltmeCikisYapanAraclar.Length; i++)
                        {
                            string[] Split = aracYazdirs.DuzeltmeCikisYapanAraclar[i].Split('\n');
                            yazdir.Add(Split[0]);
                            double iSatirSayisi = Math.Ceiling((double)Split[1].Length / 30);
                            int iBaslangic = 0;
                            int iBitis = 0;
                            for (int j = 0; j < iSatirSayisi; j++)
                            {
                                iBaslangic = j * 30;
                                iBitis = 30;
                                if (Split[1].Length < iBaslangic + 30)
                                {
                                    iBitis = Split[1].Length - iBaslangic;
                                }
                                yazdir.Add(Split[1].Substring(iBaslangic, iBitis));
                            }
                        }
                        yazdir.Add("-------");
                    }
                    if (aracYazdirs.DuzeltmeBekleyenAraclar != null && aracYazdirs.DuzeltmeBekleyenAraclar.Length > 0)
                    {
                        yazdir.Add("Düzeltme Bekleyen Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.DuzeltmeBekleyenAraclar.Length; i++)
                        {
                            string[] Split = aracYazdirs.DuzeltmeBekleyenAraclar[i].Split('\n');
                            yazdir.Add(Split[0]);
                            double iSatirSayisi = Math.Ceiling((double)Split[1].Length / 30);
                            int iBaslangic = 0;
                            int iBitis = 0;
                            for (int j = 0; j < iSatirSayisi; j++)
                            {
                                iBaslangic = j * 30;
                                iBitis = 30;
                                if (Split[1].Length < iBaslangic + 30)
                                {
                                    iBitis = Split[1].Length - iBaslangic;
                                }
                                yazdir.Add(Split[1].Substring(iBaslangic, iBitis));
                            }
                        }
                        yazdir.Add("-------");
                    }

                    if (aracYazdirs.VeresiyeBekleyenAraclar != null && aracYazdirs.VeresiyeBekleyenAraclar.Length > 0)
                    {
                        yazdir.Add("Veresiye Bekleyen Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.VeresiyeBekleyenAraclar.Length; i++)
                        {
                            string[] Split = aracYazdirs.VeresiyeBekleyenAraclar[i].Split('\n');
                            yazdir.Add(Split[0]);
                            double iSatirSayisi = Math.Ceiling((double)Split[1].Length / 30);
                            int iBaslangic = 0;
                            int iBitis = 0;
                            for (int j = 0; j < iSatirSayisi; j++)
                            {
                                iBaslangic = j * 30;
                                iBitis = 30;
                                if (Split[1].Length < iBaslangic + 30)
                                {
                                    iBitis = Split[1].Length - iBaslangic;
                                }
                                yazdir.Add(Split[1].Substring(iBaslangic, iBitis));
                            }
                        }
                        yazdir.Add("-------");
                    }

                    if (aracYazdirs.VeresiyeCikisYapanAraclar != null && aracYazdirs.VeresiyeCikisYapanAraclar.Length > 0)
                    {
                        yazdir.Add("Veresiye Çıkış Yapan Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.VeresiyeCikisYapanAraclar.Length; i++)
                        {
                            string[] Split = aracYazdirs.VeresiyeCikisYapanAraclar[i].Split('\n');
                            yazdir.Add(Split[0]);
                            double iSatirSayisi = Math.Ceiling((double)Split[1].Length / 30);
                            int iBaslangic = 0;
                            int iBitis = 0;
                            for (int j = 0; j < iSatirSayisi; j++)
                            {
                                iBaslangic = j * 30;
                                iBitis = 30;
                                if (Split[1].Length < iBaslangic + 30)
                                {
                                    iBitis = Split[1].Length - iBaslangic;
                                }
                                yazdir.Add(Split[1].Substring(iBaslangic, iBitis));
                            }
                        }
                        yazdir.Add("-------");
                    }

                    if (aracYazdirs.MisafirCikisYapanAraclar != null && aracYazdirs.MisafirCikisYapanAraclar.Length > 0)
                    {
                        yazdir.Add("Misafir Çıkış Yapan Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.MisafirCikisYapanAraclar.Length; i++)
                        {
                            string[] Split = aracYazdirs.MisafirCikisYapanAraclar[i].Split('\n');
                            yazdir.Add(Split[0]);
                            double iSatirSayisi = Math.Ceiling((double)Split[1].Length / 30);
                            int iBaslangic = 0;
                            int iBitis = 0;
                            for (int j = 0; j < iSatirSayisi; j++)
                            {
                                iBaslangic = j * 30;
                                iBitis = 30;
                                if (Split[1].Length < iBaslangic + 30)
                                {
                                    iBitis = Split[1].Length - iBaslangic;
                                }
                                yazdir.Add(Split[1].Substring(iBaslangic, iBitis));
                            }
                        }
                        yazdir.Add("-------");
                    }
                    if (aracYazdirs.MisafirBekleyenAraclar != null && aracYazdirs.MisafirBekleyenAraclar.Length > 0)
                    {
                        yazdir.Add("Misafir Bekleyen Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.MisafirBekleyenAraclar.Length; i++)
                        {
                            string[] Split = aracYazdirs.MisafirBekleyenAraclar[i].Split('\n');
                            yazdir.Add(Split[0]);
                            double iSatirSayisi = Math.Ceiling((double)Split[1].Length / 30);
                            int iBaslangic = 0;
                            int iBitis = 0;
                            for (int j = 0; j < iSatirSayisi; j++)
                            {
                                iBaslangic = j * 30;
                                iBitis = 30;
                                if (Split[1].Length < iBaslangic + 30)
                                {
                                    iBitis = Split[1].Length - iBaslangic;
                                }
                                yazdir.Add(Split[1].Substring(iBaslangic, iBitis));
                            }
                        }
                        yazdir.Add("-------");
                    }

                    if (aracYazdirs.YeniAboneAraclar != null && aracYazdirs.YeniAboneAraclar.Length > 0)
                    {
                        yazdir.Add("Yeni Abone Araç  Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.YeniAboneAraclar.Length; i++)
                        {
                            yazdir.Add(aracYazdirs.YeniAboneAraclar[i]);
                        }
                        yazdir.Add("-------");
                    }

                    if (aracYazdirs.AboneUcretiAlinanAraclar != null && aracYazdirs.AboneUcretiAlinanAraclar.Length > 0)
                    {
                        yazdir.Add("Abone Ücreti Alınan Araç  Listesi");
                        yazdir.Add("-------");
                        for (int i = 0; i < aracYazdirs.AboneUcretiAlinanAraclar.Length; i++)
                        {
                            yazdir.Add(aracYazdirs.AboneUcretiAlinanAraclar[i]);
                        }
                        yazdir.Add("-------");
                    }

                    var doc = new PrintDocument();
                    doc.PrinterSettings.PrinterName = "Microsoft Print To PDF";
                    //doc.PrinterSettings.PrinterName = "Hoin-58-Series";
                    doc.PrintPage += (sender, args) => CikisYapanAracYazdirYeniYazdir(yazdir, args);
                    doc.Print();
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
            }
        }

        int iSayfaSayisi = 0;
        int iBaslangic = 0;
        int iBitis = 76;
        private void CikisYapanAracYazdirYeniYazdir(object sender, PrintPageEventArgs e)
        {
            iSayfaSayisi++;
            int iSatir = 15;
            List<string> aracYazdirs = sender as List<string>;
            if (aracYazdirs != null)
            {
                bool lDurum = true;
                if (aracYazdirs.Count < iBitis)
                {
                    iBitis = aracYazdirs.Count;
                    lDurum = false;
                }

                for (int i = iBaslangic; i < iBitis; i++)
                {
                    e.Graphics.DrawString(aracYazdirs[i], new Font("Arial", 8), Brushes.Black, 3, iSatir);
                    iSatir += 15;
                }

                iBaslangic += 76;
                iBitis += 76;

                e.HasMorePages = lDurum;
            }
        }

        public void CikisYapanAracExcelYeni(
            int? sayfaNo,
            string cPlaka,
            int? iKodAracTipi,
            string dGirisTarihi,
            string dCikisTarihi,
            int? iAbonelikDurumu,
            string dAboneBaslangicTarihi,
            string dAboneBitisTarihi,
            int? iDuzeltmeTipi,
            int? iKodUrun2)
        {
            try
            {
                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.dCikisTarihi != Convert.ToDateTime("1900-01-01") &&
                                        table.iAktifMi == 1 &&
                                        table.iKodAracTipi != 4 && // Misafir Dilse
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                     select new Models.AracYeni
                                     {
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                         iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                         cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                         cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                         cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                         cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                         lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                         iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                         cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                         iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                         dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                         dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                     }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeIptal = (from table in dc.Aracs
                                          join tableAracTipis in dc.AracTipis
                                             on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                          from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                          join tableMusteri3s in dc.Musteri3s
                                             on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                          from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                          where
                                             table.iKodLokasyon == iKodLokasyonLogin &&
                                             table.iAktifMi == -1 &&
                                             table.iKodAracTipi != 4 && // Misafir Dilse
                                             (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                             (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                          select new Models.AracYeni
                                          {
                                              cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                              cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                              cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                              iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                              cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                              cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                              iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                              cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                              cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                              iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                              cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                              iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                              dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                              fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                              fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                              dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                          }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeBekleyen = (from table in dc.Aracs
                                             join tableAracTipis in dc.AracTipis
                                                on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                             from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                             join tableMusteri3s in dc.Musteri3s
                                                on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                             from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                             where
                                                table.iKodLokasyon == iKodLokasyonLogin &&
                                                table.iAktifMi == 1 &&
                                                table.iKodAracTipi != 4 && // Misafir Dilse
                                                (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                             select new Models.AracYeni
                                             {
                                                 cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                 cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                 cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                 iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                 cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                 cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                 iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                 cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                 cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                 iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                 cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                 iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                 dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                 fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                 fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                 dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                             }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeMisafirArac = (from table in dc.Aracs
                                                join tableAracTipis in dc.AracTipis
                                                   on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                                from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                                join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                                from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                                where
                                                   table.iKodLokasyon == iKodLokasyonLogin &&
                                                   table.iAktifMi == 1 &&
                                                   table.iKodAracTipi == 4 && // Misafir Araç
                                                   (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                   (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                                select new Models.AracYeni
                                                {
                                                    cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                    cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                    cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                    iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                    cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                    cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                    iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                    cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                    cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                    iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                    cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                    iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                    dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                    fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                    fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                    dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var abonelistesi = (from table in dc.Aboneliks
                                        join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                        from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                        where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.iAktifMi == 1
                                        select new Models.Abonelik
                                        {
                                            dKayitTarihi = (table.dKayitTarihi != null ? Convert.ToDateTime(table.dKayitTarihi) : Convert.ToDateTime("1900-01-01")),
                                            cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                            cOdemeString = (table.cOdeme != null && table.cOdeme.ToString() != string.Empty ? table.cOdeme : string.Empty)
                                        }).ToList();

                    var abonelistesi2 = abonelistesi;
                    double dAbonelikUcretiOdeyenlerTutari = 0;
                    int iAbonelikUcretiOdeyenler = 0;


                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        listeleme = listeleme.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.cPlaka == cPlaka).ToList();
                    }

                    DateTime dGirisTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dCikisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dGirisTarihi))
                    {
                        dGirisTarihiLocal = Convert.ToDateTime(dGirisTarihi);
                    }
                    if (!String.IsNullOrEmpty(dCikisTarihi))
                    {
                        dCikisTarihiLocal = Convert.ToDateTime(dCikisTarihi);
                    }

                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") || dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();



                            abonelistesi = abonelistesi.Where(
                                   model =>
                                   model.dKayitTarihi.Date >= dGirisTarihiLocal.Date
                                   ).ToList();
                        }

                        if (dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            abonelistesi = abonelistesi.Where(
                              model =>
                              model.dKayitTarihi.Date <= dCikisTarihiLocal.Date
                              ).ToList();

                        }

                        for (int i = 0; i < abonelistesi2.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(abonelistesi2[i].cOdemeString))
                            {
                                abonelistesi2[i].aboneOdemeTakvimis = JsonConvert.DeserializeObject<List<Models.AboneOdemeTakvimi>>(abonelistesi2[i].cOdemeString);

                                for (int j = 0; j < abonelistesi2[i].aboneOdemeTakvimis.Count; j++)
                                {
                                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                                    {
                                        if (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date)
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                    else if (dGirisTarihiLocal == Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                    {
                                        if (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date)
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                    else if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                    {
                                        if ((Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date) && (Convert.ToDateTime(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date))
                                        {
                                            dAbonelikUcretiOdeyenlerTutari += Convert.ToDouble(abonelistesi2[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                            iAbonelikUcretiOdeyenler++;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    DateTime dAboneBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dAboneBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dAboneBaslangicTarihi))
                    {
                        dAboneBaslangicTarihiLocal = Convert.ToDateTime(dAboneBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dAboneBitisTarihi))
                    {
                        dAboneBitisTarihiLocal = Convert.ToDateTime(dAboneBitisTarihi);
                    }

                    if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") || dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();
                        }

                        if (dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            if (dGirisTarihiLocal.Date == DateTime.Now.Date)
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == DateTime.Now.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01")
                                    ).ToList();
                            }
                            else
                            {
                                listelemeBekleyen = listelemeBekleyen.Where(
                                    model =>
                                    model.dGirisTarihi.Date == dGirisTarihiLocal.Date && (model.dCikisTarihi.Date != dGirisTarihiLocal.Date && model.dCikisTarihi.Date != Convert.ToDateTime("1900-01-01"))
                                    ).ToList();
                            }

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (iKodAracTipi != null && iKodAracTipi >= 0)
                    {
                        listeleme = listeleme.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                    }

                    if (iAbonelikDurumu != null)
                    {
                        listeleme = listeleme.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                    }

                    if (iDuzeltmeTipi != null)
                    {
                        listeleme = listeleme.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                    }

                    List<Models.UrunJson2> toplamUrunler = new List<Models.UrunJson2>();
                    List<Models.AracYeni> listelemeYeni = new List<Models.AracYeni>();
                    int iSatirNo = 1;
                    for (int i = 0; i < listeleme.Count; i++)
                    {
                        listeleme[i].iSatirNumarasi = iSatirNo++;
                        if (!String.IsNullOrEmpty(listeleme[i].cUrun))
                        {
                            listeleme[i].urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(listeleme[i].cUrun);
                            for (int j = 0; j < listeleme[i].urun2Listesi.Count; j++)
                            {
                                if (iKodUrun2 != null && iKodUrun2 > 0)
                                {
                                    if (iKodUrun2 == listeleme[i].urun2Listesi[j].iKodUrun2)
                                    {
                                        listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                        listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                        toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                        break;
                                    }
                                }
                                else
                                {
                                    listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                    listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                    toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                }
                            }

                            listelemeYeni.Add(listeleme[i]);
                        }
                        else if (iKodUrun2 == null || iKodUrun2 == 0)
                        {
                            listelemeYeni.Add(listeleme[i]);
                        }
                    }
                    listeleme = listelemeYeni;

                    List<Models.Arac.AracYeniToplaUrun> toplamUrunListesi =
                        toplamUrunler.GroupBy(l => l.iKodUrun2)
                            .Select(cl => new Models.Arac.AracYeniToplaUrun
                            {
                                iKodUrun2 = cl.First().iKodUrun2,
                                iToplam = cl.Sum(c => Convert.ToInt32(c.iAdet)),
                                dToplamFiyat = cl.Sum(c => Convert.ToDouble(c.cFiyat.ToString().Replace(".", ""))),
                            }).ToList();

                    double dToplamUrunler = 0;
                    if (toplamUrunListesi != null)
                    {
                        for (int i = 0; i < toplamUrunListesi.Count; i++)
                        {
                            dToplamUrunler += toplamUrunListesi[i].dToplamFiyat;
                        }
                    }

                    string[] aboneUcretiAlinanAraclar = new string[0];
                    for (int i = 0; i < abonelistesi.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(abonelistesi[i].cOdemeString))
                        {
                            abonelistesi[i].aboneOdemeTakvimis = JsonConvert.DeserializeObject<List<Models.AboneOdemeTakvimi>>(abonelistesi[i].cOdemeString);

                            for (int j = 0; j < abonelistesi[i].aboneOdemeTakvimis.Count; j++)
                            {
                                if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal == Convert.ToDateTime("1900-01-01"))
                                {
                                    if (Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date)
                                    {
                                        Array.Resize(ref aboneUcretiAlinanAraclar, (aboneUcretiAlinanAraclar.Length + 1));
                                        aboneUcretiAlinanAraclar[aboneUcretiAlinanAraclar.Length - 1] = abonelistesi[i].cPlaka.ToUpper();
                                    }
                                }
                                else if (dGirisTarihiLocal == Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                {
                                    if (Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date)
                                    {
                                        Array.Resize(ref aboneUcretiAlinanAraclar, (aboneUcretiAlinanAraclar.Length + 1));
                                        aboneUcretiAlinanAraclar[aboneUcretiAlinanAraclar.Length - 1] = abonelistesi[i].cPlaka.ToUpper();
                                    }
                                }
                                else if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") && dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                                {
                                    if ((Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date >= dGirisTarihiLocal.Date) &&(Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date <= dCikisTarihiLocal.Date))
                                    {
                                        Array.Resize(ref aboneUcretiAlinanAraclar, (aboneUcretiAlinanAraclar.Length + 1));
                                        aboneUcretiAlinanAraclar[aboneUcretiAlinanAraclar.Length - 1] = abonelistesi[i].cPlaka.ToUpper();
                                    }
                                }
                            }
                        }
                    }



                    Models.AracYeni.AracYazdir aracYazdirs = new Models.AracYeni.AracYazdir();
                    aracYazdirs.iToplamAracSayisi = listeleme.Count();
                    aracYazdirs.dUrunFiyatToplam = dToplamUrunler;
                    aracYazdirs.fOtoparkToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti);
                    aracYazdirs.fDuzeltmeEksiltmeTutar = listeleme.Where(x => x.iDuzeltmeTipi == 1).Sum(x => x.fDuzeltme);
                    aracYazdirs.fDuzeltmeArtirmaTutar = listeleme.Where(x => x.iDuzeltmeTipi == 2).Sum(x => x.fDuzeltme);
                    aracYazdirs.fOtoparkDuzeltmeliToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti + (x.iDuzeltmeTipi == 1 ? -1 * x.fDuzeltme : 0) + (x.iDuzeltmeTipi == 2 ? x.fDuzeltme : 0));
                    aracYazdirs.fVeresiyeToplamTutar = listeleme.Where(x => x.lVeresiye == true).Sum(x => x.fVeresiye);
                    aracYazdirs.fGenelToplam = (aracYazdirs.fOtoparkToplamTutar + aracYazdirs.fDuzeltmeArtirmaTutar + (float)aracYazdirs.dUrunFiyatToplam) - aracYazdirs.fDuzeltmeEksiltmeTutar;
                    aracYazdirs.fAbonelikUcreti = (float)dAbonelikUcretiOdeyenlerTutari;
                    aracYazdirs.iAboneArac = iAbonelikUcretiOdeyenler;
                    aracYazdirs.iYeniAboneArac = abonelistesi.Count;
                    aracYazdirs.urunListesis = new List<Models.AracYeni.AracYazdir.UrunListesi>();
                    aracYazdirs.AboneUcretiAlinanAraclar = aboneUcretiAlinanAraclar;
                    List<Models.Urun2> urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                    for (int i = 0; i < urun2Listesi.Count; i++)
                    {
                        int iToplam = 0;
                        double dToplamFiyat = 0;

                        for (int j = 0; j < toplamUrunListesi.Count; j++)
                        {
                            if (urun2Listesi[i].iKodUrun2 == toplamUrunListesi[j].iKodUrun2)
                            {
                                iToplam = toplamUrunListesi[j].iToplam;
                                dToplamFiyat = toplamUrunListesi[j].dToplamFiyat;
                                break;
                            }
                        }

                        aracYazdirs.urunListesis.Add(new Models.AracYeni.AracYazdir.UrunListesi
                        {
                            cUrun = urun2Listesi[i].cAdi,
                            iToplam = iToplam,
                            dToplamFiyat = dToplamFiyat,
                            lStokTutlacakMi = urun2Listesi[i].lStokTutlacakMi
                        });
                    }

                    aracYazdirs.iToplamBekleyenAracSayisi = listelemeBekleyen.Count();

                    aracYazdirs.iAboneCikisYapanAracSayisi = listeleme.Where(x => x.iAboneMi == 1).Count();
                    aracYazdirs.AboneCikisYapanAraclar = listeleme.Where(x => x.iAboneMi == 1).Select(x => x.cPlaka).ToArray();
                    aracYazdirs.iAboneBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iAboneMi == 1).Count();
                    aracYazdirs.AboneBekleyenAraclar = listelemeBekleyen.Where(x => x.iAboneMi == 1).Select(x => x.cPlaka).ToArray();
                    aracYazdirs.iAboneAracSayisi = aracYazdirs.iAboneCikisYapanAracSayisi + aracYazdirs.iAboneBekleyenAracSayisi;

                    aracYazdirs.iDuzeltmeCikisYapanAracSayisi = listeleme.Where(x => x.iDuzeltmeTipi > 0).Count();
                    aracYazdirs.DuzeltmeCikisYapanAraclar = listeleme.Where(x => x.iDuzeltmeTipi > 0).Select(x => x.cPlaka.ToUpper() + (x.iDuzeltmeTipi == 1 ? " / -" : " / +") + string.Format("{0:N2}", x.fDuzeltme) + " TL / " + x.cAciklama).ToArray();
                    aracYazdirs.iDuzeltmeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Count();
                    aracYazdirs.DuzeltmeBekleyenAraclar = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Select(x => x.cPlaka.ToUpper() + (x.iDuzeltmeTipi == 1 ? " / -" : " / +") + string.Format("{0:N2}", x.fDuzeltme) + " TL / " + x.cAciklama).ToArray();
                    aracYazdirs.iDuzeltmeAracSayisi = aracYazdirs.iDuzeltmeCikisYapanAracSayisi + aracYazdirs.iDuzeltmeBekleyenAracSayisi;

                    aracYazdirs.iVeresiyeCikisYapanAracSayisi = listeleme.Where(x => x.lVeresiye == true).Count();
                    aracYazdirs.VeresiyeCikisYapanAraclar = listeleme.Where(x => x.lVeresiye == true).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iVeresiyeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.lVeresiye == true).Count();
                    aracYazdirs.VeresiyeBekleyenAraclar = listeleme.Where(x => x.lVeresiye == true).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iVeresiyeAracSayisi = aracYazdirs.iVeresiyeBekleyenAracSayisi + aracYazdirs.iVeresiyeCikisYapanAracSayisi;

                    aracYazdirs.iMisafirCikisYapanAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Count();
                    aracYazdirs.MisafirCikisYapanAraclar = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iMisafirBekleyenAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Count();
                    aracYazdirs.MisafirBekleyenAraclar = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iMisafirAracSayisi = aracYazdirs.iMisafirCikisYapanAracSayisi + aracYazdirs.iMisafirBekleyenAracSayisi;

                    aracYazdirs.YeniAboneAraclar = abonelistesi.Where(x => x.dKayitTarihi.Date == DateTime.Now.Date).Select(x => x.cPlaka.ToUpper()).ToArray();

                    aracYazdirs.iIptalEdilenAracSayisi = listelemeIptal.Count();

                    ExcelPackage Ep = new ExcelPackage();
                    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

                    Sheet.Cells["A:I"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    Sheet.Cells["A:I"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                    Color backgroundColor = System.Drawing.ColorTranslator.FromHtml("#195595");
                    Color backgroundColor1 = System.Drawing.ColorTranslator.FromHtml("#F1F1F1");
                    Color backgroundColor2 = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    Color textColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    Sheet.Cells["A1:I1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    Sheet.Cells["A1:I1"].Style.Font.Color.SetColor(textColor);
                    Sheet.Cells["A1:I1"].Style.Font.Bold = true;

                    Sheet.Cells["A1"].Value = "#";
                    Sheet.Cells["B1"].Value = "Plaka";
                    Sheet.Cells["C1"].Value = "Tipi";
                    Sheet.Cells["D1"].Value = "Giriş Tarihi";
                    Sheet.Cells["E1"].Value = "Çıkış Tarihi";
                    Sheet.Cells["F1"].Value = "Otopark Süresi";
                    Sheet.Cells["G1"].Value = "Abone";
                    Sheet.Cells["H1"].Value = "Abone Başlangıç Tarihi";
                    Sheet.Cells["I1"].Value = "Abone Başlangıç Tarihi";

                    int row = 2;
                    int satir = 0;
                    Color rowBackgroundColor = backgroundColor2;

                    foreach (var item in listeleme)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        double dToplamFiyat = 0;

                        Sheet.Cells[string.Format("A{0}", row)].Value = item.iSatirNumarasi;
                        Sheet.Cells[string.Format("B{0}", row)].Value = item.cPlaka.ToUpper();
                        Sheet.Cells[string.Format("C{0}", row)].Value = item.cAracTipi;
                        Sheet.Cells[string.Format("D{0}", row)].Value = item.cGirisTarihi;
                        Sheet.Cells[string.Format("E{0}", row)].Value = item.cCikisTarihi;
                        Sheet.Cells[string.Format("F{0}", row)].Value = item.cOtoparkSuresi + " dakika";
                        if (item.iAboneMi == 1)
                        {
                            Sheet.Cells[string.Format("G{0}", row)].Value = "Evet";
                        }
                        else if (item.iAboneMi == 2)
                        {
                            Sheet.Cells[string.Format("G{0}", row)].Value = "Hayır";
                        }
                        if (item.cAboneBaslangicTarihi == "01.01.1900")
                        {
                            Sheet.Cells[string.Format("H{0}", row)].Value = string.Empty;
                        }
                        else
                        {
                            Sheet.Cells[string.Format("H{0}", row)].Value = item.cAboneBaslangicTarihi;
                        }
                        if (item.cAboneBitisTarihi == "01.01.1900")
                        {
                            Sheet.Cells[string.Format("I{0}", row)].Value = string.Empty;
                        }
                        else
                        {
                            Sheet.Cells[string.Format("I{0}", row)].Value = item.cAboneBitisTarihi;
                        }

                        row++;

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        dToplamFiyat += Convert.ToDouble(item.cOtoparkUcreti.Replace(".", ","));

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Otopark Ücreti : " + item.cOtoparkUcreti + " TL";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        row++;

                        if (item.urun2Listesi != null)
                        {
                            for (int j = 0; j < item.urun2Listesi.Count; j++)
                            {
                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                                dToplamFiyat += Convert.ToDouble(item.urun2Listesi[j].cFiyat.Replace(".", ","));

                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                                Sheet.Cells[string.Format("A{0}", row)].Value = item.urun2Listesi[j].cUrun + " : " + item.urun2Listesi[j].iAdet + " x " + item.urun2Listesi[j].cBirimFiyati + " TL" + " = " + item.urun2Listesi[j].cFiyat + " TL";
                                Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                                row++;
                            }
                        }

                        if (item.iDuzeltmeTipi == 1)
                        {
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            dToplamFiyat -= Convert.ToDouble(item.cDuzeltme.Replace(".", ","));

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = "Düzeltme : -" + item.cDuzeltme + " TL";
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                            row++;
                        }
                        else if (item.iDuzeltmeTipi == 2)
                        {
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            dToplamFiyat += Convert.ToDouble(item.cDuzeltme.Replace(".", ","));

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = "Düzeltme : +" + item.cDuzeltme + " TL";
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                            row++;
                        }
                        if (item.iDuzeltmeTipi > 0 || item.iUrunSilindiMi > 0)
                        {
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = "Açıklama : " + item.cAciklama;
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                            row++;
                        }
                        if (item.lVeresiye == true)
                        {
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = "Veresiye : +" + item.cVeresiye + " TL";
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                            row++;
                        }
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Ücret : " + String.Format("{0:N2}", dToplamFiyat) + " TL";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        row++;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Raporlar";
                    Sheet.Cells[string.Format("A{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row)].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    Sheet.Cells[string.Format("A{0}", row)].Style.Font.Color.SetColor(textColor);
                    Sheet.Cells[string.Format("A{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Çıkış Yapan Sayısı : " + aracYazdirs.iToplamAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Bekleyen Sayısı : " + aracYazdirs.iToplamBekleyenAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Abone Sayısı : " + aracYazdirs.iAboneAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;



                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Çıkış Yapan Abone Sayısı : " + aracYazdirs.iAboneCikisYapanAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Bekleyen Abone Sayısı : " + aracYazdirs.iAboneBekleyenAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Düzeltme Sayısı : " + aracYazdirs.iDuzeltmeAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;
                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Çıkış Yapan Düzeltme Sayısı : " + aracYazdirs.iDuzeltmeCikisYapanAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Bekleyen Düzeltme Sayısı : " + aracYazdirs.iDuzeltmeBekleyenAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;


                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Veresiye Sayısı: " + aracYazdirs.iVeresiyeAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;


                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Çıkış Yapan Veresiye Sayısı: " + aracYazdirs.iVeresiyeCikisYapanAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Bekleyen Veresiye Sayısı: " + aracYazdirs.iVeresiyeBekleyenAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;


                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Misafir Sayısı : " + aracYazdirs.iMisafirAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;


                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Çıkış Yapan Misafir Sayısı : " + aracYazdirs.iMisafirCikisYapanAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Bekleyen Misafir Sayısı : " + aracYazdirs.iMisafirBekleyenAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam İptal Edilen Sayısı : " + aracYazdirs.iIptalEdilenAracSayisi;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Çıkış Yapan Ücret : " + String.Format("{0:N2}", aracYazdirs.fGenelToplam) + " TL";
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Otopark Ücreti (Düzeltmesiz) : " + String.Format("{0:N2}", aracYazdirs.fOtoparkToplamTutar) + " TL";
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Otopark Ücreti (Düzeltmeli) : " + String.Format("{0:N2}", aracYazdirs.fOtoparkDuzeltmeliToplamTutar) + " TL";
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Düzeltme (Eksiltme) Ücreti : " + String.Format("{0:N2}", aracYazdirs.fDuzeltmeEksiltmeTutar) + " TL";
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;



                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Düzeltme (Artırma) Ücreti : " + String.Format("{0:N2}", aracYazdirs.fDuzeltmeArtirmaTutar) + " TL";
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Veresiye Ücreti : " + String.Format("{0:N2}", aracYazdirs.fVeresiyeToplamTutar) + " TL";
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;


                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Yeni Abone Araç Sayısı : " + String.Format("{0:N2}", aracYazdirs.iYeniAboneArac);
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;



                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Abone Ücreti Alınan Araç Sayısı : " + String.Format("{0:N2}", aracYazdirs.iAboneArac);
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;


                    satir++;
                    rowBackgroundColor = backgroundColor2;
                    if (satir % 2 == 0)
                    {
                        rowBackgroundColor = backgroundColor1;
                    }

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Abone Ücreti Toplam Tutar : " + String.Format("{0:N2}", aracYazdirs.fAbonelikUcreti);
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    int iToplamYikamaSayisi = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.iToplam);
                    double dToplamYikamaUcreti = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.dToplamFiyat);
                    if (iToplamYikamaSayisi > 0 && dToplamYikamaUcreti > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Yıkama Sayısı : " + iToplamYikamaSayisi;
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Yıkama Ücreti : " + String.Format("{0:N2}", dToplamYikamaUcreti) + " TL";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;
                    }

                    int iToplamUrunSayisi = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.iToplam);
                    double dToplamUrunUcreti = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.dToplamFiyat);
                    if (iToplamUrunSayisi > 0 && dToplamUrunUcreti > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Ürün Sayısı : " + iToplamUrunSayisi;
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Ürün Ücreti : " + String.Format("{0:N2}", dToplamUrunUcreti) + " TL";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;
                    }

                    if (aracYazdirs.urunListesis != null && aracYazdirs.urunListesis.Count > 0)
                    {
                        for (int j = 0; j < aracYazdirs.urunListesis.Count; j++)
                        {
                            if (aracYazdirs.urunListesis[j].iToplam > 0)
                            {
                                satir++;
                                rowBackgroundColor = backgroundColor2;
                                if (satir % 2 == 0)
                                {
                                    rowBackgroundColor = backgroundColor1;
                                }

                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                                Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.urunListesis[j].cUrun + " Sayısı : " + aracYazdirs.urunListesis[j].iToplam;
                                Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                row++;

                                satir++;
                                rowBackgroundColor = backgroundColor2;
                                if (satir % 2 == 0)
                                {
                                    rowBackgroundColor = backgroundColor1;
                                }

                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                                Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                                Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.urunListesis[j].cUrun + " Ücreti : " + String.Format("{0:N2}", aracYazdirs.urunListesis[j].dToplamFiyat) + " TL";
                                Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                row++;
                            }
                        }
                    }
                    if (aracYazdirs.AboneCikisYapanAraclar != null && aracYazdirs.AboneCikisYapanAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Abone Çıkış Yapan Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.AboneCikisYapanAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.AboneCikisYapanAraclar[i].ToUpper();
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }
                    if (aracYazdirs.AboneBekleyenAraclar != null && aracYazdirs.AboneBekleyenAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Abone Bekleyen Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.AboneBekleyenAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.AboneBekleyenAraclar[i].ToUpper();
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }
                    if (aracYazdirs.DuzeltmeCikisYapanAraclar != null && aracYazdirs.DuzeltmeCikisYapanAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Düzeltme Çıkış Yapan Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.DuzeltmeCikisYapanAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.DuzeltmeCikisYapanAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }
                    if (aracYazdirs.DuzeltmeBekleyenAraclar != null && aracYazdirs.DuzeltmeBekleyenAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Düzeltme Bekleyen Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.DuzeltmeBekleyenAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.DuzeltmeBekleyenAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }
                    if (aracYazdirs.MisafirCikisYapanAraclar != null && aracYazdirs.MisafirCikisYapanAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Misafir Çıkış Yapan Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.MisafirCikisYapanAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.MisafirCikisYapanAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }
                    if (aracYazdirs.MisafirBekleyenAraclar != null && aracYazdirs.MisafirBekleyenAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Misafir Bekleyen Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.MisafirBekleyenAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.MisafirBekleyenAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }

                    if (aracYazdirs.VeresiyeCikisYapanAraclar != null && aracYazdirs.VeresiyeCikisYapanAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Veresiye Çıkış Yapan Araç Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.VeresiyeCikisYapanAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.VeresiyeCikisYapanAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }

                    if (aracYazdirs.VeresiyeBekleyenAraclar != null && aracYazdirs.VeresiyeBekleyenAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Veresiye Bekleyen Araç Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.VeresiyeBekleyenAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.VeresiyeBekleyenAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }

                    if (aracYazdirs.YeniAboneAraclar != null && aracYazdirs.YeniAboneAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Yeni Abone Araç Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.YeniAboneAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.YeniAboneAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }


                    if (aracYazdirs.AboneUcretiAlinanAraclar != null && aracYazdirs.AboneUcretiAlinanAraclar.Length > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                        Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                        Sheet.Cells[string.Format("A{0}", row)].Value = "Abone Ücreti Alınan Araç Listesi";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        row++;

                        for (int i = 0; i < aracYazdirs.AboneUcretiAlinanAraclar.Length; i++)
                        {
                            satir++;
                            rowBackgroundColor = backgroundColor2;
                            if (satir % 2 == 0)
                            {
                                rowBackgroundColor = backgroundColor1;
                            }

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(rowBackgroundColor);

                            Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("I{0}", row)].Merge = true;
                            Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.AboneUcretiAlinanAraclar[i];
                            Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            row++;
                        }
                    }



                    Sheet.Cells["A:I"].AutoFitColumns();
                    Sheet.DefaultRowHeight = 20;

                    for (int i = 1; i < row; i++)
                    {
                        Sheet.Cells["A" + i + ":I" + i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells["A" + i + ":I" + i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells["A" + i + ":I" + i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells["A" + i + ":I" + i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }


                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment: filename=" + "Rapor.xlsx");
                    Response.BinaryWrite(Ep.GetAsByteArray());
                    Response.End();
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
            }
        }

        public void RaporlarExcelYeni(
            int? sayfaNo,
            string cPlaka,
            int? iKodAracTipi,
            string dGirisTarihi,
            string dCikisTarihi,
            int? iAbonelikDurumu,
            string dAboneBaslangicTarihi,
            string dAboneBitisTarihi,
            int? iDuzeltmeTipi,
            int? iKodUrun2)
        {
            try
            {
                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.dCikisTarihi != Convert.ToDateTime("1900-01-01") &&
                                        table.iAktifMi == 1 &&
                                        table.iKodAracTipi != 4 && // Misafir Dilse
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                     select new Models.AracYeni
                                     {
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                         cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                         iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                         cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                         cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                         cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                         cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                         cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                         lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                         iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                         cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                         iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                         dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                         fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                         fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                         dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                         dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))
                                     }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeIptal = (from table in dc.Aracs
                                          join tableAracTipis in dc.AracTipis
                                             on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                          from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                          join tableMusteri3s in dc.Musteri3s
                                             on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                          from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                          where
                                             table.iKodLokasyon == iKodLokasyonLogin &&
                                             table.iAktifMi == -1 &&
                                             table.iKodAracTipi != 4 && // Misafir Dilse
                                             (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                             (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                          select new Models.AracYeni
                                          {
                                              cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                              cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                              cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                              iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                              cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                              cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                              cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                              iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                              cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                              cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                              iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                              cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                              iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                              dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                              fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                              fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                              fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                              dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                              dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                          }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeBekleyen = (from table in dc.Aracs
                                             join tableAracTipis in dc.AracTipis
                                                on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                             from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                             join tableMusteri3s in dc.Musteri3s
                                                on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                             from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                             where
                                                table.iKodLokasyon == iKodLokasyonLogin &&
                                                table.iAktifMi == 1 &&
                                                table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                                table.iKodAracTipi != 4 && // Misafir Dilse
                                                (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                             select new Models.AracYeni
                                             {
                                                 cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                 cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                 cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                 iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                 cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                 cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                 cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                 iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                 cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                 cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                 iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                 cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                 iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                 dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                 fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                 fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                 dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                 dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                             }).OrderByDescending(x => x.dCikisTarihi).ToList();

                    var listelemeMisafirArac = (from table in dc.Aracs
                                                join tableAracTipis in dc.AracTipis
                                                   on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                                from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                                join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                                from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                                where
                                                   table.iKodLokasyon == iKodLokasyonLogin &&
                                                   table.iAktifMi == 1 &&
                                                   table.iKodAracTipi == 4 && // Misafir Araç
                                                   (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                                   (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                                select new Models.AracYeni
                                                {
                                                    cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                                    cAracTipi = (tableAracTipis != null && tableAracTipis.cAdi != null && tableAracTipis.cAdi != string.Empty ? tableAracTipis.cAdi : "-"),
                                                    cGirisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkSuresi = (table.fOtoparkSuresi != null ? (float)table.fOtoparkSuresi : 0).ToString(),
                                                    iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && table.dAboneBaslangicTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date && table.dAboneBitisTarihi.Value.Date != Convert.ToDateTime("1900-01-01").Date ? 1 : 2),
                                                    cAboneBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cAboneBitisTarihi = String.Format("{0:dd.MM.yyyy}", (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01"))),
                                                    cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0)),
                                                    cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                                    iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                                    cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                                    cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                                    iUrunSilindiMi = (table.iUrunSilindiMi != null ? (int)table.iUrunSilindiMi : 0),
                                                    cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                                    iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                                    dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dCikisTarihi = (table.dCikisTarihi != null ? Convert.ToDateTime(table.dCikisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    fOtoparkUcreti = (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0),
                                                    fDuzeltme = (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0),
                                                    fVeresiye = (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0),
                                                    dAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null ? Convert.ToDateTime(table.dAboneBaslangicTarihi) : Convert.ToDateTime("1900-01-01")),
                                                    dAboneBitisTarihi = (table.dAboneBitisTarihi != null ? Convert.ToDateTime(table.dAboneBitisTarihi) : Convert.ToDateTime("1900-01-01")),
                                                }).OrderByDescending(x => x.dCikisTarihi).ToList();


                    var abonelistesi = (from table in dc.Aboneliks
                                        join tableMusteri3s in dc.Musteri3s
                                                   on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                        from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                        where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.iAktifMi == 1
                                        select new Models.Abonelik
                                        {
                                            dKayitTarihi = (table.dKayitTarihi != null ? Convert.ToDateTime(table.dKayitTarihi) : Convert.ToDateTime("1900-01-01")),
                                            cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                            cOdemeString = (table.cOdeme != null && table.cOdeme.ToString() != string.Empty ? table.cOdeme : string.Empty)
                                        }).ToList();

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        listeleme = listeleme.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.cPlaka == cPlaka).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.cPlaka == cPlaka).ToList();
                    }

                    DateTime dGirisTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dCikisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dGirisTarihi))
                    {
                        dGirisTarihiLocal = Convert.ToDateTime(dGirisTarihi);
                    }
                    if (!String.IsNullOrEmpty(dCikisTarihi))
                    {
                        dCikisTarihiLocal = Convert.ToDateTime(dCikisTarihi);
                    }

                    if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01") || dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dGirisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dGirisTarihi.Date >= dGirisTarihiLocal.Date
                                ).ToList();
                        }

                        if (dCikisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dCikisTarihi.Date <= dCikisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    DateTime dAboneBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                    DateTime dAboneBitisTarihiLocal = Convert.ToDateTime("1900-01-01");

                    if (!String.IsNullOrEmpty(dAboneBaslangicTarihi))
                    {
                        dAboneBaslangicTarihiLocal = Convert.ToDateTime(dAboneBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dAboneBitisTarihi))
                    {
                        dAboneBitisTarihiLocal = Convert.ToDateTime(dAboneBitisTarihi);
                    }

                    if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01") || dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                    {
                        if (dAboneBaslangicTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBaslangicTarihi.Date >= dAboneBaslangicTarihiLocal.Date
                                ).ToList();
                        }

                        if (dAboneBitisTarihiLocal != Convert.ToDateTime("1900-01-01"))
                        {
                            listeleme = listeleme.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            listelemeIptal = listelemeIptal.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            listelemeBekleyen = listelemeBekleyen.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();

                            listelemeMisafirArac = listelemeMisafirArac.Where(
                                model =>
                                model.dAboneBitisTarihi.Date <= dAboneBitisTarihiLocal.Date
                                ).ToList();
                        }
                    }

                    if (iKodAracTipi != null && iKodAracTipi >= 0)
                    {
                        listeleme = listeleme.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iKodAracTipi == iKodAracTipi).ToList();
                    }

                    if (iAbonelikDurumu != null)
                    {
                        listeleme = listeleme.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iAboneMi == iAbonelikDurumu).ToList();
                    }

                    if (iDuzeltmeTipi != null)
                    {
                        listeleme = listeleme.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeIptal = listelemeIptal.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeBekleyen = listelemeBekleyen.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                        listelemeMisafirArac = listelemeMisafirArac.Where(model => model.iDuzeltmeTipi == iDuzeltmeTipi).ToList();
                    }

                    List<Models.UrunJson2> toplamUrunler = new List<Models.UrunJson2>();
                    List<Models.AracYeni> listelemeYeni = new List<Models.AracYeni>();
                    int iSatirNo = 1;
                    for (int i = 0; i < listeleme.Count; i++)
                    {
                        listeleme[i].iSatirNumarasi = iSatirNo++;
                        if (!String.IsNullOrEmpty(listeleme[i].cUrun))
                        {
                            listeleme[i].urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(listeleme[i].cUrun);
                            for (int j = 0; j < listeleme[i].urun2Listesi.Count; j++)
                            {
                                if (iKodUrun2 != null && iKodUrun2 > 0)
                                {
                                    if (iKodUrun2 == listeleme[i].urun2Listesi[j].iKodUrun2)
                                    {
                                        listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                        listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                        toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                        break;
                                    }
                                }
                                else
                                {
                                    listeleme[i].urun2Listesi[j].cUrun = new Models.Urun2().GonderAdi(listeleme[i].urun2Listesi[j].iKodUrun2, iKodLokasyonLogin);
                                    listeleme[i].urun2Listesi[j].cFiyat = string.Format("{0:N2}", Convert.ToDouble(listeleme[i].urun2Listesi[j].cFiyat.Replace(",", "").Replace(".", ",")));
                                    toplamUrunler.Add(listeleme[i].urun2Listesi[j]);
                                }
                            }

                            listelemeYeni.Add(listeleme[i]);
                        }
                        else if (iKodUrun2 == null || iKodUrun2 == 0)
                        {
                            listelemeYeni.Add(listeleme[i]);
                        }
                    }
                    listeleme = listelemeYeni;

                    List<Models.Arac.AracYeniToplaUrun> toplamUrunListesi =
                        toplamUrunler.GroupBy(l => l.iKodUrun2)
                            .Select(cl => new Models.Arac.AracYeniToplaUrun
                            {
                                iKodUrun2 = cl.First().iKodUrun2,
                                iToplam = cl.Sum(c => Convert.ToInt32(c.iAdet)),
                                dToplamFiyat = cl.Sum(c => Convert.ToDouble(c.cFiyat.ToString().Replace(".", ""))),
                            }).ToList();

                    double dToplamUrunler = 0;
                    if (toplamUrunListesi != null)
                    {
                        for (int i = 0; i < toplamUrunListesi.Count; i++)
                        {
                            dToplamUrunler += toplamUrunListesi[i].dToplamFiyat;
                        }
                    }

                    int iAboneArac = 0;
                    double cOdemeler = 0;
                    for (int i = 0; i < abonelistesi.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(abonelistesi[i].cOdemeString))
                        {
                            abonelistesi[i].aboneOdemeTakvimis = JsonConvert.DeserializeObject<List<Models.AboneOdemeTakvimi>>(abonelistesi[i].cOdemeString);

                            for (int j = 0; j < abonelistesi[i].aboneOdemeTakvimis.Count; j++)
                            {
                                if (Convert.ToDateTime(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTarihi).Date == DateTime.Now.Date)
                                {

                                    cOdemeler += Convert.ToDouble(abonelistesi[i].aboneOdemeTakvimis[j].cOdemeTutar.Replace(",", "").Replace(".", ","));
                                }
                                iAboneArac++;
                            }
                        }
                    }

                    int iYeniAboneArac = 0;
                    for (int i = 0; i < abonelistesi.Count; i++)
                    {
                        if (abonelistesi[i].dKayitTarihi.Date != Convert.ToDateTime("1900-01-01") &&
                            abonelistesi[i].dKayitTarihi.Date == DateTime.Now.Date)
                        {
                            iYeniAboneArac++;
                        }
                    }

                    Models.AracYeni.AracYazdir aracYazdirs = new Models.AracYeni.AracYazdir();
                    aracYazdirs.iToplamAracSayisi = listeleme.Count();
                    aracYazdirs.dUrunFiyatToplam = dToplamUrunler;
                    aracYazdirs.fOtoparkToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti);
                    aracYazdirs.fDuzeltmeEksiltmeTutar = listeleme.Where(x => x.iDuzeltmeTipi == 1).Sum(x => x.fDuzeltme);
                    aracYazdirs.fDuzeltmeArtirmaTutar = listeleme.Where(x => x.iDuzeltmeTipi == 2).Sum(x => x.fDuzeltme);
                    aracYazdirs.fOtoparkDuzeltmeliToplamTutar = listeleme.Sum(x => x.fOtoparkUcreti + (x.iDuzeltmeTipi == 1 ? -1 * x.fDuzeltme : 0) + (x.iDuzeltmeTipi == 2 ? x.fDuzeltme : 0));
                    aracYazdirs.fVeresiyeToplamTutar = listeleme.Where(x => x.lVeresiye == true).Sum(x => x.fVeresiye);
                    aracYazdirs.fGenelToplam = (aracYazdirs.fOtoparkToplamTutar + aracYazdirs.fDuzeltmeArtirmaTutar + (float)aracYazdirs.dUrunFiyatToplam) - aracYazdirs.fDuzeltmeEksiltmeTutar;
                    aracYazdirs.fAbonelikUcreti = (float)cOdemeler;
                    aracYazdirs.iAboneArac = iAboneArac;
                    aracYazdirs.iYeniAboneArac = iYeniAboneArac;
                    aracYazdirs.urunListesis = new List<Models.AracYeni.AracYazdir.UrunListesi>();

                    List<Models.Urun2> urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                    for (int i = 0; i < urun2Listesi.Count; i++)
                    {
                        int iToplam = 0;
                        double dToplamFiyat = 0;

                        for (int j = 0; j < toplamUrunListesi.Count; j++)
                        {
                            if (urun2Listesi[i].iKodUrun2 == toplamUrunListesi[j].iKodUrun2)
                            {
                                iToplam = toplamUrunListesi[j].iToplam;
                                dToplamFiyat = toplamUrunListesi[j].dToplamFiyat;
                                break;
                            }
                        }

                        aracYazdirs.urunListesis.Add(new Models.AracYeni.AracYazdir.UrunListesi
                        {
                            cUrun = urun2Listesi[i].cAdi,
                            iToplam = iToplam,
                            dToplamFiyat = dToplamFiyat,
                            lStokTutlacakMi = urun2Listesi[i].lStokTutlacakMi
                        });
                    }

                    aracYazdirs.iToplamBekleyenAracSayisi = listelemeBekleyen.Count();

                    aracYazdirs.iAboneCikisYapanAracSayisi = listeleme.Where(x => x.iAboneMi == 1).Count();
                    aracYazdirs.AboneCikisYapanAraclar = listeleme.Where(x => x.iAboneMi == 1).Select(x => x.cPlaka).ToArray();
                    aracYazdirs.iAboneBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iAboneMi == 1).Count();
                    aracYazdirs.AboneBekleyenAraclar = listelemeBekleyen.Where(x => x.iAboneMi == 1).Select(x => x.cPlaka).ToArray();
                    aracYazdirs.iAboneAracSayisi = aracYazdirs.iAboneCikisYapanAracSayisi + aracYazdirs.iAboneBekleyenAracSayisi;

                    aracYazdirs.iDuzeltmeCikisYapanAracSayisi = listeleme.Where(x => x.iDuzeltmeTipi > 0).Count();
                    aracYazdirs.DuzeltmeCikisYapanAraclar = listeleme.Where(x => x.iDuzeltmeTipi > 0).Select(x => x.cPlaka.ToUpper() + (x.iDuzeltmeTipi == 1 ? " / -" : " / +") + string.Format("{0:N2}", x.fDuzeltme) + " TL / " + x.cAciklama).ToArray();
                    aracYazdirs.iDuzeltmeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Count();
                    aracYazdirs.DuzeltmeBekleyenAraclar = listelemeBekleyen.Where(x => x.iDuzeltmeTipi > 0).Select(x => x.cPlaka.ToUpper() + (x.iDuzeltmeTipi == 1 ? " / -" : " / +") + string.Format("{0:N2}", x.fDuzeltme) + " TL / " + x.cAciklama).ToArray();
                    aracYazdirs.iDuzeltmeAracSayisi = aracYazdirs.iDuzeltmeCikisYapanAracSayisi + aracYazdirs.iDuzeltmeBekleyenAracSayisi;

                    aracYazdirs.iVeresiyeCikisYapanAracSayisi = listeleme.Where(x => x.lVeresiye == true).Count();
                    aracYazdirs.VeresiyeCikisYapanAraclar = listeleme.Where(x => x.lVeresiye == true).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iVeresiyeBekleyenAracSayisi = listelemeBekleyen.Where(x => x.lVeresiye == true).Count();
                    aracYazdirs.VeresiyeBekleyenAraclar = listeleme.Where(x => x.lVeresiye == true).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iVeresiyeAracSayisi = aracYazdirs.iVeresiyeBekleyenAracSayisi + aracYazdirs.iVeresiyeCikisYapanAracSayisi;

                    aracYazdirs.iMisafirCikisYapanAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Count();
                    aracYazdirs.MisafirCikisYapanAraclar = listelemeMisafirArac.Where(x => x.dCikisTarihi != Convert.ToDateTime("1900-01-01")).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iMisafirBekleyenAracSayisi = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Count();
                    aracYazdirs.MisafirBekleyenAraclar = listelemeMisafirArac.Where(x => x.dCikisTarihi == Convert.ToDateTime("1900-01-01")).Select(x => x.cPlaka.ToUpper() + " / " + x.cAciklama).ToArray();
                    aracYazdirs.iMisafirAracSayisi = aracYazdirs.iMisafirCikisYapanAracSayisi + aracYazdirs.iMisafirBekleyenAracSayisi;

                    aracYazdirs.iIptalEdilenAracSayisi = listelemeIptal.Count();

                    ExcelPackage Ep = new ExcelPackage();
                    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

                    Sheet.Cells["A:B"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    Sheet.Cells["A:B"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                    Color backgroundColor = System.Drawing.ColorTranslator.FromHtml("#195595");
                    Color backgroundColor1 = System.Drawing.ColorTranslator.FromHtml("#F1F1F1");
                    Color backgroundColor2 = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    Color textColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    Sheet.Cells["A1:B1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A1:B1"].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    Sheet.Cells["A1:B1"].Style.Font.Color.SetColor(textColor);
                    Sheet.Cells["A1:B1"].Style.Font.Bold = true;



                    int row = 1;
                    int satir = 0;
                    Color rowBackgroundColor = backgroundColor2;

                    Sheet.Cells["A2"].Value = "deneme1";
                    Sheet.Cells["B2"].Value = "deneme1";

                    Sheet.Cells[string.Format("A{0}", row) + ":" + string.Format("B{0}", row)].Merge = true;
                    Sheet.Cells[string.Format("A{0}", row)].Value = "Raporlar";
                    Sheet.Cells[string.Format("A{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[string.Format("A{0}", row)].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    Sheet.Cells[string.Format("A{0}", row)].Style.Font.Color.SetColor(textColor);
                    Sheet.Cells[string.Format("A{0}", row)].Style.Font.Bold = true;
                    Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    row++;

                    Sheet.Cells["A2"].Value = "Toplam Çıkış Yapan Sayısı";
                    Sheet.Cells[string.Format("A2", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B2"].Value = aracYazdirs.iToplamAracSayisi;
                    Sheet.Cells["A3"].Value = "Toplam Bekleyen Sayısı";
                    Sheet.Cells[string.Format("A3", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B3"].Value = aracYazdirs.iToplamBekleyenAracSayisi;
                    Sheet.Cells["A4"].Value = "Toplam Abone Sayısı";
                    Sheet.Cells[string.Format("A4", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B4"].Value = aracYazdirs.iAboneAracSayisi;
                    Sheet.Cells["A5"].Value = "Toplam Çıkış Yapan Abone Sayısı";
                    Sheet.Cells[string.Format("A5", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B5"].Value = aracYazdirs.iAboneCikisYapanAracSayisi;
                    Sheet.Cells["A6"].Value = "Toplam Bekleyen Abone Sayısı";
                    Sheet.Cells[string.Format("A6", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B6"].Value = aracYazdirs.iAboneBekleyenAracSayisi;
                    Sheet.Cells["A7"].Value = "Toplam Düzeltme Sayısı";
                    Sheet.Cells[string.Format("A7", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B7"].Value = aracYazdirs.iDuzeltmeAracSayisi;
                    Sheet.Cells["A8"].Value = "Toplam Çıkış Yapan Düzeltme Sayısı";
                    Sheet.Cells[string.Format("A8", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B8"].Value = aracYazdirs.iDuzeltmeCikisYapanAracSayisi;
                    Sheet.Cells["A9"].Value = "Toplam Bekleyen Düzeltme Sayısı ";
                    Sheet.Cells[string.Format("A9", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B9"].Value = aracYazdirs.iDuzeltmeBekleyenAracSayisi;
                    Sheet.Cells["A10"].Value = "Toplam Veresiye Sayısı ";
                    Sheet.Cells[string.Format("A10", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B10"].Value = aracYazdirs.iVeresiyeAracSayisi;
                    Sheet.Cells["A11"].Value = "Toplam Çıkış Yapan Veresiye Sayısı ";
                    Sheet.Cells[string.Format("A11", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B11"].Value = aracYazdirs.iVeresiyeCikisYapanAracSayisi;
                    Sheet.Cells["A12"].Value = "Toplam Bekleyen Veresiye Sayısı";
                    Sheet.Cells[string.Format("A12", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B12"].Value = aracYazdirs.iVeresiyeBekleyenAracSayisi;
                    Sheet.Cells["A13"].Value = "Toplam Misafir Sayısı ";
                    Sheet.Cells[string.Format("A13", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B13"].Value = aracYazdirs.iMisafirAracSayisi;
                    Sheet.Cells["A14"].Value = "Toplam Çıkış Yapan Misafir Sayısı  ";
                    Sheet.Cells[string.Format("A14", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B14"].Value = aracYazdirs.iMisafirCikisYapanAracSayisi;
                    Sheet.Cells["A15"].Value = "Toplam Bekleyen Misafir Sayısı ";
                    Sheet.Cells[string.Format("A15", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B15"].Value = aracYazdirs.iMisafirBekleyenAracSayisi;
                    Sheet.Cells["A16"].Value = "Toplam İptal Edilen Sayısı  ";
                    Sheet.Cells[string.Format("A16", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B16"].Value = aracYazdirs.iIptalEdilenAracSayisi;
                    Sheet.Cells["A17"].Value = "Genel Otopark Toplam Ücreti  ";
                    Sheet.Cells[string.Format("A17", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B17"].Value = aracYazdirs.fGenelToplam + " TL";
                    Sheet.Cells["A18"].Value = "Toplam Otopark Ücreti (Düzeltmesiz)";
                    Sheet.Cells[string.Format("A18", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B18"].Value = aracYazdirs.fOtoparkToplamTutar + " TL";
                    Sheet.Cells["A19"].Value = "Toplam Otopark Ücreti (Düzeltmeli)";
                    Sheet.Cells[string.Format("A19", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B19"].Value = aracYazdirs.fOtoparkDuzeltmeliToplamTutar + " TL";
                    Sheet.Cells["A20"].Value = "Toplam Düzeltme (Eksiltme) Ücreti";
                    Sheet.Cells[string.Format("A20", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B20"].Value = aracYazdirs.fDuzeltmeEksiltmeTutar + " TL";
                    Sheet.Cells["A21"].Value = "Toplam Düzeltme (Artırma) Ücreti";
                    Sheet.Cells[string.Format("A21", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B21"].Value = aracYazdirs.fDuzeltmeArtirmaTutar + " TL";
                    Sheet.Cells["A22"].Value = "Toplam Veresiye Ücreti";
                    Sheet.Cells[string.Format("A22", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B22"].Value = aracYazdirs.fVeresiyeToplamTutar + " TL";
                    Sheet.Cells["A23"].Value = "Yeni Abone Olan Araç Sayısı";
                    Sheet.Cells[string.Format("A23", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B23"].Value = aracYazdirs.iYeniAboneArac;
                    Sheet.Cells["A24"].Value = "Abone Ücreti Alınan Araç Sayısı";
                    Sheet.Cells[string.Format("A24", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B24"].Value = aracYazdirs.iAboneArac;
                    Sheet.Cells["A25"].Value = " Abone Ücreti Toplam Tutar";
                    Sheet.Cells[string.Format("A25", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    Sheet.Cells["B25"].Value = aracYazdirs.fAbonelikUcreti + "TL";



                    int iToplamYikamaSayisi = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.iToplam);
                    double dToplamYikamaUcreti = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == false).Sum(x => x.dToplamFiyat);
                    if (iToplamYikamaSayisi > 0 && dToplamYikamaUcreti > 0)
                    {
                        row = 23;
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Yıkama Sayısı : ";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        Sheet.Cells[string.Format("B{0}", row)].Value = iToplamYikamaSayisi;
                        row++;

                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Yıkama Ücreti : ";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        Sheet.Cells[string.Format("B{0}", row)].Value = String.Format("{0:N2}", dToplamYikamaUcreti) + " TL";
                        row++;
                    }

                    int iToplamUrunSayisi = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.iToplam);
                    double dToplamUrunUcreti = aracYazdirs.urunListesis.Where(x => x.lStokTutlacakMi == true).Sum(x => x.dToplamFiyat);
                    if (iToplamUrunSayisi > 0 && dToplamUrunUcreti > 0)
                    {
                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }


                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Ürün Sayısı  ";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        Sheet.Cells[string.Format("B{0}", row)].Value = iToplamUrunSayisi;
                        row++;

                        satir++;
                        rowBackgroundColor = backgroundColor2;
                        if (satir % 2 == 0)
                        {
                            rowBackgroundColor = backgroundColor1;
                        }

                        Sheet.Cells[string.Format("A{0}", row)].Value = "Toplam Ürün Ücreti  ";
                        Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        Sheet.Cells[string.Format("B{0}", row)].Value = String.Format("{0:N2}", dToplamUrunUcreti) + " TL";
                        row++;
                    }

                    if (aracYazdirs.urunListesis != null && aracYazdirs.urunListesis.Count > 0)
                    {
                        for (int j = 0; j < aracYazdirs.urunListesis.Count; j++)
                        {
                            if (aracYazdirs.urunListesis[j].iToplam > 0)
                            {
                                satir++;
                                rowBackgroundColor = backgroundColor2;
                                if (satir % 2 == 0)
                                {
                                    rowBackgroundColor = backgroundColor1;
                                }

                                Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.urunListesis[j].cUrun + " Sayısı : ";
                                Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                Sheet.Cells[string.Format("B{0}", row)].Value = aracYazdirs.urunListesis[j].iToplam;
                                row++;

                                satir++;
                                rowBackgroundColor = backgroundColor2;
                                if (satir % 2 == 0)
                                {
                                    rowBackgroundColor = backgroundColor1;
                                }

                                Sheet.Cells[string.Format("A{0}", row)].Value = aracYazdirs.urunListesis[j].cUrun + " Ücreti : ";
                                Sheet.Cells[string.Format("A{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                Sheet.Cells[string.Format("B{0}", row)].Value = String.Format("{0:N2}", aracYazdirs.urunListesis[j].dToplamFiyat) + " TL";
                                row++;
                            }
                        }
                    }

                    Sheet.Cells["A:B"].AutoFitColumns();
                    Sheet.DefaultRowHeight = 20;

                    for (int i = 1; i < row; i++)
                    {
                        Sheet.Cells["A" + i + ":B" + i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells["A" + i + ":B" + i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells["A" + i + ":B" + i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells["A" + i + ":B" + i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }


                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment: filename=" + "Rapor.xlsx");
                    Response.BinaryWrite(Ep.GetAsByteArray());
                    Response.End();
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
            }
        }

        [HttpGet]
        public ActionResult BekleyenListeleYeni(string cPlaka)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 133))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                        table.iAktifMi == 1 &&
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                     select new Models.AracYeni
                                     {
                                         iKodArac = table.iKodArac,
                                         cResim = (tableAracTipis != null && tableAracTipis.cFotograf != null && tableAracTipis.cFotograf.ToString() != string.Empty ? tableAracTipis.cFotograf : string.Empty),
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                         iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? 1 : 2),
                                         iKodAracTipi = (table.iKodAracTipi != null && table.iKodAracTipi > 0 ? (int)table.iKodAracTipi : 0),
                                         dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                         lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                         iStoksuzUrun = 0
                                     }).OrderByDescending(x => x.iKodArac).ToList();

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        string cPlakaLocal = cPlaka.Replace("*", "");
                        var plakaArama = listeleme.Where(model => model.cPlaka.ToLower() == cPlakaLocal.ToLower()).FirstOrDefault();
                        if (plakaArama != null)
                        {
                            return Redirect("/Arac/GuncelleYeni/" + plakaArama.iKodArac + "/1");
                        }
                    }

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        listeleme = listeleme.Where(x => x.cPlaka.Contains(cPlaka)).ToList();
                    }

                    if (listeleme != null && listeleme.Count > 0)
                    {
                        for (int i = 0; i < listeleme.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(listeleme[i].cResim))
                            {
                                listeleme[i].resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(listeleme[i].cResim);
                                if (listeleme[i].resimListesi.Count > 0 && !String.IsNullOrEmpty(listeleme[i].resimListesi[0].cKucukResim))
                                {
                                    listeleme[i].cResim = "/Files/th-" + listeleme[i].resimListesi[0].cKucukResim.Replace("th-", "");
                                }
                                else
                                {
                                    listeleme[i].cResim = "/Images/no-image.jpg";
                                }
                            }

                            if (!String.IsNullOrEmpty(listeleme[i].cUrun))
                            {
                                listeleme[i].urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(listeleme[i].cUrun);

                                if (listeleme[i].urun2Listesi != null)
                                {
                                    for (int j = 0; j < listeleme[i].urun2Listesi.Count; j++)
                                    {
                                        if (listeleme[i].urun2Listesi[j].iStokTutlacakMi == 0)
                                        {
                                            listeleme[i].iStoksuzUrun = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.cToplamAracSayisi = listeleme.Count();
                    ViewBag.cToplamAboneAracSayisi = listeleme.Where(x => x.iAboneMi == 1).Count();
                    ViewBag.cToplamMisafirAracSayisi = listeleme.Where(x => x.iKodAracTipi == 4).Count();
                    ViewBag.cToplamDuzeltmeYapilanAracSayisi = listeleme.Where(x => x.iDuzeltmeTipi != 0).Count();
                    ViewBag.cToplamYikamaYapilanAracSayisi = listeleme.Where(x => x.iStoksuzUrun == 1).Count();
                    ViewBag.cToplamVeresiyeAracSayisi = listeleme.Where(x => x.lVeresiye == true).Count();

                    return View(listeleme);

                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                return View();
            }
        }

        [HttpGet]
        public ActionResult BekleyenSilYeni(string id)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 133))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodArac = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var silme = (from table in dc.Aracs
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           table.iAktifMi == 1
                                         select table).FirstOrDefault();

                            if (silme != null && silme.dGirisTarihi >= DateTime.Now.AddMinutes(-3))
                            {
                                silme.iAktifMi = -1;
                                silme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                silme.dTarih = DateTime.Now;
                                dc.SubmitChanges();
                            }
                        }
                    }
                }

                return Redirect("/Arac/BekleyenListeleYeni");
            }
            catch (Exception Ex)
            {
                return Redirect("/Arac/BekleyenListeleYeni");
            }
        }


        [HttpGet]
        public ActionResult VeresiyeListeleYeni(string cPlaka)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 135))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Aracs
                                     join tableAracTipis in dc.AracTipis
                                        on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                     from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                     join tableMusteri3s in dc.Musteri3s
                                        on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                     from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                     where
                                        table.iKodLokasyon == iKodLokasyonLogin &&
                                        (table.fVeresiye != null && table.fVeresiye > 0.00) &&
                                        table.iAktifMi == 1 &&
                                        (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                        (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                     select new Models.AracYeni
                                     {
                                         iKodArac = table.iKodArac,
                                         cResim = (tableAracTipis != null && tableAracTipis.cFotograf != null && tableAracTipis.cFotograf.ToString() != string.Empty ? tableAracTipis.cFotograf : string.Empty),
                                         cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                         iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                         iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? 1 : 2),
                                         iKodAracTipi = (table.iKodAracTipi != null && table.iKodAracTipi > 0 ? (int)table.iKodAracTipi : 0),
                                         dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")),
                                         cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                         lVeresiye = (table.fVeresiye != null && table.fVeresiye > 0.00 ? true : false),
                                         iStoksuzUrun = 0
                                     }).OrderByDescending(x => x.iKodArac).ToList();

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        string cPlakaLocal = cPlaka.Replace("*", "");
                        var plakaArama = listeleme.Where(model => model.cPlaka.ToLower() == cPlakaLocal.ToLower()).FirstOrDefault();
                        if (plakaArama != null)
                        {
                            return Redirect("/Arac/GuncelleYeni/" + plakaArama.iKodArac + "/1");
                        }
                    }

                    if (!String.IsNullOrEmpty(cPlaka))
                    {
                        listeleme = listeleme.Where(x => x.cPlaka.Contains(cPlaka)).ToList();
                    }

                    if (listeleme != null && listeleme.Count > 0)
                    {
                        for (int i = 0; i < listeleme.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(listeleme[i].cResim))
                            {
                                listeleme[i].resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(listeleme[i].cResim);
                                if (listeleme[i].resimListesi.Count > 0 && !String.IsNullOrEmpty(listeleme[i].resimListesi[0].cKucukResim))
                                {
                                    listeleme[i].cResim = "/Files/th-" + listeleme[i].resimListesi[0].cKucukResim.Replace("th-", "");
                                }
                                else
                                {
                                    listeleme[i].cResim = "/Images/no-image.jpg";
                                }
                            }

                            if (!String.IsNullOrEmpty(listeleme[i].cUrun))
                            {
                                listeleme[i].urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(listeleme[i].cUrun);

                                if (listeleme[i].urun2Listesi != null)
                                {
                                    for (int j = 0; j < listeleme[i].urun2Listesi.Count; j++)
                                    {
                                        if (listeleme[i].urun2Listesi[j].iStokTutlacakMi == 0)
                                        {
                                            listeleme[i].iStoksuzUrun = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.cToplamAracSayisi = listeleme.Count();
                    ViewBag.cToplamAboneAracSayisi = listeleme.Where(x => x.iAboneMi == 1).Count();
                    ViewBag.cToplamMisafirAracSayisi = listeleme.Where(x => x.iKodAracTipi == 4).Count();
                    ViewBag.cToplamDuzeltmeYapilanAracSayisi = listeleme.Where(x => x.iDuzeltmeTipi != 0).Count();
                    ViewBag.cToplamYikamaYapilanAracSayisi = listeleme.Where(x => x.iStoksuzUrun == 1).Count();
                    ViewBag.cToplamVeresiyeAracSayisi = listeleme.Where(x => x.lVeresiye == true).Count();

                    return View(listeleme);

                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                return View();
            }
        }


        [HttpGet]
        public ActionResult EkleYeni()
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 133))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;
                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "EkleYeni_Get", Ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult EkleYeni(Models.AracYeni arac, int? sayfaNo, string siralamaSekli, string arama, string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 133))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                if (!String.IsNullOrEmpty(arac.cPlaka) && arac.cPlaka.IndexOf("*") != -1)
                {
                    string cPlakaLocal = arac.cPlaka.Replace("*", "");

                    using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                    {
                        var plakaArama = (from table in dc.Aracs
                                          join tableAracTipis in dc.AracTipis
                                             on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                          from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                          join tableMusteri3s in dc.Musteri3s
                                             on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                          from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                          where
                                             (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty && tableMusteri3s.cPlaka.ToLower() == cPlakaLocal.ToLower()) &&
                                             table.iKodLokasyon == iKodLokasyonLogin &&
                                             table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                             table.iAktifMi == 1 &&
                                             (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                             (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1)
                                          select new Models.AracYeni
                                          {
                                              iKodArac = table.iKodArac,
                                              cResim = (tableAracTipis != null && tableAracTipis.cFotograf != null && tableAracTipis.cFotograf.ToString() != string.Empty ? tableAracTipis.cFotograf : string.Empty),
                                              cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : "-"),
                                              iDuzeltmeTipi = (table.iDuzeltmeTipi != null && table.iDuzeltmeTipi > 0 ? (int)table.iDuzeltmeTipi : 0),
                                              iAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? 1 : 2),
                                              iKodAracTipi = (table.iKodAracTipi != null && table.iKodAracTipi > 0 ? (int)table.iKodAracTipi : 0),
                                              dGirisTarihi = (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01"))
                                          }).FirstOrDefault();

                        if (plakaArama != null)
                        {
                            return Redirect("/Arac/GuncelleYeni/" + plakaArama.iKodArac + "/1");
                        }
                    }
                }

                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;
                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                string cUrun2Listesi = string.Empty;
                if (!String.IsNullOrEmpty(arac.cUrun2Listesi))
                {
                    string[] cUrunler = arac.cUrun2Listesi.Split('|');
                    if (cUrunler.Length > 0)
                    {
                        for (int i = 0; i < cUrunler.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(cUrun2Listesi))
                            {
                                cUrun2Listesi += ",";
                            }

                            string[] cUrun = cUrunler[i].Split('*');

                            if (String.IsNullOrEmpty(cUrun[0]))
                            {
                                cUrun[0] = "";
                            }

                            if (String.IsNullOrEmpty(cUrun[1]))
                            {
                                cUrun[1] = "0";
                            }

                            if (String.IsNullOrEmpty(cUrun[2]))
                            {
                                cUrun[2] = "0";
                            }

                            if (String.IsNullOrEmpty(cUrun[2]))
                            {
                                cUrun[3] = "0";
                            }

                            if (String.IsNullOrEmpty(cUrun[2]))
                            {
                                cUrun[4] = "0";
                            }

                            cUrun2Listesi += "{\"cKodu\":\"" + cUrun[0] + "\",\"iKodUrun2\":\"" + cUrun[1] + "\",\"iAdet\":\"" + cUrun[2] + "\",\"cBirimFiyati\":\"" + cUrun[3] + "\",\"cFiyat\":\"" + cUrun[4] + "\"}";
                        }

                        cUrun2Listesi = "[" + cUrun2Listesi + "]";
                    }

                    if (!String.IsNullOrEmpty(cUrun2Listesi))
                    {
                        arac.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(cUrun2Listesi);

                        if (arac.urun2Listesi != null)
                        {
                            for (int i = 0; i < arac.urun2Listesi.Count; i++)
                            {
                                if (String.IsNullOrEmpty(arac.urun2Listesi[i].cKodu) || arac.urun2Listesi[i].iKodUrun2 == 0 || arac.urun2Listesi[i].iAdet == 0 || String.IsNullOrEmpty(arac.urun2Listesi[i].cBirimFiyati) || arac.urun2Listesi[i].cBirimFiyati == "0.00" || String.IsNullOrEmpty(arac.urun2Listesi[i].cFiyat) || arac.urun2Listesi[i].cFiyat == "0.00")
                                {
                                    ModelState.AddModelError("cUrun2Listesi", "Lütfen bu alanı doldurun!");
                                }
                            }
                        }
                    }
                }

                if (arac.iKodAracTipi == 4 && String.IsNullOrEmpty(arac.cAciklama)) // Misafir Araç
                {
                    ModelState.AddModelError("cAciklama", "Lütfen bu alanı doldurun!");
                }

                if (ModelState.IsValid)
                {
                    using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                    {
                        int iKodMusteri3 = 0;
                        DateTime dAbonelikBaslangicTarihi = Convert.ToDateTime("1900-01-01");
                        DateTime dAbonelikBitisTarihi = Convert.ToDateTime("1900-01-01");
                        string cPlaka = string.Empty;
                        if (!String.IsNullOrEmpty(arac.cPlaka))
                        {
                            cPlaka = arac.cPlaka;
                            var musteri3Kontrol = (from table in dc.Musteri3s
                                                   where
                                                     table.cPlaka == arac.cPlaka &&
                                                     table.iAktifMi == 1
                                                   select table).FirstOrDefault();

                            if (musteri3Kontrol == null)
                            {
                                Data.Musteri3 musteri3 = new Data.Musteri3();
                                musteri3.cPlaka = arac.cPlaka;
                                musteri3.cFotograf = string.Empty;
                                musteri3.cFirmaAdi = string.Empty;
                                musteri3.cAdi = string.Empty;
                                musteri3.cSoyadi = string.Empty;
                                musteri3.cAdres = string.Empty;
                                musteri3.iKodIlce = 0;
                                musteri3.iKodMahalle = 0;
                                musteri3.iKodSehir = 0;
                                musteri3.cEnlem = string.Empty;
                                musteri3.cBoylam = string.Empty;
                                musteri3.cTelefon = string.Empty;
                                musteri3.cFaks = string.Empty;
                                musteri3.cEMail = string.Empty;
                                musteri3.cWeb = string.Empty;
                                musteri3.cVergiDairesi = string.Empty;
                                musteri3.cVergiNumarasi = string.Empty;
                                musteri3.iAktifMi = 1;
                                musteri3.dTarih = DateTime.Now;
                                musteri3.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                dc.Musteri3s.InsertOnSubmit(musteri3);
                                dc.SubmitChanges();
                                iKodMusteri3 = musteri3.iKodMusteri3;

                                var cikisKontrol = (from table in dc.Aracs
                                                    where
                                                       table.iKodMusteri3 == iKodMusteri3 &&
                                                       table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                                       table.iAktifMi == 1
                                                    select table).FirstOrDefault();

                                if (cikisKontrol != null)
                                {
                                    ViewBag.iSonuc = -3;
                                }
                            }
                            else
                            {
                                iKodMusteri3 = musteri3Kontrol.iKodMusteri3;
                                cPlaka = musteri3Kontrol.cPlaka;

                                var cikisKontrol = (from table in dc.Aracs
                                                    where
                                                       table.iKodMusteri3 == iKodMusteri3 &&
                                                       table.dCikisTarihi.Value.Date == Convert.ToDateTime("1900-01-01") &&
                                                       table.iAktifMi == 1
                                                    select table).FirstOrDefault();

                                if (cikisKontrol == null)
                                {
                                    var aboneKontrol =
                                        (from table in dc.Aboneliks
                                         where
                                           table.iKodMusteri3 == iKodMusteri3 &&
                                           table.dBaslangicTarihi.Value.Date <= DateTime.Now.Date &&
                                           table.dBitisTarihi.Value.Date >= DateTime.Now.Date &&
                                           table.iAktifMi == 1
                                         select table).FirstOrDefault();

                                    if (aboneKontrol != null)
                                    {
                                        dAbonelikBaslangicTarihi = Convert.ToDateTime(aboneKontrol.dBaslangicTarihi);
                                        dAbonelikBitisTarihi = Convert.ToDateTime(aboneKontrol.dBitisTarihi);
                                    }
                                }
                                else
                                {
                                    ViewBag.iSonuc = -3;
                                }
                            }
                        }

                        if (ViewBag.iSonuc == null || ViewBag.iSonuc != -3)
                        {
                            Data.Arac yenikayit = new Data.Arac();
                            yenikayit.iKodLokasyon = iKodLokasyonLogin;
                            yenikayit.iKodMusteri3 = iKodMusteri3;
                            yenikayit.iKodAracTipi = arac.iKodAracTipi;
                            yenikayit.cAciklama = arac.cAciklama;
                            yenikayit.cUrun = cUrun2Listesi;
                            yenikayit.dGirisTarihi = DateTime.Now;
                            yenikayit.dCikisTarihi = Convert.ToDateTime("1900-01-01");
                            yenikayit.fOtoparkSuresi = 0;
                            yenikayit.fOtoparkUcreti = 0;
                            yenikayit.fDuzeltme = 0;
                            yenikayit.iDuzeltmeTipi = 0;
                            yenikayit.fTutar = 0;
                            yenikayit.iKDVTuru = 0;
                            yenikayit.iKDVOrani = 0;
                            yenikayit.fGenelTutar = 0;
                            yenikayit.iUrunSilindiMi = 0;
                            yenikayit.dTarih = DateTime.Now;
                            yenikayit.iAktifMi = 1;
                            yenikayit.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                            yenikayit.dAboneBaslangicTarihi = dAbonelikBaslangicTarihi;
                            yenikayit.dAboneBitisTarihi = dAbonelikBitisTarihi;
                            dc.Aracs.InsertOnSubmit(yenikayit);
                            dc.SubmitChanges();

                            if (arac.iKodAracTipi != 4) // Misafir Değilse
                            {
                                BarkodYazdirYeni(
                                    yenikayit.iKodArac,
                                    cPlaka,
                                    (int)yenikayit.iKodAracTipi,
                                    Convert.ToDateTime(yenikayit.dGirisTarihi));
                            }

                            ViewBag.iSonuc = 1;
                            return Redirect("/Arac/EkleYeni");
                        }
                    }
                }
                else
                {
                    ViewBag.iSonuc = -1;
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
            }

            return View(arac);
        }

        public void BarkodYazdirYeni(int iKodArac, string cPlaka, int iKodAracTipi, DateTime dGirisTarihi)
        {
            cPlakaLocal = cPlaka;
            cAracTipiLocal = new Models.AracTipi().GonderAdi(iKodAracTipi);
            cGirisTarihiLocal = dGirisTarihi.ToString("dd MMMM yyyy, dddd, HH:mm");

            var doc = new PrintDocument();
            //doc.PrinterSettings.PrinterName = "Microsoft Print To PDF";
            doc.PrinterSettings.PrinterName = "Hoin-58-Series";
            doc.PrintPage += new PrintPageEventHandler(ProvideContent);
            doc.Print();
        }

        public ActionResult YenidenBarkodYazdirYeni(int iKodArac, string cPlaka, int iKodAracTipi, DateTime dGirisTarihi)
        {
            cPlakaLocal = cPlaka;
            cAracTipiLocal = new Models.AracTipi().GonderAdi(iKodAracTipi);
            cGirisTarihiLocal = dGirisTarihi.ToString("dd MMMM yyyy, dddd, HH:mm");

            var doc = new PrintDocument();
            //doc.PrinterSettings.PrinterName = "Microsoft Print To PDF";
            doc.PrinterSettings.PrinterName = "Hoin-58-Series";
            doc.PrintPage += new PrintPageEventHandler(ProvideContent);
            doc.Print();

            return Redirect("/Arac/BekleyenListeleYeni");
        }

        [HttpGet]
        public ActionResult GuncelleYeni(string id, string id2)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 133))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaTipi = id2;
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;
                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(id2))
                {
                    int iKodArac = 0;
                    int iSayfaTipi = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0 && int.TryParse(id2, out iSayfaTipi) && iSayfaTipi > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           table.iAktifMi == 1 &&
                                           (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                           (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1) &&
                                           (tableKullanici2s != null && tableKullanici2s.iAktifMi == 1)
                                         select new Models.AracYeni
                                         {
                                             iKodArac = table.iKodArac,
                                             iSayfaTipi = iSayfaTipi,
                                             iPostTipi = 0,
                                             cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : string.Empty),
                                             cGenelTutar = string.Format("{0:N2}", new Models.AracYeni().AracGenelTutar(table.cUrun, (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0), (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0), (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0), (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")), DateTime.Now, iKodLokasyonLogin, (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? 1 : 2), (int)table.iKodAracTipi)),
                                             cAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? "Evet" : "Hayır"),
                                             cAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null && table.dAboneBaslangicTarihi != Convert.ToDateTime("1900-01-01") ? String.Format("{0:dd.MM.yyyy}", Convert.ToDateTime(table.dAboneBaslangicTarihi)) : "-"),
                                             cAboneBitisTarihi = (table.dAboneBaslangicTarihi != null && table.dAboneBaslangicTarihi != Convert.ToDateTime("1900-01-01") ? String.Format("{0:dd.MM.yyyy}", Convert.ToDateTime(table.dAboneBitisTarihi)) : "-"),
                                             iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                             cGirisTarihi = (table.dGirisTarihi != null && table.dGirisTarihi != Convert.ToDateTime("1900-01-01") ? String.Format("{0:dd.MM.yyyy, HH:mm}", Convert.ToDateTime(table.dGirisTarihi)) : "-"),
                                             cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", DateTime.Now),
                                             dCikisTarihi = DateTime.Now,
                                             cOtoparkSuresi = new Models.AracYeni().AracOtoparkSuresi((table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")), DateTime.Now).ToString(),
                                             iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                             cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                             cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                             cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                             cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                             cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0))
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cUrun))
                                {
                                    okuma.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(okuma.cUrun);
                                    string cUrunler = string.Empty;
                                    for (int i = 0; i < okuma.urun2Listesi.Count; i++)
                                    {
                                        okuma.urun2Listesi[i].iStokTutlacakMi = new Models.Urun2().GonderStokTutulacakMi(okuma.urun2Listesi[i].iKodUrun2, iKodLokasyonLogin);

                                        if (!String.IsNullOrEmpty(cUrunler))
                                        {
                                            cUrunler += "|";
                                        }

                                        cUrunler += okuma.urun2Listesi[i].cKodu + "*" + okuma.urun2Listesi[i].iKodUrun2 + "*" + okuma.urun2Listesi[i].iAdet + "*" + okuma.urun2Listesi[i].cBirimFiyati + "*" + okuma.urun2Listesi[i].cFiyat;
                                    }
                                    okuma.cUrun2Listesi = cUrunler;
                                }
                            }

                            return View(okuma);
                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -2;
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
            }

            return View();
        }

        [HttpPost]
        public ActionResult GuncelleYeni(string id, string id2, Models.AracYeni arac)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 133))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaTipi = id2;
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;
                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                if (arac != null && arac.iKodArac > 0)
                {
                    string cUrun2Listesi = string.Empty;
                    if (!String.IsNullOrEmpty(arac.cUrun2Listesi))
                    {
                        string[] cUrunler = arac.cUrun2Listesi.Split('|');
                        if (cUrunler.Length > 0)
                        {
                            for (int i = 0; i < cUrunler.Length; i++)
                            {
                                if (!String.IsNullOrEmpty(cUrun2Listesi))
                                {
                                    cUrun2Listesi += ",";
                                }

                                string[] cUrun = cUrunler[i].Split('*');

                                if (String.IsNullOrEmpty(cUrun[0]))
                                {
                                    cUrun[0] = "";
                                }

                                if (String.IsNullOrEmpty(cUrun[1]))
                                {
                                    cUrun[1] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[2] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[3] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[4] = "0";
                                }

                                cUrun2Listesi += "{\"cKodu\":\"" + cUrun[0] + "\",\"iKodUrun2\":\"" + cUrun[1] + "\",\"iAdet\":\"" + cUrun[2] + "\",\"cBirimFiyati\":\"" + cUrun[3] + "\",\"cFiyat\":\"" + cUrun[4] + "\"}";
                            }

                            cUrun2Listesi = "[" + cUrun2Listesi + "]";
                        }

                        if (!String.IsNullOrEmpty(cUrun2Listesi))
                        {
                            arac.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(cUrun2Listesi);

                            if (arac.urun2Listesi != null)
                            {
                                for (int i = 0; i < arac.urun2Listesi.Count; i++)
                                {
                                    if (String.IsNullOrEmpty(arac.urun2Listesi[i].cKodu) || arac.urun2Listesi[i].iKodUrun2 == 0 || arac.urun2Listesi[i].iAdet == 0 || String.IsNullOrEmpty(arac.urun2Listesi[i].cBirimFiyati) || arac.urun2Listesi[i].cBirimFiyati == "0.00" || String.IsNullOrEmpty(arac.urun2Listesi[i].cFiyat) || arac.urun2Listesi[i].cFiyat == "0.00")
                                    {
                                        ModelState.AddModelError("cUrun2Listesi", "Lütfen bu alanı doldurun!");
                                    }
                                }
                            }
                        }
                    }

                    if (arac.iDuzeltmeTipi > 0 && String.IsNullOrEmpty(arac.cAciklama))
                    {
                        ModelState.AddModelError("cAciklama", "Lütfen bu alanı doldurun!");
                    }

                    if (arac.iDuzeltmeTipi > 0 && (String.IsNullOrEmpty(arac.cDuzeltme) || Convert.ToDouble(arac.cDuzeltme.ToString().Replace(".", ",")) == 0))
                    {
                        ModelState.AddModelError("cDuzeltme", "Lütfen bu alanı doldurun!");
                    }

                    if ((arac.iDuzeltmeTipi == null || arac.iDuzeltmeTipi == 0) && Convert.ToDouble(arac.cDuzeltme.ToString().Replace(".", ",")) > 0)
                    {
                        ModelState.AddModelError("iDuzeltmeTipi", "Lütfen bu alanı doldurun!");
                    }

                    if (arac.iKodAracTipi == 4 && String.IsNullOrEmpty(arac.cAciklama)) // Misafir Araç
                    {
                        ModelState.AddModelError("cAciklama", "Lütfen bu alanı doldurun!");
                    }
                    if (String.IsNullOrEmpty(arac.cVeresiye) && String.IsNullOrEmpty(arac.cAciklama))
                    {
                        ModelState.AddModelError("cAciklama", "Lütfen bu alanı doldurun!");
                    }

                    if (ModelState.IsValid)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var guncelleme = (from table in dc.Aracs
                                              where
                                                table.iKodArac == arac.iKodArac &&
                                                table.iAktifMi == 1
                                              select table).FirstOrDefault();
                            if (guncelleme != null)
                            {
                                guncelleme.iKodAracTipi = arac.iKodAracTipi;
                                guncelleme.cUrun = cUrun2Listesi;
                                guncelleme.fOtoparkSuresi = Convert.ToInt32(arac.cOtoparkSuresi);
                                guncelleme.iDuzeltmeTipi = (arac.iDuzeltmeTipi != null && arac.iDuzeltmeTipi > 0 ? (int)arac.iDuzeltmeTipi : 0);
                                guncelleme.fVeresiye = Convert.ToDouble(arac.cVeresiye.ToString().Replace(".", ","));
                                guncelleme.fDuzeltme = Convert.ToDouble(arac.cDuzeltme.ToString().Replace(".", ","));
                                guncelleme.cAciklama = (arac.cAciklama != null && arac.cAciklama.ToString() != string.Empty ? arac.cAciklama.ToString() : string.Empty);
                                guncelleme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                guncelleme.fOtoparkUcreti =
                                    new Models.AracYeni().AracOtoparkTutar(
                                        cUrun2Listesi,
                                        Convert.ToDateTime(arac.cGirisTarihi),
                                        Convert.ToDateTime(arac.cCikisTarihi),
                                        iKodLokasyonLogin,
                                        (arac.cAboneMi == "Evet" ? 1 : 2),
                                        arac.iKodAracTipi);
                                guncelleme.iKDVOrani = 5;
                                guncelleme.iKDVTuru = 2;
                                guncelleme.fGenelTutar =
                                    new Models.AracYeni().AracGenelTutar(
                                        cUrun2Listesi,
                                        (int)guncelleme.iDuzeltmeTipi,
                                        (float)guncelleme.fVeresiye,
                                        (float)guncelleme.fDuzeltme,
                                        Convert.ToDateTime(arac.cGirisTarihi),
                                        Convert.ToDateTime(arac.cCikisTarihi),
                                        iKodLokasyonLogin,
                                        (arac.cAboneMi == "Evet" ? 1 : 2),
                                        arac.iKodAracTipi);
                                guncelleme.fTutar = Math.Round((double)guncelleme.fGenelTutar / 1.18, 2);

                                if (arac.iSayfaTipi == 1)
                                {
                                    guncelleme.dCikisTarihi = arac.dCikisTarihi;
                                }

                                dc.SubmitChanges();

                                ViewBag.iSonuc = 2;
                                if (arac.iPostTipi == 0)
                                {
                                    return Redirect("/Arac/BekleyenListeleYeni");
                                }
                                else
                                {
                                    return Redirect("/Arac/EkleYeni");
                                }

                            }
                            else
                            {
                                ViewBag.iSonuc = -2;
                            }

                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -1;
                    }
                }
                else
                {
                    ViewBag.iSonuc = -2;
                }

            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "EkleYeni_Post", Ex.Message);
            }

            return View(arac);
        }

        [HttpGet]
        public ActionResult VeresiyeGuncelleYeni(string id, string id2)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 135))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaTipi = id2;
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;
                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(id2))
                {
                    int iKodArac = 0;
                    int iSayfaTipi = 0;
                    if (int.TryParse(id, out iKodArac) && iKodArac > 0 && int.TryParse(id2, out iSayfaTipi) && iSayfaTipi > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Aracs
                                         join tableAracTipis in dc.AracTipis
                                            on table.iKodAracTipi equals tableAracTipis.iKodAracTipi into tableAracTipisClass
                                         from tableAracTipis in tableAracTipisClass.DefaultIfEmpty()
                                         join tableMusteri3s in dc.Musteri3s
                                            on table.iKodMusteri3 equals tableMusteri3s.iKodMusteri3 into tableMusteri3sClass
                                         from tableMusteri3s in tableMusteri3sClass.DefaultIfEmpty()
                                         join tableKullanici2s in dc.Kullanici2s
                                            on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                         from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                         where
                                           table.iKodArac == iKodArac &&
                                           table.iKodLokasyon == iKodLokasyonLogin &&
                                           table.iAktifMi == 1 &&
                                           (tableAracTipis != null && tableAracTipis.iAktifMi == 1) &&
                                           (tableMusteri3s != null && tableMusteri3s.iAktifMi == 1) &&
                                           (tableKullanici2s != null && tableKullanici2s.iAktifMi == 1)
                                         select new Models.AracYeni
                                         {
                                             iKodArac = table.iKodArac,
                                             iSayfaTipi = iSayfaTipi,
                                             iPostTipi = 0,
                                             cPlaka = (tableMusteri3s != null && tableMusteri3s.cPlaka != null && tableMusteri3s.cPlaka != string.Empty ? tableMusteri3s.cPlaka : string.Empty),
                                             cGenelTutar = string.Format("{0:N2}", new Models.AracYeni().AracGenelTutar(table.cUrun, (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0), (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0), (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0), (table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")), DateTime.Now, iKodLokasyonLogin, (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? 1 : 2), (int)table.iKodAracTipi)),
                                             cAboneMi = (table.dAboneBaslangicTarihi != null && table.dAboneBitisTarihi != null && DateTime.Now.Date >= table.dAboneBaslangicTarihi && DateTime.Now.Date <= table.dAboneBitisTarihi ? "Evet" : "Hayır"),
                                             cAboneBaslangicTarihi = (table.dAboneBaslangicTarihi != null && table.dAboneBaslangicTarihi != Convert.ToDateTime("1900-01-01") ? String.Format("{0:dd.MM.yyyy}", Convert.ToDateTime(table.dAboneBaslangicTarihi)) : "-"),
                                             cAboneBitisTarihi = (table.dAboneBaslangicTarihi != null && table.dAboneBaslangicTarihi != Convert.ToDateTime("1900-01-01") ? String.Format("{0:dd.MM.yyyy}", Convert.ToDateTime(table.dAboneBitisTarihi)) : "-"),
                                             iKodAracTipi = (table.iKodAracTipi != null ? (int)table.iKodAracTipi : 0),
                                             cGirisTarihi = (table.dGirisTarihi != null && table.dGirisTarihi != Convert.ToDateTime("1900-01-01") ? String.Format("{0:dd.MM.yyyy, HH:mm}", Convert.ToDateTime(table.dGirisTarihi)) : "-"),
                                             cCikisTarihi = String.Format("{0:dd.MM.yyyy, HH:mm}", DateTime.Now),
                                             dCikisTarihi = DateTime.Now,
                                             cOtoparkSuresi = new Models.AracYeni().AracOtoparkSuresi((table.dGirisTarihi != null ? Convert.ToDateTime(table.dGirisTarihi) : Convert.ToDateTime("1900-01-01")), DateTime.Now).ToString(),
                                             iDuzeltmeTipi = (table.iDuzeltmeTipi != null ? (int)table.iDuzeltmeTipi : 0),
                                             cVeresiye = string.Format("{0:N2}", (table.fVeresiye != null && table.fVeresiye > 0 ? (float)table.fVeresiye : 0)),
                                             cDuzeltme = string.Format("{0:N2}", (table.fDuzeltme != null && table.fDuzeltme > 0 ? (float)table.fDuzeltme : 0)),
                                             cAciklama = (table.cAciklama != null && table.cAciklama.ToString() != string.Empty ? table.cAciklama : string.Empty),
                                             cUrun = (table.cUrun != null && table.cUrun.ToString() != string.Empty ? table.cUrun : string.Empty),
                                             cOtoparkUcreti = string.Format("{0:N2}", (table.fOtoparkUcreti != null ? (float)table.fOtoparkUcreti : 0))
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cUrun))
                                {
                                    okuma.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(okuma.cUrun);
                                    string cUrunler = string.Empty;
                                    for (int i = 0; i < okuma.urun2Listesi.Count; i++)
                                    {
                                        okuma.urun2Listesi[i].iStokTutlacakMi = new Models.Urun2().GonderStokTutulacakMi(okuma.urun2Listesi[i].iKodUrun2, iKodLokasyonLogin);

                                        if (!String.IsNullOrEmpty(cUrunler))
                                        {
                                            cUrunler += "|";
                                        }

                                        cUrunler += okuma.urun2Listesi[i].cKodu + "*" + okuma.urun2Listesi[i].iKodUrun2 + "*" + okuma.urun2Listesi[i].iAdet + "*" + okuma.urun2Listesi[i].cBirimFiyati + "*" + okuma.urun2Listesi[i].cFiyat;
                                    }
                                    okuma.cUrun2Listesi = cUrunler;
                                }
                            }

                            return View(okuma);
                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -2;
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
            }

            return View();
        }

        [HttpPost]
        public ActionResult VeresiyeGuncelleYeni(string id, string id2, Models.AracYeni arac)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullaniciLogin = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullaniciLogin = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                int iKodKullaniciTipiLogin = 0;
                if (Session["iKodKullaniciTipi"] != null && Convert.ToInt32(Session["iKodKullaniciTipi"]) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(Session["iKodKullaniciTipi"]);
                }
                else if (GetCookie("iKodKullaniciTipi") != null && Convert.ToInt32(GetCookie("iKodKullaniciTipi")) > 0)
                {
                    iKodKullaniciTipiLogin = Convert.ToInt32(GetCookie("iKodKullaniciTipi"));
                }

                int iKodLokasyonLogin = 0;
                if (Session["iKodLokasyon"] != null && Convert.ToInt32(Session["iKodLokasyon"]) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(Session["iKodLokasyon"]);
                }
                else if (GetCookie("iKodLokasyon") != null && Convert.ToInt32(GetCookie("iKodLokasyon")) > 0)
                {
                    iKodLokasyonLogin = Convert.ToInt32(GetCookie("iKodLokasyon"));
                }

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 135))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaTipi = id2;
                ViewBag.iKodLokasyonLogin = iKodLokasyonLogin;
                ViewBag.AracTipiListesi = new Models.AracTipi().Gonder();
                ViewBag.DuzeltmeTipiListesi = new Models.DuzeltmeTipi().Gonder();
                ViewBag.Musteri3Listesi = new Models.Musteri3().Gonder();
                ViewBag.Urun2Listesi = new Models.Urun2().Gonder(iKodLokasyonLogin);

                if (arac != null && arac.iKodArac > 0)
                {
                    string cUrun2Listesi = string.Empty;
                    if (!String.IsNullOrEmpty(arac.cUrun2Listesi))
                    {
                        string[] cUrunler = arac.cUrun2Listesi.Split('|');
                        if (cUrunler.Length > 0)
                        {
                            for (int i = 0; i < cUrunler.Length; i++)
                            {
                                if (!String.IsNullOrEmpty(cUrun2Listesi))
                                {
                                    cUrun2Listesi += ",";
                                }

                                string[] cUrun = cUrunler[i].Split('*');

                                if (String.IsNullOrEmpty(cUrun[0]))
                                {
                                    cUrun[0] = "";
                                }

                                if (String.IsNullOrEmpty(cUrun[1]))
                                {
                                    cUrun[1] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[2] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[3] = "0";
                                }

                                if (String.IsNullOrEmpty(cUrun[2]))
                                {
                                    cUrun[4] = "0";
                                }

                                cUrun2Listesi += "{\"cKodu\":\"" + cUrun[0] + "\",\"iKodUrun2\":\"" + cUrun[1] + "\",\"iAdet\":\"" + cUrun[2] + "\",\"cBirimFiyati\":\"" + cUrun[3] + "\",\"cFiyat\":\"" + cUrun[4] + "\"}";
                            }

                            cUrun2Listesi = "[" + cUrun2Listesi + "]";
                        }

                        if (!String.IsNullOrEmpty(cUrun2Listesi))
                        {
                            arac.urun2Listesi = JsonConvert.DeserializeObject<List<Models.UrunJson2>>(cUrun2Listesi);

                            if (arac.urun2Listesi != null)
                            {
                                for (int i = 0; i < arac.urun2Listesi.Count; i++)
                                {
                                    if (String.IsNullOrEmpty(arac.urun2Listesi[i].cKodu) || arac.urun2Listesi[i].iKodUrun2 == 0 || arac.urun2Listesi[i].iAdet == 0 || String.IsNullOrEmpty(arac.urun2Listesi[i].cBirimFiyati) || arac.urun2Listesi[i].cBirimFiyati == "0.00" || String.IsNullOrEmpty(arac.urun2Listesi[i].cFiyat) || arac.urun2Listesi[i].cFiyat == "0.00")
                                    {
                                        ModelState.AddModelError("cUrun2Listesi", "Lütfen bu alanı doldurun!");
                                    }
                                }
                            }
                        }
                    }


                    if (ModelState.IsValid)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var guncelleme = (from table in dc.Aracs
                                              where
                                                table.iKodArac == arac.iKodArac &&
                                                table.iAktifMi == 1
                                              select table).FirstOrDefault();
                            if (guncelleme != null)
                            {
                                guncelleme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                guncelleme.fGenelTutar =
                                    new Models.AracYeni().AracGenelTutar(
                                        cUrun2Listesi,
                                        (int)guncelleme.iDuzeltmeTipi,
                                        (float)guncelleme.fVeresiye,
                                        (float)guncelleme.fDuzeltme,
                                        Convert.ToDateTime(arac.cGirisTarihi),
                                        Convert.ToDateTime(arac.cCikisTarihi),
                                        iKodLokasyonLogin,
                                        (arac.cAboneMi == "Evet" ? 1 : 2),
                                        arac.iKodAracTipi);
                                guncelleme.fTutar = Math.Round((double)guncelleme.fGenelTutar / 1.18, 2);

                                if (arac.iSayfaTipi == 1)
                                {
                                    guncelleme.dCikisTarihi = arac.dCikisTarihi;
                                }

                                dc.SubmitChanges();

                                ViewBag.iSonuc = 2;
                                return Redirect("/Arac/VeresiyeListeleYeni");


                            }
                            else
                            {
                                ViewBag.iSonuc = -2;
                            }

                        }
                    }
                    else
                    {
                        ViewBag.iSonuc = -1;
                    }
                }
                else
                {
                    ViewBag.iSonuc = -2;
                }

            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Arac", "EkleYeni_Post", Ex.Message);
            }

            return View(arac);
        }

        private string GetCookie(string cName)
        {
            if (Request.Cookies.AllKeys.Contains(cName))
            {
                return Request.Cookies[cName].Value;
            }
            return null;
        }
    }
}