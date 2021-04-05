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
        readonly List<CharacterImageInfo> CharacterImageList = new List<CharacterImageInfo>();

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists("auth.txt"))
            {
                File.WriteAllText("auth.txt", $"putTheAuthCodeHere{Environment.NewLine}{Environment.NewLine}https://developer.smash.gg/docs/authentication");
            }

            //check if out folder exists, create it if it doesn't
            if (!Directory.Exists("out/"))
            {
                Directory.CreateDirectory("out/");
            }

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

            p1Image.ItemsSource = CharacterImageList;
            p1Image.SelectedIndex = 0;
            p2Image.ItemsSource = CharacterImageList;
            p2Image.SelectedIndex = 0;
        }

        #region Events

        private void p1ScoreUp_Click(object sender, RoutedEventArgs e)
        {
            UpdateScoreText(p1Score, 1);
        }

        private void p1ScoreDown_Click(object sender, RoutedEventArgs e)
        {
            UpdateScoreText(p1Score, -1);
        }

        private void p2ScoreUp_Click(object sender, RoutedEventArgs e)
        {
            UpdateScoreText(p2Score, 1);
        }

        private void p2ScoreDown_Click(object sender, RoutedEventArgs e)
        {
            UpdateScoreText(p2Score, -1);
        }


        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            ExportText();

            ExportScore();

            //only export images if there is something to copy and the option is checked
            if(CharacterImageList.Count != 0  && exportImages.IsChecked.GetValueOrDefault())
            {
                ExportImages();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            p1TextBox.Text = "left name";
            p2TextBox.Text = "right name";

            p1Score.Text = "0";
            p2Score.Text = "0";

            p1Image.SelectedIndex = 0;
            p2Image.SelectedIndex = 0;

            bracketTextBox.Text = "bracket";

            apiText.Text = "";
            errorsTextBlock.Text = "";
        }

        private void getMatch_Click(object sender, RoutedEventArgs e)
        {
            //set the auth for the api.
            string auth = File.ReadAllLines("auth.txt")[0];
            ApiHelper.SetAuthCode(auth);

            _ = GetAPIInfo(apiText.Text);
        }

        private void exportImages_Checked(object sender, RoutedEventArgs e)
        {
            p1Image.Visibility = Visibility.Visible;
            p2Image.Visibility = Visibility.Visible;
        }

        private void exportImages_Unchecked(object sender, RoutedEventArgs e)
        {
            p1Image.Visibility = Visibility.Collapsed;
            p2Image.Visibility = Visibility.Collapsed;
        }

        #endregion

        private void UpdateScoreText(TextBlock text, int amount)
        {
            int score = int.Parse(text.Text);
            score += amount;
            if (score < 0)
                score = 0;

            text.Text = score.ToString();

            if (autoUpdateScore.IsChecked.GetValueOrDefault())
            {
                ExportScore();
            }
        }

        private void ExportScore()
        {
            //left score
            File.WriteAllText("out/p1Score.txt", p1Score.Text);

            //right score
            File.WriteAllText("out/p2Score.txt", p2Score.Text);
        }

        private void ExportImages()
        {
            //left image
            int itemIndex = p1Image.SelectedIndex;
            FileInfo fi = CharacterImageList[itemIndex].fileInfo;
            string path = fi.FullName;
            //errorsTextBlock.Text = itemIndex.ToString() + " " + path;
            File.Copy(path, $"out/p1Image{fi.Extension}", true);

            //right image
            itemIndex = p2Image.SelectedIndex;
            fi = CharacterImageList[itemIndex].fileInfo;
            File.Copy(fi.FullName, $"out/p2Image{fi.Extension}", true);
        }

        private void ExportText()
        {
            //left player name
            File.WriteAllText("out/p1Name.txt", p1TextBox.Text);

            //right player name
            File.WriteAllText("out/p2Name.txt", p2TextBox.Text);

            //bracket name
            File.WriteAllText("out/bracketName.txt", bracketTextBox.Text);
        }

        #region API Stuff

        async Task GetAPIInfo(string id)
        {
            if (id == "") return;

            string sId = id;

            //check if id starts with h, indicating its a link (starting with http). if so, only get the id part
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

                //SetInfo(info);
            }
            catch (Exception e)
            {
                errorsTextBlock.Text = e.Message;
            }
            finally
            {
                SmashGGInfoModel info = await ApiHelper.GetSetInfo(sId);

                SetInfo(info);
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
            //float p1Score = info.set.slots[0].standing.stats.score.value;

            //player 2 score
            //float p2Score = info.set.slots[1].standing.stats.score.value;

            p1TextBox.Text = p1Name;
            p2TextBox.Text = p2Name;
            this.p1Score.Text = "0";
            this.p2Score.Text = "0";
            bracketTextBox.Text = bracketName;
        }

        #endregion

        
    }
}

