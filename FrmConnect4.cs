using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Project_CSharp
{
    public partial class FrmConnect4 : Form
    {
        public FrmConnect4()
        {

        private TextBox endbox;
        private bool GameRun;
        private SortedList<string, Graphics> Discs = new SortedList<string, Graphics>();

        // *** On Load
        private void frm4OpEenRij_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            Timer.Enabled = true;
            GameRun = true;
            Button bt;
            foreach (TextBox tb in grbSelectie.Controls)
            {
                tb.MouseEnter += grpSelectie_TextBox_MouseEnter;
                tb.MouseLeave += grpSelectie_TextBox_MouseLeave;
                tb.MouseClick += grpSelectie_TextBox_MouseClick;
            }

            // *** Make all playfields round
            var Pth = new System.Drawing.Drawing2D.GraphicsPath();
            var Reg = new Region(Pth);
            foreach (var obj in this.Controls)
            {
                switch (obj)
                {
                    case GroupBox box:
                    {
                        Pth.AddEllipse(new Rectangle(0, 0, 50, 50));
                        Reg = new Region(Pth);
                        GroupBox gb = box;
                        foreach (var tb in gb.Controls)
                            tb.Region = Reg;
                        break;
                    }
                    case TextBox _:
                        Pth.AddEllipse(new Rectangle(0, 0, 50, 50));
                        Reg = new Region(Pth);
                        obj.Region = Reg;
                        break;
                    case Button button:
                    {
                        bt = button;
                        if (bt.BackgroundImage != null)
                        {
                            var map = new Bitmap(bt.BackgroundImage, bt.Width - 1, bt.Height - 1);
                            // *** Voor elke pixel
                            for (var i = 1; i <= map.Width - 1; i++)
                            {
                                for (var j = 1; j <= map.Height - 1; j++)
                                {
                                    // *** Als de pixel leeg is
                                    if (map.GetPixel(i, j) == Color.FromArgb(00, 00, 00, 00))
                                        // *** Teken hem bij de graphische tekening
                                        Pth.AddRectangle(new Rectangle(i, j, 1, 1));
                                }
                            }

                            Reg = new Region(Pth);
                            bt.Region = Reg;
                            bt.BackgroundImage = null;
                        }

                        if (BackgroundImage != null)
                            bt.Image = new Bitmap(bt.Image, bt.Width - 1, bt.Height - 1);
                        break;
                    }
                }

                Pth.Reset();
            }

            // *** Start global randomizer
            RandomNumberGenerator.Create();
        }

        // *** animation step timer
        private void Timer_Timer()
        {
            // *** Animation trigger
            DropDisc();
        }

        // *** Sub To add discs to animation
        private void AddDisc(object sender)
        {
            // *** Clearing Disc list
            Discs.Clear();
            // *** declaration of variables
            int Ystart;
            TextBox tb;
            string Name = sender.Name.Split("Y")[0];
            endbox.BackColor = Color.White;
            // *** Get every field in the row and draw an object to start with at the top
            for (var i = 5; i >= 0; i += -1)
            {
                foreach (var obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box)) continue;
                    tb = box;
                    if (tb.Name != Name + "Y" + System.Convert.ToString(i) || i < sender.Name.Split("Y")[1] ||
                        tb.Text != "") continue;
                    Ystart = -(5 - tb.Name.Split(chr("Y"))[1]) * 56;
                    var key = tb.Name + ":Y" + Ystart;
                    Graphics g = tb.CreateGraphics;
                    Discs.Add(key, g);
                }
            }
        }

        // *** Animation function for discs
        private void DropDisc()
        {
            // *** Declaration of the variables
            var tempDiscs = new SortedList<string, Graphics>();
            var yStart = 0;
            Brush brush = new SolidBrush(txtBeurt.BackColor);
            var pen = new Pen(Color.White);
            string name;
            string key;
            Graphics g;
            Rectangle size;
            // *** For each disc
            foreach (KeyValuePair<string, Graphics> disc in Discs)
            {
                // *** Gather information about the disc
                name = disc.Key.Split(chr(":"))[0];
                key = disc.Key.Split(chr(":"))[1];
                g = disc.Value;
                yStart = key.Split(chr("Y"))[1];
                // *** Build a new disc, overwrite and replace the old one
                size = new Rectangle(0, yStart, 50, 50);
                g.DrawPie(pen, new Rectangle(0, yStart - 8, 50, 50), 0, 0);
                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(0, yStart - 8, 50, 50));
                g.DrawPie(pen, size, 0, 0);
                g.FillEllipse(brush, size);

                // *** Check if it is the last field
                if (name == endbox.Name)
                {
                    // *** Check if last field has his disc yet to be centered
                    if (yStart < 0)
                        tempDiscs.Add(name + ":Y" + yStart + 8, g);
                    else
                    {
                        endbox.BackColor = txtBeurt.BackColor;
                        EndRound(endbox);
                    }
                }
                else
                    // *** Check disc is still visible
                if (yStart <= 50)
                    tempDiscs.Add(name + ":Y" + yStart + 8, g);
            }

            // *** Update disc list to only contain the still needing to be animated discs
            Discs.Clear();
            foreach (KeyValuePair<string, Graphics> disc in tempDiscs)
                Discs.Add(disc.Key, disc.Value);
            tempDiscs.Clear();
        }

        // *** Begin button
        private void btnBegin_Click(object sender, EventArgs e)
        {
            // *** Turn game progression on
            GameRun = true;
            // *** Choose randomly what player begins
            if (Conversion.Fix(VBMath.Rnd() * 2) == 1)
                txtBeurt.BackColor = Color.Red;
            else
                txtBeurt.BackColor = Color.Yellow;
            // *** Disable / enable appropriate controls
            btnBegin.Enabled = false;
            btnStop.Enabled = true;
            // *** Reset playing field
            TextBox tb;
            foreach (object obj in grbSelectie.Controls)
            {
                if (!(obj is TextBox box)) continue;
                tb = box;
                tb.Enabled = true;
                tb.Tag = "";
                tb.BackColor = Color.White;
            }

            foreach (object obj in grbVeld.Controls)
            {
                if (!(obj is TextBox box)) continue;
                tb = box;
                tb.BackColor = Color.White;
                tb.Clear();
            }
        }

        // *** Stop button
        private void btnStop_Click(object sender, EventArgs e)
        {
            // *** Turn game progression off
            GameRun = false;
            // *** Disable / enable appropriate controls
            lblWinner.Text = "";
            btnBegin.Enabled = true;
            btnStop.Enabled = false;
            txtBeurt.BackColor = Color.White;
            foreach (var obj in grbSelectie.Controls)
            {
                if (obj is TextBox)
                    obj.Enabled = false;
            }
        }

        // *** Menu button
        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // *** Mouse enter playfields
        private void grpSelectie_TextBox_MouseEnter(TextBox sender, EventArgs e)
        {
            // *** Make function only run when game progression is on
            if ((GameRun == false))
                return;
            // *** Let player know where the player is hovering by changing the background color
            sender.BackColor = txtBeurt.BackColor;
            // *** Get lowest empty field
            var tb = GetLowestEmptyField(sender);
            // *** Set preview color of where the disc would land
            tb.BackColor = txtBeurt.BackColor == Color.Red ? Color.OrangeRed : Color.YellowGreen;
        }

        // *** Mouse leave playfields
        private void grpSelectie_TextBox_MouseLeave(TextBox sender, EventArgs e)
        {
            // *** Search for preview
            var tb = GetHighestPlayerField(sender);
            // *** If he lowest field is indeed a preview, set it to white
            if (txtBeurt.BackColor == sender.BackColor)
                tb.BackColor = Color.White;
            // *** Make the selection field white
            sender.BackColor = Color.White;
        }

        // *** Mouse click playfields
        private void grpSelectie_TextBox_MouseClick(TextBox sender, EventArgs e)
        {
            // *** If the clicked field is white, cancel
            if (sender.BackColor == Color.White)
                return;
            // *** Search for preview
            var tb = GetHighestPlayerField(sender);
            // *** If preview is the selected field, cancel
            if (tb.Name == sender.Name)
                return;
            // *** Disable all selectable fields
            foreach (TextBox textb in grbSelectie.Controls)
                textb.Enabled = false;
            // *** Disc drop animation
            endbox = tb;
            AddDisc(tb);
            // *** Add Verification data
            tb.Text = @" ";
        }

        // *** Gettingthe lowest empty field
        private TextBox GetLowestEmptyField(TextBox beginbox)
        {
            // *** Create verifaction data
            var name = beginbox.Name.Split("Y".ToCharArray())[0];
            // *** Check all fields in a row and return the lowest not verified field
            for (var i = 0; i <= 5; i += 1)
            {
                foreach (Control tb in grbVeld.Controls)
                {
                    if (!(tb is TextBox box)) continue;
                    if (box.Name == name + "Y" + System.Convert.ToString(i) &&
                        box.BackColor == Color.White && box.Name != " ")
                        return box;
                }
            }

            return beginbox;
        }

        // *** Getting highest played field
        private TextBox GetHighestPlayerField(TextBox beginbox)
        {
            // *** Declaration of variables
            var name = beginbox.Name.Split(chr("Y"))[0];
            // *** Find the last player field (This is a preview)
            for (var i = 5; i >= 0; i += -1)
            {
                foreach (object obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box)) continue;
                    if (box.BackColor == Color.YellowGreen |
                        box.BackColor == Color.OrangeRed &&
                        box.Name == name + "Y" + System.Convert.ToString(i) && box.Text != " ")
                        return box;
                }
            }

            // *** If there is no preview return itself
            return beginbox;
        }

        // *** Endround
        private void EndRound(TextBox playedField)
        {
            // *** Declaration of winner variable
            var strPoint = PointCheck(playedField);
            // *** Get sender
            var sender = new TextBox();
            foreach (TextBox tb in grbSelectie.Controls)
            {
                tb.Enabled = true;
                if (tb.Name.Split("Y".ToCharArray())[0] == playedField.Name.Split("Y".ToCharArray())[0])
                    sender = tb;
            }

            switch (strPoint)
            {
                // *** Check who made a point, if no-one skip to next round
                case "No Point":
                {
                    // *** Switch player color
                    RoleSwap();
                    foreach (TextBox tb in grbSelectie.Controls)
                        tb.Enabled = true;
                    return;
                }
                case "Draw":
                {
                    GameRun = false;
                    // *** Empty all selection controls
                    foreach (TextBox tb in grbSelectie.Controls)
                    {
                        tb.Clear();
                        tb.Enabled = false;
                    }

                    // *** Show who won
                    lblWinner.Text = strPoint;
                    // *** Simulate a new mouse enter event
                    grpSelectie_TextBox_MouseEnter(sender, EventArgs.Empty);
                    return;
                }
            }

            GameRun = false;
            // *** Empty all selection controls
            foreach (TextBox tb in grbSelectie.Controls)
            {
                tb.Clear();
                tb.Enabled = false;
            }

            // *** Show who won
            lblWinner.Text = strPoint + @" Wins";
            // *** Simulate a new mouse enter event
            grpSelectie_TextBox_MouseEnter(sender, EventArgs.Empty);
        }

        // *** Pointcheck
        private string PointCheck(TextBox playedField)
        {
            // *** Make variables to manage coördinates or points
            var point = "No Point";
            string pfCoords = playedField.Name.Split("X".ToCharArray())[1];
            var pfNameX = playedField.Name.Split("X".ToCharArray())[0] + "X";
            var pfNameY = playedField.Name.Split("Y".ToCharArray())[0] + "Y";
            var pfx = System.Convert.ToInt32(pfCoords.Split("Y".ToCharArray())[0]);
            var pfy = System.Convert.ToInt32(pfCoords.Split("Y".ToCharArray())[1]);
            string[,] check = new string[4, 7];
            TextBox tb;
            // *** Build Checklist for all directions
            for (var i = pfx - 3; i <= pfx + 3; i += 1)
            {
                foreach (var obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box)) continue;
                    tb = box;
                    // *** Horizontal		Check(0,x)
                    if (tb.Name == pfNameX + System.Convert.ToString(i) + "Y" + pfy)
                        check[0, ((i - pfx) + 3)] = tb.Name;
                    // *** Diagonal /		Check(1,x)
                    if (tb.Name == pfNameX + System.Convert.ToString(i) + "Y" + (pfy + (i - pfx)))
                        check[1, ((i - pfx) + 3)] = tb.Name;
                    // *** Diagonal \		Check(2,x)
                    if (tb.Name == pfNameX + System.Convert.ToString(i) + "Y" + (pfy - (i - pfx)))
                        check[2, ((i - pfx) + 3)] = tb.Name;
                }
            }

            // *** Vertical		Check(3,x)
            for (var i = pfy - 3; i <= pfy + 3; i += 1)
            {
                foreach (object obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box)) continue;
                    tb = box;
                    if (tb.Name == pfNameY + System.Convert.ToString(i))
                        check[3, ((i - pfy) + 3)] = tb.Name;
                }
            }

            // *** Declaration of variables to count point streaks
            var curColor = txtBeurt.BackColor;
            var count = 0;
            var winnerTemp = new string[7];
            var winners = new string[7];
            var winner = new string[7];
            // *** Check all checklists if a player has won
            for (var i = 0; i <= 3; i += 1)
            {
                for (var j = 0; j <= 6; j += 1)
                {
                    foreach (var obj in grbVeld.Controls)
                    {
                        if (!(obj is TextBox box)) continue;
                        tb = box;
                        if (tb.Name != check[i, j]) continue;
                        // *** Check if the played field has the same color as the player
                        if (tb.BackColor == curColor)
                        {
                            // *** Add field to point count, point + 1
                            winner[count] = tb.Name;
                            count += 1;
                        }
                        else
                        {
                            // *** Reset point count to 0
                            count = 0;
                        }

                        // *** If 4 or more points are made, mark them
                        if (count < 4) continue;
                        // *** Mark all point containing fields
                        for (var k = 0; k <= count - 1; k += 1)
                        {
                            foreach (var ob in grbVeld.Controls)
                            {
                                if (!(ob is TextBox textBox)) continue;
                                tb = textBox;
                                if (tb.Name == winner[k])
                                    tb.Tag = "1";
                            }
                        }

                        // *** Set winner name
                        point = curColor.ToString.Split("]")[0].Split("[")[1];
                    }
                }

                count = 0;
            }

            // *** color all the winning fields
            foreach (var obj in grbVeld.Controls)
            {
                if (!(obj is TextBox box)) continue;
                tb = box;
                if ((string) tb.Tag == "1")
                    tb.BackColor = Color.Green;
            }

            if ((point != "No Point")) return point;
            {
                foreach (var obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box)) continue;
                    tb = box;
                    if ((int) tb.Name.Split("Y".ToCharArray())[1] == 5 & tb.BackColor != Color.White)
                    {
                        count += 1;
                    }
                }

                if (count == 7)
                    point = "Draw";
            }

            // *** Return winner
            return point;
        }

        // *** Role swap
        private void RoleSwap()
        {
            // *** Change current active player
            txtBeurt.BackColor = txtBeurt.BackColor == Color.Yellow ? Color.Red : Color.Yellow;
        }
    }
}

}