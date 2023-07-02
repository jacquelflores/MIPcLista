using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ProyectoDSWI.Models;
using ProyectoDSWI.Filters;
using System.Configuration;

namespace ProyectoDSWI.Controllers
{
    public class CarritoController : Controller
    {
        // GET: Carrito
        ProyectoBDEntities db = new ProyectoBDEntities();

        private int getIndice(int id)
        {
            List<Carrito1Item> compras = (List<Carrito1Item>)Session["carrito"];
            for (int i = 0; i < compras.Count; i++)
            {
                if (compras[i].Producto.IdProducto == id)
                    return i;
            }
            return -1;
        }

        [HttpGet]
        [AuthorizeUser(idOperacion: 5)]
        public ActionResult AgregarCarrito()
        {
            return View();
        }

        [HttpPost]

        public JsonResult AgregarCarrito(int id, int cantidad)
        {

            if (Session["carrito"] == null)
            {
                List<Carrito1Item> compras = new List<Carrito1Item>();
                compras.Add(new Carrito1Item(db.Producto.Find(id), cantidad));
                Session["carrito"] = compras;
            }
            else
            {
                List<Carrito1Item> compras = (List<Carrito1Item>)Session["carrito"];
                int Indice = getIndice(id);
                if (Indice == -1)
                    compras.Add(new Carrito1Item(db.Producto.Find(id), cantidad));
                else
                    compras[Indice].Cantidad += cantidad;
                Session["carrito"] = compras;
            }
            //return View();
            return Json(new { response = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            List<Carrito1Item> compras = (List<Carrito1Item>)Session["carrito"];
            compras.RemoveAt(getIndice(id));
            return View("AgregarCarrito");
        }

        public ActionResult FinalizarCompra(string tipoTarjeta, string NumTarjeta, string NomTitu)
        {
            List<Carrito1Item> compras = (List<Carrito1Item>)Session["carrito"];
            if (compras != null && compras.Count > 0)
            {
                Venta newVenta = new Venta();
                newVenta.DiaVenta = DateTime.Now;
                newVenta.SubTotal = compras.Sum(x => x.Producto.Precio * x.Cantidad);
                newVenta.Iva = newVenta.SubTotal * 0.18;
                newVenta.Total = newVenta.SubTotal + newVenta.Iva;

                newVenta.ListaVenta = (from Producto in compras
                                       select new ListaVenta
                                       {
                                           IdProducto = Producto.Producto.IdProducto,
                                           Cantidad = Producto.Cantidad,
                                           Total = Producto.Cantidad * Producto.Producto.Precio
                                       }).ToList();

                string nombreUsuario = (string)Session["nombreUsuario"];
                string correoUsuario = (string)Session["correoUsuario"];
                string dniUsuario = (string)Session["dniUsuario"];

                try
                {
                    db.Venta.Add(newVenta);
                    envioMailSolicitudCompra(nombreUsuario, correoUsuario, dniUsuario, compras,tipoTarjeta,NumTarjeta, NomTitu, "");
                }
                catch (Exception e)
                {
                    
                }
                db.SaveChanges();
                Session["carrito"] = new List<Carrito1Item>();
            }
            return View();
        }
        private int envioMailSolicitudCompra(string nombres, string correo, string dni, List<Carrito1Item> listaProductos ,string tipoTarjeta, string NumTarjeta,string NomTitu, string correoCopia = "")
        {
            string listaProductosBody = "";
            double total = 0;
            foreach (var item in listaProductos)
            {
                listaProductosBody = listaProductosBody + "<br><img src=" + item.Producto.Foto + " ><br>" +
                    item.Producto.NombrePro + "<p>Precio: </p>" + item.Producto.Precio +
                    "<p> Cantidad: </p>" + item.Cantidad;
                total = (double)(total + item.Cantidad * item.Producto.Precio);
            }
            listaProductosBody = listaProductosBody + "<p>Subtotal: </p>" + total;

            string mail_contact = ConfigurationManager.AppSettings["MAIL_CONTACT"];
            var asunto = "Notificación de nueva compra";
            var body = string.Format("<b>Hola, en Venta de equipos informaticos agradecemos tu preferencia </b>, <br>" +
                "La compra del usuario {0} con DNI {1} y correo registrado: {2} con tarjeta: {5} , numero: {6}, con titular: {7}  . Ha realizado la compra de: <br>{3}" +
                  "<br>Saludos." +
                "<br>En caso de problemas, contactarse a {4} <br><br>Atentamente<br>Equipo Venta de equipos informaticos<br>",
                nombres , dni, correo, listaProductosBody,
                mail_contact, tipoTarjeta, NumTarjeta, NomTitu);
            return new Carrito1Item().sendMailSMTP(correo, asunto, body, correoCopia);
        }
    }
}