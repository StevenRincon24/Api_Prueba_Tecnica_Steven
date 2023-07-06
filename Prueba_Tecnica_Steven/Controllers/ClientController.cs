using Microsoft.AspNetCore.Mvc;
using Prueba_Tecnica_Steven.Model;
using Prueba_Tecnica_Steven.Repository;
using System.Net;
using System.Net.Mail;
using System.Web.Http.Cors;

namespace Prueba_Tecnica_Steven.Controllers
{
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : Controller
    {
        private IClientCollection db = new ClientCollection();

        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            return Ok(await db.GetAllClients());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllClientsDetails(string id)
        {
            return Ok(await db.GetClientById(id));
        }

        [HttpGet("nit/{nit}")]
        public async Task<IActionResult> GetClientByNIT(string nit)
        {
            var client = await db.GetClientByNit(nit);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            await db.InsertClient(client);

            return Created("create", true); 
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient([FromBody] Client client, string id)
        {
            client.Id = new MongoDB.Bson.ObjectId(id);
            await db.UpdateClient(client);

            return Created("create", true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(string id)
        {
            await db.DeleteClient(id);

            return NoContent();
        }

        [HttpPost("{nit}/bill")]
        public async Task<IActionResult> AddInvoiceToClient(string nit, [FromBody] Bill bill)
        {
            var client = await db.GetClientByNit(nit);
            if (client == null)
            {
                return NotFound();
            }
            if (client.Bills == null)
            {
                client.Bills = new List<Bill>();
            }

            client.Bills.Add(bill);
            await db.UpdateClient(client);

            return Ok(client);
        }

        [HttpGet("{nit}/bills")]
        public async Task<IActionResult> GetClientBills(string nit)
        {
            var client = await db.GetClientByNit(nit);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client.Bills);
        }

        [HttpPost("{nit}/sendemail")]
        public async Task<IActionResult> UpdateBillStatus(string nit, [FromBody] string billCode)
        {
            string body;
            var client = db.GetClientByNit(nit).Result;
            if (client == null)
            {
                return NotFound();
            }

            var codeBill = client.Bills.FirstOrDefault(b => b.billCode == billCode);
            if (codeBill == null)
            {
                return NotFound("Bill not found");
            }
            else
            {
                if (codeBill.billStatus.Equals("primerrecordatorio"))
                {
                    body = "<body>" +
               "<h1>El estado de su factura ha cambiado</h1>"
               + "<p>Le enviamos este correo con el fin de hacerle saber que el estado de su factura ha cambiado a segundo recordatorio</p>" +
               "</body>";
                    await db.UpdateBillStatus(client.nit, codeBill.billCode, "segundorecordatorio");
                }
                else if (codeBill.billStatus.Equals("") || (codeBill.billStatus.Equals(null)))
                {
                    body = "<body>" +
               "<h1>El estado de su factura ha cambiado</h1>"
               + "<p>Le enviamos este correo con el fin de hacerle saber que el estado de su factura ha cambiado a primer recordatorio</p>" +
               "</body>";
                    await db.UpdateBillStatus(client.nit, codeBill.billCode, "primerrecordatorio");
                }
                else if(codeBill.billStatus.Equals("segundorecordatorio"))
                {
                    body = "<body>" +
               "<h2>El estado de su factura ha cambiado</h2>"
               + "<p>Le enviamos este correo con el fin de hacerle saber que el estado de su factura ha cambiado a desactivada</p>" +
               "</body>";
                    await db.UpdateBillStatus(client.nit, codeBill.billCode, "desactivado");

                }
                else
                {
                    return NotFound("No se puede actualizar el estado");
                }
                
            }

            
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("hovar.rincon@uptc.edu.co", "RealMadrid7");
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("hovar.rincon@uptc.edu.co", "Cambio de estado");
            mailMessage.To.Add(client.correo);
            
            mailMessage.Subject = "Cambio de estado de la factura";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            try
            {
                smtpClient.Send(mailMessage);
                return Ok("Mail Sended");
            }
            catch (SmtpException ex)
            {
                return StatusCode(500, "Failed to send email: " + ex.Message);
            }
        }

    }
}
