﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Wox.Plugin.SystemPlugins
{
    public class Sys : BaseSystemPlugin
    {
        List<Result> availableResults = new List<Result>();

        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        [DllImport("user32")]
        public static extern void LockWorkStation();

        protected override List<Result> QueryInternal(Query query)
        {
            if (query.RawQuery.EndsWith(" ") || query.RawQuery.Length <= 1) return new List<Result>();

            List<Result> results = new List<Result>();

            foreach (Result availableResult in availableResults)
            {
                if (availableResult.Title.ToLower().StartsWith(query.RawQuery.ToLower()))
                {
                    results.Add(availableResult);
                }
            }
            return results;
        }

        protected override void InitInternal(PluginInitContext context)
        {
            availableResults.Add(new Result
            {
                Title = "Shutdown",
                SubTitle = "Shutdown Computer",
                Score = 100,
                IcoPath = "Images\\exit.png",
                Action = (c) =>
                {
                    Process.Start("shutdown", "/s /t 0");
                    return true;
                }
            });
            availableResults.Add(new Result
            {
                Title = "Log off",
                SubTitle = "Log off current user",
                Score = 20,
                IcoPath = "Images\\logoff.png",
                Action = (c) => ExitWindowsEx(EWX_LOGOFF, 0)
            });
            availableResults.Add(new Result
            {
                Title = "Lock",
                SubTitle = "Lock this computer",
                Score = 20,
                IcoPath = "Images\\lock.png",
                Action = (c) =>
                {
                    LockWorkStation();
                    return true;
                }
            });
            availableResults.Add(new Result
            {
                Title = "Exit",
                SubTitle = "Close this app",
                Score = 110,
                IcoPath = "Images\\app.png",
                Action = (c) =>
                {
                    context.API.CloseApp();
                    return true;
                }
            });
            availableResults.Add(new Result
            {
                Title = "Restart Wox",
                SubTitle = "Restart Wox",
                Score = 110,
                IcoPath = "Images\\restart.png",
                Action = (c) =>
                {
                    ProcessStartInfo Info = new ProcessStartInfo();
                    Info.Arguments = "/C ping 127.0.0.1 -n 1 && \"" + Application.ExecutablePath + "\"";
                    Info.WindowStyle = ProcessWindowStyle.Hidden;
                    Info.CreateNoWindow = true;
                    Info.FileName = "cmd.exe";
                    Process.Start(Info);
                    context.API.CloseApp();
                    return true;
                }
            });
            availableResults.Add(new Result
            {
                Title = "Setting",
                SubTitle = "Tweak this app",
                Score = 40,
                IcoPath = "Images\\app.png",
                Action = (c) =>
                {
                    context.API.OpenSettingDialog();
                    return true;
                }
            });
        }


        public override string ID
        {
            get { return "CEA08895D2544B019B2E9C5009600DF4"; }
        }

        public override string Name
        {
            get { return "System Commands"; }
        }

        public override string IcoPath
        {
            get { return @"Images\lock.png"; }
        }

        public override string Description
        {
            get { return "Provide System related commands. e.g. shutdown,lock,setting etc."; }
        }
    }
}
