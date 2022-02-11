using System.Collections.Generic;
using System.Linq;
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
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Book
        [HttpGet]
        [Route("[action]")]
        [Route("api/Book/GetBook")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook()
        {
            var result = await _context.Book.ToListAsync();
            return new JsonResult(new {
                status = "success",
                data = result
            });
        }

        // GET: api/Book/5
        [HttpGet]
        [Route("[action]")]
        [Route("api/Book/GetBookById")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }

            return new JsonResult(new {
                status = "success",
                data = book
            });
        }
        [HttpPut("{id}")]
        [Route("[action]")]
        [Route("api/Employee/EditBook")]
        public async Task<IActionResult> EditBook(Book book)
        {
            int id = book.Id;

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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
                data = book
            });
        }

        

        [HttpPost]
        [Route("[action]")]
        [Route("api/Book/AddBook")]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            _context.Book.Add(book);
            await _context.SaveChangesAsync();
            // var result = CreatedAtAction("AddBook", new { id = book.Id }, book);

            return new JsonResult(new {
                status = "success",
                data = book
            });
        }

        // DELETE: api/Book/5
        [HttpDelete]
        [Route("[action]")]
        [Route("api/Book/DeleteBook")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return new JsonResult(new {
                status = "delete success",
                data = book
            });
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
