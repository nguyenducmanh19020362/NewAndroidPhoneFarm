using Code.Models;
using Code.ViewModels;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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

namespace Code.Views
{
    /// <summary>
    /// Interaction logic for QuanLyHeThongView.xaml
    /// </summary>
    public partial class QuanLyHeThongView : UserControl
    {
        public ObservableCollection<ThongTinThietBi> thietBis;
        public SeriesCollection SeriesCollectionYoutube { get; set; }
        public SeriesCollection SeriesCollectionFacebook { get; set; }

        private BackgroundWorker bk = new BackgroundWorker();
        DbSet<TaiKhoanGoogle> objectListGoogle = null;
        DbSet<TaiKhoanFacebook> objectListFacebook = null;

        private int stkYoutubeTC = 0;
        private int stkYoutubeTB = 0;
        private int stkFacebookTC = 0;
        private int stkFacebookTB = 0;
        public QuanLyHeThongView()
        {
            thietBis = new ObservableCollection<ThongTinThietBi>();
            InitializeComponent();
            SeriesCollectionYoutube = new SeriesCollection()
            {
                 new PieSeries
                {
                    Title = "Thành Công",
                    Values = new ChartValues<ObservableValue>{new ObservableValue(0)},
                    DataLabels = true,
                    FontSize = 20
                },
                 new PieSeries
                {
                    Title = "Thất Bại",
                    Values = new ChartValues<ObservableValue>{new ObservableValue(0)},
                    DataLabels = true,
                    FontSize = 20
                },
            };
            SeriesCollectionFacebook = new SeriesCollection()
            {
                 new PieSeries
                {
                    Title = "Thành Công",
                    Values = new ChartValues<ObservableValue>{new ObservableValue(0)},
                    DataLabels = true
                },
                 new PieSeries
                {
                    Title = "Thất Bại",
                    Values = new ChartValues<ObservableValue>{new ObservableValue(0)},
                    DataLabels = true
                },
            };
            DataContext = this;
            thietBis.Clear();
            ListIdThietBi.ItemsSource = thietBis;
            hethong.ItemsSource = thietBis;
            bk.DoWork += (o, e) =>
            {
                objectListGoogle = DataProvider.Ins.db.TaiKhoanGoogles;
                objectListFacebook = DataProvider.Ins.db.TaiKhoanFacebooks;
            };
            bk.RunWorkerCompleted += (o, e) =>
            {
                if (Header.Visibility == Visibility.Visible)
                {
                    LoadTable();
                }
                else
                {
                    LoadChart("Tat ca");
                }

            };
            bk.RunWorkerAsync();
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            bk.RunWorkerAsync();
        }
        private void LoadTable()
        {
            ObservableCollection<ThongTinThietBi> listItems = new ObservableCollection<ThongTinThietBi>();
            int TKFacebookTB = 0;
            int TKFacebookTC = 0;
            int TKYoutubeTB = 0;
            int TKYoutubeTC = 0;
            thietBis.Clear();
            listItems.Clear();
            foreach (var item in objectListFacebook)
            {
                var index = isExists(item.IDThietBi.Trim(), listItems);
                if (index != -1)
                {
                    if (item.TrangThai == 1)
                    {
                        listItems[index].SoTaiKhoanFacebookTC = listItems[index].SoTaiKhoanFacebookTC + 1;
                        TKFacebookTC++;
                    }
                    else
                    {
                        listItems[index].SoTaiKhoanFacebookTB = listItems[index].SoTaiKhoanFacebookTB + 1;
                        TKFacebookTB++;
                    }
                }
                else
                {
                    ThongTinThietBi newThietBi = new ThongTinThietBi();
                    newThietBi.SoThuTu = listItems.Count;
                    newThietBi.MaThietBi = item.IDThietBi;
                    if (item.TrangThai == 1)
                    {
                        newThietBi.SoTaiKhoanFacebookTC = 1;
                        TKFacebookTC++;
                    }
                    else
                    {
                        newThietBi.SoTaiKhoanFacebookTB = 1;
                        TKFacebookTB++;
                    }
                    newThietBi.SoTaiKhoanYoutubeTC = 0;
                    newThietBi.SoTaiKhoanYoutubeTB = 0;
                    listItems.Add(newThietBi);
                }
            }
            foreach (var item in objectListGoogle)
            {
                var index = isExists(item.IDThietBi.Trim(), listItems);
                if (index != -1)
                {
                    if (item.TrangThai == 1)
                    {
                        listItems[index].SoTaiKhoanYoutubeTC = listItems[index].SoTaiKhoanYoutubeTC + 1;
                        TKYoutubeTC++;
                    }
                    else
                    {
                        listItems[index].SoTaiKhoanYoutubeTB = listItems[index].SoTaiKhoanYoutubeTB + 1;
                        TKYoutubeTB++;
                    }
                }
                else
                {
                    ThongTinThietBi newThietBi = new ThongTinThietBi();
                    newThietBi.SoThuTu = listItems.Count;
                    newThietBi.MaThietBi = item.IDThietBi.Trim();
                    if (item.TrangThai == 1)
                    {
                        newThietBi.SoTaiKhoanYoutubeTC = 1;
                        TKYoutubeTC++;
                    }
                    else
                    {
                        newThietBi.SoTaiKhoanYoutubeTB = 1;
                        TKYoutubeTB++;
                    }
                    newThietBi.SoTaiKhoanFacebookTC = 0;
                    newThietBi.SoTaiKhoanFacebookTB = 0;
                    listItems.Add(newThietBi);
                }
            }

            var allItems = new ThongTinThietBi();
            allItems.MaThietBi = "Tat ca";
            allItems.SoTaiKhoanYoutubeTB = TKYoutubeTB;
            allItems.SoTaiKhoanYoutubeTC = TKYoutubeTC;
            allItems.SoTaiKhoanFacebookTC = TKFacebookTC;
            allItems.SoTaiKhoanFacebookTB = TKFacebookTB;
            thietBis.Add(allItems);
            for (int i = 0; i < listItems.Count; i++)
            {
                thietBis.Add(listItems[i]);
            }
        }

