using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using ExtensionMethods;

using System.Threading;


namespace PulsarReaded
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDialog;
        ParsekParcer Parcer = new ParsekParcer("DatchikInfo.txt", "KPInfo.txt");
        Regex regex = new Regex(@"([0-9]{4}[0-9ABCDEF]{4})(.pls)", RegexOptions.IgnoreCase);

        public MainWindow()
        {
            InitializeComponent();
            InitializeOpenFileDialog();                      
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                if (chunkCB.IsChecked == true)
                {
                    List<Record> Chunk = new List<Record>();
                    foreach (string s in openFileDialog.FileNames)
                        Chunk.AddRange(Parcer.ParseFile(s));
                    FileManager.Make("Export " + DateTime.Now.ToString().Replace(":", "-"), Chunk, fileType.Text);
                        
                }
                else
                {
                    foreach (string s in openFileDialog.FileNames)
                        FileManager.Make(regex.Match(s).Groups[1].Value, Parcer.ParseFile(s), fileType.Text);                            
                }
                Log.Text += "Экспорт завершен?\n";              
            }
        } 

        private void InitializeOpenFileDialog()
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PLS files (*.pls)|*pls";           
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Укажите файлы для конвертации";
        }


    }
}
