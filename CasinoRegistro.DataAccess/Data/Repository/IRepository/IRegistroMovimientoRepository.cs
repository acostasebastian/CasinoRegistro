using CasinoRegistro.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository.IRepository
{
    public interface IRegistroMovimientoRepository : IRepository<RegistroMovimiento>
    {
        IEnumerable<SelectListItem>? GetListaCajeros();

        void Update(RegistroMovimiento registroMovimiento);
    }


}
