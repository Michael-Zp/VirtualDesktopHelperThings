using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirtualDesktopHelperStuff
{
    public partial class Form1 : Form
    {
        KeyboardHook hook = new KeyboardHook();
        IntPtr windowToBeMoved;
        private int _MouseIndex = 0;
        private bool _FirstStart = true;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        public Form1()
        {
            InitializeComponent();

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(VirtualDesktopHelperStuff.ModifierKeys.Shift | VirtualDesktopHelperStuff.ModifierKeys.Alt, Keys.F1);

            listBox1.Location = new System.Drawing.Point(0, 0);
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            listBox1.Items.Clear();

            for (int i = 0; i < Desktop.Count; ++i)
            {
                listBox1.Items.Add(Desktop.DesktopNameFromIndex(i));
            }

            listBox1.SetSelected(0, true);
            _MouseIndex = 0;

            listBox1.Size = new System.Drawing.Size(listBox1.Width, listBox1.ItemHeight * (Desktop.Count + 1));
            Size = listBox1.Size;

            windowToBeMoved = GetForegroundWindow();

            Show();
            TopMost = true;
            TopMost = false;
            Location = MousePosition;
            Activate();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_ACTIVATEAPP = 0x001C;

            if (m.Msg == WM_ACTIVATEAPP)
            {
                if (m.WParam.ToInt64() == 0)
                {
                    Hide();
                }
            }

            base.WndProc(ref m);
        }

        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != _MouseIndex && index != -1)
            {
                _MouseIndex = index;
                listBox1.SetSelected(_MouseIndex, true);
            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Desktop.FromIndex(listBox1.SelectedIndex).MoveWindow(windowToBeMoved);
                Hide();
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (_FirstStart)
            {
                _FirstStart = false;
                Hide();
            }
        }
    }
}
