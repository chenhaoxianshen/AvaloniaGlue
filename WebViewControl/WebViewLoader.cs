using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xilium.CefGlue;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace WebViewControl {

    internal static class WebViewLoader {

        private static string[] CustomSchemes { get; } = new[] {
            ResourceUrl.LocalScheme,
            ResourceUrl.EmbeddedScheme,
            ResourceUrl.CustomScheme,
            Uri.UriSchemeHttp,
            Uri.UriSchemeHttps
        };

        private static string runTimepath;
        private static GlobalSettings globalSettings;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Initialize(GlobalSettings settings) {
            if (CefRuntimeLoader.IsLoaded) {
                return;
            }

            globalSettings = settings;
            IsUseWeb();
            string cefPath = GetProjectPath(false);
            var cefSettings = new CefSettings {
                LogSeverity = string.IsNullOrWhiteSpace(settings.LogFile) ? CefLogSeverity.Disable : (settings.EnableErrorLogOnly ? CefLogSeverity.Error : CefLogSeverity.Verbose),
                LogFile = settings.LogFile,
                LocalesDirPath = System.IO.Path.Combine(cefPath, "locales"),
                ResourcesDirPath = cefPath,
                NoSandbox = true,
                UncaughtExceptionStackSize = 100, // enable stack capture
                CachePath = settings.CachePath, // enable cache for external resources to speedup loading
                WindowlessRenderingEnabled = settings.OsrEnabled,
                RemoteDebuggingPort = settings.GetRemoteDebuggingPort(),
                UserAgent = settings.UserAgent
            };

            var customSchemes = CustomSchemes.Select(s => new CustomScheme() {
                SchemeName = s,
                SchemeHandlerFactory = new SchemeHandlerFactory()
            }).ToArray();

             
            settings.AddCommandLineSwitch("password-store", "basic");
            settings.AddCommandLineSwitch("disable-gpu", "1");
            settings.AddCommandLineSwitch("disable-gpu-compositing", "1");
            settings.AddCommandLineSwitch("enable-begin-frame-scheduling", "1");
            settings.AddCommandLineSwitch("disable-smooth-scrolling", "1");

            settings.AddCommandLineSwitch("ignore-certificate-errors","1"); 

            CefRuntimeLoader.Initialize(settings: cefSettings, flags: settings.CommandLineSwitches.ToArray(), customSchemes: customSchemes);

            AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(); };
        }


        public static void IsUseWeb() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                //HFData.E_SYSTEM_Ver = E_SYSTEM_Ver.E_WINDOWS;
                runTimepath = "winCefNet";
                //HFData.IsShowWeb = "1";//windows默认支持
            } else {
                //GetCurrentSystem();
                runTimepath = "cefnet";
            }
        }

        private static string GetProjectPath(bool isMacOS) {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, runTimepath, "Release", isMacOS ? System.IO.Path.Combine("cefclient.app", "Contents", "Frameworks", "Chromium Embedded Framework.framework") : "");
        }

        /// <summary>
        /// Release all resources and shutdown web view
        /// </summary>
        [DebuggerNonUserCode]
        public static void Cleanup() {
            CefRuntime.Shutdown(); // must shutdown cef to free cache files (so that cleanup is able to delete files)

            if (globalSettings.PersistCache) {
                return;
            }

            try {
                var dirInfo = new DirectoryInfo(globalSettings.CachePath);
                if (dirInfo.Exists) {
                    dirInfo.Delete(true);
                }
            } catch (UnauthorizedAccessException) {
                // ignore
            } catch (IOException) {
                // ignore
            }
        }

    }
}
