using System.Runtime.InteropServices;

namespace Wallpaper
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam,
            uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        const int SMTO_NORMAL = 0x0000;
        const uint WM_SPAWN_WORKER = 0x052C;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            axWindowsMediaPlayer1.URL = @"D:\2025-01-20 14-53-38.mkv";
            IntPtr progman = FindWindow("Progman", null);

            // �� Progman ������Ϣ����ʹ�䴴��һ�� WorkerW ����
            IntPtr result;
            SendMessageTimeout(progman, WM_SPAWN_WORKER, IntPtr.Zero, IntPtr.Zero,
                SMTO_NORMAL, 1000, out result);

            // 3. ö�����ж������ڣ�Ѱ�Һ��� SHELLDLL_DefView �Ӵ��ڵ��Ǹ����ڣ�
            // ���Ӧ�� WorkerW ���ھ���������Ҫ�ı�������
            IntPtr workerw = IntPtr.Zero;
            EnumWindows((tophandle, topparamhandle) =>
            {
                IntPtr shellViewWin = FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellViewWin != IntPtr.Zero)
                {
                    // �ҵ���������ͼ��Ĵ��ں��ٻ�ȡ������ WorkerW
                    workerw = FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", null);
                }
                return true;
            }, IntPtr.Zero);

            SetParent(Handle, workerw);
            if (GetWindowRect(workerw, out RECT rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                // ���ô���λ�����С
                this.Location = new Point(rect.Left, rect.Top);
                this.Size = new Size(width, height);
            }
            else
            {
                MessageBox.Show("��ȡ�������ڳߴ�ʧ�ܣ�");
            }

            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.Size = new Size(Width, Height);
        }

    }
}
