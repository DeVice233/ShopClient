using ModelsApi;
using ShopClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels.Add
{
   public class AddOrderOutViewModel : BaseViewModel
    {
        private string productTitle;
        public string ProductTitle
        {
            get => productTitle;
            set
            {
                productTitle = value;
                SignalChanged();
            }
        }

        private int count = 1;
        public int Count
        {
            get => count;
            set
            {
                if (value != count)
                {
                    count = value;
                    SignalChanged();
                    TotalCalculate();
                }
            }
        }

        private decimal? price;
        public decimal? Price
        {
            get => price;
            set
            {
                if (value != price)
                {
                    price = value;
                    SignalChanged();
                    TotalCalculate();
                }

            }
        }
        private decimal? discount;
        public decimal? Discount
        {
            get => discount;
            set
            {
                if (value != discount)
                {
                    discount = value;
                    SignalChanged();
                    TotalCalculate();
                }

            }
        }


        private decimal? total;
        public decimal? Total
        {
            get => total;
            set
            {
                total = value;
                SignalChanged();
            }
        }

        private string pickedSaleType;
        public string PickedSaleType
        {
            get => pickedSaleType;
            set
            {
                pickedSaleType = value;
                SignalChanged();
            }
        }

        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        public CustomCommand AddOne { get; set; }

        decimal? buyPrice = 0;
        List<ProductOrderInApi> productOrderIns = new List<ProductOrderInApi>();
        List<ProductOrderInApi> ThisProductOrderIns = new List<ProductOrderInApi>();
        List<OrderApi> Orders = new List<OrderApi>();
        List<ProductApi> Products = new List<ProductApi>();
        List<UnitApi> Units = new List<UnitApi>();
        List<FabricatorApi> Fabricators = new List<FabricatorApi>();

        public AddOrderOutViewModel(ProductOrderOutApi productOrderOut, SaleTypeApi saleType)
        {
            Discount = 0;
            Price = 0;
            Total = 0;

            GetList(productOrderOut, saleType);

            Save = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Внести в документ?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (Count <= 0)
                    {
                        MessageBox.Show("Необходимо выбрать количество!");
                        return;
                    }
                    if (Count > productOrderOut.Product.Count)
                    {
                        MessageBox.Show($"Количество не соотвествует остаткам на складе ({productOrderOut.Product.Count})!");
                        return;
                    }
                    try
                    {

                        //productOrderIn.Price = Price;
                        //productOrderIn.Remains = Count;
                        //productOrderIn.Count = Count;
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


            AddOne = new CustomCommand(() =>
            { 
                if (Count > productOrderOut.Product.Count)
                {
                    return;
                }
                Count++;
              
            });
        }

        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }

        private async Task GetList(ProductOrderOutApi productOrderOut, SaleTypeApi saleType)
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            productOrderIns = await Api.GetListAsync<List<ProductOrderInApi>>("ProductOrderIn");
            Fabricators = await Api.GetListAsync<List<FabricatorApi>>("Fabricator");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            Products = await Api.GetListAsync<List<ProductApi>>("Product");

            productOrderOut.Product = Products.First(s => s.Id == productOrderOut.Product.Id);
            productOrderOut.Product.Unit = Units.First(s => s.Id == productOrderOut.Product.IdUnit);
            productOrderOut.Product.Fabricator = Fabricators.First(s => s.Id == productOrderOut.Product.IdFabricator);
            ThisProductOrderIns = productOrderIns.Where(s => s.IdProduct == productOrderOut.Product.Id).ToList();
            productOrderOut.Product.Count = ThisProductOrderIns.Select(s => s.Remains).Sum();
            //List<ProductOrderInApi> thisProductOrderin = productOrderIns.Where(c => c.IdProduct == productOrderIn.IdProduct).ToList();

            //if (thisProductOrderin.Count != 0)
            //{
            //    var result = thisProductOrderin.OrderBy(x => x.IdOrder);
            //    ProductOrderInApi productOrderInRes = result.Last();
            //    buyPrice = productOrderInRes.Price;
            //}
            if (saleType.Title == "Оптовая")
            {
                Price = productOrderOut.Product.WholesalePrice;
            }
            else if (saleType.Title == "Розничная")
            {
                Price = productOrderOut.Product.RetailPrice;
            }
            PickedSaleType = $"Цена: ({saleType.Title})";
            ProductTitle = $"{productOrderOut.Product.Fabricator.Title} " + productOrderOut.Product.Title + $" ({productOrderOut.Product.Unit.Title})";
        }
        private void TotalCalculate()
        {
            Total = Count * Price - Discount;
            SignalChanged("Title");
        }

        private async Task Add(ProductOrderInApi productOrderIn)
        {
            var id = await Api.PostAsync<ProductOrderInApi>(productOrderIn, "ProductOrderIn");
        }


    }
}
