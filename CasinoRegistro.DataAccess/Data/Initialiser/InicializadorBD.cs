using CasinoRegistro.Data;
using CasinoRegistro.Models;
using CasinoRegistro.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                //Se verifica si hay migraciones pendientes. Y si las hay, se ejecutan
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

                if (_userContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _userContext.Database.Migrate();
                }


            }
            catch (Exception)
            {

                throw;
            }

            //si se encuentra algun rol Administrador, se accede
            if (_userContext.Roles.Any(ro => ro.Name == CNT.Administrador)) return;

            //Creacion de roles
            _roleManager.CreateAsync(new IdentityRole(CNT.Administrador)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Secretaria)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Cajero)).GetAwaiter().GetResult();


            //Creación del usuario inicial
            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "edith@gmail.com",
                Email = "edith@gmail.com",
                EmailConfirmed = true,               
            }, "Admin1234.").GetAwaiter().GetResult();


            IdentityUser usuario = _userContext.Users.Where(us => us.Email == "edith@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(usuario, CNT.Administrador).GetAwaiter().GetResult();


            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "maurosebastiandesarrollo@gmail.com",
                Email = "maurosebastiandesarrollo@gmail.com",
                EmailConfirmed = true,
            }, "Admin1234.").GetAwaiter().GetResult();


            IdentityUser usuarioAdmin = _userContext.Users.Where(us => us.Email == "maurosebastiandesarrollo@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(usuarioAdmin, CNT.Administrador).GetAwaiter().GetResult();

        }
    }
}
