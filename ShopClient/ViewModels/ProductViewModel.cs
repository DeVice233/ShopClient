using ModelsApi;
using ShopClient.Core;
using ShopClient.Views.Add;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopClient.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private List<ProductApi> products;
        public List<ProductApi> Products
        {
            get => products;
            set
            {
                Set(ref products, value);
                SignalChanged();
            }
        }
        private List<UnitApi> units;
        public List<UnitApi> Units
        {
            get => units;
            set
            {
                Set(ref units, value);
                SignalChanged();

            }
        }
        private List<ProductTypeApi> productTypes;
        public List<ProductTypeApi> ProductTypes
        {
            get => productTypes;
            set
            {
                Set(ref productTypes, value);
                SignalChanged();

            }
        }
        private ProductApi selectedProduct = new ProductApi { };
        public ProductApi SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                SignalChanged();
            }
        }

        public CustomCommand AddProduct{ get; set; }
        public CustomCommand EditProduct { get; set; }
        public CustomCommand DeleteProduct { get; set; }

        public ProductViewModel()
        {
            Units = new List<UnitApi>();
            ProductTypes = new List<ProductTypeApi>();
            Products = new List<ProductApi>();
            GetList();



            AddProduct = new CustomCommand(() =>
            {
                AddProduct addProduct = new AddProduct();
                addProduct.ShowDialog();
                Update();
            });
            EditProduct = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;
                AddProduct addProduct = new AddProduct(SelectedProduct);
                addProduct.ShowDialog();
                Update();
            });


        }

        private async Task Delete(ProductApi product)
        {
            var res = await Api.DeleteAsync<ProductApi>(product, "Product");
            GetList();
        }
        private async Task GetList()
        { 
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            ProductTypes = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
            Products = await Api.GetListAsync<List<ProductApi>>("Product");
           foreach(ProductApi product in Products)
            {
                product.Unit = Units.First(s => s.Id == product.IdUnit);
                product.ProductType = ProductTypes.First(s => s.Id == product.IdProductType);
            }
        }

        private void Update()
        { 
            
            GetList();
            SignalChanged("Units");
            SignalChanged("ProductTypes");
            SignalChanged("Products");
        }
    }
}
