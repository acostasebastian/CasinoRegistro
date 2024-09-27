using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CasinoRegistro.DataAccess.Data;
using CasinoRegistro.Models;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using Microsoft.CodeAnalysis;
using CasinoRegistro.Models.ViewModels;
using System.Net;
using CasinoRegistro.DataAccess.Data.Initialiser;
using CasinoRegistro.DataAccess.Helpers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Identity;
using CasinoRegistro.Utilities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Azure.Identity;
using CasinoRegistro.Data;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
//using System.Net.Mail;

namespace CasinoRegistro.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CajerosController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment; // para acceder a las carpetas del proyecto, para guardar/editar la imagen      
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CasinoRegistroDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;



        public CajerosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment,
            UserManager<IdentityUser> userManager, CasinoRegistroDbContext db, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
            _config = config;
        }

        [Authorize(Roles = "Administrador,Secretaria")]
        // GET: Admin/Cajeros >> vista de cajeros
        public async Task<IActionResult> Index()
        {
            return View();         
        }

        [Authorize(Roles = "Administrador")]
        // GET: Admin/Cajeros >> vista de Secretarias
        public async Task<IActionResult> IndexSecretarias()
        {

            return View();
        }


        [Authorize(Roles = "Administrador,Secretaria")]
        // GET: Admin/Cajeros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
                // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            };

            if (id != null)
            {
                cajeroViewModel.CajeroUserVM = _contenedorTrabajo.Cajero.Get(id.GetValueOrDefault());


                if (cajeroViewModel.CajeroUserVM == null)
                {
                    return NotFound();

                }

                if (cajeroViewModel.CajeroUserVM.EsCajero == false)
                {
                    ViewBag.Titulo = "Detalles Secretaria";
                    ViewBag.Encabezado = "Detalles Secretaria";
                }

                else
                {
                    ViewBag.Titulo = "Detalles Cajero";
                    ViewBag.Encabezado = "Detalles Cajero";
                }
            }           

       

            return View(cajeroViewModel);

        }

        [Authorize(Roles = "Administrador,Secretaria")]
        // GET: Admin/Cajeros/Create >> Cajeros
        [HttpGet]
        public IActionResult Create()
        {      
            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
                
                // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            };

            cajeroViewModel.CajeroUserVM.EsCajero = true;

            ViewBag.Titulo = "Crear Cajero";
            ViewBag.Encabezado = "Crear un nuevo Cajero";
            return View("Create",cajeroViewModel);
        }

        [Authorize(Roles = "Administrador")]
        // GET: Admin/Cajeros/Create >> Secretaria
        [HttpGet]
        public IActionResult CreateSecretaria()
        {
            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
                // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            };

            cajeroViewModel.CajeroUserVM.EsCajero = false;
            ViewBag.Titulo = "Crear Secretaria";
            ViewBag.Encabezado = "Crear una nueva Secretaria";


            return View("Create", cajeroViewModel);
        }


        [Authorize(Roles = "Administrador,Secretaria")]
        // POST: Admin/Cajeros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CajeroViewModel cajeroVM)
        {

            if (ModelState.IsValid)
            {

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Imagen
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivo = HttpContext.Request.Form.Files;

                        if (cajeroVM.CajeroUserVM.Id == 0 && archivo.Count() > 0)
                        {
                            //Nuevo Cajero
                            string nombreArchivo = Guid.NewGuid().ToString();   //como es nuevo el cajero, le pongo un Guid como nombre
                            var subidas = Path.Combine(rutaPrincipal, @"imagenes\cajeros"); //accederá a la carpeta en wwwroot
                            var extension = Path.GetExtension(archivo[0].FileName);

                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivo[0].CopyTo(fileStreams);
                            }

                            //guardo la ruta en la base de datos
                            cajeroVM.CajeroUserVM.UrlImagen = @"imagenes\cajeros\" + nombreArchivo + extension;

                        }
                        #endregion

                        #region Usuario y Rol


                        //CREA EL USUARIO EN ASPNET USERS

                        IdentityUser user = new IdentityUser();

                        user.UserName = cajeroVM.CajeroUserVM.Email;
                        user.Email = cajeroVM.CajeroUserVM.Email;
                        user.EmailConfirmed = true;
                        _userManager.CreateAsync(user, "Admin1234.").GetAwaiter().GetResult();


                        //Aquí validamos si los roles existen sino se crean
                        if (!await _roleManager.RoleExistsAsync(CNT.Administrador))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Administrador));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Secretaria));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Cajero));
                        }




                        //Obtenemos el rol seleccionado
                        string rol = Request.Form["radUsuarioRole"].ToString();

                        //Validamos si el rol seleccionado es Admin y si lo es lo agregamos
                        if (rol == CNT.Administrador)
                        {
                            await _userManager.AddToRoleAsync(user, CNT.Administrador);

                        }
                        else
                        {
                            if (rol == CNT.Secretaria)
                            {
                                await _userManager.AddToRoleAsync(user, CNT.Secretaria);
                                cajeroVM.CajeroUserVM.EsCajero = false;
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, CNT.Cajero);
                                cajeroVM.CajeroUserVM.EsCajero = true;
                            }

                        } 
                        #endregion

                        cajeroVM.CajeroUserVM.Rol = rol;
                        cajeroVM.CajeroUserVM.Estado = true;  //valor 1 
                        cajeroVM.CajeroUserVM.DeudaPesosActual = 0;

                        //Logica para guardar en BD
                        _contenedorTrabajo.Cajero.Add(cajeroVM.CajeroUserVM);


                        #region Envio del mail

                        //Para envio de correo

                        var mensaje = new MimeMessage();
                        //mensaje.From.Add(new MailboxAddress("Test Envio mail", "info@gmail.com")); // Test Envio mail: Nombre con el que aparece ademas del correo
                        mensaje.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
                        mensaje.To.Add(new MailboxAddress("Test Enviado", cajeroVM.CajeroUserVM.Email));
                        mensaje.Subject = "Creación de cajero";
                        mensaje.Body = new TextPart("plain")
                        {
                            Text = "Bienvenido. Ha sido dado de alta como cajero en el Equipo Juampi.\r\n" +
                            "Su correo y credenciales son las siguientes. Se recomienda cambiar la contraseña desde su perfil luego de iniciar sesión. \r\n \r\n" +
                            "Correo: " + cajeroVM.CajeroUserVM.Email + "\r\n"+
                            "Contraseña: Admin1234."


                           
                        };

                        using (var cliente = new SmtpClient())
                        {
                            //cliente.Connect("smtp.gmail.com", 465);
                            //cliente.Authenticate("seba.acosta85", "agsahvnskuzxrlfu");
                            cliente.Connect(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), false);
                            cliente.Authenticate(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
                            cliente.Send(mensaje);
                            cliente.Disconnect(true);

                        }

                        #endregion


                        _contenedorTrabajo.Save();

                     
                        transaction.Commit();


                        if (cajeroVM.CajeroUserVM.EsCajero == false)
                        {
                            return RedirectToAction(nameof(IndexSecretarias));
                        }

                        else
                        {
                            return RedirectToAction(nameof(Index));
                        }

                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        if (ex.InnerException != null &&
                           ex.InnerException.Message != null)
                        {

                            if (ex.InnerException.Message.Contains("IX_Cajeros_Email"))
                            {
                                ModelState.AddModelError(string.Empty, "Ya existe un cajero con el correo ingresado.");
                            }

                            else if (ex.InnerException.Message.Contains("IX_Cajeros_DNI"))
                            {
                                ModelState.AddModelError(string.Empty, "Ya existe un cajero con el DNI ingresado.");
                            }

                            else
                            {
                                ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                        }

                        return View(cajeroVM);
                    }
                }
            }

            //para no perder los datos de la lista de plataformas
            //cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();

            return View(cajeroVM);
        }

        [Authorize(Roles = "Administrador,Secretaria")]
        [HttpGet]
        // GET: Admin/Cajeros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
                // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            };

            if (id != null)
            {
                cajeroViewModel.CajeroUserVM = _contenedorTrabajo.Cajero.Get(id.GetValueOrDefault());

                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var rol = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

                if (rol == CNT.Secretaria && cajeroViewModel.CajeroUserVM.EsCajero == false)
                {
                    return View("AccesoDenegado");// PARA QUE UNA SECRETARIA PUEDA ACCEDER A EDITARLA DESDE LA URL
                }

                if (cajeroViewModel.CajeroUserVM == null)
                {
                    return NotFound();

                }

                cajeroViewModel.CajeroUserVM.EsCajero = true;

                ViewBag.Titulo = "Editar Cajero";
                ViewBag.Encabezado = "Editar Cajero";

            }

            return View("Edit",cajeroViewModel);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        // GET: Admin/Cajeros/Edit/5
        public async Task<IActionResult> EditSecretaria(int? id)
        {
            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
                // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            };

            if (id != null)
            {
                cajeroViewModel.CajeroUserVM = _contenedorTrabajo.Cajero.Get(id.GetValueOrDefault());

                if (cajeroViewModel.CajeroUserVM == null)
                {
                    return NotFound();

                }



                cajeroViewModel.CajeroUserVM.EsCajero = false;

                ViewBag.Titulo = "Editar Secretaria";
                ViewBag.Encabezado = "Editar Secretaria";
            }

            return View("Edit",cajeroViewModel);
        }

        [Authorize(Roles = "Administrador,Secretaria")]
        // POST: Admin/Cajeros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CajeroViewModel cajeroVM)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {


                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivos = HttpContext.Request.Form.Files;

                        var cajeroDesdeBd = _contenedorTrabajo.Cajero.Get(cajeroVM.CajeroUserVM.Id);

                        if (archivos.Count() > 0)
                        {
                            //Nuevo imagen para el artículo
                            string nombreArchivo = Guid.NewGuid().ToString();
                            var subidas = Path.Combine(rutaPrincipal, @"imagenes\cajeros");
                            var extension = Path.GetExtension(archivos[0].FileName);
                            var nuevaExtension = Path.GetExtension(archivos[0].FileName);


                            if (cajeroDesdeBd.UrlImagen != null)
                            {

                                var rutaImagen = Path.Combine(rutaPrincipal, cajeroDesdeBd.UrlImagen.TrimStart('\\'));

                                if (System.IO.File.Exists(rutaImagen))
                                {
                                    System.IO.File.Delete(rutaImagen);
                                }
                            }


                            //Nuevamente subimos el archivo
                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivos[0].CopyTo(fileStreams);
                            }

                            cajeroVM.CajeroUserVM.UrlImagen = @"\imagenes\cajeros\" + nombreArchivo + extension;



                           
                        }
                        else
                        {
                            //Aquí sería cuando la imagen ya existe y se conserva
                            cajeroVM.CajeroUserVM.UrlImagen = cajeroDesdeBd.UrlImagen;
                        }

                        

                        var user = await _userManager.FindByNameAsync(cajeroDesdeBd.Email);

                        //EDITO EL USUARIO EN ASPNETUSER
                        if (user != null)
                        {
                            if (cajeroDesdeBd.Email != cajeroVM.CajeroUserVM.Email)
                            {
                           
                                var setEmailResult = await _userManager.SetEmailAsync(user, cajeroVM.CajeroUserVM.Email);
                                var setUserNameResult = await _userManager.SetUserNameAsync(user, cajeroVM.CajeroUserVM.Email);


                                if (setEmailResult.Succeeded && setUserNameResult.Succeeded)
                                {
                                    await _userManager.UpdateAsync(user);
                                }
                            }

                            //Obtenemos el rol seleccionado
                            string rol = Request.Form["radUsuarioRole"].ToString();

                            //EDITO EL ROL SI CORRESPONDE
                            if (cajeroDesdeBd.Rol != rol)
                            {
                                var roles = await _userManager.GetRolesAsync(user);
                                var result = await _userManager.RemoveFromRoleAsync(user, roles[0]);

                                //Validamos si el rol seleccionado es Admin y si lo es lo agregamos
                                if (rol == CNT.Administrador)
                                {
                                    await _userManager.AddToRoleAsync(user, CNT.Administrador);
                                }
                                else
                                {
                                    if (rol == CNT.Secretaria)
                                    {
                                        await _userManager.AddToRoleAsync(user, CNT.Secretaria);                                     
                                    }
                                    else
                                    {
                                        await _userManager.AddToRoleAsync(user, CNT.Cajero);                                        
                                    }
                                }
                                cajeroVM.CajeroUserVM.Rol = rol;
                            }                         


                        }


                        _contenedorTrabajo.Cajero.Update(cajeroVM.CajeroUserVM);
                        _contenedorTrabajo.Save();

                        transaction.Commit();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        if (ex.InnerException != null &&
                           ex.InnerException.Message != null)
                        {

                            if (ex.InnerException.Message.Contains("IX_Cajeros_Email"))
                            {
                                ModelState.AddModelError(string.Empty, "Ya existe un cajero con el correo ingresado.");
                            }

                            else if (ex.InnerException.Message.Contains("IX_Cajeros_DNI"))
                            {
                                ModelState.AddModelError(string.Empty, "Ya existe un cajero con el DNI ingresado.");
                            }

                            else
                            {
                                ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                            }


                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                        }


                        return View(cajeroVM);
                    }
                }


            }
            //para no perder los datos de la lista de plataformas
            //cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();

            return View(cajeroVM);
        }
       

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var emailUsuarioActual = usuarioActual.Subject.Name;
            var emails = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual && u.EsCajero == true);

            return Json(new { data = emails });
        }

        [HttpGet]
        public IActionResult GetAllSecretaria()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var emailUsuarioActual = usuarioActual.Subject.Name;
            var emails = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual && u.EsCajero == false);

            return Json(new { data = emails });
        }


        public async Task<IActionResult> BloquearDesbloquearCajero(int id)
        {
            var cajeroDesdeBd = _contenedorTrabajo.Cajero.Get(id);

            var objFromDb = _userManager.FindByNameAsync(cajeroDesdeBd.Email);                   
          

        
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error bloqueando el cajero." });
            }

            //en un solo metodo, bloqueo o desbloqueo el usuario segun corresponda

            if (objFromDb.Result.LockoutEnd == null || objFromDb.Result.LockoutEnd < DateTime.Now)
            {
                               

                cajeroDesdeBd.Estado = false;
                
               await _userManager.SetLockoutEndDateAsync(objFromDb.Result, DateTime.Now.AddYears(1000));
               await _userManager.UpdateAsync(objFromDb.Result);
                _contenedorTrabajo.Save();
               
                return Json(new { success = true, message = "Cajero Bloqueado Correctamente" });
            }

            else
            {

                // _contenedorTrabajo.Cajero.DesloquearCajero(cajeroDesdeBd,objFromDb);

                //cajeroDesdeBd.Estado = false;

                await _userManager.SetLockoutEndDateAsync(objFromDb.Result, null);
                await _userManager.UpdateAsync(objFromDb.Result);
                cajeroDesdeBd.Estado = true;

                _contenedorTrabajo.Save();
                return Json(new { success = true, message = "Cajero DesBloqueado Correctamente" });
            }
            // return View(user);


        }

        #endregion
    }
}
