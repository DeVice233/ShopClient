using ModelsApi;
using ShopClient.Core;
using ShopClient.Views.Add;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels
{
   public class ProductTypeViewModel : BaseViewModel
    {
        private List<ProductTypeApi> productTypes;
        public List<ProductTypeApi> ProductTypes
        {
            get => productTypes;
            set
            {
                SignalChanged();
                Set(ref productTypes, value);
            }
        }

        private ProductTypeApi selectedProductType = new ProductTypeApi { };
        public ProductTypeApi SelectedProductType
        {
            get => selectedProductType;
            set
            {
                selectedProductType = value;
                SignalChanged();
            }
        }

        public CustomCommand AddProductType { get; set; }
        public CustomCommand EditProductType { get; set; }
        public CustomCommand DeleteProductType { get; set; }

        public ProductTypeViewModel()
        {
            ProductTypes = new List<ProductTypeApi>();
            GetList();


          
            AddProductType = new CustomCommand(() =>
            {
                AddProductType addProductType = new AddProductType();
                addProductType.ShowDialog();
                GetList();
                SignalChanged("ProductTypes");
            });
            EditProductType = new CustomCommand(() =>
            {
                if (SelectedProductType == null) return;
                AddProductType addProductType = new AddProductType(SelectedProductType);
                addProductType.ShowDialog();
                GetList();
                SignalChanged("ProductTypes");
            }); 
            DeleteProductType = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedProductType == null) return;
                    try
                    {
                        Delete(SelectedProductType);
                        GetList();
                        SignalChanged("ProductTypes");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    };
                }
                else return;

            });
        }

        private async Task Delete(ProductTypeApi productType)
        {
            var res = await Api.DeleteAsync<ProductTypeApi>(productType, "ProductType");  
            GetList();
        }
        private async Task GetList()
        {
            ProductTypes = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
       
        }
       
    }
}
