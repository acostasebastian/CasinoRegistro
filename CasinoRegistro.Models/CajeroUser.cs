﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CasinoRegistro.Models
{
    [Index("DNI", IsUnique = true, Name = "IX_Cajeros_DNI")]
    [Index("Email", IsUnique = true, Name = "IX_Cajeros_Email")]
    public class CajeroUser
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El Email no es una dirección de correo electrónico válida.")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string DNI { get; set; }

        [Required(ErrorMessage = "El Teléfono es obligatorio")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }


        [Required(ErrorMessage = "Las fichas a cargar es obligatorio")]
        [Display(Name = "Fichas que puede cargar")]
        public int FichasCargar { get; set; }

        [Required(ErrorMessage = "El Porcentaje de Comisión es obligatorio")]
        [Display(Name = "Porcentaje de Comisión")]
        public double PorcentajeComision { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Foto")]
        public string? UrlImagen { get; set; }

        public string? Rol { get; set; }

        public bool Estado { get; set; }

        [Display(Name = "Deuda en Pesos Actual")]
        public decimal? DeudaPesosActual { get; set; }

        [Display(Name = "Nombre Completo")]

        public bool EsCajero { get; set; }

        public string NombreCompleto
        {
            get { return Nombre + " " + Apellido; }
        }

        List<CajeroPlataforma> CajerosPlataformas { get; set; }
    }
}
