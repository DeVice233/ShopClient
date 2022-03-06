using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using ModelsApi;
using ShopClient.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using ShopClient.Views.Add;

namespace ShopClient.ViewModels.Add
{
    public class AddProductViewModel : BaseViewModel
    {
        private List<string> timeStamps;
        public List<string> TimeStamps
        {
            get => timeStamps;
            set
            {
                timeStamps = value;
                SignalChanged();
            }
        }
        private string selectedTimeStamp;
        public string SelectedTimeStamp
        {
            get => selectedTimeStamp;
            set
            {
                if (value != selectedTimeStamp)
                {
                    selectedTimeStamp = value;
                    SignalChanged();
                    Chart(AddProduct);
                }
            }
        }

        public ProductApi AddProduct { get; set; }

        private BitmapImage imageProduct;
        public BitmapImage ImageProduct
        {
            get => imageProduct;
            set
            {
                imageProduct = value;
                SignalChanged();
            }
        }
        private List<FabricatorApi> fabricators;
        public List<FabricatorApi> Fabricators
        {
            get => fabricators;
            set
            {
                fabricators = value;
                SignalChanged();
            }
        }
        private List<UnitApi> units;
        public List<UnitApi> Units
        {
            get => units;
            set
            {
                units = value;
                SignalChanged();
            }
        }
        private List<ProductTypeApi> productTypes;
        public List<ProductTypeApi> ProductTypes
        {
            get => productTypes;
            set
            {
                productTypes = value;
                SignalChanged();
            }
        }
        private FabricatorApi selectedFabricator;
        public FabricatorApi SelectedFabricator
        {
            get => selectedFabricator;
            set
            {
                selectedFabricator = value;
                SignalChanged();
            }
        }
        private ProductTypeApi selectedProductType;
        public ProductTypeApi SelectedProductType
        {
            get => selectedProductType;
            set
            {
                selectedProductType = value;
                SignalChanged();
            }
        }
        private UnitApi selectedUnit;
        public UnitApi SelectedUnit
        {
            get => selectedUnit;
            set
            {
                selectedUnit = value;
                SignalChanged();
            }
        }
        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        public CustomCommand SelectImage { get; set; }
        public CustomCommand UpdateChart { get; set; }

        public List<ProductApi> Products;
        private List<ProductCostHistoryApi> ProductCostHistories = new List<ProductCostHistoryApi>();
        private List<ProductCostHistoryApi> ThisProductCostHistory = new List<ProductCostHistoryApi>();
        ChartValues<double> Retail = new ChartValues<double>();
        ChartValues<double> Wholesale = new ChartValues<double>();
        List<DateTime> Dates = new List<DateTime>();
        DateTime[] DatesArray; 

