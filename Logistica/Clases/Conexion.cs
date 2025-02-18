using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Highsoft.Web.Mvc.Charts;
using System.Diagnostics;

namespace TruckyV2.Clases
{
    public class Conexion
    {
        public static string cn = ConfigurationManager.ConnectionStrings["Trucky"].ToString();
        public static string cnP = ConfigurationManager.ConnectionStrings["Logistica"].ToString();
        public List<PlanTransporte> getTablePlan(string dateFind)
        {
            List<PlanTransporte> listPlan = new List<PlanTransporte>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM PlanTransporte WHERE FechaPlan = @Fecha ORDER BY HrLlegadaPlan", conn);
                    cmd.Parameters.AddWithValue("@Fecha", dateFind);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listPlan.Add(new PlanTransporte()
                            {
                                Folio = dr["Folio"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Folio"]),
                                ASN = dr["ASN"] == DBNull.Value ? "" : dr["ASN"].ToString(),
                                Status = dr["Status"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Status"]),
                                FechaPlan = dr["FechaPlan"] == DBNull.Value ? "" : dr["FechaPlan"].ToString().Substring(0, 10),
                                HrLlegadaPlan = dr["HrLlegadaPlan"] == DBNull.Value ? "" : dr["HrLlegadaPlan"].ToString(),
                                HrSalidaPlan = dr["HrSalidaPlan"] == DBNull.Value ? "" : dr["HrSalidaPlan"].ToString(),
                                Proveedor = dr["Proveedor"] == DBNull.Value ? "" : dr["Proveedor"].ToString(),
                                Proyecto = dr["Proyecto"] == DBNull.Value ? "" : dr["Proyecto"].ToString(),
                                Unidad = dr["Unidad"] == DBNull.Value ? "" : dr["Unidad"].ToString(),
                                LineaTrans = dr["LineaTrans"] == DBNull.Value ? "" : dr["LineaTrans"].ToString(),
                                Proceso = dr["Proceso"] == DBNull.Value ? "" : dr["Proceso"].ToString(),
                                LlegadaReal = dr["LlegadaReal"] == DBNull.Value ? "" : dr["LlegadaReal"].ToString(),
                                EntRampaReal = dr["EntRampaReal"] == DBNull.Value ? "" : dr["EntRampaReal"].ToString(),
                                SalRampaReal = dr["SalRampaReal"] == DBNull.Value ? "" : dr["SalRampaReal"].ToString(),
                                SalidaReal = dr["SalidaReal"] == DBNull.Value ? "" : dr["SalidaReal"].ToString(),
                                Area = dr["Area"] == DBNull.Value ? "" : dr["Area"].ToString(),
                                Rampa = dr["Rampa"] == DBNull.Value ? "" : dr["Rampa"].ToString(),
                                AutorizoRampa = dr["AutorizoRampa"] == DBNull.Value ? "" : dr["AutorizoRampa"].ToString(),
                                CargadoPor = dr["CargadoPor"] == DBNull.Value ? "" : dr["CargadoPor"].ToString(),
                                FechaCarga = dr["FechaCarga"] == DBNull.Value ? "" : dr["FechaCarga"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                listPlan = new List<PlanTransporte>();
            }

            return listPlan;
        }
        public List<PlanTransporte> getDetailFolio(int Folio)
        {
            List<PlanTransporte> detailFolio = new List<PlanTransporte>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM PlanTransporte WHERE Folio = @Folio", conn);
                    cmd.Parameters.AddWithValue("@Folio", Folio);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            detailFolio.Add(new PlanTransporte()
                            {
                                Folio = dr["Folio"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Folio"]),
                                ASN = dr["ASN"] == DBNull.Value ? "" : dr["ASN"].ToString(),
                                Status = dr["Status"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Status"]),
                                FechaPlan = dr["FechaPlan"] == DBNull.Value ? "" : dr["FechaPlan"].ToString(),
                                HrLlegadaPlan = dr["HrLlegadaPlan"] == DBNull.Value ? "" : dr["HrLlegadaPlan"].ToString(),
                                HrSalidaPlan = dr["HrSalidaPlan"] == DBNull.Value ? "" : dr["HrSalidaPlan"].ToString(),
                                Proveedor = dr["Proveedor"] == DBNull.Value ? "" : dr["Proveedor"].ToString(),
                                Proyecto = dr["Proyecto"] == DBNull.Value ? "" : dr["Proyecto"].ToString(),
                                Unidad = dr["Unidad"] == DBNull.Value ? "" : dr["Unidad"].ToString(),
                                LineaTrans = dr["LineaTrans"] == DBNull.Value ? "" : dr["LineaTrans"].ToString(),
                                Proceso = dr["Proceso"] == DBNull.Value ? "" : dr["Proceso"].ToString(),
                                Carga = dr["Carga"] == DBNull.Value ? "" : dr["Carga"].ToString(),
                                FechaCarga = dr["FechaCarga"] == DBNull.Value ? "" : dr["FechaCarga"].ToString(),
                            });
                        }
                    }
                }
            }
            catch
            {
                detailFolio = new List<PlanTransporte>();
            }

            return detailFolio;
        }
        public List<Transportistas> getDataTransportista(int Folio)
        {
            List<Transportistas> dataTransportista = new List<Transportistas>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("SELECT TOP(1) * FROM Transportistas WHERE Folio = @Folio", conn);
                    cmd.Parameters.AddWithValue("@Folio", Folio);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            dataTransportista.Add(new Transportistas()
                            {
                                Folio = dr["Folio"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Folio"]),
                                Nombre = dr["Nombre"] == DBNull.Value ? "" : dr["Nombre"].ToString(),
                                Placas = dr["Placas"] == DBNull.Value ? "" : dr["Placas"].ToString(),
                                Empresa = dr["Empresa"] == DBNull.Value ? "" : dr["Empresa"].ToString(),
                                Origen = dr["Origen"] == DBNull.Value ? "" : dr["Origen"].ToString(),
                                Gafete = dr["Gafete"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Gafete"]),
                                Usuario = dr["Usuario"] == DBNull.Value ? "" : dr["Usuario"].ToString(),
                                Contenedor = dr["Contenedor"] == DBNull.Value ? "" : dr["Contenedor"].ToString(),
                                Tracto = dr["Tracto"] == DBNull.Value ? "" : dr["Tracto"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                dataTransportista = new List<Transportistas>();
            }

            return dataTransportista;
        }
        public bool ChangeStatusAndInsert(int Folio, int Status, string Rampa, string AutorizedBy)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        SqlCommand updateCommand = new SqlCommand();
                        updateCommand.Connection = connection;
                        updateCommand.Transaction = transaction;

                        switch (Status)
                        {
                            case 1:
                                updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 2, Rampa = @Rampa, AutorizoRampa = @AutorizedBy WHERE Folio = @Folio";
                                updateCommand.Parameters.AddWithValue("@Rampa", Rampa);
                                updateCommand.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);
                                break;
                            case 2:
                                updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 3, EntRampaReal = GETDATE() WHERE Folio = @Folio";
                                break;
                            case 3:
                                updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 4, SalRampaReal = GETDATE() WHERE Folio = @Folio";
                                break;
                            case 4:
                                updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 5, SalidaReal = GETDATE() WHERE Folio = @Folio";
                                break;
                            default:
                                throw new ArgumentException("Status no válido");
                        }

                        updateCommand.Parameters.AddWithValue("@Folio", Folio);
                        updateCommand.ExecuteNonQuery();

                        SqlCommand insertCommand = new SqlCommand();
                        insertCommand.Connection = connection;
                        insertCommand.Transaction = transaction;
                        insertCommand.CommandText = "INSERT INTO Movimientos (Folio, Accion, Usuario) VALUES (@Folio, @Accion, @AutorizedBy)";
                        insertCommand.Parameters.AddWithValue("@Folio", Folio);
                        insertCommand.Parameters.AddWithValue("@Accion", Status + 1);
                        insertCommand.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);
                        insertCommand.ExecuteNonQuery();

                        transaction.Commit();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public bool inserDataRegsProvsT2(int Folio, string AutorizedBy, string transportista, string Empresa, string placas, string origen, string gafete, string contenedor, string tracto)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        SqlCommand updateCommand = new SqlCommand();
                        updateCommand.Connection = connection;
                        updateCommand.Transaction = transaction;

                        updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 1, LlegadaReal = GETDATE() WHERE Folio = @Folio";
                        updateCommand.Parameters.AddWithValue("@Folio", Folio);
                        updateCommand.ExecuteNonQuery();

                        SqlCommand insertCommand1 = new SqlCommand();
                        insertCommand1.Connection = connection;
                        insertCommand1.Transaction = transaction;
                        insertCommand1.CommandText = "INSERT INTO Movimientos (Folio, Accion, Usuario) VALUES (@Folio, @Accion, @AutorizedBy)";
                        insertCommand1.Parameters.AddWithValue("@Folio", Folio);
                        insertCommand1.Parameters.AddWithValue("@Accion", 1);
                        insertCommand1.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);
                        insertCommand1.ExecuteNonQuery();

                        SqlCommand insertCommand2 = new SqlCommand();
                        insertCommand2.Connection = connection;
                        insertCommand2.Transaction = transaction;
                        insertCommand2.CommandText = "INSERT INTO Transportistas (Folio, Nombre, Placas, Empresa, Origen, Gafete, Fecha, Usuario, Contenedor, Tracto) " +
                                                     "VALUES (@Folio, @Nombre, @Placas, @Empresa, @Origen, @Gafete, GETDATE(), @Usuario, @Contenedor,@Tracto)";
                        insertCommand2.Parameters.AddWithValue("@Folio", Folio);
                        insertCommand2.Parameters.AddWithValue("@Nombre", transportista);
                        insertCommand2.Parameters.AddWithValue("@Placas", placas);
                        insertCommand2.Parameters.AddWithValue("@Empresa", Empresa);
                        insertCommand2.Parameters.AddWithValue("@Origen", origen);
                        insertCommand2.Parameters.AddWithValue("@Gafete", gafete);
                        insertCommand2.Parameters.AddWithValue("@Usuario", AutorizedBy);
                        insertCommand2.Parameters.AddWithValue("@Contenedor", contenedor);
                        insertCommand2.Parameters.AddWithValue("@Tracto", tracto);
                        insertCommand2.ExecuteNonQuery();

                        transaction.Commit();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                // Manejo del error, log, etc.
            }
            return result;
        }        
        public bool cancelFolio(int Folio, string AutorizedBy)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        SqlCommand updateCommand = new SqlCommand();
                        updateCommand.Connection = connection;
                        updateCommand.Transaction = transaction;

                        updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 6 WHERE Folio = @Folio";
                        updateCommand.Parameters.AddWithValue("@Folio", Folio);
                        updateCommand.ExecuteNonQuery();

                        SqlCommand insertCommand1 = new SqlCommand();
                        insertCommand1.Connection = connection;
                        insertCommand1.Transaction = transaction;
                        insertCommand1.CommandText = "INSERT INTO Movimientos (Folio, Accion, Usuario) VALUES (@Folio, @Accion, @AutorizedBy)";
                        insertCommand1.Parameters.AddWithValue("@Folio", Folio);
                        insertCommand1.Parameters.AddWithValue("@Accion", 6);
                        insertCommand1.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);
                        insertCommand1.ExecuteNonQuery();

