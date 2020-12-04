using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace otoparkyonetim.Controllers
{
    public class KullaniciTipiYetkiController : Controller
    {
        [HttpGet]
        public ActionResult Ekle(string id, int? sayfaNo, string siralamaSekli, string arama)
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }
                

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodKullaniciTipiYetki = 0;
                    if (int.TryParse(id, out iKodKullaniciTipiYetki) && iKodKullaniciTipiYetki > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.KullaniciTipiYetkis
                                         where
                                           table.iKodKullaniciTipiYetki == iKodKullaniciTipiYetki &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select new Models.KullaniciTipiYetki
                                         {
                                             iKodKullaniciTipiYetki = table.iKodKullaniciTipiYetki,
                                             cAdi = table.cAdi,
                                             iAktifMi = table.iAktifMi,
                                             iAktif = table.iAktifMi,
                                             iPasif = table.iAktifMi
                                         }).FirstOrDefault();

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
                new Class.Log().Hata("KullaniciTipiYetki", "Ekle_Get", Ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Ekle(Models.KullaniciTipiYetki KullaniciTipiYetki, int? sayfaNo, string siralamaSekli, string arama)
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;

                if (KullaniciTipiYetki != null)
                {
                    if (KullaniciTipiYetki.iKodKullaniciTipiYetki > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                            {
                                var kontrol = (from table in dc.KullaniciTipiYetkis
                                               where
                                                 table.iKodKullaniciTipiYetki != KullaniciTipiYetki.iKodKullaniciTipiYetki &&
                                                 table.cAdi == KullaniciTipiYetki.cAdi &&
                                                 (table.iAktifMi == 0 || table.iAktifMi == 1)
                                               select table).FirstOrDefault();

                                if (kontrol == null)
                                {
                                    var guncelleme = (from table in dc.KullaniciTipiYetkis
                                                      where
                                                        table.iKodKullaniciTipiYetki == KullaniciTipiYetki.iKodKullaniciTipiYetki &&
                                                        (table.iAktifMi == 0 || table.iAktifMi == 1)
                                                      select table).FirstOrDefault();

                                    if (guncelleme != null)
                                    {
                                        guncelleme.cAdi = KullaniciTipiYetki.cAdi;
                                        guncelleme.iAktifMi = KullaniciTipiYetki.iAktifMi;
                                        guncelleme.dTarih = DateTime.Now;
                                        guncelleme.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                        dc.SubmitChanges();

                                        ViewBag.iSonuc = 2;
                                        return Redirect("/KullaniciTipiYetki/Listele");
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
                                var kontrol = (from table in dc.KullaniciTipiYetkis
                                               where
                                                 table.cAdi == KullaniciTipiYetki.cAdi &&
                                                 (table.iAktifMi == 0 || table.iAktifMi == 1)
                                               select table).FirstOrDefault();

                                if (kontrol == null)
                                {
                                    Data.KullaniciTipiYetki yenikayit = new Data.KullaniciTipiYetki();
                                    yenikayit.cAdi = KullaniciTipiYetki.cAdi;
                                    yenikayit.iAktifMi = KullaniciTipiYetki.iAktifMi;
                                    yenikayit.dTarih = DateTime.Now;
                                    yenikayit.iSonGuncelleyenKullanici = iKodKullaniciLogin;
                                    dc.KullaniciTipiYetkis.InsertOnSubmit(yenikayit);
                                    dc.SubmitChanges();

                                    ViewBag.iSonuc = 1;
                                    return Redirect("/KullaniciTipiYetki/Listele");
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
                new Class.Log().Hata("KullaniciTipiYetki", "Ekle_Post", Ex.Message);
            }

            return View(KullaniciTipiYetki);
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodKullaniciTipiYetki = 0;
                    if (int.TryParse(id, out iKodKullaniciTipiYetki) && iKodKullaniciTipiYetki > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var okuma = (from table in dc.KullaniciTipiYetkis
                                         where
                                           table.iKodKullaniciTipiYetki == iKodKullaniciTipiYetki &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select new Models.KullaniciTipiYetki
                                         {
                                             iKodKullaniciTipiYetki = table.iKodKullaniciTipiYetki,
                                             cAdi = table.cAdi,
                                             iAktifMi = table.iAktifMi,
                                             iAktif = table.iAktifMi,
                                             iPasif = table.iAktifMi
                                         }).FirstOrDefault();

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
                new Class.Log().Hata("KullaniciTipiYetki", "Ekle_Get", Ex.Message);
            }

            return View();
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;

                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var listeleme = (from table in dc.KullaniciTipiYetkis
                                     where
                                        (table.iAktifMi == 0 || table.iAktifMi == 1)
                                     select new Models.KullaniciTipiYetki
                                     {
                                         iKodKullaniciTipiYetki = table.iKodKullaniciTipiYetki,
                                         cAdi = table.cAdi,
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
                                    cSplitArama.Any(model.iKodKullaniciTipiYetki.ToString().ToLower().Contains) ||
                                    cSplitArama.Any(new Class.StringOperations().NullSafeToLower(model.cAdi).Contains)
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
                    ViewBag.Kolon3 = siralamaSekli == "kolon3" ? "kolon3_desc" : "kolon3";

                    string cArrowYukari = "<i class=\"fa fa-caret-up pl-1\" aria-hidden=\"true\"></i>";
                    string cArrowAsagi = "<i class=\"fa fa-caret-down pl-1\" aria-hidden=\"true\"></i>";
                    switch (siralamaSekli)
                    {
                        case "kolon1":
                            listeleme = listeleme.OrderBy(s => s.iKodKullaniciTipiYetki).ToList();
                            ViewBag.cArrowKolon1 = cArrowYukari;
                            break;
                        case "kolon2":
                            listeleme = listeleme.OrderBy(s => s.cAdi).ToList();
                            ViewBag.cArrowKolon2 = cArrowYukari;
                            break;
                        case "kolon2_desc":
                            listeleme = listeleme.OrderByDescending(s => s.cAdi).ToList();
                            ViewBag.cArrowKolon2 = cArrowAsagi;
                            break;
                        case "kolon3":
                            listeleme = listeleme.OrderBy(s => s.iAktifMi).ToList();
                            ViewBag.cArrowKolon3 = cArrowYukari;
                            break;
                        case "kolon3_desc":
                            listeleme = listeleme.OrderByDescending(s => s.iAktifMi).ToList();
                            ViewBag.cArrowKolon3 = cArrowAsagi;
                            break;
                        default:
                            listeleme = listeleme.OrderByDescending(s => s.iKodKullaniciTipiYetki).ToList();
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
                new Class.Log().Hata("KullaniciTipiYetki", "Listele_Get", Ex.Message);
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                ViewBag.iSayfaNo = sayfaNo;
                ViewBag.cSiralama = siralamaSekli;
                ViewBag.cArama = arama;
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("KullaniciTipiYetki", "Ara_Get", Ex.Message);
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                if (!String.IsNullOrEmpty(id))
                {
                    int iKodKullaniciTipiYetki = 0;
                    if (int.TryParse(id, out iKodKullaniciTipiYetki) && iKodKullaniciTipiYetki > 0)
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var silme = (from table in dc.KullaniciTipiYetkis
                                         where
                                           table.iKodKullaniciTipiYetki == iKodKullaniciTipiYetki &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select table).FirstOrDefault();

                            if (silme != null)
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
                new Class.Log().Hata("KullaniciTipiYetki", "Sil_Get", Ex.Message);
            }

            return Redirect("/KullaniciTipiYetki/Listele?sayfaNo=" + sayfaNo + "&siralamaSekli=" + siralamaSekli + "&arama=" + arama);
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 4))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(durum))
                {
                    int iKodKullaniciTipiYetki = 0;
                    int iAktifMi = 0;
                    if (int.TryParse(id, out iKodKullaniciTipiYetki) && iKodKullaniciTipiYetki > 0 &&
                        int.TryParse(durum, out iAktifMi))
                    {
                        using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                        {
                            var silme = (from table in dc.KullaniciTipiYetkis
                                         where
                                           table.iKodKullaniciTipiYetki == iKodKullaniciTipiYetki &&
                                           (table.iAktifMi == 0 || table.iAktifMi == 1)
                                         select table).FirstOrDefault();

                            if (silme != null)
                            {
                                silme.iAktifMi = iAktifMi;
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
                new Class.Log().Hata("KullaniciTipiYetki", "AktifPasif_Get", Ex.Message);
            }

            return Redirect("/KullaniciTipiYetki/Listele?sayfaNo=" + sayfaNo + "&siralamaSekli=" + siralamaSekli + "&arama=" + arama);
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