        public AddProductViewModel(ProductApi product)
        {
            Units = new List<UnitApi>();
            ProductTypes = new List<ProductTypeApi>();
            Fabricators = new List<FabricatorApi>();

            TimeStamps = new List<string>();
            TimeStamps.AddRange(new string[] { "За год", "За месяц", "За все время"});
            SelectedTimeStamp = TimeStamps.Last();

            if (product == null)
            {
                AddProduct = new ProductApi { Image="picture.JPG" };
                GetList(product);
            }
            else
            {
                AddProduct = new ProductApi
                {
                    Id = product.Id,
                    Title = product.Title,
                    Description = product.Description,
                    Article = product.Article,
                    Barcode = product.Barcode,
                    Image = product.Image,
                    IdFabricator = product.IdFabricator,
                    deleted_at = product.deleted_at,
                    IdUnit = product.IdUnit,
                    IdProductType = product.IdProductType,
                    RetailPrice = product.RetailPrice,
                    MinCount = product.MinCount,
                    WholesalePrice = product.WholesalePrice
                };

                GetList(product);

                if ( product.Image == null)
                {
                    product.Image = "picture.JPG";
                }
                if (product.Image.Length == 0)
                {
                    product.Image = "picture.JPG";
                }
                ImageProduct = GetImageFromPath(Environment.CurrentDirectory + "/Products/" + product.Image);
            }


            Save = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Сохранить изменения?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        
                        AddProduct.IdProductType = SelectedProductType.Id;
                        AddProduct.IdFabricator = SelectedFabricator.Id;
                        AddProduct.IdUnit = SelectedUnit.Id;
                        if (AddProduct.Id == 0)
                        { 
                            AddProduct.WholesalePrice = 0;
                            AddProduct.RetailPrice = 0;
                            Add(AddProduct);
                        }

                        else
                        {
                            Edit(AddProduct);
                        }

                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.DataContext == this) CloseWin(window);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    };
                }
                else return;
            });

            Cancel = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Отменить изменения?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            CloseWin(window);
                        }
                    }
                else return;
            });

            UpdateChart = new CustomCommand(() =>
            {
                Chart(product);
            });

            SelectImage = new CustomCommand(() =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    try
                    {
                        var info = new FileInfo(ofd.FileName);
                        ImageProduct = GetImageFromPath(ofd.FileName);
                        AddProduct.Image = $"{info.Name}";
                        var newPath = Environment.CurrentDirectory + "/Products/" + AddProduct.Image;
                        File.Copy(ofd.FileName, newPath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });

            BuildChart();
        }

        private bool IsValid(ProductApi product) //сделать валидацию
        {
            foreach(ProductApi product1 in Products)
            {
                if (product.Article == product1.Article)
                {
                    MessageBox.Show("Артикул должен быть уникальным!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        
            return true;
        }

        public SeriesCollection SeriesCollection { get; set; }
        private string[] labels;
        public string[] Labels 
        {
            get => labels;
            set
            {
                labels = value;
                SignalChanged();
            }
        }

        public Func<double, string> YFormatter { get; set; }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }


        private async Task Add(ProductApi product)
        {
            var id = await Api.PostAsync<ProductApi>(product, "Product");
        }
        private async Task Edit(ProductApi product)
        {
            var id = await Api.PutAsync<ProductApi>(product, "Product");
        }
        private async Task GetList(ProductApi product)
        {
            ProductCostHistories = await Api.GetListAsync<List<ProductCostHistoryApi>>("ProductCostHistory");
            ProductTypes = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
            if (product != null)
            {
                ThisProductCostHistory = ProductCostHistories.Where(c => c.IdProduct == product.Id).ToList();
                PrepareChart(ThisProductCostHistory);
            }
            Fabricators = await Api.GetListAsync<List<FabricatorApi>>("Fabricator");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            Products = await Api.GetListAsync<List<ProductApi>>("Product");
            if (product == null)
            {
                SelectedUnit = Units.First();
                SelectedFabricator = Fabricators.First();
                SelectedProductType = ProductTypes.First();
            }
            else
            {
                SelectedFabricator = Fabricators.First(s => s.Id == product.IdFabricator);
                SelectedUnit = Units.First(s => s.Id == product.IdUnit);
                SelectedProductType = ProductTypes.First(s => s.Id == product.IdProductType);
            }
            SignalChanged("Fabricators");
            SignalChanged("Units");
            SignalChanged("ProductTypes");
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
        public void Chart(ProductApi product)
        {
            List<ProductCostHistoryApi> TimeStampHistory;
            if (SelectedTimeStamp == "За год")
            {
                TimeStampHistory = ProductCostHistories.Where(c => c.IdProduct == product.Id && c.ChangeDate > (DateTime.Now - TimeSpan.FromDays(365))).ToList();
                PrepareChart(TimeStampHistory);
            }
            else if (SelectedTimeStamp == "За месяц")
            {
                TimeStampHistory = ProductCostHistories.Where(c => c.IdProduct == product.Id && c.ChangeDate > (DateTime.Now - TimeSpan.FromDays(30))).ToList();
                PrepareChart(TimeStampHistory);
            }
            else if (SelectedTimeStamp == "За все время")
            {
                TimeStampHistory = ProductCostHistories.Where(c => c.IdProduct == product.Id).ToList();
                PrepareChart(TimeStampHistory);
            }
        }

        public void PrepareChart(List<ProductCostHistoryApi> productCostHistories)
        {
            Retail.Clear();
            Wholesale.Clear();
            Dates.Clear();
            var result = productCostHistories.OrderBy(x => x.ChangeDate);
            foreach (ProductCostHistoryApi productCostHistory in result)
            {

                Retail.Add((double)productCostHistory.RetailPriceValue);
                Wholesale.Add((double)productCostHistory.WholesalePirceValue);
                Dates.Add((DateTime)productCostHistory.ChangeDate);
            }
            string[] dateTimes = new string[Dates.Count];
            for (int i = 0; i < Dates.Count; i++)
            {
                dateTimes[i] = Dates[i].Date.ToShortDateString().ToString();
            }
            Labels = dateTimes;
            BuildChart();
        }

        public void BuildChart()
        {

            SeriesCollection = new SeriesCollection
              {
                new LineSeries
                {
                    Title = "Розн. цена",
                     Values = Retail
                },
                new LineSeries
                {
                    Title = "Опт. цена",
                    Values = Wholesale
                },

              };
            
            YFormatter = value => value.ToString("C");
        }
    }
}
