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
        private void BtnGetRoute_Click(object sender, EventArgs e) {
            if (t == null || graph == null || destinationComboBox == null) {
                MessageBox.Show("Traveler or map is not loaded!");
                return;
            }

            string dest = destinationComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(dest)) {
                try {
                    t.PlanRouteTo(dest, graph);
                } catch (Exception ex) {
                    MessageBox.Show($"{ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Create_Traveler_Form();
            }
        }
        private bool IsValidName(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            foreach (char c in input) {
                if (!char.IsLetter(c) && c != ' ' && c != '-')
                    return false;
            }
            return true;
        }
        private bool IsValidText(string input, string type) {
            if (type == "Location" && string.IsNullOrWhiteSpace(input)) {
                return true;
            }

            if (string.IsNullOrWhiteSpace(input)) {
                MessageBox.Show($"Please enter a {type}!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!IsValidName(input)) {
                MessageBox.Show($"{type} can only contain letters, spaces and hyphens!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!(input.Length > 0 && input.Length < 19)) {
                MessageBox.Show($"{type} must be between 2 and 19 characters!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
