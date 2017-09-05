using System;
using System.Text;
using System.Windows;

namespace StronglyTypedEnumConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SetDefaultInput();
            btnConvert.Focus();
        }

        private void SetDefaultInput()
        {
            var value = DefaultInput();
            txtInput.Text = value;
        }

        private static string DefaultInput()
        {
            var value = new StringBuilder();
            value.AppendLine("//Enter your C# simple enum here");
            value.AppendLine("enum CowboyType {");
            value.AppendLine("    Good,");
            value.AppendLine("    Bad,");
            value.AppendLine("    Ugly");
            value.AppendLine("}");
            return value.ToString();
        }

        private void txtInput_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (txtInput.Text == DefaultInput())
                txtInput.Text = string.Empty;
        }
        

        private void btnConvert_OnClick(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        private bool Convert()
        {
            var converter = new Converter();
            try
            {
                var options = new GeneratorOptions {AdditionPriority = AdditionPriority};
                txtOutput.Text = converter.Convert(txtInput.Text, options);                
            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.ToString();
                return false;
            }

            return true;
        }

        private AdditionPriority AdditionPriority => 
            (MemberAddition.IsChecked ?? false) 
            ? AdditionPriority.Members 
            : AdditionPriority.Properties;

        private void btnConvertClipboard_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Text = Clipboard.GetText();
            if (Convert())
                Clipboard.SetText(txtOutput.Text);
        }


    }
}
