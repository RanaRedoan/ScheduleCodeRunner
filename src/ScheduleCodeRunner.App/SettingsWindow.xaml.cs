using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.App;

public partial class SettingsWindow : Window
{
    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        CurrentSettings = new AppSettings
        {
            PythonExecutablePath = settings?.PythonExecutablePath ?? string.Empty,
            RExecutablePath = settings?.RExecutablePath ?? string.Empty,
            StataExecutablePath = settings?.StataExecutablePath ?? string.Empty,
            BatchShellPath = settings?.BatchShellPath ?? string.Empty
        };

        PythonPathTextBox.Text = CurrentSettings.PythonExecutablePath;
        RPathTextBox.Text = CurrentSettings.RExecutablePath;
        StataPathTextBox.Text = CurrentSettings.StataExecutablePath;
        BatchShellPathTextBox.Text = CurrentSettings.BatchShellPath;
    }

    public AppSettings CurrentSettings { get; private set; }

    private void BrowsePythonButton_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(PythonPathTextBox);
    }

    private void BrowseRButton_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(RPathTextBox);
    }

    private void BrowseStataButton_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(StataPathTextBox);
    }

    private void BrowseBatchShellButton_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(BatchShellPathTextBox);
    }

    private void BrowseInto(TextBox targetTextBox)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Executable files|*.exe|All files|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) == true)
        {
            targetTextBox.Text = dialog.FileName;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ValidationTextBlock.Text = string.Empty;
        var pythonPath = PythonPathTextBox.Text.Trim();
        var rPath = RPathTextBox.Text.Trim();
        var stataPath = StataPathTextBox.Text.Trim();
        var batchShellPath = BatchShellPathTextBox.Text.Trim();

        var validationError = ValidatePath(pythonPath, "Python");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ValidationTextBlock.Text = validationError;
            return;
        }

        validationError = ValidatePath(rPath, "R");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ValidationTextBlock.Text = validationError;
            return;
        }

        validationError = ValidatePath(stataPath, "Stata");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ValidationTextBlock.Text = validationError;
            return;
        }

        validationError = ValidatePath(batchShellPath, "Batch/CMD shell");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ValidationTextBlock.Text = validationError;
            return;
        }

        CurrentSettings = new AppSettings
        {
            PythonExecutablePath = pythonPath,
            RExecutablePath = rPath,
            StataExecutablePath = stataPath,
            BatchShellPath = batchShellPath
        };

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static string ValidatePath(string path, string runtimeName)
    {
        return !string.IsNullOrWhiteSpace(path) && !File.Exists(path)
            ? $"Selected {runtimeName} executable was not found."
            : string.Empty;
    }
}
