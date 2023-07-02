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
    public class CategoriaDAO : IDaoCategoria<Categoria1>
    {
        public List<Categoria1> ListarCategorias()
        {
            List<Categoria1> lista = new List<Categoria1>();
            SqlConnection cn = DBAccess.getConecta();
            SqlCommand cmd = new SqlCommand("usp_CategoriaListar", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Categoria1 reg = new Categoria1()
                    {
                        IdCategoria = Convert.ToInt32(dr[0]),
                        NombreCate = dr[1].ToString()
                    };
                    lista.Add(reg);
                }
                dr.Close();
                cn.Close();
            }
            catch (SqlException ex)
            { throw ex; }
            return lista;
        }
    }
}