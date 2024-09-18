using CasinoRegistro.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CasinoRegistro.DataAccess.Data;
using CasinoRegistro.Models;

namespace CasinoRegistro.DataAccess.Helpers
{
    public class UsersHelper // : IDisposable
    {

        //// private static ApplicationDbContext userContext = new ApplicationDbContext();
        ////private static CasinoRegistroDbContext db = new CasinoRegistroDbContext();

        //private readonly ApplicationDbContext _userContext;
        //private readonly CasinoRegistroDbContext _db;
        //private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;




        //public UsersHelper(CasinoRegistroDbContext db, ApplicationDbContext userContext,
        //    UserManager<IdentityUser> userManager,
        //    RoleManager<IdentityRole> roleManager) // : base(db)
        //{
        //    _db = db;
        //    _userContext = userContext;
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //}


        //public void CreateUserASP(string email, string roleName)
        //     public void CreateUserASP(string email)
        //{
        //    //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

        //    //_userContext.Users(userManager, email, roleName);


        //    //var userASP = new ApplicationUser
        //    //{
        //    //    Email = email,
        //    //    UserName = email,
        //    //};

        //    //userManager.Create(userASP, email);
        //    //userManager.AddToRole(userASP.Id, roleName);

        //    //Creación del usuario inicial
        //    _userManager.CreateAsync(new IdentityUser
        //    {
        //        //UserName = "joseandresmontoya@hotmail.com",
        //        //Email = "joseandresmontoya@hotmail.com",
        //        UserName = email,
        //        Email = email,
                
        //        EmailConfirmed = true,
        //       // Nombre = "render2web"
        //    }, "Admin1234.").GetAwaiter().GetResult();

        //    //CajeroUser usuario = _db.Cajero.Where(us => us.Email == "joseandresmontoya@hotmail.com").FirstOrDefault();
        //    CajeroUser usuario = _db.Cajero.Where(us => us.Email == email).FirstOrDefault();
        //    // _userManager.AddToRoleAsync(usuario, CNT.Administrador).GetAwaiter().GetResult();
        //}

        //public static void CheckRole(string roleName)
        //{
        //  //  var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_userContext));



        //    // Check to see if Role Exists, if not create it
        //    //if (!roleManager.RoleExists(roleName))
        //    if (_roleManager.r(roleName))
        //    {
        //        roleManager.Create(new IdentityRole(roleName));
        //    }
        //}

        //public static void CheckSuperUser()
        //{
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
        //    var email = WebConfigurationManager.AppSettings["AdminUser"];
        //    var password = WebConfigurationManager.AppSettings["AdminPassWord"];
        //    var userASP = userManager.FindByName(email);
        //    if (userASP == null)
        //    {
        //        CreateUserASP(email, "Admin", password);
        //        return;
        //    }

        //    userManager.AddToRole(userASP.Id, "Admin");
        //}
        //public static void CreateUserASP(string email, string roleName)
        //{
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

        //    var userASP = new ApplicationUser
        //    {
        //        Email = email,
        //        UserName = email,
        //    };

        //    userManager.Create(userASP, email);
        //    userManager.AddToRole(userASP.Id, roleName);
        //}

        //public static void CreateUserASP(string email, string roleName, string password)
        //{
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

        //    var userASP = new _userContext. ApplicationUser
        //    {
        //        Email = email,
        //        UserName = email,
        //    };

        //    userManager.Create(userASP, password);
        //    userManager.AddToRole(userASP.Id, roleName);
        //}

        //public static async Task PasswordRecovery(string email)
        //{
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
        //    var userASP = userManager.FindByEmail(email);
        //    if (userASP == null)
        //    {
        //        return;
        //    }

        //    var user = db.Users.FirstOrDefault(tp => tp.UserName == email);
        //    if (user == null)
        //    {
        //        return;
        //    }

        //    var random = new Random();
        //    var newPassword = string.Format("{0}{1}{2:04}*",
        //        user.FirstName.Trim().ToUpper().Substring(0, 1),
        //        user.LastName.Trim().ToLower(),
        //        random.Next(10000));

        //    userManager.RemovePassword(userASP.Id);
        //    userManager.AddPassword(userASP.Id, newPassword);

        //    var subject = "ECommerce Password Recovery";
        //    var body = string.Format(@"
        //        <h1>ECommerce Password Recovery</h1>
        //        <p>Yor new password is: <strong>{0}</strong></p>
        //        <p>Please change it for one, that you remember easyly",
        //        newPassword);

        //    await MailHelper.SendMail(email, subject, body);
        //}

        //public void Dispose()
        //{
        //    userContext.Dispose();
        //    db.Dispose();
        //}

    }
}
