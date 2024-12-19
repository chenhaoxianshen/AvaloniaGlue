using Avalonia.Controls.Shapes; 
using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SampleWebView.Avalonia {
    public class SoxHelper
    {
        private Process soxProcess;

        //private IWavePlayer _waveOut; 
        private SoundPlayer mediaPlayer;

        public string targetPath { get; set; }

        public static bool isStop;

        static Timer stopTimer;
        public SoxHelper()
        {

        }
        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="fileName">文件名，文件默认放根目录下</param>
        /// <param name="path"></param>
        public void PlaySoxVideo(bool isloop = false)
        {
            isStop = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows 相关逻辑
                mediaPlayer = new SoundPlayer();
                // 设置要播放的WAV文件的路径
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;

                var audioFile = System.IO.Path.Combine(rootPath, targetPath);

                // 设置音频文件路径
                mediaPlayer.SoundLocation = audioFile;


                // 播放音频文件
                if (isloop)
                {
                    mediaPlayer.PlayLooping();
                    // 设置定时器，3秒后触发Elapsed事件  
                    stopTimer = new Timer(10000); // 3000毫秒 = 3秒  
                    stopTimer.Elapsed += OnTimedEvent;
                    stopTimer.AutoReset = false; // 定时器只触发一次  
                    stopTimer.Enabled = true; // 启动定时器  
       
                }
                else
                {
                    mediaPlayer.PlayLooping();
                    // 设置定时器，3秒后触发Elapsed事件  
                    stopTimer = new Timer(3000); // 3000毫秒 = 3秒  
                    stopTimer.Elapsed += OnTimedEvent;
                    stopTimer.AutoReset = false; // 定时器只触发一次  
                    stopTimer.Enabled = true; // 启动定时器   
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux 相关逻辑  
                // 获取系统驱动器的根目录，例如 "C:\" 或 "D:\"
                string audioFilePath = targetPath;
                // Sox命令行参数
                string command = $"padsp play {audioFilePath} ";

                //麒麟信安特殊版本默认音频未安装audio play 使用sox 默认命令 
                //if (HFData.Small_version.Contains("kb29.ky3.x86_64")|| HFData.Small_version.Contains("4.19.0-11-linx-security-amd64"))
                //{
                //    kernel.WriteLog("audio play test");
                //    command = $"play {audioFilePath} ";
                //} 
                var loopMax = 3;
                loopMax = isloop ? 10 : 3;
                for (int i = 0; i < loopMax; i++)
                {
                    // 启动Sox进程
                    soxProcess = new Process();
                    soxProcess.StartInfo.FileName = "/bin/bash"; // 确保bash shell存在于此路径
                    soxProcess.StartInfo.Arguments = $"-c \"{command}\"";
                    soxProcess.StartInfo.UseShellExecute = false;
                    soxProcess.StartInfo.RedirectStandardOutput = true;
                    soxProcess.StartInfo.RedirectStandardError = true;
                    soxProcess.StartInfo.CreateNoWindow = true;

                    try
                    {
                        if (soxProcess.Start() && !isStop)
                        {
                            // 读取Sox输出 播放完成自动结束
                            string output = soxProcess.StandardOutput.ReadToEnd();
                            Debug.WriteLine(output);

                            // 等待进程结束 
                            soxProcess.WaitForExit();
                            if (isloop)
                            {
                                soxProcess.Kill();
                            } 
                            Console.WriteLine("播放完成");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="path"></param>
        public void StopSoxVideo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows 相关逻辑
                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop();
                }

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux 相关逻辑
                isStop = true;
                if (soxProcess != null)
                {
                    soxProcess.Kill();
                }
            }
        }
        private  void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // 停止音频播放  
            mediaPlayer.Stop(); 
        }
    }
}
