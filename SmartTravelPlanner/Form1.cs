using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Travelling;
#nullable disable

namespace SmartTravelPlanner {
    public partial class Form1 : Form {
        private Traveler t;
        private CityGraph graph;
        private Logger<string> logger;
        private ComboBox destinationComboBox;

        public Form1() {
            InitializeComponent();
            logger = new Logger<string>();
            logger.Add("Application started.");
            logger.Flush("log.txt");
        }
        private void Create_Traveler_Form() {
            if (t == null) return;
            this.Controls.Clear();
            var header = CreateHeaderLabel($"Traveler: {t.GetName()}", new Point(40, 25));
            this.Controls.Add(header);
            int yPos = 70;
            var personalPanel = CreatePersonalPanel(new Point(40, yPos));
            this.Controls.Add(personalPanel);
            yPos += 100;
            var filePanel = CreateFilePanel(new Point(40, yPos));
            this.Controls.Add(filePanel);
            yPos += 70;
            var routePanel = CreateRoutePanel(new Point(40, yPos));
            this.Controls.Add(routePanel);
            yPos += 100;
            var currentRoutePanel = CreateCurrentRoutePanel(new Point(40, yPos));
            this.Controls.Add(currentRoutePanel);
            yPos += 130;
            var mapPanel = CreateMapPanel(new Point(40, yPos));
            this.Controls.Add(mapPanel);
            var btnExit = CreateExitButton(new Point(this.ClientSize.Width - 120, this.ClientSize.Height - 50));
            this.Controls.Add(btnExit);
        }

