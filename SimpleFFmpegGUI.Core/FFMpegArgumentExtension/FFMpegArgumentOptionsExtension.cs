﻿using FFMpegCore;
using FFMpegCore.Enums;
using System;

namespace SimpleFFmpegGUI.FFMpegArgumentExtension
{
    public static class FFMpegArgumentOptionsExtension
    {
        public static FFMpegArgumentOptions WithVideoMBitrate(this FFMpegArgumentOptions opt, double bitrate)
            => opt.WithArgument(new VideoMBitrateArgument(bitrate));

        public static FFMpegArgumentOptions WithVideoMMaxBitrate(this FFMpegArgumentOptions opt, double bitrate, double bufferMagnification = 2)
            => opt.WithArgument(new VideoMMaxBitrateArgument(bitrate, bufferMagnification));

        public static FFMpegArgumentOptions WithArguments(this FFMpegArgumentOptions opt, string arg)
            => opt.WithArgument(new CustomArgument(arg));

        public static FFMpegArgumentOptions To(this FFMpegArgumentOptions opt, TimeSpan to)
            => opt.WithArgument(new ToArgument(to));

        public static FFMpegArgumentOptions WithMapping(this FFMpegArgumentOptions opt, int inputIndex, Channel channel, int? streamIndex)
            => opt.WithArgument(new MapArgument(inputIndex, channel, streamIndex));
    }
}