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
    public class ProductViewModel : BaseViewModel
    {
        public List<string> ViewCountRows { get; set; }

        public string SelectedViewCountRows
        {
            get => selectedViewCountRows;
            set
            {
                selectedViewCountRows = value;
                paginationPageIndex = 0;
                Pagination();
            }
        }

        private List<ProductTypeApi> productTypeFilter;
        public List<ProductTypeApi> ProductTypeFilter
        {
            get => productTypeFilter;
            set
            {
                productTypeFilter = value;
                SignalChanged();
            }
        }

        private ProductTypeApi selectedProductTypeFilter;
        public ProductTypeApi SelectedProductTypeFilter
        {
            get => selectedProductTypeFilter;
            set
            {
                selectedProductTypeFilter = value;
                SignalChanged();
                Search();
            }
        }

        public string SearchCountRows
        {
            get => searchCountRows;
            set
            {
                searchCountRows = value;
                SignalChanged();
            }
        }
        private string pages;
        public string Pages
        {
            get => pages;
            set
            {
                pages = value;
                SignalChanged();
            }
        }
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }

        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Search();
            }
        }
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
        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        private List<ProductApi> FullProducts = new List<ProductApi>();
        public int rows = 0;
        public int CountPages = 0;
        List<ProductApi> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public ProductViewModel()
        {
            Units = new List<UnitApi>();
            ProductTypes = new List<ProductTypeApi>();
            Products = new List<ProductApi>();
            ProductTypeFilter = new List<ProductTypeApi>();
            GetList();

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "10", "50", "все" });
            selectedViewCountRows = ViewCountRows.First();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование", "Артикул" });
            selectedSearchType = SearchType.First();

            BackPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                if (paginationPageIndex > 0)
                    paginationPageIndex--;
                Pagination();
            });

            ForwardPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                int.TryParse(SelectedViewCountRows, out int rowsOnPage);
                if (rowsOnPage == 0)
                    return;
                int countPage = searchResult.Count() / rowsOnPage;
                CountPages = countPage;
                if (searchResult.Count() % rowsOnPage != 0)
                    countPage++;
                if (countPage > paginationPageIndex + 1)
                    paginationPageIndex++;
                Pagination();

            });

            AddProduct = new CustomCommand(() =>
            {
                AddProduct addProduct = new AddProduct();
                addProduct.ShowDialog();
                Update();
                InitPagination();
                Pagination();
            });
            EditProduct = new CustomCommand(() =>
            {
                if (SelectedProduct == null) return;
                AddProduct addProduct = new AddProduct(SelectedProduct);
                addProduct.ShowDialog();
                Update();
                InitPagination();
                Pagination();
            });
            DeleteProduct = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedProduct == null) return;
                    try
                    {
                        Delete(SelectedProduct);
                        Update();
                        SignalChanged("Products");
                        InitPagination();
                        Pagination();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    };
                }
                else return;

            });
        }

        private void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {FullProducts.Count}";
            paginationPageIndex = 0;
        }

        private void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Products = searchResult;
            }
            else
            {
                Products = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();

            }

            int.TryParse(SelectedViewCountRows, out rows);
            if (rows == 0)
                rows = FullProducts.Count;
            CountPages = (searchResult.Count() - 1) / rows;
            Pages = $"{paginationPageIndex + 1}/{CountPages + 1}";
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            if (SelectedProductTypeFilter == null)
                SelectedProductTypeFilter = ProductTypeFilter.Last();
            if (SelectedProductTypeFilter.Title == "Все типы")
            {
                if (SelectedSearchType == "Наименование")
                    searchResult = FullProducts
                        .Where(c => c.Title.ToLower().Contains(search)).ToList();
                else if (SelectedSearchType == "Артикул")
                    searchResult = FullProducts
                        .Where(c => c.Article.ToString().Contains(search)).ToList();
            }
            else
            {
                if (SelectedSearchType == "Наименование")
                    searchResult = FullProducts
                        .Where(c => c.Title.ToLower().Contains(search) && c.ProductType.Title.Contains(SelectedProductTypeFilter.Title)).ToList();
                else if (SelectedSearchType == "Артикул")
                    searchResult = FullProducts
                        .Where(c => c.Article.ToString().Contains(search) && c.ProductType.Title.Contains(SelectedProductTypeFilter.Title)).ToList();
            }
            InitPagination();
            Pagination();
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
            FullProducts = Products;
            foreach (ProductApi product in Products)
            {
                product.Unit = Units.First(s => s.Id == product.IdUnit);
                product.ProductType = ProductTypes.First(s => s.Id == product.IdProductType);
            } 
            ProductTypeFilter = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
            ProductTypeFilter.Add(new ProductTypeApi { Title = "Все типы" });
            SelectedProductTypeFilter = ProductTypeFilter.Last();

          
        
            InitPagination();
            Pagination();
        }

        private void Update()
        { 
            
            GetList();
            SignalChanged("Units");
            SignalChanged("ProductTypes");
            SignalChanged("Products");
            SelectedProductTypeFilter = ProductTypeFilter.Last();
        }
      
    }
}
