using CasinoRegistro.Data;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using CasinoRegistro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository
{
    public class PlataformaRepository : Repository<Plataforma>, IPlataformaRepository
    {
        private readonly CasinoRegistroDbContext _db;

        public PlataformaRepository(CasinoRegistroDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Plataforma plataforma)
        {
            var objDesdeDb = _db.Plataforma.FirstOrDefault(s => s.Id == plataforma.Id);
            objDesdeDb.URL = plataforma.URL;
            objDesdeDb.Descripcion = plataforma.Descripcion;

        }
    }
}
