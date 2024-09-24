using CasinoRegistro.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Repository.IRepository
{
    public interface ICajeroRepository : IRepository<CajeroUser>
    {
        void Update(CajeroUser cajero);

        void UpdateDeuda(int idCajero, decimal nuevaDeuda);


        void BloquearCajero(CajeroUser cajeroDesdeBd, Task<Microsoft.AspNetCore.Identity.IdentityUser?> objFromDb);
        void DesloquearCajero(CajeroUser cajeroDesdeBd, Task<Microsoft.AspNetCore.Identity.IdentityUser?> objFromDb);


    }
}
