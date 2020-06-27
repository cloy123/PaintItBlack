using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace PaintItBlack
{

    public partial class Form1 : Form
    {
        //для кнопок отменить и вперёд
        private int historyCounter; //Счетчик истории
        private List<Image> History; //Список для истории

        //рисование и pictureBox
        public Pen pen;
        private Graphics g;
        private Point x_y;
        private Point x_y2;
        private bool moving = false;

        private string FileName = "";
        private Point PointShift;

        // инструменты
        private bool RectOn = false;
        private bool EllipsOn = false;
        private bool PenOn = true;
        private bool EraserOn = false;
        private bool LineOn = false;
        private bool TextOn = false;
        private bool PipOn = false;
        private bool FillOn = false;

        private Point RealLocation1;
        private Point RealLocation2;
        private Point TextPoint;
        private Font TextFont;
        private Color FontColor = Color.Black;
        private int IntShift = 1;

        private bool pbChange = false;
        public Form1()
        {
            InitializeComponent();
            zoomPictureBox1.Image = new Bitmap(zoomPictureBox1.Width, zoomPictureBox1.Height);
        }
        public Form1(string fileName) // Открытие программы документом
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".png" || ext == "jpg" || ext == ".bmp" || ext == ".gif")
                {
                    try
                    {
                        OpenFileDialog OP = new OpenFileDialog
                        {
                            FileName = fileName
                        };
                        zoomPictureBox1.Image = new Bitmap(Image.FromFile(OP.FileName));
                        FileName = OP.FileName;
                        OP.Dispose();
                        zoomPictureBox1.Invalidate();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        zoomPictureBox1.Image = new Bitmap(zoomPictureBox1.Width, zoomPictureBox1.Height);
                    }
                }
                else
                {
                    zoomPictureBox1.Image = new Bitmap(zoomPictureBox1.Width, zoomPictureBox1.Height);
                }
            }
            else
            {
                zoomPictureBox1.Image = new Bitmap(zoomPictureBox1.Width, zoomPictureBox1.Height);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            g = Graphics.FromImage(zoomPictureBox1.Image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //Pen
            pen = new Pen(Color.Black);
            pen.Width = Convert.ToSingle(numericUpDown1.Value);
            pen.DashStyle = DashStyle.Solid;
            comboBox1.SelectedIndex = 0;
            pen.StartCap = pen.EndCap = LineCap.Round;
            //отменить и вперёд
            History = new List<Image>(); //Инициализация списка для истории
            historyCounter = 0;
            History.Add(new Bitmap(zoomPictureBox1.Image));
            //рисование с shift
            IntShift = 1;
            TextFont = textBox1.Font;
            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = 1000;
            numericUpDown2.Value = button1.BackColor.A;
            this.Text = FileName;
        }
        private void ZoomPictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moving = true;
                RealLocation1 = e.Location;
                x_y = zoomPictureBox1.ClientToImagePoint(e.Location);
            }
        }
        private void ZoomPictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            RealLocation2 = e.Location;

            Graphics g = Graphics.FromImage(zoomPictureBox1.Image);
            x_y2 = zoomPictureBox1.ClientToImagePoint(e.Location);
            if (moving && Control.ModifierKeys != Keys.Shift && PenOn)
            {
                g.DrawLine(pen, x_y, x_y2);
                g.Dispose();
                zoomPictureBox1.Invalidate();
                x_y = zoomPictureBox1.ClientToImagePoint(e.Location);
                RealLocation1 = e.Location;
            }
            //рисование прямоугольника и эллипса 
            else if (moving)
            {
                zoomPictureBox1.Invalidate();
            }
            toolStripStatusLabel2.Text = zoomPictureBox1.ClientToImagePoint(RealLocation2).ToString();
        }
        private void ZoomPictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (RectOn && moving)
            {
                try
                {
                    Graphics g = Graphics.FromImage(zoomPictureBox1.Image);
                    Rectangle TempRect = FunctionClass.CreateRect(x_y, x_y2);
                    g.DrawRectangle(pen, TempRect);
                    toolStripStatusLabel8.Text = Convert.ToString(TempRect.Height);
                    toolStripStatusLabel11.Text = Convert.ToString(TempRect.Width);
                    g.Dispose();
                    zoomPictureBox1.Invalidate();
                }
                catch { }
            }
            else if (EllipsOn && moving)
            {
                try
                {
                    Graphics g = Graphics.FromImage(zoomPictureBox1.Image);
                    Rectangle TempRect = FunctionClass.CreateRect(x_y, x_y2);
                    g.DrawEllipse(pen, TempRect);
                    toolStripStatusLabel8.Text = Convert.ToString(TempRect.Height);
                    toolStripStatusLabel11.Text = Convert.ToString(TempRect.Width);
                    g.Dispose();
                    zoomPictureBox1.Invalidate();
                }
                catch { }
            }
            else if (LineOn && moving)
            {
                try
                {
                    Graphics g = Graphics.FromImage(zoomPictureBox1.Image);
                    FunctionClass.DrawLine(x_y, x_y2, g, pen);
                    g.Dispose();
                    zoomPictureBox1.Invalidate();
                }
                catch { }
            }
            //Очистка ненужной истории
            if (History[History.Count - 1] != zoomPictureBox1.image)
            {
                ChangeHistoty();
                Undo_redo();
            }
            moving = false;
        }
        private void ZoomPictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift && PenOn)
            {
                if (IntShift != 2)
                {
                    IntShift += 1;
                }
                else
                {
                    Graphics g = Graphics.FromImage(zoomPictureBox1.Image);
                    g.DrawLine(pen, PointShift, zoomPictureBox1.ClientToImagePoint(e.Location));
                    g.Dispose();
                    zoomPictureBox1.Invalidate();
                }
                PointShift = zoomPictureBox1.ClientToImagePoint(e.Location);
            }
            else
            {
                IntShift = 1;
            }
            if (TextOn)
            {
                TextPoint = e.Location;
                textBox1.Visible = true;
                OkButton.Visible = true;
                CanleButton1.Visible = true;
                FontButton.Visible = true;
                zoomPictureBox1.AllowUserDrag = false;
                zoomPictureBox1.AllowUserZoom = false;
                ToolStripMenuItemZoom.Enabled = false;
                textBox1.Focus();
            }
            else if (PipOn && (e.Button == MouseButtons.Left))
            {
                Color color = Color.White;
                try
                {
                    color = zoomPictureBox1.image.GetPixel(zoomPictureBox1.ClientToImagePoint(e.Location).X, zoomPictureBox1.ClientToImagePoint(e.Location).Y);
                }
                catch { }
                if (color.A == 0)
                {
                    color = Color.White;
                    button1.BackColor = color;
                    numericUpDown2.Value = color.A;
                    pen.Color = color;
                }
                else
                {
                    button1.BackColor = color;
                    numericUpDown2.Value = color.A;
                    pen.Color = color;
                }
            }
            else if (FillOn && e.Button == MouseButtons.Left)
            {
                int x = zoomPictureBox1.ClientToImagePoint(e.Location).X;
                int y = zoomPictureBox1.ClientToImagePoint(e.Location).Y;
                if (x >= 0 && x < zoomPictureBox1.image.Width && y >= 0 && y < zoomPictureBox1.image.Height)
                {
                    if (FunctionClass.GetPx(x, y, ref zoomPictureBox1).ToArgb() != pen.Color.ToArgb())
                    {
                        FunctionClass.Fill(zoomPictureBox1.ClientToImagePoint(e.Location), pen.Color, ref zoomPictureBox1);
                    }
                }
            }
        }
        private void ZoomPictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (moving)
            {
                if (RectOn)
                {
                    e.Graphics.DrawRectangle(new Pen(pen.Color, pen.Width * zoomPictureBox1.Zoom), FunctionClass.CreateRect(RealLocation1, RealLocation2));
                }
                else if (EllipsOn)
                {
                    e.Graphics.DrawEllipse(new Pen(pen.Color, pen.Width * zoomPictureBox1.Zoom), FunctionClass.CreateRect(RealLocation1, RealLocation2));
                }
                else if (LineOn)
                {
                    FunctionClass.DrawLine(RealLocation1, RealLocation2, e.Graphics, new Pen(pen.Color, pen.Width * zoomPictureBox1.Zoom));
                }
            }
            else if (TextOn)
            {
                e.Graphics.DrawString(textBox1.Text, new Font(TextFont.FontFamily, TextFont.Size * zoomPictureBox1.Zoom, TextFont.Style), new SolidBrush(FontColor), TextPoint);
            }
        }

        //save///////////////
        private void SaveAsStrip_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4; //По умолчанию будет выбрано последнее расширение*.png
            SaveDlg.ShowDialog();
            if (SaveDlg.FileName != "") //Если введено не пустое имя
            {
                System.IO.FileStream fs = (System.IO.FileStream)SaveDlg.OpenFile();
                switch (SaveDlg.FilterIndex)
                {
                    case 1:
                        zoomPictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);

                        break;
                    case 2:
                        zoomPictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case 3:
                        zoomPictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case 4:
                        zoomPictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
                FileName = SaveDlg.FileName;
                pbChange = false;
            }
        }
        private void SaveToolStrip_Click(object sender, EventArgs e)
        {
            if (FileName != "")
            {
                zoomPictureBox1.image.Save(FileName);
                pbChange = false;
            }
            else
            {
                SaveAsStrip_Click(sender, e);
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveToolStrip_Click(sender, e);
        }
        private void SaveForClose(object sender, EventArgs e)
        {
            if (pbChange)
            {
                DialogResult result = MessageBox.Show("Сохранить текущее изображение? ", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: SaveToolStrip_Click(sender, e); ; break;
                    case DialogResult.Cancel: return;
                }
            }
        }
        /////////////////////////

        //open
        private void OpenStrip_Click(object sender, EventArgs e)
        {
            SaveForClose(sender, e);
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1; //По умолчанию будет выбрано первое расширение *.jpg И, когда пользователь укажет нужный путь к картинке, ее нужно будет загрузить в PictureBox:
            if (OP.ShowDialog() == DialogResult.OK)
            {
                zoomPictureBox1.Image = new Bitmap(Bitmap.FromFile(OP.FileName));
                History.Clear();
                historyCounter = 0;
                History.Add(new Bitmap(zoomPictureBox1.Image));
                Undo_redo();
                g = Graphics.FromImage(zoomPictureBox1.Image);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                FileName = OP.FileName;
                this.Text = FileName;
                pbChange = false;
                zoomPictureBox1.Invalidate();
            }
        }

        //создать
        public void CreateStrip_Click(object sender, EventArgs e)
        {
            SaveForClose(sender, e);
            CreateForm create_F = new CreateForm();
            create_F.ShowDialog();
            if (create_F.create == true)
            {
                zoomPictureBox1.image = create_F.GetNewBitmap();
                History.Clear();
                historyCounter = 0;
                History.Add(new Bitmap(zoomPictureBox1.Image));
                Undo_redo();
                g = Graphics.FromImage(zoomPictureBox1.Image);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                FileName = "";
                this.Text = FileName;
                pbChange = false;
                zoomPictureBox1.Invalidate();
            }

        }
        //выйти//////
        private void CloseStrip_Click(object sender, EventArgs e)
        {
            if (pbChange)
            {
                DialogResult result = MessageBox.Show("Сохранить текущее изображение? ", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: Close(); break;
                    case DialogResult.Yes: SaveToolStrip_Click(sender, e); ; break;
                    case DialogResult.Cancel: return;
                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pbChange)
            {
                DialogResult result = MessageBox.Show("Сохранить текущее изображение? ", "Предупреждение", MessageBoxButtons.YesNo);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: SaveToolStrip_Click(sender, e); ; break;
                }
            }
        }
        /////////////


        //выбрать кисть
        private void Pen_button_Click(object sender, EventArgs e)
        {
            PenToolStrip_Click(sender, e);
        }
        private void PenToolStrip_Click(object sender, EventArgs e)
        {
            pen.Color = button1.BackColor;
            PenOn = СhangeTool();
            ChangeCursor();
        }

        //ластик
        private void EraserStrip_Click(object sender, EventArgs e)
        {
            pen.Color = Color.White;
            EraserOn = СhangeTool();
            PenOn = true;
            ChangeCursor();
        }
        //выбрать ластик
        private void Lastic_button_Click(object sender, EventArgs e)
        {
            EraserStrip_Click(sender, e);
        }

        //pip
        private void PipToolStrip_Click(object sender, EventArgs e)
        {
            PipOn = СhangeTool();
            ChangeCursor();
        }
        private void Pip_button_Click(object sender, EventArgs e)
        {
            PipToolStrip_Click(sender, e);
        }

        //выбрать прямоугольник
        private void Rect_Strip_Click(object sender, EventArgs e)
        {
            PenToolStrip_Click(sender, e);
            RectOn = СhangeTool();
        }
        private void RectButton_Click(object sender, EventArgs e)
        {
            Rect_Strip_Click(sender, e);
        }
        //выбрать эллипс
        private void Ellipse_Strip_Click(object sender, EventArgs e)
        {
            PenToolStrip_Click(sender, e);
            EllipsOn = СhangeTool();
        }
        private void EllipsButton_Click(object sender, EventArgs e)
        {
            Ellipse_Strip_Click(sender, e);
        }

        //выбрать линию
        private void Line_Strip_Click(object sender, EventArgs e)
        {
            PenToolStrip_Click(sender, e);
            LineOn = СhangeTool();
        }
        private void LineButton_Click(object sender, EventArgs e)
        {
            Line_Strip_Click(sender, e);
        }

        //текст
        private void TextStrip_Click(object sender, EventArgs e)
        {
            zoomPictureBox1.Invalidate();
            TextOn = СhangeTool();
            ChangeCursor();
        }
        private void TextOnButton_Click(object sender, EventArgs e)
        {
            TextStrip_Click(sender, e);
        }
        //отменить
        private void Undo_lStrip_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && historyCounter != 0)
            {
                zoomPictureBox1.Image = new Bitmap(History[--historyCounter]);
                Undo_redo();
            }
        }
        private void Undo_button_Click(object sender, EventArgs e)
        {
            Undo_lStrip_Click(sender, e);
        }
        //вперёд
        private void Forward_Strip_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && History.Count > historyCounter + 1)
            {
                zoomPictureBox1.Image = new Bitmap(History[++historyCounter]);
                Undo_redo();
            }
            else MessageBox.Show("История пуста");
        }
        private void Redo_button_Click(object sender, EventArgs e)
        {
            Forward_Strip_Click(sender, e);
        }
        //Font
        private void FontSetToolStrip_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = TextFont;
            fontDialog1.ShowColor = true;
            DialogResult = fontDialog1.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                TextFont = fontDialog1.Font;
                FontColor = fontDialog1.Color;
                zoomPictureBox1.Invalidate();
            }
        }
        private void FontButton_Click(object sender, EventArgs e)
        {
            FontSetToolStrip_Click(sender, e);
        }
        //Fill
        private void BucketStrip_Click(object sender, EventArgs e)
        {
            FillOn = СhangeTool();
            ChangeCursor();
        }
        private void Fillbutton_Click(object sender, EventArgs e)
        {
            BucketStrip_Click(sender, e);
        }
        //print////
        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap myBitmap1 = new Bitmap(zoomPictureBox1.image);
            e.Graphics.DrawImage(myBitmap1, 0, 0);
            myBitmap1.Dispose();
        }
        private void PrintToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Drawing.Printing.PrintDocument myPrintDocument1 = new System.Drawing.Printing.PrintDocument();
            PrintDialog myPrinDialog1 = new PrintDialog();
            myPrintDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintDocument1_PrintPage);
            myPrinDialog1.Document = myPrintDocument1;
            if (myPrinDialog1.ShowDialog() == DialogResult.OK)
            {
                myPrintDocument1.Print();
            }
        }
        private void PrintPriviewToolStrip_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
            }
        }
        private void PrintSetupToolStrip_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = printDocument1;
            if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.DefaultPageSettings = pageSetupDialog1.PageSettings;
            }
        }
        //изменить цвет кисти
        private void Button1_Click(object sender, EventArgs e)
        {
            if (EraserOn)
            { Pen_button_Click(sender, e); }
            colorDialog1.FullOpen = true;
            DialogResult = colorDialog1.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                button1.BackColor = colorDialog1.Color;
                numericUpDown2.Value = colorDialog1.Color.A;
            }
        }
        private void Undo_redo()
        {
            if (historyCounter == 0)
            {
                undo_button.Image = Properties.Resources.undo;
                undo_button.Enabled = false;
                Undo_lStrip.Enabled = false;
            }
            else
            {
                undo_button.Image = Properties.Resources.undo_2;
                undo_button.Enabled = true;
                Undo_lStrip.Enabled = true;
            }
            if (History.Count != 0 && History.Count > historyCounter + 1)
            {
                redo_button.Image = Properties.Resources.redo_2;
                Forward_Strip.Enabled = true;
                redo_button.Enabled = true;
            }
            else
            {
                redo_button.Image = Properties.Resources.redo;
                Forward_Strip.Enabled = false;
                redo_button.Enabled = false;
            }
        }
        //размер кисти
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pen.Width = Convert.ToSingle(numericUpDown1.Value);
        }
        //стиль кисти
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    pen.DashStyle = DashStyle.Solid;
                    break;
                case 1:
                    pen.DashStyle = DashStyle.Dot;
                    break;
                case 2:
                    pen.DashStyle = DashStyle.DashDotDot;
                    break;
                case 3:
                    pen.DashStyle = DashStyle.DashDot;
                    break;
                case 4:
                    pen.DashStyle = DashStyle.Dash;
                    break;
            }
        }
        private void ChangeHistoty()
        {
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(new Bitmap(zoomPictureBox1.Image));
            if (historyCounter + 1 < 10) historyCounter++;
            if (History.Count - 1 == 10) History.RemoveAt(0);
            pbChange = true;
        }
        private void ToolStripMenuItemZoom_Click(object sender, EventArgs e)
        {//вкл-выкл масштаб
            zoomPictureBox1.AllowUserZoom = zoomPictureBox1.AllowUserZoom ? false : true;
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(zoomPictureBox1.Image);
            g.DrawString(textBox1.Text, TextFont, new SolidBrush(FontColor), zoomPictureBox1.ClientToImagePoint(TextPoint));
            g.Dispose();
            CanleButton1_Click(sender, e);
            ChangeHistoty();
            Undo_redo();
        }
        private void CanleButton1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            ToolStripMenuItemZoom.Enabled = true;
            zoomPictureBox1.AllowUserZoom = ToolStripMenuItemZoom.CheckState == CheckState.Checked ? true : false;
            zoomPictureBox1.AllowUserDrag = true;
            OkButton.Visible = false;
            CanleButton1.Visible = false;
            FontButton.Visible = false;
            textBox1.Visible = false;
            PenToolStrip_Click(sender, e);
            zoomPictureBox1.Invalidate();
        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            zoomPictureBox1.Invalidate();
        }
        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(Convert.ToInt32(numericUpDown2.Value), button1.BackColor);
            pen.Color = button1.BackColor;
        }
        private void ZoomPictureBox1_MouseEnter(object sender, EventArgs e)
        {
            ChangeCursor();
        }
        private void ChangeCursor()
        {
            if (EllipsOn || RectOn || PenOn || LineOn)
            {
                zoomPictureBox1.Cursor = Cursors.Cross;
            }
            else if (EraserOn)
            {
                zoomPictureBox1.Cursor = new Cursor(Properties.Resources.lastic_ico.Handle);
            }
            else if (TextOn)
            {
                zoomPictureBox1.Cursor = Cursors.IBeam;
            }
            else if (PipOn)
            {
                zoomPictureBox1.Cursor = new Cursor(Properties.Resources.pip_ico.Handle);
            }
            else if (FillOn)
            {
                zoomPictureBox1.Cursor = new Cursor(Properties.Resources.fillCur.Handle);
            }
        }
        private bool СhangeTool()
        {
            EllipsOn = false;
            RectOn = false;
            PenOn = false;
            EraserOn = false;
            LineOn = false;
            TextOn = false;
            PipOn = false;
            FillOn = false;
            return true;
        }
        private void Turn90RigftToolStrip_Click(object sender, EventArgs e)
        {
            Image TempImg = zoomPictureBox1.image;
            TempImg.RotateFlip(RotateFlipType.Rotate90FlipNone);
            zoomPictureBox1.image = new Bitmap(TempImg);
            zoomPictureBox1.Invalidate();
            g = Graphics.FromImage(zoomPictureBox1.image);
            g.Dispose();
        }
        private void Turn90LeftToolStrip_Click(object sender, EventArgs e)
        {
            Image TempImg = zoomPictureBox1.image;
            TempImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
            zoomPictureBox1.image = new Bitmap(TempImg);
            zoomPictureBox1.Invalidate();
            g = Graphics.FromImage(zoomPictureBox1.image);
            g.Dispose();
        }
        private void Turn180ToolStrip_Click(object sender, EventArgs e)
        {
            Image TempImg = zoomPictureBox1.image;
            TempImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
            zoomPictureBox1.image = new Bitmap(TempImg);
            zoomPictureBox1.Invalidate();
            g = Graphics.FromImage(zoomPictureBox1.image);
            g.Dispose();
        }
        private void FlipVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image TempImg = zoomPictureBox1.image;
            TempImg.RotateFlip(RotateFlipType.RotateNoneFlipY);
            zoomPictureBox1.image = new Bitmap(TempImg);
            zoomPictureBox1.Invalidate();
            g = Graphics.FromImage(zoomPictureBox1.image);
            g.Dispose();
        }
        private void FlipHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image TempImg = zoomPictureBox1.image;
            TempImg.RotateFlip(RotateFlipType.RotateNoneFlipX);
            zoomPictureBox1.image = new Bitmap(TempImg);
            zoomPictureBox1.Invalidate();
            g = Graphics.FromImage(zoomPictureBox1.image);
            g.Dispose();
        }
    }
}




    

    
