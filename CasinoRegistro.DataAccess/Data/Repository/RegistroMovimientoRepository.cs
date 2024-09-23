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
    public class RegistroMovimientoRepository : Repository<RegistroMovimiento>, IRegistroMovimientoRepository
    {

        private readonly CasinoRegistroDbContext _db;

        public RegistroMovimientoRepository(CasinoRegistroDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem>? GetListaCajeros()
        {
            return _db.Cajero.Where(c => c.Rol == "Cajero").Select(i => new SelectListItem()
            {
                Text = i.NombreCompleto,
                Value = i.Id.ToString(),

            });
        }



        public void Update(RegistroMovimiento registroMovimiento)
        {
            //var objDesdeDb = _db.Plataforma.FirstOrDefault(s => s.Id == plataforma.Id);
            //objDesdeDb.URL = plataforma.URL;
            //objDesdeDb.Descripcion = plataforma.Descripcion;

            throw new NotImplementedException();
        }

    }
}