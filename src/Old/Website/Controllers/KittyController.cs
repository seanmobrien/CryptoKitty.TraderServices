using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Services;
using CryptoKitties.Net.Services.KittyService.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;

namespace Website.Controllers
{
    [Route("[controller]/[action]/[id]")]
    public class KittyController
        : Controller
    {


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Genes(long id)
        {
            var actor = Startup.CreateSecuredActorProxy<IKittyService>(ServiceAddress.KittyService, new ActorId(id));
            var data = await actor.GetGenesAsync(false, CancellationToken.None);
            return Json(data);
        }




    }
}
