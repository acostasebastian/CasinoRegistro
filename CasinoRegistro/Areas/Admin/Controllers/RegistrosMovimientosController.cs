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
using CasinoRegistro.DataAccess.Data.Repository;

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador,Secretaria")]
    public class RegistrosMovimientosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly CasinoRegistroDbContext _db;

        public RegistrosMovimientosController(IContenedorTrabajo contenedorTrabajo, CasinoRegistroDbContext db)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _db = db;
        }

        // GET: Admin/RegistroMovimientoes
        public async Task<IActionResult> Index()
        {

            return View();
        }



        // GET: Admin/RegistroMovimientoes/Create
        public IActionResult CreateFichas()
        {
            //ViewData["CajeroId"] = new SelectList(_contenedorTrabajo.RegistroMovimiento.GetListaCajeros(), "Id", "Apellido");
            //  return View();



            RegistroMovimientoViewModel registroVM = new RegistroMovimientoViewModel()
            {
                RegistroMovimientoVM = new CasinoRegistro.Models.RegistroMovimiento
                {
                    EsIngresoFichas = true
                },
                Fecha = DateTime.Now,
               
                ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros()

            };
            return View("Create", registroVM);

        }

        public IActionResult CreateDinero()
        {
            //ViewData["CajeroId"] = new SelectList(_contenedorTrabajo.RegistroMovimiento.GetListaCajeros(), "Id", "Apellido");
            //  return View();

            //CajeroViewModel cajeroViewModel = new CajeroViewModel()
            //{
            //    CajeroUserVM = new CasinoRegistro.Models.CajeroUser(),
            //    // ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas()
            //};
            //return View(cajeroViewModel);

            RegistroMovimientoViewModel registroVM = new RegistroMovimientoViewModel()
            {
                RegistroMovimientoVM = new CasinoRegistro.Models.RegistroMovimiento
                {
                    EsIngresoFichas = false
                },
                Fecha = DateTime.Now,
               

                ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros()

            };

           
            return View("Create", registroVM);


        }

        public ActionResult CalcularDeuda(int idCajero, double pesosEntregados, double pesosDevueltos, double comision, double deudaPesosActual)
        // public double CalcularDeuda(int idCajero, double pesosEntregados, double pesosDevueltos, double comision, double deudaPesosActual)
        //public ActionResult CalcularDeuda(double pesosEntrega)
        {
            // CajeroUser deuda = new CajeroUser();
            decimal deuda = (decimal)_contenedorTrabajo.Cajero.Get(idCajero).DeudaPesosActual;


            double calculo = 0;

            calculo = ((double)deuda + pesosEntregados) - pesosDevueltos;


            return View("Create");

        }

        public decimal RealizarCalculos(RegistroMovimientoViewModel registroMovimientoVM)
        {
            decimal nuevaDeuda = 0;

            //Busco en la base de datos, la deuda actaul del cajero
            decimal deuda = (decimal)_contenedorTrabajo.Cajero.Get(registroMovimientoVM.RegistroMovimientoVM.CajeroId).DeudaPesosActual;

            //calculo la nueva deuda
            nuevaDeuda = (deuda + (decimal)registroMovimientoVM.RegistroMovimientoVM.PesosEntregados) - (decimal)registroMovimientoVM.RegistroMovimientoVM.PesosDevueltos;

            return nuevaDeuda;
        }

        // POST: Admin/RegistroMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,CajeroId,FechaCreacion,FichasCargadas,PesosEntregados,PesosDevueltos,Comision,DeudaPesosActual")] RegistroMovimiento registroMovimiento)
        public async Task<IActionResult> Create(RegistroMovimientoViewModel registroMovimientoVM)
        {

            if (ModelState.IsValid)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    //if (registroMovimientoVM.EsIngresoFichas == false)
                    //{

                    //    try
                    //    {
                    //        var deudaNueva = RealizarCalculos(registroMovimientoVM);

                    //        _contenedorTrabajo.Cajero.UpdateDeuda(registroMovimientoVM.CajeroId, deudaNueva);
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error en el calculo: " + ex.Message);
                    //    }
                    //}


                    //registroMovimientoVM.FechaCreacion = registroMovimientoVM.Fecha;


                    try
                    {
                        if (registroMovimientoVM.RegistroMovimientoVM.EsIngresoFichas == false)
                        {
                            var deudaNueva = RealizarCalculos(registroMovimientoVM);

                        _contenedorTrabajo.Cajero.UpdateDeuda(registroMovimientoVM.RegistroMovimientoVM.CajeroId, deudaNueva);
                        }

                        registroMovimientoVM.RegistroMovimientoVM.FechaCreacion = registroMovimientoVM.Fecha;

                        _contenedorTrabajo.RegistroMovimiento.Add(registroMovimientoVM.RegistroMovimientoVM);
                        transaction.Commit();
                        _contenedorTrabajo.Save();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                        registroMovimientoVM.ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros();
                    }
                }

            }
            registroMovimientoVM.ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros();
            return View(registroMovimientoVM);

        }

        //// GET: Admin/RegistroMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            RegistroMovimientoViewModel registroViewModel = new RegistroMovimientoViewModel()
            {
                RegistroMovimientoVM = new CasinoRegistro.Models.RegistroMovimiento(),
                ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros(),
               
            };

            if (id != null)
            {
                registroViewModel.RegistroMovimientoVM = _contenedorTrabajo.RegistroMovimiento.Get(id.GetValueOrDefault());

                registroViewModel.Fecha = registroViewModel.RegistroMovimientoVM.FechaCreacion;
            }

            return View(registroViewModel);
        }

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


        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.RegistroMovimiento.GetAll(includeProperties: "CajeroUser") });
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
