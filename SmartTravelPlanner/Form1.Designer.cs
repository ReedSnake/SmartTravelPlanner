namespace SmartTravelPlanner { 
    partial class Form1 : Form {
        private System.ComponentModel.IContainer components = null;
        public TextBox SetName, SetLocation;
        public ComboBox destination;
        private Button BtnCreateTraveler, BtnLoadTraveler, BtnExit;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            this.ClientSize = new Size(782, 553);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Smart Travel Planner";
            this.Padding = new Padding(20);
            this.MaximizeBox = false;
            

            // Create controls
            var titleLabel = new Label { Text = "Smart Travel Planner", Font = new Font("Georgia", 24, FontStyle.Bold), ForeColor = Color.FromArgb(46, 125, 50), AutoSize = true };
            var subtitleLabel = new Label { Text = "Start your journey", Font = new Font("Georgia", 12), ForeColor = Color.Gray, AutoSize = true };
            var nameLabel = new Label { Text = "Traveler Name *", Font = new Font("Georgia", 10), ForeColor = Color.DimGray, AutoSize = true };
            SetName = new TextBox { Size = new Size(400, 35), Font = new Font("Georgia", 10), PlaceholderText = "Enter traveler name", BorderStyle = BorderStyle.FixedSingle };
            var locationLabel = new Label { Text = "Current Location (optional)", Font = new Font("Georgia", 10), ForeColor = Color.DimGray, AutoSize = true };
            SetLocation = new TextBox { Size = new Size(400, 35), Font = new Font("Georgia", 10), PlaceholderText = "Enter your current city", BorderStyle = BorderStyle.FixedSingle };
            var infoLabel = new Label { Text = "Create new traveler or load existing one", Font = new Font("Georgia", 10), ForeColor = Color.DarkGray, AutoSize = true };

            BtnCreateTraveler = new Button { Text = "Create Traveler", Size = new Size(200, 45), BackColor = Color.FromArgb(46, 125, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            BtnLoadTraveler = new Button { Text = "Load Traveler", Size = new Size(200, 45), BackColor = Color.White, ForeColor = Color.FromArgb(46, 125, 50), FlatStyle = FlatStyle.Flat };
            BtnExit = new Button { Text = "Exit", Size = new Size(150, 40), BackColor = Color.FromArgb(211, 47, 47), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            BtnCreateTraveler.FlatAppearance.BorderSize = 0;
            BtnLoadTraveler.FlatAppearance.BorderColor = Color.FromArgb(46, 125, 50);
            BtnLoadTraveler.FlatAppearance.BorderSize = 2;
            BtnExit.FlatAppearance.BorderSize = 0;

            BtnCreateTraveler.Click += BtnCreateTraveler_Click;
            BtnLoadTraveler.Click += BtnLoadTraveler_Click;
            BtnExit.Click += BtnExit_Click;

            // Add to form
            this.Controls.AddRange(new Control[] { titleLabel, subtitleLabel, nameLabel, SetName, locationLabel, SetLocation, infoLabel, BtnCreateTraveler, BtnLoadTraveler, BtnExit });

            // Position controls
            titleLabel.Location = new Point((this.ClientSize.Width - titleLabel.Width) / 2, 80);
            subtitleLabel.Location = new Point((this.ClientSize.Width - subtitleLabel.Width) / 2, 140);
            int contentStartX = (this.ClientSize.Width - 400) / 2;
            nameLabel.Location = new Point(contentStartX, 190);
            SetName.Location = new Point(contentStartX, 215);
            locationLabel.Location = new Point(contentStartX, 265);
            SetLocation.Location = new Point(contentStartX, 290);
            infoLabel.Location = new Point(contentStartX, 340);
            BtnCreateTraveler.Location = new Point(contentStartX, 370);
            BtnLoadTraveler.Location = new Point(contentStartX + 210, 370);
            BtnExit.Location = new Point(this.ClientSize.Width - 170, this.ClientSize.Height - 60);

            this.ResumeLayout(false);
        }
    }
}
