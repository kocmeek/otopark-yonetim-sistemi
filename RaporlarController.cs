using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace otoparkcogencomtr.Controllers
{
    public class RaporlarController : Controller
    {
        public ActionResult Listele(string dBaslangicTarihi, string dBitisTarihi)
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
                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 52))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }
                

                List<Models.Raporlar> raporlars = new List<Models.Raporlar>();

                DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");
                if (!String.IsNullOrEmpty(dBaslangicTarihi) && !String.IsNullOrEmpty(dBitisTarihi))
                {
                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }
                }
                else
                {
                    dBaslangicTarihiLocal = DateTime.Now.AddDays(-7);
                    dBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", dBaslangicTarihiLocal);
                    dBitisTarihiLocal = DateTime.Now;
                    dBitisTarihi = String.Format("{0:dd.MM.yyyy}", dBitisTarihiLocal);
                }

                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                List<Models.Lokasyon.LokasyonListesi> lokasyons = new Models.Lokasyon().Gonder();

                float aracGirisi = new Models.Arac().AracGirisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracGirisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Girişi Sayısı", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                }

                float aracCikisi = new Models.Arac().AracCikisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Çıkışı Sayısı", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                }

                float iceridekiArac = new Models.Arac().IceridekiAracRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiArac > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam İçerideki Araç Sayısı", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        aracGirisi = new Models.Arac().AracGirisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (aracGirisi > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Girişi Sayısı ( " + lokasyons[i].cAdi + " )", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                        }

                        aracCikisi = new Models.Arac().AracCikisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (aracCikisi > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Çıkışı Sayısı ( " + lokasyons[i].cAdi + " )", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                        }

                        iceridekiArac = new Models.Arac().IceridekiAracRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (iceridekiArac > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam İçerideki Araç Sayısı ( " + lokasyons[i].cAdi + " )", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                        }
                    }
                }

                float kasaDevri = new Models.KasaDevri().KasaDevriRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (kasaDevri > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Kasa Devri Tutar", fDeger = (float)kasaDevri, cDeger = string.Format("{0:N2}", (float)kasaDevri) + " TL" });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        kasaDevri = new Models.KasaDevri().KasaDevriRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (kasaDevri > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Kasa Devri Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)kasaDevri, cDeger = string.Format("{0:N2}", (float)kasaDevri) + " TL" });
                        }
                    }
                }

                float gelir2 = new Models.Gelir2().Gelir2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (gelir2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gelir Genel Tutar", fDeger = (float)gelir2, cDeger = string.Format("{0:N2}", (float)gelir2) + " TL" });
                }

                float tahsilat2 = new Models.Tahsilat2().Tahsilat2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (tahsilat2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Tahsilat Tutar", fDeger = (float)tahsilat2, cDeger = string.Format("{0:N2}", (float)tahsilat2) + " TL" });
                }

                float gider2 = new Models.Gider2().Gider2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (gider2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gider Genel Tutar", fDeger = (float)gider2, cDeger = string.Format("{0:N2}", (float)gider2) + " TL" });
                }

                float odeme2 = new Models.Odeme2().Odeme2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (odeme2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ödeme Tutar", fDeger = (float)odeme2, cDeger = string.Format("{0:N2}", (float)odeme2) + " TL" });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        gelir2 = new Models.Gelir2().Gelir2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (gelir2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gelir Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)gelir2, cDeger = string.Format("{0:N2}", (float)gelir2) + " TL" });
                        }

                        tahsilat2 = new Models.Tahsilat2().Tahsilat2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (tahsilat2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Tahsilat Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)tahsilat2, cDeger = string.Format("{0:N2}", (float)tahsilat2) + " TL" });
                        }

                        gider2 = new Models.Gider2().Gider2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (gider2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gider Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)gider2, cDeger = string.Format("{0:N2}", (float)gider2) + " TL" });
                        }

                        odeme2 = new Models.Odeme2().Odeme2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (odeme2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ödeme Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)odeme2, cDeger = string.Format("{0:N2}", (float)odeme2) + " TL" });
                        }
                    }
                }

                float[] urunGirisi2 = new Models.UrunGirisi2().UrunGirisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (urunGirisi2 != null && urunGirisi2.Length == 3)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Kalemi Adedi", fDeger = (float)urunGirisi2[0], cDeger = string.Format("{0:N0}", (float)urunGirisi2[0]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Adedi", fDeger = (float)urunGirisi2[1], cDeger = string.Format("{0:N0}", (float)urunGirisi2[1]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Genel Tutar", fDeger = (float)urunGirisi2[2], cDeger = string.Format("{0:N2}", (float)urunGirisi2[2]) + " TL" });
                }

                float[] urunCikisi2 = new Models.UrunCikisi2().UrunCikisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (urunCikisi2 != null && urunCikisi2.Length == 3)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Kalemi Adedi", fDeger = (float)urunCikisi2[0], cDeger = string.Format("{0:N0}", (float)urunCikisi2[0]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Adedi", fDeger = (float)urunCikisi2[1], cDeger = string.Format("{0:N0}", (float)urunCikisi2[1]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Genel Tutar", fDeger = (float)urunCikisi2[2], cDeger = string.Format("{0:N2}", (float)urunCikisi2[2]) + " TL" });
                }

                float[] urunIade2 = new Models.UrunIade2().UrunIadeRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (urunIade2 != null && urunIade2.Length == 3)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Kalemi Adedi", fDeger = (float)urunIade2[0], cDeger = string.Format("{0:N0}", (float)urunIade2[0]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Adedi", fDeger = (float)urunIade2[1], cDeger = string.Format("{0:N0}", (float)urunIade2[1]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Genel Tutar", fDeger = (float)urunIade2[2], cDeger = string.Format("{0:N2}", (float)urunIade2[2]) + " TL" });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        urunGirisi2 = new Models.UrunGirisi2().UrunGirisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (urunGirisi2 != null && urunGirisi2.Length == 3)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Kalemi Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunGirisi2[0], cDeger = string.Format("{0:N0}", (float)urunGirisi2[0]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunGirisi2[1], cDeger = string.Format("{0:N0}", (float)urunGirisi2[1]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunGirisi2[2], cDeger = string.Format("{0:N2}", (float)urunGirisi2[2]) + " TL" });
                        }

                        urunCikisi2 = new Models.UrunCikisi2().UrunCikisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (urunCikisi2 != null && urunCikisi2.Length == 3)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Kalemi Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunCikisi2[0], cDeger = string.Format("{0:N0}", (float)urunCikisi2[0]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunCikisi2[1], cDeger = string.Format("{0:N0}", (float)urunCikisi2[1]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunCikisi2[2], cDeger = string.Format("{0:N2}", (float)urunCikisi2[2]) + " TL" });
                        }

                        urunIade2 = new Models.UrunIade2().UrunIadeRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (urunIade2 != null && urunIade2.Length == 3)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Kalemi Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunIade2[0], cDeger = string.Format("{0:N0}", (float)urunIade2[0]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunIade2[1], cDeger = string.Format("{0:N0}", (float)urunIade2[1]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunIade2[2], cDeger = string.Format("{0:N2}", (float)urunIade2[2]) + " TL" });
                        }
                    }
                }

                return View(raporlars);
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Raporlar", "Listele_Get", Ex.Message);
                return View();
            }
        }

        public ActionResult LokasyonListele(string dBaslangicTarihi, string dBitisTarihi)
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

                if (!new Class.Authority().Control(iKodKullaniciTipiLogin, 132))
                {
                    return Redirect("/CRM/AnaSayfa2");
                }

                

                List<Models.Raporlar> raporlars = new List<Models.Raporlar>();

                DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");
                if (!String.IsNullOrEmpty(dBaslangicTarihi) && !String.IsNullOrEmpty(dBitisTarihi))
                {
                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }
                }
                else
                {
                    dBaslangicTarihiLocal = DateTime.Now.AddDays(-7);
                    dBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", dBaslangicTarihiLocal);
                    dBitisTarihiLocal = DateTime.Now;
                    dBitisTarihi = String.Format("{0:dd.MM.yyyy}", dBitisTarihiLocal);
                }

                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                float aracGirisi = new Models.Arac().AracGirisiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracGirisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Girişi Sayısı", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                }

                float aracCikisi = new Models.Arac().AracCikisiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Sayısı", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                }

                float iceridekiArac = new Models.Arac().IceridekiAracRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiArac > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Sayısı", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                }

                float aracCikisiGenelTutar = new Models.Arac().AracCikisiGenelTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiGenelTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Genel Tutar", fDeger = (float)aracCikisiGenelTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiGenelTutar) + " TL" });
                }

                float aracCikisiOtoparkUcreti = new Models.Arac().AracCikisiOtoparkUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiOtoparkUcreti > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Otopark Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)aracCikisiOtoparkUcreti) + " TL" });
                }

                List<Models.Arac.UrunToplaFiyatlari> aracCikisiUrunUcreti = new Models.Arac().AracCikisiUrunUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiUrunUcreti != null)
                {
                    for (int i = 0; i < aracCikisiUrunUcreti.Count; i++)
                    {
                        raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı " + aracCikisiUrunUcreti[i].cUrun + " Adedi", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (int)aracCikisiUrunUcreti[i].iAdet) });
                        raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı " + aracCikisiUrunUcreti[i].cUrun + " Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)aracCikisiUrunUcreti[i].fFiyat) + " TL" });
                    }
                }

                float aracCikisiDuzeltmeEksiltTutar = new Models.Arac().AracCikisiDuzeltmeEksiltTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiDuzeltmeEksiltTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Düzelme (Eksilt)", fDeger = (float)aracCikisiDuzeltmeEksiltTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiDuzeltmeEksiltTutar) + " TL" });
                }

                float aracCikisiDuzeltmeArtirTutar = new Models.Arac().AracCikisiDuzeltmeArtirTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiDuzeltmeArtirTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Düzelme (Artır)", fDeger = (float)aracCikisiDuzeltmeArtirTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiDuzeltmeArtirTutar) + " TL" });
                }

                float iceridekiAracGenelTutar = new Models.Arac().IceridekiAracGenelTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracGenelTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Genel Tutar", fDeger = (float)iceridekiAracGenelTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracGenelTutar) + " TL" });
                }

                float iceridekiAracOtoparkUcreti = new Models.Arac().IceridekiAracOtoparkUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracOtoparkUcreti > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Otopark Ücreti", fDeger = (float)iceridekiAracOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)iceridekiAracOtoparkUcreti) + " TL" });
                }

                List<Models.Arac.UrunToplaFiyatlari> iceridekiAracUrunUcreti = new Models.Arac().IceridekiAracUrunUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracUrunUcreti != null)
                {
                    for (int i = 0; i < iceridekiAracUrunUcreti.Count; i++)
                    {
                        raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç " + iceridekiAracUrunUcreti[i].cUrun + " Adedi", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (int)iceridekiAracUrunUcreti[i].iAdet) });
                        raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç " + iceridekiAracUrunUcreti[i].cUrun + " Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)iceridekiAracUrunUcreti[i].fFiyat) + " TL" });
                    }
                }

                float iceridekiAracDuzeltmeEksiltTutar = new Models.Arac().IceridekiAracDuzeltmeEksiltTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracDuzeltmeEksiltTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Düzelme (Eksilt)", fDeger = (float)iceridekiAracDuzeltmeEksiltTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracDuzeltmeEksiltTutar) + " TL" });
                }

                float iceridekiAracDuzeltmeArtirTutar = new Models.Arac().IceridekiAracDuzeltmeArtirTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracDuzeltmeArtirTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Düzelme (Artır)", fDeger = (float)iceridekiAracDuzeltmeArtirTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracDuzeltmeArtirTutar) + " TL" });
                }

                return View(raporlars);
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Raporlar", "LokasyonListele_Get", Ex.Message);
                return View();
            }
        }

        public void Yazdir(string dBaslangicTarihi, string dBitisTarihi)
        {
            try
            {
                List<Models.Raporlar> raporlars = new List<Models.Raporlar>();

                DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");
                if (!String.IsNullOrEmpty(dBaslangicTarihi) && !String.IsNullOrEmpty(dBitisTarihi))
                {
                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }
                }
                else
                {
                    dBaslangicTarihiLocal = DateTime.Now.AddDays(-7);
                    dBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", dBaslangicTarihiLocal);
                    dBitisTarihiLocal = DateTime.Now;
                    dBitisTarihi = String.Format("{0:dd.MM.yyyy}", dBitisTarihiLocal);
                }

                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                List<Models.Lokasyon.LokasyonListesi> lokasyons = new Models.Lokasyon().Gonder();

                float aracGirisi = new Models.Arac().AracGirisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracGirisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Girişi Sayısı", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                }

                float aracCikisi = new Models.Arac().AracCikisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Çıkışı Sayısı", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                }

                float iceridekiArac = new Models.Arac().IceridekiAracRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiArac > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam İçerideki Araç Sayısı", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        aracGirisi = new Models.Arac().AracGirisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (aracGirisi > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Girişi Sayısı ( " + lokasyons[i].cAdi + " )", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                        }

                        aracCikisi = new Models.Arac().AracCikisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (aracCikisi > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Araç Çıkışı Sayısı ( " + lokasyons[i].cAdi + " )", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                        }

                        iceridekiArac = new Models.Arac().IceridekiAracRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (iceridekiArac > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam İçerideki Araç Sayısı ( " + lokasyons[i].cAdi + " )", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                        }
                    }
                }

                float kasaDevri = new Models.KasaDevri().KasaDevriRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (kasaDevri > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Kasa Devri Tutar", fDeger = (float)kasaDevri, cDeger = string.Format("{0:N2}", (float)kasaDevri) + " TL" });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        kasaDevri = new Models.KasaDevri().KasaDevriRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (kasaDevri > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Kasa Devri Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)kasaDevri, cDeger = string.Format("{0:N2}", (float)kasaDevri) + " TL" });
                        }
                    }
                }

                float gelir2 = new Models.Gelir2().Gelir2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (gelir2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gelir Genel Tutar", fDeger = (float)gelir2, cDeger = string.Format("{0:N2}", (float)gelir2) + " TL" });
                }

                float tahsilat2 = new Models.Tahsilat2().Tahsilat2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (tahsilat2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Tahsilat Tutar", fDeger = (float)tahsilat2, cDeger = string.Format("{0:N2}", (float)tahsilat2) + " TL" });
                }

                float gider2 = new Models.Gider2().Gider2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (gider2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gider Genel Tutar", fDeger = (float)gider2, cDeger = string.Format("{0:N2}", (float)gider2) + " TL" });
                }

                float odeme2 = new Models.Odeme2().Odeme2Rapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (odeme2 > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ödeme Tutar", fDeger = (float)odeme2, cDeger = string.Format("{0:N2}", (float)odeme2) + " TL" });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        gelir2 = new Models.Gelir2().Gelir2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (gelir2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gelir Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)gelir2, cDeger = string.Format("{0:N2}", (float)gelir2) + " TL" });
                        }

                        tahsilat2 = new Models.Tahsilat2().Tahsilat2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (tahsilat2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Tahsilat Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)tahsilat2, cDeger = string.Format("{0:N2}", (float)tahsilat2) + " TL" });
                        }

                        gider2 = new Models.Gider2().Gider2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (gider2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Gider Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)gider2, cDeger = string.Format("{0:N2}", (float)gider2) + " TL" });
                        }

                        odeme2 = new Models.Odeme2().Odeme2Rapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (odeme2 > 0)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ödeme Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)odeme2, cDeger = string.Format("{0:N2}", (float)odeme2) + " TL" });
                        }
                    }
                }

                float[] urunGirisi2 = new Models.UrunGirisi2().UrunGirisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (urunGirisi2 != null && urunGirisi2.Length == 3)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Kalemi Adedi", fDeger = (float)urunGirisi2[0], cDeger = string.Format("{0:N0}", (float)urunGirisi2[0]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Adedi", fDeger = (float)urunGirisi2[1], cDeger = string.Format("{0:N0}", (float)urunGirisi2[1]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Genel Tutar", fDeger = (float)urunGirisi2[2], cDeger = string.Format("{0:N2}", (float)urunGirisi2[2]) + " TL" });
                }

                float[] urunCikisi2 = new Models.UrunCikisi2().UrunCikisiRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (urunCikisi2 != null && urunCikisi2.Length == 3)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Kalemi Adedi", fDeger = (float)urunCikisi2[0], cDeger = string.Format("{0:N0}", (float)urunCikisi2[0]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Adedi", fDeger = (float)urunCikisi2[1], cDeger = string.Format("{0:N0}", (float)urunCikisi2[1]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Genel Tutar", fDeger = (float)urunCikisi2[2], cDeger = string.Format("{0:N2}", (float)urunCikisi2[2]) + " TL" });
                }

                float[] urunIade2 = new Models.UrunIade2().UrunIadeRapor(0, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (urunIade2 != null && urunIade2.Length == 3)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Kalemi Adedi", fDeger = (float)urunIade2[0], cDeger = string.Format("{0:N0}", (float)urunIade2[0]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Adedi", fDeger = (float)urunIade2[1], cDeger = string.Format("{0:N0}", (float)urunIade2[1]) });
                    raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Genel Tutar", fDeger = (float)urunIade2[2], cDeger = string.Format("{0:N2}", (float)urunIade2[2]) + " TL" });
                }

                if (lokasyons != null && lokasyons.Count > 0)
                {
                    for (int i = 0; i < lokasyons.Count; i++)
                    {
                        urunGirisi2 = new Models.UrunGirisi2().UrunGirisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (urunGirisi2 != null && urunGirisi2.Length == 3)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Kalemi Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunGirisi2[0], cDeger = string.Format("{0:N0}", (float)urunGirisi2[0]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Ürün Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunGirisi2[1], cDeger = string.Format("{0:N0}", (float)urunGirisi2[1]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Girişi Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunGirisi2[2], cDeger = string.Format("{0:N2}", (float)urunGirisi2[2]) + " TL" });
                        }

                        urunCikisi2 = new Models.UrunCikisi2().UrunCikisiRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (urunCikisi2 != null && urunCikisi2.Length == 3)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Kalemi Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunCikisi2[0], cDeger = string.Format("{0:N0}", (float)urunCikisi2[0]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Ürün Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunCikisi2[1], cDeger = string.Format("{0:N0}", (float)urunCikisi2[1]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün Çıkışı Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunCikisi2[2], cDeger = string.Format("{0:N2}", (float)urunCikisi2[2]) + " TL" });
                        }

                        urunIade2 = new Models.UrunIade2().UrunIadeRapor((int)lokasyons[i].iKodLokasyon, dBaslangicTarihiLocal, dBitisTarihiLocal);
                        if (urunIade2 != null && urunIade2.Length == 3)
                        {
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Kalemi Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunIade2[0], cDeger = string.Format("{0:N0}", (float)urunIade2[0]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Ürün Adedi ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunIade2[1], cDeger = string.Format("{0:N0}", (float)urunIade2[1]) });
                            raporlars.Add(new Models.Raporlar { cAdi = "Toplam Ürün İade Genel Tutar ( " + lokasyons[i].cAdi + " )", fDeger = (float)urunIade2[2], cDeger = string.Format("{0:N2}", (float)urunIade2[2]) + " TL" });
                        }
                    }
                }

                Response.ClearContent();
                Response.ContentType = "text/html";
                Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

                Response.Write("<html>");
                Response.Write("<head>");
                Response.Write("<meta charset=\"utf-8\">");
                Response.Write("</head>");
                Response.Write("<body onload=\"window.print();\">");

                Response.Write("<table style=\"border:1px solid #000;border-collapse: collapse;width: 1110px;\">");

                Response.Write("<tr>");

                Response.Write("<th colspan=\"5\" style=\"padding: 7px;vertical-align: middle;width: 100px;text-align: center;font-family: Calibri;font-size: 14pt;font-weight: 700;border:1px solid #000;\">RAPORLAR</th>");

                Response.Write("</tr>");

                for (int i = 0; i < raporlars.Count; i++)
                {
                    Response.Write("<tr>");

                    Response.Write("<td style=\"padding: 6px;padding-left: 15px;vertical-align: middle;width: 1000px;text-align: left;font-family: Calibri;font-size: 11pt;font-weight: 400;border:1px solid #000;\">" + raporlars[i].cAdi + "</td>");
                    Response.Write("<td style=\"padding: 6px;vertical-align: middle;width: 150px;text-align: right;font-family: Calibri;font-size: 11pt;font-weight: 400;border:1px solid #000;\">" + raporlars[i].cDeger + "</td>");

                    Response.Write("</tr>");
                }

                Response.Write("</table>");
                Response.Write("</body>");
                Response.Write("</html>");

                Response.End();
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Raporlar", "Listele_Get", Ex.Message);
            }
        }

        public void LokasyonYazdir(string dBaslangicTarihi, string dBitisTarihi)
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

                List<Models.Raporlar> raporlars = new List<Models.Raporlar>();

                DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");
                if (!String.IsNullOrEmpty(dBaslangicTarihi) && !String.IsNullOrEmpty(dBitisTarihi))
                {
                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }
                }
                else
                {
                    dBaslangicTarihiLocal = DateTime.Now.AddDays(-7);
                    dBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", dBaslangicTarihiLocal);
                    dBitisTarihiLocal = DateTime.Now;
                    dBitisTarihi = String.Format("{0:dd.MM.yyyy}", dBitisTarihiLocal);
                }

                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                float aracGirisi = new Models.Arac().AracGirisiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracGirisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Girişi Sayısı", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                }

                float aracCikisi = new Models.Arac().AracCikisiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Sayısı", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                }

                float iceridekiArac = new Models.Arac().IceridekiAracRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiArac > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Sayısı", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                }

                float aracCikisiGenelTutar = new Models.Arac().AracCikisiGenelTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiGenelTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Genel Tutar", fDeger = (float)aracCikisiGenelTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiGenelTutar) + " TL" });
                }

                float aracCikisiOtoparkUcreti = new Models.Arac().AracCikisiOtoparkUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiOtoparkUcreti > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Otopark Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)aracCikisiOtoparkUcreti) + " TL" });
                }

                List<Models.Arac.UrunToplaFiyatlari> aracCikisiUrunUcreti = new Models.Arac().AracCikisiUrunUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiUrunUcreti != null)
                {
                    for (int i = 0; i < aracCikisiUrunUcreti.Count; i++)
                    {
                        raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı " + aracCikisiUrunUcreti[i].cUrun + " Adedi", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (int)aracCikisiUrunUcreti[i].iAdet) });
                        raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı " + aracCikisiUrunUcreti[i].cUrun + " Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)aracCikisiUrunUcreti[i].fFiyat) + " TL" });
                    }
                }

                float aracCikisiDuzeltmeEksiltTutar = new Models.Arac().AracCikisiDuzeltmeEksiltTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiDuzeltmeEksiltTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Düzelme (Eksilt)", fDeger = (float)aracCikisiDuzeltmeEksiltTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiDuzeltmeEksiltTutar) + " TL" });
                }

                float aracCikisiDuzeltmeArtirTutar = new Models.Arac().AracCikisiDuzeltmeArtirTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiDuzeltmeArtirTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Düzelme (Artır)", fDeger = (float)aracCikisiDuzeltmeArtirTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiDuzeltmeArtirTutar) + " TL" });
                }

                float iceridekiAracGenelTutar = new Models.Arac().IceridekiAracGenelTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracGenelTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Genel Tutar", fDeger = (float)iceridekiAracGenelTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracGenelTutar) + " TL" });
                }

                float iceridekiAracOtoparkUcreti = new Models.Arac().IceridekiAracOtoparkUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracOtoparkUcreti > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Otopark Ücreti", fDeger = (float)iceridekiAracOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)iceridekiAracOtoparkUcreti) + " TL" });
                }

                List<Models.Arac.UrunToplaFiyatlari> iceridekiAracUrunUcreti = new Models.Arac().IceridekiAracUrunUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracUrunUcreti != null)
                {
                    for (int i = 0; i < iceridekiAracUrunUcreti.Count; i++)
                    {
                        raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç " + iceridekiAracUrunUcreti[i].cUrun + " Adedi", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (int)iceridekiAracUrunUcreti[i].iAdet) });
                        raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç " + iceridekiAracUrunUcreti[i].cUrun + " Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)iceridekiAracUrunUcreti[i].fFiyat) + " TL" });
                    }
                }

                float iceridekiAracDuzeltmeEksiltTutar = new Models.Arac().IceridekiAracDuzeltmeEksiltTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracDuzeltmeEksiltTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Düzelme (Eksilt)", fDeger = (float)iceridekiAracDuzeltmeEksiltTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracDuzeltmeEksiltTutar) + " TL" });
                }

                float iceridekiAracDuzeltmeArtirTutar = new Models.Arac().IceridekiAracDuzeltmeArtirTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracDuzeltmeArtirTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Düzelme (Artır)", fDeger = (float)iceridekiAracDuzeltmeArtirTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracDuzeltmeArtirTutar) + " TL" });
                }

                if (raporlars != null && raporlars.Count > 0)
                {
                    Response.ClearContent();
                    Response.ContentType = "text/html";
                    Response.ContentEncoding = System.Text.Encoding.Unicode;
                    Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

                    Response.Write("<html>");
                    Response.Write("<head>");
                    Response.Write("<meta charset=\"utf-8\">");
                    Response.Write("</head>");
                    Response.Write("<body onload=\"window.print();\">");

                    Response.Write("<table style=\"border:1px solid #000;border-collapse: collapse;width: 1110px;\">");

                    Response.Write("<tr>");

                    Response.Write("<th colspan=\"5\" style=\"padding: 7px;vertical-align: middle;width: 100px;text-align: center;font-family: Calibri;font-size: 14pt;font-weight: 700;border:1px solid #000;\">RAPORLAR</th>");

                    Response.Write("</tr>");

                    for (int i = 0; i < raporlars.Count; i++)
                    {
                        Response.Write("<tr>");

                        Response.Write("<td style=\"padding: 6px;padding-left: 15px;vertical-align: middle;width: 1000px;text-align: left;font-family: Calibri;font-size: 11pt;font-weight: 400;border:1px solid #000;\">" + raporlars[i].cAdi + "</td>");
                        Response.Write("<td style=\"padding: 6px;vertical-align: middle;width: 150px;text-align: center;font-family: Calibri;font-size: 11pt;font-weight: 400;border:1px solid #000;\">" + raporlars[i].cDeger + "</td>");

                        Response.Write("</tr>");
                    }

                    Response.Write("</table>");
                    Response.Write("</body>");
                    Response.Write("</html>");

                    Response.End();
                }
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Raporlar", "Listele_Get", Ex.Message);
            }
        }

        public ActionResult LokasyonBarkodYaziciYazdir(string dBaslangicTarihi, string dBitisTarihi)
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

                List<Models.Raporlar> raporlars = new List<Models.Raporlar>();

                DateTime dBaslangicTarihiLocal = Convert.ToDateTime("1900-01-01");
                DateTime dBitisTarihiLocal = Convert.ToDateTime("1900-01-01");
                if (!String.IsNullOrEmpty(dBaslangicTarihi) && !String.IsNullOrEmpty(dBitisTarihi))
                {
                    if (!String.IsNullOrEmpty(dBaslangicTarihi))
                    {
                        dBaslangicTarihiLocal = Convert.ToDateTime(dBaslangicTarihi);
                    }
                    if (!String.IsNullOrEmpty(dBitisTarihi))
                    {
                        dBitisTarihiLocal = Convert.ToDateTime(dBitisTarihi);
                    }
                }
                else
                {
                    dBaslangicTarihiLocal = DateTime.Now.AddDays(-7);
                    dBaslangicTarihi = String.Format("{0:dd.MM.yyyy}", dBaslangicTarihiLocal);
                    dBitisTarihiLocal = DateTime.Now;
                    dBitisTarihi = String.Format("{0:dd.MM.yyyy}", dBitisTarihiLocal);
                }

                ViewBag.dBaslangicTarihi = dBaslangicTarihi;
                ViewBag.dBitisTarihi = dBitisTarihi;

                float aracGirisi = new Models.Arac().AracGirisiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracGirisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Girişi Sayısı", fDeger = (float)aracGirisi, cDeger = string.Format("{0:N0}", (float)aracGirisi) });
                }

                float aracCikisi = new Models.Arac().AracCikisiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisi > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Sayısı", fDeger = (float)aracCikisi, cDeger = string.Format("{0:N0}", (float)aracCikisi) });
                }

                float iceridekiArac = new Models.Arac().IceridekiAracRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiArac > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Sayısı", fDeger = (float)iceridekiArac, cDeger = string.Format("{0:N0}", (float)iceridekiArac) });
                }

                float aracCikisiGenelTutar = new Models.Arac().AracCikisiGenelTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiGenelTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Genel Tutar", fDeger = (float)aracCikisiGenelTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiGenelTutar) + " TL" });
                }

                float aracCikisiOtoparkUcreti = new Models.Arac().AracCikisiOtoparkUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiOtoparkUcreti > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Otopark Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)aracCikisiOtoparkUcreti) + " TL" });
                }

                List<Models.Arac.UrunToplaFiyatlari> aracCikisiUrunUcreti = new Models.Arac().AracCikisiUrunUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiUrunUcreti != null)
                {
                    for (int i = 0; i < aracCikisiUrunUcreti.Count; i++)
                    {
                        raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı " + aracCikisiUrunUcreti[i].cUrun + " Adedi", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (int)aracCikisiUrunUcreti[i].iAdet) });
                        raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı " + aracCikisiUrunUcreti[i].cUrun + " Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)aracCikisiUrunUcreti[i].fFiyat) + " TL" });
                    }
                }

                float aracCikisiDuzeltmeEksiltTutar = new Models.Arac().AracCikisiDuzeltmeEksiltTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiDuzeltmeEksiltTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Düzelme (Eksilt)", fDeger = (float)aracCikisiDuzeltmeEksiltTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiDuzeltmeEksiltTutar) + " TL" });
                }

                float aracCikisiDuzeltmeArtirTutar = new Models.Arac().AracCikisiDuzeltmeArtirTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (aracCikisiDuzeltmeArtirTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "Araç Çıkışı Düzelme (Artır)", fDeger = (float)aracCikisiDuzeltmeArtirTutar, cDeger = string.Format("{0:N0}", (float)aracCikisiDuzeltmeArtirTutar) + " TL" });
                }

                float iceridekiAracGenelTutar = new Models.Arac().IceridekiAracGenelTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracGenelTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Genel Tutar", fDeger = (float)iceridekiAracGenelTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracGenelTutar) + " TL" });
                }

                float iceridekiAracOtoparkUcreti = new Models.Arac().IceridekiAracOtoparkUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracOtoparkUcreti > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Otopark Ücreti", fDeger = (float)iceridekiAracOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)iceridekiAracOtoparkUcreti) + " TL" });
                }

                List<Models.Arac.UrunToplaFiyatlari> iceridekiAracUrunUcreti = new Models.Arac().IceridekiAracUrunUcretiRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracUrunUcreti != null)
                {
                    for (int i = 0; i < iceridekiAracUrunUcreti.Count; i++)
                    {
                        raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç " + iceridekiAracUrunUcreti[i].cUrun + " Adedi", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (int)iceridekiAracUrunUcreti[i].iAdet) });
                        raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç " + iceridekiAracUrunUcreti[i].cUrun + " Ücreti", fDeger = (float)aracCikisiOtoparkUcreti, cDeger = string.Format("{0:N0}", (float)iceridekiAracUrunUcreti[i].fFiyat) + " TL" });
                    }
                }

                float iceridekiAracDuzeltmeEksiltTutar = new Models.Arac().IceridekiAracDuzeltmeEksiltTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracDuzeltmeEksiltTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Düzelme (Eksilt)", fDeger = (float)iceridekiAracDuzeltmeEksiltTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracDuzeltmeEksiltTutar) + " TL" });
                }

                float iceridekiAracDuzeltmeArtirTutar = new Models.Arac().IceridekiAracDuzeltmeArtirTutarRapor(iKodLokasyonLogin, dBaslangicTarihiLocal, dBitisTarihiLocal);
                if (iceridekiAracDuzeltmeArtirTutar > 0)
                {
                    raporlars.Add(new Models.Raporlar { cAdi = "İçerideki Araç Düzelme (Artır)", fDeger = (float)iceridekiAracDuzeltmeArtirTutar, cDeger = string.Format("{0:N0}", (float)iceridekiAracDuzeltmeArtirTutar) + " TL" });
                }

                if (raporlars != null && raporlars.Count > 0)
                {
                    var doc = new PrintDocument();
                    doc.PrinterSettings.PrinterName = "Hoin-58-Series";
                    doc.PrintPage += (sender, args) => ProvideContent(raporlars, args);
                    doc.Print();
                }

                return Redirect("/Raporlar/LokasyonListele?dBaslangicTarihi=" + dBaslangicTarihi + "&dBitisTarihi=" + dBitisTarihi);
            }
            catch (Exception Ex)
            {
                ViewBag.iSonuc = -2;
                new Class.Log().Hata("Raporlar", "Listele_Get", Ex.Message);
                return Redirect("/Raporlar/LokasyonListele?dBaslangicTarihi=" + dBaslangicTarihi + "&dBitisTarihi=" + dBitisTarihi);
            }
        }

        private void ProvideContent(object sender, PrintPageEventArgs e)
        {
            List<Models.Raporlar> raporlars = sender as List<Models.Raporlar>;
            if (raporlars != null && raporlars.Count > 0)
            {
                int iSatir = 15;
                for (int i = 0; i < raporlars.Count; i++)
                {
                    e.Graphics.DrawString(raporlars[i].cAdi, new Font("Arial", 8), Brushes.Black, 3, iSatir);
                    iSatir += 15;
                    e.Graphics.DrawString(raporlars[i].cDeger, new Font("Arial", 8), Brushes.Black, 3, iSatir);
                    iSatir += 15;
                }
                e.Graphics.DrawString("-------", new Font("Arial", 8), Brushes.Black, 3, iSatir);
            }
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