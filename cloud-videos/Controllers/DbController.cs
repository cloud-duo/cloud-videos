using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using cloud_videos.Helpers;
using cloud_videos.Models;

namespace cloud_videos.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DbController : ApiController
    {
        private const string CreateString = "insert into data values (@video,@audio)";
        private const string GetString = "select * from data";

        [HttpPost]
        [Route("api/db")]
        public async Task<IHttpActionResult> Create(HistoryEntry request)
        {
            using (var connection = new SqlConnection(ConfigHelper.GetDbConnectionString()))
            {
                var command = new SqlCommand(CreateString, connection);
                command.Parameters.AddWithValue("@video", request.VideoUrl);
                command.Parameters.AddWithValue("@audio", request.AudioUrl);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/db")]
        public async Task<IHttpActionResult> GetAll()
        {
            var toReturn = new List<HistoryEntry>();
            using (var connection = new SqlConnection(ConfigHelper.GetDbConnectionString()))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(GetString, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        toReturn.Add(new HistoryEntry(reader[0].ToString(), reader[1].ToString()));
                    }
                }
            }

            return Ok(toReturn);
        }
    }
}