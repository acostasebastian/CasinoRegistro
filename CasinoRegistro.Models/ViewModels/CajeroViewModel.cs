using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CasinoRegistro.Models.ViewModels
{
    public class CajeroViewModel // : Plataforma
    {
        public CajeroUser CajeroUserVM { get; set; }

     //    public CajeroPlataforma CajeroPlataformaVM { get; set; }
       // public int PlataformaVMId { get; set; }

        //public int[] Ids { get; set; }
        public List<int> IdsPlataformas { get; set; }

        public IEnumerable<SelectListItem>? ListaPlataformas { get; set; }
    }
}
