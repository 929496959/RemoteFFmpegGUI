﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleFFmpegGUI.Dto;
using System.Threading.Tasks;

namespace SimpleFFmpegGUI.WebAPI.Controllers
{
    public class QueueController : FFmpegControllerBase
    {
        public QueueController(ILogger<MediaInfoController> logger,
            IConfiguration config,
        PipeClient pipeClient) : base(logger, config, pipeClient) { }

        [HttpGet]
        [Route("Status")]
        public async Task<StatusDto> GetStatus()
        {
            var status = await pipeClient.InvokeAsync(p => p.GetStatus());
            HideAbsolutePath(status.Task);
            return status;
        }

        [HttpPost]
        [Route("Start")]
        public async Task StartAsync()
        {
            await pipeClient.InvokeAsync(p => p.StartQueue());
        }

        [HttpPost]
        [Route("Pause")]
        public async Task PauseAsync()
        {
            await pipeClient.InvokeAsync(p => p.PauseQueue());
        }

        [HttpPost]
        [Route("Resume")]
        public async Task ResumeAsync()
        {
            await pipeClient.InvokeAsync(p => p.ResumeQueue());
        }

        [HttpPost]
        [Route("Cancel")]
        public async Task CancelAsync()
        {
            await pipeClient.InvokeAsync(p => p.CancelQueue());
        }
    }
}