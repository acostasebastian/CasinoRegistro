using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.Models.ViewModels
{
    public class RegistroMovimientoViewModel //:RegistroMovimiento
    {
        public RegistroMovimiento RegistroMovimientoVM { get; set; }
        public IEnumerable<SelectListItem>? ListaCajeros { get; set; }


        [Required(ErrorMessage = "La Fecha es obligatoria")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

      

        public decimal pesosEntrega { get; set; }

    }
}
