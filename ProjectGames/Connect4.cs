using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace ProjectGames
{
    public partial class FrmConnect4 : Form
    {
        #region Initialization Functions

        public FrmConnect4()
        {
            InitializeComponent();
        }

        private TextBox _endbox;
        private bool _gameRun;
        private readonly SortedList<string, Graphics> _discs = new SortedList<string, Graphics>();
        private readonly Random _rand = new Random();

        // *** On Load
        private void frm4OpEenRij_Load(object sender, EventArgs e)
        {
            btnBegin.Click += btnBegin_Click;
            btnStop.Click += btnStop_Click;
            tmrTimer.Elapsed += Timer_Timer;

            MaximumSize = Size;
            MinimumSize = Size;
            tmrTimer.Enabled = true;
            _gameRun = true;
            foreach (TextBox tb in grbSelectie.Controls)
            {
                tb.MouseEnter += grpSelectie_TextBox_MouseEnter;
                tb.MouseLeave += grpSelectie_TextBox_MouseLeave;
                tb.MouseClick += grpSelectie_TextBox_MouseClick;
            }

            // *** Make all playfields round
            var pth = new System.Drawing.Drawing2D.GraphicsPath();
            foreach (var obj in this.Controls)
            {
                Region reg;
                switch (obj)
                {
                    case GroupBox box:
                        {
                            pth.AddEllipse(new Rectangle(0, 0, 50, 50));
                            reg = new Region(pth);
                            GroupBox gb = box;
                            foreach (var tb in gb.Controls)
                                ((TextBox)tb).Region = reg;
                            break;
                        }
                    case TextBox box:
                        pth.AddEllipse(new Rectangle(0, 0, 50, 50));
                        reg = new Region(pth);
                        box.Region = reg;
                        break;
                    case Button button:
                        {
                            var bt = button;
                            if (bt.BackgroundImage != null)
                            {
                                var map = new Bitmap(bt.BackgroundImage, bt.Width - 1, bt.Height - 1);
                                // *** For every pixel
                                for (var i = 1; i <= map.Width - 1; i++)
                                {
                                    for (var j = 1; j <= map.Height - 1; j++)
                                    {
                                        // *** If pixel = empty
                                        if (map.GetPixel(i, j) == Color.FromArgb(00, 00, 00, 00))
                                        {
                                            // *** Add pixel to hitbox
                                            pth.AddRectangle(new Rectangle(i, j, 1, 1));
                                        }
                                    }
                                }

                                reg = new Region(pth);
                                bt.Region = reg;
                                bt.BackgroundImage = null;
                            }

                            if (BackgroundImage != null)
                            {
                                bt.Image = new Bitmap(bt.Image, bt.Width - 1, bt.Height - 1);
                            }

                            break;
                        }
                }

                pth.Reset();
            }

            // *** Start global randomizer
            RandomNumberGenerator.Create();
        }

        #endregion

        #region Disc Functions

        // *** animation step timer
        private void Timer_Timer(object sender, EventArgs eventArgs)
        {
            // *** Animation trigger
            DropDisc();
        }

        // *** Sub To add discs to animation
        private void AddDisc(object sender)
        {
            // *** Clearing Disc list
            _discs.Clear();
            // *** declaration of variables
            string name = ((TextBox)sender).Name.Split('Y')[0];
            _endbox.BackColor = Color.White;
            // *** Get every field in the row and draw an object to start with at the top
            for (var i = 5; i >= 0; i += -1)
            {
                foreach (var obj in grbVeld.Controls)
                {
                    if (!((TextBox)obj is TextBox box)) continue;
                    var tb = box;
                    if (tb.Name != name + "Y" + Convert.ToString(i) || i < int.Parse(tb.Name.Split('Y')[1]) || tb.Text != "") continue;
                    var ystart = -(5 - int.Parse(tb.Name.Split('Y')[1])) * 56;
                    var key = tb.Name + ":Y" + ystart;
                    Graphics g = tb.CreateGraphics();
                    _discs.Add(key, g);
                }
            }
        }

        // *** Animation function for discs
        private void DropDisc()
        {
            // *** Declaration of the variables
            var tempDiscs = new SortedList<string, Graphics>();
            Brush brush = new SolidBrush(txtBeurt.BackColor);
            var pen = new Pen(Color.White);
            // *** For each disc
            foreach (var disc in _discs)
            {
                // *** Gather information about the disc
                var name = disc.Key.Split(':')[0];
                var key = disc.Key.Split(':')[1];
                var g = disc.Value;
                var yStart = int.Parse(key.Split('Y')[1]);
                // *** Build a new disc, overwrite and replace the old one
                var size = new Rectangle(0, yStart, 50, 50);
                g.DrawPie(pen, new Rectangle(0, yStart - 8, 50, 50), 0, 0);
                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(0, yStart - 8, 50, 50));
                g.DrawPie(pen, size, 0, 0);
                g.FillEllipse(brush, size);

                // *** Check if it is the last field
                if (name == _endbox.Name)
                {
                    // *** Check if last field has his disc yet to be centered
                    if (yStart < 0)
                    {
                        tempDiscs.Add(name + ":Y" + (yStart + 8), g);
                    }
                    else
                    {
                        _endbox.BackColor = txtBeurt.BackColor;
                        EndRound(_endbox);
                    }
                }
                else
                {
                    // *** Check disc is still visible
                    if (yStart <= 50)
                    {
                        tempDiscs.Add(name + ":Y" + (yStart + 8), g);
                    }
                }
            }

            // *** Update disc list to only contain the still needing to be animated discs
            _discs.Clear();
            foreach (KeyValuePair<string, Graphics> disc in tempDiscs)
            {
                _discs.Add(disc.Key, disc.Value);
            }

            tempDiscs.Clear();
        }

        #endregion

        #region Button Functoins

        // *** Begin button
        private void btnBegin_Click(object sender, EventArgs e)
        {
            btnBegin.Text = @"Restart";
            TextBox tb;

            // *** Turn game progression on
            _gameRun = true;
            // *** Choose randomly what player begins

            txtBeurt.BackColor = _rand.Next(2) == 1 ? Color.Red : Color.Yellow;
            // *** Disable / enable appropriate controls
            btnStop.Enabled = true;
            // *** Reset playing field
            foreach (object obj in grbSelectie.Controls)
            {
                if (!(obj is TextBox box)) continue;
                tb = box;
                tb.BackColor = Color.White;
                tb.Tag = "";
                tb.Enabled = true;
            }

            foreach (object obj in grbVeld.Controls)
            {
                if (!(obj is TextBox box)) continue;
                tb = box;
                tb.BackColor = Color.White;
                tb.Tag = "";
                tb.Clear();
            }
        }

        // *** Stop button
        private void btnStop_Click(object sender, EventArgs e)
        {
            btnBegin.Text = @"Begin";
            // *** Turn game progression off
            _gameRun = false;
            // *** Disable / enable appropriate controls
            lblWinner.Text = "";
            btnStop.Enabled = false;
            txtBeurt.BackColor = Color.White;
            foreach (var obj in grbSelectie.Controls)
            {
                if (obj is TextBox box)
                    box.Enabled = false;
            }
        }

        #endregion

        #region Playfield Functions

        // *** Mouse enter playfields
        private void grpSelectie_TextBox_MouseEnter(object sender, EventArgs e)
        {
            TextBox localsender = (TextBox)sender;
            // *** Make function only run when game progression is on
            if ((_gameRun == false))
            {
                return;
            }

            // *** Let player know where the player is hovering by changing the background color
            localsender.BackColor = txtBeurt.BackColor;
            // *** Get lowest empty field
            var tb = GetLowestEmptyField(localsender);
            // *** Set preview color of where the disc would land
            tb.BackColor = txtBeurt.BackColor == Color.Red ? Color.OrangeRed : Color.YellowGreen;
        }

        // *** Mouse leave playfields
        private void grpSelectie_TextBox_MouseLeave(object sender, EventArgs e)
        {
            TextBox localsender = (TextBox)sender;
            // *** Search for preview
            var tb = GetHighestPlayerField(localsender);
            // *** If he lowest field is indeed a preview, set it to white
            if (txtBeurt.BackColor == localsender.BackColor)
            {
                tb.BackColor = Color.White;
            }

            // *** Make the selection field white
            localsender.BackColor = Color.White;
        }

        // *** Mouse click playfields
        private void grpSelectie_TextBox_MouseClick(object sender, EventArgs e)
        {
            TextBox localsender = (TextBox)sender;
            // *** If the clicked field is white, cancel
            if (localsender.BackColor == Color.White)
            {
                return;
            }

            // *** Search for preview
            var tb = GetHighestPlayerField(localsender);
            // *** If preview is the selected field, cancel
            if (tb.Name == localsender.Name)
            {
                return;
            }

            // *** Disable all selectable fields
            foreach (TextBox textb in grbSelectie.Controls)
            {
                textb.Enabled = false;
            }

            // *** Disc drop animation
            _endbox = tb;
            AddDisc(tb);
            // *** Add Verification data
            tb.Text = @" ";
        }

        // *** Gettingthe lowest empty field
        private TextBox GetLowestEmptyField(TextBox beginbox)
        {
            // *** Create verifaction data
            var name = beginbox.Name.Split('Y')[0];
            // *** Check all fields in a row and return the lowest not verified field
            for (var i = 0; i <= 5; i += 1)
            {
                foreach (Control tb in grbVeld.Controls)
                {
                    if (!(tb is TextBox box))
                    {
                        continue;
                    }

                    if (box.Name == name + "Y" + Convert.ToString(i) && box.BackColor == Color.White && box.Name != " ")
                    {
                        return box;
                    }
                }
            }

            return beginbox;
        }

        // *** Getting highest played field
        private TextBox GetHighestPlayerField(TextBox beginbox)
        {
            // *** Declaration of variables
            var name = beginbox.Name.Split('Y')[0];
            // *** Find the last player field (This is a preview)
            for (var i = 5; i >= 0; i += -1)
            {
                foreach (object obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box))
                    {
                        continue;
                    }

                    if (box.BackColor == Color.YellowGreen | box.BackColor == Color.OrangeRed && box.Name == name + "Y" + Convert.ToString(i) && box.Text != @" ")
                    {
                        return box;
                    }
                }
            }

            // *** If there is no preview return itself
            return beginbox;
        }

        #endregion

        #region EndTurn Functions

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
                if (tb.Name.Split('Y')[0] == playedField.Name.Split('Y')[0])
                {
                    sender = tb;
                }
            }

            switch (strPoint)
            {
                // *** Check who made a point, if no-one skip to next round
                case "No Point":
                    {
                        // *** Switch player color
                        RoleSwap();
                        foreach (TextBox tb in grbSelectie.Controls)
                        {
                            tb.Enabled = true;
                        }

                        return;
                    }
                case "Draw":
                    {
                        _gameRun = false;
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

            _gameRun = false;
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
            string pfCoords = playedField.Name.Split('X')[1];
            var pfNameX = playedField.Name.Split('X')[0] + "X";
            var pfNameY = playedField.Name.Split('Y')[0] + "Y";
            var pfx = Convert.ToInt32(pfCoords.Split('Y')[0]);
            var pfy = Convert.ToInt32(pfCoords.Split('Y')[1]);
            string[,] check = new string[4, 7];
            TextBox tb;
            // *** Build Checklist for all directions
            for (var i = pfx - 3; i <= pfx + 3; i += 1)
            {
                foreach (var obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box))
                    {
                        continue;
                    }

                    tb = box;
                    // *** Horizontal		Check(0,x)
                    if (tb.Name == pfNameX + Convert.ToString(i) + "Y" + pfy)
                    {
                        check[0, ((i - pfx) + 3)] = tb.Name;
                    }

                    // *** Diagonal /		Check(1,x)
                    if (tb.Name == pfNameX + Convert.ToString(i) + "Y" + (pfy + (i - pfx)))
                    {
                        check[1, ((i - pfx) + 3)] = tb.Name;
                    }

                    // *** Diagonal \		Check(2,x)
                    if (tb.Name == pfNameX + Convert.ToString(i) + "Y" + (pfy - (i - pfx)))
                    {
                        check[2, ((i - pfx) + 3)] = tb.Name;
                    }
                }
            }

            // *** Vertical	Check(3,x)
            for (var i = pfy - 3; i <= pfy + 3; i += 1)
            {
                foreach (object obj in grbVeld.Controls)
                {
                    if (!(obj is TextBox box))
                    {
                        continue;
                    }

                    tb = box;
                    if (tb.Name == pfNameY + Convert.ToString(i))
                    {
                        check[3, ((i - pfy) + 3)] = tb.Name;
                    }
                }
            }

            // *** Declaration of variables to count point streaks
            var curColor = txtBeurt.BackColor;
            var count = 0;
            var winner = new string[7];
            // *** Check all checklists if a player has won
            for (var i = 0; i <= 3; i += 1)
            {
                for (var j = 0; j <= 6; j += 1)
                {
                    foreach (var obj in grbVeld.Controls)
                    {
                        if (!(obj is TextBox box))
                        {
                            continue;
                        }

                        tb = box;
                        if (tb.Name != check[i, j])
                        {
                            continue;
                        }

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
                        if (count < 4)
                        {
                            continue;
                        }

                        // *** Mark all point containing fields
                        for (var k = 0; k <= count - 1; k += 1)
                        {
                            foreach (var ob in grbVeld.Controls)
                            {
                                if (!(ob is TextBox textBox))
                                {
                                    continue;
                                }

                                tb = textBox;
                                if (tb.Name == winner[k])
                                {
                                    tb.Tag = "1";
                                }
                            }
                        }

                        // *** Set winner name
                        point = curColor.Name;
                    }
                }

                count = 0;
            }

            // *** color all the winning fields
            foreach (var obj in grbVeld.Controls)
            {
                if (!(obj is TextBox box))
                {
                    continue;
                }

                tb = box;
                if ((string)tb.Tag == "1")
                {
                    tb.BackColor = Color.Green;
                }
            }

            if ((point != "No Point"))
            {
                return point;
            }

            foreach (var obj in grbVeld.Controls)
            {
                if (!(obj is TextBox box))
                {
                    continue;
                }

                tb = box;
                if (int.Parse(tb.Name.Split('Y')[1]) == 5 & tb.BackColor != Color.White)
                {
                    count += 1;
                }
            }

            if (count == 7)
            {
                point = "Draw";
            }

            Debug.WriteLine("point check complete: " + point);
            // *** Return winner
            return point;
        }

        // *** Role swap
        private void RoleSwap()
        {
            // *** Change current active player
            txtBeurt.BackColor = txtBeurt.BackColor == Color.Yellow ? Color.Red : Color.Yellow;
        }

        #endregion

        #region menu Function

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuGameManDontGetAngry_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form mdga = new ManDonTGetAngry();
            mdga.Location = this.Location;
            mdga.StartPosition = FormStartPosition.Manual;
            mdga.FormClosing += delegate { this.Show(); };
            mdga.Show();
            this.Hide();
        }

        #endregion
    }
}