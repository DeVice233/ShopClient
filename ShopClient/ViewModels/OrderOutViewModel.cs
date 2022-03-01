﻿using ModelsApi;
using ShopClient.Core;
using ShopClient.Helper;
using ShopClient.Views.Add;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels
{
    public class OrderOutViewModel : BaseViewModel
    { 
        private object _clickedTreeElement;
        public object ClickedTreeElement
        {
            get => _clickedTreeElement;

            set
            {
                Set(ref _clickedTreeElement, value);
                UpdateListWithTreeView();
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
        private ObservableCollection<ProductTypeTreeView> productTypeTreeViews = new ObservableCollection<ProductTypeTreeView>();
        public ObservableCollection<ProductTypeTreeView> ProductTypeTreeViews
        {
            get => productTypeTreeViews;
            set
            {
                Set(ref productTypeTreeViews, value);
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

        public List<SaleTypeApi> saleTypes;
        public List<SaleTypeApi> SaleTypes {
            get => saleTypes;
            set
            {
                Set(ref saleTypes, value);
                SignalChanged();
            }
        }

        private ObservableCollection<ProductOrderOutApi> productOrderOuts;
        public ObservableCollection<ProductOrderOutApi> ProductOrderOuts
        {
            get => productOrderOuts;
            set
            {
                Set(ref productOrderOuts, value);
                SignalChanged();
            }
        }
        private ProductOrderOutApi selectedProductOrderOuts;
        public ProductOrderOutApi SelectedProductOrderOuts
        {
            get => selectedProductOrderOuts;
            set
            {
                selectedProductOrderOuts = value;
                SignalChanged();
            }
        }
        private SaleTypeApi selectedSaleType;
        public SaleTypeApi SelectedSaleType
        {
            get => selectedSaleType;
            set
            {
                selectedSaleType = value;
                SignalChanged();
            }
        }

        private List<ProductOrderInApi> FullProductOrderIns = new List<ProductOrderInApi>();
        private List<ProductApi> FullProducts = new List<ProductApi>();
        private List<ActionTypeApi> ActionTypes = new List<ActionTypeApi>();
        private List<FabricatorApi> Fabricators = new List<FabricatorApi>();
        List<ProductApi> searchResult;

        public CustomCommand AddProduct { get; set; }

        public OrderOutViewModel()
        {
            Units = new List<UnitApi>();
            ProductTypes = new List<ProductTypeApi>();
            Products = new List<ProductApi>();
            ProductTypeFilter = new List<ProductTypeApi>();
            GetList();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование", "Артикул" });
            selectedSearchType = SearchType.First();

            AddProduct = new CustomCommand(() =>
            {
                if (SelectedSaleType == null)
                {
                    MessageBox.Show("Выберите тип продажи!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (SelectedProduct == null) return;
                ProductOrderOutApi productOrderOut = new ProductOrderOutApi { Product = SelectedProduct };
                AddOrderOut addOrderOut = new AddOrderOut(productOrderOut, SelectedSaleType);
                addOrderOut.ShowDialog();
                //if (productOrderIn.Count <= 0 || productOrderIn.Count == null)
                //{
                //    Update();
                //    return;
                //}
                //GetProperties(productOrderIn);
                //ProductOrderIns.Add(productOrderIn);
                Update();
            });

            UpdateList();
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
            UpdateList();
        }

        private async Task GetList()
        {
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            SaleTypes = await Api.GetListAsync<List<SaleTypeApi>>("SaleType");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            Fabricators = await Api.GetListAsync<List<FabricatorApi>>("Fabricator");
            ProductTypes = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
            Products = await Api.GetListAsync<List<ProductApi>>("Product");
            FullProductOrderIns = await Api.GetListAsync<List<ProductOrderInApi>>("ProductOrderIn");
            FullProducts = Products;
            foreach (ProductApi product in Products)
            {
                product.Count = FullProductOrderIns.Where(s => s.IdProduct == product.Id).Select(s => s.Remains).Sum();
                product.Unit = Units.First(s => s.Id == product.IdUnit);
                product.ProductType = ProductTypes.First(s => s.Id == product.IdProductType);
                product.Fabricator = Fabricators.First(s => s.Id == product.IdFabricator);
            }
            ProductTypeFilter = await Api.GetListAsync<List<ProductTypeApi>>("ProductType");
            ProductTypeFilter.Add(new ProductTypeApi { Title = "Все типы" });
            SelectedProductTypeFilter = ProductTypeFilter.Last();
            PrepareTreeView();
        }

        private void PrepareTreeView()
        {
            ProductTypeTreeViews.Clear();
            foreach (ProductTypeApi productType in ProductTypes)
            {
                var productTypeTreeView = new ProductTypeTreeView { Title = productType.Title };
                ProductTypeTreeViews.Add(productTypeTreeView);
                var ProductByProductType = Products.Where(s => s.IdProductType == productType.Id);
                foreach (FabricatorApi fab in Fabricators)
                {
                    var a = 0;
                    foreach (ProductApi prod in ProductByProductType)
                    {
                        if (prod.IdFabricator == fab.Id)
                        {
                            var fabricatorTreeView = new FabricatorTreeView { Title = fab.Title };
                            productTypeTreeView.Fabricators.Add(fabricatorTreeView);
                            a = 1;
                        }
                        if (a == 1) break;
                    }

                }
            }
            SignalChanged("ProductTypeTreeViews");
        }
        private void UpdateList()
        {
            Products = searchResult;
        }
        private void UpdateListWithTreeView()
        {
            if (ClickedTreeElement == null) Products = FullProducts;
            if (ClickedTreeElement is ProductTypeTreeView)
            {
                var clickedEl = (ProductTypeTreeView)ClickedTreeElement;
                Products = FullProducts.Where(s => s.ProductType.Title == clickedEl.Title).ToList();
            }
            if (ClickedTreeElement is FabricatorTreeView)
            {
                var clickedEl = (FabricatorTreeView)ClickedTreeElement;
                Products = FullProducts.Where(s => s.ProductType.Title == clickedEl.Parent && s.Fabricator.Title == clickedEl.Title).ToList();
            }

        }
        private void Update()
        {
            GetList();
            SignalChanged("Units");
            SignalChanged("ProductTypes");
            SignalChanged("Products");
            SignalChanged("ProductOrderIns");
            SignalChanged("LegalClients");
            SelectedProductTypeFilter = ProductTypeFilter.Last();
        }
    }
}