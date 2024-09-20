using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using web_app_cache_performance.Model;

namespace web_app_cache_performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> getUser()
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "select id, nome, email from usuarios;";
            var usuarios = await connection.QueryAsync<Usuario>(query);


            return Ok(usuarios);
        }

    }
}
