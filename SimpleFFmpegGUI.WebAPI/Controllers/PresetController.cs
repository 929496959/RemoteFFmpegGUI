﻿using Furion.FriendlyException;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleFFmpegGUI.Dto;
using SimpleFFmpegGUI.Model;
using SimpleFFmpegGUI.WebAPI.Dto;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleFFmpegGUI.WebAPI.Controllers
{
    public class PresetController : FFmpegControllerBase
    {
        public PresetController(ILogger<MediaInfoController> logger,
            IConfiguration config,
        PipeClient pipeClient) : base(logger, config, pipeClient) { }

        [HttpGet]
        [Route("List")]
        public async Task<List<CodePreset>> GetPresets(TaskType? type)
        {
            if (type.HasValue)
            {
                return (await pipeClient.InvokeAsync(p => p.GetPresets())).Where(p => p.Type == type).ToList();
            }
            return await pipeClient.InvokeAsync(p => p.GetPresets());
        }

        [HttpPost]
        [Route("Add")]
        public Task<int> AddAsync([FromBody] CodePresetDto request)
        {
            CheckNull(request, "请求");
            return pipeClient.InvokeAsync(p => p.AddOrUpdatePreset(request.Name, request.Type, request.Arguments));
        }

        [HttpPost]
        [Route("Delete")]
        public async Task DeleteAsync(int id)
        {
            await pipeClient.InvokeAsync(p => p.DeletePreset(id));
        }
    }
}