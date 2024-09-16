using CasinoRegistro.Data;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using CasinoRegistro.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository
{
    public class CajeroRepository : Repository<CajeroUser>, ICajeroRepository
    {
        private readonly CasinoRegistroDbContext _db;

        public CajeroRepository(CasinoRegistroDbContext db) : base(db)
        {
            _db = db;
        }       

        public void Update(CajeroUser cajero)
        {
           
            //LA CONTRASEÑA LO PUEDE HACER EL CAJERO DESDE EL PERFIL, EL CORREO NO ESTÁ HABILITADO
            var objDesdeDb = _db.Cajero.FirstOrDefault(s => s.Id == cajero.Id);
            objDesdeDb.Email = cajero.Email;
            objDesdeDb.Nombre = cajero.Nombre;
            objDesdeDb.Apellido = cajero.Apellido;
            objDesdeDb.DNI = cajero.DNI;
            objDesdeDb.Telefono = cajero.Telefono;
            objDesdeDb.FichasCargar = cajero.FichasCargar;
            objDesdeDb.PorcentajeComision = cajero.PorcentajeComision;
            objDesdeDb.UrlImagen = cajero.UrlImagen;


        }
      
    }
}
