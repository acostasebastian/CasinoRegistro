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
using CasinoRegistro.Utilities;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using System.Security.Claims;

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador,Secretaria")]
    public class PlataformasController : Controller
    {
        #region variables string
    
        //datatable - paginacion, ordenamiento y busquda

        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColum = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        //logger
        string informacion = "";

        ClaimsIdentity claimsIdentity;
        Claim usuarioActual;

        string emailUsuarioActual ="";


        #endregion


        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ILogger<Plataforma> _logger;

        public PlataformasController(IContenedorTrabajo contenedorTrabajo, ILogger<Plataforma> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _logger = logger;
            
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
                claimsIdentity = (ClaimsIdentity)this.User.Identity;
                usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                emailUsuarioActual = usuarioActual.Subject.Name;
    
                _logger.LogInformation("CREACIÓN DE PLATAFORMA \r\n Usuario registrado para el guardado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                //Logica para guardar en BD
                _contenedorTrabajo.Plataforma.Add(plataforma);

                try
                {                
                    _contenedorTrabajo.Save();               


                    informacion = "URL: " + plataforma.URL;
                    _logger.LogInformation("CREACIÓN DE PLATAFORMA \r\n Plataforma guardada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

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
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("CREACIÓN DE PLATAFORMA \r\n Error al querer guardar en Plataforma - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);
                        informacion = ex.Message;
                        _logger.LogWarning("CREACIÓN DE PLATAFORMA \r\n Error al querer guardar en Plataforma {Time} - {@informacion}", DateTime.Now, informacion);
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
                claimsIdentity = (ClaimsIdentity)this.User.Identity;
                usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                emailUsuarioActual = usuarioActual.Subject.Name;
                               
                
                _logger.LogInformation("EDICIÓN DE PLATAFORMA \r\n Usuario registrado para la edición: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                //Logica para actualizar en BD
                _contenedorTrabajo.Plataforma.Update(plataforma);

                try
                {
                    _contenedorTrabajo.Save();

                    informacion = "URL: "+ plataforma.URL + " - Id: " + plataforma.Id;
                    _logger.LogInformation("EDICIÓN DE PLATAFORMA \r\n Plataforma editada correctamente {Time} - {@informacion}", DateTime.Now, informacion);
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
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);                           
                            
                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("EDICIÓN DE PLATAFORMA \r\n Error al querer editar la plataforma - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                        informacion = ex.Message;
                        _logger.LogWarning("EDICIÓN DE PLATAFORMA \r\n Error al querer editar la plataforma {Time} - {@informacion}", DateTime.Now, informacion);
                    }
                }


            }
            return View(plataforma);
        }


        #region Llamadas a la API

        [HttpPost]
        public IActionResult GetAll()
        {

            //logistica datatable           
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColum = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault(); //column por la que esta ordenado
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault(); //asc/desc
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;

            IEnumerable<Plataforma>? listaPlataformas;


            if (searchValue != "")
            {
                listaPlataformas = _contenedorTrabajo.Plataforma.GetAll(p => p.Id.ToString().Contains(searchValue) || p.URL.Contains(searchValue) || p.Descripcion.Contains(searchValue));

            }
            else
            {
                listaPlataformas = _contenedorTrabajo.Plataforma.GetAll();
            };

            // este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Plataforma.GetLambda<Plataforma>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaPlataformas = listaPlataformas.OrderByDescending(getNombreColumnaLambda);
            }
            else
            {
                listaPlataformas = listaPlataformas.OrderBy(getNombreColumnaLambda);
            }


            recordsTotal = listaPlataformas.Count();

            listaPlataformas = listaPlataformas.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaPlataformas });


            // return Json(new { data = _contenedorTrabajo.Plataforma.GetAll() });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Plataforma.Get(id);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;

                       
            _logger.LogInformation("BORRADO DE PLATAFORMA \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (objFromDb == null)
            {
                informacion = "Plataforma no encontrada";
                _logger.LogWarning("BORRADO DE PLATAFORMA \r\n Error al querer borrar la plataforma {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = false, message = "Error borrando la plataforma" });
            }

            _contenedorTrabajo.Plataforma.Remove(objFromDb);
            _contenedorTrabajo.Save();

            informacion = "URL: " + objFromDb.URL + " - Id: " + objFromDb.Id;
            _logger.LogInformation("BORRADO DE PLATAFORMA \r\n Plataforma borrada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

            return Json(new { success = true, message = "Plataforma Borrada Correctamente" });
        }  

        #endregion
    }
}
