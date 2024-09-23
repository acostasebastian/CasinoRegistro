using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.Models.ViewModels
{
    public class RegistroMovimientoViewModel :RegistroMovimiento
    {
        //public RegistroMovimiento RegistroMovimiento { get; set; }
        public IEnumerable<SelectListItem>? ListaCajeros { get; set; }


        [Required(ErrorMessage = "La Fecha es obligatoria")]
        public DateTime Fecha { get; set; }

    }
}
