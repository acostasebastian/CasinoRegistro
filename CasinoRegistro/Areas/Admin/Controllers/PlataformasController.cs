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

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlataformasController : Controller
    {      
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PlataformasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        // GET: Admin/Plataformas
        public async Task<IActionResult> Index()       
        {
            return View();        
        }

        [HttpGet]
        // GET: Admin/Plataformas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Plataformas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                //Logica para guardar en BD
                _contenedorTrabajo.Plataforma.Add(plataforma);

                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                       ex.InnerException != null &&
                       ex.InnerException.Message.Contains("IX_Plataformas_URL"))
                    {
                        ModelState.AddModelError(string.Empty, "Esta Plataforma ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    }
                }
            }
            return View(plataforma);
        }

        //// GET: Admin/Plataformas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {          

            Plataforma plataforma = new Plataforma();            
            plataforma = _contenedorTrabajo.Plataforma.Get(id);

            
            if (plataforma == null)
            {
                return NotFound();
            }
            return View(plataforma);
        }

        //// POST: Admin/Plataformas/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                //Logica para actualizar en BD
                _contenedorTrabajo.Plataforma.Update(plataforma);

                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                      ex.InnerException != null &&
                      ex.InnerException.Message.Contains("IX_Plataformas_URL"))
                    {
                        ModelState.AddModelError(string.Empty, "Esta Plataforma ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    }
                }


            }
            return View(plataforma);
        }
      

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Plataforma.GetAll() });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Plataforma.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error borrando la plataforma" });
            }

            _contenedorTrabajo.Plataforma.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Plataforma Borrada Correctamente" });
        }

        #endregion
    }
}