        private Label CreateHeaderLabel(string text, Point location) {
            return new Label {
                Text = text,
                Font = new Font("Georgia", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 125, 50),
                AutoSize = true,
                Location = location
            };
        }
        private void UpdateDestinationComboBox() {
            if (destinationComboBox != null) {
                var autoComplete = new AutoCompleteStringCollection();
                destinationComboBox.Items.Clear();

                if (graph?.adjacencyList?.Count > 0) {
                    foreach (var kv in graph.adjacencyList) {
                        if (!destinationComboBox.Items.Contains(kv.Key.city)) {
                            destinationComboBox.Items.Add(kv.Key.city);
                            autoComplete.Add(kv.Key.city);
                        }
                    }
                }
                destinationComboBox.AutoCompleteCustomSource = autoComplete;
                destinationComboBox.Text = "";
            }
        }
        private Panel CreatePersonalPanel(Point location) {
            var panel = new Panel {
                Size = new Size(600, 90),
                Location = location,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var nameLabel = new Label {
                Text = $"Name: {t.GetName()}",
                Location = new Point(25, 20),
                Size = new Size(250, 25),
                Font = new Font("Georgia", 11F, FontStyle.Regular)
            };

            var btnChangeName = CreateChangeButton("Change Name", new Point(25, 50));
            btnChangeName.Click += BtnChangeName_Click;

            var locLabel = new Label {
                Text = $"Location: {(string.IsNullOrEmpty(t.GetLocation()) ? "Not set" : t.GetLocation())}",
                Location = new Point(300, 20),
                Size = new Size(250, 25),
                Font = new Font("Georgia", 11F, FontStyle.Regular)
            };

            var btnChangeLoc = CreateChangeButton("Change Location", new Point(300, 50));
            btnChangeLoc.Click += BtnChangeLocation_Click;

            panel.Controls.AddRange(new Control[] { nameLabel, btnChangeName, locLabel, btnChangeLoc });
            return panel;
        }

        private Panel CreateFilePanel(Point location) {
            var panel = new Panel {
                Size = new Size(600, 60),
                Location = location,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnSave = CreatePrimaryButton("Save Traveler", new Point(25, 15), new Size(150, 30), 9F);
            btnSave.Click += BtnSaveTraveler_Click;

            var btnLoadTraveler = CreateChangeButton("Load Traveler", new Point(190, 15));
            btnLoadTraveler.Click += BtnLoadTraveler_Click;

            var btnLoadMap = CreatePrimaryButton("Load Map", new Point(355, 15), new Size(150, 30), 9F);
            btnLoadMap.Click += BtnLoadMap_Click;

            panel.Controls.AddRange(new Control[] { btnSave, btnLoadTraveler, btnLoadMap });
            return panel;
        }

        private Panel CreateRoutePanel(Point location) {
            var panel = new Panel {
                Size = new Size(600, 90),
                Location = location,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var destLabel = new Label {
                Text = "Destination:",
                Location = new Point(25, 20),
                Size = new Size(100, 25),
                Font = new Font("Georgia", 10F, FontStyle.Regular)
            };

            destinationComboBox = new ComboBox {
                Location = new Point(130, 18),
                Size = new Size(250, 25),
                Font = new Font("Georgia", 9F, FontStyle.Regular),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.CustomSource
            };

            destinationComboBox.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                    destinationComboBox.DroppedDown = false;
            };

            UpdateDestinationComboBox();

            var btnFindRoute = CreatePrimaryButton("Find Route", new Point(400, 15), new Size(150, 30), 9F);
            btnFindRoute.Click += BtnGetRoute_Click;

            var currentLocInfo = new Label {
                Text = string.IsNullOrEmpty(t.GetLocation()) ? "Set your location first" : $"From: {t.GetLocation()}",
                Location = new Point(25, 55),
                Size = new Size(350, 25),
                ForeColor = Color.DarkGray,
                Font = new Font("Georgia", 9F, FontStyle.Regular)
            };
            panel.Controls.AddRange(new Control[] { destLabel, destinationComboBox, btnFindRoute, currentLocInfo });
            return panel;
        }

        private Panel CreateCurrentRoutePanel(Point location) {
            var panel = new Panel {
                Size = new Size(600, 120),
                Location = location,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var routeDisplay = new TextBox {
                Location = new Point(25, 20),
                Size = new Size(550, 50),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Georgia", 9F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(232, 245, 233),
                Text = string.IsNullOrEmpty(t.GetRoute()) ? "No route planned" : t.GetRoute()
            };
            var distanceLabel = CreateDistanceLabel();

            panel.Controls.AddRange(new Control[] { routeDisplay, distanceLabel });
            return panel;
        }
        private Panel CreateMapPanel(Point location) {
            var panel = new Panel {
                Size = new Size(600, 60),
                Location = location,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnAddCity = CreateChangeButton("Add City", new Point(25, 15));
            btnAddCity.Click += BtnAddCity_Click;

            var btnRemoveCity = CreateRemoveButton("Remove City", new Point(190, 15));
            btnRemoveCity.Click += BtnRemoveCity_Click;

            panel.Controls.AddRange(new Control[] { btnAddCity, btnRemoveCity });
            return panel;
        }

        private Button CreateChangeButton(string text, Point location) {
            return new Button { 
                Text = text,
                Location = location,
                Size = new Size(140, 30),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(46, 125, 50),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderColor = Color.FromArgb(46, 125, 50),
                    BorderSize = 1,
                    MouseOverBackColor = Color.FromArgb(232, 245, 233)
                },
                Font = new Font("Georgia", 9F, FontStyle.Regular)
            };
        }

        private Button CreatePrimaryButton(string text, Point location, Size size, float fontSize) {
            return new Button {
                Text = text,
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = Color.FromArgb(76, 175, 80)
                },
                Font = new Font("Georgia", fontSize, FontStyle.Regular)
            };
        }

        private Button CreateRemoveButton(string text, Point location) {
            return new Button {
                Text = text,
                Location = location,
                Size = new Size(150, 30),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(198, 40, 40),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderColor = Color.FromArgb(198, 40, 40),
                    BorderSize = 1,
                    MouseOverBackColor = Color.FromArgb(255, 235, 238)
                },
                Font = new Font("Georgia", 9F, FontStyle.Regular)
            };
        }

        private Button CreateExitButton(Point location) {
            var button = new Button {
                Text = "Exit",
                Location = location,
                Size = new Size(80, 35),
                Font = new Font("Georgia", 9F, FontStyle.Regular),
                BackColor = Color.FromArgb(211, 47, 47),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
            BorderSize = 0,
            MouseOverBackColor = Color.FromArgb(198, 40, 40),
            MouseDownBackColor = Color.FromArgb(183, 28, 28)
        },
            };
            button.Click += BtnExit_Click;
            return button;
        }

        private Label CreateDistanceLabel() {
            var label = new Label {
                Location = new Point(25, 80),
                Size = new Size(400, 25),
                Font = new Font("Georgia", 10F, FontStyle.Bold)
            };

            if (t != null && graph != null && !string.IsNullOrEmpty(t.GetRoute())) {
                var pathList = new List<string>(t.GetRoute().Split(" -> ", StringSplitOptions.RemoveEmptyEntries));
                if (pathList.Count > 1) {
                    label.Text = $"Distance: {graph.GetPathDistance(pathList)} km";
                    label.ForeColor = Color.FromArgb(46, 125, 50);
                    return label;
                }
            }
            label.Text = graph == null ? "Load map to see distance" : "Distance: -";
            label.ForeColor = Color.Gray;
            return label;
        }
    }
}
