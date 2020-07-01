using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
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

namespace FG_Stream_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CharacterImageInfo> CharacterImageList = new List<CharacterImageInfo>();

        public MainWindow()
        {
            InitializeComponent();

            //check if images folder exists, create it if it doesn't
            if (!Directory.Exists("images/"))
            {
                Directory.CreateDirectory("images/");
            }

            PopulateCharacterImages();
        }

        private void PopulateCharacterImages()
        {
            DirectoryInfo di = new DirectoryInfo("images/");
            FileInfo[] Images = di.GetFiles("*.*");

            int i = 0;
            foreach (var path in Images)
            {
                //Console.WriteLine($"Path: {"images/"}{path.FullName}");
                string newName = path.Name.Split('.')[0];
                CharacterImageList.Add(new CharacterImageInfo { ID = i, Photo = path.FullName, Name = newName, fileInfo = path });
                i++;
            }

            leftImage.ItemsSource = CharacterImageList;
            leftImage.SelectedIndex = 0;
            rightImage.ItemsSource = CharacterImageList;
            rightImage.SelectedIndex = 0;
        }

        public struct CharacterImageInfo
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

        private void rightScoreUp_Click(object sender, RoutedEventArgs e)
        {
            int score = int.Parse(rightScore.Text);
            score++;
            rightScore.Text = score.ToString();
        }

        private void rightScoreDown_Click(object sender, RoutedEventArgs e)
        {
            int score = int.Parse(rightScore.Text);
            score--;
            if (score < 0)
                score = 0;
            rightScore.Text = score.ToString();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //check if out folder exists, create it if it doesn't
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

            //only export images if there is something to copy and the option is checked
            if(CharacterImageList.Count != 0  && exportImages.IsChecked.GetValueOrDefault())
            {
                //left image
                int itemIndex = leftImage.SelectedIndex;
                FileInfo fi = CharacterImageList[itemIndex].fileInfo;
                string path = fi.FullName;
                //errorsTextBlock.Text = itemIndex.ToString() + " " + path;
                File.Copy(path, $"out/leftimage{fi.Extension}", true);

                //right image
                itemIndex = rightImage.SelectedIndex;
                fi = CharacterImageList[itemIndex].fileInfo;
                File.Copy(fi.FullName, $"out/rightImage{fi.Extension}", true);
            }
            
            //bracket name
            File.WriteAllText("out/bracketName.txt", bracketTextBox.Text);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            leftNameTextBox.Text = "left name";
            rightNameTextBox.Text = "right name";

            leftScore.Text = "0";
            rightScore.Text = "0";

            leftImage.SelectedIndex = 0;
            rightImage.SelectedIndex = 0;

            bracketTextBox.Text = "bracket";

            apiText.Text = "";
            errorsTextBlock.Text = "";
        }

        private void getMatch_Click(object sender, RoutedEventArgs e)
        {
            _ = GetAPIInfo(apiText.Text);
        }

        private void exportImages_Checked(object sender, RoutedEventArgs e)
        {
            leftImage.Visibility = Visibility.Visible;
            rightImage.Visibility = Visibility.Visible;
        }

        private void exportImages_Unchecked(object sender, RoutedEventArgs e)
        {
            leftImage.Visibility = Visibility.Collapsed;
            rightImage.Visibility = Visibility.Collapsed;
        }

        #region API Stuff

        async Task GetAPIInfo(string id)
        {
            if (id == "") return;

            string sId = id;

            //check if id starts with h, indicating its a link. if so, only get the id part
            if(sId[0] == 'h')
            {
                string[] split = id.Split('/');
                sId = split[split.Length - 1];
            }

            //check the new id is actually a number
            if (!int.TryParse(sId, out _))
            {
                errorsTextBlock.Text = "Invalid link or set id";
                return;
            }

            try
            {
                SmashGGInfoModel info = await ApiHelper.GetSetInfo(sId);

                SetInfo(info);      
            }
            catch (Exception e)
            {
                errorsTextBlock.Text = e.Message;
            }
            finally
            {

            }
        }

        private void SetInfo(SmashGGInfoModel info)
        {
            if(info.set == null)
            {
                errorsTextBlock.Text = "Could not find set with that id";
                return;
            }

            errorsTextBlock.Text = "";


            //bracket name
            string bracketName = info.set.fullRoundText;

            //player 1 name
            string p1Name = info.set.slots[0].entrant.name;

            //player 2 name
            string p2Name = info.set.slots[1].entrant.name;

            //player 1 score
            float p1Score = info.set.slots[0].standing.stats.score.value;

            //player 2 score
            float p2Score = info.set.slots[1].standing.stats.score.value;

            leftNameTextBox.Text = p1Name;
            rightNameTextBox.Text = p2Name;
            leftScore.Text = p1Score.ToString();
            rightScore.Text = p2Score.ToString();
            bracketTextBox.Text = bracketName;
        }

        #endregion

        
    }
}

