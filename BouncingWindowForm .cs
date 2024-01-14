using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace HertaX3
{
    public partial class BouncingWindowForm : Form
    {
        private const int windowWidth = 200;
        private const int windowHeight = 300;

        private Point windowPosition;
        private Point windowVelocity;

        private int seed;
        private readonly Random random;
        private WindowsUtils windowsUtils = new WindowsUtils();

        public UpdateAction[] anims;

        public Timer moveTimer = new Timer(20);
        public Timer controlTimer = new Timer(10);
        public Timer animsTimer = new Timer(30);

        private Bitmap[] resources;
        private int currentBackground;

        public new Image BackgroundImage
        {
            get => base.BackgroundImage;
            set
            {
                base.BackgroundImage = value;
                if (base.IsHandleCreated)
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        base.BackgroundImage = value;
                    });
                }
            }
        }

        public BouncingWindowForm(Point startPos, int seed, UpdateAction[] anims, Bitmap[] resources)
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            random = new Random(seed);

            InitializeWindow(startPos);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.seed = seed;
            this.anims = anims;

            controlTimer.Enabled = false;
            moveTimer.Enabled = false;
            animsTimer.Enabled = false;
            controlTimer.Elapsed += animTimer_Tick;
            moveTimer.Elapsed += UpdateFrame;
            animsTimer.Elapsed += UpdateUI;


            this.resources = resources;
        }

        private void InitializeWindow(Point startPos)
        {
            Location = startPos;
            windowPosition = startPos;
            windowVelocity = new Point(random.Next(-10, 10), random.Next(-10, 10));
        }

        //移动弹跳功能
        public void StartMove()
        {
            moveTimer.Enabled = true;
            moveTimer.Start();
        }
        public void StartAnim()
        {
            animsTimer.Enabled = true;
            animsTimer.Start();
        }
        private void MoveWindow()
        {
            windowPosition.X += windowVelocity.X;
            windowPosition.Y += windowVelocity.Y;

            // 碰撞检测和反弹逻辑
            if (windowPosition.X < 0 || windowPosition.X > Screen.PrimaryScreen.Bounds.Width - windowWidth)
            {
                windowVelocity.X = -windowVelocity.X;
                //碰到边边，桌面消失
                if(Program.MS < Program.AnimMaxMS) windowsUtils.SwitchDesktopIconsVisible();
            }

            if (windowPosition.Y < 0 || windowPosition.Y > Screen.PrimaryScreen.Bounds.Height - windowHeight)
            {
                windowVelocity.Y = -windowVelocity.Y;
                if (Program.MS < Program.AnimMaxMS) windowsUtils.SwitchDesktopIconsVisible();
            }

            // 移动窗口
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => { Location = windowPosition; }));

        }
        private void UpdateFrame(object? sender, System.Timers.ElapsedEventArgs e)
        {
            MoveWindow();
            Invalidate();
        }
        private void UpdateUI(object? sender, System.Timers.ElapsedEventArgs e)
        {
            currentBackground++;
            int index = currentBackground % resources.Length;
            BackgroundImage = resources[index];
        }
        private void BouncingWindowForm_Load(object sender, EventArgs e)
        {
            controlTimer.Enabled = true;
            controlTimer.Start();

            BackgroundImageLayout = ImageLayout.Zoom;
            BackgroundImage = resources[0];
        }
        private void animTimer_Tick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (UpdateAction action in anims)
            {
                if (action == null) continue;
                if (action.isInvoked) continue;
                if (Program.MS > action.ms)
                {
                    action.Invoke();
                }
            }
        }
    }
}