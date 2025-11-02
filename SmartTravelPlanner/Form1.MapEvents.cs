using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTravelPlanner
{
    public partial class Form1 : Form
    {
        private void BtnLoadMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Select map file";
            openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openDialog.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openDialog.FilterIndex = 1;

            DialogResult result = openDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openDialog.FileName;

                try
                {
                    graph = CityGraph.LoadFromFile(filePath);
                    Create_Traveler_Form();
                    MessageBox.Show("Map loaded successfully!", "Success");
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"Error loading map: {ex.Message}", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void BtnAddCity_Click(object sender, EventArgs e)
        {
            

            Form dlg1 = new Form();
            dlg1.Text = "Add City Connection";
            dlg1.Size = new Size(300, 200);

            Label lbl1 = new Label();
            lbl1.Text = "City 1:";
            lbl1.Location = new Point(10, 20);
            lbl1.Size = new Size(60, 20);
            dlg1.Controls.Add(lbl1);

            TextBox city1 = new TextBox();
            city1.Location = new Point(80, 20);
            city1.Size = new Size(150, 20);
            dlg1.Controls.Add(city1);

            Label lbl2 = new Label();
            lbl2.Text = "City 2:";
            lbl2.Location = new Point(10, 50);
            lbl2.Size = new Size(60, 20);
            dlg1.Controls.Add(lbl2);

            TextBox city2 = new TextBox();
            city2.Location = new Point(80, 50);
            city2.Size = new Size(150, 20);
            dlg1.Controls.Add(city2);

            Label lbl3 = new Label();
            lbl3.Text = "Distance:";
            lbl3.Location = new Point(10, 80);
            lbl3.Size = new Size(60, 20);
            dlg1.Controls.Add(lbl3);

            TextBox distance = new TextBox();
            distance.Location = new Point(80, 80);
            distance.Size = new Size(150, 20);
            dlg1.Controls.Add(distance);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Location = new Point(50, 120);
            ok.DialogResult = DialogResult.OK;
            dlg1.Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Location = new Point(140, 120);
            cancel.DialogResult = DialogResult.Cancel;
            dlg1.Controls.Add(cancel);

            if (dlg1.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(city1.Text) && !string.IsNullOrWhiteSpace(city2.Text) && !string.IsNullOrWhiteSpace(distance.Text))
                {
                    if (!IsValidName(city1.Text.Trim()))
                    {
                        MessageBox.Show("City 1 name can only contain letters, spaces and hyphens!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!IsValidName(city2.Text.Trim()))
                    {
                        MessageBox.Show("City 2 name can only contain letters, spaces and hyphens!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string distanceText = distance.Text.Trim();

                    if (!int.TryParse(distanceText, out int distanceValue))
                    {
                        MessageBox.Show("Distance must be a valid number!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (distanceValue <= 0)
                    {
                        MessageBox.Show("Distance must be a positive number!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    List<string> connection = new List<string> {
                        city1.Text.Trim(),
                        city2.Text.Trim(),
                        distanceValue.ToString()
                    };

                    if (graph == null) graph = new CityGraph();
                
                    graph.AddCity(connection);
                    
                    Create_Traveler_Form();
                    MessageBox.Show("City connection added successfully!");
                }
                else
                {
                    MessageBox.Show("Please fill in all fields!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void BtnRemoveCity_Click(object sender, EventArgs e)
        {
            if (graph == null || graph.adjacencyList.Count == 0)
            {
                MessageBox.Show("Map is not loaded or empty!");
                return;
            }

            Form removeForm = new Form();
            removeForm.Text = "Remove City";
            removeForm.Size = new Size(300, 150);
            removeForm.StartPosition = FormStartPosition.CenterScreen;

            Label lblCity = new Label();
            lblCity.Text = "Select city to remove:";
            lblCity.Location = new Point(10, 20);
            lblCity.Size = new Size(150, 20);
            removeForm.Controls.Add(lblCity);

            ComboBox cityComboBox = new ComboBox();
            cityComboBox.Location = new Point(10, 45);
            cityComboBox.Size = new Size(250, 20);
            cityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (var kv in graph.adjacencyList)
            {
                cityComboBox.Items.Add(kv.Key.city);
            }

            if (cityComboBox.Items.Count > 0)
                cityComboBox.SelectedIndex = 0;

            removeForm.Controls.Add(cityComboBox);

            Button btnOk = new Button();
            btnOk.Text = "Remove";
            btnOk.Location = new Point(50, 80);
            btnOk.Size = new Size(75, 25);
            btnOk.DialogResult = DialogResult.OK;
            removeForm.Controls.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(140, 80);
            btnCancel.Size = new Size(75, 25);
            btnCancel.DialogResult = DialogResult.Cancel;
            removeForm.Controls.Add(btnCancel);

            removeForm.AcceptButton = btnOk;
            removeForm.CancelButton = btnCancel;

            if (removeForm.ShowDialog() == DialogResult.OK)
            {
                string selectedCity = cityComboBox.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(selectedCity))
                {
                    if (t != null && t.GetLocation() == selectedCity)
                    {
                        MessageBox.Show($"Cannot remove city '{selectedCity}'. Change traveler's location.");
                        return;
                    }

                    if (t != null && t.HasCity(selectedCity))
                    {
                        MessageBox.Show($"Cannot remove city '{selectedCity}'. Remove it from route.");
                        return;
                    }

                    if (graph.adjacencyList.Count <= 2)
                    {
                        MessageBox.Show("Cannot remove city - map must have at least 2 cities!");
                        return;
                    }

                    var isolatedCities = new List<string>();
                    var cityNode = new TNode(selectedCity);

                    if (graph.adjacencyList.ContainsKey(cityNode))
                    {
                        foreach (var edge in graph.adjacencyList[cityNode])
                        {
                            var connectedCity = edge.city;
                            var connectedNode = new TNode(connectedCity);

                            if (graph.adjacencyList.ContainsKey(connectedNode) &&
                                graph.adjacencyList[connectedNode].Count == 1)
                            {
                                isolatedCities.Add(connectedCity);
                            }
                        }
                    }

                    string confirmationMessage = $"Are you sure you want to remove city '{selectedCity}' and all connections to it?";

                    if (isolatedCities.Count > 0)
                    {
                        confirmationMessage += $"\n\nAfter removal, the following cities will be isolated (no connections):\n";
                        foreach (var isolatedCity in isolatedCities)
                        {
                            confirmationMessage += $"- {isolatedCity}\n";
                        }
                        confirmationMessage += $"\nDo you want to remove these isolated cities as well?";
                    }

                    DialogResult confirm = MessageBox.Show(
                        confirmationMessage,
                        "Confirm Removal",
                        isolatedCities.Count > 0 ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes || confirm == DialogResult.No)
                    {
                        bool success = graph.RemoveCity(selectedCity);

                        if (success)
                        {
                            if (confirm == DialogResult.Yes && isolatedCities.Count > 0)
                            {
                                foreach (var isolatedCity in isolatedCities)
                                {
                                    if (t != null && t.GetLocation() == isolatedCity)
                                    {
                                        MessageBox.Show($"Cannot remove isolated city '{isolatedCity}' because traveler is currently there!");
                                        continue;
                                    }

                                    if (t != null && t.HasCity(isolatedCity))
                                    {
                                        MessageBox.Show($"Cannot remove isolated city '{isolatedCity}' because it's in traveler's route!");
                                        continue;
                                    }
                                    graph.RemoveCity(isolatedCity);
                                }
                            }

                            UpdateDestinationComboBox();
                            Create_Traveler_Form();

                            string successMessage = $"City '{selectedCity}' removed successfully!";
                            if (confirm == DialogResult.Yes && isolatedCities.Count > 0)
                            {
                                successMessage += $"\nAlso removed {isolatedCities.Count} isolated cities.";
                            }

                            MessageBox.Show(successMessage, "Success");
                        }
                        else
                        {
                            MessageBox.Show($"Error removing city '{selectedCity}'", "Error");
                        }
                    }
                }
            }
        }

    }
}
