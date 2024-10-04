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
using Microsoft.AspNetCore.Authorization;

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador,Secretaria")]
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
                       ex.InnerException.Message != null)                                       
                    {

                        if (ex.InnerException.Message.Contains("IX_Plataformas_URL"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Plataforma ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty,ex.InnerException.Message);
                        }             
                     
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
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_Plataformas_URL"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Plataforma ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                        }

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

        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColum = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        [HttpPost]
        public IActionResult GetAll()
        {

            //logistica datatable           
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColum = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;

            IEnumerable<Plataforma>? listaPlataformas;


            if (searchValue != "")
            {
                listaPlataformas = _contenedorTrabajo.Plataforma.GetAll(p => p.URL.Contains(searchValue) || p.Descripcion.Contains(searchValue));

            }
            else
            {
                listaPlataformas = _contenedorTrabajo.Plataforma.GetAll();
            };


            recordsTotal = listaPlataformas.Count();

            listaPlataformas = listaPlataformas.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaPlataformas });


            // return Json(new { data = _contenedorTrabajo.Plataforma.GetAll() });
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
