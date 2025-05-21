using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginApi.Models;

namespace LoginApi.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginContext _context;

        public AuthController(LoginContext context)
        {
            _context = context;
        }

        [HttpGet("/")]
        public IActionResult GetHtmlFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "text/html");
        }


        [HttpPost("/")]
        public async Task<IActionResult> PostLogin([FromForm] LoginRequest loginRequest)
        {
            // Assuming you have a login service or logic to check user credentials
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == loginRequest.Name);

            if (user == null || loginRequest.Password != user.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            if (user.IsLocked)
            {
                return Conflict("Your user is locked!");
            }

            user.IsLoggedIn = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.Id,
                user.Name,
                user.IsLoggedIn,
                user.IsLocked
            });
        }

        [HttpGet("register")]
        public IActionResult GetRegisterHtmlFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "register.html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "text/html");
        }


        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromForm] User user)
        {

            Console.WriteLine($"username: {user.Name}");
            Console.WriteLine($"password: {user.Password}");
            // Check if the user already exists in the database
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == user.Name);

            if (existingUser != null)
            {
                // User already exists, return an error
                return Conflict("User with the same name already exists.");
            }

            user.IsAdmin = false;

            // Save the new user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Return a success message or redirect (e.g., to the login page)
            return Redirect("http://localhost:5070/");
        }

        [HttpGet("api/all")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync(); // Get all users from the database

            List<object> filteredUsers = new List<object>();
            foreach (var user in users)
            {
                var filteredUser = new
                {
                    user.Id,
                    user.Name,
                    user.IsLoggedIn,
                    user.IsLocked,
                };

                filteredUsers.Add(filteredUser);
            }

            return Ok(filteredUsers);
        }



        [HttpGet("api/{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.Name,
                user.IsLoggedIn,
                user.IsLocked
            }
            );
        }

        [HttpGet("admin")]
        public IActionResult GetAdminHtmlFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin.html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "text/html");
        }

        [HttpPost("admin")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLogin adminLogin)
        {
            // Check if the admin credentials are correct
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == adminLogin.AdminID && u.IsAdmin && u.Password == adminLogin.Password);


            Console.WriteLine(adminUser);
            Console.WriteLine(adminUser.Id);

            if (adminUser == null)
            {
                return Unauthorized("Invalid admin credentials.");
            }

            // Admin login is successful, return relevant info
            return Ok(new
            {
                adminUser.Id,
                adminUser.Name,
                adminUser.IsLoggedIn
            });
        }

    }
}
/*

    [HttpPost]
        public async Task<IActionResult> PostTodoItem([FromForm] TodoItem todoItem)
        {
            if (todoItem == null)
            {
                return BadRequest("Invalid data.");
            }

            Console.WriteLine(todoItem);


            return Ok();
            // Here you can save the todoItem to the database
        }


        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync(); // Get all users from the database
            return Ok(users); // Return the list of users as JSON
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<TodoItem>> PostUser(User user)
        {
            bool alreadyExists = false;

            Console.WriteLine($"Item ID: {user.Id}");
            Console.WriteLine($"Item name: {user.Name}");
            Console.WriteLine($"item password: {user.Password}");
            Console.WriteLine($"Item locked?: {user.isLocked}");
            Console.WriteLine($"Item logged in?: {user.isLoggedIn}");

            var users = await _context.Users.ToListAsync();
            Console.WriteLine(users.Count);

            foreach (var u in users)
            {
                Console.WriteLine($"db-item ID: {u.Id}");
                Console.WriteLine($"db-item name: {u.Name}");
                Console.WriteLine($"db-item password: {u.Password}");
                Console.WriteLine($"db-item ready?: {u.isLoggedIn}");

                if (user.Name == u.Name)
                {
                    Console.WriteLine("That task is already on your list!");
                    alreadyExists = true;
                    return BadRequest("Task already exists.");
                }
            }

            if (!alreadyExists)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            //    return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> PostLogin([FromBody] LoginRequest loginRequest)
        {
            //take login credentials from somewhere
            var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == loginRequest.Name);

            if (user == null || loginRequest.Password != user.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            var userInfo = new
            {
                user.Id,
                user.Name,
                user.Password,
                user.isLocked,
                user.isLoggedIn
                // Include other necessary fields
            };

            return Ok(userInfo);

            // return redirect to a new page where all user information from database for that user
        }


        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
*/