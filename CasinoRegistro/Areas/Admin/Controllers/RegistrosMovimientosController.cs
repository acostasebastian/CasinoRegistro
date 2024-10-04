﻿using System;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using CasinoRegistro.Utilities;
using System.Security.Claims;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Drawing.Printing;
//using System.Net.Mail;

namespace CasinoRegistro.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Administrador,Secretaria")]
    public class RegistrosMovimientosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly CasinoRegistroDbContext _db;
        // private readonly CultureInfo _cultura;
        private readonly IConfiguration _config;

        public RegistrosMovimientosController(IContenedorTrabajo contenedorTrabajo, CasinoRegistroDbContext db
            //, CultureInfo cultura
            , IConfiguration config
            )
        {
            _contenedorTrabajo = contenedorTrabajo;
            _db = db;
            _config = config;
        }

        [Authorize(Roles = "Administrador,Secretaria")]
        // GET: Admin/RegistroMovimientoes
        public async Task<IActionResult> Index()
        {

            return View();
        }

        [Authorize(Roles = "Administrador,Secretaria,Cajero")]
        // GET: Admin/RegistroMovimientoes
        public async Task<IActionResult> MovimientosPorCajero()
        {

            return View();
        }


        [Authorize(Roles = "Administrador,Secretaria")]
        // GET: Admin/RegistroMovimientoes/Create
        public IActionResult CreateFichas()
        { 
            RegistroMovimientoViewModel registroVM = new RegistroMovimientoViewModel()
            {                
                EsIngresoFichas =true,
                Fecha = DateTime.Now,
               
                ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros()

            };
            var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            ViewBag.CurrentDateTime = currentDateTime;

            ViewBag.Encabezado = "Crear un nuevo Registro de Fichas";




            return View("Create", registroVM);

        }

        [Authorize(Roles = "Administrador,Secretaria")]
        public IActionResult CreateDinero()
        {
            RegistroMovimientoViewModel registroVM = new RegistroMovimientoViewModel()
            {               
                EsIngresoFichas = false,
                Fecha = DateTime.Now,
               

                ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros()

            };
            var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            ViewBag.CurrentDateTime = currentDateTime;

            ViewBag.Encabezado = "Crear un nuevo Registro de Dinero";

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



        // POST: Admin/RegistroMovimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador,Secretaria")]
        [HttpPost]
        [ValidateAntiForgeryToken]     
        public async Task<IActionResult> Create(RegistroMovimientoViewModel registroMovimientoVM)
        {
            var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            if (ModelState.IsValid)
            {              

                if (validarDatos(registroMovimientoVM) == false)
                {                 

                    using (var transaction = _db.Database.BeginTransaction())
                    {

                        try
                        {
                            if (registroMovimientoVM.EsIngresoFichas == false)
                            {
                                var deudaNueva = RealizarCalculos(registroMovimientoVM, Metodo.Creacion);

                                _contenedorTrabajo.Cajero.UpdateDeuda(registroMovimientoVM.CajeroId, deudaNueva);
                            }

                            registroMovimientoVM.FechaCreacion = registroMovimientoVM.Fecha;

                            _contenedorTrabajo.RegistroMovimiento.Add(registroMovimientoVM);

                            #region envio del correo


                            string textoCorreo = "Se ha registrado un nuevo movimiento en su cuenta de cajero en el Equipo Juampi.\r\n\r\n";

                            if (registroMovimientoVM.EsIngresoFichas == true)
                            {
                                textoCorreo = textoCorreo + "Fichas Ingresadas: " + registroMovimientoVM.FichasCargadas;
                            }
                            else
                            {
                                textoCorreo = textoCorreo 
                                    + "Pesos entregados a usted: " + registroMovimientoVM.PesosEntregados+"\r\n"
                                    + "Pesos devueltos por usted: " + registroMovimientoVM.PesosDevueltos + "\r\n"
                                    + "Comisión para usted: " + registroMovimientoVM.Comision + "\r\n"
                                    + "Nueva deuda: " + registroMovimientoVM.CajeroUser.DeudaPesosActual + "\r\n"
                                    ;
                            }

                            var mensaje = new MimeMessage();
                            // mensaje.From.Add(new MailboxAddress("Test Envio mail", "seba.acosta85@gmail.com")); // Test Envio mail: Nombre con el que aparece ademas del correo y cuenta desde la que aparece
                            mensaje.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
                            mensaje.To.Add(new MailboxAddress("Test Enviado", registroMovimientoVM.CajeroUser.Email));
                            mensaje.Subject = "Nuevo movimiento";
                            mensaje.Body = new TextPart("plain")
                            {
                                Text = textoCorreo
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

                            transaction.Commit();
                            _contenedorTrabajo.Save();
                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                            registroMovimientoVM.ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros();

                            currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            ViewBag.CurrentDateTime = currentDateTime;
                        }
                    }
                }

               

            }

            currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            ViewBag.CurrentDateTime = currentDateTime;
            registroMovimientoVM.ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros();
            return View(registroMovimientoVM);

        }

       
        // GET: Admin/RegistroMovimientoes/Edit/5
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<IActionResult> Edit(int? id)
        {
            RegistroMovimientoViewModel registroViewModel = new RegistroMovimientoViewModel();
            //{
            //    ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros(),
            //};

            if (id != null)
            {
                RegistroMovimiento registroMovimiento = _contenedorTrabajo.RegistroMovimiento.Get(id.GetValueOrDefault());

                if (registroMovimiento == null)
                {
                   return NotFound();
                   
                }

                registroViewModel = ToViewModel(registroMovimiento);
                registroViewModel.ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros();  
                 
            }

            ViewBag.CurrentDateTime = registroViewModel.Fecha.ToString("yyyy-MM-dd HH:mm"); 

            if (registroViewModel.EsIngresoFichas)
            {
                ViewBag.Encabezado = "Editar Registro de Fichas";
            }

            else
            {
                ViewBag.Encabezado = "Editar Registro de Dinero";
            }
     

            return View(registroViewModel);
        }



        // POST: Admin/RegistroMovimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador,Secretaria")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegistroMovimientoViewModel registroMovimientoVM)
        {
            //if (id != registroMovimiento.Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        RegistroMovimiento registroMovimiento = ToRegistro(registroMovimientoVM);


                        if (registroMovimientoVM.EsIngresoFichas == false)
                        {
                            var deudaNueva = RealizarCalculos(registroMovimiento, Metodo.Edicion);

                            _contenedorTrabajo.Cajero.UpdateDeuda(registroMovimiento.CajeroId, deudaNueva);
                        }


                        _contenedorTrabajo.RegistroMovimiento.Update(registroMovimiento);
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
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                        }

                            
                    }
                    registroMovimientoVM.ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros();

                    ViewBag.CurrentDateTime = registroMovimientoVM.Fecha.ToString("yyyy-MM-dd HH:mm");
                    return View(registroMovimientoVM);
                }


               
               
            }
            registroMovimientoVM.ListaCajeros = _contenedorTrabajo.Cajero.GetListaCajeros();
            return View(registroMovimientoVM);
        }


        [Authorize(Roles = "Administrador,Secretaria,Cajero")]
        // GET: Admin/RegistroMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            RegistroMovimientoViewModel registroViewModel = new RegistroMovimientoViewModel();

            RegistroMovimiento registroMovimiento = _contenedorTrabajo.RegistroMovimiento.GetFirstOrDefault(c => c.Id == id.GetValueOrDefault(), includeProperties: "CajeroUser");

            if (registroMovimiento == null)
            {
                return NotFound();

            }

            registroViewModel = ToViewModel(registroMovimiento);
         
            ViewBag.CurrentDateTime = registroViewModel.Fecha.ToString("yyyy-MM-dd HH:mm");

            return View(registroViewModel);
        }


        #region Metodos Propios

        public decimal RealizarCalculos(RegistroMovimiento registroMovimiento, Metodo metodo)
        {
            decimal nuevaDeuda = 0;
            decimal deuda = 0;

            //Busco en la base de datos, la deuda actaul del cajero
            deuda = (decimal)_contenedorTrabajo.Cajero.Get(registroMovimiento.CajeroId).DeudaPesosActual;

            if (metodo == Metodo.Creacion)
            {

                //calculo la nueva deuda
                nuevaDeuda = (deuda + (decimal)registroMovimiento.PesosEntregados) - (decimal)registroMovimiento.PesosDevueltos;
            }

            else if (metodo == Metodo.Edicion)
            {
                //busco en el registro los valores ingresados al crearlo
                decimal pesosEntregadosOld = (decimal)_contenedorTrabajo.RegistroMovimiento.Get(registroMovimiento.Id).PesosEntregados;
                decimal pesosDevueltosOld = (decimal)_contenedorTrabajo.RegistroMovimiento.Get(registroMovimiento.Id).PesosDevueltos;

                //a la deuda existente, resto los valores antes ingresados para volver a hacer el calculo
                deuda = (deuda + pesosDevueltosOld) - pesosEntregadosOld;

                //calculo la nueva deuda
                nuevaDeuda = (deuda + (decimal)registroMovimiento.PesosEntregados) - (decimal)registroMovimiento.PesosDevueltos;
            }

            else if (metodo == Metodo.Eliminicacion)
            {
                //calculo la nueva deuda eliminando los registros
                nuevaDeuda = (deuda + (decimal)registroMovimiento.PesosDevueltos - (decimal)registroMovimiento.PesosEntregados);
            }




                return nuevaDeuda;
        }

        private RegistroMovimientoViewModel ToViewModel(RegistroMovimiento registroMovimiento)
        {      
            return new RegistroMovimientoViewModel
            {



                CajeroId = registroMovimiento.CajeroId,
                CajeroUser = registroMovimiento.CajeroUser,//_contenedorTrabajo.Cajero.Get(registroMovimiento.CajeroId),                
                FechaCreacion = registroMovimiento.FechaCreacion,
                FichasCargadas = registroMovimiento.FichasCargadas,
                //PesosEntregados = registroMovimiento.PesosEntregados,
                //PesosDevueltos = registroMovimiento.PesosDevueltos,
                //Comision = registroMovimiento.Comision,
                
                PesosDevueltos = registroMovimiento.PesosDevueltos,
                Comision = registroMovimiento.Comision,
                EsIngresoFichas = registroMovimiento.EsIngresoFichas,
                Fecha = registroMovimiento.FechaCreacion,
                CajeroVMId = registroMovimiento.CajeroId,
                sPesosEntregados = registroMovimiento.PesosEntregados.ToString(),
                sPesosDevueltos = registroMovimiento.PesosDevueltos.ToString(),
                sComision = registroMovimiento.Comision.ToString(),
            };

        }

        private RegistroMovimiento ToRegistro(RegistroMovimientoViewModel registroMovimientoVM)
        {
            return new RegistroMovimiento
            {
                Id = registroMovimientoVM.Id,
                CajeroId = registroMovimientoVM.CajeroVMId,
                CajeroUser = _contenedorTrabajo.Cajero.Get(registroMovimientoVM.CajeroVMId),
                FechaCreacion = registroMovimientoVM.Fecha,
                FichasCargadas = registroMovimientoVM.FichasCargadas,


                

                PesosEntregados = registroMovimientoVM.sPesosEntregados != null ? decimal.Parse(registroMovimientoVM.sPesosEntregados) : 0,
                PesosDevueltos = registroMovimientoVM.sPesosDevueltos != null ? decimal.Parse(registroMovimientoVM.sPesosDevueltos) : 0,
                Comision = registroMovimientoVM.sComision != null ?  decimal.Parse(registroMovimientoVM.sComision) : 0,

                //  PesosEntregados = registroMovimientoVM.PesosEntregados,
                //PesosDevueltos = registroMovimientoVM.PesosDevueltos,
                //Comision = registroMovimientoVM.Comision,
                EsIngresoFichas = registroMovimientoVM.EsIngresoFichas,


            };
        }

        public bool validarDatos(RegistroMovimientoViewModel registroMovimientoVM)
        {
            bool masErrores = false;

            if (registroMovimientoVM.EsIngresoFichas == true)
            {

                if (registroMovimientoVM.FichasCargadas <= 0)
                {
                    ModelState.AddModelError(registroMovimientoVM.FichasCargadas.ToString(), "La cantidad de fichas debe ser mayor a cero.");
                    masErrores = true;

                }


                //busco las fichas maximas que puede cargar el cajero
                int fichasCajeroBD = _contenedorTrabajo.Cajero.Get(registroMovimientoVM.CajeroId).FichasCargar;

                if (registroMovimientoVM.FichasCargadas > fichasCajeroBD)
                {
                    ModelState.AddModelError(registroMovimientoVM.FichasCargadas.ToString(), "La cantidad de fichas ingresadas es mayor a la que se le pueden cargar a este cajero.");
                    masErrores = true;

                }
            }
            else
            {
                if (registroMovimientoVM.PesosEntregados <= 0 && registroMovimientoVM.PesosDevueltos <= 0 && registroMovimientoVM.Comision <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Se debe ingresar al menos un monto de dinero.");
                    masErrores = true;
                }
            }



            return masErrores;
        }

        #endregion

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

            IEnumerable<RegistroMovimiento>? listaRegistros;


            if (searchValue != "")
            {
                listaRegistros = _contenedorTrabajo.RegistroMovimiento.GetAll(r => r.CajeroUser.Nombre.Contains(searchValue) || r.CajeroUser.Apellido.Contains(searchValue), includeProperties: "CajeroUser");               

            }
            else
            {
                listaRegistros = _contenedorTrabajo.RegistroMovimiento.GetAll(includeProperties: "CajeroUser");
            };
           

            recordsTotal = listaRegistros.Count();

            listaRegistros = listaRegistros.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaRegistros });

        }

        [HttpPost]
        public IActionResult GetAllCajero()
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

          

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var emailUsuarioActual = usuarioActual.Subject.Name;

            IEnumerable<RegistroMovimiento>? listaRegistros;


            if (searchValue != "")
            {
                listaRegistros = _contenedorTrabajo.RegistroMovimiento.GetAll(r => r.CajeroUser.Email == emailUsuarioActual 
                                   && ( r.FichasCargadas.ToString().Contains(searchValue) || r.PesosEntregados.ToString().Contains(searchValue)
                                        || r.PesosDevueltos.ToString().Contains(searchValue) || r.Comision.ToString().Contains(searchValue)
                                        && r.CajeroUser.DeudaPesosActual.ToString().Contains(searchValue) || r.FechaCreacion.ToString().Contains(searchValue)

                                   ), 
                                   
                                   
                                   includeProperties: "CajeroUser");

            }
            else
            {
                listaRegistros = _contenedorTrabajo.RegistroMovimiento.GetAll(r => r.CajeroUser.Email == emailUsuarioActual, includeProperties: "CajeroUser");
            };


            recordsTotal = listaRegistros.Count();

            listaRegistros = listaRegistros.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaRegistros });
                    
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.RegistroMovimiento.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error borrando el registro" });
            }


            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {

                    if (objFromDb.EsIngresoFichas == false)
                    {
                        var deudaNueva = RealizarCalculos(objFromDb, Metodo.Eliminicacion);

                        _contenedorTrabajo.Cajero.UpdateDeuda(objFromDb.CajeroId, deudaNueva);
                    }


                    //_contenedorTrabajo.RegistroMovimiento.Update(registroMovimiento);

                    _contenedorTrabajo.RegistroMovimiento.Remove(objFromDb);
                    _contenedorTrabajo.Save();
                

                    transaction.Commit();

                    return Json(new { success = true, message = "Registro borrado Correctamente" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();


                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    }

                    return Json(new { success = false, message = "Error borrando el registro" });
                }
                //registroMovimientoVM.ListaCajeros = _contenedorTrabajo.RegistroMovimiento.GetListaCajeros();

                //ViewBag.CurrentDateTime = registroMovimientoVM.Fecha.ToString("yyyy-MM-dd HH:mm");
                //return View(registroMovimientoVM);
            }


           
        }

        #endregion
    }
}
