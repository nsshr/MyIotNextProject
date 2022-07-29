/*using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Text;
using System.Data;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
//using Microsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("[controller]")]

public class TwinController : ControllerBase

{

    private IConfiguration Configuration;
    private static RegistryManager registryManager;
    private static DeviceClient Client = null;
    private readonly ILogger<TwinController> _logger;

    public TwinController(IConfiguration _configuration, ILogger<TwinController> logger)
    {
        Configuration = _configuration;
        _logger = logger;
        registryManager = RegistryManager.CreateFromConnectionString(this.Configuration.GetConnectionString("NxTIoTHubSAP"));
        Client = DeviceClient.CreateFromConnectionString(this.Configuration.GetConnectionString("
        "), Microsoft.Azure.Devices.Client.TransportType.Mqtt);
    }


    [HttpPut]
    [Route("AddTags")]

    public async Task<IActionResult> AddTagsAsync(string deviceId)
    {
        if(string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.",nameof(deviceId));
        }
        try
        {
            var twin = await registryManager.GetTwinAsync(deviceId);
            if(twin == null){
                throw new Exception("Device twin not found for : " + deviceId);
            }
            var patch = 
                @"{
                    tags:{
                        location:{
                            Region: 'US',
                            plant: 'Boston'
                        }
                    }
            
                }";

                var updateTwin = await registryManager.updateTwinAsync(twin.deviceId,patch,twin.ETag);
                return Ok("Device "+ deviceId + "tag updated");
        }
        catch(Exception ex)
        {
            throw new Exception("couldn't Add Tag for Device" + deviceId + "due to: " + ex);
        }
    }



}


*/
