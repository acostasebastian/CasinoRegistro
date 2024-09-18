using CasinoRegistro.Data;
using CasinoRegistro.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Data.Initialiser
{
    public class InicializadorBD : IInicializadorBD
    {

        private readonly ApplicationDbContext _userContext;
        private readonly CasinoRegistroDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;




        public InicializadorBD(CasinoRegistroDbContext db, ApplicationDbContext userContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager) // : base(db)
        {
            _db = db;
            _userContext = userContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Inicializar()
        {
            //Creación del usuario inicial
            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "joseandresmontoya@hotmail.com",
                Email = "joseandresmontoya@hotmail.com",
                EmailConfirmed = true,
                // Nombre = "render2web"
            }, "Admin123*").GetAwaiter().GetResult();

            CajeroUser usuario = _db.Cajero.Where(us => us.Email == "joseandresmontoya@hotmail.com").FirstOrDefault();
        }
    }
}
