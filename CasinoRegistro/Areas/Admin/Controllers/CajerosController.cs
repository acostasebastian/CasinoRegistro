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

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria")]
    [Area("Admin")]
    public class CajerosController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment; // para acceder a las carpetas del proyecto, para guardar/editar la imagen      
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CasinoRegistroDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;


        public CajerosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment,
            UserManager<IdentityUser> userManager, CasinoRegistroDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
        }

        // GET: Admin/Cajeros
        public async Task<IActionResult> Index()
        {

            //Opción 1: Obtener todos los usuario
            //return View(_contenedorTrabajo.Cajero.GetAll());

            //Opción 2: Obtener todos los usuarios menos el que esté logueado, para no bloquearse el mismo


           

            return View();
            // var user = await _userManager.FindByNameAsync(claimsIdentity.Name);

            //// return View(_contenedorTrabajo.Cajero.GetAll());

            //return View(_contenedorTrabajo.Cajero.GetAll(u => u.Id != usuarioActual.Value));
        }


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
            }

            return View(cajeroViewModel);

        }

        // GET: Admin/Cajeros/Create
        [HttpGet]
        public IActionResult Create()
        {
            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
                // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            };
            return View(cajeroViewModel);
        }

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

                        ////Logica para guardar en BD
                        //_contenedorTrabajo.Cajero.Add(cajeroVM.CajeroUserVM);


                        //_contenedorTrabajo.Save();


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
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, CNT.Cajero);
                            }

                        }

                        cajeroVM.CajeroUserVM.Rol = rol;
                        cajeroVM.CajeroUserVM.Estado = true;  //valor 1 
                        cajeroVM.CajeroUserVM.DeudaPesosActual = 0;

                        //Logica para guardar en BD
                        _contenedorTrabajo.Cajero.Add(cajeroVM.CajeroUserVM);


                        _contenedorTrabajo.Save();

                        transaction.Commit();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        if (ex.InnerException != null &&
                           ex.InnerException != null)
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

                        return View(cajeroVM);
                    }
                }
            }

            //para no perder los datos de la lista de plataformas
            //cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();

            return View(cajeroVM);
        }

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

                if (cajeroViewModel.CajeroUserVM == null)
                {
                    return NotFound();

                }

            }

            return View(cajeroViewModel);
        }

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
                                        //TODO: CORREGIR CUANDO PUEDA HACER QUE EL EDIT MUESTRE LO ELEGIDO TAMBIEN
                                        cajeroVM.CajeroUserVM.Rol = "Cajero";
                                    }
                                }
                                cajeroVM.CajeroUserVM.Rol = rol;
                            }
                            else
                            {
                                //TODO: CORREGIR CUANDO PUEDA HACER QUE EL EDIT MUESTRE LO ELEGIDO
                                await _userManager.AddToRoleAsync(user, CNT.Cajero);
                                cajeroVM.CajeroUserVM.Rol = "Cajero";
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
                           ex.InnerException != null)
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
                     

                        return View(cajeroVM);
                    }
                }


            }
            //para no perder los datos de la lista de plataformas
            //cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();

            return View(cajeroVM);
        }

        //// GET: Admin/Cajeros/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var cajeroUser = await _context.Cajero
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (cajeroUser == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(cajeroUser);
        //}

        //// POST: Admin/Cajeros/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var cajeroUser = await _context.Cajero.FindAsync(id);
        //    if (cajeroUser != null)
        //    {
        //        _context.Cajero.Remove(cajeroUser);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CajeroUserExists(int id)
        //{
        //    return _context.Cajero.Any(e => e.Id == id);
        //}

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var emailUsuarioActual = usuarioActual.Subject.Name;
            var emails = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual);

            return Json(new { data = emails });
        }
     

        public async Task<IActionResult> BloquearDesloquearCajero(int id)
        {
            var cajeroDesdeBd = _contenedorTrabajo.Cajero.Get(id);

            var objFromDb = _userManager.FindByNameAsync(cajeroDesdeBd.Email);

         
          

            //var objFromDb = _contenedorTrabajo.Cajero.GetString(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error bloqueando el cajero." });
            }

            //en un solo metodo, bloqueo o desbloqueo el usuario segun corresponda

            if (objFromDb.Result.LockoutEnd == null || objFromDb.Result.LockoutEnd < DateTime.Now)
            {

                //_contenedorTrabajo.Cajero.BloquearCajero(cajeroDesdeBd,objFromDb);

               // objFromDb.Result.LockoutEnd = DateTime.Now.AddYears(1000);

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
