using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace otoparkyonetim.Controllers
{
    public class Kullanici2Controller : Controller
    {
        [HttpGet]
        public ActionResult Giris()
        {
            try
            {
                if (Session["iKodKullanici"] != null || GetCookie("iKodKullanici") != null)
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.LokasyonListesi = new Models.Lokasyon().Gonder();
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Kullanici2", "Giris_Get", Ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Giris(Models.Kullanici2.Kullanici2Giris kullaniciGiris)
        {
            try
            {
                if (Session["iKodKullanici"] != null || GetCookie("iKodKullanici") != null)
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.LokasyonListesi = new Models.Lokasyon().Gonder();

                if (ModelState.IsValid)
                {
                    using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                    {
                        var resultKullanici2s =
                            (from tableKullanici2s in dc.Kullanici2s
                             join tableKullaniciTipis in dc.KullaniciTipis
                                on tableKullanici2s.iKodKullaniciTipi equals tableKullaniciTipis.iKodKullaniciTipi into tableKullaniciTipisClass
                             from tableKullaniciTipis in tableKullaniciTipisClass.DefaultIfEmpty()
                             join tableLokasyons in dc.Lokasyons
                                on tableKullanici2s.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                             from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                             where
                                tableKullanici2s.iKodLokasyon == kullaniciGiris.iKodLokasyon &&
                                tableKullanici2s.cEMail == kullaniciGiris.cEMail &&
                                tableKullanici2s.cSifre == kullaniciGiris.cSifre &&
                                tableKullanici2s.iAktifMi == 1 &&
                                tableKullaniciTipis.iAktifMi == 1
                             select new Models.Kullanici2
                             {
                                 iKodKullanici2 = tableKullanici2s.iKodKullanici2,
                                 iKodKullaniciTipi = (int)tableKullanici2s.iKodKullaniciTipi,
                                 iKodLokasyon = (int)tableKullanici2s.iKodLokasyon,
                                 cEMail = tableKullanici2s.cEMail,
                                 cAdi = tableKullanici2s.cAdi,
                                 cSoyadi = tableKullanici2s.cSoyadi,
                                 cKullaniciTipi = tableKullaniciTipis.cAdi,
                                 cLokasyon = tableLokasyons.cAdi,
                                 cResimListesi = tableKullanici2s.cFotograf
                             }).FirstOrDefault();

                        if (resultKullanici2s != null)
                        {
                            if (!String.IsNullOrEmpty(resultKullanici2s.cResimListesi))
                            {
                                resultKullanici2s.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(resultKullanici2s.cResimListesi);
                            }

                            if (kullaniciGiris.lBeniHatirla == false)
                            {
                                Session.Add("iKodKullanici", resultKullanici2s.iKodKullanici2);
                                Session.Add("iKodKullaniciTipi", resultKullanici2s.iKodKullaniciTipi);
                                Session.Add("iKodLokasyon", resultKullanici2s.iKodLokasyon);
                                Session.Add("cEPosta", resultKullanici2s.cEMail);
                                Session.Add("cAd", new Class.TextLowerAndFirstUpper().Send(resultKullanici2s.cAdi));
                                Session.Add("cSoyad", resultKullanici2s.cSoyadi.ToUpper());
                                Session.Add("cKullaniciTipi", new Class.TextLowerAndFirstUpper().Send(resultKullanici2s.cKullaniciTipi));
                                Session.Add("cLokasyon", new Class.TextLowerAndFirstUpper().Send(resultKullanici2s.cLokasyon));

                                if (!String.IsNullOrEmpty(resultKullanici2s.cResimListesi) &&
                                    resultKullanici2s.resimListesi != null &&
                                    resultKullanici2s.resimListesi.Count > 0 &&
                                    !String.IsNullOrEmpty(resultKullanici2s.resimListesi[0].cKucukResim))
                                {
                                    Session.Add("cFotograf", "/Files/" + resultKullanici2s.resimListesi[0].cKucukResim);
                                }
                                else
                                {
                                    Session.Add("cFotograf", "/Images/user.png");
                                }
                            }
                            else
                            {
                                CreateCookie("iKodKullanici", resultKullanici2s.iKodKullanici2.ToString());
                                CreateCookie("iKodKullaniciTipi", resultKullanici2s.iKodKullaniciTipi.ToString());
                                CreateCookie("iKodLokasyon", resultKullanici2s.iKodLokasyon.ToString());
                                CreateCookie("cEPosta", resultKullanici2s.cEMail);
                                CreateCookie("cAd", new Class.TextLowerAndFirstUpper().Send(resultKullanici2s.cAdi));
                                CreateCookie("cSoyad", resultKullanici2s.cSoyadi.ToUpper());
                                CreateCookie("cKullaniciTipi", new Class.TextLowerAndFirstUpper().Send(resultKullanici2s.cKullaniciTipi));
                                CreateCookie("cLokasyon", new Class.TextLowerAndFirstUpper().Send(resultKullanici2s.cLokasyon));

                                if (!String.IsNullOrEmpty(resultKullanici2s.cResimListesi) &&
                                    resultKullanici2s.resimListesi != null &&
                                    resultKullanici2s.resimListesi.Count > 0 &&
                                    !String.IsNullOrEmpty(resultKullanici2s.resimListesi[0].cKucukResim))
                                {
                                    CreateCookie("cFotograf", "/Files/" + resultKullanici2s.resimListesi[0].cKucukResim);
                                }
                                else
                                {
                                    CreateCookie("cFotograf", "/Images/user.png");
                                }
                            }

                            return Redirect("/CRM/AnaSayfa2");
                        }
                        else
                        {
                            ViewBag.iSonuc = -7;
                        }
                    }
                }
                else
                {
                    ViewBag.iSonuc = -6;
                }
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Kullanici2", "Ara_Post", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult Cikis()
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
                }

                Session.RemoveAll();
                DeleteCookie("iKodKullanici");
                DeleteCookie("iKodKullaniciTipi");
                DeleteCookie("iKodLokasyon");
                DeleteCookie("cEPosta");
                DeleteCookie("cAd");
                DeleteCookie("cSoyad");
                DeleteCookie("cKullaniciTipi");
                DeleteCookie("cLokasyon");
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Kullanici2", "Cikis_Get", Ex.Message);
            }

            return Redirect("/Kullanici2/Giris");
        }

        [HttpGet]
        public ActionResult Ekle(string id, int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.KullaniciTipiListesi = new Models.KullaniciTipi().Gonder();
                ViewBag.LokasyonListesi = new Models.Lokasyon().Gonder();

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodKullanici2 = 0;
                    if (int.TryParse(id, out iKodKullanici2) && iKodKullanici2 > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Kullanici2s
                                         where
                                           table.iKodKullanici2 == iKodKullanici2 &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select new Models.Kullanici2
                                         {
                                             iKodKullanici2 = table.iKodKullanici2,
                                             iKodKullaniciTipi = (int)table.iKodKullaniciTipi,
                                             iKodLokasyon = (int)table.iKodLokasyon,
                                             cResimListesi = table.cFotograf,
                                             cAdi = table.cAdi,
                                             cSoyadi = table.cSoyadi,
                                             cTelefon = table.cTelefon,
                                             cGSM = table.cGSM,
                                             cEMail = table.cEMail,
                                             cSifre = table.cSifre,
                                             iAktifMi = table.iAktifMi,
                                             iAktif = table.iAktifMi,
                                             iPasif = table.iAktifMi
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cResimListesi))
                                {
                                    okuma.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(okuma.cResimListesi);
                                    string cResimler = string.Empty;
                                    for (int i = 0; i < okuma.resimListesi.Count; i++)
                                    {
                                        if (!String.IsNullOrEmpty(cResimler))
                                        {
                                            cResimler += "|";
                                        }

                                        cResimler += okuma.resimListesi[i].cKucukResim.Replace("th-", "");
                                    }
                                    okuma.cResimListesi = cResimler;
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
                new Class.Log().Hata("Kullanici2", "Ekle_Get", Ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Ekle(Models.Kullanici2 kullanici, int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

     
                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.KullaniciTipiListesi = new Models.KullaniciTipi().Gonder();
                ViewBag.LokasyonListesi = new Models.Lokasyon().Gonder();

                if (kullanici != null)
                {
                    string cResimListesi = string.Empty;
                    if (!String.IsNullOrEmpty(kullanici.cResimListesi))
                    {
                        string[] cResimler = kullanici.cResimListesi.Split('|');
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
                            kullanici.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(cResimListesi);
                        }
                    }

                    if (kullanici.iKodKullanici2 > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                            {
                                var kontrol = (from table in dc.Kullanici2s
                                               where
                                                 table.iKodKullanici2 != kullanici.iKodKullanici2 &&
                                                 table.cAdi == kullanici.cEMail &&
                                                 (table.iAktifMi == 0 || table.iAktifMi == 1)
                                               select table).FirstOrDefault();

                                if (kontrol == null)
                                {
                                    var guncelleme = (from table in dc.Kullanici2s
                                                      where
                                                        table.iKodKullanici2 == kullanici.iKodKullanici2 &&
                                                        (table.iAktifMi == 0 || table.iAktifMi == 1)
                                                      select table).FirstOrDefault();

                                    if (guncelleme != null)
                                    {
                                        guncelleme.iKodKullaniciTipi = kullanici.iKodKullaniciTipi;
                                        guncelleme.iKodLokasyon = kullanici.iKodLokasyon;
                                        guncelleme.cFotograf = cResimListesi;
                                        guncelleme.cAdi = kullanici.cAdi;
                                        guncelleme.cSoyadi = kullanici.cSoyadi;
                                        guncelleme.cTelefon = kullanici.cTelefon;
                                        guncelleme.cGSM = kullanici.cGSM;
                                        guncelleme.cEMail = kullanici.cEMail;
                                        guncelleme.cSifre = kullanici.cSifre;
                                        guncelleme.dTarih = DateTime.Now;
                                        guncelleme.iAktifMi = kullanici.iAktifMi;
                                        guncelleme.iSonGuncelleyenKullanici = iKodKullanici2Login;
                                        dc.SubmitChanges();

                                        ViewBag.iSonuc = 2;
                                        return Redirect("/Kullanici2/Listele");
                                    }
                                    else
                                    {
                                        ViewBag.iSonuc = -2;
                                    }
                                }
                                else
                                {
                                    ViewBag.iSonuc = -4;
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
                        if (ModelState.IsValid)
                        {
                            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                            {
                                var kontrol = (from table in dc.Kullanici2s
                                               where
                                                 table.cEMail == kullanici.cEMail &&
                                                 (table.iAktifMi == 0 || table.iAktifMi == 1)
                                               select table).FirstOrDefault();

                                if (kontrol == null)
                                {
                                    Data.Kullanici2 yenikayit = new Data.Kullanici2();
                                    yenikayit.iKodKullaniciTipi = kullanici.iKodKullaniciTipi;
                                    yenikayit.iKodLokasyon = kullanici.iKodLokasyon;
                                    yenikayit.cFotograf = cResimListesi;
                                    yenikayit.cAdi = kullanici.cAdi;
                                    yenikayit.cSoyadi = kullanici.cSoyadi;
                                    yenikayit.cTelefon = kullanici.cTelefon;
                                    yenikayit.cGSM = kullanici.cGSM;
                                    yenikayit.cEMail = kullanici.cEMail;
                                    yenikayit.cSifre = kullanici.cSifre;
                                    yenikayit.dTarih = DateTime.Now;
                                    yenikayit.iAktifMi = kullanici.iAktifMi;
                                    yenikayit.iSonGuncelleyenKullanici = iKodKullanici2Login;
                                    dc.Kullanici2s.InsertOnSubmit(yenikayit);
                                    dc.SubmitChanges();

                                    ViewBag.iSonuc = 1;
                                    return Redirect("/Kullanici2/Listele");
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
                new Class.Log().Hata("Kullanici2", "Ekle_Post", Ex.Message);
            }

            return View(kullanici);
        }

        [HttpGet]
        public ActionResult DetayliBilgi(string id, int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
                ViewBag.KullaniciTipiListesi = new Models.KullaniciTipi().Gonder();
                ViewBag.LokasyonListesi = new Models.Lokasyon().Gonder();

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodKullanici2 = 0;
                    if (int.TryParse(id, out iKodKullanici2) && iKodKullanici2 > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.Kullanici2s
                                         where
                                           table.iKodKullanici2 == iKodKullanici2 &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select new Models.Kullanici2
                                         {
                                             iKodKullanici2 = table.iKodKullanici2,
                                             iKodKullaniciTipi = (int)table.iKodKullaniciTipi,
                                             iKodLokasyon = (int)table.iKodLokasyon,
                                             cResimListesi = table.cFotograf,
                                             cAdi = table.cAdi,
                                             cSoyadi = table.cSoyadi,
                                             cTelefon = table.cTelefon,
                                             cGSM = table.cGSM,
                                             cEMail = table.cEMail,
                                             cSifre = table.cSifre,
                                             iAktifMi = table.iAktifMi,
                                             iAktif = table.iAktifMi,
                                             iPasif = table.iAktifMi
                                         }).FirstOrDefault();

                            if (okuma != null)
                            {
                                if (!String.IsNullOrEmpty(okuma.cResimListesi))
                                {
                                    okuma.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(okuma.cResimListesi);
                                    string cResimler = string.Empty;
                                    for (int i = 0; i < okuma.resimListesi.Count; i++)
                                    {
                                        if (!String.IsNullOrEmpty(cResimler))
                                        {
                                            cResimler += "|";
                                        }

                                        cResimler += okuma.resimListesi[i].cKucukResim.Replace("th-", "");
                                    }
                                    okuma.cResimListesi = cResimler;
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
                new Class.Log().Hata("Kullanici2", "Ekle_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult Guncelle()
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 39))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }


                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var okuma = (from table in dc.Kullanici2s
                                 where
                                   table.iKodKullanici2 == iKodKullanici2Login &&
                                   (table.iAktifMi == 0 || table.iAktifMi == 1)
                                 select new Models.Kullanici2
                                 {
                                     iKodKullanici2 = table.iKodKullanici2,
                                     cResimListesi = table.cFotograf,
                                     cAdi = table.cAdi,
                                     cSoyadi = table.cSoyadi,
                                     cTelefon = table.cTelefon,
                                     cGSM = table.cGSM,
                                     cEMail = table.cEMail,
                                     cSifre = table.cSifre
                                 }).FirstOrDefault();

                    if (okuma != null)
                    {
                        if (!String.IsNullOrEmpty(okuma.cResimListesi))
                        {
                            okuma.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(okuma.cResimListesi);
                            string cResimler = string.Empty;
                            for (int i = 0; i < okuma.resimListesi.Count; i++)
                            {
                                if (!String.IsNullOrEmpty(cResimler))
                                {
                                    cResimler += "|";
                                }

                                cResimler += okuma.resimListesi[i].cKucukResim.Replace("th-", "");
                            }
                            okuma.cResimListesi = cResimler;
                        }
                    }

                    return View(okuma);
                }


            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Kullanici2", "Guncelle_Get", Ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Guncelle(Models.Kullanici2 kullanici)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 39))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }


                if (kullanici != null)
                {
                    string cResimListesi = string.Empty;
                    if (!String.IsNullOrEmpty(kullanici.cResimListesi))
                    {
                        string[] cResimler = kullanici.cResimListesi.Split('|');
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
                            kullanici.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(cResimListesi);
                        }
                    }

                    if (kullanici.iKodKullanici2 > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                            {
                                var kontrol = (from table in dc.Kullanici2s
                                               where
                                                 table.iKodKullanici2 != kullanici.iKodKullanici2 &&
                                                 table.cEMail == kullanici.cEMail &&
                                                 (table.iAktifMi == 0 || table.iAktifMi == 1)
                                               select table).FirstOrDefault();

                                if (kontrol == null)
                                {
                                    var guncelleme = (from table in dc.Kullanici2s
                                                      where
                                                        table.iKodKullanici2 == kullanici.iKodKullanici2 &&
                                                        (table.iAktifMi == 0 || table.iAktifMi == 1)
                                                      select table).FirstOrDefault();

                                    if (guncelleme != null)
                                    {
                                        guncelleme.cFotograf = cResimListesi;
                                        guncelleme.cAdi = kullanici.cAdi;
                                        guncelleme.cSoyadi = kullanici.cSoyadi;
                                        guncelleme.cTelefon = kullanici.cTelefon;
                                        guncelleme.cGSM = kullanici.cGSM;
                                        guncelleme.cEMail = kullanici.cEMail;
                                        guncelleme.cSifre = kullanici.cSifre;
                                        guncelleme.dTarih = DateTime.Now;
                                        dc.SubmitChanges();

                                        ViewBag.iSonuc = 2;
                                    }
                                    else
                                    {
                                        ViewBag.iSonuc = -2;
                                    }
                                }
                                else
                                {
                                    ViewBag.iSonuc = -4;
                                }
                            }
                        }
                        else
                        {
                            ViewBag.iSonuc = -5;
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
                new Class.Log().Hata("Guncelle", "Ekle_Post", Ex.Message);
            }

            return View(kullanici);
        }

        [HttpGet]
        public ActionResult Listele(int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }


                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.Kullanici2s
                                     join tableKullaniciTipis in dc.KullaniciTipis
                                        on table.iKodKullaniciTipi equals tableKullaniciTipis.iKodKullaniciTipi into tableKullaniciTipisClass
                                     from tableKullaniciTipis in tableKullaniciTipisClass.DefaultIfEmpty()
                                     join tableLokasyons in dc.Lokasyons
                                        on table.iKodLokasyon equals tableLokasyons.iKodLokasyon into tableLokasyonsClass
                                     from tableLokasyons in tableLokasyonsClass.DefaultIfEmpty()
                                     join tableKullanici2s in dc.Kullanici2s
                                       on table.iSonGuncelleyenKullanici equals tableKullanici2s.iKodKullanici2 into tableKullanici2sClass
                                     from tableKullanici2s in tableKullanici2sClass.DefaultIfEmpty()
                                     where
                                        (table.iAktifMi == 0 || table.iAktifMi == 1) &&
                                        (tableKullanici2s != null && tableKullanici2s.iAktifMi != null && tableKullanici2s.iAktifMi == 1)
                                     select new Models.Kullanici2
                                     {
                                         iKodKullanici2 = table.iKodKullanici2,
                                         cKullanici2 = 
                                            "Kullanıcı Tipi : " + (tableKullaniciTipis != null && tableKullaniciTipis.cAdi != null && tableKullaniciTipis.cAdi != string.Empty ? tableKullaniciTipis.cAdi : "-") +
                                            "<br/>Lokasyon : " + (tableLokasyons != null && tableLokasyons.cAdi != null && tableLokasyons.cAdi != string.Empty ? tableLokasyons.cAdi : "-") +
                                            "<br/>Ad Soyad : " + (table.cAdi + " " + table.cSoyadi) +
                                            "<br/>GSM : " + (table.cGSM != null && table.cGSM != string.Empty ? table.cGSM : "-") +
                                            "<br/>E-Mail : " + table.cEMail +
                                            "<br/>Kullanıcı : " + (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         cKullaniciTipi = tableKullaniciTipis.cAdi,
                                         cLokasyon = tableLokasyons.cAdi,
                                         cAdi = table.cAdi + " " + table.cSoyadi,
                                         cGSM = table.cGSM,
                                         cEMail = table.cEMail,
                                         cKullanici = (tableKullanici2s != null && tableKullanici2s.cAdi != null && tableKullanici2s.cAdi != string.Empty && tableKullanici2s.cSoyadi != null && tableKullanici2s.cSoyadi != string.Empty ? tableKullanici2s.cAdi + " " + tableKullanici2s.cSoyadi : "-"),
                                         iAktifMi = table.iAktifMi
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
                                    cSplitArama.Any(model.iKodKullanici2.ToString().ToLower().Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cKullaniciTipi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cLokasyon).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cAdi).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cGSM).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cKullanici).Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cEMail).Contains)
                                    ).ToList();
                        }
                        else
                        {
                            ViewBag.cSplitArama = null;
                            ViewBag.cArama = string.Empty;
                        }
                    }

                    ViewBag.cSiralama = siralamaSekli;
                    ViewBag.Kolon1 = siralamaSekli == "kolon1" ? "kolon1_desc" : "kolon1";
                    ViewBag.Kolon2 = siralamaSekli == "kolon2" ? "kolon2_desc" : "kolon2";
                    ViewBag.Kolon7 = siralamaSekli == "kolon7" ? "kolon7_desc" : "kolon7";

                    string cArrowYukari = "<i class=\"fa fa-caret-up pl-1\" aria-hidden=\"true\"></i>";
                    string cArrowAsagi = "<i class=\"fa fa-caret-down pl-1\" aria-hidden=\"true\"></i>";
                    switch (siralamaSekli)
                    {
                        case "kolon1":
                            listeleme = listeleme.OrderBy(s => s.iKodKullanici2).ToList();
                            ViewBag.cArrowKolon1 = cArrowYukari;
                            break;
                        case "kolon2":
                            listeleme = listeleme.OrderBy(s => s.cKullanici2).ToList();
                            ViewBag.cArrowKolon2 = cArrowYukari;
                            break;
                        case "kolon2_desc":
                            listeleme = listeleme.OrderByDescending(s => s.cKullanici2).ToList();
                            ViewBag.cArrowKolon2 = cArrowAsagi;
                            break;
                        case "kolon7":
                            listeleme = listeleme.OrderBy(s => s.iAktifMi).ToList();
                            ViewBag.cArrowKolon7 = cArrowYukari;
                            break;
                        case "kolon7_desc":
                            listeleme = listeleme.OrderByDescending(s => s.iAktifMi).ToList();
                            ViewBag.cArrowKolon7 = cArrowAsagi;
                            break;
                        default:
                            listeleme = listeleme.OrderByDescending(s => s.iKodKullanici2).ToList();
                            ViewBag.cArrowKolon1 = cArrowAsagi;
                            break;
                    }

                    int iSayfaNo = sayfaNo ?? 1;
                    int iListelemeSayisi = 50;
                    ViewBag.iSayfaNo = iSayfaNo;
                    ViewBag.iToplamKayitSayisi = listeleme.Count;
                    ViewBag.iIlkKayit = ((((int)ViewBag.iSayfaNo - 1) * iListelemeSayisi) + 1);
                    ViewBag.iSonKayit = (((int)ViewBag.iSayfaNo * iListelemeSayisi));
                    if (ViewBag.iSonKayit > (int)ViewBag.iToplamKayitSayisi)
                    {
                        ViewBag.iSonKayit = (int)ViewBag.iToplamKayitSayisi;
                    }

                    return View(listeleme.ToPagedList(iSayfaNo, iListelemeSayisi));
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Kullanici2", "Listele_Get", Ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Ara(int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }



                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Kullanici2", "Ara_Get", Ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult Sil(string id, int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodKullanici2 = 0;
                    if (int.TryParse(id, out iKodKullanici2) && iKodKullanici2 > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var silme = (from table in dc.Kullanici2s
                                         where
                                           table.iKodKullanici2 == iKodKullanici2 &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select table).FirstOrDefault();

                            if (silme != null)
                            {
                                silme.iAktifMi = -1;
                                silme.iSonGuncelleyenKullanici = iKodKullanici2Login;
                                silme.dTarih = DateTime.Now;
                                dc.SubmitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Kullanici2", "Sil_Get", Ex.Message);
            }

            return Redirect("/Kullanici2/Listele?sayfaNo=" + sayfaNo + "&siralamaSekli=" + siralamaSekli + "&arama=" + arama);
        }

        [HttpGet]
        public ActionResult AktifPasif(string id, string durum, int? sayfaNo, string siralamaSekli, string arama)
        {
            try
            {
                if (Session["iKodKullanici"] == null && GetCookie("iKodKullanici") == null)
                {
                    return Redirect("/Kullanici2/Giris");
                }

                int iKodKullanici2Login = 0;
                if (Session["iKodKullanici"] != null && Convert.ToInt32(Session["iKodKullanici"]) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(Session["iKodKullanici"]);
                }
                else if (GetCookie("iKodKullanici") != null && Convert.ToInt32(GetCookie("iKodKullanici")) > 0)
                {
                    iKodKullanici2Login = Convert.ToInt32(GetCookie("iKodKullanici"));
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 38))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(durum))
                {
                    int iKodKullanici2 = 0;
                    int iAktifMi = 0;
                    if (int.TryParse(id, out iKodKullanici2) && iKodKullanici2 > 0 &&
                        int.TryParse(durum, out iAktifMi))
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var silme = (from table in dc.Kullanici2s
                                         where
                                           table.iKodKullanici2 == iKodKullanici2 &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select table).FirstOrDefault();

                            if (silme != null)
                            {
                                silme.iAktifMi = iAktifMi;
                                silme.iSonGuncelleyenKullanici = iKodKullanici2Login;
                                silme.dTarih = DateTime.Now;
                                dc.SubmitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Kullanici2", "AktifPasif_Get", Ex.Message);
            }

            return Redirect("/Kullanici2/Listele?sayfaNo=" + sayfaNo + "&siralamaSekli=" + siralamaSekli + "&arama=" + arama);
        }

        private string GetCookie(string cName)
        {
            if (Request.Cookies.AllKeys.Contains(cName))
            {
                return Request.Cookies[cName].Value;
            }
            return null;
        }

        private void CreateCookie(string name, string value)
        {
            HttpCookie cookieVisitor = new HttpCookie(name, value);
            cookieVisitor.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(cookieVisitor);
        }

        private void DeleteCookie(string name)
        {
            if (GetCookie(name) != null)
            {
                Response.Cookies.Remove(name);
                Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
        }
    }
}
