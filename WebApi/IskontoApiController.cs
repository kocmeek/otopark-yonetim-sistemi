using System;
using System.Linq;
using System.Web.Http;

namespace otoparkyonetim.Controllers
{
    public class IskontoApiController : ApiController
    {
        public int Get(int id)
        {
            try
            {
                if (id > 0)
                {
                    using (Data.DataClassesDataContext dc = new Data.DataClassesDataContext())
                    {
                        var result =
                            (from table in dc.Musteri4s
                             where
                                table.iKodMusteri4 == id &&
                                table.iAktifMi == 1
                             select table).FirstOrDefault();
                        if (result != null)
                        {
                            return (int)result.iIskontoYuzdeOrani;
                        }
                    }
                }
                return 0;
            }
            catch (Exception Ex)
            {
                new Class.Log().Hata("Models Class Hatası", "IskontoApiController.cs", Ex.Message);
                return 0;
            }
        }
    }
}
