using ModelsApi;
using ShopClient.ViewModels.Add;
using Spire.Barcode;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ShopClient.Views.Add
{
    /// <summary>
    /// Логика взаимодействия для AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        public AddProduct()
        {
            InitializeComponent();
            DataContext = new AddProductViewModel(null);
            GenerateBarcode("0", "0");
        }
        public AddProduct(ProductApi  product)
        {
            InitializeComponent();
            DataContext = new AddProductViewModel(product);
            GenerateBarcode(product.Barcode.ToString(), product.Article.ToString());
        }
        private void GenerateBarcode(string data, string member)
        {
            BarcodeSettings st = new BarcodeSettings();
            st.Code128SetMode = Code128SetMode.Auto;
            //st.Data = AddProduct.Barcode.ToString();
            st.Data = data;
            st.TextFont = new System.Drawing.Font("Arial", 1);
            st.ShowTopText = false;
            st.ShowTextOnBottom = false;
            st.HasBorder = false;
            BarCodeGenerator bg = new BarCodeGenerator(st);
            using (MemoryStream ms = new MemoryStream())
            {
              if (File.Exists(Environment.CurrentDirectory + $"/{member}_Barcode.png"))
                    File.Delete(Environment.CurrentDirectory + $"/{member}_Barcode.png");
            }
            bg.GenerateImage().Save(member + "_Barcode.png");
            img.Source = GetImageFromPath(Environment.CurrentDirectory  + $"/{member}_Barcode.png");
        }
        private BitmapImage GetImageFromPath(string url)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(url, UriKind.Absolute);
            img.EndInit();
            return img;
        }
    }
}
