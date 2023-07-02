using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ProyectoDSWI.Models
{
    public class Carrito1Item
    {
        private Producto _producto;

        public Producto Producto
        {
            get { return _producto; }
            set { _producto = value; }
        }

        private int _cantidad;
        public int Cantidad
        {
            get { return _cantidad; }
            set { _cantidad = value; }
        }

        public Carrito1Item()
        { }

        public Carrito1Item(Producto producto, int cantidad)
        {
            this._producto = producto;
            this._cantidad = cantidad;
        }

        public int sendMailSMTP(string email, string asunto, string body, string correoCopia = "")
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From =  new MailAddress("csar251214@gmail.com");
                mail.To.Add(email);
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;
                if (!String.IsNullOrEmpty(correoCopia)) mail.CC.Add(correoCopia);
                try
                {
                    int contador = 1;
                    string copianext = ConfigurationManager.AppSettings["MAIL_COPIA" + contador];
                    do
                    {
                        if (!String.IsNullOrEmpty(copianext))
                        {
                            mail.CC.Add(copianext);
                            contador++;
                            copianext = ConfigurationManager.AppSettings["MAIL_COPIA" + contador];
                        }
                    } while (!String.IsNullOrEmpty(copianext));

                }
                catch (Exception)
                {
                }

                mail.Priority = MailPriority.Normal;

                //SmtpClient client = new SmtpClient();
                //

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = "csar251214@gmail.com",
                    Password = "ljamauociyuxuzdo"
                };

                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mail);
                //


                //client.Send(mail);

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}