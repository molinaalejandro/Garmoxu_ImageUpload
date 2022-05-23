using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garmoxu_ImageUpload
{
    public partial class FrmImageUpload : Form
    {
        // Instancia de la conexión a la base de datos.
        private MySqlConnection ConexionBD;

        public FrmImageUpload()
        {
            InitializeComponent();
            RdbLocal.Checked = true;
        }

        #region Cambio de conexión
        private void RdbLocal_CheckedChanged(object sender, EventArgs e)
        {
            if (RdbLocal.Checked)
            {
                RdbRemoto.Checked = false;
                CambiarModoConexionBD();
            }
        }

        private void RdbRemoto_CheckedChanged(object sender, EventArgs e)
        {
            if (RdbRemoto.Checked)
            {
                RdbLocal.Checked = false;
                CambiarModoConexionBD();
            }
        }

        // Abre la conexion a la base de datos.
        private void CambiarModoConexionBD()
        {
            // Conexión local
            string servidorLocal = "localhost"; //Nombre o IP del servidor.
            string bdLocal = "garmoxu"; //Nombre de la base de datos.
            string usuarioLocal = "root"; //Usuario de acceso.
            string passwordLocal = "root"; //Contraseña de usuario de acceso.

            // Conexión remota
            string servidorRemoto = "sql781.main-hosting.eu"; //Nombre o IP del servidor.
            string bdRemoto = "u184120704_garmoxudb"; //Nombre de la base de datos.
            string usuarioRemoto = "u184120704_admindam"; //Usuario de acceso.
            string passwordRemoto = "damAdmin123"; //Contraseña de usuario de acceso.

            if (RdbLocal.Checked)
            {
                ConexionBD = new MySqlConnection(
                    "Database=" + bdLocal + "; Data Source=" + servidorLocal + "; User Id=" + usuarioLocal + "; Password=" + passwordLocal + ";");
            }
            else
            {
                ConexionBD = new MySqlConnection(
                    "Database=" + bdRemoto + "; Data Source=" + servidorRemoto + "; User Id=" + usuarioRemoto + "; Password=" + passwordRemoto + ";");
            }
        }
        #endregion Cambio de conexión

        #region Platos
        private void BtnPlatos_Click(object sender, EventArgs e)
        {
            InsertarImagenesPlatos();
            MessageBox.Show("¡Operación completada con éxito!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion Platos

        #region Categorias
        private void BtnCategorias_Click(object sender, EventArgs e)
        {
            InsertarImagenesCategorias();
            MessageBox.Show("¡Operación completada con éxito!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion Categorias

        #region Usuarios
        private void BtnUsuarios_Click(object sender, EventArgs e)
        {
            InsertarImagenesUsuarios();
            MessageBox.Show("¡Operación completada con éxito!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion Usuarios

        #region Todas las imágenes
        private void BtnTodo_Click(object sender, EventArgs e)
        {
            InsertarImagenesPlatos();
            InsertarImagenesCategorias();
            InsertarImagenesUsuarios();
            MessageBox.Show("¡Operación completada con éxito!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion Todas las imágenes

        #region Inserción de imagenes
        private void InsertarImagenesPlatos()
        {
            InsertarImagenes("PlatosComida", "ImagenPlato", "IdPlatoComida");
        }

        private void InsertarImagenesCategorias()
        {
            InsertarImagenes("Categorias", "ImagenCategoria", "IdCategoria");
        }

        private void InsertarImagenesUsuarios()
        {
            InsertarImagenes("Usuarios", "ImagenUsuario", "NombreUsuario");
        }
        private void InsertarImagenes(string tabla, string columnaDeImagen, string columnaId)
        {
            try
            {
                ConexionBD.Open();

                List<string> claves = new List<string>();
                List<byte[]> imagenBytes = new List<byte[]>();
                RecogerClavesEImagenes(ref claves, ref imagenBytes, tabla);

                for (int i = 0; i < claves.Count; i++)
                {
                    string sql = string.Format("UPDATE {0} SET {1} = @imagen WHERE {2} = '{3}'", tabla, columnaDeImagen, columnaId, claves[i]);
                    MySqlCommand cmd = new MySqlCommand(sql, ConexionBD);
                    cmd.Parameters.Add("@imagen", MySqlDbType.MediumBlob).Value = imagenBytes[i];
                    cmd.ExecuteNonQuery();
                }

            }
            catch (AggregateException ex)
            {
                string mensaje = "No se ha podido establecer la conexión al servidor, revise su conexión a internet y el estado del servidor.";
                if (MessageBox.Show(mensaje, "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error).Equals(DialogResult.Retry)) Application.Restart();
                else Environment.Exit(0);
            }
            catch (Exception ex)
            {
                string mensaje = "Ha ocurrido el siguiente error de tipo '" + ex.GetType().Name + "' : \n" + ex.Message;
                MessageBox.Show(mensaje, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if(ConexionBD.State.Equals(ConnectionState.Open)) ConexionBD.Close();
        }

        private void RecogerClavesEImagenes(ref List<string> claves, ref List<byte[]> imagenBytes, string rutaDirectorio)
        {
            DirectoryInfo dir = new DirectoryInfo(rutaDirectorio);
            if (dir.Exists)
            {
                FileInfo[] imagenes = dir.GetFiles();
                if (imagenes.Length != 0)
                {
                    StreamReader reader = new StreamReader(imagenes[0].FullName);
                    foreach (FileInfo imagen in imagenes)
                    {
                        reader = new StreamReader(imagen.FullName);
                        claves.Add(Path.GetFileNameWithoutExtension(imagen.Name));
                        imagenBytes.Add(File.ReadAllBytes(imagen.FullName));
                    }
                    reader.Close();
                }
            }
        }
        #endregion Inserción de imagenes
    }
}
