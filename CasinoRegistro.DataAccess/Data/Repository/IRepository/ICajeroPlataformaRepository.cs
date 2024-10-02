using CasinoRegistro.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository.IRepository
{
    public interface ICajeroPlataformaRepository : IRepository<CajeroPlataforma>
    {

        void Update(CajeroPlataforma cajeroPlataforma);

   
        //List<SelectListItem> GetListaPlataformasCajeros(IEnumerable<CajeroPlataforma> cajeroPlataformasPorCajero);
        IEnumerable<SelectListItem> GetListaPlataformasCajeros(IEnumerable<CajeroPlataforma> cajeroPlataformasPorCajero);

    }
}
