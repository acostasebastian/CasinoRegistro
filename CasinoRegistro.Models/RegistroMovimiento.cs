﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.Models
{
    public class RegistroMovimiento
    {

        [Key]
        public int Id { get; set; }

        //Cajero para el que se carga el movimiento
        [Required(ErrorMessage = "El Cajero es obligatorio")]
        public int CajeroId { get; set; }

        [ForeignKey("CajeroId")]
        [Display(Name = "Cajero")]        
        public CajeroUser? CajeroUser { get; set; }
      

        [Required(ErrorMessage = "La Fecha es obligatoria")]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]

        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fichas Cargadas")]
        public int? FichasCargadas { get; set; }

        [Display(Name = "Pesos Entregados")]        
        public decimal? PesosEntregados { get; set; }

        [Display(Name = "Pesos Devueltos")]
        public decimal? PesosDevueltos { get; set; }

        [Display(Name = "Comisión")]
        public decimal? Comision { get; set; }

        public bool EsIngresoFichas { get; set; }



    }
}
