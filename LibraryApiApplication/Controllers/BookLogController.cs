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
    public class BookLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookLogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BookLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLog>>> GetBookLog()
        {
            var result = await _context.BookLog.ToListAsync();
            return new JsonResult(new {
                status = "success",
                data = result
            });
        }

        // GET: api/BookLog/5
        [HttpGet("{id}")]
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
        public async Task<IActionResult> EditBookLog(int id, BookLog booklog)
        {
            booklog.Id = id;

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


        // PUT: api/BookLog/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookLog(int id, BookLog booklog)
        {
            if (id != booklog.Id)
            {
                return new JsonResult(new
                {
                    status = "bad request",
                });
            }

            _context.Entry(booklog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookLogExists(id))
                {
                    return new JsonResult(new
                    {
                        status = "not found",
                    });
                }
                else
                {
                    throw;
                }
            }

            return new JsonResult(new
            {
                status = "no content",
            });
        }

        // POST: api/BookLog
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BookLog>> AddBook(BookLog booklog)
        {
            _context.BookLog.Add(booklog);
            await _context.SaveChangesAsync();
            // var result = CreatedAtAction("AddBookLog", new { id = booklog.Id }, booklog);

            return new JsonResult(new {
                status = "success",
                data = booklog
            });
        }

        // DELETE: api/BookLog/5
        [HttpDelete("{id}")]
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
                status = "success"
            });
        }

        private bool BookLogExists(int id)
        {
            return _context.BookLog.Any(e => e.Id == id);
        }
    }
}
