using NAudio.Wave;

namespace HertaX3
{
    internal static class Program
    {
        //程序开始到目前的毫秒数
        static double ms;
        static int msOffset;
        public static double MS => ms;
        public const int AnimMaxMS = 28000;
        public const int WinMaxMS = 29000;
        public const int AppMaxMS = 29500;

        static Action musicPlayer = new Action(PlayMusicAndQuit);
        static Action ticker = new Action(Tick);
        static Action othersUpdater = new Action(UpdateOthers);
        static Action uiUpdater = new Action(UpdateUI);
        static Action wallpaperChanger = new Action(ChangeWallpaper);

        public static UpdateAction[] actions = new UpdateAction[10];
        public static UpdateAction[] ht1Anims = new UpdateAction[10];
        public static UpdateAction[] ht2Anims = new UpdateAction[10];
        public static UpdateAction[] ht3Anims = new UpdateAction[10];
        static CancellationTokenSource cancellationToken = new CancellationTokenSource();

        //动画物体
        static BouncingWindowForm ht1;
        static BouncingWindowForm ht2;
        static BouncingWindowForm ht3;
        static Bitmap[] ht1Resources;
        static Bitmap[] ht2Resources;
        static Bitmap[] ht3Resources;

        //拓展
        static WindowsUtils windowsUtils;
        static Rectangle screen;
        static int wallpaperIndex;
        static string oriWallpaperPath;//程序运行前壁纸的路径

        [STAThread]
        static void Main()
        {
            screen = Screen.PrimaryScreen.Bounds;
            windowsUtils = new WindowsUtils();
            ApplicationConfiguration.Initialize();

            //获取offset
            try
            {
                msOffset = int.Parse(File.ReadAllText("Offset.txt"));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            //获取初始值
            oriWallpaperPath = windowsUtils.GetWallPaperPath();

            //最小化所有窗口
            windowsUtils.MinimizeAllWindows();

            //获取资源
            List<Bitmap> resources = new List<Bitmap>();
            for (int i = 0; i < 8; i++)
            {
                string name = $"HT1_{i}";
                Bitmap res = (Bitmap)Resources.ResourceManager.GetObject(name);
                resources.Add(res);
            }
            ht1Resources = resources.ToArray();
            resources.Clear();
            for (int i = 0; i < 8; i++)
            {
                string name = $"HT2_{i}";
                Bitmap res = (Bitmap)Resources.ResourceManager.GetObject(name);
                resources.Add(res);
            }
            ht2Resources = resources.ToArray();
            resources.Clear();
            for (int i = 0; i < 8; i++)
            {
                string name = $"HT3_{i}";
                Bitmap res = (Bitmap)Resources.ResourceManager.GetObject(name);
                resources.Add(res);
            }
            ht3Resources = resources.ToArray();
            resources.Clear();

            //添加动画
            actions[1] = new UpdateAction(2600, () =>
            {
                ht1 = new BouncingWindowForm(new Point(screen.Width / 2 - 100, screen.Height / 2 - 150), DateTime.Now.Millisecond, ht1Anims, ht1Resources);

                //黑塔1动画
                ht1Anims[0] = new UpdateAction(8200, ht1.StartMove);
                ht1Anims[1] = new UpdateAction(8210, ht1.StartAnim);
                ht1Anims[2] = new UpdateAction(WinMaxMS, ht1.Hide);

                ht1.ShowDialog();
            });
            actions[2] = new UpdateAction(3300, () =>
            {
                ht2 = new BouncingWindowForm(new Point(screen.Width / 2 - 300, screen.Height / 2 - 150), DateTime.Now.Millisecond, ht2Anims, ht2Resources);

                //黑塔2动画
                ht2Anims[0] = new UpdateAction(7000, ht2.StartMove);
                ht2Anims[1] = new UpdateAction(7010, ht2.StartAnim);
                ht2Anims[2] = new UpdateAction(WinMaxMS, ht2.Hide);

                ht2.ShowDialog();
            });
            actions[3] = new UpdateAction(3800, () =>
            {
                ht3 = new BouncingWindowForm(new Point(screen.Width / 2 + 100, screen.Height / 2 - 150), DateTime.Now.Millisecond, ht3Anims, ht3Resources);

                //黑塔3动画
                ht3Anims[0] = new UpdateAction(9200, ht3.StartMove);
                ht3Anims[1] = new UpdateAction(9210, ht3.StartAnim);
                ht3Anims[2] = new UpdateAction(WinMaxMS, ht3.Hide);

                ht3.ShowDialog();
            });


            Task.Run(ticker, cancellationToken.Token);
            Task.Run(othersUpdater, cancellationToken.Token);
            Task.Run(uiUpdater, cancellationToken.Token);
            Task.Run(wallpaperChanger, cancellationToken.Token);
            Task.Run(musicPlayer);

            Application.Run(new MainForm());
        }

        static void PlayMusicAndQuit()
        {
            //播放音乐
            using (var ms = new MemoryStream(Resources.Herta))
            using (var rdr = new Mp3FileReader(ms))
            using (var wavStream = WaveFormatConversionStream.CreatePcmStream(rdr))
            using (var baStream = new BlockAlignReductionStream(wavStream))
            using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
            {
                waveOut.Init(baStream);
                waveOut.Play();
                while (MS < WinMaxMS)
                {
                    Thread.Sleep(100);
                }
            }

            cancellationToken.Cancel();//取消所有进程

            //自动退出
            //显示任务栏
            windowsUtils.ShowTaskBar();
            windowsUtils.ShowDesktop();
            windowsUtils.ShowDesktopIcons();

            windowsUtils.SetWallpaperByPath(oriWallpaperPath);

            Application.Exit();
        }
        static void Tick()
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                DateTime current = DateTime.Now;
                ms = (current - start).TotalMilliseconds - msOffset;
            }
        }
        static void UpdateOthers()
        {
            while (true)
            {
                Console.WriteLine(MS);
                if (ms < AnimMaxMS)
                {
                    //鼠标靠近任务栏，任务栏消失
                    if (screen.Height - Cursor.Position.Y < 60 && windowsUtils.IsTaskBarVisible())
                    {
                        windowsUtils.HideTaskBar();
                    }
                    else if (screen.Height - Cursor.Position.Y >= 60 && !windowsUtils.IsTaskBarVisible())
                    {
                        windowsUtils.ShowTaskBar();
                    }
                }
            }
        }
        static void UpdateUI()
        {
            while (true)
            {
                foreach (UpdateAction action in actions)
                {
                    if (action == null) continue;
                    if (action.isInvoked) continue;
                    if (ms > action.ms) action.Invoke();
                }
            }
        }
        static void ChangeWallpaper()
        {
            while (true)
            {
                if (ms < AnimMaxMS && ms > 11000)
                {
                    //壁纸替换
                    wallpaperIndex++;
                    wallpaperIndex = wallpaperIndex % 5;
                    windowsUtils.SetWallpaperByPath(Path.GetFullPath($"Wallpapers/WP_{wallpaperIndex}.png"));
                    Thread.Sleep(100);
                }
            }
        }

    }

    public class UpdateAction
    {
        public Action? action;
        public double ms;
        public bool isInvoked;

        public UpdateAction(double ms, Action ac)
        {
            this.ms = ms;
            this.isInvoked = false;
            this.action = ac;
        }
        public void Invoke()
        {
            isInvoked = true;
            if (action != null) Task.Run(action);
        }
    }

}