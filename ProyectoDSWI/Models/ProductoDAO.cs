using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ProyectoDSWI.Entity;
using ProyectoDSWI.Services;
using ProyectoDSWI.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoDSWI.Models
{
    public class ProductoDAO : IDaoProducto<Producto1>
    {
        public void ActualizarProducto(Producto1 p)
        {
            SqlConnection cn = DBAccess.getConecta();
            SqlCommand cmd = new SqlCommand("usp_ProductoActualizar", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);
            cmd.Parameters.AddWithValue("@NombrePro", p.NombrePro);
            cmd.Parameters.AddWithValue("@Precio", p.Precio);
            cmd.Parameters.AddWithValue("@Foto", p.Foto);
            cmd.Parameters.AddWithValue("@IdCategoria", p.IdCategoria);
            cmd.Parameters.AddWithValue("@Stock", p.Stock);

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally { cn.Close(); }

        }

        public void BajaProducto(Producto1 p)
        {
            SqlConnection cn = DBAccess.getConecta();
            SqlCommand cmd = new SqlCommand("usp_ProductoBaja", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);

            try
            {
                cn.Open();
                bool ires = cmd.ExecuteNonQuery() == 1 ? true : false;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally { cn.Close(); }
        }

        public Producto1 BuscarProducto(int id)
        {
            Producto1 reg = null;
            SqlConnection cn = DBAccess.getConecta();
            SqlCommand cmd = new SqlCommand("usp_ProductoDatos", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdProducto", id);

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    reg = new Producto1()
                    {
                        IdProducto = Convert.ToInt32(dr[0]),
                        NombrePro = dr[1].ToString(),
                        Precio = Convert.ToDecimal(dr[2]),
                        FechaCreacion = Convert.ToDateTime(dr[3]),
                        Foto = dr[4].ToString(),
                        IdCategoria = Convert.ToInt32(dr[5]),
                        Stock = Convert.ToInt32(dr[6])
                    };
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally { cn.Close(); }
            return reg;
        }

        public void InsertarProducto(Producto1 p)
        {
            SqlConnection cn = DBAccess.getConecta();
            SqlCommand cmd = new SqlCommand("usp_ProductoInsertar", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NombrePro", p.NombrePro);
            cmd.Parameters.AddWithValue("@Precio", p.Precio);
            cmd.Parameters.AddWithValue("@FechaCreacion", p.FechaCreacion);
            cmd.Parameters.AddWithValue("@Foto", p.Foto);
            cmd.Parameters.AddWithValue("@IdCategoria", p.IdCategoria);
            cmd.Parameters.AddWithValue("@Stock", p.Stock);

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally { cn.Close(); }
        }

        public List<Producto1> ListarProductos()
        {
            List<Producto1> lista = new List<Producto1>();
            SqlConnection cn = DBAccess.getConecta();
            SqlCommand cmd = new SqlCommand("usp_Producto_Activo", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Producto1 reg = new Producto1()
                    {
                        IdProducto = Convert.ToInt32(dr[0]),
                        NombrePro = dr[1].ToString(),
                        Precio = Convert.ToDecimal(dr[2]),
                        FechaCreacion = Convert.ToDateTime(dr[3]),
                        Foto = dr[4].ToString(),
                        IdCategoria = Convert.ToInt32(dr[5]),
                        Stock = Convert.ToInt32(dr[6])
                    };
                    lista.Add(reg);
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally { cn.Close(); }
            return lista;
        }
    }
}