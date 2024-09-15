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
        //private readonly CasinoRegistroDbContext _context;

        //public PlataformasController(CasinoRegistroDbContext context)
        //{
        //    _context = context;
        //}

        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PlataformasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // GET: Admin/Plataformas
        public async Task<IActionResult> Index()
          //   public IActionResult Index()
        {
           // return View();
            //return View(await _context.Plataforma.ToListAsync());
           // return View( _contenedorTrabajo.Plataforma.GetAll());
            return View(await _contenedorTrabajo.Plataforma.GetAll());
        }

        // GET: Admin/Plataformas/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var plataforma = await _contenedorTrabajo.Plataforma
        //        .GetFirstOrDefault(m => m.Id == id);
        //    if (plataforma == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(plataforma);
        //}

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
            // plataforma = _contenedorTrabajo.Plataforma.Get(id);
            plataforma = await _contenedorTrabajo.Plataforma.Get(id);

            //var plataforma = await _contenedorTrabajo.Plataforma.FindAsync(id);
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

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(plataforma);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!PlataformaExists(plataforma.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
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

        //// GET: Admin/Plataformas/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var plataforma = await _context.Plataforma
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (plataforma == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(plataforma);
        //}

        //// POST: Admin/Plataformas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var plataforma = await _context.Plataforma.FindAsync(id);
        //    if (plataforma != null)
        //    {
        //        _context.Plataforma.Remove(plataforma);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PlataformaExists(int id)
        //{
        //    return _context.Plataforma.Any(e => e.Id == id);
        //}
    }
}
