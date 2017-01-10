using System;

namespace SpartacusMin.Net
{
    public class MailClient
    {
        public MailClient()
        {
        }

        /// <summary>
        /// Envia um único e-mail para um único destinatário.
        /// </summary>
        /// <param name="p_host">Servidor.</param>
        /// <param name="p_port">Porta.</param>
        /// <param name="p_user">Usuário.</param>
        /// <param name="p_password">Senha.</param>
        /// <param name="p_from">Remetente.</param>
        /// <param name="p_to">Destinatário.</param>
        /// <param name="p_subject">Assunto.</param>
        /// <param name="p_body">Corpo.</param>
        public void Send(
            string p_host,
            int p_port,
            string p_user,
            string p_password,
            string p_from,
            string p_to,
            string p_subject,
            string p_body
        )
        {
            try
            {
                System.Net.Mail.SmtpClient v_client;
                System.Net.Mail.MailMessage v_message;

                v_client = new System.Net.Mail.SmtpClient();
                v_client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                v_client.UseDefaultCredentials = false;

                v_client.Host = p_host;
                v_client.Port = p_port;
                v_client.EnableSsl = false;
                v_client.Credentials = new System.Net.NetworkCredential(p_user, p_password);

                v_message = new System.Net.Mail.MailMessage();
                v_message.From = new System.Net.Mail.MailAddress(p_from);
                v_message.To.Add(new System.Net.Mail.MailAddress(p_to));
                v_message.Subject = p_subject;
                v_message.BodyEncoding = System.Text.Encoding.UTF8;
                v_message.IsBodyHtml = false;
                v_message.Body = p_body;

                v_client.Send(v_message);
            }
            catch (System.Net.Mail.SmtpException exc)
            {
                throw new SpartacusMin.Net.Exception(exc);
            }
            catch (System.Exception exc)
            {
                throw new SpartacusMin.Net.Exception(exc);
            }
        }

        /// <summary>
        /// Envia um único e-mail para uma lista de destinatários.
        /// </summary>
        /// <param name="p_host">Servidor.</param>
        /// <param name="p_port">Porta.</param>
        /// <param name="p_user">Usuário.</param>
        /// <param name="p_password">Senha.</param>
        /// <param name="p_from">Remetente.</param>
        /// <param name="p_to">Destinatário.</param>
        /// <param name="p_subject">Assunto.</param>
        /// <param name="p_body">Corpo.</param>
        public void Send(
            string p_host,
            int p_port,
            string p_user,
            string p_password,
            string p_from,
            System.Collections.Generic.List<string> p_to,
            string p_subject,
            string p_body
        )
        {
            try
            {
                System.Net.Mail.SmtpClient v_client;
                System.Net.Mail.MailMessage v_message;

                v_client = new System.Net.Mail.SmtpClient();
                v_client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                v_client.UseDefaultCredentials = false;

                v_client.Host = p_host;
                v_client.Port = p_port;
                v_client.EnableSsl = false;
                v_client.Credentials = new System.Net.NetworkCredential(p_user, p_password);

                v_message = new System.Net.Mail.MailMessage();
                v_message.From = new System.Net.Mail.MailAddress(p_from);
                foreach(string s in p_to)
                    v_message.To.Add(new System.Net.Mail.MailAddress(s));
                v_message.Subject = p_subject;
                v_message.BodyEncoding = System.Text.Encoding.UTF8;
                v_message.IsBodyHtml = false;
                v_message.Body = p_body;

                v_client.Send(v_message);
            }
            catch (System.Net.Mail.SmtpException exc)
            {
                throw new SpartacusMin.Net.Exception(exc);
            }
            catch (System.Exception exc)
            {
                throw new SpartacusMin.Net.Exception(exc);
            }
        }

		/// <summary>
		/// Envia um único e-mail para um único destinatário.
		/// </summary>
		/// <param name="p_host">Servidor.</param>
		/// <param name="p_port">Porta.</param>
		/// <param name="p_user">Usuário.</param>
		/// <param name="p_password">Senha.</param>
		/// <param name="p_from">Remetente.</param>
		/// <param name="p_to">Destinatário.</param>
		/// <param name="p_subject">Assunto.</param>
		/// <param name="p_body">Corpo.</param>
		/// <param name="p_htmlbody">Se o corpo é HTML.</param>
		public void Send(
			string p_host,
			int p_port,
			string p_user,
			string p_password,
			string p_from,
			string p_to,
			string p_subject,
			string p_body,
			bool p_htmlbody
		)
		{
			try
			{
				System.Net.Mail.SmtpClient v_client;
				System.Net.Mail.MailMessage v_message;

				v_client = new System.Net.Mail.SmtpClient();
				v_client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				v_client.UseDefaultCredentials = false;

				v_client.Host = p_host;
				v_client.Port = p_port;
				v_client.EnableSsl = false;
				v_client.Credentials = new System.Net.NetworkCredential(p_user, p_password);

				v_message = new System.Net.Mail.MailMessage();
				v_message.From = new System.Net.Mail.MailAddress(p_from);
				v_message.To.Add(new System.Net.Mail.MailAddress(p_to));
				v_message.Subject = p_subject;
				v_message.BodyEncoding = System.Text.Encoding.UTF8;
				v_message.IsBodyHtml = p_htmlbody;
				v_message.Body = p_body;

				v_client.Send(v_message);
			}
			catch (System.Net.Mail.SmtpException exc)
			{
				throw new SpartacusMin.Net.Exception(exc);
			}
			catch (System.Exception exc)
			{
				throw new SpartacusMin.Net.Exception(exc);
			}
		}

		/// <summary>
		/// Envia um único e-mail para uma lista de destinatários.
		/// </summary>
		/// <param name="p_host">Servidor.</param>
		/// <param name="p_port">Porta.</param>
		/// <param name="p_user">Usuário.</param>
		/// <param name="p_password">Senha.</param>
		/// <param name="p_from">Remetente.</param>
		/// <param name="p_to">Destinatário.</param>
		/// <param name="p_subject">Assunto.</param>
		/// <param name="p_body">Corpo.</param>
		/// <param name="p_htmlbody">Se o corpo é HTML.</param>
		public void Send(
			string p_host,
			int p_port,
			string p_user,
			string p_password,
			string p_from,
			System.Collections.Generic.List<string> p_to,
			string p_subject,
			string p_body,
			bool p_htmlbody
		)
		{
			try
			{
				System.Net.Mail.SmtpClient v_client;
				System.Net.Mail.MailMessage v_message;

				v_client = new System.Net.Mail.SmtpClient();
				v_client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				v_client.UseDefaultCredentials = false;

				v_client.Host = p_host;
				v_client.Port = p_port;
				v_client.EnableSsl = false;
				v_client.Credentials = new System.Net.NetworkCredential(p_user, p_password);

				v_message = new System.Net.Mail.MailMessage();
				v_message.From = new System.Net.Mail.MailAddress(p_from);
				foreach(string s in p_to)
					v_message.To.Add(new System.Net.Mail.MailAddress(s));
				v_message.Subject = p_subject;
				v_message.BodyEncoding = System.Text.Encoding.UTF8;
				v_message.IsBodyHtml = p_htmlbody;
				v_message.Body = p_body;

				v_client.Send(v_message);
			}
			catch (System.Net.Mail.SmtpException exc)
			{
				throw new SpartacusMin.Net.Exception(exc);
			}
			catch (System.Exception exc)
			{
				throw new SpartacusMin.Net.Exception(exc);
			}
		}
    }
}
