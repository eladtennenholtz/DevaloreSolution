using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;

namespace NZWalksAPI.Controllers
{
    //https://localhost:portnumber/api/regions
    [Route("api/[controller]")]
    [ApiController]//Will tell this app that this controller is for api use 
    public class RegionsController : ControllerBase
    {
        
        private readonly NZWalksDbContext dbContext;
        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
            
        }

        //GET ALL REGIONS
        //GET: https://localhost:portnumber/api/regions
        [HttpGet]
        public IActionResult GetAll()
        {
            //Get Data From Database - Domain Models
            var regionsDomain = dbContext.Regions.ToList();

            //Map Domain Models to DTO's
            var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl
                });
            }
            

            //Return DTO's
            return Ok(regionsDto);
        }

        //GET SINGLE REGION (Get region be id)
        //GET: https://localhost:portnumber/api/regions/{id}
        
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById(Guid id) {

            //Get Region Domain Model From Database
            var region = dbContext.Regions.Find(id);//Find can only be used with the id. 
            //var region = dbContext.Regions.FirstOrDefault(x =>x.Id==id);
            //FirstOrDefault can be used to check other properties.
            if(region == null)
            {
                return NotFound();
            }

            //Map/Convert Region Model to DTO

            var regionDto = new RegionDto
            {
                Id=region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };

            //Return DTO
            return Ok(regionDto);
        }

        //POST To Create New Region
        //POST: https://localhost:portnumber/api/regions

        [HttpPost]
        public IActionResult Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Map/Convert Dto To Domain Model
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };

            //Use Domain Model to Create Region
            dbContext.Add(regionDomainModel);
            dbContext.SaveChanges();

            //Map Domain Model Back to Dto
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return CreatedAtAction(nameof(GetById),new {id=regionDto.Id},regionDto);




        }

    }
}
