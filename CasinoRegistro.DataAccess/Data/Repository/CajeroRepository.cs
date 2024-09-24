using CasinoRegistro.Data;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using CasinoRegistro.Models;
using Microsoft.AspNetCore.Identity;
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
       // private readonly ApplicationDbContext _dbAplication;

        public CajeroRepository(CasinoRegistroDbContext db
            //, ApplicationDbContext dbAplication
            ) : base(db)
        {
            _db = db;
         //   _dbAplication = dbAplication;
        }


        public void DesloquearCajero(CajeroUser cajeroDesdeBd, Task<IdentityUser?> objFromDb)
        {

            //le agrego al campo de la tabla AspUseres la fecha actual
            //haciendo que el cajero quede desbloqueado
            objFromDb.Result.LockoutEnd = null;
            cajeroDesdeBd.Estado = true;

        }
        public void BloquearCajero(CajeroUser cajeroDesdeBd,Task<IdentityUser?> objFromDb)
        {

            //le agrego al campo de la tabla AspUseres una fecha hasta la cual estará bloqueado
            //haciendo que el cajero quede bloqueado
            objFromDb.Result.LockoutEnd = DateTime.Now.AddYears(1000);

            cajeroDesdeBd.Estado = false;

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
            objDesdeDb.Rol  = cajero.Rol;

        }


        public void UpdateDeuda(int idCajero, decimal nuevaDeuda)
        {            
            var objDesdeDb = _db.Cajero.FirstOrDefault(s => s.Id == idCajero);
            objDesdeDb.DeudaPesosActual = nuevaDeuda;
        

        }


    }
}
