using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApiApplication.DbContext;
using LibraryApiApplication.Model;

namespace LibraryApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookLogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BookLog
        [HttpGet]
        [Route("[action]")]
        [Route("api/BookLog/GetBookLog")]
        public async Task<ActionResult<IEnumerable<BookLog>>> GetBookLog()
        {
            var result = await _context.BookLog.ToListAsync();
            return new JsonResult(new {
                status = "success",
                data = result
            });
        }

        // GET: api/BookLog/5
        [HttpGet]
        [Route("[action]")]
        [Route("api/BookLog/GetBookLogById")]
        public async Task<ActionResult<BookLog>> GetBookLogById(int id)
        {
            var booklog = await _context.BookLog.FindAsync(id);

            if (booklog == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }

            return new JsonResult(new {
                status = "success",
                data = booklog
            });;
        }
        [HttpPut("{id}")]
        [Route("[action]")]
        [Route("api/BookLog/EditBookLog")]
        public async Task<IActionResult> EditBookLog(BookLog booklog)
        {
            int id = booklog.Id;
            
            _context.Entry(booklog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookLogExists(id))
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    return new JsonResult(new
                    {
                        status = "Not Found",
                    });
                }
                else
                {
                    throw;
                }
            }

            return new JsonResult(new
            {
                status = "success",
                data = booklog
            });
        }
        
        
        
        // POST: api/BookLog
        [HttpPost]
        [Route("[action]")]
        [Route("api/BookLog/AddBookLog")]
        public async Task<ActionResult<BookLog>> AddBookLog(BookLog booklog)
        {
            var book = await _context.Book.FindAsync(booklog.BookId);
            if (book == null || book.Remains == 0)
            {
                return new JsonResult(new {
                    status = "Book Not Found"
                });
            }
            else
            {
                book.Remains = book.Remains - 1;
                _context.Entry(book).State = EntityState.Modified;
                _context.BookLog.Add(booklog);
                await _context.SaveChangesAsync();
                // var result = CreatedAtAction("AddBookLog", new { id = booklog.Id }, booklog);

                return new JsonResult(new {
                    status = "success",
                    data = booklog,
                });
                
            }
            
        }

        // DELETE: api/BookLog/5
        [HttpDelete]
        [Route("[action]")]
        [Route("api/BookLog/DeleteBookLog")]
        public async Task<ActionResult<BookLog>> DeleteBookLog(int id)
        {
            var booklog = await _context.BookLog.FindAsync(id);
            if (booklog == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }

            _context.BookLog.Remove(booklog);
            await _context.SaveChangesAsync();

            return new JsonResult(new {
                status = "delete success",
                data = booklog
            });
        }
        
        [HttpPost]
        [Route("[action]")]
        [Route("api/BookLog/DeleteBookLog")]
        public async Task<ActionResult<BookLog>> EmailBookLog(BookLog booklog)
        {
            if (booklog == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }
            var book = await _context.Book.FindAsync(booklog.BookId);
            var member = await _context.Member.FindAsync(booklog.MemberId);
            string format = "yyyy.MM.dd HH:mm:ss:ffff";
            string date = booklog.EndTime.ToString(format, DateTimeFormatInfo.InvariantInfo);
            string body = getHtml(member.Name, book.Title, date);
            Email(body, member.Email);

            _context.BookLog.Remove(booklog);
            await _context.SaveChangesAsync();

            return new JsonResult(new {
                status = "delete success",
                data = booklog
            });
        }

        private bool BookLogExists(int id)
        {
            return _context.BookLog.Any(e => e.Id == id);
        }
        
          public static string getHtml(string name, string book, string time)
        {
            try
            {
                string messageBody = "<font>Virtual Library Remainder </font><br><br>";
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart =
                    "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTrStart + "Dear, our Member " + name +
                               " ! This is Virtual Library, we would like to inform you that this is your last day for borrowing book. Please return book tomorrow!" +
                               htmlTdEnd;
                messageBody += htmlTrStart + "Time :" + time + ". " + htmlTdEnd;
                messageBody += htmlTrStart + "Member Name : " + name + htmlTdEnd;
                messageBody += htmlTrStart + "Book : " + book + htmlTdEnd;
                messageBody += htmlTrStart + "If you already return the book please ignore this message! " + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                messageBody = messageBody + htmlTableEnd;
                return messageBody; 
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static void Email(string htmlString, string toEmail) {  
            try {  
                MailMessage message = new MailMessage();  
                SmtpClient smtp = new SmtpClient();  
                message.From = new MailAddress("libraryvirtual77@gmail.com");  
                message.To.Add(new MailAddress(toEmail));  
                message.Subject = "Virtual Book Reminder";  
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlString;  
                smtp.Port = 587;  
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;  
                smtp.UseDefaultCredentials = false;  
                smtp.Credentials = new NetworkCredential("libraryvirtual77@gmail.com", "Mom190465");  
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;  
                smtp.Send(message);  
            } catch (Exception) {}  
        }
        
        
        
        
    }
}
