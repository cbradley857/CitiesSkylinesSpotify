using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ColossalFramework.Plugins;
using Message = System.Windows.Forms.Message;
using Timer = System.Threading.Timer;
using System.IO;

namespace CitiesSkylinesSpotify
{
    public class SpotifyAPI
    {
        /*
            This uses code from https://github.com/ranveer5289/SpotifyNotifier-Windows and https://github.com/mscoolnerd/SpotifyLib
        */

        public event EventHandler SpotifyExited;
        public event EventHandler TrackChanged;
        public event EventHandler SpotifyOpened;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        

        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("user32.dll", SetLastError = true)]
        //static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32")]
        private static extern bool GetMessage(ref Message lpMsg, IntPtr handle, uint mMsgFilterInMain, uint mMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        const int KeyMessage = 0x319;
        const int ControlKey = 0x11;

        const long PlaypauseKey = 0xE0000L;
        const long NexttrackKey = 0xB0000L;
        const long PreviousKey = 0xC0000L;

        private Process _SpotifyProcess;
        private Thread _BackgroundChangeTracker;
        private WinEventDelegate _ProcDelegate;
        private Timer _ProcessWatcher;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        public SpotifyAPI()
        {
            AttachToProcess();
            JoinBackgroundProcess();
        }

        public void JoinBackgroundProcess()
        {
            if (_BackgroundChangeTracker != null && _BackgroundChangeTracker.IsAlive)
                return;

            _BackgroundChangeTracker = new Thread(BackgroundChangeTrackerWork) { IsBackground = true };
            _BackgroundChangeTracker.Start();
        }

        private void BackgroundChangeTrackerWork(object obj)
        {
            try
            {
                if (_SpotifyProcess == null)
                    return;

                if (_SpotifyProcess != null)
                {
                    var hwndSpotify = _SpotifyProcess.MainWindowHandle;
                    var pidSpotify = _SpotifyProcess.Id;

                    var hWinEventHook = SetWinEventHook(0x0800c, 0x800c, IntPtr.Zero, _ProcDelegate, Convert.ToUInt32(pidSpotify), 0, 0);
                    var msg = new Message();
                    while (GetMessage(ref msg, hwndSpotify, 0, 0))
                        UnhookWinEvent(hWinEventHook);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception) { }
        }

        private string output = "";
        private bool isOpen = false;
        private int id;
        private IntPtr windowId;

        private void AttachToProcess()
        {
            _SpotifyProcess = null;
            
            GetProcessId();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Output: " + output);

            string[] split = output.Split(':');
            isOpen = bool.Parse(split[0]);

            if (isOpen)
            {
                id = int.Parse(split[1]);
                windowId = new IntPtr(int.Parse(split[2]));
            }

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Stored. Values - WindowID: " + windowId + " & id: " + id);

            //id = Process.GetProcessesByName("spotify").Where(c => c.ProcessName != string.Empty).First().Id;

            //Console.WriteLine(id);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "ID is set? " + id + " | Value: " + id);

            if (isOpen)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Attaching to process...");
                _SpotifyProcess = Process.GetProcessById(id);
                //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Main Handle: " + _SpotifyProcess.MainWindowHandle);
            }

            if(_SpotifyProcess != null)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Attaching successfull!");
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Setting up...");
                _SpotifyProcess.EnableRaisingEvents = true;
                _SpotifyProcess.Exited += (o, e) =>
                {
                    _SpotifyProcess = null;
                    _BackgroundChangeTracker.Abort();
                    _BackgroundChangeTracker = null;
                    WaitForSpotify();
                    OnSpotifyExited();
                };
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Process Name: " + _SpotifyProcess.ProcessName);
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Window Name: " + (_SpotifyProcess.MainWindowTitle ?? "Not Set"));
            }
        }

        private void GetProcessId()
        {
            string targetExe = Directory.GetCurrentDirectory() + @"\Files\Mods\SpotiMod\IdGetter.exe";

            var proc = new Process();
            proc.StartInfo.FileName = targetExe;

            // set up output redirection
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.CreateNoWindow = true;
            // see below for output handler
            proc.ErrorDataReceived += proc_DataReceived;
            proc.OutputDataReceived += proc_DataReceived;

            proc.Start();

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit();
        }

        private void proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            output += e.Data;
        }

        private void WaitForSpotify()
        {
            _ProcessWatcher = new Timer(WaitForSpotifyCallback, null, 1000, 1000);
        }

        private void WaitForSpotifyCallback(object args)
        {
            AttachToProcess();
            if (_SpotifyProcess != null)
            {

                //Start track change tracker
                JoinBackgroundProcess();

                //Kill timer
                if (_ProcessWatcher != null)
                {
                    _ProcessWatcher.Dispose();
                    _ProcessWatcher = null;
                }

                //Notify UI that Spotify is available
                OnSpotifyOpenend();
            }
        }

        protected virtual void OnSpotifyOpenend()
        {
            var handler = SpotifyOpened;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnSpotifyExited()
        {
            var handler = SpotifyExited;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public bool PausePlay()
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "calling with values:\nHandle: " + windowId + ", KeyMessage: " + KeyMessage
                + "Key: " + PlaypauseKey);

            if (_SpotifyProcess != null)
            {
                bool postResult = PostMessage(windowId, KeyMessage, IntPtr.Zero, new IntPtr(PlaypauseKey));
                //bool postResult = PostMessage(new HandleRef(_SpotifyProcess, _SpotifyProcess.MainWindowHandle), KeyMessage, IntPtr.Zero, new IntPtr(PlaypauseKey));
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "PostResult: " + postResult);
                return true;
            }

            return false;
        }

        public bool NextTrack()
        {
            bool postResult = PostMessage(windowId, KeyMessage, IntPtr.Zero, new IntPtr(NexttrackKey));

            return postResult;
        }

        public bool PreviousTrack()
        {
            bool postResult = PostMessage(windowId, KeyMessage, IntPtr.Zero, new IntPtr(PreviousKey));

            return postResult;
        }
    }
}
