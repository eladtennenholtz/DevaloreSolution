using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using DevaloreAssignment.Models.UserResponceFromApi;
using DevaloreAssignment.Models.UserRequest;

namespace DevaloreAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static List<Result> Users = new List<Result>();
        

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<UserApiResponse> FetchUserDataFromApi(string apiUrl)
        {
            var response = await _httpClient.GetStringAsync(apiUrl);
            return JsonConvert.DeserializeObject<UserApiResponse>(response);
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsersData(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
            {
                return BadRequest("Gender parameter is required.");
            }

            string apiUrl = $"https://randomuser.me/api/?results=10&gender={gender}";
            var userApiResponse = await FetchUserDataFromApi(apiUrl);

            return Ok(userApiResponse.Results);
        }

        [HttpGet("get-most-popular-country")]
        public async Task<IActionResult> GetMostPopularCountry()
        {
            var apiUrl = "https://randomuser.me/api/?results=5000";
            var userApiResponse = await FetchUserDataFromApi(apiUrl);

            var countryCounts = userApiResponse.Results
                .GroupBy(u => u.Location.Country)
                .Select(group => new { Country = group.Key, Count = group.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            var mostPopularCountry = countryCounts.FirstOrDefault()?.Country;

            return Ok(mostPopularCountry);
        }

        [HttpGet("get-list-of-mails")]
        public async Task<IActionResult> GetListOfMails()
        {
            string apiUrl = "https://randomuser.me/api/?results=30";
            var userApiResponse = await FetchUserDataFromApi(apiUrl);

            var emailList = userApiResponse.Results.Select(user => user.Email).ToList();
            return Ok(emailList);
        }

        [HttpGet("get-the-oldest-user")]
        public async Task<IActionResult> GetTheOldestUser()
        {
            string apiUrl = "https://randomuser.me/api/?results=100";
            var userApiResponse = await FetchUserDataFromApi(apiUrl);

            var oldestUser = FindOldestUser(userApiResponse.Results);

            if (oldestUser != null)
            {
                return Ok(new { Name = oldestUser.Name.First, Age = oldestUser.Dob.Age });
            }

            return NotFound("No users found");
        }

        private Result FindOldestUser(List<Result> users)
        {
            Result oldestUser = null;
            DateTime oldestDob = DateTime.Now;

            foreach (var user in users)
            {
                DateTime dob = user.Dob.Date;
                if (dob < oldestDob)
                {
                    oldestDob = dob;
                    oldestUser = user;
                }
            }

            return oldestUser;
        }
        [HttpPost("create-new-user")]
        public IActionResult CreateNewUser([FromBody] CreateUserRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            var newUser = new Result
            {
                Id = new Id { Value = request.Id },
                Name = new Name { First = request.Name },
                Email = request.Email,
                Gender = request.Gender,
                Phone = request.Phone,
                Location = new Location { Country = request.Country }
            };
            
            Users.Add(newUser);

            return Ok("User created successfully.");
        }


        [HttpGet("get-new-user")]
        public IActionResult GetNewUser()
        {
            var lastUser = Users.LastOrDefault();

            if (lastUser != null)
            {
                return Ok(lastUser);
            }

            return NotFound("No new user found");
        }



        [HttpPut("update-user-data")]
        public IActionResult UpdateUserData([FromBody] UserRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }
            

            var userToUpdate = Users.FirstOrDefault(u => u.Id.Value == request.Id);

            if (userToUpdate == null)
            {
                return NotFound("User not found.");
            }

            userToUpdate.Name.First = request.Name;
            userToUpdate.Email = request.Email;
            userToUpdate.Gender = request.Gender;
            userToUpdate.Phone = request.Phone;
            userToUpdate.Location.Country = request.Country;

            return Ok("User data updated successfully.");
        }
    }

}


