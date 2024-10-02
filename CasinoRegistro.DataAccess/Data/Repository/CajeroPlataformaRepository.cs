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
    public class CajeroPlataformaRepository : Repository<CajeroPlataforma>, ICajeroPlataformaRepository
    {

        private readonly CasinoRegistroDbContext _db;
        // private readonly ApplicationDbContext _dbAplication;

        public CajeroPlataformaRepository(CasinoRegistroDbContext db) : base(db)
        {
            _db = db;
        }
            


        //Si en vez de usar IEnumerable uso una lista, debe usarse así el metodo >> public List<SelectListItem> GetListaPlataformasCajeros(IEnumerable<CajeroPlataforma> cajeroPlataformasPorCajero)
        public IEnumerable<SelectListItem>? GetListaPlataformasCajeros(IEnumerable<CajeroPlataforma> cajeroPlataformasPorCajero)
        {          

            //busco las plataformas y las agrego en una lista
            List<Plataforma> lst = new List<Plataforma>();
           
                foreach (var item in cajeroPlataformasPorCajero)
                {
                    lst.Add(item.Plataforma);

                }           
            
            //la lista antes creada, la convierto de esta forma en una lista de SelectLisItem
            List<SelectListItem> plat = lst.ConvertAll(p =>
            {
                return new SelectListItem()
                {
                    Text = p.URL,
                    Value = p.Id.ToString()
                };
            });

            return plat;
        }

        public void Update(CajeroPlataforma cajeroPlataforma)
        {
            throw new NotImplementedException();
        }

     
    }
}
