using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CasinoRegistro.DataAccess.Data;
using CasinoRegistro.Models;
using Microsoft.AspNetCore.Authorization;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using CasinoRegistro.Models.ViewModels;

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador,Secretaria")]
    public class RegistrosMovimientosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public RegistrosMovimientosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // GET: Admin/RegistroMovimientoes
        public async Task<IActionResult> Index()
        {
            //var casinoRegistroDbContext = _contenedorTrabajo.RegistroMovimiento.Include(r => r.CajeroUser);

            //return View(await casinoRegistroDbContext.ToListAsync());
            //return View(_contenedorTrabajo.Cajero.GetAll());
            return View();
        }

        // GET: Admin/RegistroMovimientoes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var registroMovimiento = await _context.RegistroMovimiento
        //        .Include(r => r.CajeroUser)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (registroMovimiento == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(registroMovimiento);
        //}

        // GET: Admin/RegistroMovimientoes/Create
        public IActionResult Create()
        {
            //ViewData["CajeroId"] = new SelectList(_contenedorTrabajo.RegistroMovimiento.GetListaCajeros(), "Id", "Apellido");
          //  return View();
            RegistroMovimientoViewModel registroVM = new RegistroMovimientoViewModel()
            {
                //RegistroMovimiento = new Models.RegistroMovimiento
                //{
                //    FechaCreacion = DateTime.Now,
                //},
                //RegistroMovimiento = new Models.RegistroMovimiento(),

                Fecha = DateTime.Now,

                ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros()

            };
            return View(registroVM);

        }

        // POST: Admin/RegistroMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,CajeroId,FechaCreacion,FichasCargadas,PesosEntregados,PesosDevueltos,Comision,DeudaPesosActual")] RegistroMovimiento registroMovimiento)
          public async Task<IActionResult> Create(RegistroMovimientoViewModel registroMovimientoVM)
        {

            //if (ModelState.IsValid)
            //{
            //    _contenedorTrabajo.RegistroMovimiento.Add(registroMovimiento);
            //    _contenedorTrabajo.Save();
            //    return RedirectToAction(nameof(Index));
            //}
            ////ViewData["CajeroId"] = new SelectList(_context.Cajero, "Id", "Apellido", registroMovimiento.CajeroId);
            //ViewData["CajeroId"] = new SelectList(_contenedorTrabajo.RegistroMovimiento.GetListaCajeros(), "Id", "Apellido", registroMovimiento.CajeroId);
            //return View(registroMovimiento);

            ///-------------------
            //ORIGINAL
            //if (ModelState.IsValid)
            //{
            //    _context.Add(registroMovimiento);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["CajeroId"] = new SelectList(_context.Cajero, "Id", "Apellido", registroMovimiento.CajeroId);
            //return View(registroMovimiento);



            //--------------

            if (ModelState.IsValid)
            {
                // registroMovimientoVM.FechaCreacion = registroMovimientoVM.FechaCreacion;
                //var fecha = ModelState.Keys["FechaCreacion"].raw[1] ; // AHI APARECE EL VALOR, PERO NO PUEDO ACCEDER A EL

                registroMovimientoVM.FechaCreacion = registroMovimientoVM.Fecha;

                _contenedorTrabajo.RegistroMovimiento.Add(registroMovimientoVM);
                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    registroMovimientoVM.ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros();
                }


            }
            registroMovimientoVM.ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros();
            return View(registroMovimientoVM);

        }

        //// GET: Admin/RegistroMovimientoes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var registroMovimiento = await _context.RegistroMovimiento.FindAsync(id);
        //    if (registroMovimiento == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CajeroId"] = new SelectList(_context.Cajero, "Id", "Apellido", registroMovimiento.CajeroId);
        //    return View(registroMovimiento);
        //}

        //// POST: Admin/RegistroMovimientoes/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,CajeroId,FechaCreacion,FichasCargadas,PesosEntregados,PesosDevueltos,Comision,DeudaPesosActual")] RegistroMovimiento registroMovimiento)
        //{
        //    if (id != registroMovimiento.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(registroMovimiento);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RegistroMovimientoExists(registroMovimiento.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CajeroId"] = new SelectList(_context.Cajero, "Id", "Apellido", registroMovimiento.CajeroId);
        //    return View(registroMovimiento);
        //}
              


        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.RegistroMovimiento.GetAll(includeProperties:"CajeroUser") });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.RegistroMovimiento.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error borrando la plataforma" });
            }

            _contenedorTrabajo.RegistroMovimiento.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Plataforma Borrada Correctamente" });
        }

        #endregion
    }
}
