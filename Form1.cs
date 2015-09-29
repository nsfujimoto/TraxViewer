using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TraxViewer_v2
{
    public partial class Form1 : Form
    {
        PictureBox picturebox1 = new PictureBox();
        Label label = new Label();
        Bitmap canvas;
        Graphics g;
        Image[] img = new Image[33];
        Board board = new Board();
        const int img_width = 30;
        const int img_height = 30;

        public Form1()
        {
            InitializeComponent();
            initpanel();
            initimg();
        }



        void initpanel()
        {
            picturebox1.Size = new Size(15360, 15360);
            picturebox1.Location = new Point(-7680 + (panel1.Size.Width / 2), -7680 + (panel1.Size.Height / 2));
            this.Controls.Add(picturebox1);
            panel1.Controls.Add(picturebox1);

            label.Size = new Size(0, 0);
            label.Location = new Point(7680 + (panel1.Size.Width / 2), 7680 + (panel1.Size.Height / 2));
            panel1.Controls.Add(label);
            panel1.ScrollControlIntoView(label);
        }
        void initimg()
        {
            img[1] = new Bitmap(GetType(), "vertical_w.png");
            img[2] = new Bitmap(GetType(), "horizontal_w.png");
            img[4] = new Bitmap(GetType(), "upper_left_w.png");
            img[8] = new Bitmap(GetType(), "lower_right_w.png");
            img[16] = new Bitmap(GetType(), "upper_right_w.png");
            img[32] = new Bitmap(GetType(), "lower_left_w.png");
            canvas = new Bitmap(picturebox1.Width, picturebox1.Height);
            g = Graphics.FromImage(canvas);
            picturebox1.Image = canvas;

            for (int i = 0; i < 512; i++)
            {
                g.DrawLine(Pens.Gray, 0, i * img_height, 15360, i * img_height);
                g.DrawLine(Pens.Black, i * img_width, 0, i * img_width, 15360);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string s in richTextBox1.Lines)
            {
                if (s == "") continue;
                if (board.sn_convert_place(s) == 1) richTextBox2.Text += s + "\n";
                else richTextBox2.Text += "不正な値\n";
            }
            richTextBox1.Clear();
            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    if (board.raw_board[i, j] == 0) continue;
                    else
                    {
                        g.DrawImage(img[board.raw_board[i, j]], img_width * i, img_height * j);
                    }
                }
            }
            picturebox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //canvas = new Bitmap(picturebox1.Width, picturebox1.Height);
            g.Clear(picturebox1.BackColor);
            //g = Graphics.FromImage(canvas);

            for (int i = 0; i < 512; i++)
            {
                g.DrawLine(Pens.Gray, 0, i * img_height, 15360, i * img_height);
                g.DrawLine(Pens.Black, i * img_width, 0, i * img_width, 15360);
            }
            picturebox1.Refresh();
            board = new Board();
            richTextBox1.Clear();
            richTextBox2.Clear();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                if (textBox1.Text == "") ;
                else if (board.sn_convert_place(textBox1.Text) == 1) richTextBox2.Text += textBox1.Text + "\n";
                else richTextBox1.Text += "不正な値\n";
                textBox1.Clear();
                for (int i = 0; i < 512; i++)
                {
                    for (int j = 0; j < 512; j++)
                    {
                        if (board.raw_board[i, j] == 0) continue;
                        else
                        {
                            g.DrawImage(img[board.raw_board[i, j]], img_width * i, img_height * j);
                        }
                    }
                }
                picturebox1.Refresh();
            }
        }
    }
}
