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
using NuGet.Packaging.Signing;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using Microsoft.VisualBasic;
namespace CasinoRegistro.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CajerosController : Controller
    {
        #region variables string
        
        //imagen
        
        string nombreArchivo = "";
        string subidas = "";
        string extension = "";

        string rutaImagen = "";
        string rutaImagenAntigua = "";
        string emailAntiguo = "";

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

        string emailUsuarioActual = "";


        #endregion

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment; // para acceder a las carpetas del proyecto, para guardar/editar la imagen      
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CasinoRegistroDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<Plataforma> _logger;


        public CajerosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment,
            UserManager<IdentityUser> userManager, CasinoRegistroDbContext db, RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger<Plataforma> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
            _config = config;
            _logger = logger;
      
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
            if (id == null)
            {
                return NotFound();
            }

            CajeroViewModel cajeroViewModel = new CajeroViewModel()
            {
                CajeroUserVM = new CasinoRegistro.Models.CajeroUser()                              
            };

            if (id != null)
            {
                cajeroViewModel.CajeroUserVM = _contenedorTrabajo.Cajero.Get(id.GetValueOrDefault());


                if (cajeroViewModel.CajeroUserVM == null)
                {
                    return NotFound();

                }

                ////busco si el cajero tiene plataformas asociadas. Y si las tiene, las traigo en una lista 
                var cajeroPlataformasPorCajero = _contenedorTrabajo.CajeroPlataforma.GetAll( cp => cp.CajeroUserId == cajeroViewModel.CajeroUserVM.Id, includeProperties:"Plataforma");
                
                
                if (cajeroPlataformasPorCajero != null)
                { 

                    //En vez de IEnumerable<SelectListItem> también funciona con List<SelectListItem>
                    //ACA BUSCO LOS DATOS DE LAS PLATAFORMAS QUE CORRESPONDAN Y LAS CONVIERTO A SelectListItem
                    IEnumerable<SelectListItem> plat = _contenedorTrabajo.CajeroPlataforma.GetListaPlataformasCajeros(cajeroPlataformasPorCajero);
           
                //Paso a la vista la lista de plataformas a través de un ViewBag
                ViewBag.plataformas = plat;
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

                ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas(),           

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
                claimsIdentity = (ClaimsIdentity)this.User.Identity;
                usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                emailUsuarioActual = usuarioActual.Subject.Name;

                string tipo = cajeroVM.CajeroUserVM.EsCajero == true ? "Cajero" : "Secretaria";

                informacion = "Usuario: " + emailUsuarioActual + " - Tipo: " + tipo;
                
                _logger.LogInformation("CREACIÓN DE CAJERO \r\n Usuario registrado para el guardado: {Time} - {@informacion}", DateTime.Now, informacion);

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
                            nombreArchivo = Guid.NewGuid().ToString();   //como es nuevo el cajero, le pongo un Guid como nombre
                            subidas = Path.Combine(rutaPrincipal, @"imagenes\cajeros"); //accederá a la carpeta en wwwroot
                            extension = Path.GetExtension(archivo[0].FileName);
                         

                            //guardo la ruta en la base de datos
                            cajeroVM.CajeroUserVM.UrlImagen = @"\imagenes\cajeros\" + nombreArchivo + extension;


                            informacion = "URL Imagen: " + cajeroVM.CajeroUserVM.UrlImagen;
                            _logger.LogInformation("CREACIÓN DE CAJERO \r\n Guardado de imagen {Time} - {@informacion}", DateTime.Now, informacion);

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


                        informacion = "Rol seleccionado: " + rol;
                        _logger.LogInformation("CREACIÓN DE CAJERO \r\n Creación de rol {Time} - {@informacion}", DateTime.Now, informacion);

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

                        informacion = "Agregado a BD. ";
                        _logger.LogInformation("CREACIÓN DE CAJERO \r\n Cajero.Add {Time} - {@informacion}", DateTime.Now, informacion);


                        #region Envio del mail

                        //Para envio de correo

                        var mensaje = new MimeMessage();
                        //mensaje.From.Add(new MailboxAddress("Test Envio mail", "info@gmail.com")); // Test Envio mail: Nombre con el que aparece ademas del correo
                        mensaje.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
                        mensaje.To.Add(new MailboxAddress("Test Enviado", cajeroVM.CajeroUserVM.Email));

                        if (cajeroVM.CajeroUserVM.EsCajero)
                        {
                            mensaje.Subject = "Creación de cajero";

                            mensaje.Body = new TextPart("plain")
                            {
                                Text = "Bienvenido. Ha sido dado de alta como cajero en el Equipo Juampi.\r\n" +
                               "Su correo y credenciales son las siguientes. Se recomienda cambiar la contraseña desde su perfil luego de iniciar sesión. \r\n \r\n" +
                               "Correo: " + cajeroVM.CajeroUserVM.Email + "\r\n" +
                               "Contraseña: " + _config["EmailSettings:PasswordProvisoria"]



                            };
                        }

                        else
                        {
                            mensaje.Subject = "Creación de secretaria";

                            mensaje.Body = new TextPart("plain")
                            {
                                Text = "Bienvenido. Ha sido dado de alta como secretaria en el Equipo Juampi.\r\n" +
                               "Su correo y credenciales son las siguientes. Se recomienda cambiar la contraseña desde su perfil luego de iniciar sesión. \r\n \r\n" +
                               "Correo: " + cajeroVM.CajeroUserVM.Email + "\r\n" +
                               "Contraseña: " + _config["EmailSettings:PasswordProvisoria"]

                            };
                        }

                        informacion = "Email: " + cajeroVM.CajeroUserVM.Email;
                        _logger.LogInformation("CREACIÓN DE CAJERO \r\n Creación de texto para mail {Time} - {@informacion}", DateTime.Now, informacion);

                        #endregion

                        #region CREACION DE CORREO EN PLESK 

                        ////PROBAR ESTO --------------------------------------
                        ////USERNAME, PASSWORD Y DOMINIO SE PODRIAN PONER EN CONFIGURACION
                        ////EL 2 ES EL ID QUE DARAN EN PLESK

                        //string username = Request.Form["username"];
                        //string password = Request.Form["password"];

                        //Plesk plesk = new Plesk("https://tudominio.es:8443/enterprise/control/agent.php", "usuario de plesk", "contraseña");
                        //var result = plesk.AddEmail(2, username, password);
                        ////PROBAR ESTO --------------------------------------
                        ///
                        //informacion = "Username: " + username;
                        //_logger.LogInformation("Creación de Email en Plesk {Time} - {@informacion}", DateTime.Now, informacion);

                        #endregion


                        _contenedorTrabajo.Save();


                        if (archivo.Count() > 0) // cajeroVM.CajeroUserVM.Id == 0 &&
                        {

                            //recien guardo la imagen en la carpeta cuando se que todo fue bien
                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivo[0].CopyTo(fileStreams);
                                                                
                                _logger.LogInformation("CREACIÓN DE CAJERO \r\n Guardado de Imagen en carpeta {Time}", DateTime.Now);
                            }
                        }

                        #region Asignacion tabla Cajero-Plataforma
                        if (cajeroVM.CajeroUserVM.EsCajero && cajeroVM.IdsPlataformas != null)
                        {
                            //Se realiza en este punto porque es cuando se asigna el Id del Cajero                           

                            for (int i = 0; i < cajeroVM.IdsPlataformas.Count(); i++)
                            {
                                CajeroPlataforma cajeroPlataforma = new CajeroPlataforma();
                                //cajeroPlataforma.Id = 0;

                                cajeroPlataforma.CajeroUserId = cajeroVM.CajeroUserVM.Id;
                                cajeroPlataforma.PlataformaId = cajeroVM.IdsPlataformas[i];

                                _contenedorTrabajo.CajeroPlataforma.Add(cajeroPlataforma);
                                _contenedorTrabajo.Save();

                                informacion = "Cajero: " + cajeroPlataforma.CajeroUserId + " - " + "Plataforma: " + cajeroPlataforma.PlataformaId;
                                _logger.LogInformation("CREACIÓN DE CAJERO \r\n Asignación Cajero-Plataforma {Time} - {@informacion}", DateTime.Now, informacion);
                            }
                        }

                        #endregion

                        
                        transaction.Commit();
                        _logger.LogInformation("CREACIÓN DE CAJERO \r\n Commit Exitoso {Time}", DateTime.Now);

                        //envio del correo después del commit, para que no se mande antes de la creacion del correo
                        using (var cliente = new SmtpClient())
                        {
                            //cliente.Connect("smtp.gmail.com", 465);
                            //cliente.Authenticate("seba.acosta85", "agsahvnskuzxrlfu");
                            cliente.Connect(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), false);
                            cliente.Authenticate(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
                            cliente.Send(mensaje);
                            cliente.Disconnect(true);

                            
                            _logger.LogInformation("CREACIÓN DE CAJERO \r\n Envio de E-mail {Time}", DateTime.Now);

                        }


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

                            informacion = ex.InnerException.Message;
                            _logger.LogWarning("CREACIÓN DE CAJERO \r\n Error al querer guardar en Cajeros - InnerException {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                            informacion = ex.Message;
                            _logger.LogWarning("CREACIÓN DE CAJERO \r\n Error al querer guardar en Cajeros {Time} - {@informacion}", DateTime.Now, informacion);
                        }

                        var archivo = HttpContext.Request.Form.Files;
                      

                        //para no perder los datos de la lista de plataformas
                        cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();

                        return View(cajeroVM);
                    }
                }
            }


            if (cajeroVM.CajeroUserVM.EsCajero)
            {
                //para no perder los datos de la lista de plataformas
                cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();
            }           

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
                ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas(),
                IdsPlataformas = new List<int>()

            };

            if (id != null)
            {
                cajeroViewModel.CajeroUserVM = _contenedorTrabajo.Cajero.Get(id.GetValueOrDefault());

                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var rol = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

                if (rol == CNT.Secretaria && cajeroViewModel.CajeroUserVM.EsCajero == false)
                {
                    return View("AccesoDenegado");// PARA QUE UNA SECRETARIA PUEDA ACCEDER A EDITARLA DESDE LA URL, DESPUES DE ESTAR EDITANDO UN CAJERO
                }

                if (cajeroViewModel.CajeroUserVM == null)
                {
                    return NotFound();

                }


                //busco si el cajero tiene plataformas asociadas. Y si las tiene, las traigo en una lista 
                var cajeroPlataformasPorCajero = _contenedorTrabajo.CajeroPlataforma.GetAll(cp => cp.CajeroUserId == cajeroViewModel.CajeroUserVM.Id, includeProperties: "Plataforma");



                if (cajeroPlataformasPorCajero.Count() > 0)
                {


                    foreach (var item in cajeroPlataformasPorCajero)
                    {
                        cajeroViewModel.IdsPlataformas.Add(item.PlataformaId);
                    }

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
                claimsIdentity = (ClaimsIdentity)this.User.Identity;
                usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                emailUsuarioActual = usuarioActual.Subject.Name;

                string tipo = cajeroVM.CajeroUserVM.EsCajero == true ? "Cajero" : "Secretaria";

                informacion = "Usuario: " + emailUsuarioActual + " - Tipo: " + tipo + " - Id: " + cajeroVM.CajeroUserVM.Id;

                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Usuario registrado para la edición: {Time} - {@informacion}", DateTime.Now, informacion);

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {

                        #region Imagen
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivos = HttpContext.Request.Form.Files;

                        var cajeroDesdeBd = _contenedorTrabajo.Cajero.Get(cajeroVM.CajeroUserVM.Id);

                        emailAntiguo = cajeroDesdeBd.Email;

                        if (archivos.Count() > 0)
                        {
                            //Nuevo imagen para el artículo
                            nombreArchivo = Guid.NewGuid().ToString();
                            subidas = Path.Combine(rutaPrincipal, @"imagenes\cajeros");
                            extension = Path.GetExtension(archivos[0].FileName);                           


                            if (cajeroDesdeBd.UrlImagen != null)
                            {
                                //guardamos la ruta de la imagen de este archivo, para que si luego todo va bien, antes del commit eliminemos la imagen
                                // la guardo porque sino se pierde en el UPDATE y no vuelve a atras con el rollback
                                rutaImagenAntigua = Path.Combine(rutaPrincipal, cajeroDesdeBd.UrlImagen.TrimStart('\\'));

                                informacion = "URL Imagen Antigua: " + rutaImagenAntigua;
                                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);
                            }

                            cajeroVM.CajeroUserVM.UrlImagen = @"\imagenes\cajeros\" + nombreArchivo + extension;

                            informacion = "URL Imagen Nueva: " + cajeroVM.CajeroUserVM.UrlImagen;
                            _logger.LogInformation("EDICIÓN DE CAJERO \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);


                        }
                        else
                        {
                            //Aquí sería cuando la imagen ya existe y se conserva
                            cajeroVM.CajeroUserVM.UrlImagen = cajeroDesdeBd.UrlImagen;

                            informacion = "Se conserva la misma imagen.";
                            _logger.LogInformation("EDICIÓN DE CAJERO \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        #endregion


                        #region Aspnet Users y Rol
                        var user = await _userManager.FindByNameAsync(cajeroDesdeBd.Email);

                        //EDITO EL USUARIO EN ASPNETUSER
                        if (user != null)
                        {
                            if (emailAntiguo != cajeroVM.CajeroUserVM.Email)
                            {

                           
                                var setEmailResult = await _userManager.SetEmailAsync(user, cajeroVM.CajeroUserVM.Email);
                                var setUserNameResult = await _userManager.SetUserNameAsync(user, cajeroVM.CajeroUserVM.Email);


                                informacion = "Cambio de Correo: Antiguo: " + emailAntiguo + ", Nuevo correo: " + cajeroVM.CajeroUserVM.Email;
                                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Edición de Usuario en AspNetUser {Time} - {@informacion}", DateTime.Now, informacion);

                                if (setEmailResult.Succeeded && setUserNameResult.Succeeded)
                                {
                                    await _userManager.UpdateAsync(user);

                                    informacion = "Actualización success";
                                    _logger.LogInformation("EDICIÓN DE CAJERO \r\n Edición de Usuario en AspNetUser {Time} - {@informacion}", DateTime.Now, informacion);

                                }
                            }

                            //Obtenemos el rol seleccionado
                            string rol = Request.Form["radUsuarioRole"].ToString();

                            informacion = "Rol seleccionado: " + rol;
                            _logger.LogInformation("EDICIÓN DE CAJERO \r\n Edición de rol {Time} - {@informacion}", DateTime.Now, informacion);


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
                        #endregion


                        _contenedorTrabajo.Cajero.Update(cajeroVM.CajeroUserVM);

                        informacion = "Edición en BD. ";
                        _logger.LogInformation("EDICIÓN DE CAJERO \r\n Cajero.Update {Time} - {@informacion}", DateTime.Now, informacion);


                        #region Envio del mail

                        //Para envio de correo si se cambia

                        var mensaje = new MimeMessage();
                        if (user != null)
                        {
                            if (emailAntiguo != cajeroVM.CajeroUserVM.Email)
                            {

                                
                                //mensaje.From.Add(new MailboxAddress("Test Envio mail", "info@gmail.com")); // Test Envio mail: Nombre con el que aparece ademas del correo
                                mensaje.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
                                mensaje.To.Add(new MailboxAddress("Test Enviado", cajeroVM.CajeroUserVM.Email));

                                if (cajeroVM.CajeroUserVM.EsCajero)
                                {
                                    mensaje.Subject = "Creación de cajero";

                                    mensaje.Body = new TextPart("plain")
                                    {
                                        Text = "Bienvenido. Ha sido dado de alta como cajero en el Equipo Juampi.\r\n" +
                                       "Su correo y credenciales son las siguientes. Se recomienda cambiar la contraseña desde su perfil luego de iniciar sesión. \r\n \r\n" +
                                       "Correo: " + cajeroVM.CajeroUserVM.Email + "\r\n" +
                                       "Contraseña: "+ _config["EmailSettings:PasswordProvisoria"]



                                    };
                                }

                                else
                                {
                                    mensaje.Subject = "Creación de secretaria";

                                    mensaje.Body = new TextPart("plain")
                                    {
                                        Text = "Bienvenido. Ha sido dado de alta como secretaria en el Equipo Juampi.\r\n" +
                                       "Su correo y credenciales son las siguientes. Se recomienda cambiar la contraseña desde su perfil luego de iniciar sesión. \r\n \r\n" +
                                       "Correo: " + cajeroVM.CajeroUserVM.Email + "\r\n" +
                                       "Contraseña: " + _config["EmailSettings:PasswordProvisoria"]

                                    };
                                }

                                informacion = "Email: " + cajeroVM.CajeroUserVM.Email;
                                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Creación de texto para mail en edición {Time} - {@informacion}", DateTime.Now, informacion);
                            }
                        }

                        #endregion



                        #region tabla Cajero-Plataforma


                        //busco si el cajero tiene plataformas asociadas. Y si las tiene, las traigo en una lista para luego eliminarlas y crear las nuevas seleccionadas
                        var cajeroPlataformasPorCajero = _contenedorTrabajo.CajeroPlataforma.GetAll(cp => cp.CajeroUserId == cajeroVM.CajeroUserVM.Id, includeProperties: "Plataforma");


                        if (cajeroPlataformasPorCajero.Count() > 0)
                        {

                            foreach (var item in cajeroPlataformasPorCajero)
                            {
                                var objFromDb = _contenedorTrabajo.CajeroPlataforma.Get(item.Id);

                                _contenedorTrabajo.CajeroPlataforma.Remove(objFromDb);
                                _contenedorTrabajo.Save();

                               
                                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Remoción Cajero-Plataforma {Time}", DateTime.Now);
                            }

                         
                        }

                        if (cajeroVM.CajeroUserVM.EsCajero && cajeroVM.IdsPlataformas != null)
                        {
                            //Se realiza en este punto porque es cuando se asigna el Id del Cajero                           

                            for (int i = 0; i < cajeroVM.IdsPlataformas.Count(); i++)
                            {
                                CajeroPlataforma cajeroPlataforma = new CajeroPlataforma();
                                //cajeroPlataforma.Id = 0;

                                cajeroPlataforma.CajeroUserId = cajeroVM.CajeroUserVM.Id;
                                cajeroPlataforma.PlataformaId = cajeroVM.IdsPlataformas[i];

                                _contenedorTrabajo.CajeroPlataforma.Add(cajeroPlataforma);
                                _contenedorTrabajo.Save();

                                informacion = "Cajero: " + cajeroPlataforma.CajeroUserId + " - " + "Plataforma: " + cajeroPlataforma.PlataformaId;
                                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Edición Cajero-Plataforma {Time} - {@informacion}", DateTime.Now, informacion);

                            }
                        }

                        #endregion

                        _contenedorTrabajo.Save();


                        #region CREACION DE CORREO EN PLESK 

                        ////PROBAR ESTO --------------------------------------
                        ////USERNAME, PASSWORD Y DOMINIO SE PODRIAN PONER EN CONFIGURACION
                        ////EL 2 ES EL ID QUE DARAN EN PLESK

                        //string username = Request.Form["username"];
                        //string password = Request.Form["password"];

                        //Plesk plesk = new Plesk("https://tudominio.es:8443/enterprise/control/agent.php", "usuario de plesk", "contraseña");
                        //var result = plesk.AddEmail(2, username, password);
                        ////PROBAR ESTO --------------------------------------
                        ///
                        //informacion = "Username: " + username;
                        //_logger.LogInformation("EDICIÓN DE CAJERO \r\n Creación de Email en Plesk {Time} - {@informacion}", DateTime.Now, informacion);

                        #endregion


                        //elimino la imagen vieja y agrego la nueva cuando se que todo en la transaccion esta bien

                        if (rutaImagenAntigua != "")
                        {                                                      

                            if (System.IO.File.Exists(rutaImagenAntigua))
                            {
                                System.IO.File.Delete(rutaImagenAntigua);
                            }
                        }

                        if (archivos.Count() > 0)
                        {
                            //Nuevamente subimos el archivo
                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivos[0].CopyTo(fileStreams);

                                _logger.LogInformation("EDICIÓN DE CAJERO \r\n Guardado de Imagen en carpeta {Time}", DateTime.Now);
                            }
                        }                      


                        transaction.Commit();
                        _logger.LogInformation("EDICIÓN DE CAJERO \r\n Commit Exitoso {Time}", DateTime.Now);

                        if (user != null)
                        {
                            if (emailAntiguo != cajeroVM.CajeroUserVM.Email)

                                //envio del correo después del commit, para que no se mande antes de la creacion del correo
                                using (var cliente = new SmtpClient())
                                {
                                    //cliente.Connect("smtp.gmail.com", 465);
                                    //cliente.Authenticate("seba.acosta85", "agsahvnskuzxrlfu");
                                    cliente.Connect(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), false);
                                    cliente.Authenticate(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
                                    cliente.Send(mensaje);
                                    cliente.Disconnect(true);


                                    _logger.LogInformation("EDICIÓN DE CAJERO \r\n Envio de E-mail {Time}", DateTime.Now);
                                } 
                        }

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

                            informacion = ex.InnerException.Message;
                            _logger.LogWarning("EDICIÓN DE CAJERO \r\n Error al querer editar en Cajeros - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                            informacion = ex.Message;
                            _logger.LogWarning("EDICIÓN DE CAJERO \r\n Error al querer editar en Cajeros {Time} - {@informacion}", DateTime.Now, informacion);
                        }

                        //para no perder los datos de la lista de plataformas
                        cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();
                        return View(cajeroVM);
                    }
                }


            }
            //para no perder los datos de la lista de plataformas
            cajeroVM.ListaPlataformas = _contenedorTrabajo.Plataforma.GetListaPlataformas();
            return View(cajeroVM);
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


            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var emailUsuarioActual = usuarioActual.Subject.Name;

            IEnumerable<CajeroUser>? listaCajeros;

            if (searchValue != "")
            {

                listaCajeros = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual && u.EsCajero == true && 
                                (u.Id.ToString().Contains(searchValue) || u.Apellido.Contains(searchValue) || u.Nombre.Contains(searchValue ) || u.Email.Contains(searchValue) || u.DeudaPesosActual.ToString().Contains(searchValue)));

            }
            else
            {
                listaCajeros = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual && u.EsCajero == true);
            };

            //este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Cajero.GetLambda<CajeroUser>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaCajeros = listaCajeros.OrderByDescending(getNombreColumnaLambda);
            }
            else 
            {
                listaCajeros = listaCajeros.OrderBy(getNombreColumnaLambda);
            }         


            recordsTotal = listaCajeros.Count();
            
            listaCajeros = listaCajeros.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaCajeros });
        }        

        [HttpPost]     
        public IActionResult GetAllSecretaria()
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


            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var emailUsuarioActual = usuarioActual.Subject.Name;

            IEnumerable<CajeroUser>? listaCajeros;


            if (searchValue != "")
            {

                listaCajeros = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual && u.EsCajero == false && 
                                (u.Id.ToString().Contains(searchValue) || u.Apellido.Contains(searchValue) || u.Nombre.Contains(searchValue) || u.Email.Contains(searchValue)));

            }
            else
            {
                listaCajeros = _contenedorTrabajo.Cajero.GetAll(u => u.Email != emailUsuarioActual && u.EsCajero == false);
            };


            //este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Cajero.GetLambda<CajeroUser>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaCajeros = listaCajeros.OrderByDescending(getNombreColumnaLambda);
            }

            else
            {
                listaCajeros = listaCajeros.OrderBy(getNombreColumnaLambda);
            }

            recordsTotal = listaCajeros.Count();

            listaCajeros = listaCajeros.Skip(skip).Take(pageSize).ToList();
                        
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaCajeros });
        }


        public async Task<IActionResult> BloquearDesbloquearCajero(int id)
        {
            var cajeroDesdeBd = _contenedorTrabajo.Cajero.Get(id);

            var objFromDb = _userManager.FindByNameAsync(cajeroDesdeBd.Email);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;


            _logger.LogInformation("BLOQUEO DE CAJERO \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (objFromDb == null)
            {
           
                _logger.LogWarning("BLOQUEO DE CAJERO \r\n Error al buscar el Cajero a bloquear {Time}", DateTime.Now);

                return Json(new { success = false, message = "Error bloqueando el cajero." });
            }

            //en un solo metodo, bloqueo o desbloqueo el usuario segun corresponda

            if (objFromDb.Result.LockoutEnd == null || objFromDb.Result.LockoutEnd < DateTime.Now)
            {
                               

                cajeroDesdeBd.Estado = false;
                
               await _userManager.SetLockoutEndDateAsync(objFromDb.Result, DateTime.Now.AddYears(1000));
               await _userManager.UpdateAsync(objFromDb.Result);
                _contenedorTrabajo.Save();


                informacion = "Cajero Bloqueado Correctamente";
                _logger.LogWarning("BLOQUEO DE CAJERO \r\n Bloqueo de Cajeros {Time} - {@informacion}", DateTime.Now, informacion);


                return Json(new { success = true, message = "Cajero Bloqueado Correctamente" });
            }

            else
            {
        
                await _userManager.SetLockoutEndDateAsync(objFromDb.Result, null);
                await _userManager.UpdateAsync(objFromDb.Result);
                cajeroDesdeBd.Estado = true;

                _contenedorTrabajo.Save();

                informacion = "Cajero DesBloqueado Correctamente";
                _logger.LogWarning("BLOQUEO DE CAJERO \r\n Bloqueo de Cajeros {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = true, message = "Cajero DesBloqueado Correctamente" });
            }   

        }     

        #endregion
    }
}
