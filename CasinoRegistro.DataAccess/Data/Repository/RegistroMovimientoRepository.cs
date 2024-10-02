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



        public void Update(RegistroMovimiento registroMovimiento)
        {
            var objDesdeDb = _db.RegistroMovimiento.FirstOrDefault(s => s.Id == registroMovimiento.Id);
            objDesdeDb.CajeroId = registroMovimiento.CajeroId;
            objDesdeDb.FechaCreacion = registroMovimiento.FechaCreacion;
            objDesdeDb.FichasCargadas = registroMovimiento.FichasCargadas;
            objDesdeDb.PesosEntregados  = registroMovimiento.PesosEntregados; 
            objDesdeDb.PesosDevueltos = registroMovimiento.PesosDevueltos;
            objDesdeDb.Comision = registroMovimiento.Comision;
            objDesdeDb.EsIngresoFichas = registroMovimiento.EsIngresoFichas;

        }

    }
}