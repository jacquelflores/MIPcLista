using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using ProyectoDSWI.Entity;
using ProyectoDSWI.Models;
using ProyectoDSWI.Filters;

namespace ProyectoDSWI.Controllers
{
    public class ProductoController : Controller
    {
        private ProyectoBDEntities db = new ProyectoBDEntities();
        [AuthorizeUser(idOperacion: 5)]
        public ActionResult Index2()
        {
            return View(db.Producto.ToList().OrderBy(x => x.NombrePro));
        }

        SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["Business1"].ConnectionString);
        // GET: Producto
        CategoriaDAO objcat = new CategoriaDAO();
        ProductoDAO objpro = new ProductoDAO();

        List<Producto1> Productos()
        {
            List<Producto1> temporal = new List<Producto1>();
            SqlCommand cmd = new SqlCommand("usp_Producto_Activo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Producto1 reg = new Producto1
                {
                    IdProducto = dr.GetInt32(0),

                    NombrePro = dr.GetString(1),

                    Precio = dr.GetDecimal(2),

                    FechaCreacion = dr.GetDateTime(3),

                    Foto = dr.GetString(4),

                    IdCategoria = dr.GetInt32(5),

                    Stock = dr.GetInt32(6)
                };
                temporal.Add(reg);
            }
            dr.Close(); cn.Close();
            return temporal;
        }

        List<Categoria1> Categorias()

        {

            List<Categoria1> temporal = new List<Categoria1>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Categoria", cn);

            cmd.CommandType = CommandType.Text;

            cn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())

            {

                Categoria1 reg = new Categoria1

                {

                    IdCategoria = dr.GetInt32(0),

                    NombreCate = dr.GetString(1)

                };

                temporal.Add(reg);

            }

            dr.Close(); cn.Close();

            return temporal;

        }


        [AuthorizeUser(idOperacion: 2)]
        public ActionResult Index()
        {
            return View(objpro.ListarProductos().ToList());
        }

        public ActionResult Details(int id)
        {
            return View(objpro.BuscarProducto(id));
        }

        public ActionResult Create()
        {
            ViewBag.categorias = new SelectList(Categorias(), "IdCategoria", "NombreCate");
            return View(new Producto1());
            /*ViewBag.categorias = new SelectList(objcat.ListarCategorias(),
                "IdCategoria", "NombreCate");

            return View(new Producto());*/
        }

        [HttpPost]

        public ActionResult Create(Producto1 reg)
        {
            if (!ModelState.IsValid)
            {
                return View(reg);
            }
            ViewBag.mensaje = " ";
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                SqlCommand cmd = new SqlCommand("usp_ProductoInsertar", cn, tr);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NombrePro", reg.NombrePro);
                cmd.Parameters.AddWithValue("@Precio", reg.Precio);
                cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@Foto", reg.Foto);
                cmd.Parameters.AddWithValue("@IdCategoria", reg.IdCategoria);
                cmd.Parameters.AddWithValue("@Stock", reg.Stock);
                int q = cmd.ExecuteNonQuery();
                tr.Commit();
                ViewBag.mensaje = q.ToString() + " Producto Agregado";
            }
            catch (SqlException ex)
            {
                ViewBag.mensaje = ex.Message;
                tr.Rollback();
            }
            finally
            {
                cn.Close();
            }
            
            ViewBag.categorias = new SelectList(Categorias(), "IdCategoria", "NombreCate", reg.IdCategoria);
            return View(reg);
        }

        public ActionResult Edit(int id)
        {
            Producto1 reg = objpro.BuscarProducto(id);
            ViewBag.categorias = new SelectList(objcat.ListarCategorias(),
                "IdCategoria", "NombreCate", reg.IdCategoria);
            return View(objpro.BuscarProducto(id));
        }

        [HttpPost]
        public ActionResult Edit(Producto1 reg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objpro.ActualizarProducto(reg);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            { return View(); }
        }

        public ActionResult Delete(int id)
        {
            Producto1 pro = objpro.BuscarProducto(id);
            return View(pro);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto1 pro = objpro.BuscarProducto(id);
            objpro.BajaProducto(pro);
            return RedirectToAction("Index");
        }
    }
}