                        transaction.Commit();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;   
            }
            return result;
        }
        public bool resetFolio(int Folio, string AutorizedBy)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        SqlCommand updateCommand = new SqlCommand();
                        updateCommand.Connection = connection;
                        updateCommand.Transaction = transaction;

                        updateCommand.CommandText = "UPDATE TOP(1) PlanTransporte SET Status = 0, LlegadaReal = NULL, EntRampaReal = NULL, SalRampaReal = NULL, SalidaReal = NULL, Rampa = NULL, AutorizoRampa = NULL WHERE Folio = @Folio";
                        updateCommand.Parameters.AddWithValue("@Folio", Folio);
                        updateCommand.ExecuteNonQuery();

                        SqlCommand insertCommand1 = new SqlCommand();
                        insertCommand1.Connection = connection;
                        insertCommand1.Transaction = transaction;
                        insertCommand1.CommandText = "INSERT INTO Movimientos (Folio, Accion, Usuario) VALUES (@Folio, @Accion, @AutorizedBy)";
                        insertCommand1.Parameters.AddWithValue("@Folio", Folio);
                        insertCommand1.Parameters.AddWithValue("@Accion", 7);
                        insertCommand1.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);
                        insertCommand1.ExecuteNonQuery();

                        SqlCommand deleteCommand1 = new SqlCommand();
                        deleteCommand1.Connection = connection;
                        deleteCommand1.Transaction = transaction;
                        deleteCommand1.CommandText = "DELETE TOP(1) Transportistas WHERE Folio = @Folio";
                        deleteCommand1.Parameters.AddWithValue("@Folio", Folio);
                        deleteCommand1.ExecuteNonQuery(); // Aquí se ejecuta el comando de eliminación

                        transaction.Commit();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }


        public (int insertados, int total, string mensaje) InsertarDatosPlanCSV(string nombreUsuario, List<PlanTransporte> listPlan)
        {
            int cantidadInsertados = 0;
            int totalRegistros = listPlan.Count; // Total de registros en la lista
            string Message = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(cn))
                {
                    conn.Open();
                    foreach (var dato in listPlan)
                    {
                        DateTime hrLlegadaPlan = DateTime.ParseExact(dato.HrLlegadaPlan.Substring(0, 16), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        DateTime hrSalidaPlan = DateTime.ParseExact(dato.HrSalidaPlan.Substring(0, 16), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO PlanTransporte (FechaPlan, Proveedor, Proyecto, Unidad, LineaTrans, Carga, HrLlegadaPlan, HrSalidaPlan, Proceso, FechaCarga, CargadoPor, Area, ASN) VALUES (@FechaPlan, @Proveedor, @Proyecto, @TipoUnidad, @LineaTransportista, @Carga, @HrLlegadaPlan, @HrSalidaPlan, @Proceso, GETDATE(), @CargadoPor, @Area, @ASN)", conn))
                        {
                            cmd.Parameters.AddWithValue("@FechaPlan", DateTime.ParseExact(dato.FechaPlan, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@Proveedor", dato.Proveedor);
                            cmd.Parameters.AddWithValue("@Proyecto", dato.Proyecto);
                            cmd.Parameters.AddWithValue("@TipoUnidad", dato.Unidad);
                            cmd.Parameters.AddWithValue("@LineaTransportista", dato.LineaTrans);
                            cmd.Parameters.AddWithValue("@Carga", dato.Carga);
                            cmd.Parameters.AddWithValue("@HrLlegadaPlan", hrLlegadaPlan);
                            cmd.Parameters.AddWithValue("@HrSalidaPlan", hrSalidaPlan);
                            cmd.Parameters.AddWithValue("@Proceso", dato.Proceso);
                            cmd.Parameters.AddWithValue("@CargadoPor", nombreUsuario);
                            cmd.Parameters.AddWithValue("@Area", dato.Area);
                            cmd.Parameters.AddWithValue("@ASN", dato.ASN);

                            int filasAfectadas = cmd.ExecuteNonQuery();
                            if (filasAfectadas > 0)
                            {
                                cantidadInsertados += filasAfectadas;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return (cantidadInsertados, totalRegistros, Message); // Retorna el número de insertados, el total y el error
            }

            return (cantidadInsertados, totalRegistros, Message);
        }



        public int validateUser(int Nomina, string Password, out string Nombre, out string Apellidos, out string Message, out string Code)
        {
            int result = 0;
            Message = string.Empty;
            Nombre = string.Empty;
            Apellidos = string.Empty;
            Code = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("Production.dbo.sp_Validar_Usuario", conn);
                    cmd.Parameters.AddWithValue("nomina", Nomina);
                    cmd.Parameters.AddWithValue("password", Password);
                    cmd.Parameters.AddWithValue("idPlataforma", 36);
                    cmd.Parameters.Add("login", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("respuesta", SqlDbType.VarChar, 300).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("nombreUsuario", SqlDbType.VarChar, 150).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("apellidosUsuario", SqlDbType.VarChar, 300).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("code", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    result = Convert.ToInt32(cmd.Parameters["login"].Value);
                    Message = cmd.Parameters["respuesta"].Value.ToString();
                    Nombre = cmd.Parameters["nombreUsuario"].Value.ToString();
                    Apellidos = cmd.Parameters["apellidosUsuario"].Value.ToString();
                    Code = cmd.Parameters["code"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Message = ex.Message;
            }
            return result;
        }

        public Users obtenerDatosUsuario(int nomina)
        {
            Users user = null;

            try
            {
                using (var conn = new SqlConnection(Conexion.cnP))
                {
                    conn.Open();

                    // Query para traer todos los datos del usuario y los IDs de vistas permitidas en una sola consulta
                    var query = @"
                        SELECT 
	                        US.Nomina, RP.Fotografia AS PathPerfil, US.IDDepartamento, 
                            US.ModuloPerfilID
                        FROM Usuarios AS US INNER JOIN RH..Reclutamiento_Personal AS RP ON US.Nomina =  RP.Nomina 
                        WHERE US.Nomina = @Nomina";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nomina", nomina);

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                // Inicializar el usuario solo una vez
                                if (user == null)
                                {
                                    user = new Users
                                    {
                                        Nomina = dr["Nomina"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Nomina"]),
                                        PathPerfil = dr["PathPerfil"] == DBNull.Value ? "" : dr["PathPerfil"].ToString(),
                                    };
                                }

                                // Añadir los IDs de departamentos sin duplicar
                                string idDepartamento = dr["IDDepartamento"] == DBNull.Value ? "" : dr["IDDepartamento"].ToString();
                                if (!user.IDDepartamentos.Contains(idDepartamento))
                                {
                                    user.IDDepartamentos.Add(idDepartamento);
                                }

                                // Añadir los IDs de vistas permitidas sin duplicar
                                int moduloPerfilID = dr["ModuloPerfilID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ModuloPerfilID"]);
                                if (!user.VistasPermitidas.Contains(moduloPerfilID))
                                {
                                    user.VistasPermitidas.Add(moduloPerfilID);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                user = null; // Retorna null en caso de error
            }

            return user;
        }





        public int resetStepFolio(int Folio, int Tipo, string AutorizedBy, out string Message)
        {
            int result = 0;
            Message = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    SqlCommand command = new SqlCommand("sp_ResetMovsFolio", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    // Agregar parámetros de entrada
                    command.Parameters.AddWithValue("@Folio", Folio);
                    command.Parameters.AddWithValue("@Tipo", Tipo);
                    command.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);

                    command.Parameters.Add("Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("Message", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    command.ExecuteNonQuery();

                    result = Convert.ToInt32(command.Parameters["Result"].Value);
                    Message = command.Parameters["Message"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                result = 0;
            }
            return result;
        }
        public bool updateFolio(int Folio, string AutorizedBy, string FechaPlan, string HrLlegadaPlan, string HrSalidaPlan, string Proveedor, string Proyecto, string Unidad, string Carga, string LineaTrans, string ASN)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {

                        DateTime hrLlegadaPlan = DateTime.ParseExact(HrLlegadaPlan, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime hrSalidaPlan = DateTime.ParseExact(HrSalidaPlan, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        SqlCommand updateCommand = new SqlCommand();
                        updateCommand.Connection = connection;
                        updateCommand.Transaction = transaction;

                        updateCommand.CommandText = "UPDATE PlanTransporte SET FechaPlan = @FechaPlan, HrLlegadaPlan = @HrLlegadaPlan, HrSalidaPlan = @HrSalidaPlan, Proveedor = @Proveedor, Proyecto = @Proyecto, Unidad = @Unidad, Carga = @Carga, LineaTrans = @LineaTrans, ASN = @ASN WHERE Folio = @Folio";
                        updateCommand.Parameters.AddWithValue("@Folio", Folio);
                        updateCommand.Parameters.AddWithValue("@ASN", ASN);
                        updateCommand.Parameters.AddWithValue("@FechaPlan", FechaPlan);
                        updateCommand.Parameters.AddWithValue("@HrLlegadaPlan", hrLlegadaPlan);
                        updateCommand.Parameters.AddWithValue("@HrSalidaPlan", hrSalidaPlan);
                        updateCommand.Parameters.AddWithValue("@Proveedor", Proveedor);
                        updateCommand.Parameters.AddWithValue("@Proyecto", Proyecto);
                        updateCommand.Parameters.AddWithValue("@Unidad", Unidad);
                        updateCommand.Parameters.AddWithValue("@Carga", Carga);
                        updateCommand.Parameters.AddWithValue("@LineaTrans", LineaTrans);
                        updateCommand.ExecuteNonQuery();

                        SqlCommand insertCommand = new SqlCommand();
                        insertCommand.Connection = connection;
                        insertCommand.Transaction = transaction;
                        insertCommand.CommandText = "INSERT INTO Movimientos (Folio, Accion, Usuario) VALUES (@Folio, @Accion, @AutorizedBy)";
                        insertCommand.Parameters.AddWithValue("@Folio", Folio);
                        insertCommand.Parameters.AddWithValue("@Accion", 9);
                        insertCommand.Parameters.AddWithValue("@AutorizedBy", AutorizedBy);
                        insertCommand.ExecuteNonQuery();

                        transaction.Commit();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public List<PlanTransporte> getFilterCustomers(string dateFind)
        {
            List<PlanTransporte> listCustomer = new List<PlanTransporte>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("SELECT DISTINCT(Proveedor) FROM PlanTransporte WHERE FechaPlan = @Fecha", conn);
                    cmd.Parameters.AddWithValue("@Fecha", dateFind);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listCustomer.Add(new PlanTransporte()
                            {
                                Proveedor = dr["Proveedor"] == DBNull.Value ? "" : dr["Proveedor"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                listCustomer = new List<PlanTransporte>();
            }

            return listCustomer;
        }
        public string ConvertSHA256(string text)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding encoding = Encoding.UTF8;
                byte[] result = hash.ComputeHash(encoding.GetBytes(text));
                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        //PLANES ETIQUETAS SEMANAL
        
        
        ////// Plan etiquetas
        public bool InsertarDatosPlanXLSX(DataTable datos)
        {
            bool result = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(cnP))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("InsertarPlanSemanalEtiquetas", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter tableParam = cmd.Parameters.AddWithValue("@PlanSemanal", datos);
                        tableParam.SqlDbType = SqlDbType.Structured;

                        cmd.ExecuteNonQuery();
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                result = false;
            }

            return result;
        }

        ////// Modificar plan
        public List<RegistroPlanBD> consultarPlanes(string proceso, string fecha)
        {
            List<RegistroPlanBD> registros = new List<RegistroPlanBD>();
            if (string.IsNullOrEmpty(proceso) || string.IsNullOrEmpty(fecha))
            {
                return registros;
            }
            DateTime fechaConvertida;
            if (!DateTime.TryParse(fecha, out fechaConvertida))
            {
                return registros;
            }
            try
            {

                using (SqlConnection connection = new SqlConnection(cnP))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT * FROM PlanSemanalEtiquetas WHERE Proceso = @Proceso AND FechaPlan = @Fecha ORDER BY Linea, NoParte", connection))
                    {
                        command.Parameters.Add("@Proceso", SqlDbType.VarChar).Value = proceso;
                        command.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaConvertida;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var registro = new RegistroPlanBD
                                {
                                    Linea = reader["Linea"].ToString(),
                                    TituloPlan = reader["TituloPlan"].ToString(),
                                    NoParte = reader["NoParte"].ToString(),
                                    FechaPlan = Convert.ToDateTime(reader["FechaPlan"]),
                                    CantidadEtiquetas = Convert.ToInt32(reader["CantidadEtiquetas"]),
                                    UsuarioCreacion = reader["UsuarioCreacion"].ToString(),
                                    EtiquetaParcial = Convert.ToInt32(reader["EtiquetaParcial"]),
                                    Proceso = reader["Proceso"].ToString(),
                                    CantEtqSoli = Convert.ToInt32(reader["CantEtqSoli"])
                                };
                                registros.Add(registro);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return registros;
        }

        public bool actualizarMultiplesRegistros(string proceso, string fecha, List<RegistroActualizacion> registros)
        {

            if (string.IsNullOrEmpty(proceso) || string.IsNullOrEmpty(fecha) || registros == null || !registros.Any())
            {
                return false;
            }

            if (registros.Any(r => r.CantidadEtiquetas < 0 || r.EtiquetaParcial < 0))
            {
                throw new ArgumentException("No se permiten valores negativos");
            }

            DateTime fechaConvertida;
            if (!DateTime.TryParse(fecha, out fechaConvertida))
            {
                throw new ArgumentException("Formato de fecha inválido");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(cnP))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var registro in registros)
                            {
                                using (SqlCommand command = new SqlCommand(@"
                                    UPDATE PlanSemanalEtiquetas 
                                    SET 
                                        CantidadEtiquetas = @CantidadEtiquetas, 
                                        EtiquetaParcial = @EtiquetaParcial, 
                                        CantEtqSoli = CASE
                                                        WHEN CantEtqSoli = CantidadEtiquetas THEN @CantidadEtiquetas
                                                        ELSE CantEtqSoli
                                                      END
                                    WHERE Proceso = @Proceso 
                                    AND CONVERT(date, FechaPlan) = @Fecha 
                                    AND NoParte = @NoParte", connection, transaction))
                                {
                                    command.Parameters.Add("@Proceso", SqlDbType.VarChar).Value = proceso;
                                    command.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaConvertida;
                                    command.Parameters.Add("@NoParte", SqlDbType.VarChar).Value = registro.NoParte;
                                    command.Parameters.Add("@CantidadEtiquetas", SqlDbType.Int).Value = registro.CantidadEtiquetas;
                                    command.Parameters.Add("@EtiquetaParcial", SqlDbType.Int).Value = registro.EtiquetaParcial;

                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error en la transacción: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la conexión: {ex.Message}");
                return false;
            }
        }

        //////   Consultar Planes
        public List<string> cargarLineas(string PlanSeleccionado)
        {
            List<string> listaLineas = new List<string>();
            using (SqlConnection connection = new SqlConnection(cnP))
            {
                try
                {
                    connection.Open();
                    string query = "Select DISTINCT(Linea) FROM PlanSemanalEtiquetas WHERE Proceso = @PlanSeleccionado ORDER BY Linea";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PlanSeleccionado", PlanSeleccionado);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                string Linea = reader["Linea"].ToString();
                                listaLineas.Add(Linea);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error en consultar lineas: " + ex.Message);
                    throw new Exception("Error al cargar las líneas: " + ex.Message);
                }
            }
            return listaLineas;
        }

        public List<ResumenPlan> ObtenerResumenCompleto(string plan, string fecha)
        {
            List<ResumenPlan> resumen = new List<ResumenPlan>();

            if (string.IsNullOrEmpty(plan) || string.IsNullOrEmpty(fecha))
            {
                return resumen;
            }

            DateTime fechaConvertida;

            if (!DateTime.TryParse(fecha, out fechaConvertida))
            {
                return resumen;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(cnP))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "Select * FROM Tier2_EntregaEtiquetas WHERE FechaPlan = @Fecha AND Proceso = @Plan ORDER BY Linea, NoParte", connection))
                    {
                        command.Parameters.Add("@Plan", SqlDbType.VarChar).Value = plan;
                        command.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaConvertida;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var registro = new ResumenPlan
                                {
                                    Linea = reader["Linea"].ToString(),
                                    NoParte = reader["NoParte"].ToString(),
                                    CantidadEtiquetas = (reader.IsDBNull(reader.GetOrdinal("CantidadEtiquetas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadEtiquetas"))).ToString(),
                                    EtiquetasParciales = (reader.IsDBNull(reader.GetOrdinal("CantidadParcial")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadParcial"))).ToString(),
                                    UsuarioRecibio = reader["UsuarioRecibio"].ToString(),
                                    FechaEntrega = reader["FechaEntrega"].ToString()
                                };
                                resumen.Add(registro);
                            }
                        }
                    }
                }
                return resumen;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en el Conexion-ObtenerResumenCompleto: " + ex.Message);
                throw;
            }
        }

        public List<ResumenPlan> ObtenerResumenPorLinea(string plan, string fecha, string linea)
        {
            List<ResumenPlan> resumen = new List<ResumenPlan>();

            if (string.IsNullOrEmpty(plan) || string.IsNullOrEmpty(fecha))
            {
                return resumen;
            }

            DateTime fechaConvertida;

            if (!DateTime.TryParse(fecha, out fechaConvertida))
            {
                return resumen;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(cnP))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "Select * FROM Tier2_EntregaEtiquetas WHERE FechaPlan = @Fecha AND Proceso = @Plan AND Linea = @Linea", connection))
                    {
                        command.Parameters.Add("@Plan", SqlDbType.VarChar).Value = plan;
                        command.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaConvertida;
                        command.Parameters.Add("@Linea", SqlDbType.VarChar).Value = linea;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var registro = new ResumenPlan
                                {
                                    NoParte = reader["NoParte"].ToString(),
                                    CantidadEtiquetas = (reader.IsDBNull(reader.GetOrdinal("CantidadEtiquetas")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadEtiquetas"))).ToString(),
                                    EtiquetasParciales = (reader.IsDBNull(reader.GetOrdinal("CantidadParcial")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadParcial"))).ToString(),
                                    UsuarioRecibio = reader["UsuarioRecibio"].ToString(),
                                    FechaEntrega = reader["FechaEntrega"].ToString()
                                };
                                resumen.Add(registro);
                            }
                        }
                    }
                }
                return resumen;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en el Conexion-ObtenerResumenCompleto: " + ex.Message);
                throw;
            }
        }

        //////   Catalogo etiquetas
        public List<Object> consultarEtiquetas()
        {
            var listaEtq = new List<Object>();
            using (SqlConnection connection = new SqlConnection(cnP))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT NoParte, COUNT(NoEtiqueta) AS Total FROM CAT_ETIQS_PLASTICAS GROUP BY NoParte ORDER BY NoParte";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaEtq.Add(new
                                {
                                    NoParte = reader["NoParte"].ToString(),
                                    Total = Convert.ToInt32(reader["Total"])
                                });
                            }
                        }
                    }
                    return listaEtq;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public List<Object> consultarDetalleEtiquetas(string NoParte)
        {
            var listaDetalleEtq = new List<Object>();
            using (SqlConnection connection = new SqlConnection(cnP))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT NoParte, NoEtiqueta, Estado FROM CAT_ETIQS_PLASTICAS WHERE No Parte = @NoParte ORDER BY NoEtiqueta";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@NoParte", SqlDbType.VarChar).Value = NoParte;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaDetalleEtq.Add(new
                                {
                                    NoParte = reader["NoParte"].ToString(),
                                    NoEtiqueta = reader["NoEtiqueta"].ToString(),
                                    Estado = Convert.ToInt32(reader["Estado"])
                                });
                            }
                        }
                    }
                    return listaDetalleEtq;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

    }
}