using System;
using System.Collections.Generic;
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace WPFRESTClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string picPath =null;

        public MainWindow()
        {
            //Run Client


            InitializeComponent();
        }

        public HttpClient client{ get; set; }

        private void TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void AddPic_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog fd = new OpenFileDialog();

            if (fd.ShowDialog() == true)
            {
                btnUpdate.IsEnabled = true;
                btnCreate.IsEnabled = true;
                picPath = fd.FileName;

               imgToAdd.Source = new BitmapImage(new Uri(picPath));
            }

        }

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54321/");
            HttpResponseMessage response;
            HttpResponseMessage response2 = await client.GetAsync("api/Data/");
            List<Items> items = await response2.Content.ReadAsAsync<List<Items>>();
            Items lastItem = items.Last<Items>();

            Items itm = new Items();
            itm.Id = (lastItem.Id+1);
            itm.name = txbName.Text;
            
            // Work with Image
            byte[] data;
            data = System.IO.File.ReadAllBytes(picPath);
            
            itm.image = Convert.ToBase64String(data);

            response = await client.PostAsJsonAsync("api/Data", itm);
        }

        private async void BtnRead_Click(object sender, RoutedEventArgs e)
        {
          

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54321/");
            HttpResponseMessage response = await client.GetAsync("api/Data/");
            
            List<Items> items = await response.Content.ReadAsAsync<List<Items>>();
            
            // Not Good, Clear field
            dataGrid.Children.RemoveRange(14, 1000);

            int marginLeft = 1;
            int marginTop = 1;
            for (int i = 0; i < items.Count; ++i)
            {
                
                //Span labels
                //MessageBox.Show(items[i].Id.ToString() + " " + items[i].name);
                Label btnD = new Label();
                btnD.Name = "lblTemp";
                btnD.Width = 110;
                btnD.Height = 25;
                btnD.Content = items[i].name+" id:"+items[i].Id.ToString();
                btnD.Background = Brushes.Azure;
                btnD.VerticalAlignment = VerticalAlignment.Top;
                btnD.HorizontalAlignment = HorizontalAlignment.Left;
                Thickness ml = btnD.Margin;
                ml.Left = 135* marginLeft;
                btnD.Margin = ml;
                Thickness mt = btnD.Margin;
                mt.Top = 15+ marginTop;
                btnD.Margin = mt;
                dataGrid.Children.Add(btnD);

                //Span Images
                //image from string
                byte[] image = Convert.FromBase64String(items[i].image);
                BitmapImage logo = new BitmapImage();
                logo = LoadImage(image);

                Image img = new Image();
                img.Name = "imgTemp";
                img.Width = 110;
                img.Height= 100;

                img.VerticalAlignment = VerticalAlignment.Top;
                img.HorizontalAlignment = HorizontalAlignment.Left;
                Thickness il = img.Margin;
                il.Left = 135 * marginLeft;
                img.Margin = il;
                Thickness it = img.Margin;
                it.Top = 35 + marginTop;
                img.Margin = it;

                img.Source = logo;

                dataGrid.Children.Add(img);
                if (marginLeft == 3)
                {
                    marginLeft = 0;
                    marginTop+=150;
                }
                    marginLeft++;
            }
        }
        public async void BtnGetById_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54321/");
            HttpResponseMessage response = await client.GetAsync("api/Data/"+tbInputId.Text);

            //Items itm = await response.Content.ReadAsAsync<Items>();
            Items itm = await response.Content.ReadAsAsync<Items>();
            lblImageName.Content = itm.name;

            // Get Image
            if (itm.image != null)
            {
                byte[] image = Convert.FromBase64String(itm.image);
                BitmapImage logo = new BitmapImage();
                logo = LoadImage(image);
                imgImagetGetById.Source = logo;
            }
        }
        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string s = txbUpdateId.Text;
            int n;
            if (!Int32.TryParse(s, out n))
            {
                MessageBox.Show("You inputted not number.\nIn Id update box.");
            }
            else
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:54321/");
                HttpResponseMessage response;

                Items itm = new Items();
                itm.Id = Convert.ToInt32(txbUpdateId.Text);
                itm.name = txbName.Text;

                // Work with Image
                byte[] data;
                data = System.IO.File.ReadAllBytes(picPath);

                itm.image = Convert.ToBase64String(data);

                response = await client.PutAsJsonAsync("api/Data/"+ txbUpdateId.Text, itm);
            }
        }
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string s = txbDeleteId.Text;
            int n;
            if (!Int32.TryParse(s, out n))
            {
                MessageBox.Show("You inputted not number.\nIn Id delete box.");
            }
            else
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:54321/");
                HttpResponseMessage response;

                response = await client.DeleteAsync("api/Data/" + txbDeleteId.Text);
            }
        }
        //   FOR PICTURES
        // CReate an Image
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        // Regular Expressions for Int fields
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }

}
