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
        [Route("[action]")]
        [Route("api/Member/GetMember")]
        public async Task<ActionResult<IEnumerable<Member>>> GetMember()
        {
            var result = await _context.Member.ToListAsync();
            return new JsonResult(new {
                status = "success",
                data = result
            });
        }

        // GET: api/Member/5
        [HttpGet]
        [Route("[action]")]
        [Route("api/Member/GetMemberById")]
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
        [Route("[action]")]
        [Route("api/Member/EditMember")]
        public async Task<IActionResult> EditMember(Member member)
        {
            int id = member.Id;

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

        
        // POST: api/Member
        [HttpPost]
        [Route("[action]")]
        [Route("api/Member/AddMember")]
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
        [HttpDelete]
        [Route("[action]")]
        [Route("api/Member/DeleteMember")]
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
                status = "delete success",
                data = member
            });
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.Id == id);
        }
    }
}
