using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Smash_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<ImageInfo> ImageList = new List<ImageInfo>();

        public MainWindow()
        {
            InitializeComponent();
            //check if images folder exists
            if (!Directory.Exists("images/"))
            {
                Directory.CreateDirectory("images/");
            }

            PopulateList();
        }

        private void PopulateList()
        {
            DirectoryInfo di = new DirectoryInfo("images/");
             FileInfo[] Images = di.GetFiles("*.*");


            //List<string> imagePaths;
            //imagePaths = Directory.GetFiles("Images/", "*.*", SearchOption.AllDirectories).ToList();
            int i = 0;
            foreach (var path in Images)
            {
                Console.WriteLine($"Path: {"images/"}{path.FullName}");
                ImageList.Add(new ImageInfo { ID = i, Photo = path.FullName, Name = path.Name, fileInfo = path });
                i++;
            }

            leftImage.ItemsSource = ImageList;
            leftImage.SelectedIndex = 0;
            rightImage.ItemsSource = ImageList;
            rightImage.SelectedIndex = 0;
        }

        public class ImageInfo
        {
            public int ID { get; set; }
            public string Photo { get; set; }
            public string Name { get; set; }
            public FileInfo fileInfo { get; set; }
        }

        private void leftScoreUp_Click(object sender, RoutedEventArgs e)
        {
            int score = int.Parse(leftScore.Text);
            score++;
            leftScore.Text = score.ToString();
        }

        private void leftScoreDown_Click(object sender, RoutedEventArgs e)
        {
            int score = int.Parse(leftScore.Text);
            score--;
            if (score < 0)
                score = 0;
            leftScore.Text = score.ToString();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //check if out folder exists
            if (!Directory.Exists("out/"))
            {
                Directory.CreateDirectory("out/");
            }

            //left player name
            File.WriteAllText("out/leftName.txt", leftNameTextBox.Text);

            //right player name
            File.WriteAllText("out/rightName.txt", rightNameTextBox.Text);

            //left score
            File.WriteAllText("out/leftScore.txt", leftScore.Text);

            //right score
            File.WriteAllText("out/rightScore.txt", rightScore.Text);

            //left image
            int itemIndex = leftImage.SelectedIndex;
            FileInfo fi = ImageList[itemIndex].fileInfo;
            
            File.Copy(fi.FullName, $"{Directory.GetCurrentDirectory()}\\out\\leftImage{fi.Extension}", true);

            Console.WriteLine($"filePath: {fi.FullName}");
            Console.WriteLine($"fileOutput: {$"{Directory.GetCurrentDirectory()}\\out\\leftImage{fi.Extension}"}");

            //right image
            int itemIndex2 = rightImage.SelectedIndex;
            FileInfo fi2 = ImageList[itemIndex2].fileInfo;
            File.Copy(fi2.FullName, $"out/rightImage{fi2.Extension}", true);

            //bracket name
            File.WriteAllText("out/bracketName.txt", bracketTextBox.Text);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            leftNameTextBox.Text = "left name";
            rightNameTextBox.Text = "right name";

            leftScore.Text = "0";
            rightScore.Text = "0";

            bracketTextBox.Text = "bracket";

            leftImage.SelectedIndex = 0;
            rightImage.SelectedIndex = 0;
        }
    }
}
