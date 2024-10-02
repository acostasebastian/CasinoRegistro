using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CasinoRegistro.Models
{
    [Index("URL", IsUnique = true, Name = "IX_Plataformas_URL")]
    public class Plataforma
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar la URL de la plataforma")]
        public string URL { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        List<CajeroPlataforma> CajerosPlataformas { get; set; }
    }
}
