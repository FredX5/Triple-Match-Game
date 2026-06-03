using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<Image> selectedImages = new List<Image>();
        List<Button> selectedCards = new List<Button>();
        List<Image> cardImages = new List<Image>();

        int matchesFound = 0;
        int moves = 0;
        int seconds = 0;

        Timer gameTimer = new Timer();
        Timer flipTimer = new Timer();

        public Form1()
        {
            InitializeComponent();

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;

            flipTimer.Interval = 800;
            flipTimer.Tick += FlipTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbSize.SelectedIndex = 0;

            lblMatches.Text = "Matches: 0";
            lblMoves.Text = "Moves: 0";
            lblTime.Text = "Time: 00:00";

            tableGame.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            seconds++;
            lblTime.Text = "Time: " + TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
        }

        private void FlipTimer_Tick(object sender, EventArgs e)
        {
            flipTimer.Stop();

            foreach (var btn in selectedCards)
            {
                btn.BackgroundImage = null;
                btn.Text = "?";
                btn.Enabled = true;
            }

            selectedCards.Clear();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            int neededImages = cmbSize.SelectedItem.ToString() == "3x4" ? 4 : 5;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileNames.Length != neededImages)
                {
                    MessageBox.Show("You must select exactly " + neededImages + " images.");
                    return;
                }

                selectedImages.Clear();

                foreach (string file in ofd.FileNames)
                {
                    selectedImages.Add(Image.FromFile(file));
                }

                MessageBox.Show("Images uploaded successfully!");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int rows = cmbSize.SelectedItem.ToString() == "3x4" ? 4 : 5;
            int cols = 3;
            int neededImages = rows == 4 ? 4 : 5;

            if (selectedImages.Count != neededImages)
            {
                MessageBox.Show("First upload exactly " + neededImages + " images.");
                return;
            }

            tableGame.Controls.Clear();
            tableGame.RowStyles.Clear();
            tableGame.ColumnStyles.Clear();

            tableGame.RowCount = rows;
            tableGame.ColumnCount = cols;

            for (int i = 0; i < rows; i++)
                tableGame.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

            for (int i = 0; i < cols; i++)
                tableGame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));

            matchesFound = 0;
            moves = 0;
            seconds = 0;

            lblMatches.Text = "Matches: 0";
            lblMoves.Text = "Moves: 0";
            lblTime.Text = "Time: 00:00";

            cardImages.Clear();

            foreach (Image img in selectedImages)
            {
                cardImages.Add(img);
                cardImages.Add(img);
                cardImages.Add(img);
            }

            Random rnd = new Random();
            cardImages = cardImages.OrderBy(x => rnd.Next()).ToList();

            for (int i = 0; i < cardImages.Count; i++)
            {
                Button card = new Button();
                card.Dock = DockStyle.Fill;
                card.Tag = cardImages[i];
                card.Text = "?";
                card.Font = new Font("Arial", 20, FontStyle.Bold);
                card.Click += Card_Click;

                tableGame.Controls.Add(card);
            }

            gameTimer.Start();
            tableGame.Height = tableGame.Width;
            this.Height = tableGame.Bottom + 120;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Button clickedCard = sender as Button;

            if (selectedCards.Count == 3)
                return;

            clickedCard.BackgroundImage = (Image)clickedCard.Tag;
            clickedCard.BackgroundImageLayout = ImageLayout.Stretch;
            clickedCard.Text = "";
            clickedCard.Enabled = false;

            selectedCards.Add(clickedCard);

            if (selectedCards.Count == 3)
            {
                moves++;
                lblMoves.Text = "Moves: " + moves;

                Image img1 = (Image)selectedCards[0].Tag;
                Image img2 = (Image)selectedCards[1].Tag;
                Image img3 = (Image)selectedCards[2].Tag;

                if (img1 == img2 && img2 == img3)
                {
                    matchesFound++;
                    lblMatches.Text = "Matches: " + matchesFound;

                    selectedCards.Clear();

                    int totalMatches = cmbSize.SelectedItem.ToString() == "3x4" ? 4 : 5;

                    if (matchesFound == totalMatches)
                    {
                        gameTimer.Stop();

                        MessageBox.Show(
                            "YOU WIN!\nTime: " + TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss") +
                            "\nMoves: " + moves
                        );
                    }
                }
                else
                {
                    flipTimer.Start();
                }
            }
            tableGame.Height = tableGame.Width;
        }

        private void lblMatches_Click(object sender, EventArgs e)
        {

        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void lblMoves_Click(object sender, EventArgs e)
        {

        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }
    }
}