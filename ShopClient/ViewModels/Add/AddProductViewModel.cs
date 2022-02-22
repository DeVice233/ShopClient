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

namespace ShopClient.ViewModels.Add
{
   public class AddProductViewModel : BaseViewModel
    {
        public ProductApi AddProduct{ get; set; }

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


        public AddProductViewModel(ProductApi product)
        {
            Units = new List<UnitApi>();
            ProductTypes = new List<ProductTypeApi>();
         

            if (product == null)
            {
                AddProduct = new ProductApi { };
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
                    IdUnit = product.IdUnit,
                    IdProductType = product.IdProductType,
                    RetailPrice = product.RetailPrice,
                    WholesalePrice = product.WholesalePrice 
                };
           
                GetList(product);

                if(product.Image.Length == 0 || product.Image == null)
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
                            AddProduct.IdUnit = SelectedUnit.Id;
                        if (AddProduct.Id == 0)
                        {
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

        }

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
            
            ProductTypes = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            if(product == null)
            {
                SelectedUnit = Units.First();
                SelectedProductType = ProductTypes.First();
            }
            else
            {
                   SelectedUnit = Units.First(s => s.Id == product.IdUnit);
                   SelectedProductType = ProductTypes.First(s => s.Id == product.IdProductType);
            }
         
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
    }
}
