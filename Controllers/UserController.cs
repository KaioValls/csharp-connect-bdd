using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using web_app_cache_performance.Model;

namespace web_app_cache_performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static ConnectionMultiplexer redis;

        [HttpGet]
        public async Task<IActionResult> getUser()
        {
            string key = "getUsuario";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyExpireAsync(key,TimeSpan.FromSeconds(10));
            string user = await db.StringGetAsync(key);

            if (!string.IsNullOrEmpty(user)) { 
                return Ok(user);
            }


            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "select id, nome, email from usuarios;";
            var usuarios = await connection.QueryAsync<Usuario>(query);
            string usuariosJson = JsonConvert.SerializeObject(usuarios);

            await db.StringSetAsync(key, usuariosJson);

            return Ok(usuarios);
        }


        [HttpPost]
        public async Task<IActionResult> Post(Usuario usuario)
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string key = "getUsuario";
            
            string sql = @"INSERT INTO usuarios (nome, email) VALUES (@nome, @email)";
            await connection.ExecuteAsync(sql, usuario);


            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Created();

        }

        [HttpPut]
        public async Task<IActionResult> Put(int id)
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string key = "getUsuario";

            string sql = @"update usuarios
                            set nome = @nome,
                            email = @email
                            where id = @id;";
            await connection.ExecuteAsync(sql, id);

            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }

        [HttpDelete]
        [HttpPut]
        public async Task<IActionResult> Delete(int id)
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string key = "getUsuario";

            string sql = @"delete from usuarios where id = @id;";
            await connection.ExecuteAsync(sql, id);

            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }
    }
}
