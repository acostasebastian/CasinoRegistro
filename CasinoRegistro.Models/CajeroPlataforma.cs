using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.Models
{
    public class CajeroPlataforma
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CajeroUserId")]
        public int CajeroUserId { get; set; }

        [ForeignKey("PlataformaId")]
        public int PlataformaId { get; set; }

        public CajeroUser? CajeroUser { get; set; }

        public Plataforma? Plataforma { get; set; }

    }
}
