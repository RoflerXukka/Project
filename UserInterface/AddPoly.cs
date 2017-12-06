using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class AddPoly : Form
    {
        private Form ownerform;
        public AddPoly(Users main)
        {
            InitializeComponent();
            ownerform = main;
            MoveObj.BackColor = System.Drawing.Color.Transparent;
            CLOSE.FlatAppearance.BorderSize = 0;
            this.FirstText.Text = "Ф.И.О клиента";
            this.SecondText.Text = "Адрес клиента";
            this.ThirdText.Visible = false;
            this.ThirdPoly.Visible = false;
            this.FourthText.Visible = false;
            this.FourthPoly.Visible = false;
            this.Height = 200;
            this.AddPolys.Location = new System.Drawing.Point(59, 150);
            this.Cancel.Location = new System.Drawing.Point(304, 150);
        }
        public AddPoly(Books main)
        {
            InitializeComponent();
            ownerform = main;
            MoveObj.BackColor = System.Drawing.Color.Transparent;
            CLOSE.FlatAppearance.BorderSize = 0;
            this.FirstText.Text = "Название книги";
            this.SecondText.Text = "Автор";
            this.ThirdText.Text = "Жанр";
            this.FourthText.Text = "Год";
        }
        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        private void AddPolys_Click(object sender, EventArgs e)
        {
            if (ownerform is Users users) users.AddToDATA(FirstPoly.Text, SecondPoly.Text);
            else ((Books)ownerform).AddToDATA(FirstPoly.Text, SecondPoly.Text, ThirdPoly.Text, FourthPoly.Text);
            Close();
        }
        private void Cancel_Click(object sender, EventArgs e) => Close();
        private void MoveObj_MouseDown(object sender, MouseEventArgs e)
        {
            MoveObj.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }
        private void CLOSE_Click(object sender, EventArgs e) => Close();
        private void CLOSE_MouseMove(object sender, MouseEventArgs e) { CLOSE.BackColor = Color.Crimson; CLOSE.ForeColor = Color.White; }
        private void CLOSE_MouseLeave(object sender, EventArgs e) { CLOSE.BackColor = Color.Gainsboro; CLOSE.ForeColor = Color.Black; }
    }
}