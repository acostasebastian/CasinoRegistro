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

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CajerosController : Controller
    {
        //private readonly CasinoRegistroDbContext _context;

        //public CajerosController(CasinoRegistroDbContext context)
        //{
        //    _context = context;
        //}

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment; // para acceder a las carpetas del proyecto, para guardar/editar la imagen

        public CajerosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/Cajeros
        public async Task<IActionResult> Index()
        {
           // return View(await _context.Cajero.ToListAsync());
            return View(await _contenedorTrabajo.Cajero.GetAll());
        }



        // GET: Admin/Cajeros/Details/5
        //public async Task<IActionResult> Details(int? id)
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

        // GET: Admin/Cajeros/Create
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
        //public async Task<IActionResult> Create(CajeroViewModel cajeroVM, IFormFile imagen)
        public IActionResult Create(CajeroViewModel cajeroVM, IFormFile imagen)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(cajeroUser);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(cajeroUser);

            cajeroVM.CajeroUserVM.UrlImagen = imagen.FileName;
            _contenedorTrabajo.Save();

            if (ModelState.IsValid)
            {

                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivo = HttpContext.Request.Form.Files;

                if (cajeroVM.CajeroUserVM.Id == 0 && archivo.Count() > 0)
                {
                    //Nuevo Cajero
                    string nombreArchivo = Guid.NewGuid().ToString();   //como es nuevo el cajero, le pongo un Guid como nombre
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\cajeros"); //accederá a la carpeta en wwwroot
                    var extension = Path.GetExtension(archivo[0].FileName);

                    using ( var fileStreams = new FileStream(Path.Combine(subidas,nombreArchivo + extension),FileMode.Create))
                    {
                        archivo[0].CopyTo(fileStreams);
                    }

                    //guardo la ruta en la base de datos
                    cajeroVM.CajeroUserVM.UrlImagen = @"imagenes\cajeros" + nombreArchivo +extension;

                }


                //Logica para guardar en BD
                _contenedorTrabajo.Cajero.Add(cajeroVM.CajeroUserVM);

                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

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
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                        }
                    }
                   
                }
            }

            //para no perder los datos de la lista de plataformas
           //cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();

            return View(cajeroVM);
        }

        // GET: Admin/Cajeros/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            CajeroUser cajero = new CajeroUser();
            cajero = await _contenedorTrabajo.Cajero.Get(id);
            if (cajero == null)
            {
                return NotFound();
            }

            return View(cajero);
        }

        // POST: Admin/Cajeros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CajeroUser cajero)
        {
            if (ModelState.IsValid)
            {
                //Logica para actualizar en BD
                _contenedorTrabajo.Cajero.Update(cajero);

                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

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
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                        }
                    }
                }


            }

            return View(cajero);
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
    }
}
