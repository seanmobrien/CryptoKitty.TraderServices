using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Fabric.KittyCatService.Interfaces;
using CryptoKitties.Net.Fabric.KittyContractDataService.Interfaces;
using CryptoKitties.Net.Fabric.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoKitties.Net.Website.Controllers.KittyCat
{
    [Route("api/kitty-cat/[controller]")]
    public class ContractController : Controller
    {
        public ContractController(IKittyContractDataService dataService)
        {
            _dataService = dataService;
        }

        private readonly IKittyContractDataService _dataService;

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<KittyCatContractDataModel> Get(int id)
        {            
            return await _dataService.GetContractDataAsync(id, false, CancellationToken.None);
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