        private void LoadChart(string idSelected)
        {
            stkYoutubeTB = tongTK(1, idSelected);
            stkYoutubeTC = tongTK(0, idSelected);
            stkFacebookTC = tongTK(2, idSelected);
            stkFacebookTB = tongTK(3, idSelected);
            if (stkYoutubeTB == 0 && stkYoutubeTC == 0)
            {
                CircleYoutube.Visibility = Visibility.Collapsed;
            }
            else
            {
                CircleFacebook.Visibility = Visibility.Visible;
                SeriesCollectionYoutube[0].Values[0] = new ObservableValue(stkYoutubeTC);
                SeriesCollectionYoutube[1].Values[0] = new ObservableValue(stkYoutubeTB);
            }
            if (stkFacebookTB == 0 && stkFacebookTC == 0)
            {
                CircleFacebook.Visibility = Visibility.Collapsed;
            }
            else
            {
                CircleFacebook.Visibility = Visibility.Visible;
                SeriesCollectionFacebook[0].Values[0] = new ObservableValue(stkFacebookTC);
                SeriesCollectionFacebook[1].Values[0] = new ObservableValue(stkFacebookTB);
            }

        }

        private int isExists(string idThietBi, ObservableCollection<ThongTinThietBi> listItems)
        {
            for (int i = 0; i < listItems.Count; i++)
            {
                if (listItems[i].MaThietBi == idThietBi)
                {
                    return i;
                }
            }
            return -1;
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            btnTable.Background = new SolidColorBrush(Colors.AliceBlue);
            IconTable.Foreground = new SolidColorBrush(Colors.Black);
            TextTable.Foreground = new SolidColorBrush(Colors.Black);
            btnChar.Background = new SolidColorBrush();
            IconChart.Foreground = new SolidColorBrush(Colors.White);
            TextChart.Foreground = new SolidColorBrush(Colors.White);
            Header.Visibility = Visibility.Visible;
            hethong.Visibility = Visibility.Visible;
            LoadTable();
            Chart.Visibility = Visibility.Collapsed;

        }
        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            btnTable.Background = new SolidColorBrush();
            IconTable.Foreground = new SolidColorBrush(Colors.White);
            TextTable.Foreground = new SolidColorBrush(Colors.White);
            btnChar.Background = new SolidColorBrush(Colors.AliceBlue);
            IconChart.Foreground = new SolidColorBrush(Colors.Black);
            TextChart.Foreground = new SolidColorBrush(Colors.Black);
            Header.Visibility = Visibility.Collapsed;
            hethong.Visibility = Visibility.Collapsed;
            LoadChart("Tat ca");
            Chart.Visibility = Visibility.Visible;
        }

        private int tongTK(int state, string idThietBi)
        {
            int tong = 0;
            for (int i = 0; i < thietBis.Count; i++)
            {
                switch (state)
                {
                    case 0:
                        if (idThietBi == thietBis[i].MaThietBi)
                        {
                            tong = tong + thietBis[i].SoTaiKhoanYoutubeTC;
                        }
                        break;
                    case 1:
                        if (idThietBi == thietBis[i].MaThietBi)
                        {
                            tong = tong + thietBis[i].SoTaiKhoanYoutubeTB;
                        }
                        break;
                    case 2:
                        if (idThietBi == thietBis[i].MaThietBi)
                        {
                            tong = tong + thietBis[i].SoTaiKhoanFacebookTC;
                        }
                        break;
                    case 3:
                        if (idThietBi == thietBis[i].MaThietBi)
                        {
                            tong = tong + thietBis[i].SoTaiKhoanFacebookTB;
                        }
                        break;
                }
            }
            return tong;
        }

        private void Selected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var thietBiSelected = (ThongTinThietBi)ListIdThietBi.SelectedItem;
            if (thietBiSelected != null)
            {
                LoadChart(thietBiSelected.MaThietBi);
            }
        }

        private void PieChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {

        }
    }
}