using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOOGLE_APPLICATION_CREDENTIALS_Setup
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Browse the JSON file that contains the service account key",
                Filter = "Json File (*.json)|*.json",
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            using (dialog)
            {
                try
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        AddToEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", dialog.FileName, EnvironmentVariableTarget.User);
                        // Exit with exit code 0 in case of success.
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            Environment.Exit(-1); /* no selection of the JSON file will be considered as failure */
        }

        static void AddToEnvironmentVariable(string variable, string newValue, EnvironmentVariableTarget target)
        {
            Environment.SetEnvironmentVariable(variable, newValue, EnvironmentVariableTarget.Process);
            if (target != EnvironmentVariableTarget.Process)
                Environment.SetEnvironmentVariable(variable, newValue, target);
        }
    }
}
