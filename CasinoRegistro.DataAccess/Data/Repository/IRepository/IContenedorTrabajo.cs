using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        //Aquí se deben de ir agregando los diferentes repositorios
        IPlataformaRepository Plataforma { get; }

     //   ICajeroRepository Cajero { get; }

        void Save();
    }
}
