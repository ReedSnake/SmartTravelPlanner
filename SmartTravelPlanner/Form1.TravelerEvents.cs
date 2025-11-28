using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelling;

namespace SmartTravelPlanner {
    public partial class Form1 : Form {
        private void BtnCreateTraveler_Click(object sender, EventArgs e) {
            if (!IsValidText(SetName.Text, "Name")) return;

            string location = SetLocation.Text.Trim();
            if (!string.IsNullOrWhiteSpace(location) && !IsValidText(location, "Location"))
                return;

            string name = SetName.Text.Trim();
            t = new Traveler(name);

            if (!string.IsNullOrWhiteSpace(location))
                t.SetLocation(location);

            Create_Traveler_Form();
        }
        private void BtnChangeName_Click(object sender, EventArgs e) {
            Form dlg1 = new Form();
            TextBox newName = new TextBox();

            newName.PlaceholderText = "Enter new name";
            newName.Location = new Point(10, 10);
            newName.Size = new Size(200, 25);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Location = new Point(30, 60);
            ok.DialogResult = DialogResult.OK;

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Location = new Point(120, 60);
            cancel.DialogResult = DialogResult.Cancel;

            dlg1.Controls.Add(newName);
            dlg1.Controls.Add(ok);
            dlg1.Controls.Add(cancel);

            if (dlg1.ShowDialog() == DialogResult.OK)
            {
                if (!IsValidText(newName.Text, "Name")) return;

                t.Name = newName.Text.Trim();
                Create_Traveler_Form();
            }
        }
        private void BtnChangeLocation_Click(object sender, EventArgs e) {
            Form dlg1 = new Form();
            TextBox newLocation = new TextBox();

            newLocation.PlaceholderText = "Enter new location";
            newLocation.Location = new Point(10, 10);
            newLocation.Size = new Size(200, 25);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Location = new Point(30, 60);
            ok.DialogResult = DialogResult.OK;

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Location = new Point(120, 60);
            cancel.DialogResult = DialogResult.Cancel;

            dlg1.Controls.Add(newLocation);
            dlg1.Controls.Add(ok);
            dlg1.Controls.Add(cancel);

            if (dlg1.ShowDialog() == DialogResult.OK)
            {
                if (!IsValidText(newLocation.Text, "Location")) return;

                t.SetLocation(newLocation.Text.Trim());
                Create_Traveler_Form();
            }
        }
        private void BtnLoadTraveler_Click(object sender, EventArgs e) {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Select the traveler file";
            openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            openDialog.FilterIndex = 1;

            DialogResult result = openDialog.ShowDialog();

            if (result == DialogResult.OK) {
                string filePath = openDialog.FileName;
                try {
                    t = Traveler.LoadFromFile(filePath);
                    Create_Traveler_Form();
                    MessageBox.Show("Traveler loaded successfully!", "Success");
                }
                catch (FileLoadException ex) {
                    MessageBox.Show($"Error loading traveler: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void BtnSaveTraveler_Click(object sender, EventArgs e) {
            if (t == null) {
                MessageBox.Show("Traveler is not created!");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save traveler file";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            saveDialog.FilterIndex = 1;

            DialogResult result = saveDialog.ShowDialog();

            if (result == DialogResult.OK) {
                string filePath = saveDialog.FileName;

                try {
                    t.SaveToFile(filePath);
                    MessageBox.Show("Traveler saved successfully!", "Success");
                }
                catch (Exception ex) {
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void BtnExit_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Exit Application",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                Application.Exit();
            }
        }
    }
}
