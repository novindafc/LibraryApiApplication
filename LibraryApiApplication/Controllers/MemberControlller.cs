using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using K4os.Compression.LZ4.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApiApplication.DbContext;
using LibraryApiApplication.Model;

namespace LibraryApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Member
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetBook()
        {
            var result = await _context.Member.ToListAsync();
            return new JsonResult(new {
                status = "success",
                data = result
            });
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMemberById(int id)
        {
            var member = await _context.Member.FindAsync(id);

            if (member == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }

            return new JsonResult(new {
                status = "success",
                data = member
            });;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditMember(int id, Member member)
        {
            member.Id = id;

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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
                data = member
            });
        }


        // PUT: api/Member/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return new JsonResult(new
                {
                    status = "bad request",
                });
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Member
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Member>> AddMember(Member member)
        {
            _context.Member.Add(member);
            await _context.SaveChangesAsync();
            // var result = CreatedAtAction("AddMember", new { id = member.Id }, member);

            return new JsonResult(new {
                status = "success",
                data = member
            });
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Member>> DeleteMember(int id)
        {
            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return new JsonResult(new {
                    status = "Not Found"
                });
            }

            _context.Member.Remove(member);
            await _context.SaveChangesAsync();

            return new JsonResult(new {
                status = "success"
            });
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.Id == id);
        }
    }
}
