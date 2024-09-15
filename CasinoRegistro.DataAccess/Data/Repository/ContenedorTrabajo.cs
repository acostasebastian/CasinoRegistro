using CasinoRegistro.Data;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly CasinoRegistroDbContext _db;
        public ContenedorTrabajo(CasinoRegistroDbContext db)
        {
            _db = db;
            Plataforma = new PlataformaRepository(_db);
           // Cajero = new CajeroRepository(_db);

        }

        public IPlataformaRepository Plataforma { get; private set; }

    //    public ICajeroRepository Cajero { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
