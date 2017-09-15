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

        private new MainWindowDataContext DataContext => (MainWindowDataContext) base.DataContext; 

        private void SetDefaultInput()
        {
            var value = DefaultInput();
            txtInput.Text = value;
        }

        private static string DefaultInput()
        {
            var value = new StringBuilder();
            value.AppendLine("//Enter your C# simple enum here");
            value.AppendLine("enum CowboyType : int {");
            value.AppendLine("    Good,");
            value.AppendLine("    Bad,");
            value.AppendLine("    Ugly");
            value.AppendLine("}");
            return value.ToString();
        }
        

        private void btnConvert_OnClick(object sender, RoutedEventArgs e)
        {
            Convert();
        }

        private void Convert()
        {
            var converter = new Converter();
            try
            {
                txtOutput.Text = converter.Convert(txtInput.Text, DataContext.GeneratorOptions);                
            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.ToString();
            }
        }


    }
}
