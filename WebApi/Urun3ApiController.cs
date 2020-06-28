using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace otoparkcogencomtr.Controllers
{
    public class Urun3ApiController : ApiController
    {
        public List<Models.Urun3> Get()
        {
            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
            {
                return (from table in dc.Urun3s
                        where
                          table.iAktifMi == 1
                        select new Models.Urun3
                        {
                            iKodUrun3 = table.iKodUrun3,
                            cKodu = table.cKodu,
                            cAdi = "Kodu : " + table.cKodu + " - Adı : " + table.cAdi + " - Raf : " + table.cRaf,
                            iAdet = table.iAdet - (-1 * table.iSayimFarki) < 0 ? 0 : table.iAdet - (-1 * table.iSayimFarki)
                        }).OrderBy(x => x.cAdi).ToList();
            }
        }

        public Models.Urun3 Get(string id)
        {
            using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
            {
                int iKodUrun3 = 0;
                if (int.TryParse(id, out iKodUrun3))
                {
                    var resultUrunBaglanti3s = (from table in dc.UrunBaglanti3s
                                               where
                                                 table.iKodUrun32 == iKodUrun3 &&
                                                 table.iAktifMi == 1
                                               select table).FirstOrDefault();

                    if (resultUrunBaglanti3s != null)
                    {
                        var resultUrunBaglanti3sUrun3s =
                            (from table in dc.Urun3s
                             where
                               table.iKodUrun3 == resultUrunBaglanti3s.iKodUrun31 &&
                               table.iAktifMi == 1
                             select new Models.Urun3
                             {
                                 iKodUrun3 = table.iKodUrun3,
                                 cKodu = table.cKodu,
                                 cAdi = table.cKodu + " - " + table.cAdi,
                                 iAdet = table.iAdet - (-1 * table.iSayimFarki) < 0 ? 0 : table.iAdet - (-1 * table.iSayimFarki),
                                 cSatisFiyati = string.Format("{0:N2}", table.fSatisFiyati)
                             }).FirstOrDefault();

                        if (resultUrunBaglanti3sUrun3s != null)
                        {
                            return (from table in dc.Urun3s
                                    where
                                      table.iKodUrun3 == iKodUrun3 &&
                                      table.iAktifMi == 1
                                    select new Models.Urun3
                                    {
                                        iKodUrun3 = table.iKodUrun3,
                                        cKodu = table.cKodu,
                                        cAdi = "Kodu : " + table.cKodu + " - Adı : " + table.cAdi + " - Raf : " + table.cRaf,
                                        iAdet = resultUrunBaglanti3sUrun3s.iAdet,
                                        cSatisFiyati = string.Format("{0:N2}", table.fSatisFiyati)
                                    }).FirstOrDefault();
                        }
                    }
                    else
                    {
                        return (from table in dc.Urun3s
                                where
                                  table.iKodUrun3 == iKodUrun3 &&
                                  table.iAktifMi == 1
                                select new Models.Urun3
                                {
                                    iKodUrun3 = table.iKodUrun3,
                                    cKodu = table.cKodu,
                                    cAdi = "Kodu : " + table.cKodu + " - Adı : " + table.cAdi + " - Raf : " + table.cRaf,
                                    iAdet = table.iAdet - (-1 * table.iSayimFarki) < 0 ? 0 : table.iAdet - (-1 * table.iSayimFarki),
                                    cSatisFiyati = string.Format("{0:N2}", table.fSatisFiyati)
                                }).FirstOrDefault();
                    }
                }
            }

            return null;
        }

        public int Put(Models.Urun3 urun)
        {
            if (urun != null)
            {
                using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                {
                    var kontrol = (from table in dc.Urun3s
                                   where
                                       table.cKodu == urun.cKodu &&
                                       (table.iAktifMi == 0 || table.iAktifMi == 1)
                                   select table).FirstOrDefault();

                    if (kontrol == null)
                    {
                        string cResimListesi = string.Empty;
                        if (!String.IsNullOrEmpty(urun.cResimListesi))
                        {
                            string[] cResimler = urun.cResimListesi.Split('|');
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
                                urun.resimListesi = JsonConvert.DeserializeObject<List<Models.Resim>>(cResimListesi);
                            }
                        }

                        Data.Urun3 yenikayit = new Data.Urun3();
                        yenikayit.cFotograf = cResimListesi;
                        yenikayit.cKodu = urun.cKodu;
                        yenikayit.cAdi = urun.cAdi;
                        yenikayit.cRaf = urun.cRaf;
                        yenikayit.iKritikAdet = urun.iKritikAdet;
                        yenikayit.iSayimFarki = urun.iSayimFarki;
                        yenikayit.iAdet = 0;
                        yenikayit.fSatisFiyati = Convert.ToDouble(urun.cSatisFiyati.ToString().Replace(",", "").Replace(".", ","));
                        yenikayit.dTarih = DateTime.Now;
                        yenikayit.iAktifMi = urun.iAktifMi;
                        yenikayit.iSonGuncelleyenKullanici = urun.iKodKullaniciLogin;
                        dc.Urun3s.InsertOnSubmit(yenikayit);
                        dc.SubmitChanges();

                        return yenikayit.iKodUrun3;
                    }
                    else
                    {
                        return -3;
                    }
                }
            }
            else
            {
                return -2;
            }
        }
    }
}
