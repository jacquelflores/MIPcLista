using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoDSWI.Models;

namespace ProyectoDSWI.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        ProyectoBDEntities db = new ProyectoBDEntities();
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registro()
        {
            return View();
        }
        public ActionResult NuevaContrasenia()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(String User, string Pass)
        {
            try
            {
                using (Models.ProyectoBDEntities db = new Models.ProyectoBDEntities())
                {
                    var oUser = (from d in db.usuario
                                where d.email == User && d.password == Pass.Trim()
                                select d).FirstOrDefault();
                    if (oUser == null)
                    {
                        ViewBag.Error = "Usario o Contraseña Invalida";
                        return View();
                    }

                    Session["User"] = oUser;
                    Session["nombreUsuario"] = oUser.nombre;
                    Session["correoUsuario"] = oUser.email;
                    Session["dniUsuario"] = oUser.dni;

                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Registro(string Name, string Dni, string User, string Pass, string PassRep)
        {
            if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Dni) || String.IsNullOrEmpty(User) || String.IsNullOrEmpty(Pass) || String.IsNullOrEmpty(PassRep) )
            {
                ViewBag.Error = "Debe ingresar todos los datos del formulario";
                return View();
            }
            if ((PassRep!=null && Pass!=null) && Pass != PassRep )
            {
                ViewBag.Error = "Las contraseñas ingresadas deben coincidir";
                return View();
            }
            try
            {
                using (Models.ProyectoBDEntities db = new Models.ProyectoBDEntities())
                {
                    var oUser = (from d in db.usuario
                                 where d.email == User
                                 select d).FirstOrDefault();
                    if (oUser != null)
                    {
                        ViewBag.Error = "Usario ya se encuentra registrado con este correo.";
                        return View();
                    }
                }

                usuario newUsuario = new usuario();
                newUsuario.fecha = DateTime.Now;
                newUsuario.nombre = Name;
                newUsuario.dni = Dni;
                newUsuario.email = User;
                newUsuario.password = Pass;
                newUsuario.idRol = 2;

                try
                {
                    db.usuario.Add(newUsuario);
                    envioMailRegistro(Name, User, Dni, "");
                }
                catch (Exception e)
                {

                }
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        private int envioMailRegistro(string nombres, string correo, string dni, string correoCopia = "")
        {
            string mail_contact = ConfigurationManager.AppSettings["MAIL_CONTACT"];
            var asunto = "Notificación de nuevo Registro";
            var body = string.Format("<b>Hola, Bienvenido a Venta de equipos informaticos. Agradecemos tu preferencia </b>, <br>" +
                "La persona de nombre {0} con DNI {1} y correo: {2}. Se ha registrado en nuestra web." +
                  "<br>Saludos." +
                "<br>En caso de problemas, contactarse a {3} <br><br>Atentamente<br>Equipo Venta de equipos informaticos<br>",
                nombres, dni, correo,
                mail_contact);
            return new Carrito1Item().sendMailSMTP(correo, asunto, body, correoCopia);
        }

        [HttpPost]
        public ActionResult NuevaContrasenia(string Correo, string Dni, string Pass, string PassRep)
        {
            if (String.IsNullOrEmpty(Correo) || String.IsNullOrEmpty(Dni) || String.IsNullOrEmpty(Pass) || String.IsNullOrEmpty(PassRep))
            {
                ViewBag.Error = "Debe ingresar todos los datos del formulario";
                return View();
            }
            if ((PassRep != null && Pass != null) && Pass != PassRep)
            {
                ViewBag.Error = "Las contraseñas ingresadas deben coincidir";
                return View();
            }
            try
            {
                using (Models.ProyectoBDEntities db = new Models.ProyectoBDEntities())
                {
                    var oUser = (from d in db.usuario
                                 where d.email == Correo
                                 select d).FirstOrDefault();
                    if (oUser == null)
                    {
                        ViewBag.Error = "No existe un usuario registrado con este correo.";
                        return View();
                    }
                    var oUser2 = (from d in db.usuario
                                 where d.email == Correo && d.dni == Dni
                                 select d).FirstOrDefault();
                    if (oUser2 == null)
                    {
                        ViewBag.Error = "El DNI ingresado no corresponde al usuario con el correo ingresado.";
                        return View();
                    }
                }


                try
                {
                    var query = (from d in db.usuario
                                 where d.email == Correo && d.dni == Dni
                                 select d).FirstOrDefault();

                    query.password = Pass;

                    db.SaveChanges();
                    envioMailNuevaContrasenia(Correo, Dni, "");
                }
                catch (Exception e)
                {

                }
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        private int envioMailNuevaContrasenia(string correo, string dni, string correoCopia = "")
        {
            string mail_contact = ConfigurationManager.AppSettings["MAIL_CONTACT"];
            var asunto = "Notificación de cambio de contraseña";
            var body = string.Format("<b>Hola.</b>, <br>" +
                " Hemos detectado un cambio de contraseña para tu cuenta asociada al dni: {0} y correo: {1}." +
                  "<br>Saludos." +
                "<br>En caso de que no hayas sido tú o tengas otros problemas, contactarse a {2} <br><br>Atentamente<br>Equipo Venta de equipos informaticos<br>",
                dni, correo,
                mail_contact);
            return new Carrito1Item().sendMailSMTP(correo, asunto, body, correoCopia);
        }
    }
}